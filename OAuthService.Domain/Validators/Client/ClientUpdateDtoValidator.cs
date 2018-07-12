using FluentValidation;
using OAuthService.Domain.DTOs;
using OAuthService.Domain.Validators.Extensions;

namespace OAuthService.Domain.Validators
{
    public class ClientUpdateDtoValidator : AbstractValidator<ClientUpdateDto>
    {
        public ClientUpdateDtoValidator()
        {
            RuleFor(x => x.ClientId)
                .NotEmpty()
                .WithMessage("{PropertyName} cannot be empty.");
            RuleFor(x => x.ClientName)
                .MaximumLength(Constants.Validation.NameMaxLength)
                .WithMessage($"{{PropertyName}} cannot exceed ${Constants.Validation.NameMaxLength} characters.");
            RuleFor(x => x.GrantTypes)
                .NotEmpty()
                .WithMessage("{PropertyName} cannot be empty.");
            RuleFor(x => x.ClientUri).IsAValidUri();
            RuleFor(x => x.Scopes)
                .NotEmpty()
                .WithMessage("{PropertyName} cannot be empty.");
        }
    }
}
