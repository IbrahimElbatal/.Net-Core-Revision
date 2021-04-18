using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Asp.net_Core_Revsion.ViewModels
{
    public class EditRoleViewModel
    {
        [Display(Name = "Role Id")]
        public string Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Role")]
        public string Name { get; set; }

        public IEnumerable<string> Users { get; set; }
    }
}