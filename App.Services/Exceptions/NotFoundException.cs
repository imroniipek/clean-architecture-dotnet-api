using System.Net;

namespace App.Services.Exceptions;

public class NotFoundException(string entityName, object id) : AppException(HttpStatusCode.NotFound,
    $"{id}'li {entityName}Bulunamadı");