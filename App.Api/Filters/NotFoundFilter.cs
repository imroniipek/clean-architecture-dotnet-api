using System.Net;
using App.Repository.Entities;
using App.Repository.Repo.Abstract;
using App.Services.ServiceResult;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace App.Api.Filters;


//Burda throw exception yerine doğrudan Bir filtreleme classı ile hatayı bulabiliriz//
public class NotFoundFilter<T, T1>(IGenericRepository<T, T1> genericRepository) : IAsyncActionFilter
    where T : class, IBaseEntity<T1>
    where T1 : struct
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
       
        var idValue = context.ActionArguments.Values.FirstOrDefault(); //Oncelikle gelen Actionun value degerlerinden ilkini alacam buna gore silme ve ekleme yapcam

        if (idValue == null||idValue is not T1 id)
        {
            await next();
            return;
        }
        
        var anyEntity = await genericRepository.AnyAsync(id);

        if (anyEntity)
        {
            await next();
            return;
            
        }
        var entityName = typeof(T).Name;
        var actionName = context.ActionDescriptor.DisplayName;
        var result = ServiceResult.Failed(HttpStatusCode.NotFound,[$"data bulunamadı ({actionName})({entityName})"]);
        context.Result = new NotFoundObjectResult(result);
        await next();
    }
}