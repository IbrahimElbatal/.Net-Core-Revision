using Asp.net_Core_Revsion.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Asp.net_Core_Revsion.Controllers
{
    [AllowAnonymous]
    public class MCustomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MCustomController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            //Eager Loading
            //            var result = _context.Employees
            //                .Include(e => e.Department);

            //Explicit Loading
            //            var result = _context.Departments;
            //            foreach (var department in result)
            //            {
            //                _context.Entry(department)
            //                     .Collection<Employee>(d=>d.Employees).Load();
            //            }

            //fixing up
            var result = _context.Departments;
            foreach (var department in result)
            {
                _context.Employees.Where(e => e.DepartmentId == department.Id).Load();
            }
            return View(result);
        }
    }
}