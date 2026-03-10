using App.Services.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace App.Api.ExceptionHandler;

public class GlobalExceptionHandler:IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, System.Exception exception, CancellationToken cancellationToken)
    {
        if (exception is GlobalException globalException)
        {

            var result = App.Services.ServiceResult.ServiceResult.Failed(
                statusCode: globalException.StatusCode,
                errorList:new []{globalException.Message}

            );
            await httpContext.Response.WriteAsJsonAsync(result, cancellationToken);
            return true;
        }

        return false;
    }
}