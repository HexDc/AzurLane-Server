using System;
using System.Threading;
using GNetwork;
using Service;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "GameServer";
            if (args.Length < 1)
                new Thread(new ThreadStart(new ServiceG(new string[] { "-c", "GameServer.ini" }).ServiceMainProc)).Start();
            else
                new Thread(new ThreadStart(new ServiceG(args).ServiceMainProc)).Start();
        }
    }
}
