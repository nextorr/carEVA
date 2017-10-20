using NUnit.Framework;
using carEVA.Controllers.API;
using carEVA.Utils;
using carEVA.Models;
using System.Threading;
using System.Net;
using System.Web.Http.Results;
using System.Collections.Generic;

namespace carEVA.Test
{
    [TestFixture]
    public class apiCourseCatalogControllerTest
    {
        private carEVAContext db = new carEVAContext();
        [Test]
        public void Base_getcourses_functionality_database()
        {
            //Arrange
            var apiController = new courseCatalogController(db);
            apiController.Request = new System.Net.Http.HttpRequestMessage();
            apiController.Configuration = new System.Web.Http.HttpConfiguration();
            //Act
            var result = apiController.GetCourses(userUtils.getValidPublicKey(db));
            var response = result.ExecuteAsync(CancellationToken.None).Result;
            //Assert
            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(result);
        }

        [Test]
        public void Getcourses_Returns_Organization_Course_database()
        {
            //Arrange
            var apiController = new courseCatalogController(db);
            apiController.Request = new System.Net.Http.HttpRequestMessage();
            apiController.Configuration = new System.Web.Http.HttpConfiguration();

            //Act
            var result = apiController.GetCourses(userUtils.getValidPublicKey(db));
            var contentResult = result as OkNegotiatedContentResult<List<evaOrganizationCourse>>;
            //Assert
            Assert.IsNotNull(contentResult,"base");
            Assert.IsNotNull(contentResult.Content,"content");
            Assert.IsNotNull(contentResult.Content[0].evaOrganizationID, "organization");
        }
    }
    
}
