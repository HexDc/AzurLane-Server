using ProtoBuf;
using System.Collections.Generic;

namespace p10
{
    [ProtoContract]
    public class sc_10999
    {
        [ProtoMember(1)]
        public uint reason { get; set; }
    }
}
