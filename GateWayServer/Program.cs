using Service;
using System.Threading;

namespace GateWayServer
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length < 1)
                new Thread(new ThreadStart(new ServiceAZ(new string[] { "-c", "GateWay.ini" }).ServiceMainProc)).Start();
            else
                new Thread(new ThreadStart(new ServiceAZ(args).ServiceMainProc)).Start();
        }
    }
}
