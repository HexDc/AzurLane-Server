using ProtoBuf;
using System.Collections.Generic;

namespace p11
{
    [ProtoContract]
    public class sc_11015
    {
        [ProtoMember(1)]
        public List<BENEFITBUFF> buff_list { get; set; }
    }
}
