using App.Services.ServiceResult;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
namespace App.Api.Filters;

public class CheckFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionArguments.TryGetValue("id", out var idValue) && idValue is int id && id <= 0)
        {
            var result = ServiceResult.Failed(
                HttpStatusCode.BadRequest,
                new[] { "Id değeri 0'dan büyük olmalıdır." }
            );

            context.Result = new BadRequestObjectResult(result);
            return; // Burada return diyerek işlemi kesiyoruz, controller'a gitmiyor.
        }
        
        // Eğer yukarıdaki if'e girmezse, isteği bir sonraki filtreye veya controller'a gönderir.
        await next();
    }
}