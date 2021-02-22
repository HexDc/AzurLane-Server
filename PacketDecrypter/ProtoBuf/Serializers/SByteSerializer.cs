using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
    // Token: 0x0200041A RID: 1050
    internal sealed class SByteSerializer : IProtoSerializer
	{
		// Token: 0x060035C5 RID: 13765 RVA: 0x001350C4 File Offset: 0x001334C4
		public SByteSerializer(TypeModel model)
		{
		}

        // Token: 0x17000378 RID: 888
        // (get) Token: 0x060035C6 RID: 13766 RVA: 0x001350CC File Offset: 0x001334CC
        public Type ExpectedType => expectedType;

        // Token: 0x17000376 RID: 886
        // (get) Token: 0x060035C7 RID: 13767 RVA: 0x001350D3 File Offset: 0x001334D3
        bool IProtoSerializer.RequiresOldValue => false;

        // Token: 0x17000377 RID: 887
        // (get) Token: 0x060035C8 RID: 13768 RVA: 0x001350D6 File Offset: 0x001334D6
        bool IProtoSerializer.ReturnsValue => true;

        // Token: 0x060035C9 RID: 13769 RVA: 0x001350D9 File Offset: 0x001334D9
        public object Read(object value, ProtoReader source)
		{
			return source.ReadSByte();
		}

		// Token: 0x060035CA RID: 13770 RVA: 0x001350E6 File Offset: 0x001334E6
		public void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteSByte((sbyte)value, dest);
		}

		// Token: 0x040024E6 RID: 9446
		private static readonly Type expectedType = typeof(sbyte);
	}
}
