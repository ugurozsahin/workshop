using Microsoft.AspNetCore.Builder;

namespace Web.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseApplicationStartTimeHeader(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApplicationStartTimeHeader>();
        }

        public static IApplicationBuilder UseExceptionLoggingHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionLoggingHandler>();
        }
    }
}
