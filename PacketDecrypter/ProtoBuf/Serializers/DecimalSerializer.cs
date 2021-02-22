using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	// Token: 0x02000405 RID: 1029
	internal sealed class DecimalSerializer : IProtoSerializer
	{
		// Token: 0x0600353C RID: 13628 RVA: 0x00133877 File Offset: 0x00131C77
		public DecimalSerializer(TypeModel model)
		{
		}

		// Token: 0x1700033C RID: 828
		// (get) Token: 0x0600353D RID: 13629 RVA: 0x0013387F File Offset: 0x00131C7F
		public Type ExpectedType
		{
			get
			{
				return DecimalSerializer.expectedType;
			}
		}

		// Token: 0x1700033A RID: 826
		// (get) Token: 0x0600353E RID: 13630 RVA: 0x00133886 File Offset: 0x00131C86
		bool IProtoSerializer.RequiresOldValue
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700033B RID: 827
		// (get) Token: 0x0600353F RID: 13631 RVA: 0x00133889 File Offset: 0x00131C89
		bool IProtoSerializer.ReturnsValue
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003540 RID: 13632 RVA: 0x0013388C File Offset: 0x00131C8C
		public object Read(object value, ProtoReader source)
		{
			return BclHelpers.ReadDecimal(source);
		}

		// Token: 0x06003541 RID: 13633 RVA: 0x00133899 File Offset: 0x00131C99
		public void Write(object value, ProtoWriter dest)
		{
			BclHelpers.WriteDecimal((decimal)value, dest);
		}

		// Token: 0x040024B9 RID: 9401
		private static readonly Type expectedType = typeof(decimal);
	}
}
