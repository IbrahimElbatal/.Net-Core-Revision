using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Asp.net
{
    public class CustomMiddlewareUsingIMiddleWareInterface : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            context.Response.Headers.Add("c", "cc");
            await next.Invoke(context);
        }
    }
}