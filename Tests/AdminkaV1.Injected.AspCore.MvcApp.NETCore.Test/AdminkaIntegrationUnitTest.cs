using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DashboardCode.Routines;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.NETCore.Test
{
    // TODO: put <PreserveCompilationContext>true</PreserveCompilationContext> to the proj file (instead of copy)
    /// <summary>
    /// Integration unit tests. 
    /// Further readings:
    /// add/remove request/ headers https://andrewlock.net/adding-default-security-headers-in-asp-net-core/ 
    /// hhtps https://andrewlock.net/introduction-to-integration-testing-with-xunit-and-testserver-in-asp-net-core/
    /// </summary>
    [TestClass]
    public class AdminkaIntegrationUnitTest
    {
        readonly TestServer testServer;
        readonly HttpClient httpClient;
        public AdminkaIntegrationUnitTest()
        {
            var hostBuilder = new WebHostBuilder()
                .UseContentRoot(TestManager.GetContentRoot())
                // TODO: should I use it and when?
                //  .UseEnvironment("Development")
                .ConfigureServices(TestManager.InitializeServices)
                .UseStartup<Startup>()
                // TODO: should I do configuration in place (or trust that one from Start)
                // to overwrite configuration for test
                /*.UseConfiguration(
                    new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                    .SetBasePath(System.IO.Path.GetFullPath(@"../../../../APIProjectFolder/"))
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddUserSecrets<Startup>()
                    .Build()
                )*/;
            testServer = new TestServer(hostBuilder);

            httpClient = testServer.CreateClient();
        }

        [TestMethod]
        public async Task TestRoot()
        {
            var response = await testServer
                .CreateRequest("/")
                .SendAsync("GET");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("<html"));
        }

        [TestMethod]
        public async Task TestRoles()
        {
            var response = await testServer
                .CreateRequest("/Roles")
                .SendAsync("GET");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("<html"));
        }

        [TestMethod]
        public async Task TestRoleDetails()
        {
            var response = await testServer
                .CreateRequest("/Roles/Details/1")
                .SendAsync("GET");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("<html"));
        }

        [TestMethod]
        public async Task TestRoleCreateDelete()
        {
            var roleName = "TestIslandRole1";

            var logger = new List<string>();
            var loggingTransientsFactory = InjectedManager.ComposeListMemberLoggerFactory(logger);

            var routine = new AdminkaRoutineHandler(
                TestManager.ApplicationSettings,
                loggingTransientsFactory,
                new MemberTag(typeof(AdminkaIntegrationUnitTest)), new UserContext("UnitTest"),
                new { input = "Input text" });
            routine.StorageRoutineHandler.HandleOrmFactory((ormHandlerFactory) =>
            {
                ormHandlerFactory.Create<Role>().Handle((repository, storage) =>
                {
                    var res = storage.Handle(batch =>
                     {
                         var r = repository.Find(e => e.RoleName == roleName);
                         if (r != null)
                             batch.Remove(r);
                     });
                    if (!res.IsOk())
                        throw new Exception("Can't prepare role");
                });
            });

            var detailsHttpResponseMessage = httpClient.GetAsync("/Roles/Details/1").Result;
            detailsHttpResponseMessage.EnsureSuccessStatusCode();
            var contentDetails = await detailsHttpResponseMessage.Content.ReadAsStringAsync();
            Assert.IsTrue(contentDetails.Contains("<html"));

            var createHttpResponseMessage = httpClient.GetAsync("/Roles/Create").Result;
            createHttpResponseMessage.EnsureSuccessStatusCode();
            httpClient.TransferAntiforgeryCookie(createHttpResponseMessage);
            var createHtmlDocument = await createHttpResponseMessage.GetDocument();

            var (tokenkKey1, token1) = createHtmlDocument.GetRequestVerificationToken();

            var formData1 = new Dictionary<string, string>
            {
                {tokenkKey1, token1},
                {"RoleName", roleName},
                {"Groups", "1"},
                {"Privileges", "CFGS,VLOG"},
            };

            var formUrlEncodedContentC = new FormUrlEncodedContent(formData1);
            var postRequest1 = new HttpRequestMessage(HttpMethod.Post, "/Roles/Create") { Content = formUrlEncodedContentC };

            var createConfirmHttpResponseMessage = await httpClient.SendAsync(postRequest1);
            Assert.IsTrue(createConfirmHttpResponseMessage.StatusCode == HttpStatusCode.Found);
            var location = createConfirmHttpResponseMessage.Headers.Location;

            var listHttpResponseMessage = httpClient.GetAsync("/Roles").Result;
            listHttpResponseMessage.EnsureSuccessStatusCode();
            var listHtmlDocument = await listHttpResponseMessage.GetDocument();

            var id = listHtmlDocument.GetTableCell("adminka-roles-table-id", 2, cell => cell == roleName, 1);

            var editHttpResponseMessage = httpClient.GetAsync("/Roles/Edit/" + id).Result;
            editHttpResponseMessage.EnsureSuccessStatusCode();
            var editHtmlDocument = await editHttpResponseMessage.GetDocument();

            var (tokenkKeyE, tokenE) = editHtmlDocument.GetRequestVerificationToken();
            var (rowVersionKeyE, rowVersionE) = editHtmlDocument.GetField("RowVersion");
            var (roleIdKeyE, roleIdE) = editHtmlDocument.GetField("RoleId");
            var formDataE = new Dictionary<string, string>
            {
                {tokenkKeyE, tokenE},
                {rowVersionKeyE, rowVersionE},
                {roleIdKeyE, roleIdE},
                {"RoleName", roleName},
                {"Groups", "1"},
                {"Privileges", "CFGS"},
            };
            var formUrlEncodedContentE = new FormUrlEncodedContent(formDataE);
            var postRequestEC = new HttpRequestMessage(HttpMethod.Post, "/Roles/Edit") { Content = formUrlEncodedContentE };
            var editConfirmHttpResponseMessage = await httpClient.SendAsync(postRequestEC);
            //var doc = editConfirmHttpResponseMessage.GetDocument();
            Assert.IsTrue(editConfirmHttpResponseMessage.StatusCode == HttpStatusCode.Found);

            var deleteHttpResponseMessage = httpClient.GetAsync("/Roles/Delete/" + id).Result;
            deleteHttpResponseMessage.EnsureSuccessStatusCode();
            var deleteHtmlDocument = await deleteHttpResponseMessage.GetDocument();

            var (tokenkKeyD, tokenD) = deleteHtmlDocument.GetRequestVerificationToken();
            var (rowVersionKeyD, rowVersionD) = deleteHtmlDocument.GetField("RowVersion");
            var (roleIdKeyD, roleIdD) = deleteHtmlDocument.GetField("RoleId");

            var formDataD = new Dictionary<string, string>
            {
                {tokenkKeyD, tokenD},
                {rowVersionKeyD, rowVersionD},
                {roleIdKeyD, roleIdD},
            };

            var formUrlEncodedContentD = new FormUrlEncodedContent(formDataD);
            var postRequestD = new HttpRequestMessage(HttpMethod.Post, "/Roles/Delete") { Content = formUrlEncodedContentD };
            var deleteConfirmHttpResponseMessage = await httpClient.SendAsync(postRequestD);
            Assert.IsTrue(deleteConfirmHttpResponseMessage.StatusCode == HttpStatusCode.Found);
        }

        [TestMethod]
        public async Task TestRoleCreateDelete2()
        {
            var roleName = "TestIslandRole1";

            var logger = new List<string>();
            var loggingTransientsFactory = InjectedManager.ComposeListMemberLoggerFactory(logger);

            var routine = new AdminkaRoutineHandler(
                TestManager.ApplicationSettings,
                loggingTransientsFactory,
                new MemberTag(typeof(AdminkaIntegrationUnitTest)), new UserContext("UnitTest"),
                new { input = "Input text" });
            routine.StorageRoutineHandler.HandleOrmFactory((ormHandlerFactory) =>
            {
                ormHandlerFactory.Create<Role>().Handle((repository, storage) =>
                {
                    var res = storage.Handle(batch =>
                    {
                        var r = repository.Find(e => e.RoleName == roleName);
                        if (r != null)
                            batch.Remove(r);
                    });
                    if (!res.IsOk())
                        throw new Exception("Can't prepare role");
                });
            });

            var detailsHttpResponseMessage = httpClient.GetAsync("/Roles/Details/1").Result;
            detailsHttpResponseMessage.EnsureSuccessStatusCode();
            var contentDetails = await detailsHttpResponseMessage.Content.ReadAsStringAsync();
            Assert.IsTrue(contentDetails.Contains("<html"));

            var createHttpResponseMessage = httpClient.GetAsync("/Roles/Create").Result;
            createHttpResponseMessage.EnsureSuccessStatusCode();
            httpClient.TransferAntiforgeryCookie(createHttpResponseMessage);
            var createHtmlDocument = await createHttpResponseMessage.GetDocument();

            var (tokenkKey1, token1) = createHtmlDocument.GetRequestVerificationToken();

            var formData1 = new Dictionary<string, string>
            {
                {tokenkKey1, token1},
                {"RoleName", roleName},
                {"Groups", "1"},
                {"Privileges", "CFGS,VLOG"},
            };

            var formUrlEncodedContentC = new FormUrlEncodedContent(formData1);
            var postRequest1 = new HttpRequestMessage(HttpMethod.Post, "/Roles/Create") { Content = formUrlEncodedContentC };

            var createConfirmHttpResponseMessage = await httpClient.SendAsync(postRequest1);
            Assert.IsTrue(createConfirmHttpResponseMessage.StatusCode == HttpStatusCode.Found);
            var location = createConfirmHttpResponseMessage.Headers.Location;

            var listHttpResponseMessage = httpClient.GetAsync("/Roles").Result;
            listHttpResponseMessage.EnsureSuccessStatusCode();
            var listHtmlDocument = await listHttpResponseMessage.GetDocument();

            var id = listHtmlDocument.GetTableCell("adminka-roles-table-id", 2, cell => cell == roleName, 1);

            var editHttpResponseMessage = httpClient.GetAsync("/Roles/Edit/" + id).Result;
            editHttpResponseMessage.EnsureSuccessStatusCode();
            var editHtmlDocument = await editHttpResponseMessage.GetDocument();

            var (tokenkKeyE, tokenE) = editHtmlDocument.GetRequestVerificationToken();
            var (rowVersionKeyE, rowVersionE) = editHtmlDocument.GetField("RowVersion");
            var (roleIdKeyE, roleIdE) = editHtmlDocument.GetField("RoleId");
            var formDataE = new Dictionary<string, string>
            {
                {tokenkKeyE, tokenE},
                {rowVersionKeyE, rowVersionE},
                {roleIdKeyE, roleIdE},
                {"RoleName", roleName},
                {"Groups", "1"},
                {"Privileges", "CFGS"},
            };
            var formUrlEncodedContentE = new FormUrlEncodedContent(formDataE);
            var postRequestEC = new HttpRequestMessage(HttpMethod.Post, "/Roles/Edit") { Content = formUrlEncodedContentE };
            var editConfirmHttpResponseMessage = await httpClient.SendAsync(postRequestEC);
            //var doc = editConfirmHttpResponseMessage.GetDocument();
            Assert.IsTrue(editConfirmHttpResponseMessage.StatusCode == HttpStatusCode.Found);

            var deleteHttpResponseMessage = httpClient.GetAsync("/Roles/Delete/" + id).Result;
            deleteHttpResponseMessage.EnsureSuccessStatusCode();
            var deleteHtmlDocument = await deleteHttpResponseMessage.GetDocument();

            var (tokenkKeyD, tokenD) = deleteHtmlDocument.GetRequestVerificationToken();
            var (rowVersionKeyD, rowVersionD) = deleteHtmlDocument.GetField("RowVersion");
            var (roleIdKeyD, roleIdD) = deleteHtmlDocument.GetField("RoleId");

            var formDataD = new Dictionary<string, string>
            {
                {tokenkKeyD, tokenD},
                {rowVersionKeyD, rowVersionD},
                {roleIdKeyD, roleIdD},
            };

            var formUrlEncodedContentD = new FormUrlEncodedContent(formDataD);
            var postRequestD = new HttpRequestMessage(HttpMethod.Post, "/Roles/Delete") { Content = formUrlEncodedContentD };
            var deleteConfirmHttpResponseMessage = await httpClient.SendAsync(postRequestD);
            Assert.IsTrue(deleteConfirmHttpResponseMessage.StatusCode == HttpStatusCode.Found);
        }
    }
}
