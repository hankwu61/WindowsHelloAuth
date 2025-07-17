using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WindowsHelloAuth.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<FidoCredential> FidoCredentials { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // Configure FidoCredential entity
            builder.Entity<FidoCredential>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.CredentialId).IsUnique();
                entity.HasOne(e => e.User)
                      .WithMany(u => u.FidoCredentials)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
} 