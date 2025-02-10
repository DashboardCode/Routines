namespace DashboardCode.Routines.Configuration.Test
{
#if !NET9_0_OR_GREATER
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
#endif
    public partial class ExceptionHandlerTest
    {

#if NET9_0_OR_GREATER
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