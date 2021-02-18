using ProtoBuf;

namespace p11
{
    [ProtoContract]
    public class cs_11001
    {
        [ProtoMember(1)]
        public uint timestamp { get; set; }
    }
}
