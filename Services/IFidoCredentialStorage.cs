using WindowsHelloAuth.Models;

namespace WindowsHelloAuth.Services
{
    public interface IFidoCredentialStorage
    {
        Task<List<FidoCredential>> GetCredentialsByUserIdAsync(string userId);
        Task<FidoCredential?> GetCredentialByIdAsync(byte[] credentialId);
        Task AddCredentialAsync(FidoCredential credential);
        Task<List<FidoCredential>> GetCredentialsForUserAsync(string userId);
        Task UpdateCredentialCounterAsync(byte[] credentialId, uint counter);
        Task<bool> DeleteCredentialAsync(byte[] credentialId, string userId);
    }
} 