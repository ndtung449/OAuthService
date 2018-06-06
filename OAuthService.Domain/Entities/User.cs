using Microsoft.AspNetCore.Identity;

namespace OAuthService.Domain.Entities
{
    public class User : IdentityUser<string>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsBlocked { get; set; }
    }
}
