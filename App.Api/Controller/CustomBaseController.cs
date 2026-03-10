using System.Net;
using App.Services.ServiceResult;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controller;

[ApiController]

[Route("api/[Controller]")]
public abstract class CustomBaseController : ControllerBase
{
    // ServiceResult<T> döndüğünde uygun HTTP response üretir
    protected IActionResult CreateActionResult<T>(ServiceResult<T> result)
    {
        if (result.IsFailed)
        {
            return new ObjectResult(new { errors = result.ErrorMessages })
            {
                StatusCode = (int)result.StatusCode
            };
        }

        if (result.StatusCode == HttpStatusCode.Created)
        {
            return Created(result.Url!, result.Data);
        }

        if (result.StatusCode == HttpStatusCode.NoContent)
        {
            return new NoContentResult();
        }

        return new ObjectResult(result.Data)
        {
            StatusCode = (int)result.StatusCode
        };
    }

    // ServiceResult (generic olmayan) döndüğünde uygun HTTP response üretir
    protected IActionResult CreateActionResult(ServiceResult result)
    {
        if (result.IsFailed)
        {
            return new ObjectResult(new { errors = result.ErrorMessages })
            {
                StatusCode = (int)result.StatusCode
            };
        }

        if (result.StatusCode == HttpStatusCode.NoContent)
        {
            return new NoContentResult();
        }

        return new ObjectResult(null)
        {
            StatusCode = (int)result.StatusCode
        };
    }
}