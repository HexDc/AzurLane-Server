using ProtoBuf;

namespace common
{
    [ProtoContract]
    public class USERSIMPLEINFO
    {
        [ProtoMember(1)]
        public uint id { get; set; }

        [ProtoMember(2)]
        public string name { get; set; }

        [ProtoMember(3)]
        public uint lv { get; set; }

        [ProtoMember(4)]
        public DISPLAYINFO display { get; set; }
    }
}
