using ProtoBuf;

namespace common
{
    [ProtoContract]
    public class SHIPSTATE
    {
        [ProtoMember(1)]
        public uint state { get; set; }

        [ProtoMember(2)]
        public uint state_info_1 { get; set; }

        [ProtoMember(3)]
        public uint state_info_2 { get; set; }

        [ProtoMember(4)]
        public uint state_info_3 { get; set; }

        [ProtoMember(5)]
        public uint state_info_4 { get; set; }
    }
}
