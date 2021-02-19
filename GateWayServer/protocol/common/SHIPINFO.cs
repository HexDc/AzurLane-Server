using ProtoBuf;
using System.Collections.Generic;

namespace common
{
    [ProtoContract]
    public class SHIPINFO
    {
        [ProtoMember(1)]
        public uint id { get; set; }

        [ProtoMember(2)]
        public uint template_id { get; set; }

        [ProtoMember(3)]
        public uint level { get; set; }

        [ProtoMember(4)]
        public uint exp { get; set; }

        [ProtoMember(5)]
        public List<EQUIPINFO> equip_info_list { get; set; }

        [ProtoMember(6)]
        public uint energy { get; set; }

        [ProtoMember(7)]
        public SHIPSTATE state { get; set; }

        [ProtoMember(8)]
        public uint is_locked { get; set; }

        [ProtoMember(9)]
        public List<TRANSFORM_INFO> transform_list { get; set; }

        [ProtoMember(10)]
        public List<SHIPSKILL> skill_id_list { get; set; }

        [ProtoMember(11)]
        public uint intimacy { get; set; }

        [ProtoMember(12)]
        public uint proficiency { get; set; }

        [ProtoMember(13)]
        public List<STRENGTH_INFO> strength_list { get; set; }

        [ProtoMember(14)]
        public uint create_time { get; set; }

        [ProtoMember(15)]
        public uint skin_id { get; set; }

        [ProtoMember(16)]
        public uint propose { get; set; }

        [ProtoMember(17)]
        public string name { get; set; }

        [ProtoMember(18)]
        public uint change_name_timestamp { get; set; }

        [ProtoMember(19)]
        public uint commanderid { get; set; }

        [ProtoMember(20)]
        public uint max_level { get; set; }

        [ProtoMember(21)]
        public uint blue_print_flag { get; set; }

        [ProtoMember(22)]
        public uint common_flag { get; set; }

        [ProtoMember(23)]
        public uint activity_npc { get; set; }

        [ProtoMember(24)]
        public List<uint> meta_repair_list { get; set; }

        [ProtoMember(25)]
        public List<SHIPCOREINFO> core_list { get; set; }
    }
}
