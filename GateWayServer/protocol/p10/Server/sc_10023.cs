using ProtoBuf;

namespace p10
{
    [ProtoContract]
    class sc_10023
    {
        [ProtoMember(1)]
        public uint result { get; set; }

        [ProtoMember(2)]
        public uint user_id { get; set; }

        [ProtoMember(3)]
        public string server_ticket { get; set; }

        [ProtoMember(4)]
        public uint server_load { get; set; }

        [ProtoMember(5)]
        public uint db_load { get; set; }
    }
}
