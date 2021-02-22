using ProtoBuf.Meta;
using System;
using System.Reflection;

namespace ProtoBuf.Serializers
{
    // Token: 0x02000423 RID: 1059
    internal sealed class TypeSerializer : IProtoTypeSerializer, IProtoSerializer
	{
		// Token: 0x06003615 RID: 13845 RVA: 0x00135A0C File Offset: 0x00133E0C
		public TypeSerializer(TypeModel model, Type forType, int[] fieldNumbers, IProtoSerializer[] serializers, MethodInfo[] baseCtorCallbacks, bool isRootType, bool useConstructor, CallbackSet callbacks, Type constructType, MethodInfo factory)
		{
			Helpers.Sort(fieldNumbers, serializers);
			bool flag = false;
			for (int i = 1; i < fieldNumbers.Length; i++)
			{
				if (fieldNumbers[i] == fieldNumbers[i - 1])
				{
					throw new InvalidOperationException("Duplicate field-number detected; " + fieldNumbers[i].ToString() + " on: " + forType.FullName);
				}
				if (!flag && serializers[i].ExpectedType != forType)
				{
					flag = true;
				}
			}
			this.forType = forType;
			this.factory = factory;
			if (constructType == null)
			{
				constructType = forType;
			}
			else if (!forType.IsAssignableFrom(constructType))
			{
				throw new InvalidOperationException(forType.FullName + " cannot be assigned from " + constructType.FullName);
			}
			this.constructType = constructType;
			this.serializers = serializers;
			this.fieldNumbers = fieldNumbers;
			this.callbacks = callbacks;
			this.isRootType = isRootType;
			this.useConstructor = useConstructor;
			if (baseCtorCallbacks != null && baseCtorCallbacks.Length == 0)
			{
				baseCtorCallbacks = null;
			}
			this.baseCtorCallbacks = baseCtorCallbacks;
			if (Helpers.GetUnderlyingType(forType) != null)
			{
				throw new ArgumentException("Cannot create a TypeSerializer for nullable types", "forType");
			}
			if (model.MapType(TypeSerializer.iextensible).IsAssignableFrom(forType))
			{
				if (forType.IsValueType || !isRootType || flag)
				{
					throw new NotSupportedException("IExtensible is not supported in structs or classes with inheritance");
				}
				isExtensible = true;
			}
			hasConstructor = (!constructType.IsAbstract && Helpers.GetConstructor(constructType, Helpers.EmptyTypes, true) != null);
			if (constructType != forType && useConstructor && !hasConstructor)
			{
				throw new ArgumentException("The supplied default implementation cannot be created: " + constructType.FullName, "constructType");
			}
		}

		// Token: 0x06003616 RID: 13846 RVA: 0x00135BDC File Offset: 0x00133FDC
		public bool HasCallbacks(TypeModel.CallbackType callbackType)
		{
			if (callbacks != null && callbacks[callbackType] != null)
			{
				return true;
			}
			for (int i = 0; i < serializers.Length; i++)
			{
				if (serializers[i].ExpectedType != forType && ((IProtoTypeSerializer)serializers[i]).HasCallbacks(callbackType))
				{
					return true;
				}
			}
			return false;
		}

        // Token: 0x17000394 RID: 916
        // (get) Token: 0x06003617 RID: 13847 RVA: 0x00135C53 File Offset: 0x00134053
        public Type ExpectedType => forType;

        // Token: 0x17000395 RID: 917
        // (get) Token: 0x06003618 RID: 13848 RVA: 0x00135C5B File Offset: 0x0013405B
        private bool CanHaveInheritance => (forType.IsClass || forType.IsInterface) && !forType.IsSealed;

        // Token: 0x06003619 RID: 13849 RVA: 0x00135C8E File Offset: 0x0013408E
        bool IProtoTypeSerializer.CanCreateInstance()
		{
			return true;
		}

