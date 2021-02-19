using ProtoBuf;

namespace p12
{
    [ProtoContract]
    public class cs_12006
    {
        [ProtoMember(1)]
        public uint ship_id { get; set; }

        [ProtoMember(2)]
        public uint equip_id { get; set; }

        [ProtoMember(3)]
        public uint pos { get; set; }

        [ProtoMember(4)]
        public uint type { get; set; }
    }
}
