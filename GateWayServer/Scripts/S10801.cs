using p10;
using ProtoBuf;
using System.IO;
using PacketStream;
using System.Net.Sockets;
using System.Collections.Generic;
using System;

namespace Scripts
{
    public class S10801
    {
        public static void OnHash(Socket ClientSocket)
        {
            ClientSocket.Send(_GetHashCode(), 0, _GetHashCode().Length, SocketFlags.None);
        }

        private static byte[] _GetHashCode()
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
            using(var ms = new MemoryStream())
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

            return packStream.ToArray();
        }
    }
}
