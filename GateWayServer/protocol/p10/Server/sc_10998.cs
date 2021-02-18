using ProtoBuf;
using System.Collections.Generic;

namespace p10
{
    [ProtoContract]
    public class sc_10998
    {
        [ProtoMember(1)]
        public uint cmd { get; set; }

        [ProtoMember(2)]
        public uint result { get; set; }
    }
}
