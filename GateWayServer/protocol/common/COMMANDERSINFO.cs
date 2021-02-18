using ProtoBuf;

namespace common
{
    [ProtoContract]
    public class COMMANDERSINFO
    {
        [ProtoMember(1)]
        public uint pos { get; set; }

        [ProtoMember(2)]
        public uint id { get; set; }
    }
}
