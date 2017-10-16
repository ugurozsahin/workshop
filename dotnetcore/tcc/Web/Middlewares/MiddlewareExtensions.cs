using Microsoft.AspNetCore.Builder;

namespace Web.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionLoggingHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionLoggingHandler>();
        }
    }
}
