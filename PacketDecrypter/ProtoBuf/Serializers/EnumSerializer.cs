using System;

namespace ProtoBuf.Serializers
{
    // Token: 0x02000408 RID: 1032
    internal sealed class EnumSerializer : IProtoSerializer
	{
		// Token: 0x06003550 RID: 13648 RVA: 0x001339A4 File Offset: 0x00131DA4
		public EnumSerializer(Type enumType, EnumSerializer.EnumPair[] map)
		{
			if (enumType == null)
			{
				throw new ArgumentNullException("enumType");
			}
			this.enumType = enumType;
			this.map = map;
			if (map != null)
			{
				for (int i = 1; i < map.Length; i++)
				{
					for (int j = 0; j < i; j++)
					{
						if (map[i].WireValue == map[j].WireValue && !object.Equals(map[i].RawValue, map[j].RawValue))
						{
							throw new ProtoException("Multiple enums with wire-value " + map[i].WireValue.ToString());
						}
						if (object.Equals(map[i].RawValue, map[j].RawValue) && map[i].WireValue != map[j].WireValue)
						{
							throw new ProtoException("Multiple enums with deserialized-value " + map[i].RawValue);
						}
					}
				}
			}
		}

		// Token: 0x06003551 RID: 13649 RVA: 0x00133AC8 File Offset: 0x00131EC8
		private ProtoTypeCode GetTypeCode()
		{
			Type underlyingType = Helpers.GetUnderlyingType(enumType);
			if (underlyingType == null)
			{
				underlyingType = enumType;
			}
			return Helpers.GetTypeCode(underlyingType);
		}

        // Token: 0x17000345 RID: 837
        // (get) Token: 0x06003552 RID: 13650 RVA: 0x00133AF4 File Offset: 0x00131EF4
        public Type ExpectedType => enumType;

        // Token: 0x17000343 RID: 835
        // (get) Token: 0x06003553 RID: 13651 RVA: 0x00133AFC File Offset: 0x00131EFC
        bool IProtoSerializer.RequiresOldValue => false;

        // Token: 0x17000344 RID: 836
        // (get) Token: 0x06003554 RID: 13652 RVA: 0x00133AFF File Offset: 0x00131EFF
        bool IProtoSerializer.ReturnsValue => true;

        // Token: 0x06003555 RID: 13653 RVA: 0x00133B04 File Offset: 0x00131F04
        private int EnumToWire(object value)
		{
			switch (GetTypeCode())
			{
			case ProtoTypeCode.SByte:
				return (sbyte)value;
			case ProtoTypeCode.Byte:
				return (byte)value;
			case ProtoTypeCode.Int16:
				return (short)value;
			case ProtoTypeCode.UInt16:
				return (ushort)value;
			case ProtoTypeCode.Int32:
				return (int)value;
			case ProtoTypeCode.UInt32:
				return (int)((uint)value);
			case ProtoTypeCode.Int64:
				return (int)((long)value);
			case ProtoTypeCode.UInt64:
				return (int)((ulong)value);
			default:
				throw new InvalidOperationException();
			}
		}

		// Token: 0x06003556 RID: 13654 RVA: 0x00133B88 File Offset: 0x00131F88
		private object WireToEnum(int value)
		{
			switch (GetTypeCode())
			{
			case ProtoTypeCode.SByte:
				return Enum.ToObject(enumType, (sbyte)value);
			case ProtoTypeCode.Byte:
				return Enum.ToObject(enumType, (byte)value);
			case ProtoTypeCode.Int16:
				return Enum.ToObject(enumType, (short)value);
			case ProtoTypeCode.UInt16:
				return Enum.ToObject(enumType, (ushort)value);
			case ProtoTypeCode.Int32:
				return Enum.ToObject(enumType, value);
			case ProtoTypeCode.UInt32:
				return Enum.ToObject(enumType, (uint)value);
			case ProtoTypeCode.Int64:
				return Enum.ToObject(enumType, (long)value);
			case ProtoTypeCode.UInt64:
				return Enum.ToObject(enumType, (ulong)value);
			default:
				throw new InvalidOperationException();
			}
		}

		// Token: 0x06003557 RID: 13655 RVA: 0x00133C3C File Offset: 0x0013203C
		public object Read(object value, ProtoReader source)
		{
			int num = source.ReadInt32();
			if (map == null)
			{
				return WireToEnum(num);
			}
			for (int i = 0; i < map.Length; i++)
			{
				if (map[i].WireValue == num)
				{
					return map[i].TypedValue;
				}
			}
			source.ThrowEnumException(ExpectedType, num);
			return null;
		}

		// Token: 0x06003558 RID: 13656 RVA: 0x00133CB4 File Offset: 0x001320B4
		public void Write(object value, ProtoWriter dest)
		{
			if (map == null)
			{
				ProtoWriter.WriteInt32(EnumToWire(value), dest);
			}
			else
			{
				for (int i = 0; i < map.Length; i++)
				{
					if (object.Equals(map[i].TypedValue, value))
					{
						ProtoWriter.WriteInt32(map[i].WireValue, dest);
						return;
					}
				}
				ProtoWriter.ThrowEnumException(dest, value);
			}
		}

		// Token: 0x040024BC RID: 9404
		private readonly Type enumType;

		// Token: 0x040024BD RID: 9405
		private readonly EnumSerializer.EnumPair[] map;

		// Token: 0x02000409 RID: 1033
		public struct EnumPair
		{
			// Token: 0x06003559 RID: 13657 RVA: 0x00133D32 File Offset: 0x00132132
			public EnumPair(int wireValue, object raw, Type type)
			{
				WireValue = wireValue;
				RawValue = raw;
				TypedValue = (Enum)Enum.ToObject(type, raw);
			}

			// Token: 0x040024BE RID: 9406
			public readonly object RawValue;

			// Token: 0x040024BF RID: 9407
			public readonly Enum TypedValue;

			// Token: 0x040024C0 RID: 9408
			public readonly int WireValue;
		}
	}
}
