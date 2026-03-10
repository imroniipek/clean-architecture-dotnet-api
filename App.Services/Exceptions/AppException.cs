using System.Net;

namespace App.Services.Exceptions;

public class AppException:System.Exception
{
    public HttpStatusCode StatusCode { get; }


    public AppException(HttpStatusCode statuscode, string message) : base(message)
    {
        StatusCode = statuscode;
    }
}