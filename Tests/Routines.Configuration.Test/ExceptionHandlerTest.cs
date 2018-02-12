namespace DashboardCode.Routines.Configuration.Test
{
#if !(NETCOREAPP1_1 || NETCOREAPP2_0)
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
#endif
    public partial class ExceptionHandlerTest
    {

#if NETCOREAPP1_1 || NETCOREAPP2_0
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