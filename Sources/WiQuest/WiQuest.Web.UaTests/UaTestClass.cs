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
            : base(browser, version, os, screenResultion, deviceName, deviceOrientation, applicationName: "WIQuest.Web") { }


        [Test]
        public void RegisterUserTest()
        {
            Driver.Navigate().GoToUrl(BaseUrl + "/WiQuiz/Development/Dev");
            Driver.FindElement(By.LinkText("Zur Anmeldung/Registrierung »")).Click();
            Driver.FindElement(By.LinkText("Register as a new user")).Click();
            Driver.FindElement(By.Id("UserName")).Clear();
            Driver.FindElement(By.Id("UserName")).SendKeys("TestUser");
            Driver.FindElement(By.Id("Password")).Clear();
            Driver.FindElement(By.Id("Password")).SendKeys("test#12345!ABC");
            Driver.FindElement(By.Id("ConfirmPassword")).Clear();
            Driver.FindElement(By.Id("ConfirmPassword")).SendKeys("test#12345!ABC");
            Driver.FindElement(By.CssSelector("input.btn.btn-default")).Click();
            Driver.Navigate().GoToUrl(BaseUrl + "/WiQuiz/Development/Dev");
            Driver.FindElement(By.LinkText("Zur Anmeldung/Registrierung »")).Click();
            Assert.AreEqual("Hello TestUser!", Driver.FindElement(By.CssSelector("a[title=\"Manage\"]")).Text);
        }

        [Test]
        public void LogoutUserTest()
        {
            Driver.Navigate().GoToUrl(BaseUrl + "/WiQuiz/Development/Dev/Quiz/Home");
            Driver.FindElement(By.LinkText("Log off")).Click();
            Driver.FindElement(By.LinkText("Zur Anmeldung/Registrierung »")).Click();
            Assert.AreEqual("Log in.", Driver.FindElement(By.XPath("//h2")).Text);
        }

        [Test]
        public void LoginUserTest()
        {
            Driver.Navigate().GoToUrl(BaseUrl + "/WiQuiz/Development/Dev");
            Driver.FindElement(By.LinkText("Zur Anmeldung/Registrierung »")).Click();
            Driver.FindElement(By.Id("Email")).Clear();
            Driver.FindElement(By.Id("Email")).SendKeys("TestUser");
            Driver.FindElement(By.Id("Password")).Clear();
            Driver.FindElement(By.Id("Password")).SendKeys("test#12345!ABC");
            Driver.FindElement(By.CssSelector("input.btn.btn-default")).Click();
            Assert.AreEqual("Hello TestUser!", Driver.FindElement(By.CssSelector("a[title=\"Manage\"]")).Text);

        }
    }
}
