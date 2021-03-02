using ProtoBuf;

namespace common
{
    [ProtoContract]
    public class SHIPSKILL
    {
        [ProtoMember(1)]
        public uint skill_id { get; set; }

        [ProtoMember(2)]
        public uint skill_lv { get; set; }

        [ProtoMember(3)]
        public uint skill_exp { get; set; }
    }
}
