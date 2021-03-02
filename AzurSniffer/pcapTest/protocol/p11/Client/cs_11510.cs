using ProtoBuf;

namespace p11
{
    [ProtoContract]
    public class cs_11510
    {
        [ProtoMember(1)]
        public string pay_id { get; set; }

        [ProtoMember(2)]
        public uint code { get; set; }
    }
}
