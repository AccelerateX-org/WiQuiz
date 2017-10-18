using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace WIQuest.Web.UaTests.Base
{
    public class UaTestBaseClass
    {
        private IWebDriver _driver;
        public IWebDriver Driver { get;  private set; }

        private readonly bool _isLocalMode;

        private readonly string _sauceLabsUserName;
        private readonly string _sauceLabsAccessKey;

        private readonly string _browser;
        private readonly string _version;
        private readonly string _os;
        private readonly string _deviceName;
        private readonly string _deviceOrientation;
        private readonly string _screenResultion;

        public UaTestBaseClass(string browser, string version, string os, string screenResultion, string deviceName, string deviceOrientation)
        {
            _browser = browser;
            _version = version;
            _os = os;
            _screenResultion = screenResultion;
            _deviceName = deviceName;
            _deviceOrientation = deviceOrientation;

            try
            {
                _sauceLabsUserName = Environment.GetEnvironmentVariable("SL_USERNAME");
                _sauceLabsAccessKey = Environment.GetEnvironmentVariable("SL_API_KEY");

                if (string.IsNullOrEmpty(_sauceLabsUserName) || string.IsNullOrEmpty(_sauceLabsAccessKey))
                {
                    _isLocalMode = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(@"Exception while obtaining SauceLabs credentials. Error while obtaining Environment Variables..." + e);
            }
        }

        [SetUp]
        public void Init()
        {
            var caps = new DesiredCapabilities();
            caps.SetCapability(CapabilityType.BrowserName, _browser);
            caps.SetCapability(CapabilityType.Version, _version);
            caps.SetCapability(CapabilityType.Platform, _os);
            caps.SetCapability("screenResolution", _screenResultion);
            caps.SetCapability("deviceName", _deviceName);
            caps.SetCapability("deviceOrientation", _deviceOrientation);
            //caps.SetCapability("username", "ax_team");
            //caps.SetCapability("accessKey", "8b2e1a39-dcc0-48a0-a704-89f1182decf8");
            caps.SetCapability("username", _sauceLabsUserName);
            caps.SetCapability("accessKey", _sauceLabsAccessKey);
            caps.SetCapability("name", TestContext.CurrentContext.Test.Name);

            try
            {
                _driver = !_isLocalMode ? new RemoteWebDriver(new Uri("http://ondemand.saucelabs.com:80/wd/hub"), caps, TimeSpan.FromSeconds(600)) : new ChromeDriver();
            }
            catch (Exception e)
            {
                Console.WriteLine(@"Exception while starting WebDriver..." + e);
            }

            Driver = _driver;
        }

        [TearDown]
        public void CleanUp()
        {
            if (_isLocalMode)
            {
                _driver.Quit();
                return;
            }

            var passed = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed;

            try
            {
                ((IJavaScriptExecutor)_driver).ExecuteScript("sauce:job-result=" + (passed ? "passed" : "failed"));
            }
            finally
            {
                _driver.Quit();
            }
        }
    }
}
