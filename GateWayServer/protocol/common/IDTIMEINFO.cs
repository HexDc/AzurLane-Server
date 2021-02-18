using ProtoBuf;

namespace common
{
    [ProtoContract]
    public class IDTIMEINFO
    {
        [ProtoMember(1)]
        public uint id { get; set; }

        [ProtoMember(2)]
        public uint time { get; set; }
    }
}
