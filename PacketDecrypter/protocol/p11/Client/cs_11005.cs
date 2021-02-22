using ProtoBuf;

namespace p11
{
    [ProtoContract]
    public class cs_11005
    {
        [ProtoMember(1)]
        public uint type { get; set; }

        [ProtoMember(2)]
        public uint id { get; set; }
    }
}
