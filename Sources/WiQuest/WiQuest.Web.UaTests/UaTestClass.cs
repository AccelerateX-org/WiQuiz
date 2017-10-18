using System;
using NUnit.Framework;
using OpenQA.Selenium;
using WIQuest.Web.UaTests.Base;

namespace WiQuest.Web.UaTests
{
    [TestFixture("Chrome", "61.0", "Windows 10", "1280x1024", "", "")]
    //[TestFixture("Firefox", "55.0", "Windows 10", "1280x1024", "", "")]
    public class UaTestClass : UaTestBaseClass
    {
        public UaTestClass(string browser, string version, string os, string screenResultion, string deviceName, string deviceOrientation) 
            : base(browser, version, os, screenResultion, deviceName, deviceOrientation, applicationName: "WIQuest.Web") { }


        [Test]
        public void A_RegisterUserTest()
        {
            Driver.Navigate().GoToUrl(BaseUrl);
            Driver.FindElement(By.LinkText("Zur Anmeldung/Registrierung »")).Click();
            Driver.FindElement(By.LinkText("Register as a new user")).Click();
            Driver.FindElement(By.Id("UserName")).Clear();
            Driver.FindElement(By.Id("UserName")).SendKeys("TestUser");
            Driver.FindElement(By.Id("Password")).Clear();
            Driver.FindElement(By.Id("Password")).SendKeys("test#12345!ABC");
            Driver.FindElement(By.Id("ConfirmPassword")).Clear();
            Driver.FindElement(By.Id("ConfirmPassword")).SendKeys("test#12345!ABC");
            Driver.FindElement(By.CssSelector("input.btn.btn-default")).Click();
            Driver.Navigate().GoToUrl(BaseUrl);
            Driver.FindElement(By.LinkText("Zur Anmeldung/Registrierung »")).Click();
            Assert.AreEqual("Hello TestUser!", Driver.FindElement(By.CssSelector("a[title=\"Manage\"]")).Text);
        }

        /*[Test]
        public void B_LogoutUserTest()
        {
            Driver.Navigate().GoToUrl(BaseUrl + "/Quiz/Home");
            Driver.FindElement(By.LinkText("Log off")).Click();
            Driver.FindElement(By.LinkText("Zur Anmeldung/Registrierung »")).Click();
            Assert.AreEqual("Log in.", Driver.FindElement(By.XPath("//h2")).Text);
        }*/

        [Test]
        public void B_LoginUserTest()
        {
            Driver.Navigate().GoToUrl(BaseUrl);
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
