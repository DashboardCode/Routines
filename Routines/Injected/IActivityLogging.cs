using System;

namespace Vse.Routines.Injected
{
    // Note: the alternative to many specialized methods and interfaces could be a "Category" parameter, 
    // but inspite of this Category is potential complexity problem (as it is in MS EntLib)
    public interface IActivityLogging
    {
        void LogActivityStart(DateTime dt);
        void LogActivityFinish(DateTime dt, TimeSpan ts, bool isSuccess);
    }
}
