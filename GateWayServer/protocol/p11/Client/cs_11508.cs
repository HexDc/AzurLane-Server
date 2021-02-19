using ProtoBuf;

namespace p11
{
    [ProtoContract]
    public class cs_11508
    {
        [ProtoMember(1)]
        public string key { get; set; }

        [ProtoMember(2)]
        public string platform { get; set; }
    }
}
