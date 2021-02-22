using ProtoBuf;

namespace p12
{
    [ProtoContract]
    public class cs_12008
    {
        [ProtoMember(1)]
        public uint type { get; set; }

        [ProtoMember(2)]
        public uint pos { get; set; }
    }
}
