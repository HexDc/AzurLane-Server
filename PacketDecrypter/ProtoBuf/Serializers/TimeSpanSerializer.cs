using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
    // Token: 0x02000421 RID: 1057
    internal sealed class TimeSpanSerializer : IProtoSerializer
	{
		// Token: 0x06003602 RID: 13826 RVA: 0x0013568A File Offset: 0x00133A8A
		public TimeSpanSerializer(TypeModel model)
		{
		}

        // Token: 0x1700038E RID: 910
        // (get) Token: 0x06003603 RID: 13827 RVA: 0x00135692 File Offset: 0x00133A92
        public Type ExpectedType => expectedType;

        // Token: 0x1700038C RID: 908
        // (get) Token: 0x06003604 RID: 13828 RVA: 0x00135699 File Offset: 0x00133A99
        bool IProtoSerializer.RequiresOldValue => false;

        // Token: 0x1700038D RID: 909
        // (get) Token: 0x06003605 RID: 13829 RVA: 0x0013569C File Offset: 0x00133A9C
        bool IProtoSerializer.ReturnsValue => true;

        // Token: 0x06003606 RID: 13830 RVA: 0x0013569F File Offset: 0x00133A9F
        public object Read(object value, ProtoReader source)
		{
			return BclHelpers.ReadTimeSpan(source);
		}

		// Token: 0x06003607 RID: 13831 RVA: 0x001356AC File Offset: 0x00133AAC
		public void Write(object value, ProtoWriter dest)
		{
			BclHelpers.WriteTimeSpan((TimeSpan)value, dest);
		}

		// Token: 0x040024F6 RID: 9462
		private static readonly Type expectedType = typeof(TimeSpan);
	}
}
