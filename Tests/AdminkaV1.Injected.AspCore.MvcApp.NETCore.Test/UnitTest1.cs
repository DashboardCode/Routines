using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Controllers;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.NETCore.Test
{
    [TestClass]
    public class AdminkaIntegrationUnitTest
    {
        readonly TestServer server;
        readonly HttpClient client = new HttpClient();

        public AdminkaIntegrationUnitTest()
        {
            server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            client = server.CreateClient();
        }

        [TestMethod]
        public async void RolesIndex()
        {
            var httpRequestMessage = new HttpRequestMessage();
            var response = await client.SendAsync(httpRequestMessage);

            //var testSession = Startup.GetTestSession();

            IConfigurationRoot configurationRoot=null;
            var controller = new RolesController(configurationRoot);

            var result = await controller.Index();

            //var viewResult = Assert.IsInstanceOfType<List<>>(result);
            //var model = Assert.IsAssignableFrom<IEnumerable<StormSessionViewModel>>(
            //    viewResult.ViewData.Model);
            //Assert..Equal(2, model.Count());
        }
    }
}
