using ProtoBuf;

namespace p11
{
    [ProtoContract]
    public class KEYVALUE
    {
        [ProtoMember(1)]
        public uint key { get; set; }

        [ProtoMember(2)]
        public uint value { get; set; }
    }
}
