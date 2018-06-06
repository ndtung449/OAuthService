using System.ComponentModel.DataAnnotations;

namespace OAuthService.Domain.DTOs
{
    public class ClientProfileForm
    {
        [MaxLength(Constants.Validation.UriMaxLength)]
        public string FaviconUrl { get; set; }

        [MaxLength(Constants.Validation.UriMaxLength)]
        public string FaviconLocalUrl { get; set; }
    }
}
