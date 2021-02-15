using p10;
using ProtoBuf;
using System.IO;
using PacketStream;
using System.Net.Sockets;

namespace Scripts
{
    public class S10023
    {
        public void OnTutorial(Socket ClientSocket)
        {
            ClientSocket.Send(m_10023(), 0, m_10023().Length, SocketFlags.None);
        }

        private byte[] m_10023()
        {
            MemoryStream memoryStream = new MemoryStream();
            Serializer.Serialize(memoryStream, new sc_10023
            {
                result = 0,//0은 튜토리얼 보내기.
                user_id = 0,
                server_ticket = "16130413370e95b519e791f3f09b4f0ec80c818cdc",
                server_load = 0,
                db_load = 0,
            });
            byte[] array = memoryStream.ToArray();
            PackStream packStream = new PackStream(15 + array.Length);

            memoryStream.Close();
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
