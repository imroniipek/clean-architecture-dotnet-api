using System.Net;

namespace App.Services.Exceptions;

public class GlobalException:AppException
{
    public GlobalException(string message) : base(HttpStatusCode.BadGateway, message) {}
}