using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PaymentGatewayDemo.Api.Filters.Exception;

public class SwaggerGeneralExceptionOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Add global response codes
        if (!operation.Responses.ContainsKey("400"))
            operation.Responses.Add("400",
                new OpenApiResponse { Description = "Bad request - probably validation errors" });
        if (!operation.Responses.ContainsKey("401"))
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
    }
}