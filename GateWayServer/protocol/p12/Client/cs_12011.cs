using ProtoBuf;

namespace p12
{
    [ProtoContract]
    public class cs_12011
    {
        [ProtoMember(1)]
        public uint ship_id { get; set; }

        [ProtoMember(2)]
        public uint remould_id { get; set; }

        [ProtoMember(3)]
        public uint material_id { get; set; }
    }
}
