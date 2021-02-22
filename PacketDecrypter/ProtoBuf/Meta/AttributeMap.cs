using System;
using System.Reflection;

namespace ProtoBuf.Meta
{
	// Token: 0x020003D0 RID: 976
	internal abstract class AttributeMap
	{
		// Token: 0x060032BF RID: 12991
		public abstract bool TryGet(string key, bool publicOnly, out object value);

		// Token: 0x060032C0 RID: 12992 RVA: 0x00127D9E File Offset: 0x0012619E
		public bool TryGet(string key, out object value)
		{
			return this.TryGet(key, true, out value);
		}

		// Token: 0x170002B1 RID: 689
		// (get) Token: 0x060032C1 RID: 12993
		public abstract Type AttributeType { get; }

		// Token: 0x060032C2 RID: 12994 RVA: 0x00127DAC File Offset: 0x001261AC
		public static AttributeMap[] Create(TypeModel model, Type type, bool inherit)
		{
			object[] customAttributes = type.GetCustomAttributes(inherit);
			AttributeMap[] array = new AttributeMap[customAttributes.Length];
			for (int i = 0; i < customAttributes.Length; i++)
			{
				array[i] = new AttributeMap.ReflectionAttributeMap((Attribute)customAttributes[i]);
			}
			return array;
		}

		// Token: 0x060032C3 RID: 12995 RVA: 0x00127DF0 File Offset: 0x001261F0
		public static AttributeMap[] Create(TypeModel model, MemberInfo member, bool inherit)
		{
			object[] customAttributes = member.GetCustomAttributes(inherit);
			AttributeMap[] array = new AttributeMap[customAttributes.Length];
			for (int i = 0; i < customAttributes.Length; i++)
			{
				array[i] = new AttributeMap.ReflectionAttributeMap((Attribute)customAttributes[i]);
			}
			return array;
		}

		// Token: 0x060032C4 RID: 12996 RVA: 0x00127E34 File Offset: 0x00126234
		public static AttributeMap[] Create(TypeModel model, Assembly assembly)
		{
			object[] customAttributes = assembly.GetCustomAttributes(false);
			AttributeMap[] array = new AttributeMap[customAttributes.Length];
			for (int i = 0; i < customAttributes.Length; i++)
			{
				array[i] = new AttributeMap.ReflectionAttributeMap((Attribute)customAttributes[i]);
			}
			return array;
		}

		// Token: 0x170002B2 RID: 690
		// (get) Token: 0x060032C5 RID: 12997
		public abstract object Target { get; }

		// Token: 0x020003D1 RID: 977
		private sealed class ReflectionAttributeMap : AttributeMap
		{
			// Token: 0x060032C6 RID: 12998 RVA: 0x00127E77 File Offset: 0x00126277
			public ReflectionAttributeMap(Attribute attribute)
			{
				this.attribute = attribute;
			}

			// Token: 0x170002B3 RID: 691
			// (get) Token: 0x060032C7 RID: 12999 RVA: 0x00127E86 File Offset: 0x00126286
			public override object Target
			{
				get
				{
					return this.attribute;
				}
			}

			// Token: 0x170002B4 RID: 692
			// (get) Token: 0x060032C8 RID: 13000 RVA: 0x00127E8E File Offset: 0x0012628E
			public override Type AttributeType
			{
				get
				{
					return this.attribute.GetType();
				}
			}

			// Token: 0x060032C9 RID: 13001 RVA: 0x00127E9C File Offset: 0x0012629C
			public override bool TryGet(string key, bool publicOnly, out object value)
			{
				MemberInfo[] instanceFieldsAndProperties = Helpers.GetInstanceFieldsAndProperties(this.attribute.GetType(), publicOnly);
				MemberInfo[] array = instanceFieldsAndProperties;
				int i = 0;
				while (i < array.Length)
				{
					MemberInfo memberInfo = array[i];
					if (string.Equals(memberInfo.Name, key, StringComparison.OrdinalIgnoreCase))
					{
						PropertyInfo propertyInfo = memberInfo as PropertyInfo;
						if (propertyInfo != null)
						{
							value = propertyInfo.GetValue(this.attribute, null);
							return true;
						}
						FieldInfo fieldInfo = memberInfo as FieldInfo;
						if (fieldInfo != null)
						{
							value = fieldInfo.GetValue(this.attribute);
							return true;
						}
						throw new NotSupportedException(memberInfo.GetType().Name);
					}
					else
					{
						i++;
					}
				}
				value = null;
				return false;
			}

			// Token: 0x040023E3 RID: 9187
			private readonly Attribute attribute;
		}
	}
}
