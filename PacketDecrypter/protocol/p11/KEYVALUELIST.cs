using ProtoBuf;
using System.Collections.Generic;

namespace p11
{
    [ProtoContract]
    public class KEYVALUELIST
    {
        [ProtoMember(1)]
        public uint key { get; set; }

        [ProtoMember(2)]
        public List<KEYVALUE> value_list { get; set; }
    }
}
