using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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

        if (context.Exception is ArgumentException argumentException)
        {
            ErrorObject error = new(argumentException.Message);

            context.Result = new BadRequestObjectResult(error);
            context.ExceptionHandled = true;
        }
        else
        {
            context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            context.ExceptionHandled = true;
            loggerFactory.CreateLogger(context.Controller.GetType()).LogCritical(context.Exception, "Something went wrong");
        }

    }

    public int Order => int.MaxValue - 10;
}