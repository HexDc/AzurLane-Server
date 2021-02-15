using ProtoBuf;
using System.Collections.Generic;

namespace p10
{
    [ProtoContract]
    public class sc_10021
    {
        [ProtoMember(1)]
        public uint result { get; set; }

        [ProtoMember(2)]
        public List<ServerInfo> serverlist { get; set; }

        [ProtoMember(3)]
        public uint account_id { get; set; }

        [ProtoMember(4)]
        public string server_ticket { get; set; }

        [ProtoMember(5)]
        public NoticeInfo notice_list { get; set; }

        [ProtoMember(6)]
        public uint device { get; set; }
    }
}
