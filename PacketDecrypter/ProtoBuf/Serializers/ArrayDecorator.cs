using ProtoBuf.Meta;
using System;
using System.Collections;

namespace ProtoBuf.Serializers
{
    // Token: 0x020003FF RID: 1023
    internal sealed class ArrayDecorator : ProtoDecoratorBase
	{
		// Token: 0x06003513 RID: 13587 RVA: 0x001333E8 File Offset: 0x001317E8
		public ArrayDecorator(TypeModel model, IProtoSerializer tail, int fieldNumber, bool writePacked, WireType packedWireType, Type arrayType, bool overwriteList, bool supportNull) : base(tail)
		{
			itemType = arrayType.GetElementType();
			Type type = (!supportNull) ? (Helpers.GetUnderlyingType(itemType) ?? itemType) : itemType;
			if ((writePacked || packedWireType != WireType.None) && fieldNumber <= 0)
			{
				throw new ArgumentOutOfRangeException("fieldNumber");
			}
			if (!ListDecorator.CanPack(packedWireType))
			{
				if (writePacked)
				{
					throw new InvalidOperationException("Only simple data-types can use packed encoding");
				}
				packedWireType = WireType.None;
			}
			this.fieldNumber = fieldNumber;
			this.packedWireType = packedWireType;
			if (writePacked)
			{
				options |= 1;
			}
			if (overwriteList)
			{
				options |= 2;
			}
			if (supportNull)
			{
				options |= 4;
			}
			this.arrayType = arrayType;
		}

        // Token: 0x17000328 RID: 808
        // (get) Token: 0x06003514 RID: 13588 RVA: 0x001334CF File Offset: 0x001318CF
        public override Type ExpectedType => arrayType;

        // Token: 0x17000329 RID: 809
        // (get) Token: 0x06003515 RID: 13589 RVA: 0x001334D7 File Offset: 0x001318D7
        public override bool RequiresOldValue => AppendToCollection;

        // Token: 0x1700032A RID: 810
        // (get) Token: 0x06003516 RID: 13590 RVA: 0x001334DF File Offset: 0x001318DF
        public override bool ReturnsValue => true;

        // Token: 0x1700032B RID: 811
        // (get) Token: 0x06003517 RID: 13591 RVA: 0x001334E2 File Offset: 0x001318E2
        private bool AppendToCollection => (options & 2) == 0;

        // Token: 0x1700032C RID: 812
        // (get) Token: 0x06003518 RID: 13592 RVA: 0x001334EF File Offset: 0x001318EF
        private bool SupportNull => (options & 4) != 0;

        // Token: 0x06003519 RID: 13593 RVA: 0x00133500 File Offset: 0x00131900
        public override void Write(object value, ProtoWriter dest)
		{
			IList list = (IList)value;
			int count = list.Count;
			bool flag = (options & 1) != 0;
			SubItemToken token;
			if (flag)
			{
				ProtoWriter.WriteFieldHeader(fieldNumber, WireType.String, dest);
				token = ProtoWriter.StartSubItem(value, dest);
				ProtoWriter.SetPackedField(fieldNumber, dest);
			}
			else
			{
				token = default(SubItemToken);
			}
			bool flag2 = !SupportNull;
			for (int i = 0; i < count; i++)
			{
				object obj = list[i];
				if (flag2 && obj == null)
				{
					throw new NullReferenceException();
				}
				Tail.Write(obj, dest);
			}
			if (flag)
			{
				ProtoWriter.EndSubItem(token, dest);
			}
		}

		// Token: 0x0600351A RID: 13594 RVA: 0x001335B8 File Offset: 0x001319B8
		public override object Read(object value, ProtoReader source)
		{
			int field = source.FieldNumber;
			BasicList basicList = new BasicList();
			if (packedWireType != WireType.None && source.WireType == WireType.String)
			{
				SubItemToken token = ProtoReader.StartSubItem(source);
				while (ProtoReader.HasSubValue(packedWireType, source))
				{
					basicList.Add(Tail.Read(null, source));
				}
				ProtoReader.EndSubItem(token, source);
			}
			else
			{
				do
				{
					basicList.Add(Tail.Read(null, source));
				}
				while (source.TryReadFieldHeader(field));
			}
			int num = (!AppendToCollection) ? 0 : ((value != null) ? ((Array)value).Length : 0);
			Array array = Array.CreateInstance(itemType, num + basicList.Count);
			if (num != 0)
			{
				((Array)value).CopyTo(array, 0);
			}
			basicList.CopyTo(array, num);
			return array;
		}

		// Token: 0x040024AA RID: 9386
		private readonly int fieldNumber;

		// Token: 0x040024AB RID: 9387
		private const byte OPTIONS_WritePacked = 1;

		// Token: 0x040024AC RID: 9388
		private const byte OPTIONS_OverwriteList = 2;

		// Token: 0x040024AD RID: 9389
		private const byte OPTIONS_SupportNull = 4;

		// Token: 0x040024AE RID: 9390
		private readonly byte options;

		// Token: 0x040024AF RID: 9391
		private readonly WireType packedWireType;

		// Token: 0x040024B0 RID: 9392
		private readonly Type arrayType;

		// Token: 0x040024B1 RID: 9393
		private readonly Type itemType;
	}
}
