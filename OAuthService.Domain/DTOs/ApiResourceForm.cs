using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OAuthService.Domain.DTOs
{
    public class ApiResourceForm
    {
        [Required, MaxLength(Constants.Validation.NameMaxLength)]
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public List<string> UserClaims { get; set; }

        public List<string> Scopes { get; set; }
    }
}
