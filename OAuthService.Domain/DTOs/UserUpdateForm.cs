using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OAuthService.Domain.DTOs
{
    public class UserUpdateForm
    {
        [Required]
        [MaxLength(Constants.Validation.UserNameMaxLength)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(Constants.Validation.EmailMaxLength)]
        public string Email { get; set; }

        [Required]
        [MaxLength(Constants.Validation.NameMaxLength)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(Constants.Validation.NameMaxLength)]
        public string LastName { get; set; }

        public List<string> Roles { get; set; }

        public List<UserClaim> Claims { get; set; }
    }
}
