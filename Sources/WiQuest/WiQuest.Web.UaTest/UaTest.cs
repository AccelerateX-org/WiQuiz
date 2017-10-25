﻿using System;
using NUnit.Framework;
using OpenQA.Selenium;
using WIQuest.Web.UaTest.Base;

namespace WiQuest.Web.UaTest
{
    [TestFixture("Chrome", "61.0", "Windows 10", "1280x1024", "", "")]
    //[TestFixture("Firefox", "55.0", "Windows 10", "1280x1024", "", "")]
    public class UaTest : UaTestBaseClass
    {
        public UaTest(string browser, string version, string os, string screenResultion, string deviceName, string deviceOrientation) 
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