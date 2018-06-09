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
            RuleFor(x => x.ClientName).IsASafeString(maxLength: Constants.Validation.NameMaxLength);
            RuleFor(x => x.GrantTypes)
                .NotEmpty()
                .WithMessage("{PropertyName} cannot be empty.");
            RuleFor(x => x.Uri).IsAValidUri();
            RuleFor(x => x.Scopes)
                .NotEmpty()
                .WithMessage("{PropertyName} cannot be empty.");
        }
    }
}
