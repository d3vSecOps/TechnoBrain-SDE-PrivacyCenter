using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using Moq;

namespace Employees.Hierachy.Tests
{
    public class GetEmployeesTest
    {
        string csvFilePath = @"E:/PROJECTS/TechnoBrain/Files/File.csv";
        ReadCsvFile readCsv = new ReadCsvFile();

        [Fact]
        public async Task ReturnEmployeeList()
        {

            var empList = await readCsv.GetEmployees(csvFilePath);
            Assert.Equal(10, empList.Count);
        }

    }
}
