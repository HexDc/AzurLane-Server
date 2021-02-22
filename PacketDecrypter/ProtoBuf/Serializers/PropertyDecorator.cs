using ProtoBuf.Meta;
using System;
using System.Reflection;

namespace ProtoBuf.Serializers
{
    // Token: 0x02000418 RID: 1048
    internal sealed class PropertyDecorator : ProtoDecoratorBase
	{
		// Token: 0x060035B6 RID: 13750 RVA: 0x00134E4E File Offset: 0x0013324E
		public PropertyDecorator(TypeModel model, Type forType, PropertyInfo property, IProtoSerializer tail) : base(tail)
		{
			this.forType = forType;
			this.property = property;
			PropertyDecorator.SanityCheck(model, property, tail, out readOptionsWriteValue, true, true);
			shadowSetter = PropertyDecorator.GetShadowSetter(model, property);
		}

        // Token: 0x17000370 RID: 880
        // (get) Token: 0x060035B7 RID: 13751 RVA: 0x00134E84 File Offset: 0x00133284
        public override Type ExpectedType => forType;

        // Token: 0x17000371 RID: 881
        // (get) Token: 0x060035B8 RID: 13752 RVA: 0x00134E8C File Offset: 0x0013328C
        public override bool RequiresOldValue => true;

        // Token: 0x17000372 RID: 882
        // (get) Token: 0x060035B9 RID: 13753 RVA: 0x00134E8F File Offset: 0x0013328F
        public override bool ReturnsValue => false;

        // Token: 0x060035BA RID: 13754 RVA: 0x00134E94 File Offset: 0x00133294
        private static void SanityCheck(TypeModel model, PropertyInfo property, IProtoSerializer tail, out bool writeValue, bool nonPublic, bool allowInternal)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			writeValue = (tail.ReturnsValue && (PropertyDecorator.GetShadowSetter(model, property) != null || (property.CanWrite && Helpers.GetSetMethod(property, nonPublic, allowInternal) != null)));
			if (!property.CanRead || Helpers.GetGetMethod(property, nonPublic, allowInternal) == null)
			{
				throw new InvalidOperationException("Cannot serialize property without a get accessor");
			}
			if (!writeValue && (!tail.RequiresOldValue || Helpers.IsValueType(tail.ExpectedType)))
			{
				throw new InvalidOperationException("Cannot apply changes to property " + property.DeclaringType.FullName + "." + property.Name);
			}
		}

		// Token: 0x060035BB RID: 13755 RVA: 0x00134F5C File Offset: 0x0013335C
		private static MethodInfo GetShadowSetter(TypeModel model, PropertyInfo property)
		{
			Type reflectedType = property.ReflectedType;
			MethodInfo instanceMethod = Helpers.GetInstanceMethod(reflectedType, "Set" + property.Name, new Type[]
			{
				property.PropertyType
			});
			if (instanceMethod == null || !instanceMethod.IsPublic || instanceMethod.ReturnType != model.MapType(typeof(void)))
			{
				return null;
			}
			return instanceMethod;
		}

		// Token: 0x060035BC RID: 13756 RVA: 0x00134FC5 File Offset: 0x001333C5
		public override void Write(object value, ProtoWriter dest)
		{
			value = property.GetValue(value, null);
			if (value != null)
			{
				Tail.Write(value, dest);
			}
		}

		// Token: 0x060035BD RID: 13757 RVA: 0x00134FEC File Offset: 0x001333EC
		public override object Read(object value, ProtoReader source)
		{
			object value2 = (!Tail.RequiresOldValue) ? null : property.GetValue(value, null);
			object obj = Tail.Read(value2, source);
			if (readOptionsWriteValue && obj != null)
			{
				if (shadowSetter == null)
				{
					property.SetValue(value, obj, null);
				}
				else
				{
					shadowSetter.Invoke(value, new object[]
					{
						obj
					});
				}
			}
			return null;
		}

		// Token: 0x060035BE RID: 13758 RVA: 0x00135074 File Offset: 0x00133474
		internal static bool CanWrite(TypeModel model, MemberInfo member)
		{
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			PropertyInfo propertyInfo = member as PropertyInfo;
			if (propertyInfo != null)
			{
				return propertyInfo.CanWrite || PropertyDecorator.GetShadowSetter(model, propertyInfo) != null;
			}
			return member is FieldInfo;
		}

		// Token: 0x040024E1 RID: 9441
		private readonly PropertyInfo property;

		// Token: 0x040024E2 RID: 9442
		private readonly Type forType;

		// Token: 0x040024E3 RID: 9443
		private readonly bool readOptionsWriteValue;

		// Token: 0x040024E4 RID: 9444
		private readonly MethodInfo shadowSetter;
	}
}
