using ProtoBuf;

namespace p10
{
    [ProtoContract]

    public class cs_10024
    {
        /// <summary>
        /// 초기 닉네임
        /// </summary>
        [ProtoMember(1)]
        public string nick_name { get; set; }

        /// <summary>
        /// 초기 함선
        /// </summary>
        [ProtoMember(2)]
        public uint ship_id { get; set; }

        [ProtoMember(3)]
        public string device_id { get; set; }
    }
}
