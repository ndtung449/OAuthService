using System.Collections.Generic;

namespace OAuthService.Domain.DTOs
{
    public class ApiResourceViewModel
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public List<string> UserClaims { get; set; }

        public List<string> Scopes { get; set; }
    }
}
