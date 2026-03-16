using System.Net;
using App.Services.ServiceResult;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace App.Api.Filters.ValidationFilters;

public class ValidationFilter:IAsyncActionFilter
{
     private readonly IServiceProvider _serviceProvider;

    public ValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var errors = new List<string>();

        foreach (var actionArgument in context.ActionArguments.Values)
        {
            //FluentValidation:veri doğru mu kontrol eder
            
            //ValidationContext:kontrol edilecek veriyi validator’a verir
            
            if (actionArgument is null)
                continue;

            var argumentType = actionArgument.GetType();

            var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);
            
            var validator = _serviceProvider.GetService(validatorType);

            if (validator is null)
                continue;

            var validationContextType = typeof(ValidationContext<>).MakeGenericType(argumentType);
            
            var validationContext = Activator.CreateInstance(validationContextType, actionArgument);

            if (validationContext is null)
                continue;

            var validateAsyncMethod = validatorType.GetMethod("ValidateAsync", new[] { validationContextType, typeof(CancellationToken) });

            if (validateAsyncMethod is null)
                continue;

            var task = (Task)validateAsyncMethod.Invoke(validator, new object[] { validationContext, context.HttpContext.RequestAborted })!;
            await task;

            var resultProperty = task.GetType().GetProperty("Result"); //GetType ile ne tur bir obje oldugunu buluyoruz ve ordaki Result alıyoruz
            
            var validationResult = resultProperty?.GetValue(task) as ValidationResult;

            if (validationResult is null || validationResult.IsValid)
                continue;

            errors.AddRange(validationResult.Errors.Select(x => x.ErrorMessage));
        }

        if (errors.Count > 0)
        {
            var serviceResult = ServiceResult.Failed(HttpStatusCode.BadRequest, errors);
            context.Result = new BadRequestObjectResult(serviceResult);
            return;
        }

        await next();
    }

  
}