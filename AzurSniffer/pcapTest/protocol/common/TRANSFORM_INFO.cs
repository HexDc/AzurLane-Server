using ProtoBuf;

namespace common
{
    [ProtoContract]
    public class TRANSFORM_INFO
    {
        [ProtoMember(1)]
        public uint id { get; set; }

        [ProtoMember(2)]
        public uint level { get; set; }
    }
}
