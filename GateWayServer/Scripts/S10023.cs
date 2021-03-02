using p10;
using ProtoBuf;
using System.IO;
using PacketStream;
using System.Net.Sockets;

namespace Scripts
{
    public class S10023
    {
        public byte[] OnTutorial()
        {
            return m_10023();
        }

        private byte[] m_10023()
        {
            byte[] array;
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, new sc_10023
                {
                    result = 0,//0은 튜토리얼 보내기.
                    user_id = 67751892,
                    server_ticket = "16144400104a8afc841fb14e8dd3b1448782f03472",
                    server_load = 0,
                    db_load = 0,
                });
                array = ms.ToArray();
            }
            var packStream = new PackStream(15 + array.Length);

            packStream.WriteUint16((uint)(9 + array.Length));
            packStream.WriteUint8(0);
            packStream.WriteUint16(10023);
            packStream.WriteUint16(0);
            packStream.WriteUint8(8);
            packStream.WriteUint8(0);
            packStream.WriteUint8(16);
            packStream.WriteUint8(0);
            packStream.WriteBuffer(array);

            return packStream.ToArray();
        }
    }
}
