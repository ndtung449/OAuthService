using FluentValidation;
using FluentValidation.Results;
using OAuthService.Core.Exceptions;
using System;
using System.Linq;
using System.Reflection;

namespace OAuthService.Core.Services
{
    public abstract class BaseService
    {
        protected virtual void EnsureModelValid(object model)
        {
            ValidationResult validationResult = Validate(model);
            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage);
                throw new BadRequestException(string.Join("\r\n", errorMessages));
            }
        }

        protected virtual ValidationResult Validate(object model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            Type baseValidatorType = typeof(AbstractValidator<>);
            Type modelType = model.GetType();
            var genericValidatorType = baseValidatorType.MakeGenericType(modelType);

            Type validatorType = FindValidatorType(Assembly.GetExecutingAssembly(), genericValidatorType);

            var validatorInstance = (IValidator)Activator.CreateInstance(validatorType);
            return validatorInstance.Validate(model);
        }


        protected virtual Type FindValidatorType(Assembly assembly, Type genericValidatorType)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (genericValidatorType == null) throw new ArgumentNullException(nameof(genericValidatorType));
            return assembly.GetTypes().FirstOrDefault(t => t.IsSubclassOf(genericValidatorType));
        }
    }
}
