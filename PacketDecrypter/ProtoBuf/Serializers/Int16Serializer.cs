using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
    // Token: 0x02000410 RID: 1040
    internal sealed class Int16Serializer : IProtoSerializer
	{
		// Token: 0x06003576 RID: 13686 RVA: 0x00134A18 File Offset: 0x00132E18
		public Int16Serializer(TypeModel model)
		{
		}

        // Token: 0x17000353 RID: 851
        // (get) Token: 0x06003577 RID: 13687 RVA: 0x00134A20 File Offset: 0x00132E20
        public Type ExpectedType => Int16Serializer.expectedType;

        // Token: 0x17000351 RID: 849
        // (get) Token: 0x06003578 RID: 13688 RVA: 0x00134A27 File Offset: 0x00132E27
        bool IProtoSerializer.RequiresOldValue => false;

        // Token: 0x17000352 RID: 850
        // (get) Token: 0x06003579 RID: 13689 RVA: 0x00134A2A File Offset: 0x00132E2A
        bool IProtoSerializer.ReturnsValue => true;

        // Token: 0x0600357A RID: 13690 RVA: 0x00134A2D File Offset: 0x00132E2D
        public object Read(object value, ProtoReader source)
		{
			return source.ReadInt16();
		}

		// Token: 0x0600357B RID: 13691 RVA: 0x00134A3A File Offset: 0x00132E3A
		public void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteInt16((short)value, dest);
		}

		// Token: 0x040024C8 RID: 9416
		private static readonly Type expectedType = typeof(short);
	}
}
