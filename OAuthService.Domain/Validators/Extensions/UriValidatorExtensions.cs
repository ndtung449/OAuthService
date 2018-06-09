using FluentValidation;
using System;

namespace OAuthService.Domain.Validators.Extensions
{
    public static class UriValidatorExtensions
    {
        public static IRuleBuilderOptions<T, string> IsAValidUri<T>(this IRuleBuilder<T, string> ruleBuilder,
               UriKind uriKind = UriKind.RelativeOrAbsolute)
        {
            return ruleBuilder
                .Must(uri => Uri.IsWellFormedUriString(uri, uriKind))
                .WithMessage("{PropertyName} is not well formed")
                .MaximumLength(Constants.Validation.UriMaxLength)
                .WithMessage($"{{PropertyName}} cannot exceed {Constants.Validation.UriMaxLength} characters.");
        }
    }
}
