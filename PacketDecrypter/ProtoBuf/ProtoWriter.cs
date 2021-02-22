using System;
using System.IO;
using System.Text;
using ProtoBuf.Meta;

namespace ProtoBuf
{
	// Token: 0x020003F9 RID: 1017
	public sealed class ProtoWriter : IDisposable
	{
		// Token: 0x060034B9 RID: 13497 RVA: 0x00131C9C File Offset: 0x0013009C
		public ProtoWriter(Stream dest, TypeModel model, SerializationContext context)
		{
			if (dest == null)
			{
				throw new ArgumentNullException("dest");
			}
			if (!dest.CanWrite)
			{
				throw new ArgumentException("Cannot write to stream", "dest");
			}
			this.dest = dest;
			this.ioBuffer = BufferPool.GetBuffer();
			this.model = model;
			this.wireType = WireType.None;
			if (context == null)
			{
				context = SerializationContext.Default;
			}
			else
			{
				context.Freeze();
			}
			this.context = context;
		}

		// Token: 0x060034BA RID: 13498 RVA: 0x00131D28 File Offset: 0x00130128
		public static void WriteObject(object value, int key, ProtoWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (writer.model == null)
			{
				throw new InvalidOperationException("Cannot serialize sub-objects unless a model is provided");
			}
			SubItemToken token = ProtoWriter.StartSubItem(value, writer);
			if (key >= 0)
			{
				writer.model.Serialize(key, value, writer);
			}
			else if (writer.model == null || !writer.model.TrySerializeAuxiliaryType(writer, value.GetType(), DataFormat.Default, 1, value, false))
			{
				TypeModel.ThrowUnexpectedType(value.GetType());
			}
			ProtoWriter.EndSubItem(token, writer);
		}

		// Token: 0x060034BB RID: 13499 RVA: 0x00131DBC File Offset: 0x001301BC
		public static void WriteRecursionSafeObject(object value, int key, ProtoWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (writer.model == null)
			{
				throw new InvalidOperationException("Cannot serialize sub-objects unless a model is provided");
			}
			SubItemToken token = ProtoWriter.StartSubItem(null, writer);
			writer.model.Serialize(key, value, writer);
			ProtoWriter.EndSubItem(token, writer);
		}

		// Token: 0x060034BC RID: 13500 RVA: 0x00131E10 File Offset: 0x00130210
		internal static void WriteObject(object value, int key, ProtoWriter writer, PrefixStyle style, int fieldNumber)
		{
			if (writer.model == null)
			{
				throw new InvalidOperationException("Cannot serialize sub-objects unless a model is provided");
			}
			if (writer.wireType != WireType.None)
			{
				throw ProtoWriter.CreateException(writer);
			}
			switch (style)
			{
			case PrefixStyle.Base128:
				writer.wireType = WireType.String;
				writer.fieldNumber = fieldNumber;
				if (fieldNumber > 0)
				{
					ProtoWriter.WriteHeaderCore(fieldNumber, WireType.String, writer);
				}
				break;
			case PrefixStyle.Fixed32:
			case PrefixStyle.Fixed32BigEndian:
				writer.fieldNumber = 0;
				writer.wireType = WireType.Fixed32;
				break;
			default:
				throw new ArgumentOutOfRangeException("style");
			}
			SubItemToken token = ProtoWriter.StartSubItem(value, writer, true);
			if (key < 0)
			{
				if (!writer.model.TrySerializeAuxiliaryType(writer, value.GetType(), DataFormat.Default, 1, value, false))
				{
					TypeModel.ThrowUnexpectedType(value.GetType());
				}
			}
			else
			{
				writer.model.Serialize(key, value, writer);
			}
			ProtoWriter.EndSubItem(token, writer, style);
		}

		// Token: 0x060034BD RID: 13501 RVA: 0x00131EF3 File Offset: 0x001302F3
		internal int GetTypeKey(ref Type type)
		{
			return this.model.GetKey(ref type);
		}

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x060034BE RID: 13502 RVA: 0x00131F01 File Offset: 0x00130301
		internal NetObjectCache NetCache
		{
			get
			{
				return this.netCache;
			}
		}

