using System.Text.Json.Serialization;

namespace WindowsHelloAuth.Models
{
    // FIDO2 Credential Creation
    public class CredentialCreateOptions
    {
        [JsonPropertyName("rp")]
        public PublicKeyCredentialRpEntity Rp { get; set; } = new();

        [JsonPropertyName("user")]
        public Fido2User User { get; set; } = new();

        [JsonPropertyName("challenge")]
        public string Challenge { get; set; } = string.Empty;

        [JsonPropertyName("pubKeyCredParams")]
        public List<PubKeyCredParam> PubKeyCredParams { get; set; } = new();

        [JsonPropertyName("timeout")]
        public int? Timeout { get; set; }

        [JsonPropertyName("excludeCredentials")]
        public List<PublicKeyCredentialDescriptor>? ExcludeCredentials { get; set; }

        [JsonPropertyName("authenticatorSelection")]
        public AuthenticatorSelection? AuthenticatorSelection { get; set; }

        [JsonPropertyName("attestation")]
        public string? Attestation { get; set; }
    }

    public class PublicKeyCredentialRpEntity
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class Fido2User
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; } = string.Empty;
    }

    public class PubKeyCredParam
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "public-key";

        [JsonPropertyName("alg")]
        public int Alg { get; set; }
    }

    public class AuthenticatorSelection
    {
        [JsonPropertyName("authenticatorAttachment")]
        public string? AuthenticatorAttachment { get; set; }

        [JsonPropertyName("requireResidentKey")]
        public bool RequireResidentKey { get; set; }

        [JsonPropertyName("userVerification")]
        public string UserVerification { get; set; } = "preferred";
    }

    public class PublicKeyCredentialDescriptor
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = "public-key";

        [JsonPropertyName("transports")]
        public List<string>? Transports { get; set; }
    }

    // FIDO2 Assertion
    public class AssertionOptions
    {
        [JsonPropertyName("challenge")]
        public string Challenge { get; set; } = string.Empty;

        [JsonPropertyName("timeout")]
        public int? Timeout { get; set; }

        [JsonPropertyName("rpId")]
        public string? RpId { get; set; }

        [JsonPropertyName("allowCredentials")]
        public List<PublicKeyCredentialDescriptor>? AllowCredentials { get; set; }

        [JsonPropertyName("userVerification")]
        public string UserVerification { get; set; } = "preferred";
    }

    // Client Response Models
    public class AuthenticatorAttestationRawResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("rawId")]
        public string RawId { get; set; } = string.Empty;

        [JsonPropertyName("response")]
        public AttestationResponse Response { get; set; } = new();

        [JsonPropertyName("type")]
        public string Type { get; set; } = "public-key";
    }

    public class AttestationResponse
    {
        [JsonPropertyName("clientDataJSON")]
        public string ClientDataJSON { get; set; } = string.Empty;

        [JsonPropertyName("attestationObject")]
        public string AttestationObject { get; set; } = string.Empty;
    }

    public class AuthenticatorAssertionRawResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("rawId")]
        public string RawId { get; set; } = string.Empty;

        [JsonPropertyName("response")]
        public AssertionResponse Response { get; set; } = new();

        [JsonPropertyName("type")]
        public string Type { get; set; } = "public-key";
    }

    public class AssertionResponse
    {
        [JsonPropertyName("clientDataJSON")]
        public string ClientDataJSON { get; set; } = string.Empty;

        [JsonPropertyName("authenticatorData")]
        public string AuthenticatorData { get; set; } = string.Empty;

        [JsonPropertyName("signature")]
        public string Signature { get; set; } = string.Empty;

        [JsonPropertyName("userHandle")]
        public string? UserHandle { get; set; }
    }
} 