using ProtoBuf;

namespace common
{
    [ProtoContract]
    public class DROPINFO
    {
        [ProtoMember(1)]
        public uint type { get; set; }

        [ProtoMember(2)]
        public uint id { get; set; }

        [ProtoMember(3)]
        public uint number { get; set; }
    }
}
