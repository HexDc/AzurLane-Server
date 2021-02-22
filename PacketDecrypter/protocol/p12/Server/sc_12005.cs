using ProtoBuf;
using System.Collections.Generic;

namespace p12
{
    [ProtoContract]
    public class sc_12005
    {
        [ProtoMember(1)]
        public uint result { get; set; }

        [ProtoMember(2)]
        public List<uint> ship_id_list { get; set; }
    }
}
