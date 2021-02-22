using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	// Token: 0x02000401 RID: 1025
	internal sealed class BooleanSerializer : IProtoSerializer
	{
		// Token: 0x06003522 RID: 13602 RVA: 0x00133706 File Offset: 0x00131B06
		public BooleanSerializer(TypeModel model)
		{
		}

		// Token: 0x17000332 RID: 818
		// (get) Token: 0x06003523 RID: 13603 RVA: 0x0013370E File Offset: 0x00131B0E
		public Type ExpectedType
		{
			get
			{
				return BooleanSerializer.expectedType;
			}
		}

		// Token: 0x06003524 RID: 13604 RVA: 0x00133715 File Offset: 0x00131B15
		public void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteBoolean((bool)value, dest);
		}

		// Token: 0x06003525 RID: 13605 RVA: 0x00133723 File Offset: 0x00131B23
		public object Read(object value, ProtoReader source)
		{
			return source.ReadBoolean();
		}

		// Token: 0x17000330 RID: 816
		// (get) Token: 0x06003526 RID: 13606 RVA: 0x00133730 File Offset: 0x00131B30
		bool IProtoSerializer.RequiresOldValue
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000331 RID: 817
		// (get) Token: 0x06003527 RID: 13607 RVA: 0x00133733 File Offset: 0x00131B33
		bool IProtoSerializer.ReturnsValue
		{
			get
			{
				return true;
			}
		}

		// Token: 0x040024B4 RID: 9396
		private static readonly Type expectedType = typeof(bool);
	}
}
