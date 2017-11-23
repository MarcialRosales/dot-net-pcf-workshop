using System;
using System.Threading;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.PhantomJS;
using Xunit;

namespace MyFirstUnitTests
{
    public class GithubTest : IDisposable
    {
        public GithubTest()
        {
            // UsePhatomJS();
            UseChrome();
            // UseInternetExplorer();
        }

        private void UseInternetExplorer()
        {
            _driver = new InternetExplorerDriver("C:/Users/vagrant"); 
        }
        private void UsePhatomJS()
        {
            PhantomJSOptions options = new PhantomJSOptions();
            options.AddAdditionalCapability("phantomjs.page.settings.userAgent",
                "Mozilla/5.0 (iPad; CPU OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A5355d Safari/8536.25");
            _driver = new PhantomJSDriver(options);
        }

        private void UseChrome()
        {
            var options = new ChromeOptions();
            options.AddArgument("headless");
            _driver = new ChromeDriver(options);
        }
        public void Dispose()
        {
            try
            {
                _driver.Close();
                _driver.Dispose();
            }
            catch
            {
                _driver.Quit();
            }
        }

        private IWebDriver _driver;


        [Fact]
        public void searchResultPageTitleShouldContainTheSearchCriteria()
        {
            var home = new HomePage(_driver)
                .Go()
                .search("concourse")
                .title.Should().Be("Search · concourse · GitHub");
            
        }

        abstract class BasePage
        {
            protected IWebDriver _driver;

            public BasePage(IWebDriver webDriver)
            {
                _driver = webDriver;

            }

            public String title
            {
                get { return _driver.Title; }
                set { }
            }
            
        }
        class SearchResultPage : BasePage
        {
            public SearchResultPage(IWebDriver webDriver) : base(webDriver)
            {
            }
        }
        class HomePage : BasePage
        {
            public HomePage(IWebDriver webDriver) : base(webDriver)
            { 
            }
            public HomePage Go()               
            {
                _driver.Navigate().GoToUrl("https://github.com/MarcialRosales");
                return this;
            }
            public SearchResultPage search(String criteria)
            {
                typeCriteria(criteria);
                return new SearchResultPage(_driver);
            }

            private HomePage typeCriteria(String criteria)
            {
                var search = _driver.FindElement(By.CssSelector("input[name=q]"));
                search.Displayed.Should().BeTrue();
                search.SendKeys(criteria);
                search.SendKeys(Keys.Enter);

                return this;
            }
        }
    }

}