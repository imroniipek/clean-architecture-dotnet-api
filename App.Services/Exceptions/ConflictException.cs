using System.Net;

namespace App.Services.Exceptions;

public class ConflictException:AppException
{
    public ConflictException(string message) : base(HttpStatusCode.Conflict,message)
    {}
}