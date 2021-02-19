using ProtoBuf;

namespace p11
{
    [ProtoContract]
    public class sc_11402
    {
        [ProtoMember(1)]
        public uint result { get; set; }

        [ProtoMember(2)]
        public uint room_id { get; set; }
    }
}