using Tool;
using System;
using ProtoBuf;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;

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
            byte[] m_bBuffer = new byte[512];
            ListenSocket.Bind(EndPoint);
            ListenSocket.Listen(5);
            while(true)
            {

            }
        }

        private bool CheckLogin(string id, string pw)
        {
            return true;
        }
    }
    [ProtoContract]
    public class PRO_COMMANDER
    {
        [ProtoMember(1)]
        public string TYPE { get; set; }

        [ProtoMember(2)]
        public string BOX1 { get; set; }

        [ProtoMember(3)]
        public string BOX2 { get; set; }
    }
}
