using ProtoBuf.Serializers;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ProtoBuf.Meta
{
    // Token: 0x020003E1 RID: 993
    public sealed class SubType
	{
		// Token: 0x0600339B RID: 13211 RVA: 0x0012E608 File Offset: 0x0012CA08
		public SubType(int fieldNumber, MetaType derivedType, DataFormat format)
		{
			if (derivedType == null)
			{
				throw new ArgumentNullException("derivedType");
			}
			if (fieldNumber <= 0)
			{
				throw new ArgumentOutOfRangeException("fieldNumber");
			}
			this.fieldNumber = fieldNumber;
			this.derivedType = derivedType;
			dataFormat = format;
		}

        // Token: 0x170002E3 RID: 739
        // (get) Token: 0x0600339C RID: 13212 RVA: 0x0012E648 File Offset: 0x0012CA48
        public int FieldNumber => fieldNumber;

        // Token: 0x170002E4 RID: 740
        // (get) Token: 0x0600339D RID: 13213 RVA: 0x0012E650 File Offset: 0x0012CA50
        public MetaType DerivedType => derivedType;

        // Token: 0x170002E5 RID: 741
        // (get) Token: 0x0600339E RID: 13214 RVA: 0x0012E658 File Offset: 0x0012CA58
        internal IProtoSerializer Serializer
		{
			get
			{
				if (serializer == null)
				{
					serializer = BuildSerializer();
				}
				return serializer;
			}
		}

		// Token: 0x0600339F RID: 13215 RVA: 0x0012E678 File Offset: 0x0012CA78
		private IProtoSerializer BuildSerializer()
		{
			WireType wireType = WireType.String;
			if (dataFormat == DataFormat.Group)
			{
				wireType = WireType.StartGroup;
			}
			IProtoSerializer tail = new SubItemSerializer(derivedType.Type, derivedType.GetKey(false, false), derivedType, false);
			return new TagDecorator(fieldNumber, wireType, false, tail);
		}

		// Token: 0x04002422 RID: 9250
		private readonly int fieldNumber;

		// Token: 0x04002423 RID: 9251
		private readonly MetaType derivedType;

		// Token: 0x04002424 RID: 9252
		private readonly DataFormat dataFormat;

		// Token: 0x04002425 RID: 9253
		private IProtoSerializer serializer;

		// Token: 0x020003E2 RID: 994
		internal sealed class Comparer : IComparer, IComparer<SubType>
		{
			// Token: 0x060033A1 RID: 13217 RVA: 0x0012E6D0 File Offset: 0x0012CAD0
			public int Compare(object x, object y)
			{
				return Compare(x as SubType, y as SubType);
			}

			// Token: 0x060033A2 RID: 13218 RVA: 0x0012E6E4 File Offset: 0x0012CAE4
			public int Compare(SubType x, SubType y)
			{
				if (ReferenceEquals(x, y)) return 0;
				if (x == null) return -1;
				if (y == null) return 1;

				return x.FieldNumber.CompareTo(y.FieldNumber);
			}

			// Token: 0x04002426 RID: 9254
			public static readonly Comparer Default = new Comparer();
		}
	}
}
