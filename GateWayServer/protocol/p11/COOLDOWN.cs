using ProtoBuf;

namespace p11
{
    [ProtoContract]
    public class COOLDOWN
    {
        [ProtoMember(1)]
        public uint key { get; set; }

        [ProtoMember(2)]
        public uint timestamp { get; set; }
    }
}
