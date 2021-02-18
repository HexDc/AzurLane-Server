using ProtoBuf;

namespace p10
{
    [ProtoContract]
    public class sc_10002
    {
        [ProtoMember(1)]
        public uint result { get; set; }
    }
}
