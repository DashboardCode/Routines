using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DashboardCode.Routines;
using DashboardCode.AdminkaV1.AuthenticationDom;
using AngleSharp.Html.Dom;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DashboardCode.AdminkaV1.Injected.AspCore.WebApp.IntegrationTest
{
    // TODO: put <PreserveCompilationContext>true</PreserveCompilationContext> to the proj file (instead of copy)
    /// <summary>
    /// Integration testing sample. 
    /// Further readings:
    /// add/remove request/ headers https://andrewlock.net/adding-default-security-headers-in-asp-net-core/ 
    /// hhtps https://andrewlock.net/introduction-to-integration-testing-with-xunit-and-testserver-in-asp-net-core/
    /// </summary>
    [TestClass]
    public class AdminkaIntegrationUnitTest
    {
        readonly CustomWebApplicationFactory<Startup> customWebApplicationFactory;
        readonly TestServer testServer;
        readonly HttpClient httpClient;
        public AdminkaIntegrationUnitTest()
        {
            customWebApplicationFactory = new CustomWebApplicationFactory<Startup>();

            var clientOptions = new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            };
            //clientOptions.BaseAddress = new Uri("http://localhost");
            //clientOptions.HandleCookies = true;
            //clientOptions.MaxAutomaticRedirections = 7;

            httpClient = customWebApplicationFactory.CreateClient(clientOptions);
            testServer = customWebApplicationFactory.Server;
        }

        [TestMethod]
        public async Task TestRoot()
        {
            var response = await testServer
            .CreateRequest("/")
            .SendAsync("GET");

            //response.EnsureSuccessStatusCode();
            //var content = await response.Content.ReadAsStringAsync();
            //Assert.IsTrue(content.Contains("<html"));
            Assert.IsTrue(response.StatusCode == HttpStatusCode.Redirect);
        }

        [TestMethod]
        public async Task TestRoles()
        {
            var response = await testServer
                .CreateRequest("/Auth/Roles")
                .SendAsync("GET");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("<html"));
        }

        [TestMethod]
        public async Task TestRoleDetails()
        {
            var response = await testServer
                .CreateRequest("/Auth/Role?id=1")
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

            var routine = new AdminkaAnonymousRoutineHandlerAsync(
                TestManager.ApplicationSettings,
                loggingTransientsFactory,
                hasVerboseLoggingPrivilege: true,
                new MemberTag(typeof(AdminkaIntegrationUnitTest)),"UnitTest",
                new { input = "Input text" });


            var x = await routine.HandleAsync(async (container, closure) => 
            await container.ResolveAuthenticationDomDbContextHandlerAsync().HandleRepositoryAsync<List<Role>,Role>(repository =>
            {
                //Task.Delay(10000);
                var xx = repository.ListAsync();
                //var xx2 = repository.FindAsync(e => e.RoleId == 2);
                //Task.Delay(10000);
                return xx;
            }));

            await routine.HandleAsync(async (container, closure) => await container.ResolveAuthenticationDomDbContextHandlerAsync().HandleOrmFactoryAsync(async ormHandlerFactory =>
            {
                var routineOrmHandler = ormHandlerFactory.Create<Role>();
                await routineOrmHandler.HandleAsync(async (repository, storage) =>
                {
                    var res = await storage.HandleAsync(async batch =>
                    {
                        var r = await repository.FindAsync(e => e.RoleName == roleName);
                        if (r != null)
                            batch.Remove(r);
                    });
                    if (!res.IsOk())
                        throw new Exception("Can't prepare role");
                });
            }));

            var detailsHttpResponseMessage = await httpClient.GetAsync("/Auth/Role?id=1");
            detailsHttpResponseMessage.EnsureSuccessStatusCode();
            var contentDetails = await detailsHttpResponseMessage.Content.ReadAsStringAsync();
            Assert.IsTrue(contentDetails.Contains("<html"));

            var createHttpResponseMessage = await httpClient.GetAsync("/Auth/RoleCreate");
            createHttpResponseMessage.EnsureSuccessStatusCode();
            httpClient.TransferAntiforgeryCookie(createHttpResponseMessage);


            var content = await HtmlHelpers.GetDocumentAsync(createHttpResponseMessage);
            // Method 1
            var createHtmlDocument = await createHttpResponseMessage.GetDocument();
            var (tokenkKey1, token1) = createHtmlDocument.GetRequestVerificationToken();
            var formData1 = new Dictionary<string, string>
            {
                {tokenkKey1, token1},
                {"Entity.RoleName", roleName},
                {"Groups", "1"},
                {"PrivilegesAllowed", "CFGS,VLOG"},
                {"PrivilegesDenied", "VLOG"}
            };

            var formUrlEncodedContentC = new FormUrlEncodedContent(formData1);
            var postRequest1 = new HttpRequestMessage(HttpMethod.Post, "/Auth/RoleCreate") { Content = formUrlEncodedContentC };

            var createConfirmHttpResponseMessage = await httpClient.SendAsync(postRequest1);
            // Method 2
            //var htmlFormElement = (IHtmlFormElement)content.QuerySelector("form[id='adminka-form-role-create-id']");
            //var submitButton = (IHtmlButtonElement)htmlFormElement.QuerySelector("button[type='submit']");
            //var roleNameHtmlInputElement = (IHtmlInputElement)htmlFormElement.QuerySelector("input[id='edit-role-name-id']");
            //roleNameHtmlInputElement.Value = roleName;
            //var groupsHtmlSelectElement = (IHtmlSelectElement)htmlFormElement.QuerySelector("select[id='edit-groups-id']");
            //var createConfirmHttpResponseMessage = await httpClient.SendAsync(htmlFormElement, submitButton); 

            Assert.IsTrue(createConfirmHttpResponseMessage.StatusCode == HttpStatusCode.Found);
            var location = createConfirmHttpResponseMessage.Headers.Location;

            var listHttpResponseMessage = await httpClient.GetAsync("/Auth/Roles");
            listHttpResponseMessage.EnsureSuccessStatusCode();
            var listHtmlDocument = await listHttpResponseMessage.GetDocument();

            var id = listHtmlDocument.GetTableCell("adminka-table-roles-id", 2, cell => cell == roleName, 1);

            var editHttpResponseMessage = await httpClient.GetAsync("/Auth/RoleEdit?id=" + id);
            editHttpResponseMessage.EnsureSuccessStatusCode();
            var editHtmlDocument = await editHttpResponseMessage.GetDocument();

            var (tokenkKeyE, tokenE) = editHtmlDocument.GetRequestVerificationToken();
            var (rowVersionKeyE, rowVersionE) = editHtmlDocument.GetField("Entity.RowVersion");
            var (roleIdKeyE, roleIdE) = editHtmlDocument.GetField("Entity.RoleId");
            var formDataE = new Dictionary<string, string>
            {
                {tokenkKeyE, tokenE},
                {rowVersionKeyE, rowVersionE},
                {roleIdKeyE, roleIdE},
                {"Entity.RoleName", roleName},
                {"Groups", "1"},
                {"PrivilegesAllowed", "CFGS,VLOG"}
                
            };
            var formUrlEncodedContentE = new FormUrlEncodedContent(formDataE);
            var postRequestEC = new HttpRequestMessage(HttpMethod.Post, "/Auth/RoleEdit") { Content = formUrlEncodedContentE };
            var editConfirmHttpResponseMessage = await httpClient.SendAsync(postRequestEC);
            //var doc = editConfirmHttpResponseMessage.GetDocument();
            Assert.IsTrue(editConfirmHttpResponseMessage.StatusCode == HttpStatusCode.Found);

            var deleteHttpResponseMessage = await httpClient.GetAsync("/Auth/RoleDelete?id=" + id);
            deleteHttpResponseMessage.EnsureSuccessStatusCode();
            var deleteHtmlDocument = await deleteHttpResponseMessage.GetDocument();

            var (tokenkKeyD, tokenD) = deleteHtmlDocument.GetRequestVerificationToken();
            var (rowVersionKeyD, rowVersionD) = deleteHtmlDocument.GetField("Entity.RowVersion");
            var (roleIdKeyD, roleIdD) = deleteHtmlDocument.GetField("Entity.RoleId");

            var formDataD = new Dictionary<string, string>
            {
                {tokenkKeyD, tokenD},
                {rowVersionKeyD, rowVersionD},
                {roleIdKeyD, roleIdD},
            };

            var formUrlEncodedContentD = new FormUrlEncodedContent(formDataD);
            var postRequestD = new HttpRequestMessage(HttpMethod.Post, "/Auth/RoleDelete") { Content = formUrlEncodedContentD };
            var deleteConfirmHttpResponseMessage = await httpClient.SendAsync(postRequestD);
            Assert.IsTrue(deleteConfirmHttpResponseMessage.StatusCode == HttpStatusCode.Found);
        }

        [TestMethod]
        public async Task TestRoleCreateDelete2()
        {
            var roleName = "TestIslandRole1";

            var logger = new List<string>();
            var loggingTransientsFactory = InjectedManager.ComposeListMemberLoggerFactory(logger);

            var routine = new AdminkaAnonymousRoutineHandlerAsync(
                TestManager.ApplicationSettings,
                loggingTransientsFactory,
                hasVerboseLoggingPrivilege: true,
                new MemberTag(typeof(AdminkaIntegrationUnitTest)), "UnitTest",
                new { input = "Input text" });
            await routine.HandleAsync(async (container, closure) => await container.ResolveAuthenticationDomDbContextHandlerAsync().HandleOrmFactoryAsync(async ormHandlerFactory =>
            {
                await ormHandlerFactory.Create<Role>().HandleAsync(async (repository, storage) =>
                {
                    var res = await storage.HandleAsync(async batch =>
                    {
                        var r = await repository.FindAsync(e => e.RoleName == roleName);
                        if (r != null)
                            batch.Remove(r);
                    });
                    if (!res.IsOk())
                        throw new Exception("Can't prepare role");
                });
            }));

            var detailsHttpResponseMessage = await httpClient.GetAsync("/Auth/Role?id=1");
            detailsHttpResponseMessage.EnsureSuccessStatusCode();
            var contentDetails = await detailsHttpResponseMessage.Content.ReadAsStringAsync();
            Assert.IsTrue(contentDetails.Contains("<html"));

            var createHttpResponseMessage = await httpClient.GetAsync("/Auth/RoleCreate");
            createHttpResponseMessage.EnsureSuccessStatusCode();
            httpClient.TransferAntiforgeryCookie(createHttpResponseMessage);
            var createHtmlDocument = await createHttpResponseMessage.GetDocument();

            var (tokenkKey1, token1) = createHtmlDocument.GetRequestVerificationToken();

            var formData1 = new Dictionary<string, string>
            {
                {tokenkKey1, token1},
                {"Entity.RoleName", roleName},
                {"Groups", "1"},
                {"PrivilegesAllowed", "CFGS,VLOG"}
            };

            var formUrlEncodedContentC = new FormUrlEncodedContent(formData1);
            var postRequest1 = new HttpRequestMessage(HttpMethod.Post, "/Auth/RoleCreate") { Content = formUrlEncodedContentC };
            
            //httpClient.BaseAddress = createHttpResponseMessage.RequestMessage.RequestUri;

            var createConfirmHttpResponseMessage = await httpClient.SendAsync(postRequest1);
            Assert.IsTrue(createConfirmHttpResponseMessage.StatusCode == HttpStatusCode.Found);
            var location = createConfirmHttpResponseMessage.Headers.Location;

            var listHttpResponseMessage = await httpClient.GetAsync("/Auth/Roles");
            listHttpResponseMessage.EnsureSuccessStatusCode();
            var listHtmlDocument = await listHttpResponseMessage.GetDocument();

            var id = listHtmlDocument.GetTableCell("adminka-table-roles-id", 2, cell => cell == roleName, 1);

            var editHttpResponseMessage = await httpClient.GetAsync("/Auth/RoleEdit?id=" + id);
            editHttpResponseMessage.EnsureSuccessStatusCode();
            var editHtmlDocument = await editHttpResponseMessage.GetDocument();

            var (tokenkKeyE, tokenE) = editHtmlDocument.GetRequestVerificationToken();
            var (rowVersionKeyE, rowVersionE) = editHtmlDocument.GetField("Entity.RowVersion");
            var (roleIdKeyE, roleIdE) = editHtmlDocument.GetField("Entity.RoleId");
            var formDataE = new Dictionary<string, string>
            {
                {tokenkKeyE, tokenE},
                {rowVersionKeyE, rowVersionE},
                {roleIdKeyE, roleIdE},
                {"Entity.RoleName", roleName},
                {"Groups", "1"},
                {"PrivilegesAllowed", "CFGS"}
            };
            var formUrlEncodedContentE = new FormUrlEncodedContent(formDataE);
            var postRequestEC = new HttpRequestMessage(HttpMethod.Post, "/Auth/RoleEdit") { Content = formUrlEncodedContentE };
            var editConfirmHttpResponseMessage = await httpClient.SendAsync(postRequestEC);
            //var doc = editConfirmHttpResponseMessage.GetDocument();
            Assert.IsTrue(editConfirmHttpResponseMessage.StatusCode == HttpStatusCode.Found);

            var deleteHttpResponseMessage = await httpClient.GetAsync("/Auth/RoleDelete?id=" + id);
            deleteHttpResponseMessage.EnsureSuccessStatusCode();
            var deleteHtmlDocument = await deleteHttpResponseMessage.GetDocument();

            var (tokenkKeyD, tokenD) = deleteHtmlDocument.GetRequestVerificationToken();
            var (rowVersionKeyD, rowVersionD) = deleteHtmlDocument.GetField("Entity.RowVersion");
            var (roleIdKeyD, roleIdD) = deleteHtmlDocument.GetField("Entity.RoleId");

            var formDataD = new Dictionary<string, string>
            {
                {tokenkKeyD, tokenD},
                {rowVersionKeyD, rowVersionD},
                {roleIdKeyD, roleIdD},
            };

            var formUrlEncodedContentD = new FormUrlEncodedContent(formDataD);
            var postRequestD = new HttpRequestMessage(HttpMethod.Post, "/Auth/RoleDelete") { Content = formUrlEncodedContentD };
            var deleteConfirmHttpResponseMessage = await httpClient.SendAsync(postRequestD);
            Assert.IsTrue(deleteConfirmHttpResponseMessage.StatusCode == HttpStatusCode.Found);
        }
    }
}
