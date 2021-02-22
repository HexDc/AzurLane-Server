using ProtoBuf;

namespace common
{
    [ProtoContract]
    public class STRENGTH_INFO
    {
        [ProtoMember(1)]
        public uint id { get; set; }

        [ProtoMember(2)]
        public uint exp { get; set; }
    }
}
