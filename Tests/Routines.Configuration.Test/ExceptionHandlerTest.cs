namespace DashboardCode.Routines.Configuration.Test
{
#if !(NETCOREAPP1_1 || NETCOREAPP2_0  || NETCOREAPP2_1)
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
#endif
    public partial class ExceptionHandlerTest
    {

#if NETCOREAPP1_1 || NETCOREAPP2_0  || NETCOREAPP2_1
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