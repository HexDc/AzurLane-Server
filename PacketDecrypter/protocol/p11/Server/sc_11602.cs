using ProtoBuf;
using System.Collections.Generic;

namespace p11
{
    [ProtoContract]
    public class sc_11602
    {
        [ProtoMember(1)]
        public List<uint> emoji_list { get; set; }
    }
}
