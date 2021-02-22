using ProtoBuf;

namespace common
{
    [ProtoContract]
    public class DISPLAYINFO
    {
        [ProtoMember(1)]
        public uint icon { get; set; }

        [ProtoMember(2)]
        public uint skin { get; set; }

        [ProtoMember(3)]
        public uint icon_frame { get; set; }

        [ProtoMember(4)]
        public uint chat_frame { get; set; }

        [ProtoMember(5)]
        public uint icon_theme { get; set; }

        [ProtoMember(6)]
        public uint marry_flag { get; set; }

        [ProtoMember(7)]
        public uint transform_flag { get; set; }
    }
}
