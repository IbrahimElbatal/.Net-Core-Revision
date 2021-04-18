using System.ComponentModel.DataAnnotations;

namespace Asp.net_Core_Revsion.ViewModels
{
    public class ForegetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}