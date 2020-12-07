using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Data;
using System.Text.RegularExpressions;

namespace Employees.Hierachy
{

    public class Employee
    {
        public string empId { get; private set; }
        public string ManagerId { get; private set; }
        public long Salary { get; private set; }

        public Employee() { }

        public bool IsValid { get; private set; } = true;
        public List<Exception> ValidationErrors { get; private set; } = new List<Exception>();

        private Employee(string Id, string managerId, long salary)
        {

            if (string.IsNullOrWhiteSpace(Id))
            {
                throw new ArgumentNullException(nameof(Id));
            }

            if (salary < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(salary));
            }

            empId = Id.Trim();
            ManagerId = managerId;
            Salary = salary;
        }

        public static Employee Create(string Id, string managerId, long salary)
        {
            return new Employee(Id, managerId, salary);
        }

    }

    public class Employee_
    {
        private string csvFilePath = @"E:/PROJECTS/TechnoBrain/Files/File.csv";
        public ICsvReader _csvReader;
        

        public bool IsValid { get; private set; } = true;
        public List<Exception> ValidationErrors { get; private set; } = new List<Exception>();

        public Employee_(string _csvFilePath, ICsvReader csvReader)
        {
            _csvFilePath = string.IsNullOrWhiteSpace(_csvFilePath) ? throw new ArgumentNullException(nameof(_csvFilePath)) : _csvFilePath;
            _csvReader = csvReader;
            _csvFilePath = csvFilePath;
        }

        public Employee_() { }

        public async Task<List<Employee>> GetEmployeesDetails()
        {
            ReadCsvFile readCsv = new ReadCsvFile();
            _Employee _emp = new _Employee();

            var employees = await readCsv.GetEmployees(csvFilePath);
            if (employees != null)
            {
                _emp.ValidateEmployees();

                if (IsValid)
                {
                    return employees;
                }
                throw new AggregateException(ValidationErrors);
            }
            return null;
        }

        public async Task<long> _GetManagerBudget(string managerId)
        {
            ReadCsvFile readCsv = new ReadCsvFile();
            var employees = await readCsv.GetEmployees(csvFilePath);
            if (employees != null)
            {
                _Employee emp = new _Employee(employees);
                return emp.GetManagerBudget(managerId);
            }
            return 0;
        }

    }

    public class _Employee
    {
        private List<Employee> _employee;
        public bool IsValid { get; private set; } = true;
        public List<Exception> ValidationErrors { get; private set; } = new List<Exception>();

        public _Employee(List<Employee> employees)
        {
            _employee = employees ?? throw new ArgumentNullException(nameof(employees));
        }

        public _Employee() { }

        public void ValidateEmployees() => Parallel.Invoke(
                () => { CheckEmployeeMangers(); },
                () => { CheckNumberOfCEOs(); },
                () => { CheckCircularReference(); },
                () => { CheckThatAllManagersAreEmployees(); }
                );

        private void CheckEmployeeMangers()
        {
            foreach (var id in _employee.Select(e => e.empId).Distinct().Where(id => _employee.Where(i => i.empId == id)
            .Select(m => m.ManagerId).Distinct().Count() > 1).Select(id => id))
            {
                IsValid = false;
                ValidationErrors.Add(new Exception($"Employee of Id {id} Has More Than One Manager"));
            }
        }

        private void CheckNumberOfCEOs()
        {
            if (_employee.Where(e => e.ManagerId == string.Empty || e.ManagerId == null).Count() > 1)
            {
                IsValid = false;
                ValidationErrors.Add(new Exception("One CEO Expected - More Than One CEO Detected"));
            }
        }

        private void CheckCircularReference()
        {
            foreach (var _ in from employee in _employee.Where(e => e.ManagerId != string.Empty && e.ManagerId != null)
                              let manager = _employee.Where(e => e.ManagerId != string.Empty && e.ManagerId != null)
                              .FirstOrDefault(e => e.empId == employee.ManagerId)
                              where manager != null
                              where manager.ManagerId == employee.empId
                              select new { })
            {
                IsValid = false;
                ValidationErrors.Add(new Exception("Circular Reference Detected!"));
            }
        }

        private void CheckThatAllManagersAreEmployees()
        {
            foreach (var _ in _employee.Where(r => r.ManagerId != null && r.ManagerId != string.Empty)
                .Select(e => e.ManagerId).Where(manager => _employee.FirstOrDefault(e => e.empId == manager) == null).Select(manager => new { }))
            {
                IsValid = false;
                ValidationErrors.Add(new Exception("Some Managers Are Not Listed As Employees"));
            }
        }

        public long GetManagerBudget(string managerId)
        {
            if (string.IsNullOrWhiteSpace(managerId)) throw new ArgumentNullException(nameof(managerId));
            //if (string.IsNullOrWhiteSpace(managerId)) throw new ArgumentNullException(managerId);

            long total = 0;
            total += _employee.FirstOrDefault(e => e.empId == managerId).Salary;

            foreach (Employee item in _employee.Where(e => e.ManagerId == managerId))
            {
                if (IsManager(item.empId))
                {
                    total += GetManagerBudget(item.empId);
                }
                else
                {
                    //total += item.Salary;
                    total += GetManagerBudget(item.empId);
                }
            }
            return total;
        }

        private bool IsManager(string mgrId) => _employee.Where(e => e.ManagerId == mgrId).Count() > 0;

    }

    public class ReadCsvFile : ICsvReader
    {
        public async Task<List<Employee>> GetEmployees(string csvFilePath)
        {
            return await Task.Run(() =>
            {
                return File.ReadAllLines(csvFilePath).Select(x => x.EmployeeFromCsvLine()).ToList();
            });
        }

        public static int Grouped()
        {
            try
            {
                var lines = new List<string>() {
                "1,,1000000",
                "2,1,300000",
                "3,2,250000",
                "4,3,200000",
                "5,2,100000",
                "6,2,100000",
                "7,3,70000",
                "8,4,50000",
                "9,4,50000",
                "10,4,50000"
            };


            var aList = from l in lines
                        let p = new
                        {
                            empID = l.Split(',')[0],
                            mgrID = l.Split(',')[1],
                            empSalary = Convert.ToInt32(l.Split(',')[2])
                        }
                        select p;

            var vList = from a in aList
                        group a by a.empID into g
                        select new {
                            empID = g.Key,
                            empSalarySum = g.Sum(a => a.empSalary)
                        };

            int? managerBudget = null;
            foreach (var v in vList)
            {
                managerBudget =  v.empSalarySum;
            }

            return (int)managerBudget;
            }
            catch (Exception)
            {

                throw new Exception();
            }
            
        }
        

    }

    public static class ReadCsvFileLines
    {
        static public Employee EmployeeFromCsvLine(this string csvLine)
        {
            string[] parts = csvLine.Split(',');
            if (parts.Length == 3)
            {
                string Id = parts[0];
                string ManagerId = parts[1];
                string Salary = parts[2];
                long.TryParse(Salary, out long salary);
                return Employee.Create(Id, ManagerId, salary);
            }
            return null;
        }
    }
}
