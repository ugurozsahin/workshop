using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Web.Middlewares
{
    public class ExceptionLoggingHandler
    {
        private readonly RequestDelegate _nextDelegate;
        private readonly ILogger<ExceptionLoggingHandler> _logger;

        public ExceptionLoggingHandler(RequestDelegate next,
           ILogger<ExceptionLoggingHandler> logger)
        {
            _nextDelegate = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _nextDelegate.Invoke(httpContext).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Catched Exception!!! Path: {0}, Query: {1}, Method: {2}"
                //    , httpContext.Request.Path.HasValue ? httpContext.Request.Path.ToUriComponent() : string.Empty
                //    , httpContext.Request.Query
                //    , httpContext.Request.Method);

                throw;
            }
        }
    }
}
