using ProtoBuf;
using System.ComponentModel;

namespace p10
{
    [ProtoContract]
    public class NoticeInfo
    {
        [ProtoMember(1)]
        public uint id { get; set; }

        [ProtoMember(2)]
        public string title { get; set; }

        [ProtoMember(3)]
        [DefaultValue("")]
        public string content { get; set; }
    }
}
