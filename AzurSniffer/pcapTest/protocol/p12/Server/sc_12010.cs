using common;
using ProtoBuf;
using System.Collections.Generic;

namespace p12
{
    [ProtoContract]
    public class sc_12010
    {
        [ProtoMember(1)]
        public List<SHIPINFO> ship_list { get; set; }
    }
}
