using ProtoBuf.Serializers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ProtoBuf.Meta
{
    // Token: 0x020003D9 RID: 985
    public class MetaType : ISerializerProxy
	{
		// Token: 0x06003300 RID: 13056 RVA: 0x00128660 File Offset: 0x00126A60
		internal MetaType(RuntimeTypeModel model, Type type, MethodInfo factory)
		{
			this.factory = factory;
			if (model == null)
			{
				throw new ArgumentNullException("model");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			IProtoSerializer protoSerializer = model.TryGetBasicTypeSerializer(type);
			if (protoSerializer != null)
			{
				throw MetaType.InbuiltType(type);
			}
			this.type = type;
			this.model = model;
			if (Helpers.IsEnum(type))
			{
				EnumPassthru = type.IsDefined(model.MapType(typeof(FlagsAttribute)), false);
			}
		}

		// Token: 0x06003301 RID: 13057 RVA: 0x001286F2 File Offset: 0x00126AF2
		public override string ToString()
		{
			return type.ToString();
		}

        // Token: 0x170002C1 RID: 705
        // (get) Token: 0x06003302 RID: 13058 RVA: 0x001286FF File Offset: 0x00126AFF
        IProtoSerializer ISerializerProxy.Serializer => Serializer;

        // Token: 0x170002C2 RID: 706
        // (get) Token: 0x06003303 RID: 13059 RVA: 0x00128707 File Offset: 0x00126B07
        public MetaType BaseType => baseType;

        // Token: 0x170002C3 RID: 707
        // (get) Token: 0x06003304 RID: 13060 RVA: 0x0012870F File Offset: 0x00126B0F
        internal TypeModel Model => model;

        // Token: 0x170002C4 RID: 708
        // (get) Token: 0x06003305 RID: 13061 RVA: 0x00128717 File Offset: 0x00126B17
        // (set) Token: 0x06003306 RID: 13062 RVA: 0x00128723 File Offset: 0x00126B23
        public bool IncludeSerializerMethod
        {
            get => !HasFlag(8);
            set => SetFlag(8, !value, true);
        }

        // Token: 0x170002C5 RID: 709
        // (get) Token: 0x06003307 RID: 13063 RVA: 0x00128731 File Offset: 0x00126B31
        // (set) Token: 0x06003308 RID: 13064 RVA: 0x0012873B File Offset: 0x00126B3B
        public bool AsReferenceDefault
        {
            get => HasFlag(32);
            set => SetFlag(32, value, true);
        }

        // Token: 0x06003309 RID: 13065 RVA: 0x00128747 File Offset: 0x00126B47
        private bool IsValidSubType(Type subType)
		{
			return type.IsAssignableFrom(subType);
		}

		// Token: 0x0600330A RID: 13066 RVA: 0x00128755 File Offset: 0x00126B55
		public MetaType AddSubType(int fieldNumber, Type derivedType)
		{
			return AddSubType(fieldNumber, derivedType, DataFormat.Default);
		}

		// Token: 0x0600330B RID: 13067 RVA: 0x00128760 File Offset: 0x00126B60
		public MetaType AddSubType(int fieldNumber, Type derivedType, DataFormat dataFormat)
		{
			if (derivedType == null)
			{
				throw new ArgumentNullException("derivedType");
			}
			if (fieldNumber < 1)
			{
				throw new ArgumentOutOfRangeException("fieldNumber");
			}
			if ((!type.IsClass && !type.IsInterface) || type.IsSealed)
			{
				throw new InvalidOperationException("Sub-types can only be added to non-sealed classes");
			}
			if (!IsValidSubType(derivedType))
			{
				throw new ArgumentException(derivedType.Name + " is not a valid sub-type of " + type.Name, "derivedType");
			}
			MetaType metaType = model[derivedType];
			ThrowIfFrozen();
			metaType.ThrowIfFrozen();
			SubType value = new SubType(fieldNumber, metaType, dataFormat);
			ThrowIfFrozen();
			metaType.SetBaseType(this);
			if (subTypes == null)
			{
				subTypes = new BasicList();
			}
			subTypes.Add(value);
			return this;
		}

		// Token: 0x0600330C RID: 13068 RVA: 0x00128850 File Offset: 0x00126C50
		private void SetBaseType(MetaType baseType)
		{
			if (baseType == null)
			{
				throw new ArgumentNullException("baseType");
			}
			if (this.baseType == baseType)
			{
				return;
			}
			if (this.baseType != null)
			{
				throw new InvalidOperationException("A type can only participate in one inheritance hierarchy");
			}
			for (MetaType metaType = baseType; metaType != null; metaType = metaType.baseType)
			{
				if (object.ReferenceEquals(metaType, this))
				{
					throw new InvalidOperationException("Cyclic inheritance is not allowed");
				}
			}
			this.baseType = baseType;
		}

        // Token: 0x170002C6 RID: 710
        // (get) Token: 0x0600330D RID: 13069 RVA: 0x001288C3 File Offset: 0x00126CC3
        public bool HasCallbacks => callbacks != null && callbacks.NonTrivial;

        // Token: 0x170002C7 RID: 711
        // (get) Token: 0x0600330E RID: 13070 RVA: 0x001288DE File Offset: 0x00126CDE
        public bool HasSubtypes => subTypes != null && subTypes.Count != 0;

        // Token: 0x170002C8 RID: 712
        // (get) Token: 0x0600330F RID: 13071 RVA: 0x001288FF File Offset: 0x00126CFF
        public CallbackSet Callbacks
		{
			get
			{
				if (callbacks == null)
				{
					callbacks = new CallbackSet(this);
				}
				return callbacks;
			}
		}

        // Token: 0x170002C9 RID: 713
        // (get) Token: 0x06003310 RID: 13072 RVA: 0x0012891E File Offset: 0x00126D1E
        private bool IsValueType => type.IsValueType;

        // Token: 0x06003311 RID: 13073 RVA: 0x0012892C File Offset: 0x00126D2C
        public MetaType SetCallbacks(MethodInfo beforeSerialize, MethodInfo afterSerialize, MethodInfo beforeDeserialize, MethodInfo afterDeserialize)
		{
			CallbackSet callbackSet = Callbacks;
			callbackSet.BeforeSerialize = beforeSerialize;
			callbackSet.AfterSerialize = afterSerialize;
			callbackSet.BeforeDeserialize = beforeDeserialize;
			callbackSet.AfterDeserialize = afterDeserialize;
			return this;
		}

		// Token: 0x06003312 RID: 13074 RVA: 0x00128960 File Offset: 0x00126D60
		public MetaType SetCallbacks(string beforeSerialize, string afterSerialize, string beforeDeserialize, string afterDeserialize)
		{
			if (IsValueType)
			{
				throw new InvalidOperationException();
			}
			CallbackSet callbackSet = Callbacks;
			callbackSet.BeforeSerialize = ResolveMethod(beforeSerialize, true);
			callbackSet.AfterSerialize = ResolveMethod(afterSerialize, true);
			callbackSet.BeforeDeserialize = ResolveMethod(beforeDeserialize, true);
			callbackSet.AfterDeserialize = ResolveMethod(afterDeserialize, true);
			return this;
		}

		// Token: 0x06003313 RID: 13075 RVA: 0x001289C0 File Offset: 0x00126DC0
		internal string GetSchemaTypeName()
		{
			if (surrogate != null)
			{
				return model[surrogate].GetSchemaTypeName();
			}
			if (!Helpers.IsNullOrEmpty(name))
			{
				return name;
			}
			string text = type.Name;
			if (type.IsGenericType)
			{
				StringBuilder stringBuilder = new StringBuilder(text);
				int num = text.IndexOf('`');
				if (num >= 0)
				{
					stringBuilder.Length = num;
				}
				foreach (Type type in type.GetGenericArguments())
				{
					stringBuilder.Append('_');
					Type type2 = type;
					int key = model.GetKey(ref type2);
					MetaType metaType;
					if (key >= 0 && (metaType = model[type2]) != null && metaType.surrogate == null)
					{
						stringBuilder.Append(metaType.GetSchemaTypeName());
					}
					else
					{
						stringBuilder.Append(type2.Name);
					}
				}
				return stringBuilder.ToString();
			}
			return text;
		}

		// Token: 0x170002CA RID: 714
		// (get) Token: 0x06003314 RID: 13076 RVA: 0x00128AD9 File Offset: 0x00126ED9
		// (set) Token: 0x06003315 RID: 13077 RVA: 0x00128AE1 File Offset: 0x00126EE1
		public string Name
        {
            get => name;
            set
            {
                ThrowIfFrozen();
                name = value;
            }
        }

        // Token: 0x06003316 RID: 13078 RVA: 0x00128AF0 File Offset: 0x00126EF0
        public MetaType SetFactory(MethodInfo factory)
		{
			model.VerifyFactory(factory, type);
			ThrowIfFrozen();
			this.factory = factory;
			return this;
		}

		// Token: 0x06003317 RID: 13079 RVA: 0x00128B12 File Offset: 0x00126F12
		public MetaType SetFactory(string factory)
		{
			return SetFactory(ResolveMethod(factory, false));
		}

		// Token: 0x06003318 RID: 13080 RVA: 0x00128B22 File Offset: 0x00126F22
		private MethodInfo ResolveMethod(string name, bool instance)
		{
			if (Helpers.IsNullOrEmpty(name))
			{
				return null;
			}
			return (!instance) ? Helpers.GetStaticMethod(type, name) : Helpers.GetInstanceMethod(type, name);
		}

		// Token: 0x06003319 RID: 13081 RVA: 0x00128B54 File Offset: 0x00126F54
		internal static Exception InbuiltType(Type type)
		{
			return new ArgumentException("Data of this type has inbuilt behaviour, and cannot be added to a model in this way: " + type.FullName);
		}

		// Token: 0x0600331A RID: 13082 RVA: 0x00128B6B File Offset: 0x00126F6B
		protected internal void ThrowIfFrozen()
		{
			if ((flags & 4) != 0)
			{
				throw new InvalidOperationException("The type cannot be changed once a serializer has been generated for " + type.FullName);
			}
		}

        // Token: 0x170002CB RID: 715
        // (get) Token: 0x0600331B RID: 13083 RVA: 0x00128B97 File Offset: 0x00126F97
        public Type Type => type;

        // Token: 0x170002CC RID: 716
        // (get) Token: 0x0600331C RID: 13084 RVA: 0x00128BA0 File Offset: 0x00126FA0
        internal IProtoTypeSerializer Serializer
		{
			get
			{
				if (serializer == null)
				{
					int opaqueToken = 0;
					try
					{
						model.TakeLock(ref opaqueToken);
						if (serializer == null)
						{
							SetFlag(4, true, false);
							serializer = BuildSerializer();
						}
					}
					finally
					{
						model.ReleaseLock(opaqueToken);
					}
				}
				return serializer;
			}
		}

		// Token: 0x170002CD RID: 717
		// (get) Token: 0x0600331D RID: 13085 RVA: 0x00128C10 File Offset: 0x00127010
		internal bool IsList
		{
			get
			{
				Type type = (!IgnoreListHandling) ? TypeModel.GetListItemType(model, this.type) : null;
				return type != null;
			}
		}

		// Token: 0x0600331E RID: 13086 RVA: 0x00128C48 File Offset: 0x00127048
		private IProtoTypeSerializer BuildSerializer()
		{
			if (Helpers.IsEnum(this.type))
			{
				return new TagDecorator(1, WireType.Variant, false, new EnumSerializer(this.type, GetEnumMap()));
			}
			Type type = (!IgnoreListHandling) ? TypeModel.GetListItemType(model, this.type) : null;
			if (type != null)
			{
				if (surrogate != null)
				{
					throw new ArgumentException("Repeated data (a list, collection, etc) has inbuilt behaviour and cannot use a surrogate");
				}
				if (subTypes != null && subTypes.Count != 0)
				{
					throw new ArgumentException("Repeated data (a list, collection, etc) has inbuilt behaviour and cannot be subclassed");
				}
				Type defaultType = null;
				MetaType.ResolveListTypes(model, this.type, ref type, ref defaultType);
				ValueMember valueMember = new ValueMember(model, 1, this.type, type, defaultType, DataFormat.Default);
				return new TypeSerializer(model, this.type, new int[]
				{
					1
				}, new IProtoSerializer[]
				{
					valueMember.Serializer
				}, null, true, true, null, constructType, factory);
			}
			else
			{
				if (surrogate != null)
				{
					MetaType metaType = model[surrogate];
					MetaType metaType2;
					while ((metaType2 = metaType.baseType) != null)
					{
						metaType = metaType2;
					}
					return new SurrogateSerializer(model, this.type, surrogate, metaType.Serializer);
				}
				if (!IsAutoTuple)
				{
					fields.Trim();
					int count = fields.Count;
					int num = (subTypes != null) ? subTypes.Count : 0;
					int[] array = new int[count + num];
					IProtoSerializer[] array2 = new IProtoSerializer[count + num];
					int num2 = 0;
					if (num != 0)
					{
						foreach (object obj in subTypes)
						{
							SubType subType = (SubType)obj;
							if (!subType.DerivedType.IgnoreListHandling && model.MapType(MetaType.ienumerable).IsAssignableFrom(subType.DerivedType.Type))
							{
								throw new ArgumentException("Repeated data (a list, collection, etc) has inbuilt behaviour and cannot be used as a subclass");
							}
							array[num2] = subType.FieldNumber;
							array2[num2++] = subType.Serializer;
						}
					}
					if (count != 0)
					{
						foreach (object obj2 in fields)
						{
							ValueMember valueMember2 = (ValueMember)obj2;
							array[num2] = valueMember2.FieldNumber;
							array2[num2++] = valueMember2.Serializer;
						}
					}
					BasicList basicList = null;
					for (MetaType metaType3 = BaseType; metaType3 != null; metaType3 = metaType3.BaseType)
					{
						MethodInfo methodInfo = (!metaType3.HasCallbacks) ? null : metaType3.Callbacks.BeforeDeserialize;
						if (methodInfo != null)
						{
							if (basicList == null)
							{
								basicList = new BasicList();
							}
							basicList.Add(methodInfo);
						}
					}
					MethodInfo[] array3 = null;
					if (basicList != null)
					{
						array3 = new MethodInfo[basicList.Count];
						basicList.CopyTo(array3, 0);
						Array.Reverse(array3);
					}
					return new TypeSerializer(model, this.type, array, array2, array3, baseType == null, UseConstructor, callbacks, constructType, factory);
				}
                ConstructorInfo constructorInfo = MetaType.ResolveTupleConstructor(this.type, out MemberInfo[] members);
                if (constructorInfo == null)
				{
					throw new InvalidOperationException();
				}
				return new TupleSerializer(model, constructorInfo, members);
			}
		}

		// Token: 0x0600331F RID: 13087 RVA: 0x00128FC7 File Offset: 0x001273C7
		private static Type GetBaseType(MetaType type)
		{
			return type.type.BaseType;
		}

		// Token: 0x06003320 RID: 13088 RVA: 0x00128FD4 File Offset: 0x001273D4
		internal static bool GetAsReferenceDefault(RuntimeTypeModel model, Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (Helpers.IsEnum(type))
			{
				return false;
			}
			AttributeMap[] array = AttributeMap.Create(model, type, false);
			for (int i = 0; i < array.Length; i++)
			{
                if (array[i].AttributeType.FullName == "ProtoBuf.ProtoContractAttribute" && array[i].TryGet("AsReferenceDefault", out object obj))
                {
                    return (bool)obj;
                }
            }
			return false;
		}

		// Token: 0x06003321 RID: 13089 RVA: 0x00129054 File Offset: 0x00127454
		internal void ApplyDefaultBehaviour()
		{
			Type type = MetaType.GetBaseType(this);
			if (type != null && model.FindWithoutAdd(type) == null && MetaType.GetContractFamily(model, type, null) != MetaType.AttributeFamily.None)
			{
				model.FindOrAddAuto(type, true, false, false);
			}
			AttributeMap[] array = AttributeMap.Create(model, this.type, false);
			MetaType.AttributeFamily attributeFamily = MetaType.GetContractFamily(model, this.type, array);
			if (attributeFamily == MetaType.AttributeFamily.AutoTuple)
			{
				SetFlag(64, true, true);
			}
			bool flag = !EnumPassthru && Helpers.IsEnum(this.type);
			if (attributeFamily == MetaType.AttributeFamily.None && !flag)
			{
				return;
			}
			BasicList basicList = null;
			BasicList basicList2 = null;
			int dataMemberOffset = 0;
			int num = 1;
			bool flag2 = model.InferTagFromNameDefault;
			ImplicitFields implicitFields = ImplicitFields.None;
			string text = null;
			foreach (AttributeMap attributeMap in array)
			{
				string fullName = attributeMap.AttributeType.FullName;
				object obj;
				if (!flag && fullName == "ProtoBuf.ProtoIncludeAttribute")
				{
					int fieldNumber = 0;
					if (attributeMap.TryGet("tag", out obj))
					{
						fieldNumber = (int)obj;
					}
					DataFormat dataFormat = DataFormat.Default;
					if (attributeMap.TryGet("DataFormat", out obj))
					{
						dataFormat = (DataFormat)((int)obj);
					}
					Type type2 = null;
					try
					{
						if (attributeMap.TryGet("knownTypeName", out obj))
						{
							type2 = model.GetType((string)obj, this.type.Assembly);
						}
						else if (attributeMap.TryGet("knownType", out obj))
						{
							type2 = (Type)obj;
						}
					}
					catch (Exception innerException)
					{
						throw new InvalidOperationException("Unable to resolve sub-type of: " + this.type.FullName, innerException);
					}
					if (type2 == null)
					{
						throw new InvalidOperationException("Unable to resolve sub-type of: " + this.type.FullName);
					}
					if (IsValidSubType(type2))
					{
						AddSubType(fieldNumber, type2, dataFormat);
					}
				}
				if (fullName == "ProtoBuf.ProtoPartialIgnoreAttribute" && attributeMap.TryGet("MemberName", out obj) && obj != null)
				{
					if (basicList == null)
					{
						basicList = new BasicList();
					}
					basicList.Add((string)obj);
				}
				if (!flag && fullName == "ProtoBuf.ProtoPartialMemberAttribute")
				{
					if (basicList2 == null)
					{
						basicList2 = new BasicList();
					}
					basicList2.Add(attributeMap);
				}
				if (fullName == "ProtoBuf.ProtoContractAttribute")
				{
					if (attributeMap.TryGet("Name", out obj))
					{
						text = (string)obj;
					}
					if (Helpers.IsEnum(this.type))
					{
						if (attributeMap.TryGet("EnumPassthruHasValue", false, out obj) && (bool)obj && attributeMap.TryGet("EnumPassthru", out obj))
						{
							EnumPassthru = (bool)obj;
							if (EnumPassthru)
							{
								flag = false;
							}
						}
					}
					else
					{
						if (attributeMap.TryGet("DataMemberOffset", out obj))
						{
							dataMemberOffset = (int)obj;
						}
						if (attributeMap.TryGet("InferTagFromNameHasValue", false, out obj) && (bool)obj && attributeMap.TryGet("InferTagFromName", out obj))
						{
							flag2 = (bool)obj;
						}
						if (attributeMap.TryGet("ImplicitFields", out obj) && obj != null)
						{
							implicitFields = (ImplicitFields)((int)obj);
						}
						if (attributeMap.TryGet("SkipConstructor", out obj))
						{
							UseConstructor = !(bool)obj;
						}
						if (attributeMap.TryGet("IgnoreListHandling", out obj))
						{
							IgnoreListHandling = (bool)obj;
						}
						if (attributeMap.TryGet("AsReferenceDefault", out obj))
						{
							AsReferenceDefault = (bool)obj;
						}
						if (attributeMap.TryGet("ImplicitFirstTag", out obj) && (int)obj > 0)
						{
							num = (int)obj;
						}
					}
				}
				if (fullName == "System.Runtime.Serialization.DataContractAttribute" && text == null && attributeMap.TryGet("Name", out obj))
				{
					text = (string)obj;
				}
				if (fullName == "System.Xml.Serialization.XmlTypeAttribute" && text == null && attributeMap.TryGet("TypeName", out obj))
				{
					text = (string)obj;
				}
			}
			if (!Helpers.IsNullOrEmpty(text))
			{
				Name = text;
			}
			if (implicitFields != ImplicitFields.None)
			{
				attributeFamily &= MetaType.AttributeFamily.ProtoBuf;
			}
			MethodInfo[] array2 = null;
			BasicList basicList3 = new BasicList();
			MemberInfo[] members = this.type.GetMembers((!flag) ? (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) : (BindingFlags.Static | BindingFlags.Public));
			foreach (MemberInfo memberInfo in members)
			{
				if (memberInfo.DeclaringType == this.type)
				{
					if (!memberInfo.IsDefined(model.MapType(typeof(ProtoIgnoreAttribute)), true))
					{
						if (basicList == null || !basicList.Contains(memberInfo.Name))
						{
							bool flag3 = false;
							PropertyInfo propertyInfo;
							FieldInfo fieldInfo;
							MethodInfo methodInfo;
							if ((propertyInfo = (memberInfo as PropertyInfo)) != null)
							{
								if (!flag)
								{
									MemberInfo backingMember = null;
									if (!propertyInfo.CanWrite)
									{
										string b = "<{property.Name}>k__BackingField";
										foreach (MemberInfo memberInfo2 in members)
										{
											if (memberInfo2 is FieldInfo && memberInfo2.Name == b)
											{
												backingMember = memberInfo2;
												break;
											}
										}
									}
									Type type3 = propertyInfo.PropertyType;
									bool isPublic = Helpers.GetGetMethod(propertyInfo, false, false) != null;
									bool isField = false;
									MetaType.ApplyDefaultBehaviour_AddMembers(model, attributeFamily, flag, basicList2, dataMemberOffset, flag2, implicitFields, basicList3, memberInfo, ref flag3, isPublic, isField, ref type3, backingMember);
								}
							}
							else if ((fieldInfo = (memberInfo as FieldInfo)) != null)
							{
								Type type3 = fieldInfo.FieldType;
								bool isPublic = fieldInfo.IsPublic;
								bool isField = true;
								if (!flag || fieldInfo.IsStatic)
								{
									MetaType.ApplyDefaultBehaviour_AddMembers(model, attributeFamily, flag, basicList2, dataMemberOffset, flag2, implicitFields, basicList3, memberInfo, ref flag3, isPublic, isField, ref type3, null);
								}
							}
							else if ((methodInfo = (memberInfo as MethodInfo)) != null)
							{
								if (!flag)
								{
									AttributeMap[] array5 = AttributeMap.Create(model, methodInfo, false);
									if (array5 != null && array5.Length > 0)
									{
										MetaType.CheckForCallback(methodInfo, array5, "ProtoBuf.ProtoBeforeSerializationAttribute", ref array2, 0);
										MetaType.CheckForCallback(methodInfo, array5, "ProtoBuf.ProtoAfterSerializationAttribute", ref array2, 1);
										MetaType.CheckForCallback(methodInfo, array5, "ProtoBuf.ProtoBeforeDeserializationAttribute", ref array2, 2);
										MetaType.CheckForCallback(methodInfo, array5, "ProtoBuf.ProtoAfterDeserializationAttribute", ref array2, 3);
										MetaType.CheckForCallback(methodInfo, array5, "System.Runtime.Serialization.OnSerializingAttribute", ref array2, 4);
										MetaType.CheckForCallback(methodInfo, array5, "System.Runtime.Serialization.OnSerializedAttribute", ref array2, 5);
										MetaType.CheckForCallback(methodInfo, array5, "System.Runtime.Serialization.OnDeserializingAttribute", ref array2, 6);
										MetaType.CheckForCallback(methodInfo, array5, "System.Runtime.Serialization.OnDeserializedAttribute", ref array2, 7);
									}
								}
							}
						}
					}
				}
			}
			ProtoMemberAttribute[] array6 = new ProtoMemberAttribute[basicList3.Count];
			basicList3.CopyTo(array6, 0);
			if (flag2 || implicitFields != ImplicitFields.None)
			{
				Array.Sort<ProtoMemberAttribute>(array6);
				int num2 = num;
				foreach (ProtoMemberAttribute protoMemberAttribute in array6)
				{
					if (!protoMemberAttribute.TagIsPinned)
					{
						protoMemberAttribute.Rebase(num2++);
					}
				}
			}
			foreach (ProtoMemberAttribute normalizedAttribute in array6)
			{
				ValueMember valueMember = ApplyDefaultBehaviour(flag, normalizedAttribute);
				if (valueMember != null)
				{
					Add(valueMember);
				}
			}
			if (array2 != null)
			{
				SetCallbacks(MetaType.Coalesce(array2, 0, 4), MetaType.Coalesce(array2, 1, 5), MetaType.Coalesce(array2, 2, 6), MetaType.Coalesce(array2, 3, 7));
			}
		}

		// Token: 0x06003322 RID: 13090 RVA: 0x00129868 File Offset: 0x00127C68
		private static void ApplyDefaultBehaviour_AddMembers(TypeModel model, MetaType.AttributeFamily family, bool isEnum, BasicList partialMembers, int dataMemberOffset, bool inferTagByName, ImplicitFields implicitMode, BasicList members, MemberInfo member, ref bool forced, bool isPublic, bool isField, ref Type effectiveType, MemberInfo backingMember = null)
		{
			if (implicitMode != ImplicitFields.AllFields)
			{
				if (implicitMode == ImplicitFields.AllPublic)
				{
					if (isPublic)
					{
						forced = true;
					}
				}
			}
			else if (isField)
			{
				forced = true;
			}
			if (effectiveType.IsSubclassOf(model.MapType(typeof(Delegate))))
			{
				effectiveType = null;
			}
			if (effectiveType != null)
			{
				ProtoMemberAttribute protoMemberAttribute = MetaType.NormalizeProtoMember(model, member, family, forced, isEnum, partialMembers, dataMemberOffset, inferTagByName, backingMember);
				if (protoMemberAttribute != null)
				{
					members.Add(protoMemberAttribute);
				}
			}
		}

		// Token: 0x06003323 RID: 13091 RVA: 0x001298F8 File Offset: 0x00127CF8
		private static MethodInfo Coalesce(MethodInfo[] arr, int x, int y)
		{
			MethodInfo methodInfo = arr[x];
			if (methodInfo == null)
			{
				methodInfo = arr[y];
			}
			return methodInfo;
		}

		// Token: 0x06003324 RID: 13092 RVA: 0x00129914 File Offset: 0x00127D14
		internal static MetaType.AttributeFamily GetContractFamily(RuntimeTypeModel model, Type type, AttributeMap[] attributes)
		{
			MetaType.AttributeFamily attributeFamily = MetaType.AttributeFamily.None;
			if (attributes == null)
			{
				attributes = AttributeMap.Create(model, type, false);
			}
			for (int i = 0; i < attributes.Length; i++)
			{
				string fullName = attributes[i].AttributeType.FullName;
				if (fullName != null)
				{
					if (!(fullName == "ProtoBuf.ProtoContractAttribute"))
					{
						if (!(fullName == "System.Xml.Serialization.XmlTypeAttribute"))
						{
							if (fullName == "System.Runtime.Serialization.DataContractAttribute")
							{
								if (!model.AutoAddProtoContractTypesOnly)
								{
									attributeFamily |= MetaType.AttributeFamily.DataContractSerialier;
								}
							}
						}
						else if (!model.AutoAddProtoContractTypesOnly)
						{
							attributeFamily |= MetaType.AttributeFamily.XmlSerializer;
						}
					}
					else
					{
						bool flag = false;
						MetaType.GetFieldBoolean(ref flag, attributes[i], "UseProtoMembersOnly");
						if (flag)
						{
							return MetaType.AttributeFamily.ProtoBuf;
						}
						attributeFamily |= MetaType.AttributeFamily.ProtoBuf;
					}
				}
			}
            if (attributeFamily == MetaType.AttributeFamily.None && MetaType.ResolveTupleConstructor(type, out MemberInfo[] array) != null)
            {
                attributeFamily |= MetaType.AttributeFamily.AutoTuple;
            }
            return attributeFamily;
		}

		// Token: 0x06003325 RID: 13093 RVA: 0x001299F4 File Offset: 0x00127DF4
		internal static ConstructorInfo ResolveTupleConstructor(Type type, out MemberInfo[] mappedMembers)
		{
			mappedMembers = null;
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (type.IsAbstract)
			{
				return null;
			}
			ConstructorInfo[] constructors = Helpers.GetConstructors(type, false);
			if (constructors.Length == 0 || (constructors.Length == 1 && constructors[0].GetParameters().Length == 0))
			{
				return null;
			}
			MemberInfo[] instanceFieldsAndProperties = Helpers.GetInstanceFieldsAndProperties(type, true);
			BasicList basicList = new BasicList();
			bool flag = type.Name.IndexOf("Tuple", StringComparison.OrdinalIgnoreCase) < 0;
			for (int i = 0; i < instanceFieldsAndProperties.Length; i++)
			{
				PropertyInfo propertyInfo = instanceFieldsAndProperties[i] as PropertyInfo;
				if (propertyInfo != null)
				{
					if (!propertyInfo.CanRead)
					{
						return null;
					}
					if (flag && propertyInfo.CanWrite && Helpers.GetSetMethod(propertyInfo, false, false) != null)
					{
						return null;
					}
					basicList.Add(propertyInfo);
				}
				else
				{
					FieldInfo fieldInfo = instanceFieldsAndProperties[i] as FieldInfo;
					if (fieldInfo != null)
					{
						if (flag && !fieldInfo.IsInitOnly)
						{
							return null;
						}
						basicList.Add(fieldInfo);
					}
				}
			}
			if (basicList.Count == 0)
			{
				return null;
			}
			MemberInfo[] array = new MemberInfo[basicList.Count];
			basicList.CopyTo(array, 0);
			int[] array2 = new int[array.Length];
			int num = 0;
			ConstructorInfo constructorInfo = null;
			mappedMembers = new MemberInfo[array2.Length];
			for (int j = 0; j < constructors.Length; j++)
			{
				ParameterInfo[] parameters = constructors[j].GetParameters();
				if (parameters.Length == array.Length)
				{
					for (int k = 0; k < array2.Length; k++)
					{
						array2[k] = -1;
					}
					for (int l = 0; l < parameters.Length; l++)
					{
						for (int m = 0; m < array.Length; m++)
						{
							if (string.Compare(parameters[l].Name, array[m].Name, StringComparison.OrdinalIgnoreCase) == 0)
							{
								Type memberType = Helpers.GetMemberType(array[m]);
								if (memberType == parameters[l].ParameterType)
								{
									array2[l] = m;
								}
							}
						}
					}
					bool flag2 = false;
					for (int n = 0; n < array2.Length; n++)
					{
						if (array2[n] < 0)
						{
							flag2 = true;
							break;
						}
						mappedMembers[n] = array[array2[n]];
					}
					if (!flag2)
					{
						num++;
						constructorInfo = constructors[j];
					}
				}
			}
			return (num != 1) ? null : constructorInfo;
		}

		// Token: 0x06003326 RID: 13094 RVA: 0x00129C7C File Offset: 0x0012807C
		private static void CheckForCallback(MethodInfo method, AttributeMap[] attributes, string callbackTypeName, ref MethodInfo[] callbacks, int index)
		{
			for (int i = 0; i < attributes.Length; i++)
			{
				if (attributes[i].AttributeType.FullName == callbackTypeName)
				{
					if (callbacks == null)
					{
						callbacks = new MethodInfo[8];
					}
					else if (callbacks[index] != null)
					{
						Type reflectedType = method.ReflectedType;
						throw new ProtoException("Duplicate " + callbackTypeName + " callbacks on " + reflectedType.FullName);
					}
					callbacks[index] = method;
				}
			}
		}

		// Token: 0x06003327 RID: 13095 RVA: 0x00129CFC File Offset: 0x001280FC
		private static bool HasFamily(MetaType.AttributeFamily value, MetaType.AttributeFamily required)
		{
			return (value & required) == required;
		}

		// Token: 0x06003328 RID: 13096 RVA: 0x00129D04 File Offset: 0x00128104
		private static ProtoMemberAttribute NormalizeProtoMember(TypeModel model, MemberInfo member, MetaType.AttributeFamily family, bool forced, bool isEnum, BasicList partialMembers, int dataMemberOffset, bool inferByTagName, MemberInfo backingMember = null)
		{
			if (member == null || (family == MetaType.AttributeFamily.None && !isEnum))
			{
				return null;
			}
			int num = int.MinValue;
			int num2 = (!inferByTagName) ? 1 : -1;
			string text = null;
			bool isPacked = false;
			bool flag = false;
			bool flag2 = false;
			bool isRequired = false;
			bool asReference = false;
			bool flag3 = false;
			bool dynamicType = false;
			bool tagIsPinned = false;
			bool overwriteList = false;
			DataFormat dataFormat = DataFormat.Default;
			if (isEnum)
			{
				forced = true;
			}
			AttributeMap[] attribs = AttributeMap.Create(model, member, true);
			if (isEnum)
			{
				AttributeMap attribute = MetaType.GetAttribute(attribs, "ProtoBuf.ProtoIgnoreAttribute");
				if (attribute != null)
				{
					flag = true;
				}
				else
				{
					attribute = MetaType.GetAttribute(attribs, "ProtoBuf.ProtoEnumAttribute");
					num = Convert.ToInt32(((FieldInfo)member).GetRawConstantValue());
					if (attribute != null)
					{
						MetaType.GetFieldName(ref text, attribute, "Name");
                        if ((bool)Helpers.GetInstanceMethod(attribute.AttributeType, "HasValue").Invoke(attribute.Target, null) && attribute.TryGet("Value", out object obj))
                        {
                            num = (int)obj;
                        }
                    }
				}
				flag2 = true;
			}
			if (!flag && !flag2)
			{
				AttributeMap attribute = MetaType.GetAttribute(attribs, "ProtoBuf.ProtoMemberAttribute");
				MetaType.GetIgnore(ref flag, attribute, attribs, "ProtoBuf.ProtoIgnoreAttribute");
				if (!flag && attribute != null)
				{
					MetaType.GetFieldNumber(ref num, attribute, "Tag");
					MetaType.GetFieldName(ref text, attribute, "Name");
					MetaType.GetFieldBoolean(ref isRequired, attribute, "IsRequired");
					MetaType.GetFieldBoolean(ref isPacked, attribute, "IsPacked");
					MetaType.GetFieldBoolean(ref overwriteList, attribute, "OverwriteList");
					MetaType.GetDataFormat(ref dataFormat, attribute, "DataFormat");
					MetaType.GetFieldBoolean(ref flag3, attribute, "AsReferenceHasValue", false);
					if (flag3)
					{
						flag3 = MetaType.GetFieldBoolean(ref asReference, attribute, "AsReference", true);
					}
					MetaType.GetFieldBoolean(ref dynamicType, attribute, "DynamicType");
					tagIsPinned = (flag2 = (num > 0));
				}
				if (!flag2 && partialMembers != null)
				{
					foreach (object obj2 in partialMembers)
					{
						AttributeMap attributeMap = (AttributeMap)obj2;
                        if (attributeMap.TryGet("MemberName", out object obj3) && (string)obj3 == member.Name)
                        {
                            MetaType.GetFieldNumber(ref num, attributeMap, "Tag");
                            MetaType.GetFieldName(ref text, attributeMap, "Name");
                            MetaType.GetFieldBoolean(ref isRequired, attributeMap, "IsRequired");
                            MetaType.GetFieldBoolean(ref isPacked, attributeMap, "IsPacked");
                            MetaType.GetFieldBoolean(ref overwriteList, attribute, "OverwriteList");
                            MetaType.GetDataFormat(ref dataFormat, attributeMap, "DataFormat");
                            MetaType.GetFieldBoolean(ref flag3, attribute, "AsReferenceHasValue", false);
                            if (flag3)
                            {
                                flag3 = MetaType.GetFieldBoolean(ref asReference, attributeMap, "AsReference", true);
                            }
                            MetaType.GetFieldBoolean(ref dynamicType, attributeMap, "DynamicType");
                            if (flag2 = (tagIsPinned = (num > 0)))
                            {
                                break;
                            }
                        }
                    }
				}
			}
			if (!flag && !flag2 && MetaType.HasFamily(family, MetaType.AttributeFamily.DataContractSerialier))
			{
				AttributeMap attribute = MetaType.GetAttribute(attribs, "System.Runtime.Serialization.DataMemberAttribute");
				if (attribute != null)
				{
					MetaType.GetFieldNumber(ref num, attribute, "Order");
					MetaType.GetFieldName(ref text, attribute, "Name");
					MetaType.GetFieldBoolean(ref isRequired, attribute, "IsRequired");
					flag2 = (num >= num2);
					if (flag2)
					{
						num += dataMemberOffset;
					}
				}
			}
			if (!flag && !flag2 && MetaType.HasFamily(family, MetaType.AttributeFamily.XmlSerializer))
			{
				AttributeMap attribute = MetaType.GetAttribute(attribs, "System.Xml.Serialization.XmlElementAttribute");
				if (attribute == null)
				{
					attribute = MetaType.GetAttribute(attribs, "System.Xml.Serialization.XmlArrayAttribute");
				}
				MetaType.GetIgnore(ref flag, attribute, attribs, "System.Xml.Serialization.XmlIgnoreAttribute");
				if (attribute != null && !flag)
				{
					MetaType.GetFieldNumber(ref num, attribute, "Order");
					MetaType.GetFieldName(ref text, attribute, "ElementName");
					flag2 = (num >= num2);
				}
			}
			if (!flag && !flag2 && MetaType.GetAttribute(attribs, "System.NonSerializedAttribute") != null)
			{
				flag = true;
			}
			if (flag || (num < num2 && !forced))
			{
				return null;
			}
			return new ProtoMemberAttribute(num, forced || inferByTagName)
			{
				AsReference = asReference,
				AsReferenceHasValue = flag3,
				DataFormat = dataFormat,
				DynamicType = dynamicType,
				IsPacked = isPacked,
				OverwriteList = overwriteList,
				IsRequired = isRequired,
				Name = ((!Helpers.IsNullOrEmpty(text)) ? text : member.Name),
				Member = (backingMember ?? member),
				TagIsPinned = tagIsPinned
			};
		}

		// Token: 0x06003329 RID: 13097 RVA: 0x0012A188 File Offset: 0x00128588
		private ValueMember ApplyDefaultBehaviour(bool isEnum, ProtoMemberAttribute normalizedAttribute)
		{
			MemberInfo member;
			if (normalizedAttribute == null || (member = normalizedAttribute.Member) == null)
			{
				return null;
			}
			Type memberType = Helpers.GetMemberType(member);
			Type type = null;
			Type defaultType = null;
			MetaType.ResolveListTypes(model, memberType, ref type, ref defaultType);
			if (type != null)
			{
				int num = model.FindOrAddAuto(memberType, false, true, false);
				if (num >= 0 && model[memberType].IgnoreListHandling)
				{
					type = null;
					defaultType = null;
				}
			}
			AttributeMap[] attribs = AttributeMap.Create(model, member, true);
			object defaultValue = null;
			if (model.UseImplicitZeroDefaults)
			{
				ProtoTypeCode typeCode = Helpers.GetTypeCode(memberType);
				switch (typeCode)
				{
				case ProtoTypeCode.Boolean:
					defaultValue = false;
					break;
				case ProtoTypeCode.Char:
					defaultValue = '\0';
					break;
				case ProtoTypeCode.SByte:
					defaultValue = 0;
					break;
				case ProtoTypeCode.Byte:
					defaultValue = 0;
					break;
				case ProtoTypeCode.Int16:
					defaultValue = 0;
					break;
				case ProtoTypeCode.UInt16:
					defaultValue = 0;
					break;
				case ProtoTypeCode.Int32:
					defaultValue = 0;
					break;
				case ProtoTypeCode.UInt32:
					defaultValue = 0U;
					break;
				case ProtoTypeCode.Int64:
					defaultValue = 0L;
					break;
				case ProtoTypeCode.UInt64:
					defaultValue = 0UL;
					break;
				case ProtoTypeCode.Single:
					defaultValue = 0f;
					break;
				case ProtoTypeCode.Double:
					defaultValue = 0.0;
					break;
				case ProtoTypeCode.Decimal:
					defaultValue = 0m;
					break;
				default:
					switch (typeCode)
					{
					case ProtoTypeCode.TimeSpan:
						defaultValue = TimeSpan.Zero;
						break;
					case ProtoTypeCode.Guid:
						defaultValue = Guid.Empty;
						break;
					}
					break;
				}
			}
			AttributeMap attribute;
            if ((attribute = MetaType.GetAttribute(attribs, "System.ComponentModel.DefaultValueAttribute")) != null && attribute.TryGet("Value", out object obj))
            {
                defaultValue = obj;
            }
            ValueMember valueMember = (!isEnum && normalizedAttribute.Tag <= 0) ? null : new ValueMember(model, this.type, normalizedAttribute.Tag, member, memberType, type, defaultType, normalizedAttribute.DataFormat, defaultValue);
			if (valueMember != null)
			{
				Type declaringType = this.type;
				PropertyInfo propertyInfo = Helpers.GetProperty(declaringType, member.Name + "Specified", true);
				MethodInfo getMethod = Helpers.GetGetMethod(propertyInfo, true, true);
				if (getMethod == null || getMethod.IsStatic)
				{
					propertyInfo = null;
				}
				if (propertyInfo != null)
				{
					valueMember.SetSpecified(getMethod, Helpers.GetSetMethod(propertyInfo, true, true));
				}
				else
				{
					MethodInfo instanceMethod = Helpers.GetInstanceMethod(declaringType, "ShouldSerialize" + member.Name, Helpers.EmptyTypes);
					if (instanceMethod != null && instanceMethod.ReturnType == model.MapType(typeof(bool)))
					{
						valueMember.SetSpecified(instanceMethod, null);
					}
				}
				if (!Helpers.IsNullOrEmpty(normalizedAttribute.Name))
				{
					valueMember.SetName(normalizedAttribute.Name);
				}
				valueMember.IsPacked = normalizedAttribute.IsPacked;
				valueMember.IsRequired = normalizedAttribute.IsRequired;
				valueMember.OverwriteList = normalizedAttribute.OverwriteList;
				if (normalizedAttribute.AsReferenceHasValue)
				{
					valueMember.AsReference = normalizedAttribute.AsReference;
				}
				valueMember.DynamicType = normalizedAttribute.DynamicType;
			}
			return valueMember;
		}

		// Token: 0x0600332A RID: 13098 RVA: 0x0012A4EC File Offset: 0x001288EC
		private static void GetDataFormat(ref DataFormat value, AttributeMap attrib, string memberName)
		{
			if (attrib == null || value != DataFormat.Default)
			{
				return;
			}
            if (attrib.TryGet(memberName, out object obj) && obj != null)
            {
                value = (DataFormat)obj;
            }
        }

		// Token: 0x0600332B RID: 13099 RVA: 0x0012A523 File Offset: 0x00128923
		private static void GetIgnore(ref bool ignore, AttributeMap attrib, AttributeMap[] attribs, string fullName)
		{
			if (ignore || attrib == null)
			{
				return;
			}
			ignore = (MetaType.GetAttribute(attribs, fullName) != null);
		}

		// Token: 0x0600332C RID: 13100 RVA: 0x0012A542 File Offset: 0x00128942
		private static void GetFieldBoolean(ref bool value, AttributeMap attrib, string memberName)
		{
			MetaType.GetFieldBoolean(ref value, attrib, memberName, true);
		}

		// Token: 0x0600332D RID: 13101 RVA: 0x0012A550 File Offset: 0x00128950
		private static bool GetFieldBoolean(ref bool value, AttributeMap attrib, string memberName, bool publicOnly)
		{
			if (attrib == null)
			{
				return false;
			}
			if (value)
			{
				return true;
			}
            if (attrib.TryGet(memberName, publicOnly, out object obj) && obj != null)
            {
                value = (bool)obj;
                return true;
            }
            return false;
		}

		// Token: 0x0600332E RID: 13102 RVA: 0x0012A590 File Offset: 0x00128990
		private static void GetFieldNumber(ref int value, AttributeMap attrib, string memberName)
		{
			if (attrib == null || value > 0)
			{
				return;
			}
            if (attrib.TryGet(memberName, out object obj) && obj != null)
            {
                value = (int)obj;
            }
        }

		// Token: 0x0600332F RID: 13103 RVA: 0x0012A5C8 File Offset: 0x001289C8
		private static void GetFieldName(ref string name, AttributeMap attrib, string memberName)
		{
			if (attrib == null || !Helpers.IsNullOrEmpty(name))
			{
				return;
			}
            if (attrib.TryGet(memberName, out object obj) && obj != null)
            {
                name = (string)obj;
            }
        }

		// Token: 0x06003330 RID: 13104 RVA: 0x0012A604 File Offset: 0x00128A04
		private static AttributeMap GetAttribute(AttributeMap[] attribs, string fullName)
		{
			foreach (AttributeMap attributeMap in attribs)
			{
				if (attributeMap != null && attributeMap.AttributeType.FullName == fullName)
				{
					return attributeMap;
				}
			}
			return null;
		}

		// Token: 0x06003331 RID: 13105 RVA: 0x0012A648 File Offset: 0x00128A48
		public MetaType Add(int fieldNumber, string memberName)
		{
			AddField(fieldNumber, memberName, null, null, null);
			return this;
		}

		// Token: 0x06003332 RID: 13106 RVA: 0x0012A657 File Offset: 0x00128A57
		public ValueMember AddField(int fieldNumber, string memberName)
		{
			return AddField(fieldNumber, memberName, null, null, null);
		}

		// Token: 0x170002CE RID: 718
		// (get) Token: 0x06003333 RID: 13107 RVA: 0x0012A664 File Offset: 0x00128A64
		// (set) Token: 0x06003334 RID: 13108 RVA: 0x0012A671 File Offset: 0x00128A71
		public bool UseConstructor
        {
            get => !HasFlag(16);
            set => SetFlag(16, !value, true);
        }

        // Token: 0x170002CF RID: 719
        // (get) Token: 0x06003335 RID: 13109 RVA: 0x0012A680 File Offset: 0x00128A80
        // (set) Token: 0x06003336 RID: 13110 RVA: 0x0012A688 File Offset: 0x00128A88
        public Type ConstructType
        {
            get => constructType;
            set
            {
                ThrowIfFrozen();
                constructType = value;
            }
        }

        // Token: 0x06003337 RID: 13111 RVA: 0x0012A697 File Offset: 0x00128A97
        public MetaType Add(string memberName)
		{
			Add(GetNextFieldNumber(), memberName);
			return this;
		}

		// Token: 0x06003338 RID: 13112 RVA: 0x0012A6A8 File Offset: 0x00128AA8
		public void SetSurrogate(Type surrogateType)
		{
			if (surrogateType == type)
			{
				surrogateType = null;
			}
			if (surrogateType != null && surrogateType != null && Helpers.IsAssignableFrom(model.MapType(typeof(IEnumerable)), surrogateType))
			{
				throw new ArgumentException("Repeated data (a list, collection, etc) has inbuilt behaviour and cannot be used as a surrogate");
			}
			ThrowIfFrozen();
			surrogate = surrogateType;
		}

		// Token: 0x06003339 RID: 13113 RVA: 0x0012A708 File Offset: 0x00128B08
		internal MetaType GetSurrogateOrSelf()
		{
			if (surrogate != null)
			{
				return model[surrogate];
			}
			return this;
		}

		// Token: 0x0600333A RID: 13114 RVA: 0x0012A728 File Offset: 0x00128B28
		internal MetaType GetSurrogateOrBaseOrSelf(bool deep)
		{
			if (surrogate != null)
			{
				return model[surrogate];
			}
			MetaType metaType = baseType;
			if (metaType == null)
			{
				return this;
			}
			if (deep)
			{
				MetaType result;
				do
				{
					result = metaType;
					metaType = metaType.baseType;
				}
				while (metaType != null);
				return result;
			}
			return metaType;
		}

		// Token: 0x0600333B RID: 13115 RVA: 0x0012A77C File Offset: 0x00128B7C
		private int GetNextFieldNumber()
		{
			int num = 0;
			foreach (object obj in fields)
			{
				ValueMember valueMember = (ValueMember)obj;
				if (valueMember.FieldNumber > num)
				{
					num = valueMember.FieldNumber;
				}
			}
			if (subTypes != null)
			{
				foreach (object obj2 in subTypes)
				{
					SubType subType = (SubType)obj2;
					if (subType.FieldNumber > num)
					{
						num = subType.FieldNumber;
					}
				}
			}
			return num + 1;
		}

		// Token: 0x0600333C RID: 13116 RVA: 0x0012A814 File Offset: 0x00128C14
		public MetaType Add(params string[] memberNames)
		{
			if (memberNames == null)
			{
				throw new ArgumentNullException("memberNames");
			}
			int nextFieldNumber = GetNextFieldNumber();
			for (int i = 0; i < memberNames.Length; i++)
			{
				Add(nextFieldNumber++, memberNames[i]);
			}
			return this;
		}

		// Token: 0x0600333D RID: 13117 RVA: 0x0012A85D File Offset: 0x00128C5D
		public MetaType Add(int fieldNumber, string memberName, object defaultValue)
		{
			AddField(fieldNumber, memberName, null, null, defaultValue);
			return this;
		}

		// Token: 0x0600333E RID: 13118 RVA: 0x0012A86C File Offset: 0x00128C6C
		public MetaType Add(int fieldNumber, string memberName, Type itemType, Type defaultType)
		{
			AddField(fieldNumber, memberName, itemType, defaultType, null);
			return this;
		}

		// Token: 0x0600333F RID: 13119 RVA: 0x0012A87C File Offset: 0x00128C7C
		public ValueMember AddField(int fieldNumber, string memberName, Type itemType, Type defaultType)
		{
			return AddField(fieldNumber, memberName, itemType, defaultType, null);
		}

		// Token: 0x06003340 RID: 13120 RVA: 0x0012A88C File Offset: 0x00128C8C
		private ValueMember AddField(int fieldNumber, string memberName, Type itemType, Type defaultType, object defaultValue)
		{
			MemberInfo memberInfo = null;
			MemberInfo[] member = type.GetMember(memberName, (!Helpers.IsEnum(type)) ? (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) : (BindingFlags.Static | BindingFlags.Public));
			if (member != null && member.Length == 1)
			{
				memberInfo = member[0];
			}
			if (memberInfo == null)
			{
				throw new ArgumentException("Unable to determine member: " + memberName, "memberName");
			}
			MemberTypes memberType = memberInfo.MemberType;
			Type memberType2;
			if (memberType != MemberTypes.Field)
			{
				if (memberType != MemberTypes.Property)
				{
					throw new NotSupportedException(memberInfo.MemberType.ToString());
				}
				memberType2 = ((PropertyInfo)memberInfo).PropertyType;
			}
			else
			{
				memberType2 = ((FieldInfo)memberInfo).FieldType;
			}
			MetaType.ResolveListTypes(model, memberType2, ref itemType, ref defaultType);
			MemberInfo memberInfo2 = null;
			if (!(memberInfo as PropertyInfo).CanWrite)
			{
				MemberInfo[] member2 = type.GetMember("<{((PropertyInfo)mi).Name}>k__BackingField", (!Helpers.IsEnum(type)) ? (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) : (BindingFlags.Static | BindingFlags.Public));
				if (member2 != null && member2.Length == 1 && member2[0] is FieldInfo)
				{
					memberInfo2 = member2[0];
				}
			}
			ValueMember valueMember = new ValueMember(model, type, fieldNumber, memberInfo2 ?? memberInfo, memberType2, itemType, defaultType, DataFormat.Default, defaultValue);
			if (memberInfo2 != null)
			{
				valueMember.SetName(memberInfo.Name);
			}
			Add(valueMember);
			return valueMember;
		}

		// Token: 0x06003341 RID: 13121 RVA: 0x0012AA00 File Offset: 0x00128E00
		internal static void ResolveListTypes(TypeModel model, Type type, ref Type itemType, ref Type defaultType)
		{
			if (type == null)
			{
				return;
			}
			if (type.IsArray)
			{
				if (type.GetArrayRank() != 1)
				{
					throw new NotSupportedException("Multi-dimensional arrays are not supported");
				}
				itemType = type.GetElementType();
				if (itemType == model.MapType(typeof(byte)))
				{
					Type type2;
					itemType = (type2 = null);
					defaultType = type2;
				}
				else
				{
					defaultType = type;
				}
			}
			if (itemType == null)
			{
				itemType = TypeModel.GetListItemType(model, type);
			}
			if (itemType != null)
			{
				Type type3 = null;
				Type type4 = null;
				MetaType.ResolveListTypes(model, itemType, ref type3, ref type4);
				if (type3 != null)
				{
					throw TypeModel.CreateNestedListsNotSupported();
				}
			}
			if (itemType != null && defaultType == null)
			{
				if (type.IsClass && !type.IsAbstract && Helpers.GetConstructor(type, Helpers.EmptyTypes, true) != null)
				{
					defaultType = type;
				}
				if (defaultType == null && type.IsInterface)
				{
					Type[] genericArguments;
					if (type.IsGenericType && type.GetGenericTypeDefinition() == model.MapType(typeof(IDictionary<, >)) && itemType == model.MapType(typeof(KeyValuePair<, >)).MakeGenericType(genericArguments = type.GetGenericArguments()))
					{
						defaultType = model.MapType(typeof(Dictionary<, >)).MakeGenericType(genericArguments);
					}
					else
					{
						defaultType = model.MapType(typeof(List<>)).MakeGenericType(new Type[]
						{
							itemType
						});
					}
				}
				if (defaultType != null && !Helpers.IsAssignableFrom(type, defaultType))
				{
					defaultType = null;
				}
			}
		}

		// Token: 0x06003342 RID: 13122 RVA: 0x0012AB84 File Offset: 0x00128F84
		private void Add(ValueMember member)
		{
			int opaqueToken = 0;
			try
			{
				model.TakeLock(ref opaqueToken);
				ThrowIfFrozen();
				fields.Add(member);
			}
			finally
			{
				model.ReleaseLock(opaqueToken);
			}
		}

		// Token: 0x170002D0 RID: 720
		public ValueMember this[int fieldNumber]
		{
			get
			{
				foreach (object obj in fields)
				{
					ValueMember valueMember = (ValueMember)obj;
					if (valueMember.FieldNumber == fieldNumber)
					{
						return valueMember;
					}
				}
				return null;
			}
		}

		// Token: 0x170002D1 RID: 721
		public ValueMember this[MemberInfo member]
		{
			get
			{
				if (member == null)
				{
					return null;
				}
				foreach (object obj in fields)
				{
					ValueMember valueMember = (ValueMember)obj;
					if (valueMember.Member == member)
					{
						return valueMember;
					}
				}
				return null;
			}
		}

		// Token: 0x06003345 RID: 13125 RVA: 0x0012AC70 File Offset: 0x00129070
		public ValueMember[] GetFields()
		{
			ValueMember[] array = new ValueMember[fields.Count];
			fields.CopyTo(array, 0);
			Array.Sort<ValueMember>(array, ValueMember.Comparer.Default);
			return array;
		}

		// Token: 0x06003346 RID: 13126 RVA: 0x0012ACA8 File Offset: 0x001290A8
		public SubType[] GetSubtypes()
		{
			if (subTypes == null || subTypes.Count == 0)
			{
				return new SubType[0];
			}
			SubType[] array = new SubType[subTypes.Count];
			subTypes.CopyTo(array, 0);
			Array.Sort<SubType>(array, SubType.Comparer.Default);
			return array;
		}

		// Token: 0x06003347 RID: 13127 RVA: 0x0012AD04 File Offset: 0x00129104
		internal bool IsDefined(int fieldNumber)
		{
			foreach (object obj in fields)
			{
				ValueMember valueMember = (ValueMember)obj;
				if (valueMember.FieldNumber == fieldNumber)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003348 RID: 13128 RVA: 0x0012AD4A File Offset: 0x0012914A
		internal int GetKey(bool demand, bool getBaseKey)
		{
			return model.GetKey(type, demand, getBaseKey);
		}

		// Token: 0x06003349 RID: 13129 RVA: 0x0012AD60 File Offset: 0x00129160
		internal EnumSerializer.EnumPair[] GetEnumMap()
		{
			if (HasFlag(2))
			{
				return null;
			}
			EnumSerializer.EnumPair[] array = new EnumSerializer.EnumPair[fields.Count];
			for (int i = 0; i < array.Length; i++)
			{
				ValueMember valueMember = (ValueMember)fields[i];
				int fieldNumber = valueMember.FieldNumber;
				object rawEnumValue = valueMember.GetRawEnumValue();
				array[i] = new EnumSerializer.EnumPair(fieldNumber, rawEnumValue, valueMember.MemberType);
			}
			return array;
		}

		// Token: 0x170002D2 RID: 722
		// (get) Token: 0x0600334A RID: 13130 RVA: 0x0012ADDC File Offset: 0x001291DC
		// (set) Token: 0x0600334B RID: 13131 RVA: 0x0012ADE5 File Offset: 0x001291E5
		public bool EnumPassthru
        {
            get => HasFlag(2);
            set => SetFlag(2, value, true);
        }

        // Token: 0x170002D3 RID: 723
        // (get) Token: 0x0600334C RID: 13132 RVA: 0x0012ADF0 File Offset: 0x001291F0
        // (set) Token: 0x0600334D RID: 13133 RVA: 0x0012ADFD File Offset: 0x001291FD
        public bool IgnoreListHandling
        {
            get => HasFlag(128);
            set => SetFlag(128, value, true);
        }

        // Token: 0x170002D4 RID: 724
        // (get) Token: 0x0600334E RID: 13134 RVA: 0x0012AE0C File Offset: 0x0012920C
        // (set) Token: 0x0600334F RID: 13135 RVA: 0x0012AE15 File Offset: 0x00129215
        internal bool Pending
        {
            get => HasFlag(1);
            set => SetFlag(1, value, false);
        }

        // Token: 0x06003350 RID: 13136 RVA: 0x0012AE20 File Offset: 0x00129220
        private bool HasFlag(byte flag)
		{
			return (flags & flag) == flag;
		}

		// Token: 0x06003351 RID: 13137 RVA: 0x0012AE30 File Offset: 0x00129230
		private void SetFlag(byte flag, bool value, bool throwIfFrozen)
		{
			if (throwIfFrozen && HasFlag(flag) != value)
			{
				ThrowIfFrozen();
			}
			if (value)
				flags |= flag;
			else
				flags = (byte)(flags & ~flag);
		}

		// Token: 0x06003352 RID: 13138 RVA: 0x0012AE88 File Offset: 0x00129288
		internal static MetaType GetRootType(MetaType source)
		{
			while (source.serializer != null)
			{
				MetaType metaType = source.baseType;
				if (metaType == null)
				{
					return source;
				}
				source = metaType;
			}
			RuntimeTypeModel runtimeTypeModel = source.model;
			int opaqueToken = 0;
			MetaType result;
			try
			{
				runtimeTypeModel.TakeLock(ref opaqueToken);
				MetaType metaType2;
				while ((metaType2 = source.baseType) != null)
				{
					source = metaType2;
				}
				result = source;
			}
			finally
			{
				runtimeTypeModel.ReleaseLock(opaqueToken);
			}
			return result;
		}

		// Token: 0x06003353 RID: 13139 RVA: 0x0012AF00 File Offset: 0x00129300
		internal bool IsPrepared()
		{
			return false;
		}

        // Token: 0x170002D5 RID: 725
        // (get) Token: 0x06003354 RID: 13140 RVA: 0x0012AF03 File Offset: 0x00129303
        internal IEnumerable Fields => fields;

        // Token: 0x06003355 RID: 13141 RVA: 0x0012AF0B File Offset: 0x0012930B
        internal static StringBuilder NewLine(StringBuilder builder, int indent)
		{
			return Helpers.AppendLine(builder).Append(' ', indent * 3);
		}

        // Token: 0x170002D6 RID: 726
        // (get) Token: 0x06003356 RID: 13142 RVA: 0x0012AF1D File Offset: 0x0012931D
        internal bool IsAutoTuple => HasFlag(64);

        // Token: 0x06003357 RID: 13143 RVA: 0x0012AF28 File Offset: 0x00129328
        internal void WriteSchema(StringBuilder builder, int indent, ref bool requiresBclImport)
		{
			if (surrogate != null)
			{
				return;
			}
			ValueMember[] array = new ValueMember[fields.Count];
			fields.CopyTo(array, 0);
			Array.Sort<ValueMember>(array, ValueMember.Comparer.Default);
			if (IsList)
			{
				string schemaTypeName = model.GetSchemaTypeName(TypeModel.GetListItemType(model, type), DataFormat.Default, false, false, ref requiresBclImport);
				MetaType.NewLine(builder, indent).Append("message ").Append(GetSchemaTypeName()).Append(" {");
				MetaType.NewLine(builder, indent + 1).Append("repeated ").Append(schemaTypeName).Append(" items = 1;");
				MetaType.NewLine(builder, indent).Append('}');
			}
			else if (IsAutoTuple)
			{
                if (MetaType.ResolveTupleConstructor(type, out MemberInfo[] array2) != null)
                {
                    MetaType.NewLine(builder, indent).Append("message ").Append(GetSchemaTypeName()).Append(" {");
                    for (int i = 0; i < array2.Length; i++)
                    {
                        Type effectiveType;
                        if (array2[i] is PropertyInfo)
                        {
                            effectiveType = ((PropertyInfo)array2[i]).PropertyType;
                        }
                        else
                        {
                            if (!(array2[i] is FieldInfo))
                            {
                                throw new NotSupportedException("Unknown member type: " + array2[i].GetType().Name);
                            }
                            effectiveType = ((FieldInfo)array2[i]).FieldType;
                        }
                        MetaType.NewLine(builder, indent + 1).Append("optional ").Append(model.GetSchemaTypeName(effectiveType, DataFormat.Default, false, false, ref requiresBclImport).Replace('.', '_')).Append(' ').Append(array2[i].Name).Append(" = ").Append(i + 1).Append(';');
                    }
                    MetaType.NewLine(builder, indent).Append('}');
                }
            }
			else if (Helpers.IsEnum(type))
			{
				MetaType.NewLine(builder, indent).Append("enum ").Append(GetSchemaTypeName()).Append(" {");
				if (array.Length == 0 && EnumPassthru)
				{
					if (type.IsDefined(model.MapType(typeof(FlagsAttribute)), false))
					{
						MetaType.NewLine(builder, indent + 1).Append("// this is a composite/flags enumeration");
					}
					else
					{
						MetaType.NewLine(builder, indent + 1).Append("// this enumeration will be passed as a raw value");
					}
					foreach (FieldInfo fieldInfo in type.GetFields())
					{
						if (fieldInfo.IsStatic && fieldInfo.IsLiteral)
						{
							object rawConstantValue = fieldInfo.GetRawConstantValue();
							MetaType.NewLine(builder, indent + 1).Append(fieldInfo.Name).Append(" = ").Append(rawConstantValue).Append(";");
						}
					}
				}
				else
				{
					foreach (ValueMember valueMember in array)
					{
						MetaType.NewLine(builder, indent + 1).Append(valueMember.Name).Append(" = ").Append(valueMember.FieldNumber).Append(';');
					}
				}
				MetaType.NewLine(builder, indent).Append('}');
			}
			else
			{
				MetaType.NewLine(builder, indent).Append("message ").Append(GetSchemaTypeName()).Append(" {");
				foreach (ValueMember valueMember2 in array)
				{
					string value = (valueMember2.ItemType == null) ? ((!valueMember2.IsRequired) ? "optional" : "required") : "repeated";
					MetaType.NewLine(builder, indent + 1).Append(value).Append(' ');
					if (valueMember2.DataFormat == DataFormat.Group)
					{
						builder.Append("group ");
					}
					string schemaTypeName2 = valueMember2.GetSchemaTypeName(true, ref requiresBclImport);
					builder.Append(schemaTypeName2).Append(" ").Append(valueMember2.Name).Append(" = ").Append(valueMember2.FieldNumber);
					if (valueMember2.DefaultValue != null && !valueMember2.IsRequired)
					{
						if (valueMember2.DefaultValue is string)
						{
							builder.Append(" [default = \"").Append(valueMember2.DefaultValue).Append("\"]");
						}
						else if (valueMember2.DefaultValue is bool)
						{
							builder.Append((!(bool)valueMember2.DefaultValue) ? " [default = false]" : " [default = true]");
						}
						else
						{
							builder.Append(" [default = ").Append(valueMember2.DefaultValue).Append(']');
						}
					}
					if (valueMember2.ItemType != null && valueMember2.IsPacked)
					{
						builder.Append(" [packed=true]");
					}
					builder.Append(';');
					if (schemaTypeName2 == "bcl.NetObjectProxy" && valueMember2.AsReference && !valueMember2.DynamicType)
					{
						builder.Append(" // reference-tracked ").Append(valueMember2.GetSchemaTypeName(false, ref requiresBclImport));
					}
				}
				if (subTypes != null && subTypes.Count != 0)
				{
					MetaType.NewLine(builder, indent + 1).Append("// the following represent sub-types; at most 1 should have a value");
					SubType[] array6 = new SubType[subTypes.Count];
					subTypes.CopyTo(array6, 0);
					Array.Sort<SubType>(array6, SubType.Comparer.Default);
					foreach (SubType subType in array6)
					{
						string schemaTypeName3 = subType.DerivedType.GetSchemaTypeName();
						MetaType.NewLine(builder, indent + 1).Append("optional ").Append(schemaTypeName3).Append(" ").Append(schemaTypeName3).Append(" = ").Append(subType.FieldNumber).Append(';');
					}
				}
				MetaType.NewLine(builder, indent).Append('}');
			}
		}

		// Token: 0x040023F1 RID: 9201
		private MetaType baseType;

		// Token: 0x040023F2 RID: 9202
		private BasicList subTypes;

		// Token: 0x040023F3 RID: 9203
		internal static readonly Type ienumerable = typeof(IEnumerable);

		// Token: 0x040023F4 RID: 9204
		private CallbackSet callbacks;

		// Token: 0x040023F5 RID: 9205
		private string name;

		// Token: 0x040023F6 RID: 9206
		private MethodInfo factory;

		// Token: 0x040023F7 RID: 9207
		private readonly RuntimeTypeModel model;

		// Token: 0x040023F8 RID: 9208
		private readonly Type type;

		// Token: 0x040023F9 RID: 9209
		private IProtoTypeSerializer serializer;

		// Token: 0x040023FA RID: 9210
		private Type constructType;

		// Token: 0x040023FB RID: 9211
		private Type surrogate;

		// Token: 0x040023FC RID: 9212
		private readonly BasicList fields = new BasicList();

		// Token: 0x040023FD RID: 9213
		private const byte OPTIONS_Pending = 1;

		// Token: 0x040023FE RID: 9214
		private const byte OPTIONS_EnumPassThru = 2;

		// Token: 0x040023FF RID: 9215
		private const byte OPTIONS_Frozen = 4;

		// Token: 0x04002400 RID: 9216
		private const byte OPTIONS_PrivateOnApi = 8;

		// Token: 0x04002401 RID: 9217
		private const byte OPTIONS_SkipConstructor = 16;

		// Token: 0x04002402 RID: 9218
		private const byte OPTIONS_AsReferenceDefault = 32;

		// Token: 0x04002403 RID: 9219
		private const byte OPTIONS_AutoTuple = 64;

		// Token: 0x04002404 RID: 9220
		private const byte OPTIONS_IgnoreListHandling = 128;

		// Token: 0x04002405 RID: 9221
		private volatile byte flags;

		// Token: 0x020003DA RID: 986
		internal sealed class Comparer : IComparer, IComparer<MetaType>
		{
			// Token: 0x0600335A RID: 13146 RVA: 0x0012B5B3 File Offset: 0x001299B3
			public int Compare(object x, object y)
			{
				return Compare(x as MetaType, y as MetaType);
			}

			// Token: 0x0600335B RID: 13147 RVA: 0x0012B5C7 File Offset: 0x001299C7
			public int Compare(MetaType x, MetaType y)
			{
				if (object.ReferenceEquals(x, y))
				{
					return 0;
				}
				if (x == null)
				{
					return -1;
				}
				if (y == null)
				{
					return 1;
				}
				return string.Compare(x.GetSchemaTypeName(), y.GetSchemaTypeName(), StringComparison.Ordinal);
			}

			// Token: 0x04002406 RID: 9222
			public static readonly MetaType.Comparer Default = new MetaType.Comparer();
		}

		// Token: 0x020003DB RID: 987
		[Flags]
		internal enum AttributeFamily
		{
			// Token: 0x04002408 RID: 9224
			None = 0,
			// Token: 0x04002409 RID: 9225
			ProtoBuf = 1,
			// Token: 0x0400240A RID: 9226
			DataContractSerialier = 2,
			// Token: 0x0400240B RID: 9227
			XmlSerializer = 4,
			// Token: 0x0400240C RID: 9228
			AutoTuple = 8
		}
	}
}
