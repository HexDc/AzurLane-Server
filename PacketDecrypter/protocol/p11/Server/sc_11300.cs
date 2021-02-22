using ProtoBuf;
using System.Collections.Generic;

namespace p11
{
    [ProtoContract]
    public class sc_11300
    {
        [ProtoMember(1)]
        public List<NOTICEINFO> notice_list { get; set; }
    }
}
