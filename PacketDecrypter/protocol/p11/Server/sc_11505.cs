using ProtoBuf;

namespace p11
{
    [ProtoContract]
    public class sc_11505
    {
        [ProtoMember(1)]
        public uint result { get; set; }

        [ProtoMember(2)]
        public uint shop_id { get; set; }

        [ProtoMember(3)]
        public uint gem { get; set; }

        [ProtoMember(4)]
        public uint gem_free { get; set; }
    }
}
