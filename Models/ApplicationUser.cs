using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace WindowsHelloAuth.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty;
        public virtual ICollection<FidoCredential> FidoCredentials { get; set; } = new List<FidoCredential>();
    }
} 