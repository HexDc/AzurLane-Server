using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
    // Token: 0x02000404 RID: 1028
    internal sealed class DateTimeSerializer : IProtoSerializer
	{
		// Token: 0x06003535 RID: 13621 RVA: 0x00133805 File Offset: 0x00131C05
		public DateTimeSerializer(TypeModel model)
		{
			includeKind = (model != null && model.SerializeDateTimeKind());
		}

        // Token: 0x17000339 RID: 825
        // (get) Token: 0x06003536 RID: 13622 RVA: 0x00133822 File Offset: 0x00131C22
        public Type ExpectedType => DateTimeSerializer.expectedType;

        // Token: 0x17000337 RID: 823
        // (get) Token: 0x06003537 RID: 13623 RVA: 0x00133829 File Offset: 0x00131C29
        bool IProtoSerializer.RequiresOldValue => false;

        // Token: 0x17000338 RID: 824
        // (get) Token: 0x06003538 RID: 13624 RVA: 0x0013382C File Offset: 0x00131C2C
        bool IProtoSerializer.ReturnsValue => true;

        // Token: 0x06003539 RID: 13625 RVA: 0x0013382F File Offset: 0x00131C2F
        public object Read(object value, ProtoReader source)
		{
			return BclHelpers.ReadDateTime(source);
		}

		// Token: 0x0600353A RID: 13626 RVA: 0x0013383C File Offset: 0x00131C3C
		public void Write(object value, ProtoWriter dest)
		{
			if (includeKind)
			{
				BclHelpers.WriteDateTimeWithKind((DateTime)value, dest);
			}
			else
			{
				BclHelpers.WriteDateTime((DateTime)value, dest);
			}
		}

		// Token: 0x040024B7 RID: 9399
		private static readonly Type expectedType = typeof(DateTime);

		// Token: 0x040024B8 RID: 9400
		private readonly bool includeKind;
	}
}
