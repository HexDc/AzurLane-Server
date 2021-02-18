using ProtoBuf;

namespace p11
{
    [ProtoContract]
    public class BENEFITBUFF
    {
        [ProtoMember(1)]
        public uint id { get; set; }

        [ProtoMember(2)]
        public uint timestamp { get; set; }
    }
}
