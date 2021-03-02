using ProtoBuf;

namespace p10
{
    [ProtoContract]
    public class cs_10022
    {
        [ProtoMember(1)]
        public uint account_id { get; set; }

        [ProtoMember(2)]
        public string server_ticket { get; set; }

        [ProtoMember(3)]
        public string platform { get; set; }

        [ProtoMember(4)]
        public uint serverid { get; set; }

        [ProtoMember(5)]
        public string check_key { get; set; }

        [ProtoMember(6)]
        public string device_id { get; set; }
    }
}
