using ProtoBuf.Serializers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace ProtoBuf.Meta
{
    // Token: 0x020003E9 RID: 1001
    public class ValueMember
	{
		// Token: 0x060033F1 RID: 13297 RVA: 0x0012E7E8 File Offset: 0x0012CBE8
		public ValueMember(RuntimeTypeModel model, Type parentType, int fieldNumber, MemberInfo member, Type memberType, Type itemType, Type defaultType, DataFormat dataFormat, object defaultValue) : this(model, fieldNumber, memberType, itemType, defaultType, dataFormat)
		{
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			if (parentType == null)
			{
				throw new ArgumentNullException("parentType");
			}
			if (fieldNumber < 1 && !Helpers.IsEnum(parentType))
			{
				throw new ArgumentOutOfRangeException("fieldNumber");
			}
			this.member = member;
			this.parentType = parentType;
			if (fieldNumber < 1 && !Helpers.IsEnum(parentType))
			{
				throw new ArgumentOutOfRangeException("fieldNumber");
			}
			if (defaultValue != null && model.MapType(defaultValue.GetType()) != memberType)
			{
				defaultValue = ValueMember.ParseDefaultValue(memberType, defaultValue);
			}
			this.defaultValue = defaultValue;
			MetaType metaType = model.FindWithoutAdd(memberType);
			if (metaType != null)
			{
				asReference = metaType.AsReferenceDefault;
			}
			else
			{
				asReference = MetaType.GetAsReferenceDefault(model, memberType);
			}
		}

		// Token: 0x060033F2 RID: 13298 RVA: 0x0012E8D0 File Offset: 0x0012CCD0
		internal ValueMember(RuntimeTypeModel model, int fieldNumber, Type memberType, Type itemType, Type defaultType, DataFormat dataFormat)
		{
			if (memberType == null)
			{
				throw new ArgumentNullException("memberType");
			}
			if (model == null)
			{
				throw new ArgumentNullException("model");
			}
			this.fieldNumber = fieldNumber;
			this.memberType = memberType;
			this.itemType = itemType;
			this.defaultType = defaultType;
			this.model = model;
			this.dataFormat = dataFormat;
		}

        // Token: 0x170002EA RID: 746
        // (get) Token: 0x060033F3 RID: 13299 RVA: 0x0012E932 File Offset: 0x0012CD32
        public int FieldNumber => fieldNumber;

        // Token: 0x170002EB RID: 747
        // (get) Token: 0x060033F4 RID: 13300 RVA: 0x0012E93A File Offset: 0x0012CD3A
        public MemberInfo Member => member;

        // Token: 0x170002EC RID: 748
        // (get) Token: 0x060033F5 RID: 13301 RVA: 0x0012E942 File Offset: 0x0012CD42
        public Type ItemType => itemType;

        // Token: 0x170002ED RID: 749
        // (get) Token: 0x060033F6 RID: 13302 RVA: 0x0012E94A File Offset: 0x0012CD4A
        public Type MemberType => memberType;

        // Token: 0x170002EE RID: 750
        // (get) Token: 0x060033F7 RID: 13303 RVA: 0x0012E952 File Offset: 0x0012CD52
        public Type DefaultType => defaultType;

        // Token: 0x170002EF RID: 751
        // (get) Token: 0x060033F8 RID: 13304 RVA: 0x0012E95A File Offset: 0x0012CD5A
        public Type ParentType => parentType;

        // Token: 0x170002F0 RID: 752
        // (get) Token: 0x060033F9 RID: 13305 RVA: 0x0012E962 File Offset: 0x0012CD62
        // (set) Token: 0x060033FA RID: 13306 RVA: 0x0012E96A File Offset: 0x0012CD6A
        public object DefaultValue
        {
            get => defaultValue;
            set
            {
                ThrowIfFrozen();
                defaultValue = value;
            }
        }

        // Token: 0x060033FB RID: 13307 RVA: 0x0012E979 File Offset: 0x0012CD79
        internal object GetRawEnumValue()
		{
			return ((FieldInfo)member).GetRawConstantValue();
		}

		// Token: 0x060033FC RID: 13308 RVA: 0x0012E98C File Offset: 0x0012CD8C
		private static object ParseDefaultValue(Type type, object value)
		{
			Type underlyingType = Helpers.GetUnderlyingType(type);
			if (underlyingType != null)
			{
				type = underlyingType;
			}
			if (value is string)
			{
				string text = (string)value;
				if (Helpers.IsEnum(type))
				{
					return Helpers.ParseEnum(type, text);
				}
				ProtoTypeCode typeCode = Helpers.GetTypeCode(type);
				switch (typeCode)
				{
				case ProtoTypeCode.Boolean:
					return bool.Parse(text);
				case ProtoTypeCode.Char:
					if (text.Length == 1)
					{
						return text[0];
					}
					throw new FormatException("Single character expected: \"" + text + "\"");
				case ProtoTypeCode.SByte:
					return sbyte.Parse(text, NumberStyles.Integer, CultureInfo.InvariantCulture);
				case ProtoTypeCode.Byte:
					return byte.Parse(text, NumberStyles.Integer, CultureInfo.InvariantCulture);
				case ProtoTypeCode.Int16:
					return short.Parse(text, NumberStyles.Any, CultureInfo.InvariantCulture);
				case ProtoTypeCode.UInt16:
					return ushort.Parse(text, NumberStyles.Any, CultureInfo.InvariantCulture);
				case ProtoTypeCode.Int32:
					return int.Parse(text, NumberStyles.Any, CultureInfo.InvariantCulture);
				case ProtoTypeCode.UInt32:
					return uint.Parse(text, NumberStyles.Any, CultureInfo.InvariantCulture);
				case ProtoTypeCode.Int64:
					return long.Parse(text, NumberStyles.Any, CultureInfo.InvariantCulture);
				case ProtoTypeCode.UInt64:
					return ulong.Parse(text, NumberStyles.Any, CultureInfo.InvariantCulture);
				case ProtoTypeCode.Single:
					return float.Parse(text, NumberStyles.Any, CultureInfo.InvariantCulture);
				case ProtoTypeCode.Double:
					return double.Parse(text, NumberStyles.Any, CultureInfo.InvariantCulture);
				case ProtoTypeCode.Decimal:
					return decimal.Parse(text, NumberStyles.Any, CultureInfo.InvariantCulture);
				case ProtoTypeCode.DateTime:
					return DateTime.Parse(text, CultureInfo.InvariantCulture);
				default:
					switch (typeCode)
					{
					case ProtoTypeCode.TimeSpan:
						return TimeSpan.Parse(text);
					case ProtoTypeCode.Guid:
						return new Guid(text);
					case ProtoTypeCode.Uri:
						return text;
					}
					break;
				case ProtoTypeCode.String:
					return text;
				}
			}
			if (Helpers.IsEnum(type))
			{
				return Enum.ToObject(type, value);
			}
			return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
		}

		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x060033FD RID: 13309 RVA: 0x0012EBAC File Offset: 0x0012CFAC
		internal IProtoSerializer Serializer
		{
			get
			{
				if (serializer == null)
				{
					serializer = BuildSerializer();
				}
				return serializer;
			}
		}

		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x060033FE RID: 13310 RVA: 0x0012EBCB File Offset: 0x0012CFCB
		// (set) Token: 0x060033FF RID: 13311 RVA: 0x0012EBD3 File Offset: 0x0012CFD3
		public DataFormat DataFormat
        {
            get => dataFormat;
            set
            {
                ThrowIfFrozen();
                dataFormat = value;
            }
        }

        // Token: 0x170002F3 RID: 755
        // (get) Token: 0x06003400 RID: 13312 RVA: 0x0012EBE2 File Offset: 0x0012CFE2
        // (set) Token: 0x06003401 RID: 13313 RVA: 0x0012EBEB File Offset: 0x0012CFEB
        public bool IsStrict
        {
            get => HasFlag(1);
            set => SetFlag(1, value, true);
        }

        // Token: 0x170002F4 RID: 756
        // (get) Token: 0x06003402 RID: 13314 RVA: 0x0012EBF6 File Offset: 0x0012CFF6
        // (set) Token: 0x06003403 RID: 13315 RVA: 0x0012EBFF File Offset: 0x0012CFFF
        public bool IsPacked
        {
            get => HasFlag(2);
            set => SetFlag(2, value, true);
        }

        // Token: 0x170002F5 RID: 757
        // (get) Token: 0x06003404 RID: 13316 RVA: 0x0012EC0A File Offset: 0x0012D00A
        // (set) Token: 0x06003405 RID: 13317 RVA: 0x0012EC13 File Offset: 0x0012D013
        public bool OverwriteList
        {
            get => HasFlag(8);
            set => SetFlag(8, value, true);
        }

        // Token: 0x170002F6 RID: 758
        // (get) Token: 0x06003406 RID: 13318 RVA: 0x0012EC1E File Offset: 0x0012D01E
        // (set) Token: 0x06003407 RID: 13319 RVA: 0x0012EC27 File Offset: 0x0012D027
        public bool IsRequired
        {
            get => HasFlag(4);
            set => SetFlag(4, value, true);
        }

        // Token: 0x170002F7 RID: 759
        // (get) Token: 0x06003408 RID: 13320 RVA: 0x0012EC32 File Offset: 0x0012D032
        // (set) Token: 0x06003409 RID: 13321 RVA: 0x0012EC3A File Offset: 0x0012D03A
        public bool AsReference
        {
            get => asReference;
            set
            {
                ThrowIfFrozen();
                asReference = value;
            }
        }

        // Token: 0x170002F8 RID: 760
        // (get) Token: 0x0600340A RID: 13322 RVA: 0x0012EC49 File Offset: 0x0012D049
        // (set) Token: 0x0600340B RID: 13323 RVA: 0x0012EC51 File Offset: 0x0012D051
        public bool DynamicType
        {
            get => dynamicType;
            set
            {
                ThrowIfFrozen();
                dynamicType = value;
            }
        }

        // Token: 0x0600340C RID: 13324 RVA: 0x0012EC60 File Offset: 0x0012D060
        public void SetSpecified(MethodInfo getSpecified, MethodInfo setSpecified)
		{
			if (getSpecified != null && (getSpecified.ReturnType != model.MapType(typeof(bool)) || getSpecified.IsStatic || getSpecified.GetParameters().Length != 0))
			{
				throw new ArgumentException("Invalid pattern for checking member-specified", "getSpecified");
			}
			ParameterInfo[] parameters;
			if (setSpecified != null && (setSpecified.ReturnType != model.MapType(typeof(void)) || setSpecified.IsStatic || (parameters = setSpecified.GetParameters()).Length != 1 || parameters[0].ParameterType != model.MapType(typeof(bool))))
			{
				throw new ArgumentException("Invalid pattern for setting member-specified", "setSpecified");
			}
			ThrowIfFrozen();
			this.getSpecified = getSpecified;
			this.setSpecified = setSpecified;
		}

		// Token: 0x0600340D RID: 13325 RVA: 0x0012ED42 File Offset: 0x0012D142
		private void ThrowIfFrozen()
		{
			if (serializer != null)
			{
				throw new InvalidOperationException("The type cannot be changed once a serializer has been generated");
			}
		}

		// Token: 0x0600340E RID: 13326 RVA: 0x0012ED5C File Offset: 0x0012D15C
		private IProtoSerializer BuildSerializer()
		{
			int opaqueToken = 0;
			IProtoSerializer result;
			try
			{
				model.TakeLock(ref opaqueToken);
				Type type = (itemType != null) ? itemType : memberType;
                IProtoSerializer protoSerializer = ValueMember.TryGetCoreSerializer(model, dataFormat, type, out WireType wireType, asReference, dynamicType, OverwriteList, true);
                if (protoSerializer == null)
				{
					throw new InvalidOperationException("No serializer defined for type: " + type.FullName);
				}
				if (itemType != null && SupportNull)
				{
					if (IsPacked)
					{
						throw new NotSupportedException("Packed encodings cannot support null values");
					}
					protoSerializer = new TagDecorator(1, wireType, IsStrict, protoSerializer);
					protoSerializer = new NullDecorator(model, protoSerializer);
					protoSerializer = new TagDecorator(fieldNumber, WireType.StartGroup, false, protoSerializer);
				}
				else
				{
					protoSerializer = new TagDecorator(fieldNumber, wireType, IsStrict, protoSerializer);
				}
				if (itemType != null)
				{
					Type type2 = (!SupportNull) ? (Helpers.GetUnderlyingType(itemType) ?? itemType) : itemType;
					if (memberType.IsArray)
					{
						protoSerializer = new ArrayDecorator(model, protoSerializer, fieldNumber, IsPacked, wireType, memberType, OverwriteList, SupportNull);
					}
					else
					{
						protoSerializer = ListDecorator.Create(model, memberType, defaultType, protoSerializer, fieldNumber, IsPacked, wireType, member != null && PropertyDecorator.CanWrite(model, member), OverwriteList, SupportNull);
					}
				}
				else if (defaultValue != null && !IsRequired && getSpecified == null)
				{
					protoSerializer = new DefaultValueDecorator(model, defaultValue, protoSerializer);
				}
				if (memberType == model.MapType(typeof(Uri)))
				{
					protoSerializer = new UriDecorator(model, protoSerializer);
				}
				if (member != null)
				{
					PropertyInfo propertyInfo = member as PropertyInfo;
					if (propertyInfo != null)
					{
						protoSerializer = new PropertyDecorator(model, parentType, (PropertyInfo)member, protoSerializer);
					}
					else
					{
						FieldInfo fieldInfo = member as FieldInfo;
						if (fieldInfo == null)
						{
							throw new InvalidOperationException();
						}
						protoSerializer = new FieldDecorator(parentType, (FieldInfo)member, protoSerializer);
					}
					if (getSpecified != null || setSpecified != null)
					{
						protoSerializer = new MemberSpecifiedDecorator(getSpecified, setSpecified, protoSerializer);
					}
				}
				result = protoSerializer;
			}
			finally
			{
				model.ReleaseLock(opaqueToken);
			}
			return result;
		}

		// Token: 0x0600340F RID: 13327 RVA: 0x0012F054 File Offset: 0x0012D454
		private static WireType GetIntWireType(DataFormat format, int width)
		{
			switch (format)
			{
			case DataFormat.Default:
			case DataFormat.TwosComplement:
				return WireType.Variant;
			case DataFormat.ZigZag:
				return WireType.SignedVariant;
			case DataFormat.FixedSize:
				return (width != 32) ? WireType.Fixed64 : WireType.Fixed32;
			default:
				throw new InvalidOperationException();
			}
		}

		// Token: 0x06003410 RID: 13328 RVA: 0x0012F08A File Offset: 0x0012D48A
		private static WireType GetDateTimeWireType(DataFormat format)
		{
			switch (format)
			{
			case DataFormat.Default:
				return WireType.String;
			case DataFormat.FixedSize:
				return WireType.Fixed64;
			case DataFormat.Group:
				return WireType.StartGroup;
			}
			throw new InvalidOperationException();
		}

		// Token: 0x06003411 RID: 13329 RVA: 0x0012F0B8 File Offset: 0x0012D4B8
		internal static IProtoSerializer TryGetCoreSerializer(RuntimeTypeModel model, DataFormat dataFormat, Type type, out WireType defaultWireType, bool asReference, bool dynamicType, bool overwriteList, bool allowComplexTypes)
		{
			Type underlyingType = Helpers.GetUnderlyingType(type);
			if (underlyingType != null)
			{
				type = underlyingType;
			}
			if (Helpers.IsEnum(type))
			{
				if (allowComplexTypes && model != null)
				{
					defaultWireType = WireType.Variant;
					return new EnumSerializer(type, model.GetEnumMap(type));
				}
				defaultWireType = WireType.None;
				return null;
			}
			else
			{
				ProtoTypeCode typeCode = Helpers.GetTypeCode(type);
				switch (typeCode)
				{
				case ProtoTypeCode.Boolean:
					defaultWireType = WireType.Variant;
					return new BooleanSerializer(model);
				case ProtoTypeCode.Char:
					defaultWireType = WireType.Variant;
					return new CharSerializer(model);
				case ProtoTypeCode.SByte:
					defaultWireType = ValueMember.GetIntWireType(dataFormat, 32);
					return new SByteSerializer(model);
				case ProtoTypeCode.Byte:
					defaultWireType = ValueMember.GetIntWireType(dataFormat, 32);
					return new ByteSerializer(model);
				case ProtoTypeCode.Int16:
					defaultWireType = ValueMember.GetIntWireType(dataFormat, 32);
					return new Int16Serializer(model);
				case ProtoTypeCode.UInt16:
					defaultWireType = ValueMember.GetIntWireType(dataFormat, 32);
					return new UInt16Serializer(model);
				case ProtoTypeCode.Int32:
					defaultWireType = ValueMember.GetIntWireType(dataFormat, 32);
					return new Int32Serializer(model);
				case ProtoTypeCode.UInt32:
					defaultWireType = ValueMember.GetIntWireType(dataFormat, 32);
					return new UInt32Serializer(model);
				case ProtoTypeCode.Int64:
					defaultWireType = ValueMember.GetIntWireType(dataFormat, 64);
					return new Int64Serializer(model);
				case ProtoTypeCode.UInt64:
					defaultWireType = ValueMember.GetIntWireType(dataFormat, 64);
					return new UInt64Serializer(model);
				case ProtoTypeCode.Single:
					defaultWireType = WireType.Fixed32;
					return new SingleSerializer(model);
				case ProtoTypeCode.Double:
					defaultWireType = WireType.Fixed64;
					return new DoubleSerializer(model);
				case ProtoTypeCode.Decimal:
					defaultWireType = WireType.String;
					return new DecimalSerializer(model);
				case ProtoTypeCode.DateTime:
					defaultWireType = ValueMember.GetDateTimeWireType(dataFormat);
					return new DateTimeSerializer(model);
				default:
					switch (typeCode)
					{
					case ProtoTypeCode.TimeSpan:
						defaultWireType = ValueMember.GetDateTimeWireType(dataFormat);
						return new TimeSpanSerializer(model);
					case ProtoTypeCode.ByteArray:
						defaultWireType = WireType.String;
						return new BlobSerializer(model, overwriteList);
					case ProtoTypeCode.Guid:
						defaultWireType = WireType.String;
						return new GuidSerializer(model);
					case ProtoTypeCode.Uri:
						defaultWireType = WireType.String;
						return new StringSerializer(model);
					case ProtoTypeCode.Type:
						defaultWireType = WireType.String;
						return new SystemTypeSerializer(model);
					default:
					{
						IProtoSerializer protoSerializer = (!model.AllowParseableTypes) ? null : ParseableSerializer.TryCreate(type, model);
						if (protoSerializer != null)
						{
							defaultWireType = WireType.String;
							return protoSerializer;
						}
						if (allowComplexTypes && model != null)
						{
							int key = model.GetKey(type, false, true);
							if (asReference || dynamicType)
							{
								defaultWireType = ((dataFormat != DataFormat.Group) ? WireType.String : WireType.StartGroup);
								BclHelpers.NetObjectOptions netObjectOptions = BclHelpers.NetObjectOptions.None;
								if (asReference)
								{
									netObjectOptions |= BclHelpers.NetObjectOptions.AsReference;
								}
								if (dynamicType)
								{
									netObjectOptions |= BclHelpers.NetObjectOptions.DynamicType;
								}
								if (key >= 0)
								{
									if (asReference && Helpers.IsValueType(type))
									{
										string text = "AsReference cannot be used with value-types";
										if (type.Name == "KeyValuePair`2")
										{
											text += "; please see http://stackoverflow.com/q/14436606/";
										}
										else
										{
											text = text + ": " + type.FullName;
										}
										throw new InvalidOperationException(text);
									}
									MetaType metaType = model[type];
									if (asReference && metaType.IsAutoTuple)
									{
										netObjectOptions |= BclHelpers.NetObjectOptions.LateSet;
									}
									if (metaType.UseConstructor)
									{
										netObjectOptions |= BclHelpers.NetObjectOptions.UseConstructor;
									}
								}
								return new NetObjectSerializer(model, type, key, netObjectOptions);
							}
							if (key >= 0)
							{
								defaultWireType = ((dataFormat != DataFormat.Group) ? WireType.String : WireType.StartGroup);
								return new SubItemSerializer(type, key, model[type], true);
							}
						}
						defaultWireType = WireType.None;
						return null;
					}
					}
					break;
				case ProtoTypeCode.String:
					defaultWireType = WireType.String;
					if (asReference)
					{
						return new NetObjectSerializer(model, model.MapType(typeof(string)), 0, BclHelpers.NetObjectOptions.AsReference);
					}
					return new StringSerializer(model);
				}
			}
		}

		// Token: 0x06003412 RID: 13330 RVA: 0x0012F3E8 File Offset: 0x0012D7E8
		internal void SetName(string name)
		{
			ThrowIfFrozen();
			this.name = name;
		}

        // Token: 0x170002F9 RID: 761
        // (get) Token: 0x06003413 RID: 13331 RVA: 0x0012F3F7 File Offset: 0x0012D7F7
        public string Name => (!Helpers.IsNullOrEmpty(name)) ? name : member.Name;

        // Token: 0x06003414 RID: 13332 RVA: 0x0012F41F File Offset: 0x0012D81F
        private bool HasFlag(byte flag)
		{
			return (flags & flag) == flag;
		}

		// Token: 0x06003415 RID: 13333 RVA: 0x0012F42C File Offset: 0x0012D82C
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

		// Token: 0x170002FA RID: 762
		// (get) Token: 0x06003416 RID: 13334 RVA: 0x0012F47C File Offset: 0x0012D87C
		// (set) Token: 0x06003417 RID: 13335 RVA: 0x0012F486 File Offset: 0x0012D886
		public bool SupportNull
        {
            get => HasFlag(16);
            set => SetFlag(16, value, true);
        }

        // Token: 0x06003418 RID: 13336 RVA: 0x0012F494 File Offset: 0x0012D894
        internal string GetSchemaTypeName(bool applyNetObjectProxy, ref bool requiresBclImport)
		{
			Type type = ItemType;
			if (type == null)
			{
				type = MemberType;
			}
			return model.GetSchemaTypeName(type, DataFormat, applyNetObjectProxy && asReference, applyNetObjectProxy && dynamicType, ref requiresBclImport);
		}

		// Token: 0x0400243A RID: 9274
		private readonly int fieldNumber;

		// Token: 0x0400243B RID: 9275
		private readonly MemberInfo member;

		// Token: 0x0400243C RID: 9276
		private readonly Type parentType;

		// Token: 0x0400243D RID: 9277
		private readonly Type itemType;

		// Token: 0x0400243E RID: 9278
		private readonly Type defaultType;

		// Token: 0x0400243F RID: 9279
		private readonly Type memberType;

		// Token: 0x04002440 RID: 9280
		private object defaultValue;

		// Token: 0x04002441 RID: 9281
		private readonly RuntimeTypeModel model;

		// Token: 0x04002442 RID: 9282
		private IProtoSerializer serializer;

		// Token: 0x04002443 RID: 9283
		private DataFormat dataFormat;

		// Token: 0x04002444 RID: 9284
		private bool asReference;

		// Token: 0x04002445 RID: 9285
		private bool dynamicType;

		// Token: 0x04002446 RID: 9286
		private MethodInfo getSpecified;

		// Token: 0x04002447 RID: 9287
		private MethodInfo setSpecified;

		// Token: 0x04002448 RID: 9288
		private string name;

		// Token: 0x04002449 RID: 9289
		private const byte OPTIONS_IsStrict = 1;

		// Token: 0x0400244A RID: 9290
		private const byte OPTIONS_IsPacked = 2;

		// Token: 0x0400244B RID: 9291
		private const byte OPTIONS_IsRequired = 4;

		// Token: 0x0400244C RID: 9292
		private const byte OPTIONS_OverwriteList = 8;

		// Token: 0x0400244D RID: 9293
		private const byte OPTIONS_SupportNull = 16;

		// Token: 0x0400244E RID: 9294
		private byte flags;

		// Token: 0x020003EA RID: 1002
		internal sealed class Comparer : IComparer, IComparer<ValueMember>
		{
			// Token: 0x0600341A RID: 13338 RVA: 0x0012F4EE File Offset: 0x0012D8EE
			public int Compare(object x, object y)
			{
				return Compare(x as ValueMember, y as ValueMember);
			}

			// Token: 0x0600341B RID: 13339 RVA: 0x0012F504 File Offset: 0x0012D904
			public int Compare(ValueMember x, ValueMember y)
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
				return x.FieldNumber.CompareTo(y.FieldNumber);
			}

			// Token: 0x0400244F RID: 9295
			public static readonly ValueMember.Comparer Default = new ValueMember.Comparer();
		}
	}
}
