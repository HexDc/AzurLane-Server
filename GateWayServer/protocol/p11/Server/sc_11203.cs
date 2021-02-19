using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using common;

namespace p11
{
    [ProtoContract]
    public class sc_11203
    {
        [ProtoMember(1)]
        public uint result { get; set; }

        [ProtoMember(2)]
        public List<DROPINFO> award_list { get; set; }

        [ProtoMember(3)]
        public List<BUILDINFO> build { get; set; }

        [ProtoMember(4)]
        public uint number { get; set; }

        [ProtoMember(5)]
        public List<RETURN_USER_INFO> return_user_list { get; set; }

        [ProtoMember(6)]
        public INS_MESSAGE ins_message { get; set; }

        [ProtoMember(7)]
        public List<COLLECTIONINFO> collection_list { get; set; }

        [ProtoMember(8)]
        public List<TASKINFO> task_list { get; set; }
    }
}
