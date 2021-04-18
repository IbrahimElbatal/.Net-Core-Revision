using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asp.net_Core_Revsion.Models
{
    public class Employee
    {
        public Employee()
        {
            ImagePath = "xx";
        }
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z]+@[A-Za-z0-9]+\.[A-Za-z]+$")]
        public string Email { get; set; }

        [Required]
        public string ImagePath { get; set; }
        [ForeignKey("Department")]
        [Required]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }
        public Department Department { get; set; }

    }
}