using ProtoBuf;

namespace p11
{
    [ProtoContract]
    public class cs_11703
    {
        [ProtoMember(1)]
        public uint id { get; set; }

        [ProtoMember(2)]
        public uint discuss { get; set; }

        [ProtoMember(3)]
        public uint index { get; set; }
    }
}
