using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
    // Token: 0x02000412 RID: 1042
    internal sealed class Int64Serializer : IProtoSerializer
	{
		// Token: 0x06003584 RID: 13700 RVA: 0x00134A9A File Offset: 0x00132E9A
		public Int64Serializer(TypeModel model)
		{
		}

        // Token: 0x17000359 RID: 857
        // (get) Token: 0x06003585 RID: 13701 RVA: 0x00134AA2 File Offset: 0x00132EA2
        public Type ExpectedType => Int64Serializer.expectedType;

        // Token: 0x17000357 RID: 855
        // (get) Token: 0x06003586 RID: 13702 RVA: 0x00134AA9 File Offset: 0x00132EA9
        bool IProtoSerializer.RequiresOldValue => false;

        // Token: 0x17000358 RID: 856
        // (get) Token: 0x06003587 RID: 13703 RVA: 0x00134AAC File Offset: 0x00132EAC
        bool IProtoSerializer.ReturnsValue => true;

        // Token: 0x06003588 RID: 13704 RVA: 0x00134AAF File Offset: 0x00132EAF
        public object Read(object value, ProtoReader source)
		{
			return source.ReadInt64();
		}

		// Token: 0x06003589 RID: 13705 RVA: 0x00134ABC File Offset: 0x00132EBC
		public void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteInt64((long)value, dest);
		}

		// Token: 0x040024CA RID: 9418
		private static readonly Type expectedType = typeof(long);
	}
}
