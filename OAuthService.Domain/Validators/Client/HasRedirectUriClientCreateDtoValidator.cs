using FluentValidation;
using OAuthService.Domain.DTOs;
using OAuthService.Domain.Validators.Extensions;

namespace OAuthService.Domain.Validators
{
    public class HasRedirectUriClientCreateDtoValidator : AbstractValidator<HasRedirectUriClientCreateDto>
    {
        public HasRedirectUriClientCreateDtoValidator()
        {
            RuleFor(x => x.ClientName)
                .MaximumLength(Constants.Validation.NameMaxLength)
                .WithMessage($"{{PropertyName}} cannot exceed ${Constants.Validation.NameMaxLength} characters.");
            RuleFor(x => x.ClientUri).IsAValidUri();
            RuleFor(x => x.Scopes)
                .NotEmpty()
                .WithMessage("{PropertyName} cannot be empty.");
            RuleFor(x => x.RedirectUri).IsAValidUri();
            RuleFor(x => x.PostLogoutRedirectUri).IsAValidUri();
        }
    }
}
