using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using StudyAPI.Validation;

namespace StudyAPI.Extensions;

public static class ApiExceptionMiddlewareExtensions //classe de extansao sempre STATIC
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appError => appError.Run(async context =>
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; //define status 500
            context.Response.ContentType = "application/json"; //tipo de resposta = JSON

            var contextFeature = context.Features.Get<IExceptionHandlerFeature>(); //pega a exceção
            if (contextFeature is not null)
            {
                await context.Response.WriteAsync(new ErrorDatails()
                {
                    StatusCode = context.Response.StatusCode,
                    Message = contextFeature.Error.Message, //mensagem do erro
                    Trace = contextFeature.Error.StackTrace //caminho do erro
                }.ToString());
            }
        }));
    }
}

//definir em program.cs;

//uso valido apenas em ambiente de desenvolvimento, pois mostra o TRACE;