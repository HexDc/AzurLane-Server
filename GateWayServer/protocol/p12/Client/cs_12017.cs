using ProtoBuf;
using System.Collections.Generic;

namespace p12
{
    [ProtoContract]
    public class cs_12017
    {
        [ProtoMember(1)]
        public uint ship_id { get; set; }

        [ProtoMember(2)]
        public List<uint> material_id_list { get; set; }
    }
}
