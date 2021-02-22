using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf.Meta;

namespace ProtoBuf
{
	// Token: 0x020003FB RID: 1019
	public static class Serializer
	{
		// Token: 0x060034F5 RID: 13557 RVA: 0x001330C2 File Offset: 0x001314C2
		public static string GetProto<T>()
		{
			return RuntimeTypeModel.Default.GetSchema(RuntimeTypeModel.Default.MapType(typeof(T)));
		}

		// Token: 0x060034F6 RID: 13558 RVA: 0x001330E2 File Offset: 0x001314E2
		public static T DeepClone<T>(T instance)
		{
			return (instance != null) ? ((T)((object)RuntimeTypeModel.Default.DeepClone(instance))) : instance;
		}

		// Token: 0x060034F7 RID: 13559 RVA: 0x0013310A File Offset: 0x0013150A
		public static T Merge<T>(Stream source, T instance)
		{
			return (T)((object)RuntimeTypeModel.Default.Deserialize(source, instance, typeof(T)));
		}

		// Token: 0x060034F8 RID: 13560 RVA: 0x0013312C File Offset: 0x0013152C
		public static T Deserialize<T>(Stream source)
		{
			return (T)((object)RuntimeTypeModel.Default.Deserialize(source, null, typeof(T)));
		}

		// Token: 0x060034F9 RID: 13561 RVA: 0x00133149 File Offset: 0x00131549
		public static object Deserialize(Type type, Stream source)
		{
			return RuntimeTypeModel.Default.Deserialize(source, null, type);
		}

		// Token: 0x060034FA RID: 13562 RVA: 0x00133158 File Offset: 0x00131558
		public static void Serialize<T>(Stream destination, T instance)
		{
			if (instance != null)
			{
				RuntimeTypeModel.Default.Serialize(destination, instance);
			}
		}

		// Token: 0x060034FB RID: 13563 RVA: 0x00133178 File Offset: 0x00131578
		public static TTo ChangeType<TFrom, TTo>(TFrom instance)
		{
			TTo result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Serializer.Serialize<TFrom>(memoryStream, instance);
				memoryStream.Position = 0L;
				result = Serializer.Deserialize<TTo>(memoryStream);
			}
			return result;
		}

		// Token: 0x060034FC RID: 13564 RVA: 0x001331C4 File Offset: 0x001315C4
		public static void PrepareSerializer<T>()
		{
		}

		// Token: 0x060034FD RID: 13565 RVA: 0x001331C6 File Offset: 0x001315C6
		public static IEnumerable<T> DeserializeItems<T>(Stream source, PrefixStyle style, int fieldNumber)
		{
			return RuntimeTypeModel.Default.DeserializeItems<T>(source, style, fieldNumber);
		}

		// Token: 0x060034FE RID: 13566 RVA: 0x001331D5 File Offset: 0x001315D5
		public static T DeserializeWithLengthPrefix<T>(Stream source, PrefixStyle style)
		{
			return Serializer.DeserializeWithLengthPrefix<T>(source, style, 0);
		}

		// Token: 0x060034FF RID: 13567 RVA: 0x001331E0 File Offset: 0x001315E0
		public static T DeserializeWithLengthPrefix<T>(Stream source, PrefixStyle style, int fieldNumber)
		{
			RuntimeTypeModel @default = RuntimeTypeModel.Default;
			return (T)((object)@default.DeserializeWithLengthPrefix(source, null, @default.MapType(typeof(T)), style, fieldNumber));
		}

		// Token: 0x06003500 RID: 13568 RVA: 0x00133214 File Offset: 0x00131614
		public static T MergeWithLengthPrefix<T>(Stream source, T instance, PrefixStyle style)
		{
			RuntimeTypeModel @default = RuntimeTypeModel.Default;
			return (T)((object)@default.DeserializeWithLengthPrefix(source, instance, @default.MapType(typeof(T)), style, 0));
		}

		// Token: 0x06003501 RID: 13569 RVA: 0x0013324B File Offset: 0x0013164B
		public static void SerializeWithLengthPrefix<T>(Stream destination, T instance, PrefixStyle style)
		{
			Serializer.SerializeWithLengthPrefix<T>(destination, instance, style, 0);
		}

		// Token: 0x06003502 RID: 13570 RVA: 0x00133258 File Offset: 0x00131658
		public static void SerializeWithLengthPrefix<T>(Stream destination, T instance, PrefixStyle style, int fieldNumber)
		{
			RuntimeTypeModel @default = RuntimeTypeModel.Default;
			@default.SerializeWithLengthPrefix(destination, instance, @default.MapType(typeof(T)), style, fieldNumber);
		}

