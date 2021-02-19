using ProtoBuf;
using System.Collections.Generic;

namespace p11
{
    [ProtoContract]
    public class cs_11605
    {
        [ProtoMember(1)]
        public string password { get; set; }

        [ProtoMember(2)]
        public List<uint> system_list { get; set; }

        [ProtoMember(3)]
        public string notice { get; set; }
    }
}
