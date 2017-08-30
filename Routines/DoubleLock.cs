using System;

namespace DashboardCode.Routines
{
    /// <summary>
    /// public static DoubleLock<Include<Privilege>>  indexIncludes2Locked = new DoubleLock<Include<Privilege>>();
    /// </summary>
    public class DoubleLock
    {
        private readonly object lockKey = new object();
        object t;
        public T Get<T>(Func<T> constructor)
        {
            if (t != null)
                return (T)t;
            lock (lockKey)
            {
                if (t != null)
                    return (T)t;
                t = constructor();
                return (T)t;
            }
        }
    }
}