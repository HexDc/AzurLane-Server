using ProtoBuf;
using System.Collections.Generic;
using common;

namespace p12
{
    [ProtoContract]
    public class sc_12001
    {
        [ProtoMember(1)]
        public List<SHIPINFO> shiplist { get; set; }
    }
}
