using System;

namespace AZ_Server
{
    public class AZLOG : TSingleton<AZLOG>
    {
        private static void MSG(string msg)
        {
            Console.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd-HH-mm-ss]-- ") + msg);
        }
        public static void Info(string msg)
        {
            MSG(msg);
        }
    }
}
