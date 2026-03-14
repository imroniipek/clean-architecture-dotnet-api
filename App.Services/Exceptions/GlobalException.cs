using System.Net;

namespace App.Services.Exceptions;

public class GlobalException:AppException
{
    public GlobalException(HttpStatusCode statuscode, string message) : base(statuscode, message)
    {}
    
}