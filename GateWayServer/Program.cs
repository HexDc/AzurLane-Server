using System;
using System.Threading;
using Service;
using ProtoBuf;

namespace GateWayServer
{
    class Program
    {
        static void Main(string[] args)
        {
            new Thread(new ThreadStart(new ServiceAZ(new string[] { "-c", "GateWay.ini" }).ServiceMainProc)).Start();
            //.
        }
    }
}
