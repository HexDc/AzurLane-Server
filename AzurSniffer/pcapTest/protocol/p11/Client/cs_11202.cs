using ProtoBuf;
using System.Collections.Generic;

namespace p11
{
    [ProtoContract]
    public class cs_11202
    {
        [ProtoMember(1)]
        public uint id { get; set; }

        [ProtoMember(2)]
        public uint cmd { get; set; }

        [ProtoMember(3)]
        public uint arg1 { get; set; }

        [ProtoMember(4)]
        public uint arg2 { get; set; }

        [ProtoMember(5)]
        public uint arg3 { get; set; }

        [ProtoMember(6)]
        public List<uint> arg_list { get; set; }
    }
}
