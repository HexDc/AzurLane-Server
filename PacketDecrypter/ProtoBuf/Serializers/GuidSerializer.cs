using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
    // Token: 0x0200040B RID: 1035
    internal sealed class GuidSerializer : IProtoSerializer
	{
		// Token: 0x06003560 RID: 13664 RVA: 0x00133DEC File Offset: 0x001321EC
		public GuidSerializer(TypeModel model)
		{
		}

        // Token: 0x1700034B RID: 843
        // (get) Token: 0x06003561 RID: 13665 RVA: 0x00133DF4 File Offset: 0x001321F4
        public Type ExpectedType => expectedType;

        // Token: 0x17000349 RID: 841
        // (get) Token: 0x06003562 RID: 13666 RVA: 0x00133DFB File Offset: 0x001321FB
        bool IProtoSerializer.RequiresOldValue => false;

        // Token: 0x1700034A RID: 842
        // (get) Token: 0x06003563 RID: 13667 RVA: 0x00133DFE File Offset: 0x001321FE
        bool IProtoSerializer.ReturnsValue => true;

        // Token: 0x06003564 RID: 13668 RVA: 0x00133E01 File Offset: 0x00132201
        public void Write(object value, ProtoWriter dest)
		{
			BclHelpers.WriteGuid((Guid)value, dest);
		}

		// Token: 0x06003565 RID: 13669 RVA: 0x00133E0F File Offset: 0x0013220F
		public object Read(object value, ProtoReader source)
		{
			return BclHelpers.ReadGuid(source);
		}

		// Token: 0x040024C3 RID: 9411
		private static readonly Type expectedType = typeof(Guid);
	}
}
