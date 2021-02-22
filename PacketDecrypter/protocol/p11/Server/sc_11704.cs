using ProtoBuf;

namespace p11
{
    [ProtoContract]
    public class sc_11704
    {
        [ProtoMember(1)]
        public uint result { get; set; }

        [ProtoMember(2)]
        public INS_MESSAGE data { get; set; }
    }
}
