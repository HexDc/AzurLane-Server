using ProtoBuf;

namespace common
{
    [ProtoContract]
    public class EQUIPINFO
    {
        [ProtoMember(1)]
        public uint id { get; set; }

        [ProtoMember(2)]
        public uint skinId { get; set; }
    }
}
