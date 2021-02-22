using ProtoBuf;
using System.Collections.Generic;

namespace common
{
    [ProtoContract]
    public class SHIPCOREINFO
    {
        [ProtoMember(1)]
        public uint id { get; set; }

        [ProtoMember(2)]
        public uint exp { get; set; }

        [ProtoMember(3)]
        public List<SHIPMODELINFO> model_list { get; set; }
    }
}
