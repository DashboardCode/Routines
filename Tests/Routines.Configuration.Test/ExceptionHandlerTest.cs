namespace DashboardCode.Routines.Configuration.Test
{
#if !NETCOREAPP
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
#endif
    public partial class ExceptionHandlerTest
    {

#if NETCOREAPP
        [Xunit.Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void ExceptionHandlerStackTrace()
        {
            TestMethod();
        }
    }
}