using System.ComponentModel.DataAnnotations;

namespace OAuthService.Domain.DTOs
{
    public class ClientProfileUpdateForm
    {
        [Required]
        [MaxLength(Constants.Validation.IdMaxLength)]
        public string ClientId { get; set; }

        [MaxLength(Constants.Validation.UriMaxLength)]
        public string FaviconUrl { get; set; }

        [MaxLength(Constants.Validation.UriMaxLength)]
        public string FaviconLocalUrl { get; set; }
    }
}
