using System;
using System.Linq;
using System.Collections.Generic;

namespace DashboardCode.NLogTools.Test
{
    static class LogTestManager
    {
        const int bufferedMessageCountForOperation = 40;
        private static Random random = new Random();
        const int messageMinBytes = 1000;
        const int messageMaxBytes = 2000; // 2kb is a typewritten page

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public class VerboseMessage
        {
            public DateTime DateTime { get; set; }
            public string Message { get; set; }
        }

        public static List<VerboseMessage> Generate(int number= bufferedMessageCountForOperation)
        {
            var list = new List<VerboseMessage>();
            for (var i = 0; i < number; i++)
            {
                list.Add(new VerboseMessage() { DateTime = DateTime.Now, Message = RandomString((i % 2 == 0) ? messageMaxBytes : messageMinBytes) });
            }
            return list;
        }
    }
}
