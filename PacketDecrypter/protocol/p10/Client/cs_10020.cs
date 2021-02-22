using ProtoBuf;
using System.Collections.Generic;

namespace p10
{
    [ProtoContract]
    public class cs_10020
    {
        [ProtoMember(1)]
        public uint login_type { get; set; }

        [ProtoMember(2)]
        public string arg1 { get; set; }

        [ProtoMember(3)]
        public string arg2 { get; set; }

        [ProtoMember(4)]
        public string arg3 { get; set; }

        [ProtoMember(5)]
        public string arg4 { get; set; }

        [ProtoMember(6)]
        public string check_key { get; set; }

        [ProtoMember(7)]
        public uint device { get; set; }
    }
}