		// Token: 0x06003503 RID: 13571 RVA: 0x0013328C File Offset: 0x0013168C
		public static bool TryReadLengthPrefix(Stream source, PrefixStyle style, out int length)
		{
			int num;
			int num2;
			length = ProtoReader.ReadLengthPrefix(source, false, style, out num, out num2);
			return num2 > 0;
		}

		// Token: 0x06003504 RID: 13572 RVA: 0x001332AC File Offset: 0x001316AC
		public static bool TryReadLengthPrefix(byte[] buffer, int index, int count, PrefixStyle style, out int length)
		{
			bool result;
			using (Stream stream = new MemoryStream(buffer, index, count))
			{
				result = Serializer.TryReadLengthPrefix(stream, style, out length);
			}
			return result;
		}

		// Token: 0x06003505 RID: 13573 RVA: 0x001332F0 File Offset: 0x001316F0
		public static void FlushPool()
		{
			BufferPool.Flush();
		}

		// Token: 0x040024A8 RID: 9384
		private const string ProtoBinaryField = "proto";

		// Token: 0x040024A9 RID: 9385
		public const int ListItemTag = 1;

		// Token: 0x020003FC RID: 1020
		public static class NonGeneric
		{
			// Token: 0x06003506 RID: 13574 RVA: 0x001332F7 File Offset: 0x001316F7
			public static object DeepClone(object instance)
			{
				return (instance != null) ? RuntimeTypeModel.Default.DeepClone(instance) : null;
			}

			// Token: 0x06003507 RID: 13575 RVA: 0x00133310 File Offset: 0x00131710
			public static void Serialize(Stream dest, object instance)
			{
				if (instance != null)
				{
					RuntimeTypeModel.Default.Serialize(dest, instance);
				}
			}

			// Token: 0x06003508 RID: 13576 RVA: 0x00133324 File Offset: 0x00131724
			public static object Deserialize(Type type, Stream source)
			{
				return RuntimeTypeModel.Default.Deserialize(source, null, type);
			}

			// Token: 0x06003509 RID: 13577 RVA: 0x00133333 File Offset: 0x00131733
			public static object Merge(Stream source, object instance)
			{
				if (instance == null)
				{
					throw new ArgumentNullException("instance");
				}
				return RuntimeTypeModel.Default.Deserialize(source, instance, instance.GetType(), null);
			}

			// Token: 0x0600350A RID: 13578 RVA: 0x0013335C File Offset: 0x0013175C
			public static void SerializeWithLengthPrefix(Stream destination, object instance, PrefixStyle style, int fieldNumber)
			{
				if (instance == null)
				{
					throw new ArgumentNullException("instance");
				}
				RuntimeTypeModel @default = RuntimeTypeModel.Default;
				@default.SerializeWithLengthPrefix(destination, instance, @default.MapType(instance.GetType()), style, fieldNumber);
			}

			// Token: 0x0600350B RID: 13579 RVA: 0x00133396 File Offset: 0x00131796
			public static bool TryDeserializeWithLengthPrefix(Stream source, PrefixStyle style, Serializer.TypeResolver resolver, out object value)
			{
				value = RuntimeTypeModel.Default.DeserializeWithLengthPrefix(source, null, null, style, 0, resolver);
				return value != null;
			}

			// Token: 0x0600350C RID: 13580 RVA: 0x001333B2 File Offset: 0x001317B2
			public static bool CanSerialize(Type type)
			{
				return RuntimeTypeModel.Default.IsDefined(type);
			}
		}

		// Token: 0x020003FD RID: 1021
		public static class GlobalOptions
		{
			// Token: 0x17000327 RID: 807
			// (get) Token: 0x0600350D RID: 13581 RVA: 0x001333BF File Offset: 0x001317BF
			// (set) Token: 0x0600350E RID: 13582 RVA: 0x001333CB File Offset: 0x001317CB
			[Obsolete("Please use RuntimeTypeModel.Default.InferTagFromNameDefault instead (or on a per-model basis)", false)]
			public static bool InferTagFromName
			{
				get
				{
					return RuntimeTypeModel.Default.InferTagFromNameDefault;
				}
				set
				{
					RuntimeTypeModel.Default.InferTagFromNameDefault = value;
				}
			}
		}

		// Token: 0x020003FE RID: 1022
		// (Invoke) Token: 0x06003510 RID: 13584
		public delegate Type TypeResolver(int fieldNumber);
	}
}
