using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Employees.Hierachy.Tests
{
    public class ReadCsvFileLine
    {
        [Fact]
        public void ReadCsvFileLines_Returns_NewEmployee()
        {
            string line = "2,1,1000000";
            var employee = ReadCsvFileLines.EmployeeFromCsvLine(line);
            Assert.Equal("1", employee.ManagerId);
        }
        [Fact]
        public void ReadCsvFileLines_ReturnsNull_WhenInputIsInvalid()
        {
            string line = "1,0100";
            var employee = ReadCsvFileLines.EmployeeFromCsvLine(line);
            Assert.Null(employee);
        }
    }
}
