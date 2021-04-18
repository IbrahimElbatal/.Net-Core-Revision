using Asp.net_Core_Revsion.Models;
using System.Collections.Generic;

namespace Asp.net_Core_Revsion.Repositories
{
    public interface IEmployeeRepository
    {
        IList<Employee> GetEmployees();
        Employee GetEmployee(int id);
        void SaveEmployee(Employee employee);
    }
}