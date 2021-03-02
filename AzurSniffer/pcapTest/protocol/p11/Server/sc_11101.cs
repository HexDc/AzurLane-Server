using ProtoBuf;

namespace p11
{
    [ProtoContract]
    public class sc_11101
    {
        [ProtoMember(1)]
        public uint result { get; set; }

        [ProtoMember(2)]
        public string msg { get; set; }
    }
}
