using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ProtoBuf.Meta;

namespace ProtoBuf
{
	// Token: 0x020003CA RID: 970
	internal static class ExtensibleUtil
	{
		// Token: 0x06003291 RID: 12945 RVA: 0x001273D8 File Offset: 0x001257D8
		internal static IEnumerable<TValue> GetExtendedValues<TValue>(IExtensible instance, int tag, DataFormat format, bool singleton, bool allowDefinedTag)
		{
			IEnumerator enumerator = ExtensibleUtil.GetExtendedValues(RuntimeTypeModel.Default, typeof(TValue), instance, tag, format, singleton, allowDefinedTag).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					TValue value = (TValue)((object)obj);
					yield return value;
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			yield break;
		}

		// Token: 0x06003292 RID: 12946 RVA: 0x00127418 File Offset: 0x00125818
		internal static IEnumerable GetExtendedValues(TypeModel model, Type type, IExtensible instance, int tag, DataFormat format, bool singleton, bool allowDefinedTag)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (tag <= 0)
			{
				throw new ArgumentOutOfRangeException("tag");
			}
			IExtension extn = instance.GetExtensionObject(false);
			if (extn == null)
			{
				yield break;
			}
			Stream stream = extn.BeginQuery();
			object value = null;
			ProtoReader reader = null;
			try
			{
				SerializationContext ctx = new SerializationContext();
				reader = ProtoReader.Create(stream, model, ctx, -1);
				while (model.TryDeserializeAuxiliaryType(reader, format, tag, type, ref value, true, false, false, false) && value != null)
				{
					if (!singleton)
					{
						yield return value;
						value = null;
					}
				}
				if (singleton && value != null)
				{
					yield return value;
				}
			}
			finally
			{
				ProtoReader.Recycle(reader);
				extn.EndQuery(stream);
			}
			yield break;
		}

		// Token: 0x06003293 RID: 12947 RVA: 0x00127460 File Offset: 0x00125860
		internal static void AppendExtendValue(TypeModel model, IExtensible instance, int tag, DataFormat format, object value)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			IExtension extensionObject = instance.GetExtensionObject(true);
			if (extensionObject == null)
			{
				throw new InvalidOperationException("No extension object available; appended data would be lost.");
			}
			bool commit = false;
			Stream stream = extensionObject.BeginAppend();
			try
			{
				using (ProtoWriter protoWriter = new ProtoWriter(stream, model, null))
				{
					model.TrySerializeAuxiliaryType(protoWriter, null, format, tag, value, false);
					protoWriter.Close();
				}
				commit = true;
			}
			finally
			{
				extensionObject.EndAppend(stream, commit);
			}
		}
	}
}
