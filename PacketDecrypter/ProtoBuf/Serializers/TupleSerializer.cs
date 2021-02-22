using ProtoBuf.Meta;
using System;
using System.Reflection;

namespace ProtoBuf.Serializers
{
    // Token: 0x02000422 RID: 1058
    internal sealed class TupleSerializer : IProtoTypeSerializer, IProtoSerializer
	{
		// Token: 0x06003609 RID: 13833 RVA: 0x001356CC File Offset: 0x00133ACC
		public TupleSerializer(RuntimeTypeModel model, ConstructorInfo ctor, MemberInfo[] members)
		{
			if (ctor == null) throw new ArgumentNullException("ctor");
			if (members == null) throw new ArgumentNullException("members");

			this.ctor = ctor;
			this.members = members;
			tails = new IProtoSerializer[members.Length];
			ParameterInfo[] parameters = ctor.GetParameters();
			for (int i = 0; i < members.Length; i++)
			{
				Type parameterType = parameters[i].ParameterType;
				Type type = null;
				Type concreteType = null;
				MetaType.ResolveListTypes(model, parameterType, ref type, ref concreteType);
				Type type2 = (type != null) ? type : parameterType;
				bool asReference = false;
				int num = model.FindOrAddAuto(type2, false, true, false);
				if (num >= 0)
				{
					asReference = model[type2].AsReferenceDefault;
				}
                IProtoSerializer protoSerializer = ValueMember.TryGetCoreSerializer(model, DataFormat.Default, type2, out WireType wireType, asReference, false, false, true);
                if (protoSerializer == null)
				{
					throw new InvalidOperationException("No serializer defined for type: " + type2.FullName);
				}
				protoSerializer = new TagDecorator(i + 1, wireType, false, protoSerializer);
				IProtoSerializer protoSerializer2;
				if (type == null)
				{
					protoSerializer2 = protoSerializer;
				}
				else if (parameterType.IsArray)
				{
					protoSerializer2 = new ArrayDecorator(model, protoSerializer, i + 1, false, wireType, parameterType, false, false);
				}
				else
				{
					protoSerializer2 = ListDecorator.Create(model, parameterType, concreteType, protoSerializer, i + 1, false, wireType, true, false, false);
				}
				tails[i] = protoSerializer2;
			}
		}

		// Token: 0x0600360A RID: 13834 RVA: 0x00135819 File Offset: 0x00133C19
		public bool HasCallbacks(TypeModel.CallbackType callbackType)
		{
			return false;
		}

        // Token: 0x1700038F RID: 911
        // (get) Token: 0x0600360B RID: 13835 RVA: 0x0013581C File Offset: 0x00133C1C
        public Type ExpectedType => ctor.DeclaringType;

        // Token: 0x0600360C RID: 13836 RVA: 0x00135829 File Offset: 0x00133C29
        void IProtoTypeSerializer.Callback(object value, TypeModel.CallbackType callbackType, SerializationContext context)
		{
		}

		// Token: 0x0600360D RID: 13837 RVA: 0x0013582B File Offset: 0x00133C2B
		object IProtoTypeSerializer.CreateInstance(ProtoReader source)
		{
			throw new NotSupportedException();
		}

		// Token: 0x0600360E RID: 13838 RVA: 0x00135834 File Offset: 0x00133C34
		private object GetValue(object obj, int index)
		{
			PropertyInfo propertyInfo;
			if ((propertyInfo = (members[index] as PropertyInfo)) != null)
			{
				if (obj == null)
				{
					return (!Helpers.IsValueType(propertyInfo.PropertyType)) ? null : Activator.CreateInstance(propertyInfo.PropertyType);
				}
				return propertyInfo.GetValue(obj, null);
			}
			else
			{
				FieldInfo fieldInfo;
				if ((fieldInfo = (members[index] as FieldInfo)) == null)
				{
					throw new InvalidOperationException();
				}
				if (obj == null)
				{
					return (!Helpers.IsValueType(fieldInfo.FieldType)) ? null : Activator.CreateInstance(fieldInfo.FieldType);
				}
				return fieldInfo.GetValue(obj);
			}
		}

		// Token: 0x0600360F RID: 13839 RVA: 0x001358D0 File Offset: 0x00133CD0
		public object Read(object value, ProtoReader source)
		{
			object[] array = new object[members.Length];
			bool flag = false;
			if (value == null)
			{
				flag = true;
			}
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = GetValue(value, i);
			}
			int num;
			while ((num = source.ReadFieldHeader()) > 0)
			{
				flag = true;
				if (num <= tails.Length)
				{
					IProtoSerializer protoSerializer = tails[num - 1];
					array[num - 1] = tails[num - 1].Read((!protoSerializer.RequiresOldValue) ? null : array[num - 1], source);
				}
				else
				{
					source.SkipField();
				}
			}
			return (!flag) ? value : ctor.Invoke(array);
		}

		// Token: 0x06003610 RID: 13840 RVA: 0x00135994 File Offset: 0x00133D94
		public void Write(object value, ProtoWriter dest)
		{
			for (int i = 0; i < tails.Length; i++)
			{
				object value2 = GetValue(value, i);
				if (value2 != null)
				{
					tails[i].Write(value2, dest);
				}
			}
		}

        // Token: 0x17000390 RID: 912
        // (get) Token: 0x06003611 RID: 13841 RVA: 0x001359D8 File Offset: 0x00133DD8
        public bool RequiresOldValue => true;

        // Token: 0x17000391 RID: 913
        // (get) Token: 0x06003612 RID: 13842 RVA: 0x001359DB File Offset: 0x00133DDB
        public bool ReturnsValue => false;

        // Token: 0x06003613 RID: 13843 RVA: 0x001359E0 File Offset: 0x00133DE0
        private Type GetMemberType(int index)
		{
			Type memberType = Helpers.GetMemberType(members[index]);
			if (memberType == null)
			{
				throw new InvalidOperationException();
			}
			return memberType;
		}

		// Token: 0x06003614 RID: 13844 RVA: 0x00135A08 File Offset: 0x00133E08
		bool IProtoTypeSerializer.CanCreateInstance()
		{
			return false;
		}

		// Token: 0x040024F7 RID: 9463
		private readonly MemberInfo[] members;

		// Token: 0x040024F8 RID: 9464
		private readonly ConstructorInfo ctor;

		// Token: 0x040024F9 RID: 9465
		private readonly IProtoSerializer[] tails;
	}
}
