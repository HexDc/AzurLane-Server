using System;
//패치 완료
//https://github.com/RedpointGames/protobuf-net/blob/master/protobuf-net/BclHelpers.cs
namespace ProtoBuf
{
	// Token: 0x020003C0 RID: 960
	public static class BclHelpers
	{
		// Token: 0x06003261 RID: 12897 RVA: 0x001263A8 File Offset: 0x001247A8
		public static object GetUninitializedObject(Type type)
		{
			throw new NotSupportedException("Constructor-skipping is not supported on this platform");
		}

		// Token: 0x06003262 RID: 12898 RVA: 0x001263B4 File Offset: 0x001247B4
		public static void WriteTimeSpan(TimeSpan timeSpan, ProtoWriter dest)
		{
			BclHelpers.WriteTimeSpanImpl(timeSpan, dest, DateTimeKind.Unspecified);
		}

		// Token: 0x06003263 RID: 12899 RVA: 0x001263C0 File Offset: 0x001247C0
		private static void WriteTimeSpanImpl(TimeSpan timeSpan, ProtoWriter dest, DateTimeKind kind)
		{
			if (dest == null)
			{
				throw new ArgumentNullException("dest");
			}
			switch (dest.WireType)
			{
			case WireType.Fixed64:
				ProtoWriter.WriteInt64(timeSpan.Ticks, dest);
				break;
			case WireType.String:
			case WireType.StartGroup:
			{
				long num = timeSpan.Ticks;
				TimeSpanScale timeSpanScale;
				if (timeSpan == TimeSpan.MaxValue)
				{
					num = 1L;
					timeSpanScale = TimeSpanScale.MinMax;
				}
				else if (timeSpan == TimeSpan.MinValue)
				{
					num = -1L;
					timeSpanScale = TimeSpanScale.MinMax;
				}
				else if (num % 864000000000L == 0L)
				{
					timeSpanScale = TimeSpanScale.Days;
					num /= 864000000000L;
				}
				else if (num % 36000000000L == 0L)
				{
					timeSpanScale = TimeSpanScale.Hours;
					num /= 36000000000L;
				}
				else if (num % 600000000L == 0L)
				{
					timeSpanScale = TimeSpanScale.Minutes;
					num /= 600000000L;
				}
				else if (num % 10000000L == 0L)
				{
					timeSpanScale = TimeSpanScale.Seconds;
					num /= 10000000L;
				}
				else if (num % 10000L == 0L)
				{
					timeSpanScale = TimeSpanScale.Milliseconds;
					num /= 10000L;
				}
				else
				{
					timeSpanScale = TimeSpanScale.Ticks;
				}
				SubItemToken token = ProtoWriter.StartSubItem(null, dest);
				if (num != 0L)
				{
					ProtoWriter.WriteFieldHeader(1, WireType.SignedVariant, dest);
					ProtoWriter.WriteInt64(num, dest);
				}
				if (timeSpanScale != TimeSpanScale.Days)
				{
					ProtoWriter.WriteFieldHeader(2, WireType.Variant, dest);
					ProtoWriter.WriteInt32((int)timeSpanScale, dest);
				}
				if (kind != DateTimeKind.Unspecified)
				{
					ProtoWriter.WriteFieldHeader(3, WireType.Variant, dest);
					ProtoWriter.WriteInt32((int)kind, dest);
				}
				ProtoWriter.EndSubItem(token, dest);
				break;
			}
			default:
				throw new ProtoException("Unexpected wire-type: " + dest.WireType.ToString());
			}
		}

		// Token: 0x06003264 RID: 12900 RVA: 0x00126574 File Offset: 0x00124974
		public static TimeSpan ReadTimeSpan(ProtoReader source)
		{
			DateTimeKind dateTimeKind;
			long num = BclHelpers.ReadTimeSpanTicks(source, out dateTimeKind);
			if (num == -9223372036854775808L)
			{
				return TimeSpan.MinValue;
			}
			if (num == 9223372036854775807L)
			{
				return TimeSpan.MaxValue;
			}
			return TimeSpan.FromTicks(num);
		}

