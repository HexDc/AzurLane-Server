using ProtoBuf;
using System.Collections.Generic;

namespace p12
{
    [ProtoContract]
    public class cs_12004
    {
        [ProtoMember(1)]
        public List<uint> ship_id_list { get; set; }
    }
}
