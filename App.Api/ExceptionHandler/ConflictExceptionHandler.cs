using App.Services.Exceptions;
using App.Services.ServiceResult;
using Microsoft.AspNetCore.Diagnostics;
namespace App.Api.ExceptionHandler;
public class ConflictExceptionHandler:IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is ConflictException conflictException)
        {
            var result = ServiceResult.Failed(conflictException.StatusCode, new [] { conflictException.Message });

            httpContext.Response.StatusCode = Convert.ToInt32(conflictException.StatusCode);

            await httpContext.Response.WriteAsJsonAsync(result, cancellationToken);
            return true;
        }
        return false;
    }
}