using p10;
using PacketStream;
using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool;

namespace GateWayServer.Network
{
    public class Class1
    {
        public void Start()
        {
            new Thread(new ThreadStart(ST)).Start();
        }

        private void ST()
        {
            TcpListener Listen = new TcpListener(IPAddress.Any, 80);
            Listen.Start();
            byte[] buff = new byte[1024];

            while (true)
            {
                TcpClient TC = Listen.AcceptTcpClient();

                NetworkStream NS = TC.GetStream();

                while (NS.Read(buff, 0, buff.Length) > 0)
                {

                    int start = 0;
                    int command = buff[start + 3] << 8 | buff[start + 4];
                    int idx = buff[start + 5] << 8 | buff[start + 6];
                    string result = $"command: {command} | idx: {idx}\n";
                    Console.WriteLine(result);
                    switch (command)
                    {
                        case 8239:
                            WEB(NS);
                            break;
                        case 10800:
                            SC_10801(NS);
                            break;
                        case 10020:
                            SC_10201(NS);
                            break;
                        default:
                            Console.WriteLine(new Util().PrintBytes(buff));
                            break;
                    }
                    if (Encoding.Default.GetString(new byte[] { buff[0], buff[1], buff[2] }) == "GET")
                    {
                        GET(buff, NS);
                        TC.Close();
                        break;
                    }
                }
            }
        }
        private void WEB(NetworkStream NS)
        {
            Dictionary<string, object> dic_1 = new Dictionary<string, object>
            {
                { "id", 1 },
                { "name", "명량" },
                { "state", 0 },
                { "flag", 1 },
                { "sort", 1 }
            };
            Dictionary<string, object> dic_2 = new Dictionary<string, object>
            {
                { "id", 2 },
                { "name", "노량" },
                { "state", 0 },
                { "flag", 1 },
                { "sort", 0 }
            };

            Dictionary<string, object> dic_3 = new Dictionary<string, object>
            {
                { "id", 3 },
                { "name", "한산도" },
                { "state", 0 },
                { "flag", 1 },
                { "sort", 0 }
            };

            Dictionary<string, object> dic_4 = new Dictionary<string, object>
            {
                { "id", 4 },
                { "name", "옥계" },
                { "state", 0 },
                { "flag", 1 },
                { "sort", 0 }
            };
            ArrayList arr = new ArrayList
            {
                dic_1,
                dic_2,
                dic_3,
                dic_4
            };
            string data = JsonFx.Json.JsonWriter.Serialize(arr);
            string p = @"HTTP/1.1 200 OK
Content-Type: text/plain;charset=utf-8
Access-Control-Allow-Origin: 
Content-Length: 216

" + data;
            NS.Write(Encoding.Default.GetBytes(data), 0, Encoding.Default.GetByteCount(data));
        }

