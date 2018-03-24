using BenchmarkDotNet.Running;

namespace BenchmarkAdminka
{
    class Program
    {
        static void Main(string[] args)
        {
            var b = new BenchmarkAdminkaRoutineNLogLogger();
            //b.MeasureRoutineRepositoryExceptionNLog();
            //b.MeasureRoutineNLog();
            //b.MeasureRoutineNoAuthorizationNLog();
            //BenchmarkRunner.Run<BenchmarkAdminkaRoutineListLogger>();
            //BenchmarkRunner.Run<BenchmarkAdminkaRoutineNLogLogger>();
        }
    }
}