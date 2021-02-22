using ProtoBuf;

namespace p11
{
    [ProtoContract]
    public class NOTICEINFO
    {
        [ProtoMember(1)]
        public uint id { get; set; }

        [ProtoMember(2)]
        public string version { get; set; }

        [ProtoMember(3)]
        public string btn_title { get; set; }

        [ProtoMember(4)]
        public string title { get; set; }

        [ProtoMember(5)]
        public string title_image { get; set; }

        [ProtoMember(6)]
        public string title_desc { get; set; }

        [ProtoMember(7)]
        public string content { get; set; }
    }
}
