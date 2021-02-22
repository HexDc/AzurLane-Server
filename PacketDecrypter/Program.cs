using System;
using System.IO;
using System.Linq;
using System.Text;
using ProtoBuf;
using p10;
using p11;
using p12;

namespace PacketDecrypter
{
    class Program
    {
        static void Main()
        {
            try
            {
                string text = Console.ReadLine();
                byte[] m_bBuffer = text.Split(',').Select(x => Convert.ToByte(x)).ToArray();
                int start = 0;
                int command = m_bBuffer[start + 3] << 8 | m_bBuffer[start + 4];
                int idx = m_bBuffer[start + 5] << 8 | m_bBuffer[start + 6];
                string result = $"command: {command} | idx: {idx}\n";
                byte[] m_bResized = PacketHeaderRemove(PacketResize(m_bBuffer));
                Console.WriteLine($"\n{command}" + " {");
                using (MemoryStream ms = new MemoryStream(m_bResized))
                {
                    switch (command)
                    {
                        case 10800:
                            Print(result);
                            break;
                        case 10801:
                            sc_10801 _10801 = Serializer.Deserialize<sc_10801>(ms);
                            Print($"    gateway_ip: {_10801.gateway_ip}");
                            Print($"    gateway_port: {_10801.gateway_port}");
                            Print($"    url: {_10801.url}");
                            Print($"    proxy_ip: {_10801.proxy_ip}");
                            Print($"    proxy_port: {_10801.proxy_port}");
                            Print($"    is_ts: {_10801.is_ts}");
                            break;
                        case 10022:
                            cs_10022 _10022 = Serializer.Deserialize<cs_10022>(ms);
                            Print($"    account_id: {_10022.account_id}");
                            Print($"    server_ticket: {_10022.server_ticket}");
                            Print($"    platform: {_10022.platform}");
                            Print($"    serverid: {_10022.serverid}");
                            Print($"    check_key: {_10022.check_key}");
                            Print($"    device_id: {_10022.device_id}");
                            break;
                        case 10023:
                            sc_10023 _10023 = Serializer.Deserialize<sc_10023>(ms);
                            Print($"    result: {_10023.result}");
                            Print($"    user_id: {_10023.user_id}");
                            Print($"    server_ticket: {_10023.server_ticket}");
                            Print($"    server_load: {_10023.server_load}");
                            Print($"    db_load: {_10023.db_load}");
                            break;
                        case 11000:
                            sc_11000 _11000 = Serializer.Deserialize<sc_11000>(ms);
                            
                            Print($"    timestamp: {_11000.timestamp}");
                            Print($"    monday_0oclock_timestamp: {_11000.monday_0oclock_timestamp}");
                            break;
                        case 11001:
                            cs_11001 _11001 = Serializer.Deserialize<cs_11001>(ms);
                            Print($"    timestamp: {_11001.timestamp}");
                            break;
                        case 11003:
                            sc_11003 _11003 = Serializer.Deserialize<sc_11003>(ms);
                            Print($"    id: {_11003.id}");
                            Print($"    name: {_11003.name}");
                            Print($"    level: {_11003.level}");
                            Print($"    exp: {_11003.exp}");
                            break;
                        case 10021:
                            sc_10021 _10021 = Serializer.Deserialize<sc_10021>(ms);

                            Print($"    result: {_10021.result}");
                            foreach(ServerInfo SI in _10021.serverlist)
                            {
                                Print($"    serverlist.ids: {SI.ids}");
                                Print($"    serverlist.ip: {SI.ip}");
                                Print($"    serverlist.port: {SI.port}");
                                Print($"    serverlist.state: {SI.state}");
                                Print($"    serverlist.name: {SI.name}");
                                Print($"    serverlist.tag_state: {SI.tag_state}");
                                Print($"    serverlist.sort: {SI.sort}");
                                Print($"    serverlist.proxy_ip: {SI.proxy_ip}");
                                Print($"    serverlist.proxy_port: {SI.proxy_port}");

                            }
                            Print($"    account_id: {_10021.account_id}");
                            Print($"    server_ticket: {_10021.server_ticket}");
                            Print($"    device: {_10021.device}");
                            break;
                        default:
                            Print(result);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Console.WriteLine("}");
                Main();
            }
        }
        static void De10023()
        {
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, new sc_11003
                {
                    id = 1,
                    name="byte_test",
                    level=1,
                    exp=0
                });
            }
        }
        static void Print(string format, params object[] args)
        {
            Console.Write($"\r{format}\n", args);
        }

        static byte[] HexToBytes(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }


        static string PrintBytes(byte[] bytes)
        {
            return string.Join(", ", bytes);
        }

        static byte[] PacketResize(byte[] stream)
        {
            byte[] array = new byte[stream[1] + 2];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = stream[i];
            }
            return array;
        }

        static byte[] PacketHeaderRemove(byte[] stream)
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
