using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Employees.Hierachy.Tests
{
    public class GetEmployeeDetailsTest
    {
        private readonly Employee_ _employees;
        readonly string csvFilePath = @"E:/PROJECTS/TechnoBrain/Files/File.csv";
        private Mock<ICsvReader> mock = new Mock<ICsvReader>();

        public GetEmployeeDetailsTest()
        {
            _employees = new Employee_(csvFilePath, mock.Object);
        }

        [Fact]
        public async Task GetEmployeesDetails_ReturnEmployees()
        {
            mock.Setup(x => x.GetEmployees(csvFilePath)).ReturnsAsync(new List<Employee>());
            var result = await _employees.GetEmployeesDetails();
            Assert.Equal(9, result.Count);
        }
    }
}