        private void SC_10201(NetworkStream NS)
        {
            string m_title;
            bool is_on;
            string m_content = "";
            int server_cnt;
            List<string> m_ip = new List<string>();
            List<int> m_port = new List<int>();
            List<int> m_state = new List<int>();
            List<string> m_name = new List<string>();
            List<int> m_tag_state = new List<int>();
            List<int> m_sort = new List<int>();
            List<string> m_proxy_ip = new List<string>();
            List<int> m_proxy_port = new List<int>();
            IniFile ini = new IniFile();
            ini.Load(Directory.GetCurrentDirectory() + "\\config\\gatewaysvr.ini");
            server_cnt = ini["Default"]["server_cnt"].ToInt();
            for (int i = 1; i <= server_cnt; i++)
            {
                m_ip.Add(ini[string.Format("Server_{0}", i)]["ip"].ToString());
                m_state.Add(ini[string.Format("Server_{0}", i)]["state"].ToInt());
                m_port.Add(ini[string.Format("Server_{0}", i)]["port"].ToInt());
                m_name.Add(ini[string.Format("Server_{0}", i)]["name"].ToString());
                m_tag_state.Add(ini[string.Format("Server_{0}", i)]["tag_state"].ToInt());
                m_sort.Add(ini[string.Format("Server_{0}", i)]["sort"].ToInt());
                m_proxy_ip.Add(ini[string.Format("Server_{0}", i)]["proxy_ip"].ToString());
                m_proxy_port.Add(ini[string.Format("Server_{0}", i)]["proxy_port"].ToInt());
            }
            ini.Clear();

            ini.Load(Directory.GetCurrentDirectory() + "\\config\\notice.ini");
            is_on = ini["Default"]["On"].ToInt() == 1 ? true : false;
            m_title = ini["Default"]["title"].ToString();
            if (ini["Default"]["content_1"].ToString().Length > 1)
            {
                m_content += ini["Default"]["content_1"].ToString();
            }
            if (ini["Default"]["content_2"].ToString().Length > 1)
            {
                m_content += "\n" + ini["Default"]["content_2"].ToString();
            }
            if (ini["Default"]["content_3"].ToString().Length > 1)
            {
                m_content += "\n" + ini["Default"]["content_3"].ToString();
            }
            if (ini["Default"]["content_4"].ToString().Length > 1)
            {
                m_content += "\n" + ini["Default"]["content_4"].ToString();
            }
            var m_Serverlist = new List<ServerInfo>();

            for (int i = 0; i < server_cnt; ++i)
            {
                m_Serverlist.Add(new ServerInfo
                {
                    ids = (uint)i + 1,
                    ip = m_ip[i],
                    port = (uint)m_port[i],
                    state = (uint)m_state[i],
                    name = m_name[i],
                    tag_state = (uint)m_tag_state[i],
                    sort = (uint)m_sort[i],
                    proxy_ip = m_proxy_ip[i],
                    proxy_port = (uint)m_proxy_port[i]
                });
            }

            NoticeInfo m_Notice = new NoticeInfo
            {
                id = 0,
                title = m_title,
                content = m_content
            };
            MemoryStream memoryStream = new MemoryStream();

            if (!is_on)
            {
                Serializer.Serialize(memoryStream, new sc_10021
                {
                    result = 0,
                    serverlist = m_Serverlist,
                    account_id = 643028,
                    server_ticket = "16139726267bcf1408c971a28c50e96d290953499d",
                    device = 11
                });
            }
            else
            {
                Serializer.Serialize(memoryStream, new sc_10021
                {
                    result = 0,
                    serverlist = m_Serverlist,
                    account_id = 643028,
                    server_ticket = "16139726267bcf1408c971a28c50e96d290953499d",
                    device = 11,
                    notice_list = m_Notice
                });
            }
            byte[] Array = memoryStream.ToArray();
            memoryStream.Close();
            PackStream packStream = new PackStream(Array.Length + 15);
            packStream.WriteUint8(0);
            packStream.WriteUint8((uint)(Array.Length + 5));
            packStream.WriteUint8(0);
            packStream.WriteUint16(10021);
            packStream.WriteUint16(0);
            packStream.WriteUint8(8);
            packStream.WriteUint8(0);
            packStream.WriteBuffer(Array);
            byte[] bt = packStream.ToArray();
            NS.Write(bt, 0, bt.Length);
        }

        private void SC_10801(NetworkStream NS)
        {
            IniFile ini = new IniFile();
            ini.Load(Directory.GetCurrentDirectory() + "\\config\\gatewaysvr.ini");

            List<string> m_version = new List<string>
            {
                ini["Hash"]["azhash"].ToString(),
                ini["Hash"]["cvhash"].ToString(),
                "",
                "",
                ini["Hash"]["l2dhash"].ToString(),
                ini["Hash"]["pichash"].ToString(),
                ini["Hash"]["bgmhash"].ToString()
            };

            byte[] array;
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, new sc_10801
                {
                    gateway_ip = ini["GateWay"]["ip"].ToString(),
                    gateway_port = Convert.ToUInt16(ini["GateWay"]["port"].ToInt()),
                    url = ini["GateWay"]["url"].ToString(),
                    version = m_version,
                    proxy_ip = ini["GateWay"]["proxy_ip"].ToString(),
                    proxy_port = Convert.ToUInt16(ini["GateWay"]["proxy_port"].ToInt()),
                    is_ts = Convert.ToUInt16(ini["GateWay"]["is_ts"].ToInt())
                });
                array = ms.ToArray();
            }

            PackStream packStream = new PackStream(15 + array.Length);
            packStream.WriteUint16((uint)(9 + array.Length));
            packStream.WriteUint8(0);
            packStream.WriteUint16(10801);
            packStream.WriteUint16(0);
            packStream.WriteBuffer(array);
            packStream.WriteUint8(48);
            packStream.WriteUint16(56);
            packStream.WriteUint8(0);
            byte[] bt = packStream.ToArray();
            NS.Write(bt, 0, bt.Length);
        }
        private void GET(byte[] buff, NetworkStream NS)
        {
            string p = @"HTTP/1.1 200 OK
Content-Type: text/plain;charset=utf-8
Access-Control-Allow-Origin: 

Hello";
            NS.Write(Encoding.Default.GetBytes(p), 0, Encoding.Default.GetBytes(p).Length);
            //Console.WriteLine(new Util().PrintBytes(buff));

        }
    }
}