		// Token: 0x0600361A RID: 13850 RVA: 0x00135C91 File Offset: 0x00134091
		object IProtoTypeSerializer.CreateInstance(ProtoReader source)
		{
			return CreateInstance(source, false);
		}

		// Token: 0x0600361B RID: 13851 RVA: 0x00135C9C File Offset: 0x0013409C
		public void Callback(object value, TypeModel.CallbackType callbackType, SerializationContext context)
		{
			if (callbacks != null)
			{
				InvokeCallback(callbacks[callbackType], value, context);
			}
			IProtoTypeSerializer protoTypeSerializer = (IProtoTypeSerializer)GetMoreSpecificSerializer(value);
			if (protoTypeSerializer != null)
			{
				protoTypeSerializer.Callback(value, callbackType, context);
			}
		}

		// Token: 0x0600361C RID: 13852 RVA: 0x00135CE8 File Offset: 0x001340E8
		private IProtoSerializer GetMoreSpecificSerializer(object value)
		{
			if (!CanHaveInheritance)
			{
				return null;
			}
			Type type = value.GetType();
			if (type == forType)
			{
				return null;
			}
			for (int i = 0; i < serializers.Length; i++)
			{
				IProtoSerializer protoSerializer = serializers[i];
				if (protoSerializer.ExpectedType != forType && Helpers.IsAssignableFrom(protoSerializer.ExpectedType, type))
				{
					return protoSerializer;
				}
			}
			if (type == constructType)
			{
				return null;
			}
			TypeModel.ThrowUnexpectedSubtype(forType, type);
			return null;
		}

		// Token: 0x0600361D RID: 13853 RVA: 0x00135D78 File Offset: 0x00134178
		public void Write(object value, ProtoWriter dest)
		{
			if (isRootType)
			{
				Callback(value, TypeModel.CallbackType.BeforeSerialize, dest.Context);
			}
			IProtoSerializer moreSpecificSerializer = GetMoreSpecificSerializer(value);
			if (moreSpecificSerializer != null)
			{
				moreSpecificSerializer.Write(value, dest);
			}
			for (int i = 0; i < serializers.Length; i++)
			{
				IProtoSerializer protoSerializer = serializers[i];
				if (protoSerializer.ExpectedType == forType)
				{
					protoSerializer.Write(value, dest);
				}
			}
			if (isExtensible)
			{
				ProtoWriter.AppendExtensionData((IExtensible)value, dest);
			}
			if (isRootType)
			{
				Callback(value, TypeModel.CallbackType.AfterSerialize, dest.Context);
			}
		}

		// Token: 0x0600361E RID: 13854 RVA: 0x00135E20 File Offset: 0x00134220
		public object Read(object value, ProtoReader source)
		{
			if (isRootType && value != null)
			{
				Callback(value, TypeModel.CallbackType.BeforeDeserialize, source.Context);
			}
			int num = 0;
			int num2 = 0;
			int num3;
			while ((num3 = source.ReadFieldHeader()) > 0)
			{
				bool flag = false;
				if (num3 < num)
				{
					num2 = (num = 0);
				}
				for (int i = num2; i < fieldNumbers.Length; i++)
				{
					if (fieldNumbers[i] == num3)
					{
						IProtoSerializer protoSerializer = serializers[i];
						Type expectedType = protoSerializer.ExpectedType;
						if (value == null)
						{
							if (expectedType == forType)
							{
								value = CreateInstance(source, true);
							}
						}
						else if (expectedType != forType && ((IProtoTypeSerializer)protoSerializer).CanCreateInstance() && expectedType.IsSubclassOf(value.GetType()))
						{
							value = ProtoReader.Merge(source, value, ((IProtoTypeSerializer)protoSerializer).CreateInstance(source));
						}
						if (protoSerializer.ReturnsValue)
						{
							value = protoSerializer.Read(value, source);
						}
						else
						{
							protoSerializer.Read(value, source);
						}
						num2 = i;
						num = num3;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					if (value == null)
					{
						value = CreateInstance(source, true);
					}
					if (isExtensible)
					{
						source.AppendExtensionData((IExtensible)value);
					}
					else
					{
						source.SkipField();
					}
				}
			}
			if (value == null)
			{
				value = CreateInstance(source, true);
			}
			if (isRootType)
			{
				Callback(value, TypeModel.CallbackType.AfterDeserialize, source.Context);
			}
			return value;
		}

