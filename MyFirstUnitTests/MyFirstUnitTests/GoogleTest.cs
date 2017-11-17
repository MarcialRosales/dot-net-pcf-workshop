using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using FluentAssertions;

namespace MyFirstUnitTests
{
    public class GoogleTest
    {
        [Fact]
        public void search()
        {
            ChromeDriver driver = null;

            try
            {
                driver = new ChromeDriver();
                driver.Navigate().GoToUrl("https://google.com");
                IWebElement searchBox = driver.FindElementByCssSelector("input[type='text']");
                searchBox.Should().NotBeNull();

                driver.Close();
                driver.Dispose();
            }
            catch
            {
                driver.Quit();
            }
        }
    }
}
