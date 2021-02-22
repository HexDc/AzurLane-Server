using System;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf.Meta;

namespace ProtoBuf
{
	// Token: 0x020003C9 RID: 969
	public abstract class Extensible : IExtensible
	{
		// Token: 0x06003282 RID: 12930 RVA: 0x00127235 File Offset: 0x00125635
		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return this.GetExtensionObject(createIfMissing);
		}

		// Token: 0x06003283 RID: 12931 RVA: 0x0012723E File Offset: 0x0012563E
		protected virtual IExtension GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		// Token: 0x06003284 RID: 12932 RVA: 0x0012724C File Offset: 0x0012564C
		public static IExtension GetExtensionObject(ref IExtension extensionObject, bool createIfMissing)
		{
			if (createIfMissing && extensionObject == null)
			{
				extensionObject = new BufferExtension();
			}
			return extensionObject;
		}

		// Token: 0x06003285 RID: 12933 RVA: 0x00127264 File Offset: 0x00125664
		public static void AppendValue<TValue>(IExtensible instance, int tag, TValue value)
		{
			Extensible.AppendValue<TValue>(instance, tag, DataFormat.Default, value);
		}

		// Token: 0x06003286 RID: 12934 RVA: 0x0012726F File Offset: 0x0012566F
		public static void AppendValue<TValue>(IExtensible instance, int tag, DataFormat format, TValue value)
		{
			ExtensibleUtil.AppendExtendValue(RuntimeTypeModel.Default, instance, tag, format, value);
		}

		// Token: 0x06003287 RID: 12935 RVA: 0x00127284 File Offset: 0x00125684
		public static TValue GetValue<TValue>(IExtensible instance, int tag)
		{
			return Extensible.GetValue<TValue>(instance, tag, DataFormat.Default);
		}

		// Token: 0x06003288 RID: 12936 RVA: 0x00127290 File Offset: 0x00125690
		public static TValue GetValue<TValue>(IExtensible instance, int tag, DataFormat format)
		{
			TValue result;
			Extensible.TryGetValue<TValue>(instance, tag, format, out result);
			return result;
		}

		// Token: 0x06003289 RID: 12937 RVA: 0x001272A9 File Offset: 0x001256A9
		public static bool TryGetValue<TValue>(IExtensible instance, int tag, out TValue value)
		{
			return Extensible.TryGetValue<TValue>(instance, tag, DataFormat.Default, out value);
		}

		// Token: 0x0600328A RID: 12938 RVA: 0x001272B4 File Offset: 0x001256B4
		public static bool TryGetValue<TValue>(IExtensible instance, int tag, DataFormat format, out TValue value)
		{
			return Extensible.TryGetValue<TValue>(instance, tag, format, false, out value);
		}

		// Token: 0x0600328B RID: 12939 RVA: 0x001272C0 File Offset: 0x001256C0
		public static bool TryGetValue<TValue>(IExtensible instance, int tag, DataFormat format, bool allowDefinedTag, out TValue value)
		{
			value = default(TValue);
			bool result = false;
			foreach (TValue tvalue in ExtensibleUtil.GetExtendedValues<TValue>(instance, tag, format, true, allowDefinedTag))
			{
				value = tvalue;
				result = true;
			}
			return result;
		}

		// Token: 0x0600328C RID: 12940 RVA: 0x00127334 File Offset: 0x00125734
		public static IEnumerable<TValue> GetValues<TValue>(IExtensible instance, int tag)
		{
			return ExtensibleUtil.GetExtendedValues<TValue>(instance, tag, DataFormat.Default, false, false);
		}

		// Token: 0x0600328D RID: 12941 RVA: 0x00127340 File Offset: 0x00125740
		public static IEnumerable<TValue> GetValues<TValue>(IExtensible instance, int tag, DataFormat format)
		{
			return ExtensibleUtil.GetExtendedValues<TValue>(instance, tag, format, false, false);
		}

		// Token: 0x0600328E RID: 12942 RVA: 0x0012734C File Offset: 0x0012574C
		public static bool TryGetValue(TypeModel model, Type type, IExtensible instance, int tag, DataFormat format, bool allowDefinedTag, out object value)
		{
			value = null;
			bool result = false;
			IEnumerator enumerator = ExtensibleUtil.GetExtendedValues(model, type, instance, tag, format, true, allowDefinedTag).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					value = obj;
					result = true;
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
			return result;
		}

		// Token: 0x0600328F RID: 12943 RVA: 0x001273BC File Offset: 0x001257BC
		public static IEnumerable GetValues(TypeModel model, Type type, IExtensible instance, int tag, DataFormat format)
		{
			return ExtensibleUtil.GetExtendedValues(model, type, instance, tag, format, false, false);
		}

		// Token: 0x06003290 RID: 12944 RVA: 0x001273CB File Offset: 0x001257CB
		public static void AppendValue(TypeModel model, IExtensible instance, int tag, DataFormat format, object value)
		{
			ExtensibleUtil.AppendExtendValue(model, instance, tag, format, value);
		}

		// Token: 0x040023C6 RID: 9158
		private IExtension extensionObject;
	}
}
