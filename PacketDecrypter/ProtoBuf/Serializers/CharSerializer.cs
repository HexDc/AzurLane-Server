using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	// Token: 0x02000403 RID: 1027
	internal sealed class CharSerializer : UInt16Serializer
	{
		// Token: 0x06003530 RID: 13616 RVA: 0x001337C9 File Offset: 0x00131BC9
		public CharSerializer(TypeModel model) : base(model)
		{
		}

		// Token: 0x17000336 RID: 822
		// (get) Token: 0x06003531 RID: 13617 RVA: 0x001337D2 File Offset: 0x00131BD2
		public override Type ExpectedType
		{
			get
			{
				return CharSerializer.expectedType;
			}
		}

		// Token: 0x06003532 RID: 13618 RVA: 0x001337D9 File Offset: 0x00131BD9
		public override void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteUInt16((ushort)((char)value), dest);
		}

		// Token: 0x06003533 RID: 13619 RVA: 0x001337E7 File Offset: 0x00131BE7
		public override object Read(object value, ProtoReader source)
		{
			return (char)source.ReadUInt16();
		}

		// Token: 0x040024B6 RID: 9398
		private static readonly Type expectedType = typeof(char);
	}
}
