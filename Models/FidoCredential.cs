using System;
using System.ComponentModel.DataAnnotations;

namespace WindowsHelloAuth.Models
{
    public class FidoCredential
    {
        [Key]
        public int Id { get; set; }
        
        public string UserId { get; set; } = string.Empty;
        
        public virtual ApplicationUser User { get; set; } = null!;
        
        [Required]
        public byte[] CredentialId { get; set; } = Array.Empty<byte>();
        
        [Required]
        public byte[] PublicKey { get; set; } = Array.Empty<byte>();
        
        [Required]
        public byte[] UserHandle { get; set; } = Array.Empty<byte>();
        
        [Required]
        public uint SignatureCounter { get; set; }
        
        [Required]
        public string CredType { get; set; } = string.Empty;
        
        public DateTime RegDate { get; set; } = DateTime.UtcNow;
        
        public string AaGuid { get; set; } = string.Empty;
        
        public string? CredentialName { get; set; }
    }
} 