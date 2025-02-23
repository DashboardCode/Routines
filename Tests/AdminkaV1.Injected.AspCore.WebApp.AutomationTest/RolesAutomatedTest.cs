using Atata;
using NUnit.Framework;

namespace DashboardCode.AdminkaV1.Injected.AspCore.WebApp.AutomationTest
{
    // https://sites.google.com/a/chromium.org/chromedriver/downloads
    // wait till https://github.com/atata-framework/atata/issues/158
    [TestFixture]
    //[Ignore("Ignore a fixture")]

    // Ignore because of error
    // Message: OpenQA.Selenium.DriverServiceNotFoundException : 

    // The file D:\cot\DashboardCode\Routines\Tests\AdminkaV1.Injected.AspCore.WebApp.Automation.NETCore.Test\bin\Debug\netcoreapp3.1 
    // does not exist. The driver can be downloaded at http://chromedriver.storage.googleapis.com/index.html
    // place to D:\cot\DashboardCode\Routines\Tests\AdminkaV1.Injected.AspCore.WebApp.Automation.NETCore.Test\
    public class RolesAutomatedTest
    {
        [SetUp]
        public void SetUp()
        {
            // Find information about AtataContext set-up on https://atata.io/getting-started/
            AtataContext.GlobalConfiguration
                .UseChrome()
                .WithArguments("start-maximized")
                .UseBaseUrl("http://localhost:63557");
        }

        [TearDown]
        public void TearDown()
        {
            AtataContext.Current.Dispose();
        }

        [Test]
        public void CreateRolePage()
        {
            Go.To<CreateRolePage>()
                .Name.Set("TestIsland999").Create.Click();
        }
    }
}
