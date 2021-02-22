using ProtoBuf;

namespace common
{
    [ProtoContract]
    public class APPRECIATIONINFO
    {
        [ProtoMember(1)]
        public uint gallerys { get; set; }

        [ProtoMember(2)]
        public uint musics { get; set; }

        [ProtoMember(3)]
        public uint favor_gallerys { get; set; }

        [ProtoMember(4)]
        public uint favor_musics { get; set; }
    }
}
