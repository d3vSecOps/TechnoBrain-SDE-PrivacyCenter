using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Employees.Hierachy
{
    public interface ICsvReader
    {
      Task<List<Employee>> GetEmployees(string csvFilePath);  
    }
}
