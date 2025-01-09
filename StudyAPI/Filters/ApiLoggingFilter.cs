using Microsoft.AspNetCore.Mvc.Filters;

namespace StudyAPI.Filters;

public class ApiLoggingFilter : IActionFilter
{
    private readonly ILogger<ApiLoggingFilter> _logger;
    public ApiLoggingFilter(ILogger<ApiLoggingFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        _logger.LogInformation("Executando => OnActionExecuting");
        _logger.LogInformation("#######################################");
        _logger.LogInformation($"modelState : {context.ModelState.IsValid}");
        _logger.LogInformation("#######################################");
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _logger.LogInformation("Executado => OnActionExecuted");
        _logger.LogInformation("#######################################");
        _logger.LogInformation($"StatusCode : {context.HttpContext.Response.StatusCode}");
        _logger.LogInformation("#######################################");
    }
}