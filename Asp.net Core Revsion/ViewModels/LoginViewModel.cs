using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Asp.net_Core_Revsion.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [RegularExpression(@"^[A-Za-z0-9_]+@[A-Za-z0-9]+\.[a-zA-z]+$")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
    }
}