		// Token: 0x06003265 RID: 12901 RVA: 0x001265BC File Offset: 0x001249BC
		public static DateTime ReadDateTime(ProtoReader source)
		{
			DateTimeKind dateTimeKind;
			long num = BclHelpers.ReadTimeSpanTicks(source, out dateTimeKind);
			if (num == -9223372036854775808L)
			{
				return DateTime.MinValue;
			}
			if (num == 9223372036854775807L)
			{
				return DateTime.MaxValue;
			}
			return BclHelpers.EpochOrigin[(int)dateTimeKind].AddTicks(num);
		}

		// Token: 0x06003266 RID: 12902 RVA: 0x0012660D File Offset: 0x00124A0D
		public static void WriteDateTime(DateTime value, ProtoWriter dest)
		{
			BclHelpers.WriteDateTimeImpl(value, dest, false);
		}

		// Token: 0x06003267 RID: 12903 RVA: 0x00126617 File Offset: 0x00124A17
		public static void WriteDateTimeWithKind(DateTime value, ProtoWriter dest)
		{
			BclHelpers.WriteDateTimeImpl(value, dest, true);
		}

		// Token: 0x06003268 RID: 12904 RVA: 0x00126624 File Offset: 0x00124A24
		private static void WriteDateTimeImpl(DateTime value, ProtoWriter dest, bool includeKind)
		{
			if (dest == null)
			{
				throw new ArgumentNullException("dest");
			}
			WireType wireType = dest.WireType;
			TimeSpan timeSpan;
			if (wireType != WireType.StartGroup && wireType != WireType.String)
			{
				timeSpan = value - BclHelpers.EpochOrigin[0];
			}
			else if (value == DateTime.MaxValue)
			{
				timeSpan = TimeSpan.MaxValue;
				includeKind = false;
			}
			else if (value == DateTime.MinValue)
			{
				timeSpan = TimeSpan.MinValue;
				includeKind = false;
			}
			else
			{
				timeSpan = value - BclHelpers.EpochOrigin[0];
			}
			BclHelpers.WriteTimeSpanImpl(timeSpan, dest, (!includeKind) ? DateTimeKind.Unspecified : value.Kind);
		}

		// Token: 0x06003269 RID: 12905 RVA: 0x001266EC File Offset: 0x00124AEC
		private static long ReadTimeSpanTicks(ProtoReader source, out DateTimeKind kind)
		{
			kind = DateTimeKind.Unspecified;
			switch (source.WireType)
			{
			case WireType.Fixed64:
				return source.ReadInt64();
			case WireType.String:
			case WireType.StartGroup:
			{
				SubItemToken token = ProtoReader.StartSubItem(source);
				TimeSpanScale timeSpanScale = TimeSpanScale.Days;
				long num = 0L;
				int num2;
				while ((num2 = source.ReadFieldHeader()) > 0)
				{
					switch (num2)
					{
					case 1:
						source.Assert(WireType.SignedVariant);
						num = source.ReadInt64();
						break;
					case 2:
						timeSpanScale = (TimeSpanScale)source.ReadInt32();
						break;
					case 3:
						kind = (DateTimeKind)source.ReadInt32();
						switch (kind)
						{
						case DateTimeKind.Unspecified:
						case DateTimeKind.Utc:
						case DateTimeKind.Local:
							break;
						default:
							throw new ProtoException("Invalid date/time kind: " + kind.ToString());
						}
						break;
					default:
						source.SkipField();
						break;
					}
				}
				ProtoReader.EndSubItem(token, source);
				switch (timeSpanScale)
				{
				case TimeSpanScale.Days:
					return num * 864000000000L;
				case TimeSpanScale.Hours:
					return num * 36000000000L;
				case TimeSpanScale.Minutes:
					return num * 600000000L;
				case TimeSpanScale.Seconds:
					return num * 10000000L;
				case TimeSpanScale.Milliseconds:
					return num * 10000L;
				case TimeSpanScale.Ticks:
					return num;
				default:
					if (timeSpanScale != TimeSpanScale.MinMax)
					{
						throw new ProtoException("Unknown timescale: " + timeSpanScale.ToString());
					}
					if (num == 1L)
					{
						return long.MaxValue;
					}
					if (num != -1L)
					{
						throw new ProtoException("Unknown min/max value: " + num.ToString());
					}
					return long.MinValue;
				}
			}
			default:
				throw new ProtoException("Unexpected wire-type: " + source.WireType.ToString());
			}
		}

