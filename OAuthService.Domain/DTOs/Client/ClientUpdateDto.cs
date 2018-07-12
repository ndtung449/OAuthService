using System.Collections.Generic;

namespace OAuthService.Domain.DTOs
{
    public class ClientUpdateDto
    {
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientUri { get; set; }
        public List<string> GrantTypes { get; set; }
        public List<string> RedirectUris { get; set; }
        public List<string> PostLogoutRedirectUris { get; set; }
        public List<string> Scopes { get; set; }
    }
}
