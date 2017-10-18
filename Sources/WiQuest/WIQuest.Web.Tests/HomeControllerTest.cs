using System.Web.Mvc;
using NUnit.Framework;
using WIQuest.Web.Controllers;

namespace WIQuest.Web.Tests
{
    [TestFixture]
    public class HomeControllerTest
    {
        [Test]
        public void IsIndexViewed()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void IsContactViewed()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}