		// Token: 0x0600326A RID: 12906 RVA: 0x001268C4 File Offset: 0x00124CC4
		public static decimal ReadDecimal(ProtoReader reader)
		{
			ulong low = 0;
			ulong high = 0;
			ulong signScale = 0;

			int fieldNumber;
			SubItemToken token = ProtoReader.StartSubItem(reader);
			while ((fieldNumber = reader.ReadFieldHeader()) > 0)
			{
				switch (fieldNumber)
				{
					case FieldDecimalLow: low = reader.ReadUInt64(); break;
					case FieldDecimalHigh: high = reader.ReadUInt32(); break;
					case FieldDecimalSignScale: signScale = reader.ReadUInt32(); break;
					default: reader.SkipField(); break;
				}

			}
			ProtoReader.EndSubItem(token, reader);
			if (low == 0 && high == 0) return decimal.Zero;
			int lo = (int)(low & 0xFFFFFFFFL),
			mid = (int)((low >> 32) & 0xFFFFFFFFL),
			hi = (int)high;
			bool isNeg = (signScale & 0x0001) == 0x0001;
			byte scale = (byte)((signScale & 0x01FE) >> 1);
			return new decimal(lo, mid, hi, isNeg, scale);
		}

		// Token: 0x0600326B RID: 12907 RVA: 0x00126990 File Offset: 0x00124D90
		public static void WriteDecimal(decimal value, ProtoWriter writer)
		{
			int[] bits = decimal.GetBits(value);
			ulong a = ((ulong)bits[1]) << 32, b = ((ulong)bits[0]) & 0xFFFFFFFFL;
			ulong low = a | b;
			uint high = (uint)bits[2];
			uint signScale = (uint)(((bits[3] >> 15) & 0x01FE) | ((bits[3] >> 31) & 0x0001));

			SubItemToken token = ProtoWriter.StartSubItem(null, writer);
			if (low != 0)
			{
				ProtoWriter.WriteFieldHeader(FieldDecimalLow, WireType.Variant, writer);
				ProtoWriter.WriteUInt64(low, writer);
			}
			if (high != 0)
			{
				ProtoWriter.WriteFieldHeader(FieldDecimalHigh, WireType.Variant, writer);
				ProtoWriter.WriteUInt32(high, writer);
			}
			if (signScale != 0)
			{
				ProtoWriter.WriteFieldHeader(FieldDecimalSignScale, WireType.Variant, writer);
				ProtoWriter.WriteUInt32(signScale, writer);
			}
			ProtoWriter.EndSubItem(token, writer);
		}

		//public static void WriteDecimal(decimal value, ProtoWriter writer)
		//{
		//	int[] bits = decimal.GetBits(value);
		//	ulong num = (ulong)((ulong)((long)bits[1]) << 32);
		//	ulong num2 = (ulong)((long)bits[0] & (long)((ulong)-1));
		//	ulong num3 = num | num2;
		//	uint num4 = (uint)bits[2];
		//	uint num5 = (uint)((bits[3] >> 15 & 510) | (bits[3] >> 31 & 1));
		//	SubItemToken token = ProtoWriter.StartSubItem(null, writer);
		//	if (num3 != 0UL)
		//	{
		//		ProtoWriter.WriteFieldHeader(1, WireType.Variant, writer);
		//		ProtoWriter.WriteUInt64(num3, writer);
		//	}
		//	if (num4 != 0U)
		//	{
		//		ProtoWriter.WriteFieldHeader(2, WireType.Variant, writer);
		//		ProtoWriter.WriteUInt32(num4, writer);
		//	}
		//	if (num5 != 0U)
		//	{
		//		ProtoWriter.WriteFieldHeader(3, WireType.Variant, writer);
		//		ProtoWriter.WriteUInt32(num5, writer);
		//	}
		//	ProtoWriter.EndSubItem(token, writer);
		//}

		// Token: 0x0600326C RID: 12908 RVA: 0x00126A2C File Offset: 0x00124E2C

		public static void WriteGuid(Guid value, ProtoWriter dest)
		{
			byte[] data = value.ToByteArray();
			SubItemToken token = ProtoWriter.StartSubItem(null, dest);
			if (value != Guid.Empty)
			{
				ProtoWriter.WriteFieldHeader(1, WireType.Fixed64, dest);
				ProtoWriter.WriteBytes(data, 0, 8, dest);
				ProtoWriter.WriteFieldHeader(2, WireType.Fixed64, dest);
				ProtoWriter.WriteBytes(data, 8, 8, dest);
			}
			ProtoWriter.EndSubItem(token, dest);
		}

