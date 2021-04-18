using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Asp.net
{
    public class CustomMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.Headers.Add("MyHeader", "Ibrahim_Elbatal");
            await _next(context);
        }
    }

}