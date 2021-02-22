using ProtoBuf;

namespace p12
{
    [ProtoContract]
    public class cs_12002
    {
        [ProtoMember(1)]
        public uint id { get; set; }

        [ProtoMember(2)]
        public uint count { get; set; }
    }
}
