using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Asp.net_Core_Revsion.ViewModels
{
    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            Roles = new List<string>();
            Claims = new List<string>();
        }

        [Display(Name = "User Id")]
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<string> Claims { get; set; }
    }
}