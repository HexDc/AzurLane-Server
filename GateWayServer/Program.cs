using System;
using Service;

namespace GateWayServer
{
    class Program
    {
        static ServiceAZ SAZ;
        static void Main(string[] args)
        {
            Console.Title = "GateWayServer";
            if (args.Length < 1)
                SAZ = new ServiceAZ(new string[] { "-c", "GateWay.ini" });
            else
                SAZ = new ServiceAZ(args);

            SAZ.ServiceMainProc();
        }
    }
}
