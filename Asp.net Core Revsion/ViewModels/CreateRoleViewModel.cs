using System.ComponentModel.DataAnnotations;

namespace Asp.net_Core_Revsion.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
        [MaxLength(100)]
        [Display(Name = "Role")]
        public string Name { get; set; }
    }
}