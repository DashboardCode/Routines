using BenchmarkDotNet.Running;

namespace BenchmarkAdminka
{
    class Program
    {
        static void Main(string[] args)
        {
            //var b = new BenchmarkAdminkaRoutineListLogger();
            //b.MeasureRoutineRepositoryErrorLogList();
            //var b = new BenchmarkAdminkaRoutineNLogLogger();
            //b.MeasureRoutineRepositoryErrorNLog();
            //b.MeasureRoutineRepositoryNLog();
            //b.MeasureRoutineRepositoryExceptionMailNLog();
            //b.MeasureRoutineNLog();
            //b.MeasureRoutineNoAuthorizationNLog();
            BenchmarkRunner.Run<BenchmarkAdminkaRoutineListLogger>();
            BenchmarkRunner.Run<BenchmarkAdminkaRoutineNLogLogger>();
        }
    }
}