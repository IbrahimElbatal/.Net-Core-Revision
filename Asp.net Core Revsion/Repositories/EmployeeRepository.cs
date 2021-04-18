using Asp.net_Core_Revsion.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Asp.net_Core_Revsion.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Employee> GetEmployees()
        {
            return _context.Employees
                .Include(e => e.Department)
                .ToList();
        }

        public Employee GetEmployee(int id)
        {
            return _context.Employees.FirstOrDefault(e => e.Id == id);
        }

        public void SaveEmployee(Employee employee)
        {
            //            var entry = _context.Add(new Employee());
            //            entry.CurrentValues.SetValues(employee);
            //            _context.SaveChanges();


            var employeeToSave = new Employee()
            {
                Name = employee.Name,
                Email = employee.Email,
                DepartmentId = employee.DepartmentId,
                ImagePath = employee.ImagePath
            };
            _context.Employees.Add(employeeToSave);
            _context.SaveChanges();
        }
    }
}