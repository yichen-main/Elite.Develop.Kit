namespace API.Authentication.Foundations;
public class ApiKeyAuthenticationEndpointFilter : IEndpointFilter
{
    const string _apiKeyHeaderName = "X-Api-Key";
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var apiKey = context.HttpContext.Request.Headers[_apiKeyHeaderName];
        if (!IsApiKeyValid(apiKey))
        {
            return Results.Unauthorized();
        }
        return await next(context);
    }
    static bool IsApiKeyValid(string? apiKey)
    {
        return !string.IsNullOrWhiteSpace(apiKey);
    }
}