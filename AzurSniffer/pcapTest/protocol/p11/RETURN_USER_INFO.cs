using common;
using ProtoBuf;

namespace p11
{
    [ProtoContract]
    public class RETURN_USER_INFO
    {
        [ProtoMember(1)]
        public USERSIMPLEINFO user { get; set; }

        [ProtoMember(2)]
        public uint pt { get; set; }
    }
}
