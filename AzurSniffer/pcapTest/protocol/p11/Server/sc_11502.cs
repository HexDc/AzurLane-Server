using ProtoBuf;

namespace p11
{
    [ProtoContract]
    public class sc_11502
    {
        [ProtoMember(1)]
        public uint result { get; set; }

        [ProtoMember(2)]
        public string pay_id { get; set; }

        [ProtoMember(3)]
        public string url { get; set; }

        [ProtoMember(4)]
        public string order_sign { get; set; }
    }
}
