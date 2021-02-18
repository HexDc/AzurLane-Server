using ProtoBuf;

namespace p10
{
    [ProtoContract]
    public class sc_10025
    {
        [ProtoMember(1)]
        public uint result { get; set; }

        [ProtoMember(2)]
        public uint user_id { get; set; }
    }
}
