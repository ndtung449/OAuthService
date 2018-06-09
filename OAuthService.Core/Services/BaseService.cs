using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using OAuthService.Core.Exceptions;
using System;
using System.Linq;

namespace OAuthService.Core.Services
{
    public abstract class BaseService
    {
        protected readonly IHttpContextAccessor _contextAccessor;

        public BaseService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

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

            Type baseValidatorType = typeof(IValidator<>);
            Type modelType = model.GetType();
            Type modelValidatorType = baseValidatorType.MakeGenericType(modelType);
            var validatorInstance = (IValidator)_contextAccessor.HttpContext.RequestServices.GetService(modelValidatorType);

            return validatorInstance.Validate(model);
        }
    }
}
