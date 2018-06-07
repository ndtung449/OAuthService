using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OAuthService.Domain.DTOs
{
    public class UserCreateDto
    {
        [Required]
        [MaxLength(Constants.Validation.UserNameMaxLength)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(Constants.Validation.EmailMaxLength)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(Constants.Validation.NameMaxLength)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(Constants.Validation.NameMaxLength)]
        public string LastName { get; set; }

        public List<string> Roles { get; set; }

        public List<UserClaim> Claims { get; set; }

        [Required]
        [MaxLength(Constants.Validation.PasswordMaxLength)]
        [MinLength(Constants.Validation.PasswordMinLength)]
        public string Password { get; set; }
    }

    public class UserClaim
    {
        [Required]
        [MaxLength(Constants.Validation.ClaimTypeMaxLength)]
        public string Type { get; set; }

        [MaxLength(Constants.Validation.ClaimValueMaxLength)]
        public string Value { get; set; }
    }
}
