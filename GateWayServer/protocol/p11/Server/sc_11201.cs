using ProtoBuf;
using System.Collections.Generic;

namespace p11
{
    [ProtoContract]
    public class sc_11201
    {

        [ProtoMember(1)]
        public List<ACTIVITYINFO> activity_info { get; set; }
    }
}
