using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Asp.net_Core_Revsion.Utilities;

namespace Asp.net_Core_Revsion.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [RegularExpression(@"^[A-Za-z0-9_]+@[A-Za-z0-9]+\.[a-zA-z]+$")]
        [Remote("EmailExists", "Account")]
        [ValidateEmailDomain(allowedDomain:"pragimtech.com"
            ,ErrorMessage = "the allowed domain is pragimtech.com")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}