using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	// Token: 0x02000402 RID: 1026
	internal sealed class ByteSerializer : IProtoSerializer
	{
		// Token: 0x06003529 RID: 13609 RVA: 0x00133747 File Offset: 0x00131B47
		public ByteSerializer(TypeModel model)
		{
		}

		// Token: 0x17000335 RID: 821
		// (get) Token: 0x0600352A RID: 13610 RVA: 0x0013374F File Offset: 0x00131B4F
		public Type ExpectedType
		{
			get
			{
				return ByteSerializer.expectedType;
			}
		}

		// Token: 0x17000333 RID: 819
		// (get) Token: 0x0600352B RID: 13611 RVA: 0x00133756 File Offset: 0x00131B56
		bool IProtoSerializer.RequiresOldValue
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000334 RID: 820
		// (get) Token: 0x0600352C RID: 13612 RVA: 0x00133759 File Offset: 0x00131B59
		bool IProtoSerializer.ReturnsValue
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600352D RID: 13613 RVA: 0x0013375C File Offset: 0x00131B5C
		public void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteByte((byte)value, dest);
		}

		// Token: 0x0600352E RID: 13614 RVA: 0x0013376A File Offset: 0x00131B6A
		public object Read(object value, ProtoReader source)
		{
			return source.ReadByte();
		}

		// Token: 0x040024B5 RID: 9397
		private static readonly Type expectedType = typeof(byte);
	}
}
