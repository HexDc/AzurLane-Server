using ProtoBuf;

namespace common
{
    [ProtoContract]
    public class BUILDINFO
    {
        [ProtoMember(1)]
        public uint time { get; set; }

        [ProtoMember(2)]
        public uint finish_time { get; set; }

        [ProtoMember(3)]
        public uint build_id { get; set; }
    }
}
