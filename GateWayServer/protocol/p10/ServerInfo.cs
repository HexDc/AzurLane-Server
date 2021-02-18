using ProtoBuf;

namespace p10
{
    [ProtoContract]
    public class ServerInfo
    {
        [ProtoMember(1)]
        public uint ids { get; set; }

        [ProtoMember(2)]
        public string ip { get; set; }

        [ProtoMember(3)]
        public uint port { get; set; }

        [ProtoMember(4)]
        public uint state { get; set; }

        [ProtoMember(5)]
        public string name { get; set; }

        [ProtoMember(6)]
        public uint tag_state { get; set; }

        [ProtoMember(7)]
        public uint sort { get; set; }

        [ProtoMember(8)]
        public string proxy_ip { get; set; }

        [ProtoMember(9)]
        public uint proxy_port { get; set; }
    }
}