		// Token: 0x0600326D RID: 12909 RVA: 0x00126A84 File Offset: 0x00124E84
		public static Guid ReadGuid(ProtoReader source)
		{
			ulong num = 0UL;
			ulong num2 = 0UL;
			SubItemToken token = ProtoReader.StartSubItem(source);
			int num3;
			while ((num3 = source.ReadFieldHeader()) > 0)
			{
				if (num3 != 1)
				{
					if (num3 != 2)
					{
						source.SkipField();
					}
					else
					{
						num2 = source.ReadUInt64();
					}
				}
				else
				{
					num = source.ReadUInt64();
				}
			}
			ProtoReader.EndSubItem(token, source);
			if (num == 0UL && num2 == 0UL)
			{
				return Guid.Empty;
			}
			uint num4 = (uint)(num >> 32);
			uint a = (uint)num;
			uint num5 = (uint)(num2 >> 32);
			uint num6 = (uint)num2;
			return new Guid((int)a, (short)num4, (short)(num4 >> 16), (byte)num6, (byte)(num6 >> 8), (byte)(num6 >> 16), (byte)(num6 >> 24), (byte)num5, (byte)(num5 >> 8), (byte)(num5 >> 16), (byte)(num5 >> 24));
		}

		// Token: 0x0600326E RID: 12910 RVA: 0x00126B54 File Offset: 0x00124F54
		public static object ReadNetObject(object value, ProtoReader source, int key, Type type, BclHelpers.NetObjectOptions options)
		{
			SubItemToken token = ProtoReader.StartSubItem(source);
			int num = -1;
			int num2 = -1;
			int num3;
			while ((num3 = source.ReadFieldHeader()) > 0)
			{
				switch (num3)
				{
				case 1:
				{
					int key2 = source.ReadInt32();
					value = source.NetCache.GetKeyedObject(key2);
					continue;
				}
				case 2:
					num = source.ReadInt32();
					continue;
				case 3:
				{
					int key2 = source.ReadInt32();
					type = (Type)source.NetCache.GetKeyedObject(key2);
					key = source.GetTypeKey(ref type);
					continue;
				}
				case 4:
					num2 = source.ReadInt32();
					continue;
				case 8:
				{
					string text = source.ReadString();
					type = source.DeserializeType(text);
					if (type == null)
					{
						throw new ProtoException("Unable to resolve type: " + text + " (you can use the TypeModel.DynamicTypeFormatting event to provide a custom mapping)");
					}
					if (type == typeof(string))
					{
						key = -1;
					}
					else
					{
						key = source.GetTypeKey(ref type);
						if (key < 0)
						{
							throw new InvalidOperationException("Dynamic type is not a contract-type: " + type.Name);
						}
					}
					continue;
				}
				case 10:
				{
					bool flag = type == typeof(string);
					bool flag2 = value == null;
					bool flag3 = flag2 && (flag || (byte)(options & BclHelpers.NetObjectOptions.LateSet) != 0);
					if (num >= 0 && !flag3)
					{
						if (value == null)
						{
							source.TrapNextObject(num);
						}
						else
						{
							source.NetCache.SetKeyedObject(num, value);
						}
						if (num2 >= 0)
						{
							source.NetCache.SetKeyedObject(num2, type);
						}
					}
					object obj = value;
					if (flag)
					{
						value = source.ReadString();
					}
					else
					{
						value = ProtoReader.ReadTypedObject(obj, key, source, type);
					}
					if (num >= 0)
					{
						if (flag2 && !flag3)
						{
							obj = source.NetCache.GetKeyedObject(num);
						}
						if (flag3)
						{
							source.NetCache.SetKeyedObject(num, value);
							if (num2 >= 0)
							{
								source.NetCache.SetKeyedObject(num2, type);
							}
						}
					}
					if (num >= 0 && !flag3 && !object.ReferenceEquals(obj, value))
					{
						throw new ProtoException("A reference-tracked object changed reference during deserialization");
					}
					if (num < 0 && num2 >= 0)
					{
						source.NetCache.SetKeyedObject(num2, type);
					}
					continue;
				}
				}
				source.SkipField();
			}
			if (num >= 0 && (byte)(options & BclHelpers.NetObjectOptions.AsReference) == 0)
			{
				throw new ProtoException("Object key in input stream, but reference-tracking was not expected");
			}
			ProtoReader.EndSubItem(token, source);
			return value;
		}

