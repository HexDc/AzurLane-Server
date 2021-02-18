using ProtoBuf;

namespace p11
{
    [ProtoContract]
    public class sc_11002
    {

        [ProtoMember(1)]
        public uint timestamp { get; set; }

        [ProtoMember(2)]
        public uint monday_0oclock_timestamp { get; set; }
    }
}
