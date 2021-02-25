using System;
using System.Collections.Generic;
using System.Net.Sockets;
using GNetwork;
using Service;

namespace GateWayServer
{
    class Program
    {
        static ServiceAZ SAZ;
        
        static void Main(string[] args)
        {
            TSingleton<ExceptionManager>.CreateInstance();
            TSingleton<Network>.CreateInstance();

            Console.Title = "GateWayServer";
            if (args.Length < 1)
                SAZ = new ServiceAZ(new string[] { "-c", "GateWay.ini" });
            else
                SAZ = new ServiceAZ(args);

            SAZ.ServiceMainProc();
            while(true)
            {
                string command = Console.ReadLine().ToLower();
                string[] split = command.Split(' ');
                if(split.Length > 0 && split[0].StartsWith("/"))
                {
                    Network Norder = TSingleton<Network>.Instance;
                    switch(split[0].Replace("/", ""))
                    {
                        case "list":
                            Console.WriteLine("Currently Client Count: {0}", Norder.GetUserCount());
                            int i = 0;
                            foreach(string item in Norder.GetUserIPList())
                            {
                                ++i;
                                Console.WriteLine("[{0}] - {1}", i, item);
                            }
                            i = 0;
                            break;
                        case "clear":
                            Console.Clear();
                            Console.WriteLine("Console Cleared -- {0}", DateTime.Now.ToString("yyyy-MM-dd::HH-mm-ss"));
                            break;

                    }
                }
            }
        }
    }
}
