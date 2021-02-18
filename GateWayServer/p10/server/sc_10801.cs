using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace p10
{
    [ProtoContract(Name = "sc_10801")]
    [Serializable]
    public class sc_10801 : IExtensible
    {
        [ProtoMember(1, DataFormat = DataFormat.Default, IsRequired = true, Name = "gateway_ip")]
        public string gateway_ip { get; set; }

        [ProtoMember(2, DataFormat = DataFormat.TwosComplement, IsRequired = true, Name = "gateway_port")]
        public uint gateway_port { get; set; }

        [ProtoMember(3, DataFormat = DataFormat.Default, IsRequired = true, Name = "url")]
        public string url { get; set; }

        [ProtoMember(4, DataFormat = DataFormat.Default, Name = "version")]
        public List<string> version { get; set; }

        [ProtoMember(5, IsRequired = false, Name = "proxy_ip", DataFormat = DataFormat.Default)]
        public string proxy_ip { get; set; }

        [ProtoMember(6, IsRequired = false, Name = "proxy_port", DataFormat = DataFormat.TwosComplement)]
        public uint proxy_port { get; set; }

        [ProtoMember(7, IsRequired = false, Name = "is_ts", DataFormat = DataFormat.TwosComplement)]
        public uint is_ts { get; set; }


        private IExtension extensionObject;

        IExtension IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
        }
    }
}
