using ProtoBuf;

namespace p11
{
    [ProtoContract]
    public class sc_11201
    {
        [ProtoMember(1)]
        public ACTIVITYINFO activity_info { get; set; }
    }
}
