using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace OAuthService.Domain.DTOs.User
{
    public class UserCreateDtoValidator: AbstractValidator<UserCreateDto>
    {
        public UserCreateDtoValidator()
        {
            RuleFor(x => x.UserName)

        }
    }
}
