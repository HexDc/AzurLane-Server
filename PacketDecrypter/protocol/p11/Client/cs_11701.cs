using ProtoBuf;

namespace p11
{
    [ProtoContract]
    public class cs_11701
    {
        [ProtoMember(1)]
        public uint id { get; set; }

        [ProtoMember(2)]
        public uint cmd { get; set; }
    }
}
