using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace DashboardCode.Routines.AspNetCore
{
    /// <summary>
    /// More about middleware:
    /// https://andrewlock.net/adding-default-security-headers-in-asp-net-core/
    /// </summary>
    public class DurationMiddleware
    {
        private readonly RequestDelegate next;
        private readonly string headerName;

        public DurationMiddleware(RequestDelegate next, string headerName = "X-Duration-MSec")
        {
            this.next = next;
            this.headerName = headerName;
        }

        public async Task Invoke(HttpContext context)
        {
            var watch = new Stopwatch();
            watch.Start();

            context.Response.OnStarting(state => {
                var httpContext = (HttpContext)state;
                var duration = watch.ElapsedMilliseconds.ToString();
                httpContext.Response.Headers.Add(headerName, new[] { duration });
                return Task.CompletedTask; // TODO: what is difference between .FromResult(0);  ?
            }, context);
            await next(context);
        }
    }
}
