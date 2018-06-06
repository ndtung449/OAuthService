using System.Collections.Generic;

namespace OAuthService.Domain.DTOs
{
    public class UserViewModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string> Roles { get; set; }
        public List<UserClaim> Claims { get; set; }
    }
}
