using common;
using ProtoBuf;
using System.Collections.Generic;

namespace p11
{
    [ProtoContract]
    public class sc_11003
    {
        [ProtoMember(1, DataFormat = DataFormat.TwosComplement, IsRequired = true, Name = "id")]
        public uint id { get; set; }

        [ProtoMember(2, DataFormat = DataFormat.Default, IsRequired = true, Name = "name")]
        public string name { get; set; }

        [ProtoMember(3, DataFormat = DataFormat.TwosComplement, IsRequired = true, Name = "level")]
        public uint level { get; set; }

        [ProtoMember(4, DataFormat = DataFormat.TwosComplement, IsRequired = true, Name = "exp")]
        public uint exp { get; set; }

        [ProtoMember(5, DataFormat = DataFormat.Group, IsRequired = true, Name = "resource_list")]
        public List<RESOURCE> resource_list { get; set; }
        
        [ProtoMember(6)]
        public uint attack_count { get; set; }

        [ProtoMember(7)]
        public uint win_count { get; set; }

        [ProtoMember(8)]
        public string adv { get; set; }

        [ProtoMember(9)]
        public uint character { get; set; }

        [ProtoMember(10)]
        public uint ship_bag_max { get; set; }

        [ProtoMember(11)]
        public uint equip_bag_max { get; set; }

        [ProtoMember(12)]
        public uint gm_flag { get; set; }

        [ProtoMember(13)]
        public uint rank { get; set; }

        [ProtoMember(14)]
        public uint pvp_attack_count { get; set; }

        [ProtoMember(15)]
        public uint pvp_win_count { get; set; }

        [ProtoMember(16)]
        public uint collect_attack_count { get; set; }

        [ProtoMember(17)]
        public uint guide_index { get; set; }

        [ProtoMember(18)]
        public uint buy_oil_count { get; set; }

        [ProtoMember(19)]
        public uint chat_room_id { get; set; }

        [ProtoMember(20)]
        public List<CARDINFO> card_list { get; set; }

        [ProtoMember(21)]
        public uint max_rank { get; set; }

        [ProtoMember(22)]
        public uint register_time { get; set; }

        [ProtoMember(23)]
        public uint ship_count { get; set; }

        [ProtoMember(24)]
        public uint acc_pay_lv { get; set; }

        [ProtoMember(25)]
        public uint story_list { get; set; }

        [ProtoMember(26)]
        public uint guild_wait_time { get; set; }

        [ProtoMember(27)]
        public uint chat_msg_ban_time { get; set; }

        [ProtoMember(28)]
        public uint flag_list { get; set; }

        [ProtoMember(29)]
        public List<COOLDOWN> cd_list { get; set; }

        [ProtoMember(30)]
        public uint commander_bag_max { get; set; }

        [ProtoMember(31)]
        public uint medal_id { get; set; }

        [ProtoMember(32)]
        public List<IDTIMEINFO> icon_frame_list { get; set; }

        [ProtoMember(33)]
        public List<IDTIMEINFO> chat_frame_list { get; set; }

        [ProtoMember(34)]
        public DISPLAYINFO display { get; set; }

        [ProtoMember(35)]
        public uint rmb { get; set; }

        [ProtoMember(36)]
        public APPRECIATIONINFO appreciation { get; set; }

        [ProtoMember(37)]
        public uint theme_upload_not_allowed_time { get; set; }

    }
}
