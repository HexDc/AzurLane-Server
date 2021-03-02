using ProtoBuf;
using System.Collections.Generic;

namespace p11
{
    [ProtoContract]
    public class sc_11604
    {
        [ProtoMember(1)]
        public uint state { get; set; }

        [ProtoMember(2)]
        public List<uint> system_list { get; set; }

        [ProtoMember(3)]
        public uint fail_count { get; set; }

        [ProtoMember(4)]
        public uint fail_cd { get; set; }

        [ProtoMember(5)]
        public string notice { get; set; }
    }
}
