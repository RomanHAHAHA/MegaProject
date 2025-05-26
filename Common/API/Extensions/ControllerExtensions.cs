using System.Net;
using Common.Domain.Models.Results;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Common.API.Extensions;

public static class ControllerExtensions
{
    public static IActionResult HandleResponse(
        this ControllerBase controller, 
        BaseResponse baseResponse)
    {
        if (baseResponse.IsSuccess)
        {
            return controller.Ok(new { message = baseResponse.Description });
        }

        if (baseResponse is { Description: ValidationResult validationResult, Status: HttpStatusCode.BadRequest })
        {
            var errors = validationResult.ToDictionary();
            return controller.BadRequest(new { errors });
        }

        return CreateResponse(controller, baseResponse.Status, baseResponse.Description);
    }

    public static IActionResult HandleResponse<T>(
        this ControllerBase controller, 
        BaseResponse<T> baseResponse)
    {
        if (baseResponse.IsSuccess)
        {
            return controller.Ok(new { data = baseResponse.Data });
        }

        if (baseResponse is { Error: ValidationResult validationResult, Status: HttpStatusCode.BadRequest })
        {
            var errors = validationResult.ToDictionary();
            return controller.BadRequest(new { errors });
        }

        return CreateResponse(controller, baseResponse.Status, baseResponse.Error);
    }
    
    private static IActionResult CreateResponse(
        this ControllerBase controller,
        HttpStatusCode statusCode,
        object? error)
    {
        var errorObject = error switch
        {
            null => new { message = "Unexpected error occurred during the request." },
            string str when string.IsNullOrWhiteSpace(str) => new { message = "Unexpected error occurred during the request." },
            string str => new { message = str },
            _ => error
        };

        return statusCode switch
        {
            HttpStatusCode.NotFound => controller.NotFound(errorObject),
            HttpStatusCode.Unauthorized => controller.Unauthorized(),
            HttpStatusCode.Conflict => controller.Conflict(errorObject),
            HttpStatusCode.InternalServerError => controller.StatusCode((int)HttpStatusCode.InternalServerError, errorObject),
            _ => controller.BadRequest(errorObject),
        };
    }
}
