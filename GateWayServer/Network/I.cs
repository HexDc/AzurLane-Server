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

namespace SNetworkS
{
    public partial class I
    {
        private const int PK_HASH_CODE = 10800;
        private const int PK_USER_LOGIN_CMD = 10020;
        private const int WEB_CMD_LOAD_SERVER = 8239;
        private const int PK_10022 = 10022;
    }

    public partial class I
    {
        TcpClient TC;
        TcpListener TL;
        NetworkStream NS;
        Util util;

        public I(int port)
        {
            util = new Util();
            util.ColorMsg(ConsoleColor.White, ConsoleColor.Black, $"Network Working on {port}");
        }

        public bool Start()
        {
            new Thread(new ThreadStart(StartAccept)).Start();
            return true;
        }

        private void StartAccept()
        {
            TL = new TcpListener(IPAddress.Any, 80);
            TL.Start();

            byte[] m_bBuffer = new byte[1024];

            while (true)
            {
                TC = TL.AcceptTcpClient();
                NS = TC.GetStream();
                Console.WriteLine(TC.Client.RemoteEndPoint.ToString());
                while (NS.Read(m_bBuffer, 0, m_bBuffer.Length) > 0)
                {
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
                            S10801.OnHash(NS);
                            break;
                        case WEB_CMD_LOAD_SERVER:
                            break;
                        case PK_USER_LOGIN_CMD:
                            m_bResized = PacketHeaderRemove(PacketResize(m_bBuffer));
                            util.ColorMsg(ConsoleColor.DarkMagenta, ConsoleColor.White, "idx: {0}", command);
                            using (MemoryStream ms = new MemoryStream(m_bResized))
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
                            new S10021().OnLogin(NS);
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
                            new S10023().OnTutorial(NS);
                            break;
                        default:
                            SCMD.OnWeb(NS, m_bBuffer);
                            util.ColorMsg(ConsoleColor.DarkYellow, ConsoleColor.White, "idx: {0}", command);
                            util.ColorMsg(ConsoleColor.DarkMagenta, ConsoleColor.White, result);
                            util.ColorMsg(ConsoleColor.White, ConsoleColor.Black, util.PrintBytes(m_bBuffer));
                            break;
                    }
                    if (command == 8239)
                    {
                        SCMD.OnWeb(NS, m_bBuffer);
                        TC.Close();
                        break;
                    }
                }
            }
        }
    }

    public partial class I
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
