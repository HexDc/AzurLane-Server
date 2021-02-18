using ProtoBuf;
using System.Collections.Generic;

namespace p10
{
    [ProtoContract]
    public class cs_10996
    {
        [ProtoMember(1)]
        public uint state { get; set; }

        [ProtoMember(2)]
        public string platform { get; set; }
    }
}
