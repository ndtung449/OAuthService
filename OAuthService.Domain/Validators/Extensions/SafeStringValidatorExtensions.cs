using FluentValidation;

namespace OAuthService.Domain.Validators.Extensions
{
    public static class SafeStringValidatorExtensions
    {
        public static IRuleBuilderOptions<T, string> IsASafeString<T>(this IRuleBuilder<T, string> ruleBuilder,
            int minLength = 1, int maxLength = 200)
        {
            string safeStringPattern = $@"^\w{{{minLength},{maxLength}}}$";

            return ruleBuilder
                .Matches(safeStringPattern)
                .WithMessage("{PropertyName} must contain only letters, numbers, or underscores. " +
                    $"The number of characters must be between {minLength} and {maxLength}.");
        }
    }
}
