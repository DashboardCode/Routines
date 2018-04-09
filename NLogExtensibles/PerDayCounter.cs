using System;

namespace DashboardCode.NLogTools
{
    public class PerDayСounter
    {
        DateTime today = DateTime.Today;
        long count = 0;

        public long Count()
        {
            if (today != DateTime.Today)
            {
                today = DateTime.Today;
                count = 0;
            }
            count++;
            return count;
        }
    }
}