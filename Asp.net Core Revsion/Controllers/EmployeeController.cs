using Asp.net_Core_Revsion.Models;
using Asp.net_Core_Revsion.Repositories;
using Asp.net_Core_Revsion.Utilities;
using Asp.net_Core_Revsion.ViewModels;
using Asp.netCoreRevsion.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Asp.netCoreRevsion.Utilities.TagHelper;

namespace Asp.net_Core_Revsion.Controllers
{
    [ViewComponent(Name = "c")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _repository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IFileManager _fileManager;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(
            IEmployeeRepository repository,
            IDepartmentRepository departmentRepository,
            IFileManager fileManager,
            ILogger<EmployeeController> logger)
        {
            _repository = repository;
            _departmentRepository = departmentRepository;
            _fileManager = fileManager;
            _logger = logger;
        }

        //structure of view component that used in controller and pass model in view data
        public IViewComponentResult Invoke(ISession session)
        {
            _logger.LogInformation(1, "dddd");
            return new ViewViewComponentResult()
            {
                ViewData = new ViewDataDictionary<Employee>(ViewData,
                    session.GetJson<Employee>(""))
            };
        }
        [Authorize(Policy = "Role.Role")]
        public IActionResult Index(int page = 1)
        {
            _logger.LogInformation("Get Data From Database");
            var employees = _repository.GetEmployees();
            var pageInfo = new PageInfo()
            {
                ItemsPerPage = 3,
                TotalItems = employees.Count,
                CurrentPage = page
            };
            var emps = employees.OrderBy(e => e.Id)
                .Skip((page - 1) * pageInfo.ItemsPerPage)
                .Take(pageInfo.ItemsPerPage);

            ViewBag.emps = emps;
            ViewBag.pageInfo = pageInfo;
            return View();
        }
        public IActionResult Details(int id)
        {
            var employee = _repository.GetEmployee(id);
            if (employee == null)
                return NotFound();

            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Departments = new SelectList(
                _departmentRepository.GetDepartments(),
                "Id",
                "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EmployeeViewModel model)
        {
            ViewBag.Departments = new SelectList(
                _departmentRepository.GetDepartments(),
                "Id",
                "Name");

            if (!ModelState.IsValid)
                return View(model);

            var imagePath = _fileManager.ProcessImage(model.FormImage);
            model.Employee.ImagePath = imagePath.Result;
            _repository.SaveEmployee(model.Employee);
            return RedirectToAction("Index");
        }

        //for download image
        [HttpGet("/Images/{image}")]
        public IActionResult GetImage(string image)
        {
            var mime = image.Substring(image.LastIndexOf(".", StringComparison.Ordinal) + 1);
            return File(_fileManager.ImageStream(image), $"images/{mime}");
        }
    }
}