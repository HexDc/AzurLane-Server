using System;
using GateWayServer.Network;
using Service;

namespace GateWayServer
{
    class Program
    {
        static ServiceAZ SAZ;

        static void Main(string[] args)
        {
            new Class1().Start();
        }

        static void _Main(string[] args)
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
