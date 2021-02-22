using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf.Meta;
//패치
//https://github.com/RedpointGames/protobuf-net/blob/master/protobuf-net/ProtoReader.cs
namespace ProtoBuf
{
	// Token: 0x020003F8 RID: 1016
	public sealed class ProtoReader : IDisposable
	{
		// Token: 0x0600346F RID: 13423 RVA: 0x0012FD5E File Offset: 0x0012E15E
		public ProtoReader(Stream source, TypeModel model, SerializationContext context)
		{
			ProtoReader.Init(this, source, model, context, -1);
		}

		// Token: 0x06003470 RID: 13424 RVA: 0x0012FD70 File Offset: 0x0012E170
		public ProtoReader(Stream source, TypeModel model, SerializationContext context, int length)
		{
			ProtoReader.Init(this, source, model, context, length);
		}

		// Token: 0x1700031A RID: 794
		// (get) Token: 0x06003471 RID: 13425 RVA: 0x0012FD83 File Offset: 0x0012E183
		public int FieldNumber
		{
			get
			{
				return this.fieldNumber;
			}
		}

		// Token: 0x1700031B RID: 795
		// (get) Token: 0x06003472 RID: 13426 RVA: 0x0012FD8B File Offset: 0x0012E18B
		public WireType WireType
		{
			get
			{
				return this.wireType;
			}
		}

		// Token: 0x1700031C RID: 796
		// (get) Token: 0x06003473 RID: 13427 RVA: 0x0012FD93 File Offset: 0x0012E193
		// (set) Token: 0x06003474 RID: 13428 RVA: 0x0012FD9B File Offset: 0x0012E19B
		public bool InternStrings
		{
			get
			{
				return this.internStrings;
			}
			set
			{
				this.internStrings = value;
			}
		}

