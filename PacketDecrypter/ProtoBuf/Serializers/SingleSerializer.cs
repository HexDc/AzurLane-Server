using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
    // Token: 0x0200041B RID: 1051
    internal sealed class SingleSerializer : IProtoSerializer
	{
		// Token: 0x060035CC RID: 13772 RVA: 0x00135105 File Offset: 0x00133505
		public SingleSerializer(TypeModel model)
		{
		}

        // Token: 0x1700037B RID: 891
        // (get) Token: 0x060035CD RID: 13773 RVA: 0x0013510D File Offset: 0x0013350D
        public Type ExpectedType => SingleSerializer.expectedType;

        // Token: 0x17000379 RID: 889
        // (get) Token: 0x060035CE RID: 13774 RVA: 0x00135114 File Offset: 0x00133514
        bool IProtoSerializer.RequiresOldValue => false;

        // Token: 0x1700037A RID: 890
        // (get) Token: 0x060035CF RID: 13775 RVA: 0x00135117 File Offset: 0x00133517
        bool IProtoSerializer.ReturnsValue => true;

        // Token: 0x060035D0 RID: 13776 RVA: 0x0013511A File Offset: 0x0013351A
        public object Read(object value, ProtoReader source)
		{
			return source.ReadSingle();
		}

		// Token: 0x060035D1 RID: 13777 RVA: 0x00135127 File Offset: 0x00133527
		public void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteSingle((float)value, dest);
		}

		// Token: 0x040024E7 RID: 9447
		private static readonly Type expectedType = typeof(float);
	}
}
