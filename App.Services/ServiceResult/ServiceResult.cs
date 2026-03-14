using System.Net;
namespace App.Services.ServiceResult;

public class ServiceResult<T>
{
    public T? Data { get; private set; }

    public IEnumerable<string> ErrorMessages { get; private init; } = Enumerable.Empty<string>();

    public HttpStatusCode StatusCode { get; private set; }

    public string? Url { get; private set; }

    private bool IsSuccess => !ErrorMessages.Any();

    public bool IsFailed => !IsSuccess;

    public static ServiceResult<T> Success(T data, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new ServiceResult<T>
        {
            Data = data,
            StatusCode = statusCode
        };
    }

    public static ServiceResult<T> SuccessForCreated(T data, string url, HttpStatusCode statusCode = HttpStatusCode.Created)
    {
        return new ServiceResult<T>
        {
            Data = data,
            StatusCode = statusCode,
            Url = url
        };
    }

    public static ServiceResult<T> Failed(IEnumerable<string> errorList, HttpStatusCode statusCode=HttpStatusCode.ExpectationFailed)
    {
        return new ServiceResult<T>
        {
            ErrorMessages = errorList,
            StatusCode = statusCode
        };
    }
    
}

public class ServiceResult
{
    public IEnumerable<string> ErrorMessages { get; private set; } = Enumerable.Empty<string>();

    public HttpStatusCode StatusCode { get; private set; }

    public bool IsSuccess => !ErrorMessages.Any();

    public bool IsFailed => !IsSuccess;

    public static ServiceResult Success(HttpStatusCode statusCode)
        => new ServiceResult { StatusCode = statusCode };

    public static ServiceResult Failed(HttpStatusCode statusCode, IEnumerable<string> errorList)
        => new ServiceResult
        {
            StatusCode = statusCode,
            ErrorMessages = errorList
        };
}