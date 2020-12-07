using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Employees.Hierachy;
using System.Data;

namespace Employees.Hierachy.Tests
{
    public class GetManagerBudgetTest
    {
        string csvFilePath = @"E:/PROJECTS/TechnoBrain/Files/File.csv";
        private Employee_ emp = new Employee_();
        private Employee empl = new Employee();
        private readonly Mock<ICsvReader> mock = new Mock<ICsvReader>();


        [Theory]
        [InlineData("2", 300000)]
        [InlineData("3", 250000)]
        [InlineData("4", 200000)]
        public async Task GetManagerBudget(string empId, long budget)
        {
            budget = ReadCsvFile.Grouped();
            mock.Setup(x => x.GetEmployees(csvFilePath)).ReturnsAsync(new List<Employee>());
            var result =  await emp._GetManagerBudget(empId);
            var mgrBudget = ReadCsvFile.Grouped();
            Assert.Equal(mgrBudget, budget);
        }
    }
}
