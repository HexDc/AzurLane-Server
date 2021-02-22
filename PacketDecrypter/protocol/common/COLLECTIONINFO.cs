using ProtoBuf;
using System.Collections.Generic;

namespace common
{
    [ProtoContract]
    public class COLLECTIONINFO
    {
        [ProtoMember(1)]
        public uint id { get; set; }

        [ProtoMember(2)]
        public uint finish_time { get; set; }

        [ProtoMember(3)]
        public uint over_time { get; set; }

        [ProtoMember(4)]
        public List<uint> ship_id_list { get; set; }
    }
}
