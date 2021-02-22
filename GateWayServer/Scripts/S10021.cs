using p10;
using PacketStream;
using ProtoBuf;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace Scripts
{
    public class S10021
    {
        private string m_title;
        private bool is_on;
        private string m_content;
        private int server_cnt;
        private List<string> m_ip = new List<string>();
        private List<int> m_port = new List<int>();
        private List<int> m_state = new List<int>();
        private List<string> m_name = new List<string>();
        private List<int> m_tag_state = new List<int>();
        private List<int> m_sort = new List<int>();
        private List<string> m_proxy_ip = new List<string>();
        private List<int> m_proxy_port = new List<int>();

        public S10021()
        {
            LoadInfo();
        }

        public void OnLogin(Socket ClientSocket)
        {
            ClientSocket.Send(Ma_10021(), 0, Ma_10021().Length, SocketFlags.None);
        }

        private void LoadInfo()//가변형
        {
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
        }
        private byte[] Ma_10021()////////////Tutorial Packet!!!!!!!
        {
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
            return packStream.ToArray();
        }
    }
}
