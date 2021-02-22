using ProtoBuf;
using System.Collections.Generic;

namespace p10
{
    [ProtoContract]
    public class sc_10997
    {
        [ProtoMember(1)]
        public uint version1 { get; set; }

        [ProtoMember(2)]
        public uint version2 { get; set; }

        [ProtoMember(3)]
        public uint version3 { get; set; }

        [ProtoMember(4)]
        public uint versuin4 { get; set; }

        [ProtoMember(5)]
        public string gateway_ip { get; set; }

        [ProtoMember(6)]
        public uint gateway_port { get; set; }

        [ProtoMember(7)]
        public string url { get; set; }
    }
}
