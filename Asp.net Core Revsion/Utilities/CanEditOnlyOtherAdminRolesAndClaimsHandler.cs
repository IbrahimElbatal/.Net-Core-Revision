using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Asp.net_Core_Revsion.Utilities
{
    public class CanEditOnlyOtherAdminRolesAndClaimsHandler :
        AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            ManageAdminRolesAndClaimsRequirement requirement)
        {
            var authContext = context.Resource as AuthorizationFilterContext;
            if (authContext == null)
                return Task.CompletedTask;

            var loggedInAdmin =
                context.User.Claims
                    .SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var adminIdBeingEdited = authContext.HttpContext.Request.Query["userId"];

            if (context.User.IsInRole("Admin") &&
                context.User.HasClaim(claim=>claim.Type == "Edit Role")&&
                loggedInAdmin?.ToLower() != adminIdBeingEdited.ToString().ToLower())
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}