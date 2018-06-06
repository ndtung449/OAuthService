using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OAuthService.Domain.DTOs
{
    public class NoRedirectUriClientForm
    {
        [MaxLength(Constants.Validation.NameMaxLength)]
        public string Name { get; set; }

        [MaxLength(Constants.Validation.UriMaxLength)]
        public string Uri { get; set; }

        [Required]
        public List<string> Scopes { get; set; }
    }
}
