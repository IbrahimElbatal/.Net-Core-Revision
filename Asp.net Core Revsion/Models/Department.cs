using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Asp.net_Core_Revsion.Models
{
    public class Department
    {
        public Department()
        {
            Employees = new HashSet<Employee>();
        }
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Location { get; set; }

        public IEnumerable<Employee> Employees { get; set; }
    }
}