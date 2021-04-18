using Asp.net_Core_Revsion.Models;
using System.Collections.Generic;
using System.Linq;

namespace Asp.net_Core_Revsion.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;

        public DepartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public IList<Department> GetDepartments()
        {
            return _context.Departments.ToList();
        }
    }
}