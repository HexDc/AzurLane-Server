using ProtoBuf;
using System.Collections.Generic;
using common;

namespace p12
{
    [ProtoContract]
    public class sc_12024
    {
        [ProtoMember(1)]
        public List<uint> worklist_count { get; set; }

        [ProtoMember(2)]
        public List<BUILDINFO> worklist_list { get; set; }

        [ProtoMember(3)]
        public uint draw_count_1 { get; set; }

        [ProtoMember(4)]
        public uint draw_count_10 { get; set; }
    }
}
