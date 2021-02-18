using ProtoBuf;

namespace p11
{
    [ProtoContract]
    public class cs_11100
    {
        [ProtoMember(1)]
        public string cmd { get; set; }

        [ProtoMember(2)]
        public string arg1 { get; set; }

        [ProtoMember(3)]
        public string arg2 { get; set; }

        [ProtoMember(4)]
        public string arg3 { get; set; }
    }
}
