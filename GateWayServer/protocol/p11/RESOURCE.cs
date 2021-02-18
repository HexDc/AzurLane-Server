using ProtoBuf;

namespace p11
{
    [ProtoContract]
    public class RESOURCE
    {
        [ProtoMember(1)]
        public uint type { get; set; }

        [ProtoMember(2)]
        public uint num { get; set; }
    }
}