		// Token: 0x17000322 RID: 802
		// (get) Token: 0x060034BF RID: 13503 RVA: 0x00131F09 File Offset: 0x00130309
		internal WireType WireType
		{
			get
			{
				return this.wireType;
			}
		}

		// Token: 0x060034C0 RID: 13504 RVA: 0x00131F14 File Offset: 0x00130314
		public static void WriteFieldHeader(int fieldNumber, WireType wireType, ProtoWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (writer.wireType != WireType.None)
			{
				throw new InvalidOperationException(string.Concat(new string[]
				{
					"Cannot write a ",
					wireType.ToString(),
					" header until the ",
					writer.wireType.ToString(),
					" data has been written"
				}));
			}
			if (fieldNumber < 0)
			{
				throw new ArgumentOutOfRangeException("fieldNumber");
			}
			if (writer.packedFieldNumber == 0)
			{
				writer.fieldNumber = fieldNumber;
				writer.wireType = wireType;
				ProtoWriter.WriteHeaderCore(fieldNumber, wireType, writer);
			}
			else
			{
				if (writer.packedFieldNumber != fieldNumber)
				{
					throw new InvalidOperationException("Field mismatch during packed encoding; expected " + writer.packedFieldNumber.ToString() + " but received " + fieldNumber.ToString());
				}
				switch (wireType)
				{
				case WireType.Variant:
				case WireType.Fixed64:
				case WireType.Fixed32:
					break;
				default:
					if (wireType != WireType.SignedVariant)
					{
						throw new InvalidOperationException("Wire-type cannot be encoded as packed: " + wireType.ToString());
					}
					break;
				}
				writer.fieldNumber = fieldNumber;
				writer.wireType = wireType;
			}
		}

		// Token: 0x060034C1 RID: 13505 RVA: 0x00132064 File Offset: 0x00130464
		internal static void WriteHeaderCore(int fieldNumber, WireType wireType, ProtoWriter writer)
		{
			uint value = (uint)(fieldNumber << 3 | (int)(wireType & (WireType)7));
			ProtoWriter.WriteUInt32Variant(value, writer);
		}

		// Token: 0x060034C2 RID: 13506 RVA: 0x00132080 File Offset: 0x00130480
		public static void WriteBytes(byte[] data, ProtoWriter writer)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			ProtoWriter.WriteBytes(data, 0, data.Length, writer);
		}

