using System.Net;

namespace App.Services.Exceptions;

public class AppException:Exception
{
    public HttpStatusCode StatusCode { get; }
    public IEnumerable<string> ? Errors { get;}
    public AppException(HttpStatusCode statuscode, string message) : base(message)
    {
        StatusCode = statuscode;
    }
    public AppException(HttpStatusCode statuscode,IEnumerable<string> errors) : base("Bir Veya Birden Fazla hata oluştu")
    {
        StatusCode = statuscode;
        Errors = errors;
    }
}