using System;
using WebApplication5.Models.ExcelFiles;
using Xunit;

namespace TestProject1
{
    public class ExcelFileTest
    {
        [Fact]
        public void RegexTest1()
        {
            //Arrange
            var excelFile = new ExcelFile();
            string projectNumber = "1092-»Õ Œ (2997)";


            //Act
            var projectNumParsed = excelFile.GetProjectNumber(projectNumber);

            //Assert
            Assert.Equal("2997", projectNumParsed);
        }

        [Fact]
        public void RegexTest2()
        {
            //Arrange
            var excelFile = new ExcelFile();
            string projectNumber = "1092-»Õ Œ (2997) gggg";


            //Act
            var projectNumParsed = excelFile.GetProjectNumber(projectNumber);

            //Assert
            Assert.Equal("2997", projectNumParsed);
        }

        [Fact]
        public void RegexTest3()
        {
            //Arrange
            var excelFile = new ExcelFile();
            string projectNumber = "(2997) 1092-»Õ Œ  gggg";


            //Act
            var projectNumParsed = excelFile.GetProjectNumber(projectNumber);

            //Assert
            Assert.Equal("2997", projectNumParsed);
        }

        [Fact]
        public void RegexTest4()
        {
            //Arrange
            var excelFile = new ExcelFile();
            string projectNumber = "1092-»Õ Œ  gggg";


            //Act
            var projectNumParsed = excelFile.GetProjectNumber(projectNumber);

            //Assert
            Assert.Equal(projectNumParsed, projectNumber);
        }

    }
}
