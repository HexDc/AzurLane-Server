using ProtoBuf;
using common;

namespace p12
{
    [ProtoContract]
    public class sc_12003
    {
        [ProtoMember(1)]
        public uint result { get; set; }

        [ProtoMember(2)]
        public BUILDINFO build_info { get; set; }
    }
}
