using Command;
using Service;
using System;
using System.Threading;

namespace GateWayServer
{
    class Program
    {
        static void CmdNetwork()
        {
            ProCommand cmd = new ProCommand(20000);

            if (true)
            {
                if (!cmd.OnStart())
                {
                    Console.WriteLine("Processing Command ..");
                    Console.WriteLine("failed\n");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Processing Command ..");
                Console.WriteLine("done\n");
            }
        }

        static void Main(string[] args)
        {
            Console.Title = "GateWayServer";
            if (args.Length < 1)
                new Thread(new ThreadStart(new ServiceAZ(new string[] { "-c", "GateWay.ini" }).ServiceMainProc)).Start();
            else
                new Thread(new ThreadStart(new ServiceAZ(args).ServiceMainProc)).Start();

            new Thread(new ThreadStart(CmdNetwork)).Start();
        }
    }
}
