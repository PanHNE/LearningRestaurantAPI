using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Exceptions;

namespace RestaurantAPI.MIddleware
{
    public class TimeOutMiddleware : IMiddleware
    {
        private readonly ILogger<TimeOutMiddleware> _logger;
        private Stopwatch _stopwatch;

        public TimeOutMiddleware(ILogger<TimeOutMiddleware> logger)
        {
            _logger = logger;
            _stopwatch = new Stopwatch();
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _stopwatch.Start();
            await next.Invoke(context);
            _stopwatch.Stop();

            var elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;

            if (elapsedMilliseconds / 1000 > 4 )
            {
                var message = $"Request [{context.Request.Method}] at {context.Request.Path} took {elapsedMilliseconds} ms";
                _logger.LogInformation(message);

            }
            
        }
    }
}
