using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;

namespace Asp.netCoreRevsion.Utilities.Policies
{
    public class CustomPolicyRequirement : IAuthorizationRequirement
    {

    }

    public class CustomPolicyHandler :
        AuthorizationHandler<CustomPolicyRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CustomPolicyRequirement requirement)
        {
            var authContext = context.Resource as AuthorizationFilterContext;
            if (authContext == null)
                return Task.CompletedTask;

            authContext.HttpContext.Request.Headers.Add("success", "success");
            var c = authContext.HttpContext.User.Claims;
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

    public class CustomPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public CustomPolicyProvider(IOptions<AuthorizationOptions> options)
            : base(options)
        {
        }

        public override Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            var policy = policyName.Split(".");

            var name = policy.First();
            var value = policy.Last();


            var builder = new AuthorizationPolicyBuilder()
            .AddRequirements(new CustomPolicyRequirement())
                .RequireClaim(name, value);

            return Task.FromResult(builder.Build());
        }
    }
}
