using ProtoBuf;
using System.Collections.Generic;

namespace p11
{
    [ProtoContract]
    public class sc_11004
    {
        [ProtoMember(1)]
        public List<RESOURCE> resource_list { get; set; }
    }
}
