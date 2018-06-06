using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OAuthService.Domain.DTOs
{
    public class UpdateClientForm
    {
        [Required]
        [MaxLength(Constants.Validation.IdMaxLength)]
        public string ClientId { get; set; }

        [MaxLength(Constants.Validation.NameMaxLength)]
        public string ClientName { get; set; }

        [MaxLength(Constants.Validation.UriMaxLength)]
        public string Uri { get; set; }

        [Required]
        public List<string> GrantTypes { get; set; }
        
        public List<string> RedirectUris { get; set; }
        
        public List<string> PostLogoutRedirectUris { get; set; }

        [Required]
        public List<string> Scopes { get; set; }
    }
}
