using ProtoBuf;

namespace p10
{
    [ProtoContract]
    public class sc_10101
    {
        /// <summary>
        /// 하트비트?
        /// </summary>
        [ProtoMember(1)]
        public uint state { get; set; }
    }
}
