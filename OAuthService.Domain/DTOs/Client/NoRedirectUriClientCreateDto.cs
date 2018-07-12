using System.Collections.Generic;

namespace OAuthService.Domain.DTOs
{
    public class NoRedirectUriClientCreateDto
    {
        public string ClientName { get; set; }
        public string ClientUri { get; set; }
        public List<string> Scopes { get; set; }
    }
}
