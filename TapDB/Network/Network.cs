using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TNetwork
{
    public class Network
    {
        Socket ClientSocket;
        Socket ListenSocket;
        IPEndPoint EndPoint;

        public Network(int port = 443)
        {
            ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            EndPoint = new IPEndPoint(IPAddress.Any, port);
            ColorMsg(ConsoleColor.White, ConsoleColor.Black, $"TapDB Working on {port}");
        }

        public void Start()
        {
            new Thread(new ThreadStart(OnReceive)).Start();
        }

        private void OnReceive()
        {
            byte[] m_bBuffer = new byte[1024];

            ListenSocket.Bind(EndPoint);
            ListenSocket.Listen(1000);

            while(true)
            {
                ClientSocket = ListenSocket.Accept();

                ColorMsg(ConsoleColor.DarkBlue, ConsoleColor.White, $"Client Connect:{ClientSocket.RemoteEndPoint}");

                ClientSocket.Receive(m_bBuffer);
                ColorMsg(ConsoleColor.White, ConsoleColor.Black, PrintBytes(m_bBuffer));
                ClientSocket.Close();
            }
        }
        public string PrintBytes(byte[] bytes)
        {
            return string.Join(", ", bytes);
        }
        private void ColorMsg(ConsoleColor backcolor, ConsoleColor forecolor, string format, params object[] args)
        {
            Console.BackgroundColor = backcolor;
            Console.ForegroundColor = forecolor;
            Console.Write($"\r{format}\n", args);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
