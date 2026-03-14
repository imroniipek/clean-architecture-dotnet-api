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
                new string[] { notFoundException.Message }
            );
             httpContext.Response.StatusCode = Convert.ToInt32(notFoundException.StatusCode);
            await httpContext.Response.WriteAsJsonAsync(result, cancellationToken);

            var path = httpContext.Request.Path;

            var query = httpContext.Request.Query;
            
            return true;
        }
        
        return false;
    }
}