using FluentValidation;
using OAuthService.Domain.DTOs;
using OAuthService.Domain.Validators.Extensions;

namespace OAuthService.Domain.Validators
{
    public class ClientProfileUpdateDtoValidator: AbstractValidator<ClientProfileUpdateDto>
    {
        public ClientProfileUpdateDtoValidator()
        {
            RuleFor(x => x.ClientId)
                .NotNull()
                .WithMessage("{PropertyName} must be provided.");
            RuleFor(x => x.FaviconLocalUrl).IsAValidUri();
            RuleFor(x => x.FaviconUrl).IsAValidUri();
        }
    }
}
