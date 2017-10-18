using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using Xunit;

namespace WIQuest.Web.Tests
{

    public class UATestClass : IDisposable
    {
        private IWebDriver _driver;

        public UATestClass()
        {
            DesiredCapabilities caps = new DesiredCapabilities();
            caps.SetCapability(CapabilityType.BrowserName, "chrome");
            caps.SetCapability(CapabilityType.Version, 60);
            caps.SetCapability(CapabilityType.Platform, "Windows 10");
            caps.SetCapability("deviceName", "");
            caps.SetCapability("deviceOrientation", "");
            caps.SetCapability("username", "ax_team");
            caps.SetCapability("accessKey", "8b2e1a39-dcc0-48a0-a704-89f1182decf8");
            caps.SetCapability("name", "UATest");

            try
            {
                //_driver = new ChromeDriver();
                _driver = new RemoteWebDriver(new Uri("http://ondemand.saucelabs.com:80/wd/hub"), caps,
                    TimeSpan.FromSeconds(600));
            }
            catch (Exception e)
            {
                Console.WriteLine(@"Exception while starting chrome..." + e);
            }
        }

        public void Dispose()
        {
            _driver.Dispose();
        }

        [Fact]
        public void Should_find_search_box()
        {
            _driver.Navigate().GoToUrl("https://www.google.de");
            Console.WriteLine(_driver.Title);
            IWebElement query = _driver.FindElement(By.Name("q"));
            query.SendKeys("TestingBot1");
            query.Submit();
            Console.WriteLine(_driver.Title);
            _driver.Quit();
        }

        [Fact]
        public void Should_find_search_box_2()
        {
            _driver.Navigate().GoToUrl("https://www.google.de");
            Console.WriteLine(_driver.Title);
            IWebElement query = _driver.FindElement(By.Name("q"));
            query.SendKeys("TestingBot2");
            query.Submit();
            Console.WriteLine(_driver.Title);
            _driver.Quit();
        }
    }
}
