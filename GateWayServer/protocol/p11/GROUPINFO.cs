using common;
using ProtoBuf;
using System.Collections.Generic;

namespace p11
{
    [ProtoContract]
    public class GROUPINFO
    {
        [ProtoMember(1)]
        public uint id { get; set; }

        [ProtoMember(2)]
        public uint ship_list { get; set; }

        [ProtoMember(3)]
        public List<COMMANDERSINFO> commanders { get; set; }
    }
}
