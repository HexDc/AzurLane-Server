using ProtoBuf;

namespace p11
{
    [ProtoContract]
    public class CARDINFO
    {
        [ProtoMember(1)]
        public uint type { get; set; }

        [ProtoMember(2)]
        public uint left_date { get; set; }
    }
}
