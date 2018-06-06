using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthService.Domain.DTOs
{
    public class ApiSecretForm
    {
        public string Description { get; set; }

        [Required, MaxLength(Constants.Validation.PasswordMaxLength), MinLength(Constants.Validation.PasswordMinLength)]
        public string Secret { get; set; }
    }
}
