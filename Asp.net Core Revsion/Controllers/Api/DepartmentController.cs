
using Asp.net_Core_Revsion.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Asp.net_Core_Revsion.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentRepository _repository;

        public DepartmentController(IDepartmentRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Get()
        {
            return Ok(_repository.GetDepartments());
        }
    }
}