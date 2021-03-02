using ProtoBuf;

namespace common
{
    [ProtoContract]
    public class TASKINFO
    {
        [ProtoMember(1)]
        public uint id { get; set; }

        [ProtoMember(2)]
        public uint progress { get; set; }

        [ProtoMember(3)]
        public uint accept_time { get; set; }

        [ProtoMember(4)]
        public uint submit_time { get; set; }
    }
}
