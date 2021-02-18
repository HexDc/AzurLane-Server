using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using common;
using ProtoBuf;

namespace p11
{
    [ProtoContract]
    public class ACTIVITYINFO
    {
        [ProtoMember(1)]
        public uint id { get; set; }

        [ProtoMember(2)]
        public uint stop_time { get; set; }

        [ProtoMember(3)]
        public uint data1 { get; set; }

        [ProtoMember(4)]
        public uint data2 { get; set; }

        [ProtoMember(5)]
        public uint data3 { get; set; }

        [ProtoMember(6)]
        public uint data4 { get; set; }

        [ProtoMember(7)]
        public List<int> data1_list { get; set; }

        [ProtoMember(8)]
        public List<int> data2_list { get; set; }

        [ProtoMember(9)]
        public List<int> data3_list { get; set; }

        [ProtoMember(10)]
        public List<KEYVALUELIST> data1_key_value_list { get; set; }

        [ProtoMember(11)]
        public List<GROUPINFO> group_list { get; set; }

        [ProtoMember(12)]
        public List<INS_MESSAGE> ins_message_list { get; set; }

        [ProtoMember(13)]
        public List<COLLECTIONINFO> collection_list { get; set; }

        [ProtoMember(14)]
        public List<TASKINFO> task_list { get; set; }
    }
}
