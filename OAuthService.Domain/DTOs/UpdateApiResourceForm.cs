using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OAuthService.Domain.DTOs
{
    public class UpdateApiResourceForm
    {
        [Required, MaxLength(Constants.Validation.NameMaxLength)]
        public string Name { get; set; }

        [MaxLength(Constants.Validation.DisplayNameMaxLength)]
        public string DisplayName { get; set; }

        [MaxLength(Constants.Validation.DescriptionMaxLength)]
        public string Description { get; set; }

        public List<string> UserClaims { get; set; }

        public List<string> Scopes { get; set; }
    }
}
