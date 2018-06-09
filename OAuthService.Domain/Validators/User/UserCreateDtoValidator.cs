using FluentValidation;
using OAuthService.Domain.DTOs;
using OAuthService.Domain.Validators.Extensions;

namespace OAuthService.Domain.Validators
{
    public class UserCreateDtoValidator: AbstractValidator<UserCreateDto>
    {
        public UserCreateDtoValidator()
        {
            RuleFor(x => x.UserName).IsASafeString(Constants.Validation.UserNameMinLength, Constants.Validation.UserNameMaxLength);
            RuleFor(x => x.FirstName).IsASafeString(maxLength: Constants.Validation.NameMaxLength);
            RuleFor(x => x.LastName).IsASafeString(maxLength: Constants.Validation.NameMaxLength);
            RuleFor(x => x.Password)
                .NotNull()
                .WithMessage("Password is not allowed to empty.")
                .MinimumLength(Constants.Validation.PasswordMinLength)
                .WithMessage($"Password must be longer than {Constants.Validation.PasswordMinLength} characters");
        }
    }
}
