using p11;
using ProtoBuf;
using System.IO;

namespace Scripts
{
    class S11000
    {
        public static byte[] _11000()
        {
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, new sc_11000
                {
                    timestamp = 1614440010,
                    monday_0oclock_timestamp = 1606057200
                });
                return ms.ToArray();
            }
        }
    }
}
