using ProtoBuf;
using System.Collections.Generic;

namespace p11
{
    [ProtoContract]
    public class cs_11204
    {
        [ProtoMember(1)]
        public uint activity_id { get; set; }

        [ProtoMember(2)]
        public List<GROUPINFO> group_list { get; set; }
    }
}
