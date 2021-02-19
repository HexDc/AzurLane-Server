using ProtoBuf;
using System.Collections.Generic;

namespace p12
{
    [ProtoContract]
    public class cs_12022
    {
        [ProtoMember(1)]
        public List<uint> ship_id_list { get; set; }

        [ProtoMember(2)]
        public uint is_locked { get; set; }
    }
}
