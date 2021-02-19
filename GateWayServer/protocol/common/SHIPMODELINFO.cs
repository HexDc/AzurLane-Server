using ProtoBuf;

namespace common
{
    [ProtoContract]
    public class SHIPMODELINFO
    {
        [ProtoMember(1)]
        public uint pos { get; set; }

        [ProtoMember(2)]
        public uint id { get; set; }
    }
}
