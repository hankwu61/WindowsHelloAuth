using System.Text;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WindowsHelloAuth.Models;
using WindowsHelloAuth.Services;

namespace WindowsHelloAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Fido2Controller : ControllerBase
    {
        private readonly IFido2 _fido2;
        private readonly IFidoCredentialStorage _storage;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public Fido2Controller(
            IFido2 fido2,
            IFidoCredentialStorage storage,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _fido2 = fido2;
            _storage = storage;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        [Route("makeCredentialOptions")]
        [Authorize]
        public async Task<IActionResult> MakeCredentialOptions([FromBody] WindowsHelloRegistrationViewModel request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null) return NotFound("User not found");

                // Get existing credentials to exclude
                var existingCredentials = await _storage.GetCredentialsByUserIdAsync(user.Id);
                var excludeCredentials = existingCredentials
                    .Select(c => new Fido2NetLib.Objects.PublicKeyCredentialDescriptor(c.CredentialId))
                    .ToList();

                // Create credential creation options
                var authenticatorSelection = new Fido2NetLib.AuthenticatorSelection
                {
                    AuthenticatorAttachment = AuthenticatorAttachment.Platform, // For Windows Hello
                    RequireResidentKey = true,
                    UserVerification = UserVerificationRequirement.Required
                };

                // Create a challenge
                var challenge = new byte[32];
                using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
                {
                    rng.GetBytes(challenge);
                }

                // Handle potential null DisplayName
                string displayName = user.UserName ?? user.Email ?? "User";
                
                // Get the current domain from the request
                string domain = HttpContext.Request.Host.Host;

                var options = new Fido2NetLib.CredentialCreateOptions
                {
                    Rp = new Fido2NetLib.PublicKeyCredentialRpEntity(domain, "Windows Hello Auth System", null),
                    User = new Fido2NetLib.Fido2User
                    {
                        Id = Encoding.UTF8.GetBytes(user.Id),
                        Name = user.Email ?? string.Empty,
                        DisplayName = displayName
                    },
                    Challenge = challenge,
                    PubKeyCredParams = new List<Fido2NetLib.PubKeyCredParam>
                    {
                        new Fido2NetLib.PubKeyCredParam(COSE.Algorithm.ES256),
                        new Fido2NetLib.PubKeyCredParam(COSE.Algorithm.RS256)
                    },
                    ExcludeCredentials = excludeCredentials,
                    AuthenticatorSelection = authenticatorSelection,
                    Attestation = AttestationConveyancePreference.Direct,
                    Timeout = 60000
                };

                // Store the options and device name in session
                HttpContext.Session.SetString("fido2.attestationOptions", System.Text.Json.JsonSerializer.Serialize(options));
                HttpContext.Session.SetString("fido2.deviceName", request.DeviceName);

                // Return the options to the client
                return Ok(options);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("makeCredential")]
        [Authorize]
        public async Task<IActionResult> MakeCredential([FromBody] Fido2NetLib.AuthenticatorAttestationRawResponse attestationResponse)
        {
            try
            {
                var optionsJson = HttpContext.Session.GetString("fido2.attestationOptions");
                if (string.IsNullOrEmpty(optionsJson))
                    return BadRequest("No attestation options found in session");

                var deviceName = HttpContext.Session.GetString("fido2.deviceName") ?? "Windows Hello Device";

                var options = System.Text.Json.JsonSerializer.Deserialize<Fido2NetLib.CredentialCreateOptions>(optionsJson);
                if (options == null)
                    return BadRequest("Invalid credential options");
                
                // Make sure User.Id is not null
                if (options.User?.Id == null)
                {
                    return BadRequest("User ID is missing from the credential options");
                }
                
                // Verify and make the credentials
                var success = await _fido2.MakeNewCredentialAsync(
                    attestationResponse,
                    options,
                    (credentialId, user) => Task.FromResult(true));

                // Store the credential
                var userId = Encoding.UTF8.GetString(options.User?.Id ?? Array.Empty<byte>());
                if (string.IsNullOrEmpty(userId))
                    return BadRequest("User ID is missing");

                await _storage.AddCredentialAsync(new FidoCredential
                {
                    UserId = userId,
                    CredentialId = success.Result.CredentialId,
                    PublicKey = success.Result.PublicKey,
                    UserHandle = success.Result.User.Id,
                    SignatureCounter = success.Result.Counter,
                    CredType = success.Result.CredType,
                    RegDate = DateTime.UtcNow,
                    AaGuid = success.Result.Aaguid.ToString(),
                    CredentialName = deviceName
                });

                return Ok(new { status = "ok" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("assertionOptions")]
        public async Task<IActionResult> AssertionOptions([FromBody] WindowsHelloLoginViewModel request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null) return NotFound("User not found");

                // Get credentials to allow for login
                var credentials = await _storage.GetCredentialsByUserIdAsync(user.Id);
                if (!credentials.Any())
                    return BadRequest("No Windows Hello credentials registered for this user");

                var allowedCredentials = credentials
                    .Select(c => new Fido2NetLib.Objects.PublicKeyCredentialDescriptor(c.CredentialId))
                    .ToList();

                // Create a challenge
                var challenge = new byte[32];
                using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
                {
                    rng.GetBytes(challenge);
                }

                // Create assertion options
                var options = new Fido2NetLib.AssertionOptions
                {
                    Challenge = challenge,
                    AllowCredentials = allowedCredentials,
                    UserVerification = UserVerificationRequirement.Required,
                    Timeout = 60000,
                    RpId = HttpContext.Request.Host.Host
                };

                // Store options in session
                HttpContext.Session.SetString("fido2.assertionOptions", System.Text.Json.JsonSerializer.Serialize(options));
                HttpContext.Session.SetString("fido2.userId", user.Id);

                return Ok(options);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("makeAssertion")]
        public async Task<IActionResult> MakeAssertion([FromBody] Fido2NetLib.AuthenticatorAssertionRawResponse assertionResponse)
        {
            try
            {
                var optionsJson = HttpContext.Session.GetString("fido2.assertionOptions");
                if (string.IsNullOrEmpty(optionsJson))
                    return BadRequest("No assertion options in session");

                var userId = HttpContext.Session.GetString("fido2.userId");
                if (string.IsNullOrEmpty(userId))
                    return BadRequest("No user ID in session");

                var options = System.Text.Json.JsonSerializer.Deserialize<Fido2NetLib.AssertionOptions>(optionsJson);
                if (options == null)
                    return BadRequest("Invalid assertion options");

                // Find credentials that match the assertion
                var credentials = await _storage.GetCredentialsByUserIdAsync(userId);
                
                // Try to match the credential ID
                FidoCredential? credential = null;
                try
                {
                    // Convert assertion ID to byte array regardless of type
                    byte[] idBytes;
                    if (assertionResponse.Id is byte[] byteArray)
                    {
                        idBytes = byteArray;
                    }
                    else
                    {
                        var idString = assertionResponse.Id.ToString();
                        idBytes = !string.IsNullOrEmpty(idString) 
                            ? Convert.FromBase64String(idString) 
                            : Array.Empty<byte>();
                    }
                    
                    // Try to find by direct byte[] comparison
                    foreach (var cred in credentials)
                    {
                        if (cred.CredentialId.Length == idBytes.Length)
                        {
                            bool isMatch = true;
                            for (int i = 0; i < idBytes.Length; i++)
                            {
                                if (cred.CredentialId[i] != idBytes[i])
                                {
                                    isMatch = false;
                                    break;
                                }
                            }
                            
                            if (isMatch)
                            {
                                credential = cred;
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to decode credential ID: {ex.Message}");
                }
                
                if (credential == null)
                    return BadRequest("Credential not found");

                // FIX 3: Fix the IsUserHandleOwnerOfCredentialIdAsync delegate
                Fido2NetLib.IsUserHandleOwnerOfCredentialIdAsync callback =
                    (credentialId, userHandle) => Task.FromResult(true);
                
                var result = await _fido2.MakeAssertionAsync(
                    assertionResponse, 
                    options, 
                    credential.PublicKey,
                    credential.SignatureCounter,
                    callback);

                // Update the counter
                await _storage.UpdateCredentialCounterAsync(credential.CredentialId, result.Counter);

                // Sign the user in
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return BadRequest("User not found");

                await _signInManager.SignInAsync(user, isPersistent: true);

                return Ok(new { status = "ok" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        // Helper method that matches the IsUserHandleOwnerOfCredentialIdAsync delegate signature
        private Task<bool> CheckUserCredentialOwnership(IsUserHandleOwnerOfCredentialIdParams args)
        {
            // In a real application, we would verify that the user handle owns the credential ID
            // For simplicity in this example, we're just returning true
            return Task.FromResult(true);
        }

        // Implementation with the correct signature for Fido2 3.0.1
        private Task<bool> IsUserHandleOwnerOfCredentialId(byte[] credentialId, byte[] userHandle)
        {
            // In a real application, we would verify that the user handle owns the credential ID
            // For simplicity in this example, we're just returning true
            return Task.FromResult(true);
        }

        [HttpDelete]
        [Route("deleteCredential")]
        [Authorize]
        public async Task<IActionResult> DeleteCredential([FromQuery] string credentialId)
        {
            try
            {
                // Get current user
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }
                
                // Convert base64url credentialId to byte array
                byte[] credentialIdBytes;
                try
                {
                    credentialIdBytes = Convert.FromBase64String(credentialId);
                }
                catch (Exception)
                {
                    return BadRequest("Invalid credential ID format");
                }
                
                // Delete the credential
                bool result = await _storage.DeleteCredentialAsync(credentialIdBytes, userId);
                
                if (!result)
                {
                    return NotFound("Credential not found or not authorized to delete");
                }
                
                return Ok(new { status = "ok", message = "Credential deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
} 