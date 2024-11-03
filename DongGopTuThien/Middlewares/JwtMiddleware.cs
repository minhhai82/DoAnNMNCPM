using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var jwtService = context.RequestServices.GetRequiredService<IJwtService>();
        var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (authorizationHeader != null && authorizationHeader.StartsWith("Bearer "))
        {
            var token = authorizationHeader.Substring("Bearer ".Length).Trim();
            var claimsPrincipal = jwtService.VerifyToken(token);

            if (claimsPrincipal != null)
            {
                context.User = claimsPrincipal;
            }
        }

        await _next(context); // Call the next middleware in the pipeline
    }
}