using NUnit.Framework;
using carEVA.Controllers.API;
using carEVA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading;
using System.Net;
using Microsoft.Owin.Testing;
using System.Runtime.CompilerServices;
using Owin;
using Microsoft.Owin;

namespace carEVA.Test
{
    

    public class testOwinConfig
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute("Default", "api/{controller}/{id}", new { id = RouteParameter.Optional });
            appBuilder.UseWebApi(config);
        }
    }

    [TestFixture]
    public class loginTest
    {
        private carEVAContext db = new carEVAContext();
        private bool testAgainstDB;
        private TestServer owinServer;

        [SetUp]
        public void initTest()
        {
            testAgainstDB = true;
            if (!testAgainstDB)
            {
                Assert.Ignore("Not running user login test against the database");
            }
            owinServer = TestServer.Create<testOwinConfig>();
        }
        [Test]
        public async Task testEvaLogin_DB()
        {
            //Arrange
            //WARNING: the result of this test depends on the actual password on SIDCAR
            evaLogIn data = new evaLogIn { passKey = "Car123456*", user = "nromerob" };
            evaLogInsController apiController = new evaLogInsController(db);
            //start an in memory server
            //HttpConfiguration config = new HttpConfiguration();
            //config.Routes.MapHttpRoute("Default", "api/{controller}/{id}", new { id = RouteParameter.Optional });
            //HttpServer server = new HttpServer(config);

            string json = JsonConvert.SerializeObject(data);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            //Act
            //using (HttpMessageInvoker client = new HttpMessageInvoker(server))
            //{
            //    using (HttpRequestMessage request = new HttpRequestMessage
            //    {
            //        Method = HttpMethod.Post,
            //        RequestUri = new Uri("http://localhost/api/evalogIns"),
            //        Content = content
            //    })
            //    using (HttpResponseMessage response = client.SendAsync(request, CancellationToken.None).Result)
            //    {
            //        HttpContent result = response.Content;
            //        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            //    }
            //}

            //Act using the OWIN server inmemory host
            HttpResponseMessage owinResponse = await owinServer.CreateRequest("/api/evaLogIns")
                .And(req => req.Content = content).PostAsync();

            HttpContent resultContent = owinResponse.Content;
            //Assert

            Assert.AreEqual(HttpStatusCode.NotFound, owinResponse.StatusCode);

        }

        [TearDown]
        public void disposeTest()
        {
            owinServer.Dispose();
        }
    }
}
