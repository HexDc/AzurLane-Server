using System;
using System.IO;

namespace GateWayServer
{
    public class ExceptionManager : TSingleton<ExceptionManager>
    {
        public ExceptionManager() : base() { }
        static string Path = Directory.GetCurrentDirectory() + "\\log\\ExceptionInfo.log";

        public void Exception(string format, params object[] args)
        {
            File.AppendAllText(Path, DateTime.Now.ToString("[yyyy-MM-dd-HH-mm-ss]-- ") + "$\r{format}\n" + "\n\r");
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write($"\r{format}\n", args);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
