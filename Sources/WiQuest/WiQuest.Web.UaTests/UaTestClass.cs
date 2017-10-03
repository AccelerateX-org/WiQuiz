using System;
using NUnit.Framework;
using OpenQA.Selenium;
using WIQuest.Web.UaTests.Base;

namespace WiQuest.Web.UaTests
{
    [TestFixture("Chrome", "61.0", "Windows 10", "1920x1080", "", "")]
    [TestFixture("Firefox", "55.0", "Windows 10", "1920x1080", "", "")]
    public class UaTestClass : UaTestBaseClass
    {
        public UaTestClass(string browser, string version, string os, string screenResultion, string deviceName, string deviceOrientation) 
            : base(browser, version, os, screenResultion, deviceName, deviceOrientation) { }

        [Test]
        public void Should_find_search_box()
        {
            Driver.Navigate().GoToUrl("https://www.google.de");

            Console.WriteLine(Driver.Title);

            var query = Driver.FindElement(By.Name("q"));
            query.SendKeys("TestingBot");
            query.Submit();

            Console.WriteLine(Driver.Title);
        }
    }
}
