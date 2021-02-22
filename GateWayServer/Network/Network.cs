using Tool;
using System;
using Scripts;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using p10;
using ProtoBuf;
using System.IO;

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
        List<string> UserList;

        public Network(int port)
        {
            util = new Util();
            ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            EndPoint = new IPEndPoint(IPAddress.Any, 16000);
            util.ColorMsg(ConsoleColor.White, ConsoleColor.Black, $"Network Working on {port}");
            UserList = new List<string>();
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

                util.ColorMsg(ConsoleColor.DarkBlue, ConsoleColor.White, "Client Connect: {0}\r\n", ClientSocket.RemoteEndPoint.ToString().Trim());
                UserList.Add(ClientSocket.RemoteEndPoint.ToString());

                ClientSocket.Receive(m_bBuffer);
                int start = 0;
                int command = m_bBuffer[start + 3] << 8 | m_bBuffer[start + 4];
                int idx = m_bBuffer[start + 5] << 8 | m_bBuffer[start + 6];
                string result = $"command: {command} | idx: {idx}\n";
                byte[] m_bResized;
                switch (command)
                {
                    case 11001:
                        util.ColorMsg(ConsoleColor.DarkMagenta, ConsoleColor.White, "idx: {0}", command);
                        Console.WriteLine("!1111111111111111111");
                        break;
                    case PK_HASH_CODE:
                        util.ColorMsg(ConsoleColor.DarkMagenta, ConsoleColor.White, "idx: {0}", command);
                        //util.ColorMsg(ConsoleColor.DarkMagenta, ConsoleColor.White, util.PrintBytes(m_bBuffer));
                        S10801.OnHash(ClientSocket);
                        break;
                    case WEB_CMD_LOAD_SERVER:
                        SCMD.OnWeb(ClientSocket, m_bBuffer);
                        break;
                    case PK_USER_LOGIN_CMD:
                        m_bResized = PacketHeaderRemove(PacketResize(m_bBuffer));
                        util.ColorMsg(ConsoleColor.DarkMagenta, ConsoleColor.White, "idx: {0}", command);
                        using(MemoryStream ms = new MemoryStream(m_bResized))
                        {
                            cs_10020 _10020 = Serializer.Deserialize<cs_10020>(ms);
                            Console.WriteLine("{");
                            Console.WriteLine($"    login_type: {_10020.login_type}");
                            Console.WriteLine($"    arg1: {_10020.arg1}");
                            Console.WriteLine($"    arg2: {_10020.arg2}");
                            Console.WriteLine($"    arg3: {_10020.arg3}");
                            Console.WriteLine($"    arg4: {_10020.arg4}");
                            Console.WriteLine($"    check_key: {_10020.check_key}");
                            Console.WriteLine($"    device: {_10020.device}");
                            Console.WriteLine("}");
                        }
                        new S10021().OnLogin(ClientSocket);
                        break;
                    case PK_10022:
                        m_bResized = PacketHeaderRemove(PacketResize(m_bBuffer));
                        util.ColorMsg(ConsoleColor.DarkMagenta, ConsoleColor.White, "idx: {0}", command);
                        using (MemoryStream ms = new MemoryStream(m_bResized))
                        {
                            cs_10022 _10022 = Serializer.Deserialize<cs_10022>(ms);
                            Console.WriteLine("{");
                            Console.WriteLine($"    account_id: {_10022.account_id}");
                            Console.WriteLine($"    server_ticket: {_10022.server_ticket}");
                            Console.WriteLine($"    platform: {_10022.platform}");
                            Console.WriteLine($"    serverid: {_10022.serverid}");
                            Console.WriteLine($"    check_key: {_10022.check_key}");
                            Console.WriteLine($"    device_id: {_10022.device_id}");
                            Console.WriteLine("}");
                        }
                        new S10023().OnTutorial(ClientSocket);
                        break;
                    default:
                        SCMD.OnWeb(ClientSocket, m_bBuffer);
                        util.ColorMsg(ConsoleColor.DarkYellow, ConsoleColor.White, "idx: {0}", command);
                        util.ColorMsg(ConsoleColor.DarkMagenta, ConsoleColor.White, result);
                        util.ColorMsg(ConsoleColor.White, ConsoleColor.Black, util.PrintBytes(m_bBuffer));
                        break;
                }
                //ClientSocket.Close();
            }
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
