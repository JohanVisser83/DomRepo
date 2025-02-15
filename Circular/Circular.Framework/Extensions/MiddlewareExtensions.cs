using Microsoft.AspNetCore.Builder;
using Circular.Framework.Middleware;

namespace Circular.Framework.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app) =>
    app.UseMiddleware<ExceptionMiddleware>();
    }
}
