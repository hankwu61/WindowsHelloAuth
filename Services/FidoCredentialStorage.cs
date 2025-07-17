using Microsoft.EntityFrameworkCore;
using WindowsHelloAuth.Models;

namespace WindowsHelloAuth.Services
{
    public class FidoCredentialStorage : IFidoCredentialStorage
    {
        private readonly ApplicationDbContext _context;

        public FidoCredentialStorage(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FidoCredential>> GetCredentialsByUserIdAsync(string userId)
        {
            return await _context.FidoCredentials
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<FidoCredential?> GetCredentialByIdAsync(byte[] credentialId)
        {
            return await _context.FidoCredentials
                .FirstOrDefaultAsync(c => c.CredentialId.SequenceEqual(credentialId));
        }

        public async Task AddCredentialAsync(FidoCredential credential)
        {
            await _context.FidoCredentials.AddAsync(credential);
            await _context.SaveChangesAsync();
        }

        public async Task<List<FidoCredential>> GetCredentialsForUserAsync(string userId)
        {
            return await _context.FidoCredentials
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task UpdateCredentialCounterAsync(byte[] credentialId, uint counter)
        {
            var credential = await GetCredentialByIdAsync(credentialId);
            if (credential != null)
            {
                credential.SignatureCounter = counter;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> DeleteCredentialAsync(byte[] credentialId, string userId)
        {
            var credential = await _context.FidoCredentials
                .FirstOrDefaultAsync(c => c.CredentialId.SequenceEqual(credentialId) && c.UserId == userId);
                
            if (credential == null)
            {
                return false;
            }
            
            _context.FidoCredentials.Remove(credential);
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 