using ProtoBuf;

namespace p11
{
    [ProtoContract]
    public class sc_11205
    {
        [ProtoMember(1)]
        public uint activity_id { get; set; }

        [ProtoMember(2)]
        public uint result { get; set; }
    }
}
