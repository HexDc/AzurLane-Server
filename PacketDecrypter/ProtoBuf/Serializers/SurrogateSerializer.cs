using ProtoBuf.Meta;
using System;
using System.Reflection;

namespace ProtoBuf.Serializers
{
    // Token: 0x0200041E RID: 1054
    internal sealed class SurrogateSerializer : IProtoTypeSerializer, IProtoSerializer
	{
		// Token: 0x060035E4 RID: 13796 RVA: 0x00135280 File Offset: 0x00133680
		public SurrogateSerializer(TypeModel model, Type forType, Type declaredType, IProtoTypeSerializer rootTail)
		{
			this.forType = forType;
			this.declaredType = declaredType;
			this.rootTail = rootTail;
			toTail = GetConversion(model, true);
			fromTail = GetConversion(model, false);
		}

		// Token: 0x060035E5 RID: 13797 RVA: 0x001352BA File Offset: 0x001336BA
		bool IProtoTypeSerializer.HasCallbacks(TypeModel.CallbackType callbackType)
		{
			return false;
		}

		// Token: 0x060035E6 RID: 13798 RVA: 0x001352BD File Offset: 0x001336BD
		bool IProtoTypeSerializer.CanCreateInstance()
		{
			return false;
		}

		// Token: 0x060035E7 RID: 13799 RVA: 0x001352C0 File Offset: 0x001336C0
		object IProtoTypeSerializer.CreateInstance(ProtoReader source)
		{
			throw new NotSupportedException();
		}

		// Token: 0x060035E8 RID: 13800 RVA: 0x001352C7 File Offset: 0x001336C7
		void IProtoTypeSerializer.Callback(object value, TypeModel.CallbackType callbackType, SerializationContext context)
		{
		}

        // Token: 0x17000382 RID: 898
        // (get) Token: 0x060035E9 RID: 13801 RVA: 0x001352C9 File Offset: 0x001336C9
        public bool ReturnsValue => false;

        // Token: 0x17000383 RID: 899
        // (get) Token: 0x060035EA RID: 13802 RVA: 0x001352CC File Offset: 0x001336CC
        public bool RequiresOldValue => true;

        // Token: 0x17000384 RID: 900
        // (get) Token: 0x060035EB RID: 13803 RVA: 0x001352CF File Offset: 0x001336CF
        public Type ExpectedType => forType;

        // Token: 0x060035EC RID: 13804 RVA: 0x001352D8 File Offset: 0x001336D8
        private static bool HasCast(TypeModel model, Type type, Type from, Type to, out MethodInfo op)
		{
			MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			Type type2 = null;
			foreach (MethodInfo methodInfo in methods)
			{
				if (methodInfo.ReturnType == to)
				{
					ParameterInfo[] parameters = methodInfo.GetParameters();
					if (parameters.Length == 1 && parameters[0].ParameterType == from)
					{
						if (type2 == null)
						{
							type2 = model.MapType(typeof(ProtoConverterAttribute), false);
							if (type2 == null)
							{
								break;
							}
						}
						if (methodInfo.IsDefined(type2, true))
						{
							op = methodInfo;
							return true;
						}
					}
				}
			}
			foreach (MethodInfo methodInfo2 in methods)
			{
				if ((!(methodInfo2.Name != "op_Implicit") || !(methodInfo2.Name != "op_Explicit")) && methodInfo2.ReturnType == to)
				{
					ParameterInfo[] parameters = methodInfo2.GetParameters();
					if (parameters.Length == 1 && parameters[0].ParameterType == from)
					{
						op = methodInfo2;
						return true;
					}
				}
			}
			op = null;
			return false;
		}

		// Token: 0x060035ED RID: 13805 RVA: 0x001353FC File Offset: 0x001337FC
		public MethodInfo GetConversion(TypeModel model, bool toTail)
		{
			Type to = (!toTail) ? forType : declaredType;
			Type from = (!toTail) ? declaredType : forType;
            if (HasCast(model, declaredType, from, to, out MethodInfo result) || HasCast(model, forType, from, to, out result))
            {
                return result;
            }
            throw new InvalidOperationException("No suitable conversion operator found for surrogate: " + forType.FullName + " / " + declaredType.FullName);
		}

		// Token: 0x060035EE RID: 13806 RVA: 0x0013548F File Offset: 0x0013388F
		public void Write(object value, ProtoWriter writer)
		{
			rootTail.Write(toTail.Invoke(null, new object[]
			{
				value
			}), writer);
		}

		// Token: 0x060035EF RID: 13807 RVA: 0x001354B4 File Offset: 0x001338B4
		public object Read(object value, ProtoReader source)
		{
			object[] array = new object[]
			{
				value
			};
			value = toTail.Invoke(null, array);
			array[0] = rootTail.Read(value, source);
			return fromTail.Invoke(null, array);
		}

		// Token: 0x040024ED RID: 9453
		private readonly Type forType;

		// Token: 0x040024EE RID: 9454
		private readonly Type declaredType;

		// Token: 0x040024EF RID: 9455
		private readonly MethodInfo toTail;

		// Token: 0x040024F0 RID: 9456
		private readonly MethodInfo fromTail;

		// Token: 0x040024F1 RID: 9457
		private readonly IProtoTypeSerializer rootTail;
	}
}
