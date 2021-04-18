using Extend.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;

namespace Extend.Models
{
    public class SessionCart : Cart
    {
        public static Cart GetCart(IServiceProvider serviceProvider)
        {
            var session = serviceProvider
                .GetRequiredService<IHttpContextAccessor>()?
                .HttpContext.Session;

            SessionCart cart =
                session?.GetJson<SessionCart>("Cart") ?? new SessionCart();

            cart.Session = session;
            return cart;
        }

        [JsonIgnore]
        public ISession Session { get; set; }

        public override void AddItem(Product product)
        {
            base.AddItem(product);
            Session.SetJson("Cart", this);
        }

        public override void RemoveLine(Product product)
        {
            base.RemoveLine(product);
            Session.SetJson("Cart", this);

        }

        public override void Clear()
        {
            base.Clear();
            Session.Remove("Cart");
        }
    }
}
