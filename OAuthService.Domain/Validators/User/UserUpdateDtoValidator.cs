using FluentValidation;
using OAuthService.Domain.DTOs;
using OAuthService.Domain.Validators.Extensions;

namespace OAuthService.Domain.Validators
{
    public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
    {
        public UserUpdateDtoValidator()
        {
            RuleFor(x => x.UserName).IsASafeString(Constants.Validation.UserNameMinLength, Constants.Validation.UserNameMaxLength);
            RuleFor(x => x.FirstName).IsASafeString(maxLength: Constants.Validation.NameMaxLength);
            RuleFor(x => x.LastName).IsASafeString(maxLength: Constants.Validation.NameMaxLength);
        }
    }
}
