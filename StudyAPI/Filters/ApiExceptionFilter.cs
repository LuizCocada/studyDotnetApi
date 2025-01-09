using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StudyAPI.Filters;

public class ApiExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ApiExceptionFilter> _logger;
    public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger) //escrever isto caderno: significa que uma instancia de ILogger foi configurada para logar mensagens dentro de ApiExceptionFilter 
    {
        _logger = logger;
    }
    
    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Ocorreu uma exceção não tradada.");//logando a exceção

        context.Result = new ObjectResult("Ocorreu um problema ao tratar sua solicitação.")//retorna um error nao tratado passando mensagem e status 500
        {
            StatusCode = StatusCodes.Status500InternalServerError,
        };
    }
}