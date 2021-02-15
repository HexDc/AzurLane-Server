using System;
using Service;
using System.Threading;
using Tool;
using SNetwork;
using System.IO;
using p10;
using ProtoBuf;
using System.Text;
using System.Collections.Generic;
using PacketStream;

namespace GateWayServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "GateWayServer";
            if (args.Length < 1)
                new Thread(new ThreadStart(new ServiceAZ(new string[] { "-c", "GateWay.ini" }).ServiceMainProc)).Start();
            else
                new Thread(new ThreadStart(new ServiceAZ(args).ServiceMainProc)).Start();
        }
    }
}
