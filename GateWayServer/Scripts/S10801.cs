﻿using p10;
using ProtoBuf;
using System.IO;
using PacketStream;
using System.Net.Sockets;
using System.Collections.Generic;

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
            MemoryStream memoryStream = new MemoryStream();
            List<string> m_version = new List<string>
            {
                "$azhash$5$1$66$d1450368de02b1b0",
                "$cvhash$201$6e5e160d6b363d8d",
                "",
                "",
                "$l2dhash$203$bc6f7014e2e49ad6",
                "$pichash$8$8281b91e4240ad7f",
                "$bgmhash$8$8281b91e4240ad7f"
            };
            Serializer.Serialize(memoryStream, new sc_10801
            {
                gateway_ip = "bl-kr-gate.xdg.com",
                gateway_port = 80,
                url = "https://play.google.com/store/apps/details?id=kr.txwy.and.blhx",
                version = m_version,
                proxy_ip = "",
                proxy_port = 0,
                is_ts = 0
            });
            byte[] array = memoryStream.ToArray();
            memoryStream.Close();
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
