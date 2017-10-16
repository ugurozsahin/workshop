using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Middlewares
{
    public class ApplicationStartTimeHeader
    {
        private readonly RequestDelegate _nextDelegate;
        private readonly IDistributedCache _distributedCacher;

        public ApplicationStartTimeHeader(RequestDelegate next,
           IDistributedCache cache)
        {
            _nextDelegate = next;
            _distributedCacher = cache;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var startTimeString = "Not found.";
            var value = await _distributedCacher.GetStringAsync("applicationStartTime").ConfigureAwait(false);
            if (value != null)
            {
                startTimeString = value;
            }

            httpContext.Response.Headers.Append("Application-Start-Time", startTimeString);

            await _nextDelegate.Invoke(httpContext).ConfigureAwait(false);
        }
    }
}
