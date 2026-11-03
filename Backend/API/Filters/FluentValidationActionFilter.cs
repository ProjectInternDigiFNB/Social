using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters;

/// <summary>
/// Bộ lọc tự động chạy FluentValidation cho các Request DTO trước khi vào Controller Action.
/// </summary>
public sealed class FluentValidationActionFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
            {
                continue;
            }

            var argumentType = argument.GetType();
            if (argumentType.IsPrimitive || argumentType == typeof(string) || argumentType == typeof(Guid))
            {
                continue;
            }

            var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);
            if (context.HttpContext.RequestServices.GetService(validatorType) is IValidator validator)
            {
                var validationContext = new ValidationContext<object>(argument);
                var validationResult = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);
                if (!validationResult.IsValid)
                {
                    var problemDetails = new ValidationProblemDetails(validationResult.ToDictionary());
                    context.Result = context.Controller is ControllerBase controller
                        ? controller.ValidationProblem(problemDetails)
                        : new BadRequestObjectResult(problemDetails);
                    return;
                }
            }
        }

        await next();
    }
}
