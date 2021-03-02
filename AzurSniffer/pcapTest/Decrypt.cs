using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using p10;
using ProtoBuf;

namespace pcapTest
{
    public partial class MainForm : Form
    {
        public string PacketDecrypt(string text)
        {
            byte[] m_bBuffer = text.Split(',').Select(x => Convert.ToByte(x)).ToArray();
            return Decrypt(m_bBuffer);
        }

        private string Decrypt(byte[] data)
        {
            int command = data[3] << 8 | data[4];
            int idx = data[5] << 8 | data[6];
            string result = $"command: {command} | idx: {idx}\n";
            byte[] m_bResized = PacketHeaderRemove(PacketResize(data));
            using (var ms = new MemoryStream(m_bResized))
            {
                switch (command)
                {
                    case 8239:
                        return "Web Message";
                    case 10800:
                        cs_10800 _10800 = Serializer.Deserialize<cs_10800>(ms);
                        result += $"\nplatform: {_10800.platform}";
                        result += $"\nstate: {_10800.state}";
                        break;
                    case 10020:
                        cs_10020 _10020 = Serializer.Deserialize<cs_10020>(ms);
                        result += $"\narg1: {_10020.arg1}";
                        result += $"\narg2: {_10020.arg2}";
                        result += $"\narg3: {_10020.arg3}";
                        result += $"\narg4: {_10020.arg4}";
                        result += $"\ncheck_key: {_10020.check_key}";
                        result += $"\ndevice: {_10020.device}";
                        break;
                    case 10022:
                        cs_10022 _10022 = Serializer.Deserialize<cs_10022>(ms);
                        result += $"\naccount_id: {_10022.account_id}";
                        result += $"\naccount_id: {_10022.check_key}";
                        result += $"\ndevice_id: {_10022.device_id}";
                        result += $"\nplatform: {_10022.platform}";
                        result += $"\nserverid: {_10022.serverid}";
                        result += $"\nserver_ticket: {_10022.server_ticket}";
                        break;
                }
                return result;
            }
        }

        private byte[] PacketResize(byte[] stream)
        {
            byte[] array = new byte[stream[1] + 2];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = stream[i];
            }
            return array;
        }

        private byte[] PacketHeaderRemove(byte[] stream)
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