		// Token: 0x06003475 RID: 13429 RVA: 0x0012FDA4 File Offset: 0x0012E1A4
		private static void Init(ProtoReader reader, Stream source, TypeModel model, SerializationContext context, int length)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!source.CanRead)
			{
				throw new ArgumentException("Cannot read from stream", "source");
			}
			reader.source = source;
			reader.ioBuffer = BufferPool.GetBuffer();
			reader.model = model;
			bool flag = length >= 0;
			reader.isFixedLength = flag;
			reader.dataRemaining = ((!flag) ? 0 : length);
			if (context == null)
			{
				context = SerializationContext.Default;
			}
			else
			{
				context.Freeze();
			}
			reader.context = context;
			reader.position = (reader.available = (reader.depth = (reader.fieldNumber = (reader.ioIndex = 0))));
			reader.blockEnd = int.MaxValue;
			reader.internStrings = true;
			reader.wireType = WireType.None;
			reader.trapCount = 1U;
			if (reader.netCache == null)
			{
				reader.netCache = new NetObjectCache();
			}
		}

		// Token: 0x1700031D RID: 797
		// (get) Token: 0x06003476 RID: 13430 RVA: 0x0012FE9A File Offset: 0x0012E29A
		public SerializationContext Context
		{
			get
			{
				return this.context;
			}
		}

		// Token: 0x06003477 RID: 13431 RVA: 0x0012FEA4 File Offset: 0x0012E2A4
		public void Dispose()
		{
			this.source = null;
			this.model = null;
			BufferPool.ReleaseBufferToPool(ref this.ioBuffer);
			if (this.stringInterner != null)
			{
				this.stringInterner.Clear();
			}
			if (this.netCache != null)
			{
				this.netCache.Clear();
			}
		}

		// Token: 0x06003478 RID: 13432 RVA: 0x0012FEF8 File Offset: 0x0012E2F8
		internal int TryReadUInt32VariantWithoutMoving(bool trimNegative, out uint value)
		{
			if (this.available < 10)
			{
				this.Ensure(10, false);
			}
			if (this.available == 0)
			{
				value = 0U;
				return 0;
			}
			int num = this.ioIndex;
			value = (uint)this.ioBuffer[num++];
			if ((value & 128U) == 0U)
			{
				return 1;
			}
			value &= 127U;
			if (this.available == 1)
			{
				throw ProtoReader.EoF(this);
			}
			uint num2 = (uint)this.ioBuffer[num++];
			value |= (num2 & 127U) << 7;
			if ((num2 & 128U) == 0U)
			{
				return 2;
			}
			if (this.available == 2)
			{
				throw ProtoReader.EoF(this);
			}
			num2 = (uint)this.ioBuffer[num++];
			value |= (num2 & 127U) << 14;
			if ((num2 & 128U) == 0U)
			{
				return 3;
			}
			if (this.available == 3)
			{
				throw ProtoReader.EoF(this);
			}
			num2 = (uint)this.ioBuffer[num++];
			value |= (num2 & 127U) << 21;
			if ((num2 & 128U) == 0U)
			{
				return 4;
			}
			if (this.available == 4)
			{
				throw ProtoReader.EoF(this);
			}
			num2 = (uint)this.ioBuffer[num];
			value |= num2 << 28;
			if ((num2 & 240U) == 0U)
			{
				return 5;
			}
			if (trimNegative && (num2 & 240U) == 240U && this.available >= 10 && this.ioBuffer[++num] == 255 && this.ioBuffer[++num] == 255 && this.ioBuffer[++num] == 255 && this.ioBuffer[++num] == 255 && this.ioBuffer[num + 1] == 1)
			{
				return 10;
			}
			throw ProtoReader.AddErrorData(new OverflowException(), this);
		}

		// Token: 0x06003479 RID: 13433 RVA: 0x001300D4 File Offset: 0x0012E4D4
		private uint ReadUInt32Variant(bool trimNegative)
		{
			uint result;
			int num = this.TryReadUInt32VariantWithoutMoving(trimNegative, out result);
			if (num > 0)
			{
				this.ioIndex += num;
				this.available -= num;
				this.position += num;
				return result;
			}
			throw ProtoReader.EoF(this);
		}

		// Token: 0x0600347A RID: 13434 RVA: 0x00130124 File Offset: 0x0012E524
		private bool TryReadUInt32Variant(out uint value)
		{
			int num = this.TryReadUInt32VariantWithoutMoving(false, out value);
			if (num > 0)
			{
				this.ioIndex += num;
				this.available -= num;
				this.position += num;
				return true;
			}
			return false;
		}

		// Token: 0x0600347B RID: 13435 RVA: 0x00130170 File Offset: 0x0012E570
		public uint ReadUInt32()
		{
			switch (this.wireType)
			{
			case WireType.Variant:
				return this.ReadUInt32Variant(false);
			case WireType.Fixed64:
			{
				ulong num = this.ReadUInt64();
				return checked((uint)num);
			}
			case WireType.Fixed32:
				if (this.available < 4)
				{
					this.Ensure(4, true);
				}
				this.position += 4;
				this.available -= 4;
				return (uint)((int)this.ioBuffer[this.ioIndex++] | (int)this.ioBuffer[this.ioIndex++] << 8 | (int)this.ioBuffer[this.ioIndex++] << 16 | (int)this.ioBuffer[this.ioIndex++] << 24);
			}
			throw this.CreateWireTypeException();
		}

		// Token: 0x1700031E RID: 798
		// (get) Token: 0x0600347C RID: 13436 RVA: 0x0013025B File Offset: 0x0012E65B
		public int Position
		{
			get
			{
				return this.position;
			}
		}

		// Token: 0x0600347D RID: 13437 RVA: 0x00130264 File Offset: 0x0012E664
		internal void Ensure(int count, bool strict)
		{
			if (count > this.ioBuffer.Length)
			{
				BufferPool.ResizeAndFlushLeft(ref this.ioBuffer, count, this.ioIndex, this.available);
				this.ioIndex = 0;
			}
			else if (this.ioIndex + count >= this.ioBuffer.Length)
			{
				Helpers.BlockCopy(this.ioBuffer, this.ioIndex, this.ioBuffer, 0, this.available);
				this.ioIndex = 0;
			}
			count -= this.available;
			int num = this.ioIndex + this.available;
			int num2 = this.ioBuffer.Length - num;
			if (this.isFixedLength && this.dataRemaining < num2)
			{
				num2 = this.dataRemaining;
			}
			int num3;
			while (count > 0 && num2 > 0 && (num3 = this.source.Read(this.ioBuffer, num, num2)) > 0)
			{
				this.available += num3;
				count -= num3;
				num2 -= num3;
				num += num3;
				if (this.isFixedLength)
				{
					this.dataRemaining -= num3;
				}
			}
			if (strict && count > 0)
			{
				throw ProtoReader.EoF(this);
			}
		}

		// Token: 0x0600347E RID: 13438 RVA: 0x00130394 File Offset: 0x0012E794
		public short ReadInt16()
		{
			return checked((short)this.ReadInt32());
		}

		// Token: 0x0600347F RID: 13439 RVA: 0x0013039D File Offset: 0x0012E79D
		public ushort ReadUInt16()
		{
			return checked((ushort)this.ReadUInt32());
		}

		// Token: 0x06003480 RID: 13440 RVA: 0x001303A6 File Offset: 0x0012E7A6
		public byte ReadByte()
		{
			return checked((byte)this.ReadUInt32());
		}

		// Token: 0x06003481 RID: 13441 RVA: 0x001303AF File Offset: 0x0012E7AF
		public sbyte ReadSByte()
		{
			return checked((sbyte)this.ReadInt32());
		}

		// Token: 0x06003482 RID: 13442 RVA: 0x001303B8 File Offset: 0x0012E7B8
		public int ReadInt32()
		{
			switch (wireType)
			{
				case WireType.Variant:
					return (int)ReadUInt32Variant(true);
				case WireType.Fixed32:
					if (available < 4) Ensure(4, true);
					position += 4;
					available -= 4;
					return ioBuffer[ioIndex++]
                        | (ioBuffer[ioIndex++] << 8)
						| (ioBuffer[ioIndex++] << 16)
						| (ioBuffer[ioIndex++] << 24);
				case WireType.Fixed64:
					long l = ReadInt64();
					checked { return (int)l; }
				case WireType.SignedVariant:
					return Zag(ReadUInt32Variant(true));
				default:
					throw CreateWireTypeException();
			}
		}

		// Token: 0x06003483 RID: 13443 RVA: 0x001304B8 File Offset: 0x0012E8B8
		private static int Zag(uint ziggedValue)
		{
			int value = (int)ziggedValue;
			return (-(value & 0x01)) ^ ((value >> 1) & ~ProtoReader.Int32Msb);
		}

		// Token: 0x06003484 RID: 13444 RVA: 0x001304D8 File Offset: 0x0012E8D8
		private static long Zag(ulong ziggedValue)
		{
			long value = (long)ziggedValue;
			return (-(value & 0x01L)) ^ ((value >> 1) & ~ProtoReader.Int64Msb);
		}

		// Token: 0x06003485 RID: 13445 RVA: 0x001304FC File Offset: 0x0012E8FC
		public long ReadInt64()
		{
			switch (wireType)
			{
				case WireType.Variant:
					return (long)ReadUInt64Variant();
				case WireType.Fixed32:
					return ReadInt32();
				case WireType.Fixed64:
					if (available < 8) Ensure(8, true);
					position += 8;
					available -= 8;

					return ((long)ioBuffer[ioIndex++])
						| (((long)ioBuffer[ioIndex++]) << 8)
						| (((long)ioBuffer[ioIndex++]) << 16)
						| (((long)ioBuffer[ioIndex++]) << 24)
						| (((long)ioBuffer[ioIndex++]) << 32)
						| (((long)ioBuffer[ioIndex++]) << 40)
						| (((long)ioBuffer[ioIndex++]) << 48)
						| (((long)ioBuffer[ioIndex++]) << 56);

				case WireType.SignedVariant:
					return Zag(ReadUInt64Variant());
				default:
					throw CreateWireTypeException();
			}
		}

		// Token: 0x06003486 RID: 13446 RVA: 0x00130670 File Offset: 0x0012EA70
		private int TryReadUInt64VariantWithoutMoving(out ulong value)
		{
			if (available < 10) Ensure(10, false);
			if (available == 0)
			{
				value = 0;
				return 0;
			}
			int readPos = ioIndex;
			value = ioBuffer[readPos++];
			if ((value & 0x80) == 0) return 1;
			value &= 0x7F;
			if (available == 1) throw EoF(this);

			ulong chunk = ioBuffer[readPos++];
			value |= (chunk & 0x7F) << 7;
			if ((chunk & 0x80) == 0) return 2;
			if (available == 2) throw EoF(this);

			chunk = ioBuffer[readPos++];
			value |= (chunk & 0x7F) << 14;
			if ((chunk & 0x80) == 0) return 3;
			if (available == 3) throw EoF(this);

			chunk = ioBuffer[readPos++];
			value |= (chunk & 0x7F) << 21;
			if ((chunk & 0x80) == 0) return 4;
			if (available == 4) throw EoF(this);

			chunk = ioBuffer[readPos++];
			value |= (chunk & 0x7F) << 28;
			if ((chunk & 0x80) == 0) return 5;
			if (available == 5) throw EoF(this);

			chunk = ioBuffer[readPos++];
			value |= (chunk & 0x7F) << 35;
			if ((chunk & 0x80) == 0) return 6;
			if (available == 6) throw EoF(this);

			chunk = ioBuffer[readPos++];
			value |= (chunk & 0x7F) << 42;
			if ((chunk & 0x80) == 0) return 7;
			if (available == 7) throw EoF(this);


			chunk = ioBuffer[readPos++];
			value |= (chunk & 0x7F) << 49;
			if ((chunk & 0x80) == 0) return 8;
			if (available == 8) throw EoF(this);

			chunk = ioBuffer[readPos++];
			value |= (chunk & 0x7F) << 56;
			if ((chunk & 0x80) == 0) return 9;
			if (available == 9) throw EoF(this);

			chunk = ioBuffer[readPos];
			value |= chunk << 63; // can only use 1 bit from this chunk

			if ((chunk & ~(ulong)0x01) != 0) throw AddErrorData(new OverflowException(), this);
			return 10;
		}

		// Token: 0x06003487 RID: 13447 RVA: 0x00130910 File Offset: 0x0012ED10
		private ulong ReadUInt64Variant()
		{
			ulong result;
			int num = this.TryReadUInt64VariantWithoutMoving(out result);
			if (num > 0)
			{
				this.ioIndex += num;
				this.available -= num;
				this.position += num;
				return result;
			}
			throw ProtoReader.EoF(this);
		}

		// Token: 0x06003488 RID: 13448 RVA: 0x00130960 File Offset: 0x0012ED60
		private string Intern(string value)
		{
			if (value == null)
			{
				return null;
			}
			if (value.Length == 0)
			{
				return string.Empty;
			}
			string text;
			if (this.stringInterner == null)
			{
				this.stringInterner = new Dictionary<string, string>();
				this.stringInterner.Add(value, value);
			}
			else if (this.stringInterner.TryGetValue(value, out text))
			{
				value = text;
			}
			else
			{
				this.stringInterner.Add(value, value);
			}
			return value;
		}

		// Token: 0x06003489 RID: 13449 RVA: 0x001309D8 File Offset: 0x0012EDD8
		public string ReadString()
		{
			if (this.wireType != WireType.String)
			{
				throw this.CreateWireTypeException();
			}
			int num = (int)this.ReadUInt32Variant(false);
			if (num == 0)
			{
				return string.Empty;
			}
			if (this.available < num)
			{
				this.Ensure(num, true);
			}
			string text = ProtoReader.encoding.GetString(this.ioBuffer, this.ioIndex, num);
			if (this.internStrings)
			{
				text = this.Intern(text);
			}
			this.available -= num;
			this.position += num;
			this.ioIndex += num;
			return text;
		}

		// Token: 0x0600348A RID: 13450 RVA: 0x00130A78 File Offset: 0x0012EE78
		public void ThrowEnumException(Type type, int value)
		{
			string str = (type != null) ? type.FullName : "<null>";
			throw ProtoReader.AddErrorData(new ProtoException("No " + str + " enum is mapped to the wire-value " + value.ToString()), this);
		}

		// Token: 0x0600348B RID: 13451 RVA: 0x00130AC4 File Offset: 0x0012EEC4
		private Exception CreateWireTypeException()
		{
			return this.CreateException("Invalid wire-type; this usually means you have over-written a file without truncating or setting the length; see http://stackoverflow.com/q/2152978/23354");
		}

		// Token: 0x0600348C RID: 13452 RVA: 0x00130AD1 File Offset: 0x0012EED1
		private Exception CreateException(string message)
		{
			return ProtoReader.AddErrorData(new ProtoException(message), this);
		}

		// Token: 0x0600348D RID: 13453 RVA: 0x00130AE0 File Offset: 0x0012EEE0
		public double ReadDouble()
		{
			WireType wireType = this.wireType;
			if (wireType == WireType.Fixed32)
			{
				return (double)this.ReadSingle();
			}
			if (wireType != WireType.Fixed64)
			{
				throw this.CreateWireTypeException();
			}
			return (double)this.ReadInt64();
		}

		// Token: 0x0600348E RID: 13454 RVA: 0x00130B20 File Offset: 0x0012EF20
		public static object ReadObject(object value, int key, ProtoReader reader)
		{
			return ProtoReader.ReadTypedObject(value, key, reader, null);
		}

		// Token: 0x0600348F RID: 13455 RVA: 0x00130B2C File Offset: 0x0012EF2C
		internal static object ReadTypedObject(object value, int key, ProtoReader reader, Type type)
		{
			if (reader.model == null)
			{
				throw ProtoReader.AddErrorData(new InvalidOperationException("Cannot deserialize sub-objects unless a model is provided"), reader);
			}
			SubItemToken token = ProtoReader.StartSubItem(reader);
			if (key >= 0)
			{
				value = reader.model.Deserialize(key, value, reader);
			}
			else if (type == null || !reader.model.TryDeserializeAuxiliaryType(reader, DataFormat.Default, 1, type, ref value, true, false, true, false))
			{
				TypeModel.ThrowUnexpectedType(type);
			}
			ProtoReader.EndSubItem(token, reader);
			return value;
		}

		// Token: 0x06003490 RID: 13456 RVA: 0x00130BAC File Offset: 0x0012EFAC
		public static void EndSubItem(SubItemToken token, ProtoReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			int value = token.value;
			WireType wireType = reader.wireType;
			if (wireType != WireType.EndGroup)
			{
				if (value < reader.position)
				{
					throw reader.CreateException("Sub-message not read entirely");
				}
				if (reader.blockEnd != reader.position && reader.blockEnd != 2147483647)
				{
					throw reader.CreateException("Sub-message not read correctly");
				}
				reader.blockEnd = value;
				reader.depth--;
			}
			else
			{
				if (value >= 0)
				{
					throw ProtoReader.AddErrorData(new ArgumentException("token"), reader);
				}
				if (-value != reader.fieldNumber)
				{
					throw reader.CreateException("Wrong group was ended");
				}
				reader.wireType = WireType.None;
				reader.depth--;
			}
		}

		// Token: 0x06003491 RID: 13457 RVA: 0x00130C90 File Offset: 0x0012F090
		public static SubItemToken StartSubItem(ProtoReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			WireType wireType = reader.wireType;
			if (wireType == WireType.StartGroup)
			{
				reader.wireType = WireType.None;
				reader.depth++;
				return new SubItemToken(-reader.fieldNumber);
			}
			if (wireType != WireType.String)
			{
				throw reader.CreateWireTypeException();
			}
			int num = (int)reader.ReadUInt32Variant(false);
			if (num < 0)
			{
				throw ProtoReader.AddErrorData(new InvalidOperationException(), reader);
			}
			int value = reader.blockEnd;
			reader.blockEnd = reader.position + num;
			reader.depth++;
			return new SubItemToken(value);
		}

		// Token: 0x06003492 RID: 13458 RVA: 0x00130D38 File Offset: 0x0012F138
		public int ReadFieldHeader()
		{
			if (this.blockEnd <= this.position || this.wireType == WireType.EndGroup)
			{
				return 0;
			}
			uint num;
			if (this.TryReadUInt32Variant(out num) && num != 0U)
			{
				this.wireType = (WireType)(num & 7U);
				this.fieldNumber = (int)(num >> 3);
				if (this.fieldNumber < 1)
				{
					throw new ProtoException("Invalid field in source data: " + this.fieldNumber.ToString());
				}
			}
			else
			{
				this.wireType = WireType.None;
				this.fieldNumber = 0;
			}
			if (this.wireType != WireType.EndGroup)
			{
				return this.fieldNumber;
			}
			if (this.depth > 0)
			{
				return 0;
			}
			throw new ProtoException("Unexpected end-group in source data; this usually means the source data is corrupt");
		}

		// Token: 0x06003493 RID: 13459 RVA: 0x00130DF4 File Offset: 0x0012F1F4
		public bool TryReadFieldHeader(int field)
		{
			if (this.blockEnd <= this.position || this.wireType == WireType.EndGroup)
			{
				return false;
			}
			uint num2;
			int num = this.TryReadUInt32VariantWithoutMoving(false, out num2);
			WireType wireType;
			if (num > 0 && (int)num2 >> 3 == field && (wireType = (WireType)(num2 & 7U)) != WireType.EndGroup)
			{
				this.wireType = wireType;
				this.fieldNumber = field;
				this.position += num;
				this.ioIndex += num;
				this.available -= num;
				return true;
			}
			return false;
		}

		// Token: 0x1700031F RID: 799
		// (get) Token: 0x06003494 RID: 13460 RVA: 0x00130E80 File Offset: 0x0012F280
		public TypeModel Model
		{
			get
			{
				return this.model;
			}
		}

		// Token: 0x06003495 RID: 13461 RVA: 0x00130E88 File Offset: 0x0012F288
		public void Hint(WireType wireType)
		{
			if (this.wireType != wireType)
			{
				if ((wireType & (WireType)7) == this.wireType)
				{
					this.wireType = wireType;
				}
			}
		}

		// Token: 0x06003496 RID: 13462 RVA: 0x00130EB0 File Offset: 0x0012F2B0
		public void Assert(WireType wireType)
		{
			if (this.wireType != wireType)
			{
				if ((wireType & (WireType)7) != this.wireType)
				{
					throw this.CreateWireTypeException();
				}
				this.wireType = wireType;
			}
		}

		// Token: 0x06003497 RID: 13463 RVA: 0x00130EE4 File Offset: 0x0012F2E4
		public void SkipField()
		{
			WireType wireType = this.wireType;
			switch (wireType + 1)
			{
			case WireType.Fixed64:
			case (WireType)9:
				this.ReadUInt64Variant();
				return;
			case WireType.String:
				if (this.available < 8)
				{
					this.Ensure(8, true);
				}
				this.available -= 8;
				this.ioIndex += 8;
				this.position += 8;
				return;
			case WireType.StartGroup:
			{
				int num = (int)this.ReadUInt32Variant(false);
				if (num <= this.available)
				{
					this.available -= num;
					this.ioIndex += num;
					this.position += num;
					return;
				}
				this.position += num;
				num -= this.available;
				this.ioIndex = (this.available = 0);
				if (this.isFixedLength)
				{
					if (num > this.dataRemaining)
					{
						throw ProtoReader.EoF(this);
					}
					this.dataRemaining -= num;
				}
				ProtoReader.Seek(this.source, num, this.ioBuffer);
				return;
			}
			case WireType.EndGroup:
			{
				int num2 = this.fieldNumber;
				this.depth++;
				while (this.ReadFieldHeader() > 0)
				{
					this.SkipField();
				}
				this.depth--;
				if (this.wireType == WireType.EndGroup && this.fieldNumber == num2)
				{
					this.wireType = WireType.None;
					return;
				}
				throw this.CreateWireTypeException();
			}
			case (WireType)6:
				if (this.available < 4)
				{
					this.Ensure(4, true);
				}
				this.available -= 4;
				this.ioIndex += 4;
				this.position += 4;
				return;
			}
			throw this.CreateWireTypeException();
		}

		// Token: 0x06003498 RID: 13464 RVA: 0x001310C0 File Offset: 0x0012F4C0
		public ulong ReadUInt64()
		{
			switch (this.wireType)
			{
			case WireType.Variant:
				return this.ReadUInt64Variant();
			case WireType.Fixed64:
				if (this.available < 8)
				{
					this.Ensure(8, true);
				}
				this.position += 8;
				this.available -= 8;
				return (ulong)this.ioBuffer[this.ioIndex++] | (ulong)this.ioBuffer[this.ioIndex++] << 8 | (ulong)this.ioBuffer[this.ioIndex++] << 16 | (ulong)this.ioBuffer[this.ioIndex++] << 24 | (ulong)this.ioBuffer[this.ioIndex++] << 32 | (ulong)this.ioBuffer[this.ioIndex++] << 40 | (ulong)this.ioBuffer[this.ioIndex++] << 48 | (ulong)this.ioBuffer[this.ioIndex++] << 56;
			case WireType.Fixed32:
				return (ulong)this.ReadUInt32();
			}
			throw this.CreateWireTypeException();
		}

		// Token: 0x06003499 RID: 13465 RVA: 0x00131220 File Offset: 0x0012F620
		public float ReadSingle()
		{
			WireType wireType = this.wireType;
			if (wireType == WireType.Fixed32)
			{
				return (float)this.ReadInt32();
			}
			if (wireType != WireType.Fixed64)
			{
				throw this.CreateWireTypeException();
			}
			double num = this.ReadDouble();
			float num2 = (float)num;
			if (Helpers.IsInfinity(num2) && !Helpers.IsInfinity(num))
			{
				throw ProtoReader.AddErrorData(new OverflowException(), this);
			}
			return num2;
		}

		// Token: 0x0600349A RID: 13466 RVA: 0x00131288 File Offset: 0x0012F688
		public bool ReadBoolean()
		{
			uint num = this.ReadUInt32();
			if (num == 0U)
			{
				return false;
			}
			if (num != 1U)
			{
				throw this.CreateException("Unexpected boolean value");
			}
			return true;
		}

		// Token: 0x0600349B RID: 13467 RVA: 0x001312C0 File Offset: 0x0012F6C0
		public static byte[] AppendBytes(byte[] value, ProtoReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			WireType wireType = reader.wireType;
			if (wireType != WireType.String)
			{
				if (wireType != WireType.Variant)
				{
					throw reader.CreateWireTypeException();
				}
				return new byte[0];
			}
			else
			{
				int i = (int)reader.ReadUInt32Variant(false);
				reader.wireType = WireType.None;
				if (i == 0)
				{
					return (value != null) ? value : ProtoReader.EmptyBlob;
				}
				int num;
				if (value == null || value.Length == 0)
				{
					num = 0;
					value = new byte[i];
				}
				else
				{
					num = value.Length;
					byte[] array = new byte[value.Length + i];
					Helpers.BlockCopy(value, 0, array, 0, value.Length);
					value = array;
				}
				reader.position += i;
				while (i > reader.available)
				{
					if (reader.available > 0)
					{
						Helpers.BlockCopy(reader.ioBuffer, reader.ioIndex, value, num, reader.available);
						i -= reader.available;
						num += reader.available;
						reader.ioIndex = (reader.available = 0);
					}
					int num2 = (i <= reader.ioBuffer.Length) ? i : reader.ioBuffer.Length;
					if (num2 > 0)
					{
						reader.Ensure(num2, true);
					}
				}
				if (i > 0)
				{
					Helpers.BlockCopy(reader.ioBuffer, reader.ioIndex, value, num, i);
					reader.ioIndex += i;
					reader.available -= i;
				}
				return value;
			}
		}

		// Token: 0x0600349C RID: 13468 RVA: 0x00131438 File Offset: 0x0012F838
		private static int ReadByteOrThrow(Stream source)
		{
			int num = source.ReadByte();
			if (num < 0)
			{
				throw ProtoReader.EoF(null);
			}
			return num;
		}

		// Token: 0x0600349D RID: 13469 RVA: 0x0013145C File Offset: 0x0012F85C
		public static int ReadLengthPrefix(Stream source, bool expectHeader, PrefixStyle style, out int fieldNumber)
		{
			int num;
			return ProtoReader.ReadLengthPrefix(source, expectHeader, style, out fieldNumber, out num);
		}

		// Token: 0x0600349E RID: 13470 RVA: 0x00131474 File Offset: 0x0012F874
		public static int DirectReadLittleEndianInt32(Stream source)
		{
			return ProtoReader.ReadByteOrThrow(source) | ProtoReader.ReadByteOrThrow(source) << 8 | ProtoReader.ReadByteOrThrow(source) << 16 | ProtoReader.ReadByteOrThrow(source) << 24;
		}

		// Token: 0x0600349F RID: 13471 RVA: 0x00131499 File Offset: 0x0012F899
		public static int DirectReadBigEndianInt32(Stream source)
		{
			return ProtoReader.ReadByteOrThrow(source) << 24 | ProtoReader.ReadByteOrThrow(source) << 16 | ProtoReader.ReadByteOrThrow(source) << 8 | ProtoReader.ReadByteOrThrow(source);
		}

		// Token: 0x060034A0 RID: 13472 RVA: 0x001314C0 File Offset: 0x0012F8C0
		public static int DirectReadVarintInt32(Stream source)
		{
			uint result;
			int num = ProtoReader.TryReadUInt32Variant(source, out result);
			if (num <= 0)
			{
				throw ProtoReader.EoF(null);
			}
			return (int)result;
		}

		// Token: 0x060034A1 RID: 13473 RVA: 0x001314E8 File Offset: 0x0012F8E8
		public static void DirectReadBytes(Stream source, byte[] buffer, int offset, int count)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			int num;
			while (count > 0 && (num = source.Read(buffer, offset, count)) > 0)
			{
				count -= num;
				offset += num;
			}
			if (count > 0)
			{
				throw ProtoReader.EoF(null);
			}
		}

		// Token: 0x060034A2 RID: 13474 RVA: 0x0013153C File Offset: 0x0012F93C
		public static byte[] DirectReadBytes(Stream source, int count)
		{
			byte[] array = new byte[count];
			ProtoReader.DirectReadBytes(source, array, 0, count);
			return array;
		}

		// Token: 0x060034A3 RID: 13475 RVA: 0x0013155C File Offset: 0x0012F95C
		public static string DirectReadString(Stream source, int length)
		{
			byte[] array = new byte[length];
			ProtoReader.DirectReadBytes(source, array, 0, length);
			return Encoding.UTF8.GetString(array, 0, length);
		}

		// Token: 0x060034A4 RID: 13476 RVA: 0x00131588 File Offset: 0x0012F988
		public static int ReadLengthPrefix(Stream source, bool expectHeader, PrefixStyle style, out int fieldNumber, out int bytesRead)
		{
			fieldNumber = 0;
			switch (style)
			{
			case PrefixStyle.None:
				bytesRead = 0;
				return int.MaxValue;
			case PrefixStyle.Base128:
			{
				bytesRead = 0;
				uint num2;
				int num;
				if (!expectHeader)
				{
					num = ProtoReader.TryReadUInt32Variant(source, out num2);
					bytesRead += num;
					return (int)((bytesRead >= 0) ? num2 : uint.MaxValue);
				}
				num = ProtoReader.TryReadUInt32Variant(source, out num2);
				bytesRead += num;
				if (num <= 0)
				{
					bytesRead = 0;
					return -1;
				}
				if ((num2 & 7U) != 2U)
				{
					throw new InvalidOperationException();
				}
				fieldNumber = (int)(num2 >> 3);
				num = ProtoReader.TryReadUInt32Variant(source, out num2);
				bytesRead += num;
				if (bytesRead == 0)
				{
					throw ProtoReader.EoF(null);
				}
				return (int)num2;
			}
			case PrefixStyle.Fixed32:
			{
				int num3 = source.ReadByte();
				if (num3 < 0)
				{
					bytesRead = 0;
					return -1;
				}
				bytesRead = 4;
				return num3 | ProtoReader.ReadByteOrThrow(source) << 8 | ProtoReader.ReadByteOrThrow(source) << 16 | ProtoReader.ReadByteOrThrow(source) << 24;
			}
			case PrefixStyle.Fixed32BigEndian:
			{
				int num4 = source.ReadByte();
				if (num4 < 0)
				{
					bytesRead = 0;
					return -1;
				}
				bytesRead = 4;
				return num4 << 24 | ProtoReader.ReadByteOrThrow(source) << 16 | ProtoReader.ReadByteOrThrow(source) << 8 | ProtoReader.ReadByteOrThrow(source);
			}
			default:
				throw new ArgumentOutOfRangeException("style");
			}
		}

		// Token: 0x060034A5 RID: 13477 RVA: 0x001316B8 File Offset: 0x0012FAB8
		private static int TryReadUInt32Variant(Stream source, out uint value)
		{
			value = 0U;
			int num = source.ReadByte();
			if (num < 0)
			{
				return 0;
			}
			value = (uint)num;
			if ((value & 128U) == 0U)
			{
				return 1;
			}
			value &= 127U;
			num = source.ReadByte();
			if (num < 0)
			{
				throw ProtoReader.EoF(null);
			}
			value |= (uint)((uint)(num & 127) << 7);
			if ((num & 128) == 0)
			{
				return 2;
			}
			num = source.ReadByte();
			if (num < 0)
			{
				throw ProtoReader.EoF(null);
			}
			value |= (uint)((uint)(num & 127) << 14);
			if ((num & 128) == 0)
			{
				return 3;
			}
			num = source.ReadByte();
			if (num < 0)
			{
				throw ProtoReader.EoF(null);
			}
			value |= (uint)((uint)(num & 127) << 21);
			if ((num & 128) == 0)
			{
				return 4;
			}
			num = source.ReadByte();
			if (num < 0)
			{
				throw ProtoReader.EoF(null);
			}
			value |= (uint)((uint)num << 28);
			if ((num & 240) == 0)
			{
				return 5;
			}
			throw new OverflowException();
		}

		// Token: 0x060034A6 RID: 13478 RVA: 0x001317B0 File Offset: 0x0012FBB0
		internal static void Seek(Stream source, int count, byte[] buffer)
		{
			if (source.CanSeek)
			{
				source.Seek((long)count, SeekOrigin.Current);
				count = 0;
			}
			else if (buffer != null)
			{
				int num;
				while (count > buffer.Length && (num = source.Read(buffer, 0, buffer.Length)) > 0)
				{
					count -= num;
				}
				while (count > 0 && (num = source.Read(buffer, 0, count)) > 0)
				{
					count -= num;
				}
			}
			else
			{
				buffer = BufferPool.GetBuffer();
				try
				{
					int num2;
					while (count > buffer.Length && (num2 = source.Read(buffer, 0, buffer.Length)) > 0)
					{
						count -= num2;
					}
					while (count > 0 && (num2 = source.Read(buffer, 0, count)) > 0)
					{
						count -= num2;
					}
				}
				finally
				{
					BufferPool.ReleaseBufferToPool(ref buffer);
				}
			}
			if (count > 0)
			{
				throw ProtoReader.EoF(null);
			}
		}

		// Token: 0x060034A7 RID: 13479 RVA: 0x001318A8 File Offset: 0x0012FCA8
		internal static Exception AddErrorData(Exception exception, ProtoReader source)
		{
			if (exception != null && source != null && !exception.Data.Contains("protoSource"))
			{
				exception.Data.Add("protoSource", string.Format("tag={0}; wire-type={1}; offset={2}; depth={3}", new object[]
				{
					source.fieldNumber,
					source.wireType,
					source.position,
					source.depth
				}));
			}
			return exception;
		}

		// Token: 0x060034A8 RID: 13480 RVA: 0x0013192F File Offset: 0x0012FD2F
		private static Exception EoF(ProtoReader source)
		{
			return ProtoReader.AddErrorData(new EndOfStreamException(), source);
		}

		// Token: 0x060034A9 RID: 13481 RVA: 0x0013193C File Offset: 0x0012FD3C
		public void AppendExtensionData(IExtensible instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			IExtension extensionObject = instance.GetExtensionObject(true);
			bool commit = false;
			Stream stream = extensionObject.BeginAppend();
			try
			{
				using (ProtoWriter protoWriter = new ProtoWriter(stream, this.model, null))
				{
					this.AppendExtensionField(protoWriter);
					protoWriter.Close();
				}
				commit = true;
			}
			finally
			{
				extensionObject.EndAppend(stream, commit);
			}
		}

		// Token: 0x060034AA RID: 13482 RVA: 0x001319C4 File Offset: 0x0012FDC4
		private void AppendExtensionField(ProtoWriter writer)
		{
			ProtoWriter.WriteFieldHeader(this.fieldNumber, this.wireType, writer);
			WireType wireType = this.wireType;
			switch (wireType + 1)
			{
			case WireType.Fixed64:
			case WireType.String:
			case (WireType)9:
				ProtoWriter.WriteInt64(this.ReadInt64(), writer);
				return;
			case WireType.StartGroup:
				ProtoWriter.WriteBytes(ProtoReader.AppendBytes(null, this), writer);
				return;
			case WireType.EndGroup:
			{
				SubItemToken token = ProtoReader.StartSubItem(this);
				SubItemToken token2 = ProtoWriter.StartSubItem(null, writer);
				while (this.ReadFieldHeader() > 0)
				{
					this.AppendExtensionField(writer);
				}
				ProtoReader.EndSubItem(token, this);
				ProtoWriter.EndSubItem(token2, writer);
				return;
			}
			case (WireType)6:
				ProtoWriter.WriteInt32(this.ReadInt32(), writer);
				return;
			}
			throw this.CreateWireTypeException();
		}

		// Token: 0x060034AB RID: 13483 RVA: 0x00131A83 File Offset: 0x0012FE83
		public static bool HasSubValue(WireType wireType, ProtoReader source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (source.blockEnd <= source.position || wireType == WireType.EndGroup)
			{
				return false;
			}
			source.wireType = wireType;
			return true;
		}

		// Token: 0x060034AC RID: 13484 RVA: 0x00131AB8 File Offset: 0x0012FEB8
		internal int GetTypeKey(ref Type type)
		{
			return this.model.GetKey(ref type);
		}

		// Token: 0x17000320 RID: 800
		// (get) Token: 0x060034AD RID: 13485 RVA: 0x00131AC6 File Offset: 0x0012FEC6
		internal NetObjectCache NetCache
		{
			get
			{
				return this.netCache;
			}
		}

		// Token: 0x060034AE RID: 13486 RVA: 0x00131ACE File Offset: 0x0012FECE
		internal Type DeserializeType(string value)
		{
			return TypeModel.DeserializeType(this.model, value);
		}

		// Token: 0x060034AF RID: 13487 RVA: 0x00131ADC File Offset: 0x0012FEDC
		internal void SetRootObject(object value)
		{
			this.netCache.SetKeyedObject(0, value);
			this.trapCount -= 1U;
		}

		// Token: 0x060034B0 RID: 13488 RVA: 0x00131AF9 File Offset: 0x0012FEF9
		public static void NoteObject(object value, ProtoReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (reader.trapCount != 0U)
			{
				reader.netCache.RegisterTrappedObject(value);
				reader.trapCount -= 1U;
			}
		}

		// Token: 0x060034B1 RID: 13489 RVA: 0x00131B31 File Offset: 0x0012FF31
		public Type ReadType()
		{
			return TypeModel.DeserializeType(this.model, this.ReadString());
		}

		// Token: 0x060034B2 RID: 13490 RVA: 0x00131B44 File Offset: 0x0012FF44
		internal void TrapNextObject(int newObjectKey)
		{
			this.trapCount += 1U;
			this.netCache.SetKeyedObject(newObjectKey, null);
		}

		// Token: 0x060034B3 RID: 13491 RVA: 0x00131B61 File Offset: 0x0012FF61
		internal void CheckFullyConsumed()
		{
			if (this.isFixedLength)
			{
				if (this.dataRemaining != 0)
				{
					throw new ProtoException("Incorrect number of bytes consumed");
				}
			}
			else if (this.available != 0)
			{
				throw new ProtoException("Unconsumed data left in the buffer; this suggests corrupt input");
			}
		}

		// Token: 0x060034B4 RID: 13492 RVA: 0x00131BA0 File Offset: 0x0012FFA0
		public static object Merge(ProtoReader parent, object from, object to)
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			TypeModel typeModel = parent.Model;
			SerializationContext serializationContext = parent.Context;
			if (typeModel == null)
			{
				throw new InvalidOperationException("Types cannot be merged unless a type-model has been specified");
			}
			object result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				typeModel.Serialize(memoryStream, from, serializationContext);
				memoryStream.Position = 0L;
				result = typeModel.Deserialize(memoryStream, to, null);
			}
			return result;
		}

		// Token: 0x060034B5 RID: 13493 RVA: 0x00131C24 File Offset: 0x00130024
		internal static ProtoReader Create(Stream source, TypeModel model, SerializationContext context, int len)
		{
			ProtoReader recycled = ProtoReader.GetRecycled();
			if (recycled == null)
			{
				return new ProtoReader(source, model, context, len);
			}
			ProtoReader.Init(recycled, source, model, context, len);
			return recycled;
		}

		// Token: 0x060034B6 RID: 13494 RVA: 0x00131C54 File Offset: 0x00130054
		private static ProtoReader GetRecycled()
		{
			ProtoReader result = ProtoReader.lastReader;
			ProtoReader.lastReader = null;
			return result;
		}

		// Token: 0x060034B7 RID: 13495 RVA: 0x00131C6E File Offset: 0x0013006E
		internal static void Recycle(ProtoReader reader)
		{
			if (reader != null)
			{
				reader.Dispose();
				ProtoReader.lastReader = reader;
			}
		}

		// Token: 0x0400247F RID: 9343
		private Stream source;

		// Token: 0x04002480 RID: 9344
		private byte[] ioBuffer;

		// Token: 0x04002481 RID: 9345
		private TypeModel model;

		// Token: 0x04002482 RID: 9346
		private int fieldNumber;

		// Token: 0x04002483 RID: 9347
		private int depth;

		// Token: 0x04002484 RID: 9348
		private int dataRemaining;

		// Token: 0x04002485 RID: 9349
		private int ioIndex;

		// Token: 0x04002486 RID: 9350
		private int position;

		// Token: 0x04002487 RID: 9351
		private int available;

		// Token: 0x04002488 RID: 9352
		private int blockEnd;

		// Token: 0x04002489 RID: 9353
		private WireType wireType;

		// Token: 0x0400248A RID: 9354
		private bool isFixedLength;

		// Token: 0x0400248B RID: 9355
		private bool internStrings;

		// Token: 0x0400248C RID: 9356
		private NetObjectCache netCache;

		// Token: 0x0400248D RID: 9357
		private uint trapCount;

		// Token: 0x0400248E RID: 9358
		internal const int TO_EOF = -1;

		// Token: 0x0400248F RID: 9359
		private SerializationContext context;

		// Token: 0x04002490 RID: 9360
		private const long Int64Msb = -9223372036854775808L;

		// Token: 0x04002491 RID: 9361
		private const int Int32Msb = -2147483648;

		// Token: 0x04002492 RID: 9362
		private Dictionary<string, string> stringInterner;

		// Token: 0x04002493 RID: 9363
		private static readonly UTF8Encoding encoding = new UTF8Encoding();

		// Token: 0x04002494 RID: 9364
		private static readonly byte[] EmptyBlob = new byte[0];

		// Token: 0x04002495 RID: 9365
		[ThreadStatic]
		private static ProtoReader lastReader;
	}
}
