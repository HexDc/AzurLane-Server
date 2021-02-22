using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace p11
{
    [ProtoContract]
    public class INS_MESSAGE
    {
        [ProtoMember(1)]
        public uint id { get; set; }

        [ProtoMember(2)]
        public uint time { get; set; }

        [ProtoMember(3)]
        public string text { get; set; }

        [ProtoMember(4)]
        public string picture { get; set; }

        [ProtoMember(5)]
        public List<INS_PLAYER> player_discuss { get; set; }

        [ProtoMember(6)]
        public List<INS_NPC> npc_discuss { get; set; }

        [ProtoMember(7)]
        public List<INS_NPC> npc_reply { get; set; }

        [ProtoMember(8)]
        public uint good { get; set; }

        [ProtoMember(9)]
        public uint is_good { get; set; }

        [ProtoMember(10)]
        public uint is_read { get; set; }
    }
}
