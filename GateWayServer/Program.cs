using GNetwork;
using Service;
using System;

namespace GateWayServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "GateWayServer";

            if (args.Length < 1)
                new ServiceAZ(new string[] { "-c", "GateWay.ini" }).ServiceMainProc();
            else
                new ServiceAZ(args).ServiceMainProc();

            InitSingleton();
        }

        private static void InitSingleton()
        {
            TSingleton<ExceptionManager>.CreateInstance();
            TSingleton<Network>.CreateInstance();
            TSingleton<Command>.CreateInstance().CommandProcess();
        }
    }
}
