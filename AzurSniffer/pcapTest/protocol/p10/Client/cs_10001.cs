using ProtoBuf;

namespace p10
{
    [ProtoContract]
    public class cs_10001
    {
        [ProtoMember(1)]
        public string account { get; set; }

        [ProtoMember(2)]
        public string password { get; set; }

        [ProtoMember(3)]
        public string mail_box { get; set; }
    }
}
