using ProtoBuf;

namespace p10
{
    [ProtoContract]
    public class cs_10100
    {
        [ProtoMember(1)]
        public uint need_request { get; set; }
    }
}