		// Token: 0x0600361F RID: 13855 RVA: 0x00135FAC File Offset: 0x001343AC
		private object InvokeCallback(MethodInfo method, object obj, SerializationContext context)
		{
			object result = null;
			if (method != null)
			{
				ParameterInfo[] parameters = method.GetParameters();
				int num = parameters.Length;
				object[] array;
				bool flag;
				if (num != 0)
				{
					array = new object[parameters.Length];
					flag = true;
					for (int i = 0; i < array.Length; i++)
					{
						Type parameterType = parameters[i].ParameterType;
						object obj2;
						if (parameterType == typeof(SerializationContext))
						{
							obj2 = context;
						}
						else if (parameterType == typeof(Type))
						{
							obj2 = constructType;
						}
						else
						{
							obj2 = null;
							flag = false;
						}
						array[i] = obj2;
					}
				}
				else
				{
					array = null;
					flag = true;
				}
				if (!flag)
				{
					throw CallbackSet.CreateInvalidCallbackSignature(method);
				}
				result = method.Invoke(obj, array);
			}
			return result;
		}

		// Token: 0x06003620 RID: 13856 RVA: 0x00136074 File Offset: 0x00134474
		private object CreateInstance(ProtoReader source, bool includeLocalCallback)
		{
			object obj;
			if (factory != null)
			{
				obj = InvokeCallback(factory, null, source.Context);
			}
			else if (useConstructor)
			{
				if (!hasConstructor)
				{
					TypeModel.ThrowCannotCreateInstance(constructType);
				}
				obj = Activator.CreateInstance(constructType, true);
			}
			else
			{
				obj = BclHelpers.GetUninitializedObject(constructType);
			}
			ProtoReader.NoteObject(obj, source);
			if (baseCtorCallbacks != null)
			{
				for (int i = 0; i < baseCtorCallbacks.Length; i++)
				{
					InvokeCallback(baseCtorCallbacks[i], obj, source.Context);
				}
			}
			if (includeLocalCallback && callbacks != null)
			{
				InvokeCallback(callbacks.BeforeDeserialize, obj, source.Context);
			}
			return obj;
		}

        // Token: 0x17000392 RID: 914
        // (get) Token: 0x06003621 RID: 13857 RVA: 0x00136150 File Offset: 0x00134550
        bool IProtoSerializer.RequiresOldValue => true;

        // Token: 0x17000393 RID: 915
        // (get) Token: 0x06003622 RID: 13858 RVA: 0x00136153 File Offset: 0x00134553
        bool IProtoSerializer.ReturnsValue => false;

        // Token: 0x040024FA RID: 9466
        private readonly Type forType;

		// Token: 0x040024FB RID: 9467
		private readonly Type constructType;

		// Token: 0x040024FC RID: 9468
		private readonly IProtoSerializer[] serializers;

		// Token: 0x040024FD RID: 9469
		private readonly int[] fieldNumbers;

		// Token: 0x040024FE RID: 9470
		private readonly bool isRootType;

		// Token: 0x040024FF RID: 9471
		private readonly bool useConstructor;

		// Token: 0x04002500 RID: 9472
		private readonly bool isExtensible;

		// Token: 0x04002501 RID: 9473
		private readonly bool hasConstructor;

		// Token: 0x04002502 RID: 9474
		private readonly CallbackSet callbacks;

		// Token: 0x04002503 RID: 9475
		private readonly MethodInfo[] baseCtorCallbacks;

		// Token: 0x04002504 RID: 9476
		private readonly MethodInfo factory;

		// Token: 0x04002505 RID: 9477
		private static readonly Type iextensible = typeof(IExtensible);
	}
}
