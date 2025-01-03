using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace IoTHub.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // JWT validation logic
            await _next(context);
        }
    }
}