using FluentValidation;
using OAuthService.Domain.DTOs;

namespace OAuthService.Domain.Validators
{
    public class ApiSecretCreateDtoValidator: AbstractValidator<ApiSecretCreateDto>
    {
        public ApiSecretCreateDtoValidator()
        {
            RuleFor(x => x.Secret)
                .MinimumLength(Constants.Validation.PasswordMinLength)
                .WithMessage($"{{PropertyName}} must be at least {Constants.Validation.PasswordMinLength} characters.")
                .MaximumLength(Constants.Validation.PasswordMaxLength)
                .WithMessage($"{{PropertyName}} cannot exceed {Constants.Validation.PasswordMaxLength} characters.");
            RuleFor(x => x.Description)
                .MaximumLength(Constants.Validation.DescriptionMaxLength)
                .WithMessage($"{{PropertyName}} cannot exceed {Constants.Validation.DescriptionMaxLength} characters.");
        }
    }
}
