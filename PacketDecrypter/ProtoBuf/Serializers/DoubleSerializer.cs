using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
    // Token: 0x02000407 RID: 1031
    internal sealed class DoubleSerializer : IProtoSerializer
	{
		// Token: 0x06003549 RID: 13641 RVA: 0x00133963 File Offset: 0x00131D63
		public DoubleSerializer(TypeModel model)
		{
		}

        // Token: 0x17000342 RID: 834
        // (get) Token: 0x0600354A RID: 13642 RVA: 0x0013396B File Offset: 0x00131D6B
        public Type ExpectedType => DoubleSerializer.expectedType;

        // Token: 0x17000340 RID: 832
        // (get) Token: 0x0600354B RID: 13643 RVA: 0x00133972 File Offset: 0x00131D72
        bool IProtoSerializer.RequiresOldValue => false;

        // Token: 0x17000341 RID: 833
        // (get) Token: 0x0600354C RID: 13644 RVA: 0x00133975 File Offset: 0x00131D75
        bool IProtoSerializer.ReturnsValue => true;

        // Token: 0x0600354D RID: 13645 RVA: 0x00133978 File Offset: 0x00131D78
        public object Read(object value, ProtoReader source)
		{
			return source.ReadDouble();
		}

		// Token: 0x0600354E RID: 13646 RVA: 0x00133985 File Offset: 0x00131D85
		public void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteDouble((double)value, dest);
		}

		// Token: 0x040024BB RID: 9403
		private static readonly Type expectedType = typeof(double);
	}
}
