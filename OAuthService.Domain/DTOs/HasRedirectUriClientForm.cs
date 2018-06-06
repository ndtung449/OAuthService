using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OAuthService.Domain.DTOs
{
    public class HasRedirectUriClientForm
    {
        [MaxLength(Constants.Validation.NameMaxLength)]
        public string Name { get; set; }

        [MaxLength(Constants.Validation.UriMaxLength)]
        public string Uri { get; set; }

        [Required]
        public List<string> Scopes { get; set; }

        [Required, MaxLength(Constants.Validation.UriMaxLength)]
        public string RedirectUri { get; set; }

        [MaxLength(Constants.Validation.UriMaxLength)]
        public string PostLogoutRedirectUri { get; set; }
    }
}
