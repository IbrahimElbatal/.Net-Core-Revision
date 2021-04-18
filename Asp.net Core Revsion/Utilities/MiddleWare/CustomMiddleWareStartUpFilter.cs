using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;

namespace Asp.net
{
    //you can register it in Startup.cs or extend StartUpFilter

    public class CustomMiddleWareStartUpFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.UseMiddleware<CustomMiddleware>();
                next(builder);
            };
        }
    }
}