using Asp.net_Core_Revsion.Models;
using System.Collections.Generic;

namespace Asp.net_Core_Revsion.Repositories
{
    public interface IDepartmentRepository
    {
        IList<Department> GetDepartments();
    }
}