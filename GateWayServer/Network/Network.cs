using log4net;
using Scripts;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Tool;

namespace SNetwork
{
    public partial class Network
    {
        private const int PK_HASH_CODE = 10800;
        private const int PK_USER_LOGIN_CMD = 10020;
        private const int WEB_CMD_LOAD_SERVER = 8239;
        private const int PK_10022 = 10022;

    }

    public partial class Network
    {

        Socket ClientSocket;
        Socket ListenSocket;
        IPEndPoint EndPoint;
        Util util;

        private static readonly ILog Log = LogManager.GetLogger(typeof(Network));

        public Network(int port)
        {
            util = new Util();
            ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            EndPoint = new IPEndPoint(IPAddress.Any, port);
            util.ColorMsg(ConsoleColor.White, ConsoleColor.Black, $"Network Working on {port}");
        }

        public bool Start()
        {
            new Thread(new ThreadStart(StartAccept)).Start();
            return true;
        }

        private void StartAccept()
        {
            byte[] m_bBuffer = new byte[1024];
            ListenSocket.Bind(EndPoint);
            ListenSocket.Listen(1000);

            while (true)
            {
                ClientSocket = ListenSocket.Accept();

                util.ColorMsg(ConsoleColor.DarkBlue, ConsoleColor.White, "Client Connect: {0}\r\n", ClientSocket.RemoteEndPoint.ToString());

                ClientSocket.Receive(m_bBuffer);
                int start = 0;
                int command = m_bBuffer[start + 3] << 8 | m_bBuffer[start + 4];
                int idx = m_bBuffer[start + 5] << 8 | m_bBuffer[start + 6];
                string result = $"command: {command} | idx: {idx}\n";
                switch (command)
                {
                    case PK_HASH_CODE:
                        util.ColorMsg(ConsoleColor.DarkMagenta, ConsoleColor.White, result);
                        S10801.OnHash(ClientSocket);
                        break;
                    case WEB_CMD_LOAD_SERVER:
                        util.ColorMsg(ConsoleColor.DarkBlue, ConsoleColor.White, Encoding.UTF8.GetString(m_bBuffer));
                        SCMD.OnWeb(ClientSocket, m_bBuffer);
                        break;
                    case PK_USER_LOGIN_CMD:
                        byte[] m_bResized = PacketHeaderRemove(PacketResize(m_bBuffer));
                        util.ColorMsg(ConsoleColor.DarkMagenta, ConsoleColor.White, "idx: {0}", command);
                        util.ColorMsg(ConsoleColor.White, ConsoleColor.Black, util.PrintBytes(m_bResized));
                        new S10021().OnLogin(ClientSocket);
                        break;
                    case PK_10022:
                        util.ColorMsg(ConsoleColor.DarkMagenta, ConsoleColor.White, "idx: {0}", command);
                        util.ColorMsg(ConsoleColor.White, ConsoleColor.Black, util.PrintBytes(m_bBuffer));
                        new S10023().OnTutorial(ClientSocket);
                        break;
                    default:
                        util.ColorMsg(ConsoleColor.DarkYellow, ConsoleColor.White, "idx: {0}", command);
                        util.ColorMsg(ConsoleColor.DarkMagenta, ConsoleColor.White, result);
                        util.ColorMsg(ConsoleColor.White, ConsoleColor.Black, Encoding.UTF8.GetString(m_bBuffer));
                        break;
                }
                ClientSocket.Close();
            }
            Destroy();
        }

        private void Destroy()
        {

        }
    }

    public partial class Network
    {
        public byte[] PacketResize(byte[] stream)
        {
            byte[] array = new byte[stream[1] + 2];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = stream[i];
            }
            return array;
        }

        public byte[] PacketHeaderRemove(byte[] stream)
        {
            byte[] array = new byte[stream[1] - 5];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = stream[i + 7];
            }
            return array;
        }
    }
}
