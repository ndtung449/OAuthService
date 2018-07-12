using System.Collections.Generic;

namespace OAuthService.Domain.DTOs
{
    public class HasRedirectUriClientCreateDto
    {
        public string ClientName { get; set; }
        public string ClientUri { get; set; }
        public List<string> Scopes { get; set; }
        public string RedirectUri { get; set; }
        public string PostLogoutRedirectUri { get; set; }
    }
}
