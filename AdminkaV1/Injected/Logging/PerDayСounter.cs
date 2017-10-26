using System;
          
namespace DashboardCode.AdminkaV1.Injected.Logging
{
    public class PerDayСounter
    {
        DateTime today = DateTime.Today;
        int count = 0;
        int maxPerDay = 200;

        public bool Increment()
        {
            if (today != DateTime.Today)
            {
                today = DateTime.Today;
                count = 0;
            }
            var @value = false;
            if (count < maxPerDay)
            {
                count++;
                @value = true;
            }
            return @value;
        }
    }
}
