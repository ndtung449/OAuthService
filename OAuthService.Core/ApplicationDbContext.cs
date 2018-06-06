using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OAuthService.Domain.Entities;

namespace OAuthService.Core
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ClientProfile> ClientProfile { get; set; }
    }
}
