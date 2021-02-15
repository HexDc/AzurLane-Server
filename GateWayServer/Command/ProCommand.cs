using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool;

namespace Command
{
    class ProCommand
    {
        Socket ClientSocket;
        Socket ListenSocket;
        IPEndPoint EndPoint;
        Util util;
        public ProCommand(int port = 20000)
        {
            util = new Util();
            ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            EndPoint = new IPEndPoint(IPAddress.Any, port);
            util.ColorMsg(ConsoleColor.White, ConsoleColor.Black, $"Commander Working on {port}");
        }

        public bool OnStart()
        {
            new Thread(new ThreadStart(Start)).Start();
            return true;
        }

        private void Start()
        {
            while(true)
            {
                //TODO
            }
        }
    }
}
