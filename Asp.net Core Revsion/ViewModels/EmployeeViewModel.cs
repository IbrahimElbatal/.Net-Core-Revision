using Asp.net_Core_Revsion.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Asp.net_Core_Revsion.ViewModels
{
    public class EmployeeViewModel
    {
        public Employee Employee { get; set; }

        [Required]
        [Display(Name = "Image")]
        public IFormFile FormImage { get; set; }
    }
}