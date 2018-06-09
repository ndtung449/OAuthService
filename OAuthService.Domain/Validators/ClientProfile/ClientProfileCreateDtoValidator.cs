using FluentValidation;
using OAuthService.Domain.DTOs;
using OAuthService.Domain.Validators.Extensions;

namespace OAuthService.Domain.Validators
{
    public class ClientProfileCreateDtoValidator : AbstractValidator<ClientProfileCreateDto>
    {
        public ClientProfileCreateDtoValidator()
        {
            RuleFor(x => x.FaviconLocalUrl).IsAValidUri();
            RuleFor(x => x.FaviconUrl).IsAValidUri();
        }
    }
}
