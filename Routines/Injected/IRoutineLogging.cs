namespace DashboardCode.Routines.Injected
{
    public interface IRoutineLogging
    {
        void LogStart(object input);
        void LogFinish(bool isSuccess, object output);
    }
}
