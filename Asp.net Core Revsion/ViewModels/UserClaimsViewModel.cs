using System.Collections.Generic;
using System.Security.Claims;

namespace Asp.net_Core_Revsion.ViewModels
{
    public class UserClaimsViewModel
    {
        public UserClaimsViewModel()
        {
            UserClaims = new List<UserClaim>();
        }
        public string UserId { get; set; }
        public IList<UserClaim> UserClaims { get; set; }
    }

    public class UserClaim
    {
        public string ClaimType { get; set; }
        public bool IsSelected { get; set; }
    }

    public static class ClaimsStore
    {
        public static List<Claim> AllClaims
            = new List<Claim>()
            {
                new Claim("Create Role","Create Role"),
                new Claim("Edit Role","Edit Role"),
                new Claim("Delete Role","Delete Role"),
                new Claim("Role","Create")
            };
    }
}