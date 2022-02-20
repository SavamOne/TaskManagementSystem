using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TaskManagementSystem.BusinessLogic.Models.Exceptions;
using TaskManagementSystem.Server.Exceptions;
using TaskManagementSystem.Server.Resources;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Server.Filters;

public class ApiResponseExceptionFilter : IActionFilter, IOrderedFilter
{
    private readonly ILoggerFactory loggerFactory;

    public ApiResponseExceptionFilter(ILoggerFactory loggerFactory)
    {
        this.loggerFactory = loggerFactory;
    }

    public void OnActionExecuting(ActionExecutingContext context) {}

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception == null)
        {
            return;
        }

        if (context.Exception is ArgumentNullException argumentNullException)
        {
            ErrorObject error = new(string.Format(LocalizedResources.ApiResponseExceptionFilter_ParameterIsEmptyOrMissing, argumentNullException.ParamName));

            context.Result = new BadRequestObjectResult(error);
            context.ExceptionHandled = true;
        }
        else if (context.Exception is BusinessLogicException argumentException)
        {
            ErrorObject error = new(argumentException.Description);

            context.Result = new BadRequestObjectResult(error);
            context.ExceptionHandled = true;
        }
        else if (context.Exception is ServerException serverException)
        {
            ErrorObject error = new(serverException.Description);

            context.Result = new BadRequestObjectResult(error);
            context.ExceptionHandled = true;
        }
        else
        {
            context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            context.ExceptionHandled = true;
            loggerFactory.CreateLogger(context.Controller.GetType()).LogError(context.Exception, "Unhandled exception");
        }

    }

    public int Order => int.MaxValue - 10;
}