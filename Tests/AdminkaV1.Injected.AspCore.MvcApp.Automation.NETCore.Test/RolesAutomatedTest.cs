using Atata;
using NUnit.Framework;

namespace AdminkaV1.Injected.AspCore.MvcApp.Automation.NETCore.Test
{
    // https://sites.google.com/a/chromium.org/chromedriver/downloads
    // wait till https://github.com/atata-framework/atata/issues/158
    [TestFixture]
    //[Ignore("Ignore a fixture")]

    // Ignore because of error
    // Message: OpenQA.Selenium.DriverServiceNotFoundException : 
    // The file D:\cot\DashboardCode\Routines\Tests\AdminkaV1.Injected.AspCore.MvcApp.Automation.NETCore.Test\bin\Debug\netcoreapp2.1\chromedriver.exe 
    // does not exist. The driver can be downloaded at http://chromedriver.storage.googleapis.com/index.html
    public class RolesAutomatedTest
    {
        [SetUp]
        public void SetUp()
        {
            // Find information about AtataContext set-up on https://atata-framework.github.io/getting-started/#set-up.
            AtataContext.Configure().
                UseChrome()
                    //.WithDriverPath()
                    //WithArguments("start-maximized").
                    .WithFixOfCommandExecutionDelay()
                    .WithLocalDriverPath().
                UseBaseUrl("http://localhost:63558").
                //UseCulture("en-us").
                //UseNUnitTestName().
                //AddNUnitTestContextLogging().
                //LogNUnitError().
                Build();
        }

        [TearDown]
        public void TearDown()
        {
            AtataContext.Current.CleanUp();
        }

        [Test]
        public void CreateRolePage()
        {
            Go.To<CreateRolePage>()
                .RoleName.Set("TestIsland99").Create.Click();
        }
    }
}
