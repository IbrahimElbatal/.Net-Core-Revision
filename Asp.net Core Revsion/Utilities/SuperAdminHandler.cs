
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Asp.net_Core_Revsion.Utilities
{
    public class SuperAdminHandler :
        AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            ManageAdminRolesAndClaimsRequirement requirement)
        {
            if(context.User.IsInRole("Super Admin"))
                context.Succeed(requirement);
            
            return Task.CompletedTask;
        }
    }
}