using App.Services.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace App.Api.ExceptionHandler;

public class NotFoundExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is NotFoundException notFoundException)
        {
            var result = App.Services.ServiceResult.ServiceResult.Failed(
                notFoundException.StatusCode,
                new[] { notFoundException.Message }
            );
           
            await httpContext.Response.WriteAsJsonAsync(result, cancellationToken);

            return true;
        }
        
        return false;
    }
}