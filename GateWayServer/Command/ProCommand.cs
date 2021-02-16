using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool;
using ProtoBuf;
using System.IO;

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
                ClientSocket = ListenSocket.Accept();

                util.ColorMsg(ConsoleColor.Yellow, ConsoleColor.Blue, $"Commander Connect:{ClientSocket.RemoteEndPoint}");

                ClientSocket.Receive(m_bBuffer);

                MemoryStream memoryStream = new MemoryStream(m_bBuffer);
                PRO_COMMANDER cmd = Serializer.Deserialize<PRO_COMMANDER>(memoryStream);

                switch (cmd.TYPE)
                {
                    case "LOGIN":
                        if (CheckLogin(cmd.ID, cmd.PW))
                            ClientSocket.Send(Encoding.Default.GetBytes("SUCCESS"), 0, Encoding.Default.GetBytes("SUCCESS").Length, SocketFlags.None);
                        else
                            ClientSocket.Send(Encoding.Default.GetBytes("FAILED"), 0, Encoding.Default.GetBytes("FAILED").Length, SocketFlags.None);
                        break;
                    default:
                        util.ColorMsg(ConsoleColor.White, ConsoleColor.Black, Encoding.Default.GetString(m_bBuffer));
                        break;
                }
                ClientSocket.Close();
            }
        }

        private bool CheckLogin(string id, string pw)
        {
            return false;
        }
    }

    public class PRO_COMMANDER
    {
        public string TYPE { get; set; }
        public string ID { get; set; }
        public string PW { get; set; }
    }
}