		// Token: 0x0600326F RID: 12911 RVA: 0x00126DE0 File Offset: 0x001251E0
		public static void WriteNetObject(object value, ProtoWriter dest, int key, BclHelpers.NetObjectOptions options)
		{
			if (dest == null)
			{
				throw new ArgumentNullException("dest");
			}
			bool flag = (byte)(options & BclHelpers.NetObjectOptions.DynamicType) != 0;
			bool flag2 = (byte)(options & BclHelpers.NetObjectOptions.AsReference) != 0;
			WireType wireType = dest.WireType;
			SubItemToken token = ProtoWriter.StartSubItem(null, dest);
			bool flag3 = true;
			if (flag2)
			{
				bool flag4;
				int value2 = dest.NetCache.AddObjectKey(value, out flag4);
				ProtoWriter.WriteFieldHeader((!flag4) ? 2 : 1, WireType.Variant, dest);
				ProtoWriter.WriteInt32(value2, dest);
				if (flag4)
				{
					flag3 = false;
				}
			}
			if (flag3)
			{
				if (flag)
				{
					Type type = value.GetType();
					if (!(value is string))
					{
						key = dest.GetTypeKey(ref type);
						if (key < 0)
						{
							throw new InvalidOperationException("Dynamic type is not a contract-type: " + type.Name);
						}
					}
					bool flag5;
					int value3 = dest.NetCache.AddObjectKey(type, out flag5);
					ProtoWriter.WriteFieldHeader((!flag5) ? 4 : 3, WireType.Variant, dest);
					ProtoWriter.WriteInt32(value3, dest);
					if (!flag5)
					{
						ProtoWriter.WriteFieldHeader(8, WireType.String, dest);
						ProtoWriter.WriteString(dest.SerializeType(type), dest);
					}
				}
				ProtoWriter.WriteFieldHeader(10, wireType, dest);
				if (value is string)
				{
					ProtoWriter.WriteString((string)value, dest);
				}
				else
				{
					ProtoWriter.WriteObject(value, key, dest);
				}
			}
			ProtoWriter.EndSubItem(token, dest);
		}

		// Token: 0x040023A7 RID: 9127
		private const int FieldTimeSpanValue = 1;

		// Token: 0x040023A8 RID: 9128
		private const int FieldTimeSpanScale = 2;

		// Token: 0x040023A9 RID: 9129
		private const int FieldTimeSpanKind = 3;

		// Token: 0x040023AA RID: 9130
		internal static readonly DateTime[] EpochOrigin = new DateTime[]
		{
			new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
			new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
			new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local)
		};

		// Token: 0x040023AB RID: 9131
		private const int FieldDecimalLow = 1;

		// Token: 0x040023AC RID: 9132
		private const int FieldDecimalHigh = 2;

		// Token: 0x040023AD RID: 9133
		private const int FieldDecimalSignScale = 3;

		// Token: 0x040023AE RID: 9134
		private const int FieldGuidLow = 1;

		// Token: 0x040023AF RID: 9135
		private const int FieldGuidHigh = 2;

		// Token: 0x040023B0 RID: 9136
		private const int FieldExistingObjectKey = 1;

		// Token: 0x040023B1 RID: 9137
		private const int FieldNewObjectKey = 2;

		// Token: 0x040023B2 RID: 9138
		private const int FieldExistingTypeKey = 3;

		// Token: 0x040023B3 RID: 9139
		private const int FieldNewTypeKey = 4;

		// Token: 0x040023B4 RID: 9140
		private const int FieldTypeName = 8;

		// Token: 0x040023B5 RID: 9141
		private const int FieldObject = 10;

		// Token: 0x020003C1 RID: 961
		[Flags]
		public enum NetObjectOptions : byte
		{
			// Token: 0x040023B7 RID: 9143
			None = 0,
			// Token: 0x040023B8 RID: 9144
			AsReference = 1,
			// Token: 0x040023B9 RID: 9145
			DynamicType = 2,
			// Token: 0x040023BA RID: 9146
			UseConstructor = 4,
			// Token: 0x040023BB RID: 9147
			LateSet = 8
		}
	}
}
