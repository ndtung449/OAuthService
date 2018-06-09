using FluentValidation;
using OAuthService.Domain.DTOs;
using OAuthService.Domain.Validators.Extensions;

namespace OAuthService.Domain.Validators
{
    public class ApiResourceUpdateDtoValidator : AbstractValidator<ApiResourceUpdateDto>
    {
        public ApiResourceUpdateDtoValidator()
        {
            RuleFor(x => x.Name).IsASafeString(maxLength: Constants.Validation.NameMaxLength);
            RuleFor(x => x.DisplayName)
                .MaximumLength(Constants.Validation.DisplayNameMaxLength)
                .WithMessage($"{{PropertyName}} cannot exceed {Constants.Validation.DisplayNameMaxLength} characters.");
            RuleFor(x => x.Description)
                .MaximumLength(Constants.Validation.DescriptionMaxLength)
                .WithMessage($"{{PropertyName}} cannot exceed {Constants.Validation.DescriptionMaxLength} characters.");
        }
    }
}
