using System.Collections.Generic;

namespace OAuthService.Domain.DTOs
{
    public class NoRedirectUriClientCreateDto
    {
        public string Name { get; set; }
        public string Uri { get; set; }
        public List<string> Scopes { get; set; }
    }
}
