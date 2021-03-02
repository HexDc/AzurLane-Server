using ProtoBuf;
using System.Collections.Generic;

namespace p11
{
    [ProtoContract]
    public class INS_NPC
    {
        [ProtoMember(1)]
        public uint id { get; set; }

        [ProtoMember(2)]
        public uint time { get; set; }

        [ProtoMember(3)]
        public string text { get; set; }

        [ProtoMember(4)]
        public uint npc_reply { get; set; }
    }
}
