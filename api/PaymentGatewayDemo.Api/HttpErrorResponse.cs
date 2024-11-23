using Microsoft.AspNetCore.Mvc;
using PaymentGatewayDemo.Domain.Errors;

namespace PaymentGatewayDemo.Api;

public class HttpErrorResponse
{
    public HttpErrorResponse(int statusCode, DomainError error)
    {
        Title = error.Message;
        Message = error.Details;
        StatusCode = statusCode;
    }

    public HttpErrorResponse(int statusCode, string title, string message)
    {
        StatusCode = statusCode;
        Title = title;
        Message = message;
    }

    public int StatusCode { get; init; }
    public string Title { get; init; }
    public string Message { get; init; }


    public ActionResult ToActionResult()
    {
        return new ObjectResult(this)
        {
            StatusCode = StatusCode
        };
    }

    public static implicit operator ActionResult(HttpErrorResponse errorResponse)
    {
        return errorResponse.ToActionResult();
    }
}