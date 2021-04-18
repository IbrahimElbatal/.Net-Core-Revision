using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Asp.net
{
    public class CustomMiddleWareUsingFactoryInterface : IMiddlewareFactory
    {
        private readonly HttpContext _context;
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _provider;

        public CustomMiddleWareUsingFactoryInterface(
            HttpContext context,
            RequestDelegate next,
            IServiceProvider provider)
        {
            _context = context;
            _next = next;
            _provider = provider;
        }
        public IMiddleware Create(Type middlewareType)
        {
            return ActivatorUtilities.GetServiceOrCreateInstance(_provider, middlewareType) as IMiddleware;

        }

        public void Release(IMiddleware middleware)
        {
            _context.Response.Headers.Add("c", "ccc");
            middleware.InvokeAsync(_context, _next);
        }
    }
}