		// Token: 0x060034C3 RID: 13507 RVA: 0x001320A0 File Offset: 0x001304A0
		public static void WriteBytes(byte[] data, int offset, int length, ProtoWriter writer)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			WireType wireType = writer.wireType;
			if (wireType != WireType.Fixed32)
			{
				if (wireType != WireType.Fixed64)
				{
					if (wireType != WireType.String)
					{
						throw ProtoWriter.CreateException(writer);
					}
					ProtoWriter.WriteUInt32Variant((uint)length, writer);
					writer.wireType = WireType.None;
					if (length == 0)
					{
						return;
					}
					if (writer.flushLock == 0 && length > writer.ioBuffer.Length)
					{
						ProtoWriter.Flush(writer);
						writer.dest.Write(data, offset, length);
						writer.position += length;
						return;
					}
				}
				else if (length != 8)
				{
					throw new ArgumentException("length");
				}
			}
			else if (length != 4)
			{
				throw new ArgumentException("length");
			}
			ProtoWriter.DemandSpace(length, writer);
			Helpers.BlockCopy(data, offset, writer.ioBuffer, writer.ioIndex, length);
			ProtoWriter.IncrementedAndReset(length, writer);
		}

		// Token: 0x060034C4 RID: 13508 RVA: 0x001321A0 File Offset: 0x001305A0
		private static void CopyRawFromStream(Stream source, ProtoWriter writer)
		{
			byte[] array = writer.ioBuffer;
			int num = array.Length - writer.ioIndex;
			int num2 = 1;
			while (num > 0 && (num2 = source.Read(array, writer.ioIndex, num)) > 0)
			{
				writer.ioIndex += num2;
				writer.position += num2;
				num -= num2;
			}
			if (num2 <= 0)
			{
				return;
			}
			if (writer.flushLock == 0)
			{
				ProtoWriter.Flush(writer);
				while ((num2 = source.Read(array, 0, array.Length)) > 0)
				{
					writer.dest.Write(array, 0, num2);
					writer.position += num2;
				}
			}
			else
			{
				for (;;)
				{
					ProtoWriter.DemandSpace(128, writer);
					if ((num2 = source.Read(writer.ioBuffer, writer.ioIndex, writer.ioBuffer.Length - writer.ioIndex)) <= 0)
					{
						break;
					}
					writer.position += num2;
					writer.ioIndex += num2;
				}
			}
		}

		// Token: 0x060034C5 RID: 13509 RVA: 0x001322AF File Offset: 0x001306AF
		private static void IncrementedAndReset(int length, ProtoWriter writer)
		{
			writer.ioIndex += length;
			writer.position += length;
			writer.wireType = WireType.None;
		}

		// Token: 0x060034C6 RID: 13510 RVA: 0x001322D4 File Offset: 0x001306D4
		public static SubItemToken StartSubItem(object instance, ProtoWriter writer)
		{
			return ProtoWriter.StartSubItem(instance, writer, false);
		}

		// Token: 0x060034C7 RID: 13511 RVA: 0x001322E0 File Offset: 0x001306E0
		private void CheckRecursionStackAndPush(object instance)
		{
			int num;
			if (this.recursionStack == null)
			{
				this.recursionStack = new MutableList();
			}
			else if (instance != null && (num = this.recursionStack.IndexOfReference(instance)) >= 0)
			{
				throw new ProtoException("Possible recursion detected (offset: " + (this.recursionStack.Count - num).ToString() + " level(s)): " + instance.ToString());
			}
			this.recursionStack.Add(instance);
		}

		// Token: 0x060034C8 RID: 13512 RVA: 0x00132365 File Offset: 0x00130765
		private void PopRecursionStack()
		{
			this.recursionStack.RemoveLast();
		}

		// Token: 0x060034C9 RID: 13513 RVA: 0x00132374 File Offset: 0x00130774
		private static SubItemToken StartSubItem(object instance, ProtoWriter writer, bool allowFixed)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (++writer.depth > 25)
			{
				writer.CheckRecursionStackAndPush(instance);
			}
			if (writer.packedFieldNumber != 0)
			{
				throw new InvalidOperationException("Cannot begin a sub-item while performing packed encoding");
			}
			switch (writer.wireType)
			{
			case WireType.String:
				writer.wireType = WireType.None;
				ProtoWriter.DemandSpace(32, writer);
				writer.flushLock++;
				writer.position++;
				return new SubItemToken(writer.ioIndex++);
			case WireType.StartGroup:
				writer.wireType = WireType.None;
				return new SubItemToken(-writer.fieldNumber);
			case WireType.Fixed32:
			{
				if (!allowFixed)
				{
					throw ProtoWriter.CreateException(writer);
				}
				ProtoWriter.DemandSpace(32, writer);
				writer.flushLock++;
				SubItemToken result = new SubItemToken(writer.ioIndex);
				ProtoWriter.IncrementedAndReset(4, writer);
				return result;
			}
			}
			throw ProtoWriter.CreateException(writer);
		}

		// Token: 0x060034CA RID: 13514 RVA: 0x00132480 File Offset: 0x00130880
		public static void EndSubItem(SubItemToken token, ProtoWriter writer)
		{
			ProtoWriter.EndSubItem(token, writer, PrefixStyle.Base128);
		}

		// Token: 0x060034CB RID: 13515 RVA: 0x0013248C File Offset: 0x0013088C
		private static void EndSubItem(SubItemToken token, ProtoWriter writer, PrefixStyle style)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (writer.wireType != WireType.None)
			{
				throw ProtoWriter.CreateException(writer);
			}
			int value = token.value;
			if (writer.depth <= 0)
			{
				throw ProtoWriter.CreateException(writer);
			}
			if (writer.depth-- > 25)
			{
				writer.PopRecursionStack();
			}
			writer.packedFieldNumber = 0;
			if (value < 0)
			{
				ProtoWriter.WriteHeaderCore(-value, WireType.EndGroup, writer);
				writer.wireType = WireType.None;
				return;
			}
			switch (style)
			{
			case PrefixStyle.Base128:
			{
				int num = writer.ioIndex - value - 1;
				int num2 = 0;
				uint num3 = (uint)num;
				while ((num3 >>= 7) != 0U)
				{
					num2++;
				}
				if (num2 == 0)
				{
					writer.ioBuffer[value] = (byte)(num & 127);
				}
				else
				{
					ProtoWriter.DemandSpace(num2, writer);
					byte[] array = writer.ioBuffer;
					Helpers.BlockCopy(array, value + 1, array, value + 1 + num2, num);
					num3 = (uint)num;
					do
					{
						array[value++] = (byte)((num3 & 127U) | 128U);
					}
					while ((num3 >>= 7) != 0U);
					array[value - 1] = (byte)((int)array[value - 1] & -129);
					writer.position += num2;
					writer.ioIndex += num2;
				}
				break;
			}
			case PrefixStyle.Fixed32:
			{
				int num = writer.ioIndex - value - 4;
				ProtoWriter.WriteInt32ToBuffer(num, writer.ioBuffer, value);
				break;
			}
			case PrefixStyle.Fixed32BigEndian:
			{
				int num = writer.ioIndex - value - 4;
				byte[] array2 = writer.ioBuffer;
				ProtoWriter.WriteInt32ToBuffer(num, array2, value);
				byte b = array2[value];
				array2[value] = array2[value + 3];
				array2[value + 3] = b;
				b = array2[value + 1];
				array2[value + 1] = array2[value + 2];
				array2[value + 2] = b;
				break;
			}
			default:
				throw new ArgumentOutOfRangeException("style");
			}
			if (--writer.flushLock == 0 && writer.ioIndex >= 1024)
			{
				ProtoWriter.Flush(writer);
			}
		}

		// Token: 0x17000323 RID: 803
		// (get) Token: 0x060034CC RID: 13516 RVA: 0x0013268A File Offset: 0x00130A8A
		public SerializationContext Context
		{
			get
			{
				return this.context;
			}
		}

		// Token: 0x060034CD RID: 13517 RVA: 0x00132692 File Offset: 0x00130A92
		void IDisposable.Dispose()
		{
			this.Dispose();
		}

		// Token: 0x060034CE RID: 13518 RVA: 0x0013269A File Offset: 0x00130A9A
		private void Dispose()
		{
			if (this.dest != null)
			{
				ProtoWriter.Flush(this);
				this.dest = null;
			}
			this.model = null;
			BufferPool.ReleaseBufferToPool(ref this.ioBuffer);
		}

		// Token: 0x060034CF RID: 13519 RVA: 0x001326C6 File Offset: 0x00130AC6
		internal static int GetPosition(ProtoWriter writer)
		{
			return writer.position;
		}

		// Token: 0x060034D0 RID: 13520 RVA: 0x001326D0 File Offset: 0x00130AD0
		private static void DemandSpace(int required, ProtoWriter writer)
		{
			if (writer.ioBuffer.Length - writer.ioIndex < required)
			{
				if (writer.flushLock == 0)
				{
					ProtoWriter.Flush(writer);
					if (writer.ioBuffer.Length - writer.ioIndex >= required)
					{
						return;
					}
				}
				BufferPool.ResizeAndFlushLeft(ref writer.ioBuffer, required + writer.ioIndex, 0, writer.ioIndex);
			}
		}

		// Token: 0x060034D1 RID: 13521 RVA: 0x00132733 File Offset: 0x00130B33
		public void Close()
		{
			if (this.depth != 0 || this.flushLock != 0)
			{
				throw new InvalidOperationException("Unable to close stream in an incomplete state");
			}
			this.Dispose();
		}

		// Token: 0x060034D2 RID: 13522 RVA: 0x0013275C File Offset: 0x00130B5C
		internal void CheckDepthFlushlock()
		{
			if (this.depth != 0 || this.flushLock != 0)
			{
				throw new InvalidOperationException("The writer is in an incomplete state");
			}
		}

		// Token: 0x17000324 RID: 804
		// (get) Token: 0x060034D3 RID: 13523 RVA: 0x0013277F File Offset: 0x00130B7F
		public TypeModel Model
		{
			get
			{
				return this.model;
			}
		}

		// Token: 0x060034D4 RID: 13524 RVA: 0x00132787 File Offset: 0x00130B87
		internal static void Flush(ProtoWriter writer)
		{
			if (writer.flushLock == 0 && writer.ioIndex != 0)
			{
				writer.dest.Write(writer.ioBuffer, 0, writer.ioIndex);
				writer.ioIndex = 0;
			}
		}

		// Token: 0x060034D5 RID: 13525 RVA: 0x001327C0 File Offset: 0x00130BC0
		private static void WriteUInt32Variant(uint value, ProtoWriter writer)
		{
			ProtoWriter.DemandSpace(5, writer);
			int num = 0;
			do
			{
				writer.ioBuffer[writer.ioIndex++] = (byte)((value & 127U) | 128U);
				num++;
			}
			while ((value >>= 7) != 0U);
			byte[] array = writer.ioBuffer;
			int num2 = writer.ioIndex - 1;
			array[num2] &= 127;
			writer.position += num;
		}

		// Token: 0x060034D6 RID: 13526 RVA: 0x00132830 File Offset: 0x00130C30
		internal static uint Zig(int value)
		{
			return (uint)(value << 1 ^ value >> 31);
		}

		// Token: 0x060034D7 RID: 13527 RVA: 0x0013283A File Offset: 0x00130C3A
		internal static ulong Zig(long value)
		{
			return (ulong)(value << 1 ^ value >> 63);
		}

		// Token: 0x060034D8 RID: 13528 RVA: 0x00132844 File Offset: 0x00130C44
		private static void WriteUInt64Variant(ulong value, ProtoWriter writer)
		{
			ProtoWriter.DemandSpace(10, writer);
			int num = 0;
			do
			{
				writer.ioBuffer[writer.ioIndex++] = (byte)((value & 127UL) | 128UL);
				num++;
			}
			while ((value >>= 7) != 0UL);
			byte[] array = writer.ioBuffer;
			int num2 = writer.ioIndex - 1;
			array[num2] &= 127;
			writer.position += num;
		}

		// Token: 0x060034D9 RID: 13529 RVA: 0x001328BC File Offset: 0x00130CBC
		public static void WriteString(string value, ProtoWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (writer.wireType != WireType.String)
			{
				throw ProtoWriter.CreateException(writer);
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.Length == 0)
			{
				ProtoWriter.WriteUInt32Variant(0U, writer);
				writer.wireType = WireType.None;
				return;
			}
			int byteCount = ProtoWriter.encoding.GetByteCount(value);
			ProtoWriter.WriteUInt32Variant((uint)byteCount, writer);
			ProtoWriter.DemandSpace(byteCount, writer);
			int bytes = ProtoWriter.encoding.GetBytes(value, 0, value.Length, writer.ioBuffer, writer.ioIndex);
			ProtoWriter.IncrementedAndReset(bytes, writer);
		}

		// Token: 0x060034DA RID: 13530 RVA: 0x0013295C File Offset: 0x00130D5C
		public static void WriteUInt64(ulong value, ProtoWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			switch (writer.wireType)
			{
			case WireType.Variant:
				ProtoWriter.WriteUInt64Variant(value, writer);
				writer.wireType = WireType.None;
				return;
			case WireType.Fixed64:
				ProtoWriter.WriteInt64((long)value, writer);
				return;
			case WireType.Fixed32:
				ProtoWriter.WriteUInt32(checked((uint)value), writer);
				return;
			}
			throw ProtoWriter.CreateException(writer);
		}

		// Token: 0x060034DB RID: 13531 RVA: 0x001329CC File Offset: 0x00130DCC
		public static void WriteInt64(long value, ProtoWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			WireType wireType = writer.wireType;
			switch (wireType)
			{
			case WireType.Variant:
				if (value >= 0L)
				{
					ProtoWriter.WriteUInt64Variant((ulong)value, writer);
					writer.wireType = WireType.None;
				}
				else
				{
					ProtoWriter.DemandSpace(10, writer);
					byte[] array = writer.ioBuffer;
					int num = writer.ioIndex;
					array[num] = (byte)(value | 128L);
					array[num + 1] = (byte)((int)(value >> 7) | 128);
					array[num + 2] = (byte)((int)(value >> 14) | 128);
					array[num + 3] = (byte)((int)(value >> 21) | 128);
					array[num + 4] = (byte)((int)(value >> 28) | 128);
					array[num + 5] = (byte)((int)(value >> 35) | 128);
					array[num + 6] = (byte)((int)(value >> 42) | 128);
					array[num + 7] = (byte)((int)(value >> 49) | 128);
					array[num + 8] = (byte)((int)(value >> 56) | 128);
					array[num + 9] = 1;
					ProtoWriter.IncrementedAndReset(10, writer);
				}
				return;
			case WireType.Fixed64:
			{
				ProtoWriter.DemandSpace(8, writer);
				byte[] array = writer.ioBuffer;
				int num = writer.ioIndex;
				array[num] = (byte)value;
				array[num + 1] = (byte)(value >> 8);
				array[num + 2] = (byte)(value >> 16);
				array[num + 3] = (byte)(value >> 24);
				array[num + 4] = (byte)(value >> 32);
				array[num + 5] = (byte)(value >> 40);
				array[num + 6] = (byte)(value >> 48);
				array[num + 7] = (byte)(value >> 56);
				ProtoWriter.IncrementedAndReset(8, writer);
				return;
			}
			default:
				if (wireType != WireType.SignedVariant)
				{
					throw ProtoWriter.CreateException(writer);
				}
				ProtoWriter.WriteUInt64Variant(ProtoWriter.Zig(value), writer);
				writer.wireType = WireType.None;
				return;
			case WireType.Fixed32:
				ProtoWriter.WriteInt32(checked((int)value), writer);
				return;
			}
		}

		// Token: 0x060034DC RID: 13532 RVA: 0x00132B7C File Offset: 0x00130F7C
		public static void WriteUInt32(uint value, ProtoWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			switch (writer.wireType)
			{
			case WireType.Variant:
				ProtoWriter.WriteUInt32Variant(value, writer);
				writer.wireType = WireType.None;
				return;
			case WireType.Fixed64:
				ProtoWriter.WriteInt64((long)value, writer);
				return;
			case WireType.Fixed32:
				ProtoWriter.WriteInt32((int)value, writer);
				return;
			}
			throw ProtoWriter.CreateException(writer);
		}

		// Token: 0x060034DD RID: 13533 RVA: 0x00132BEA File Offset: 0x00130FEA
		public static void WriteInt16(short value, ProtoWriter writer)
		{
			ProtoWriter.WriteInt32((int)value, writer);
		}

		// Token: 0x060034DE RID: 13534 RVA: 0x00132BF3 File Offset: 0x00130FF3
		public static void WriteUInt16(ushort value, ProtoWriter writer)
		{
			ProtoWriter.WriteUInt32((uint)value, writer);
		}

		// Token: 0x060034DF RID: 13535 RVA: 0x00132BFC File Offset: 0x00130FFC
		public static void WriteByte(byte value, ProtoWriter writer)
		{
			ProtoWriter.WriteUInt32((uint)value, writer);
		}

		// Token: 0x060034E0 RID: 13536 RVA: 0x00132C05 File Offset: 0x00131005
		public static void WriteSByte(sbyte value, ProtoWriter writer)
		{
			ProtoWriter.WriteInt32((int)value, writer);
		}

		// Token: 0x060034E1 RID: 13537 RVA: 0x00132C0F File Offset: 0x0013100F
		private static void WriteInt32ToBuffer(int value, byte[] buffer, int index)
		{
			buffer[index] = (byte)value;
			buffer[index + 1] = (byte)(value >> 8);
			buffer[index + 2] = (byte)(value >> 16);
			buffer[index + 3] = (byte)(value >> 24);
		}

		// Token: 0x060034E2 RID: 13538 RVA: 0x00132C34 File Offset: 0x00131034
		public static void WriteInt32(int value, ProtoWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			WireType wireType = writer.wireType;
			switch (wireType)
			{
			case WireType.Variant:
				if (value >= 0)
				{
					ProtoWriter.WriteUInt32Variant((uint)value, writer);
					writer.wireType = WireType.None;
				}
				else
				{
					ProtoWriter.DemandSpace(10, writer);
					byte[] array = writer.ioBuffer;
					int num = writer.ioIndex;
					array[num] = (byte)(value | 128);
					array[num + 1] = (byte)(value >> 7 | 128);
					array[num + 2] = (byte)(value >> 14 | 128);
					array[num + 3] = (byte)(value >> 21 | 128);
					array[num + 4] = (byte)(value >> 28 | 128);
					array[num + 5] = (array[num + 6] = (array[num + 7] = (array[num + 8] = byte.MaxValue)));
					array[num + 9] = 1;
					ProtoWriter.IncrementedAndReset(10, writer);
				}
				return;
			case WireType.Fixed64:
			{
				ProtoWriter.DemandSpace(8, writer);
				byte[] array = writer.ioBuffer;
				int num = writer.ioIndex;
				array[num] = (byte)value;
				array[num + 1] = (byte)(value >> 8);
				array[num + 2] = (byte)(value >> 16);
				array[num + 3] = (byte)(value >> 24);
				array[num + 4] = (array[num + 5] = (array[num + 6] = (array[num + 7] = 0)));
				ProtoWriter.IncrementedAndReset(8, writer);
				return;
			}
			default:
				if (wireType != WireType.SignedVariant)
				{
					throw ProtoWriter.CreateException(writer);
				}
				ProtoWriter.WriteUInt32Variant(ProtoWriter.Zig(value), writer);
				writer.wireType = WireType.None;
				return;
			case WireType.Fixed32:
				ProtoWriter.DemandSpace(4, writer);
				ProtoWriter.WriteInt32ToBuffer(value, writer.ioBuffer, writer.ioIndex);
				ProtoWriter.IncrementedAndReset(4, writer);
				return;
			}
		}

		// Token: 0x060034E3 RID: 13539 RVA: 0x00132DC8 File Offset: 0x001311C8
		public static void WriteDouble(double value, ProtoWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			WireType wireType = writer.wireType;
			if (wireType != WireType.Fixed32)
			{
				if (wireType != WireType.Fixed64)
				{
					throw ProtoWriter.CreateException(writer);
				}
				ProtoWriter.WriteInt64((long)value, writer);
				return;
			}
			else
			{
				float value2 = (float)value;
				if (Helpers.IsInfinity(value2) && !Helpers.IsInfinity(value))
				{
					throw new OverflowException();
				}
				ProtoWriter.WriteSingle(value2, writer);
				return;
			}
		}

		// Token: 0x060034E4 RID: 13540 RVA: 0x00132E38 File Offset: 0x00131238
		public static void WriteSingle(float value, ProtoWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			WireType wireType = writer.wireType;
			if (wireType == WireType.Fixed32)
			{
				ProtoWriter.WriteInt32((int)value, writer);
				return;
			}
			if (wireType != WireType.Fixed64)
			{
				throw ProtoWriter.CreateException(writer);
			}
			ProtoWriter.WriteDouble((double)value, writer);
		}

		// Token: 0x060034E5 RID: 13541 RVA: 0x00132E8C File Offset: 0x0013128C
		public static void ThrowEnumException(ProtoWriter writer, object enumValue)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			string str = (enumValue != null) ? (enumValue.GetType().FullName + "." + enumValue.ToString()) : "<null>";
			throw new ProtoException("No wire-value is mapped to the enum " + str + " at position " + writer.position.ToString());
		}

		// Token: 0x060034E6 RID: 13542 RVA: 0x00132EFC File Offset: 0x001312FC
		internal static Exception CreateException(ProtoWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			return new ProtoException("Invalid serialization operation with wire-type " + writer.wireType.ToString() + " at position " + writer.position.ToString());
		}

		// Token: 0x060034E7 RID: 13543 RVA: 0x00132F50 File Offset: 0x00131350
		public static void WriteBoolean(bool value, ProtoWriter writer)
		{
			ProtoWriter.WriteUInt32((!value) ? 0U : 1U, writer);
		}

		// Token: 0x060034E8 RID: 13544 RVA: 0x00132F68 File Offset: 0x00131368
		public static void AppendExtensionData(IExtensible instance, ProtoWriter writer)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (writer.wireType != WireType.None)
			{
				throw ProtoWriter.CreateException(writer);
			}
			IExtension extensionObject = instance.GetExtensionObject(false);
			if (extensionObject != null)
			{
				Stream stream = extensionObject.BeginQuery();
				try
				{
					ProtoWriter.CopyRawFromStream(stream, writer);
				}
				finally
				{
					extensionObject.EndQuery(stream);
				}
			}
		}

		// Token: 0x060034E9 RID: 13545 RVA: 0x00132FE4 File Offset: 0x001313E4
		public static void SetPackedField(int fieldNumber, ProtoWriter writer)
		{
			if (fieldNumber <= 0)
			{
				throw new ArgumentOutOfRangeException("fieldNumber");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.packedFieldNumber = fieldNumber;
		}

		// Token: 0x060034EA RID: 13546 RVA: 0x00133010 File Offset: 0x00131410
		internal string SerializeType(Type type)
		{
			return TypeModel.SerializeType(this.model, type);
		}

		// Token: 0x060034EB RID: 13547 RVA: 0x0013301E File Offset: 0x0013141E
		public void SetRootObject(object value)
		{
			this.NetCache.SetKeyedObject(0, value);
		}

		// Token: 0x060034EC RID: 13548 RVA: 0x0013302D File Offset: 0x0013142D
		public static void WriteType(Type value, ProtoWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			ProtoWriter.WriteString(writer.SerializeType(value), writer);
		}

		// Token: 0x04002496 RID: 9366
		private Stream dest;

		// Token: 0x04002497 RID: 9367
		private TypeModel model;

		// Token: 0x04002498 RID: 9368
		private readonly NetObjectCache netCache = new NetObjectCache();

		// Token: 0x04002499 RID: 9369
		private int fieldNumber;

		// Token: 0x0400249A RID: 9370
		private int flushLock;

		// Token: 0x0400249B RID: 9371
		private WireType wireType;

		// Token: 0x0400249C RID: 9372
		private int depth;

		// Token: 0x0400249D RID: 9373
		private const int RecursionCheckDepth = 25;

		// Token: 0x0400249E RID: 9374
		private MutableList recursionStack;

		// Token: 0x0400249F RID: 9375
		private readonly SerializationContext context;

		// Token: 0x040024A0 RID: 9376
		private byte[] ioBuffer;

		// Token: 0x040024A1 RID: 9377
		private int ioIndex;

		// Token: 0x040024A2 RID: 9378
		private int position;

		// Token: 0x040024A3 RID: 9379
		private static readonly UTF8Encoding encoding = new UTF8Encoding();

		// Token: 0x040024A4 RID: 9380
		private int packedFieldNumber;
	}
}
