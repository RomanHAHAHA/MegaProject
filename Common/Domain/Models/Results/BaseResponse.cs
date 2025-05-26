using System.Net;
using FluentValidation.Results;

namespace Common.Domain.Models.Results;

public class BaseResponse<T>(
    HttpStatusCode statusCode = HttpStatusCode.OK,
    object? error = null,
    T data = default)
{
    public object? Error { get; set; } = error;

    public HttpStatusCode Status { get; set; } = statusCode;

    public T Data { get; set; } = data;

    public bool IsSuccess => Status == HttpStatusCode.OK;

    public bool IsFailure => !IsSuccess;

    public static BaseResponse<T> NotFound(string notFoundEntityName) 
        => new(HttpStatusCode.NotFound, $"{notFoundEntityName} was not found");

    public static BaseResponse<T> Ok(T data) => new(data: data);

    public static BaseResponse<T> InternalServerError(string description = "Unexpected error occured during the request") 
        => new(HttpStatusCode.InternalServerError, description);

    public static BaseResponse<T> Conflict(string description = "")
        => new(HttpStatusCode.Conflict, description);
    
    public static BaseResponse<T> UnAuthorized(string description = "")
        => new(HttpStatusCode.Unauthorized, description);
    
    public static BaseResponse<T> BadRequest(ValidationResult validationResult)
        => new(HttpStatusCode.BadRequest, validationResult);

    public static BaseResponse<T> BadRequest(string description = "")
        => new(HttpStatusCode.BadRequest, description);
    
    public static implicit operator BaseResponse<T>(T value) => Ok(value);
}

public class BaseResponse
{
    public object? Description { get; set; } 

    public HttpStatusCode Status { get; set; }

    public bool IsSuccess => Status == HttpStatusCode.OK;

    public bool IsFailure => !IsSuccess;
    
    private BaseResponse(
        HttpStatusCode statusCode = HttpStatusCode.OK,
        object? description = null)
    {
        Status = statusCode;
        Description = description;
    }

    public static BaseResponse NotFound(string notFoundEntityName)
    {
        return new BaseResponse(
            HttpStatusCode.NotFound,
            $"{notFoundEntityName} was not found");
    }

    public static BaseResponse Ok(string message = "") => new();

    public static BaseResponse InternalServerError(string error = "Unexpected error occured during the request") 
        => new(HttpStatusCode.InternalServerError, error);

    public static BaseResponse Conflict(string error = "") 
        => new(HttpStatusCode.Conflict, error);

    public static BaseResponse BadRequest(string error = "")
        => new(HttpStatusCode.BadRequest, error);
    
    public static BaseResponse BadRequest(ValidationResult validationResult)
        => new(HttpStatusCode.BadRequest, validationResult);
}