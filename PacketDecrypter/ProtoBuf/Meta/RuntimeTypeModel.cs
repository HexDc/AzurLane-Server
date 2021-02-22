using ProtoBuf.Serializers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
//패치
//https://github.com/RedpointGames/protobuf-net/blob/master/protobuf-net/Meta/RuntimeTypeModel.cs
namespace ProtoBuf.Meta
{
    // Token: 0x020003DC RID: 988
    public sealed class RuntimeTypeModel : TypeModel
	{
		// Token: 0x0600335D RID: 13149 RVA: 0x0012D1EC File Offset: 0x0012B5EC
		internal RuntimeTypeModel(bool isDefault)
		{
			AutoAddMissingTypes = true;
			UseImplicitZeroDefaults = true;
			SetOption(2, isDefault);
		}

		// Token: 0x0600335E RID: 13150 RVA: 0x0012D23D File Offset: 0x0012B63D
		private bool GetOption(ushort option)
		{
			return (options & option) == option;
		}

		// Token: 0x0600335F RID: 13151 RVA: 0x0012D24A File Offset: 0x0012B64A
		private void SetOption(ushort option, bool value)// 오픈소스에선 byte인데 얜 ushort네?
		//private void SetOption(byte option, bool value)
		{
			if (value) options |= option;
			else options &= (ushort)~option;
			//else options &= (byte)~option;
		}

		// Token: 0x170002D7 RID: 727
		// (get) Token: 0x06003360 RID: 13152 RVA: 0x0012D277 File Offset: 0x0012B677
		// (set) Token: 0x06003361 RID: 13153 RVA: 0x0012D280 File Offset: 0x0012B680
		public bool InferTagFromNameDefault
        {
            get => GetOption(1);
            set => SetOption(1, value);
        }

        // Token: 0x170002D8 RID: 728
        // (get) Token: 0x06003362 RID: 13154 RVA: 0x0012D28A File Offset: 0x0012B68A
        // (set) Token: 0x06003363 RID: 13155 RVA: 0x0012D297 File Offset: 0x0012B697
        public bool AutoAddProtoContractTypesOnly
        {
            get => GetOption(128);
            set => SetOption(128, value);
        }

        // Token: 0x170002D9 RID: 729
        // (get) Token: 0x06003364 RID: 13156 RVA: 0x0012D2A5 File Offset: 0x0012B6A5
        // (set) Token: 0x06003365 RID: 13157 RVA: 0x0012D2AF File Offset: 0x0012B6AF
        public bool UseImplicitZeroDefaults
        {
            get => GetOption(32);
            set
            {
                if (!value && GetOption(2))
                {
                    throw new InvalidOperationException("UseImplicitZeroDefaults cannot be disabled on the default model");
                }
                SetOption(32, value);
            }
        }

        // Token: 0x170002DA RID: 730
        // (get) Token: 0x06003366 RID: 13158 RVA: 0x0012D2D7 File Offset: 0x0012B6D7
        // (set) Token: 0x06003367 RID: 13159 RVA: 0x0012D2E1 File Offset: 0x0012B6E1
        public bool AllowParseableTypes
        {
            get => GetOption(64);
            set => SetOption(64, value);
        }

        // Token: 0x170002DB RID: 731
        // (get) Token: 0x06003368 RID: 13160 RVA: 0x0012D2EC File Offset: 0x0012B6EC
        // (set) Token: 0x06003369 RID: 13161 RVA: 0x0012D2F9 File Offset: 0x0012B6F9
        public bool IncludeDateTimeKind
        {
            get => GetOption(256);
            set => SetOption(256, value);
        }

        // Token: 0x0600336A RID: 13162 RVA: 0x0012D307 File Offset: 0x0012B707
        protected internal override bool SerializeDateTimeKind()
		{
			return GetOption(256);
		}

        // Token: 0x170002DC RID: 732
        // (get) Token: 0x0600336B RID: 13163 RVA: 0x0012D314 File Offset: 0x0012B714
        public static RuntimeTypeModel Default => RuntimeTypeModel.Singleton.Value;

        // Token: 0x0600336C RID: 13164 RVA: 0x0012D31B File Offset: 0x0012B71B
        public IEnumerable GetTypes()
		{
			return types;
		}

		// Token: 0x0600336D RID: 13165 RVA: 0x0012D324 File Offset: 0x0012B724
		public override string GetSchema(Type type)
		{
			BasicList basicList = new BasicList();
			MetaType metaType = null;
			bool flag = false;
			if (type == null)
			{
				foreach (object obj in types)
				{
					MetaType metaType2 = (MetaType)obj;
					MetaType surrogateOrBaseOrSelf = metaType2.GetSurrogateOrBaseOrSelf(false);
					if (!basicList.Contains(surrogateOrBaseOrSelf))
					{
						basicList.Add(surrogateOrBaseOrSelf);
						CascadeDependents(basicList, surrogateOrBaseOrSelf);
					}
				}
			}
			else
			{
				Type underlyingType = Helpers.GetUnderlyingType(type);
				if (underlyingType != null)
				{
					type = underlyingType;
				}
                flag = (ValueMember.TryGetCoreSerializer(this, DataFormat.Default, type, out WireType wireType, false, false, false, false) != null);
                if (!flag)
				{
					int num = FindOrAddAuto(type, false, false, false);
					if (num < 0)
					{
						throw new ArgumentException("The type specified is not a contract-type", "type");
					}
					metaType = ((MetaType)types[num]).GetSurrogateOrBaseOrSelf(false);
					basicList.Add(metaType);
					CascadeDependents(basicList, metaType);
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			string text = null;
			if (!flag)
			{
				IEnumerable enumerable = (metaType != null) ? basicList : types;
				IEnumerator enumerator2 = enumerable.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						object obj2 = enumerator2.Current;
						MetaType metaType3 = (MetaType)obj2;
						if (!metaType3.IsList)
						{
							string @namespace = metaType3.Type.Namespace;
							if (!Helpers.IsNullOrEmpty(@namespace))
							{
								if (!@namespace.StartsWith("System."))
								{
									if (text == null)
									{
										text = @namespace;
									}
									else if (!(text == @namespace))
									{
										text = null;
										break;
									}
								}
							}
						}
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator2 as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
			}
			if (!Helpers.IsNullOrEmpty(text))
			{
				stringBuilder.Append("package ").Append(text).Append(';');
				Helpers.AppendLine(stringBuilder);
			}
			bool flag2 = false;
			StringBuilder stringBuilder2 = new StringBuilder();
			MetaType[] array = new MetaType[basicList.Count];
			basicList.CopyTo(array, 0);
			Array.Sort<MetaType>(array, MetaType.Comparer.Default);
			if (flag)
			{
				Helpers.AppendLine(stringBuilder2).Append("message ").Append(type.Name).Append(" {");
				MetaType.NewLine(stringBuilder2, 1).Append("optional ").Append(GetSchemaTypeName(type, DataFormat.Default, false, false, ref flag2)).Append(" value = 1;");
				Helpers.AppendLine(stringBuilder2).Append('}');
			}
			else
			{
				foreach (MetaType metaType4 in array)
				{
					if (!metaType4.IsList || metaType4 == metaType)
					{
						metaType4.WriteSchema(stringBuilder2, 0, ref flag2);
					}
				}
			}
			if (flag2)
			{
				stringBuilder.Append("import \"bcl.proto\"; // schema for protobuf-net's handling of core .NET types");
				Helpers.AppendLine(stringBuilder);
			}
			return Helpers.AppendLine(stringBuilder.Append(stringBuilder2)).ToString();
		}

		// Token: 0x0600336E RID: 13166 RVA: 0x0012D638 File Offset: 0x0012BA38
		private void CascadeDependents(BasicList list, MetaType metaType)
		{
			if (metaType.IsList)
			{
				Type listItemType = TypeModel.GetListItemType(this, metaType.Type);
                if (ValueMember.TryGetCoreSerializer(this, DataFormat.Default, listItemType, out WireType wireType, false, false, false, false) == null)
                {
                    int num = FindOrAddAuto(listItemType, false, false, false);
                    if (num >= 0)
                    {
                        MetaType metaType2 = ((MetaType)types[num]).GetSurrogateOrBaseOrSelf(false);
                        if (!list.Contains(metaType2))
                        {
                            list.Add(metaType2);
                            CascadeDependents(list, metaType2);
                        }
                    }
                }
            }
			else
			{
				MetaType metaType2;
				if (metaType.IsAutoTuple)
				{
                    if (MetaType.ResolveTupleConstructor(metaType.Type, out MemberInfo[] array) != null)
                    {
                        for (int i = 0; i < array.Length; i++)
                        {
                            Type type = null;
                            if (array[i] is PropertyInfo)
                            {
                                type = ((PropertyInfo)array[i]).PropertyType;
                            }
                            else if (array[i] is FieldInfo)
                            {
                                type = ((FieldInfo)array[i]).FieldType;
                            }
                            if (ValueMember.TryGetCoreSerializer(this, DataFormat.Default, type, out WireType wireType2, false, false, false, false) == null)
                            {
                                int num2 = FindOrAddAuto(type, false, false, false);
                                if (num2 >= 0)
                                {
                                    metaType2 = ((MetaType)types[num2]).GetSurrogateOrBaseOrSelf(false);
                                    if (!list.Contains(metaType2))
                                    {
                                        list.Add(metaType2);
                                        CascadeDependents(list, metaType2);
                                    }
                                }
                            }
                        }
                    }
                }
				else
				{
					IEnumerator enumerator = metaType.Fields.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							ValueMember valueMember = (ValueMember)obj;
							Type type2 = valueMember.ItemType;
							if (type2 == null)
							{
								type2 = valueMember.MemberType;
							}
                            if (ValueMember.TryGetCoreSerializer(this, DataFormat.Default, type2, out WireType wireType3, false, false, false, false) == null)
                            {
                                int num3 = FindOrAddAuto(type2, false, false, false);
                                if (num3 >= 0)
                                {
                                    metaType2 = ((MetaType)types[num3]).GetSurrogateOrBaseOrSelf(false);
                                    if (!list.Contains(metaType2))
                                    {
                                        list.Add(metaType2);
                                        CascadeDependents(list, metaType2);
                                    }
                                }
                            }
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
				}
				if (metaType.HasSubtypes)
				{
					foreach (SubType subType in metaType.GetSubtypes())
					{
						metaType2 = subType.DerivedType.GetSurrogateOrSelf();
						if (!list.Contains(metaType2))
						{
							list.Add(metaType2);
							CascadeDependents(list, metaType2);
						}
					}
				}
				metaType2 = metaType.BaseType;
				if (metaType2 != null)
				{
					metaType2 = metaType2.GetSurrogateOrSelf();
				}
				if (metaType2 != null && !list.Contains(metaType2))
				{
					list.Add(metaType2);
					CascadeDependents(list, metaType2);
				}
			}
		}

        // Token: 0x170002DD RID: 733
        public MetaType this[Type type] => (MetaType)types[FindOrAddAuto(type, true, false, false)];

        // Token: 0x06003370 RID: 13168 RVA: 0x0012D928 File Offset: 0x0012BD28
        internal MetaType FindWithoutAdd(Type type)
		{
			foreach (object obj in types)
			{
				MetaType metaType = (MetaType)obj;
				if (metaType.Type == type)
				{
					if (metaType.Pending)
					{
						WaitOnLock(metaType);
					}
					return metaType;
				}
			}
			Type type2 = TypeModel.ResolveProxies(type);
			return (type2 != null) ? FindWithoutAdd(type2) : null;
		}

		// Token: 0x06003371 RID: 13169 RVA: 0x0012D999 File Offset: 0x0012BD99
		private static bool MetaTypeFinderImpl(object value, object ctx)
		{
			return ((MetaType)value).Type == (Type)ctx;
		}

		// Token: 0x06003372 RID: 13170 RVA: 0x0012D9AE File Offset: 0x0012BDAE
		private static bool BasicTypeFinderImpl(object value, object ctx)
		{
			return ((RuntimeTypeModel.BasicType)value).Type == (Type)ctx;
		}

		// Token: 0x06003373 RID: 13171 RVA: 0x0012D9C4 File Offset: 0x0012BDC4
		private void WaitOnLock(MetaType type)
		{
			int opaqueToken = 0;
			try
			{
				TakeLock(ref opaqueToken);
			}
			finally
			{
				ReleaseLock(opaqueToken);
			}
		}

		// Token: 0x06003374 RID: 13172 RVA: 0x0012D9F8 File Offset: 0x0012BDF8
		internal IProtoSerializer TryGetBasicTypeSerializer(Type type)
		{
			int num = basicTypes.IndexOf(RuntimeTypeModel.BasicTypeFinder, type);
			if (num >= 0)
			{
				return ((RuntimeTypeModel.BasicType)basicTypes[num]).Serializer;
			}
			object obj = basicTypes;
			IProtoSerializer result;
			lock (obj)
			{
				num = basicTypes.IndexOf(RuntimeTypeModel.BasicTypeFinder, type);
				if (num >= 0)
				{
					result = ((RuntimeTypeModel.BasicType)basicTypes[num]).Serializer;
				}
				else
				{
                    IProtoSerializer protoSerializer = (MetaType.GetContractFamily(this, type, null) != MetaType.AttributeFamily.None) ? null : ValueMember.TryGetCoreSerializer(this, DataFormat.Default, type, out WireType wireType, false, false, false, false);
                    if (protoSerializer != null)
					{
						basicTypes.Add(new RuntimeTypeModel.BasicType(type, protoSerializer));
					}
					result = protoSerializer;
				}
			}
			return result;
		}

		// Token: 0x06003375 RID: 13173 RVA: 0x0012DADC File Offset: 0x0012BEDC
		internal int FindOrAddAuto(Type type, bool demand, bool addWithContractOnly, bool addEvenIfAutoDisabled)
		{
			int num = types.IndexOf(RuntimeTypeModel.MetaTypeFinder, type);
			if (num >= 0)
			{
				MetaType metaType = (MetaType)types[num];
				if (metaType.Pending)
				{
					WaitOnLock(metaType);
				}
				return num;
			}
			bool flag = AutoAddMissingTypes || addEvenIfAutoDisabled;
			if (Helpers.IsEnum(type) || TryGetBasicTypeSerializer(type) == null)
			{
				Type type2 = TypeModel.ResolveProxies(type);
				if (type2 != null)
				{
					num = types.IndexOf(RuntimeTypeModel.MetaTypeFinder, type2);
					type = type2;
				}
				if (num < 0)
				{
					int opaqueToken = 0;
					try
					{
						TakeLock(ref opaqueToken);
						MetaType metaType;
						if ((metaType = RecogniseCommonTypes(type)) == null)
						{
							MetaType.AttributeFamily contractFamily = MetaType.GetContractFamily(this, type, null);
							if (contractFamily == MetaType.AttributeFamily.AutoTuple)
							{
								addEvenIfAutoDisabled = (flag = true);
							}
							if (!flag || (!Helpers.IsEnum(type) && addWithContractOnly && contractFamily == MetaType.AttributeFamily.None))
							{
								if (demand)
								{
									TypeModel.ThrowUnexpectedType(type);
								}
								return num;
							}
							metaType = Create(type);
						}
						metaType.Pending = true;
						bool flag2 = false;
						int num2 = types.IndexOf(RuntimeTypeModel.MetaTypeFinder, type);
						if (num2 < 0)
						{
							ThrowIfFrozen();
							num = types.Add(metaType);
							flag2 = true;
						}
						else
						{
							num = num2;
						}
						if (flag2)
						{
							metaType.ApplyDefaultBehaviour();
							metaType.Pending = false;
						}
					}
					finally
					{
						ReleaseLock(opaqueToken);
					}
					return num;
				}
				return num;
			}
			if (flag && !addWithContractOnly)
			{
				throw MetaType.InbuiltType(type);
			}
			return -1;
		}

		// Token: 0x06003376 RID: 13174 RVA: 0x0012DC74 File Offset: 0x0012C074
		private MetaType RecogniseCommonTypes(Type type)
		{
			return null;
		}

		// Token: 0x06003377 RID: 13175 RVA: 0x0012DC77 File Offset: 0x0012C077
		private MetaType Create(Type type)
		{
			ThrowIfFrozen();
			return new MetaType(this, type, defaultFactory);
		}

		// Token: 0x06003378 RID: 13176 RVA: 0x0012DC8C File Offset: 0x0012C08C
		public MetaType Add(Type type, bool applyDefaultBehaviour)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			MetaType metaType = FindWithoutAdd(type);
			if (metaType != null)
			{
				return metaType;
			}
			int opaqueToken = 0;
			if (type.IsInterface && base.MapType(MetaType.ienumerable).IsAssignableFrom(type) && TypeModel.GetListItemType(this, type) == null)
			{
				throw new ArgumentException("IEnumerable[<T>] data cannot be used as a meta-type unless an Add method can be resolved");
			}
			try
			{
				metaType = RecogniseCommonTypes(type);
				if (metaType != null)
				{
					if (!applyDefaultBehaviour)
					{
						throw new ArgumentException("Default behaviour must be observed for certain types with special handling; " + type.FullName, "applyDefaultBehaviour");
					}
					applyDefaultBehaviour = false;
				}
				if (metaType == null)
				{
					metaType = Create(type);
				}
				metaType.Pending = true;
				TakeLock(ref opaqueToken);
				if (FindWithoutAdd(type) != null)
				{
					throw new ArgumentException("Duplicate type", "type");
				}
				ThrowIfFrozen();
				types.Add(metaType);
				if (applyDefaultBehaviour)
				{
					metaType.ApplyDefaultBehaviour();
				}
				metaType.Pending = false;
			}
			finally
			{
				ReleaseLock(opaqueToken);
			}
			return metaType;
		}

		// Token: 0x170002DE RID: 734
		// (get) Token: 0x06003379 RID: 13177 RVA: 0x0012DDA4 File Offset: 0x0012C1A4
		// (set) Token: 0x0600337A RID: 13178 RVA: 0x0012DDAD File Offset: 0x0012C1AD
		public bool AutoAddMissingTypes
        {
            get => GetOption(8);
            set
            {
                if (!value && GetOption(2))
                {
                    throw new InvalidOperationException("The default model must allow missing types");
                }
                ThrowIfFrozen();
                SetOption(8, value);
            }
        }

        // Token: 0x0600337B RID: 13179 RVA: 0x0012DDDA File Offset: 0x0012C1DA
        private void ThrowIfFrozen()
		{
			if (GetOption(4))
			{
				throw new InvalidOperationException("The model cannot be changed once frozen");
			}
		}

		// Token: 0x0600337C RID: 13180 RVA: 0x0012DDF3 File Offset: 0x0012C1F3
		public void Freeze()
		{
			if (GetOption(2))
			{
				throw new InvalidOperationException("The default model cannot be frozen");
			}
			SetOption(4, true);
		}

		// Token: 0x0600337D RID: 13181 RVA: 0x0012DE14 File Offset: 0x0012C214
		protected override int GetKeyImpl(Type type)
		{
			return GetKey(type, false, true);
		}

		// Token: 0x0600337E RID: 13182 RVA: 0x0012DE20 File Offset: 0x0012C220
		internal int GetKey(Type type, bool demand, bool getBaseKey)
		{
			int result;
			try
			{
				int num = FindOrAddAuto(type, demand, true, false);
				if (num >= 0)
				{
					MetaType metaType = (MetaType)types[num];
					if (getBaseKey)
					{
						metaType = MetaType.GetRootType(metaType);
						num = FindOrAddAuto(metaType.Type, true, true, false);
					}
				}
				result = num;
			}
			catch (NotSupportedException)
			{
				throw;
			}
			catch (Exception ex)
			{
				if (ex.Message.IndexOf(type.FullName) >= 0)
				{
					throw;
				}
				throw new ProtoException(ex.Message + " (" + type.FullName + ")", ex);
			}
			return result;
		}

		// Token: 0x0600337F RID: 13183 RVA: 0x0012DED4 File Offset: 0x0012C2D4
		protected internal override void Serialize(int key, object value, ProtoWriter dest)
		{
			((MetaType)types[key]).Serializer.Write(value, dest);
		}

		// Token: 0x06003380 RID: 13184 RVA: 0x0012DEF4 File Offset: 0x0012C2F4
		protected internal override object Deserialize(int key, object value, ProtoReader source)
		{
			IProtoSerializer serializer = ((MetaType)types[key]).Serializer;
			if (value == null && Helpers.IsValueType(serializer.ExpectedType))
			{
				if (serializer.RequiresOldValue)
				{
					value = Activator.CreateInstance(serializer.ExpectedType);
				}
				return serializer.Read(value, source);
			}
			return serializer.Read(value, source);
		}

		// Token: 0x06003381 RID: 13185 RVA: 0x0012DF58 File Offset: 0x0012C358
		internal bool IsPrepared(Type type)
		{
			MetaType metaType = FindWithoutAdd(type);
			return metaType != null && metaType.IsPrepared();
		}

		// Token: 0x06003382 RID: 13186 RVA: 0x0012DF7C File Offset: 0x0012C37C
		internal EnumSerializer.EnumPair[] GetEnumMap(Type type)
		{
			int num = FindOrAddAuto(type, false, false, false);
			return (num >= 0) ? ((MetaType)types[num]).GetEnumMap() : null;
		}

		// Token: 0x170002DF RID: 735
		// (get) Token: 0x06003383 RID: 13187 RVA: 0x0012DFB7 File Offset: 0x0012C3B7
		// (set) Token: 0x06003384 RID: 13188 RVA: 0x0012DFBF File Offset: 0x0012C3BF
		public int MetadataTimeoutMilliseconds
        {
            get => metadataTimeoutMilliseconds;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("MetadataTimeoutMilliseconds");
                }
                metadataTimeoutMilliseconds = value;
            }
        }

        // Token: 0x06003385 RID: 13189 RVA: 0x0012DFDA File Offset: 0x0012C3DA
        internal void TakeLock(ref int opaqueToken)
		{
			opaqueToken = 0;
			if (Monitor.TryEnter(types, metadataTimeoutMilliseconds))
			{
				opaqueToken = GetContention();
				return;
			}
			AddContention();
			throw new TimeoutException("Timeout while inspecting metadata; this may indicate a deadlock. This can often be avoided by preparing necessary serializers during application initialization, rather than allowing multiple threads to perform the initial metadata inspection; please also see the LockContended event");
		}

		// Token: 0x06003386 RID: 13190 RVA: 0x0012E013 File Offset: 0x0012C413
		private int GetContention()
		{
			return Interlocked.CompareExchange(ref contentionCounter, 0, 0);
		}

		// Token: 0x06003387 RID: 13191 RVA: 0x0012E022 File Offset: 0x0012C422
		private void AddContention()
		{
			Interlocked.Increment(ref contentionCounter);
		}

		// Token: 0x06003388 RID: 13192 RVA: 0x0012E030 File Offset: 0x0012C430
		internal void ReleaseLock(int opaqueToken)
		{
			if (opaqueToken != 0)
			{
				Monitor.Exit(types);
				if (opaqueToken != GetContention())
				{
					LockContentedEventHandler lockContended = LockContended;
					if (lockContended != null)
					{
						string stackTrace;
						try
						{
							throw new ProtoException();
						}
						catch (Exception ex)
						{
							stackTrace = ex.StackTrace;
						}
						lockContended(this, new LockContentedEventArgs(stackTrace));
					}
				}
			}
		}

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06003389 RID: 13193 RVA: 0x0012E098 File Offset: 0x0012C498
		// (remove) Token: 0x0600338A RID: 13194 RVA: 0x0012E0D0 File Offset: 0x0012C4D0
		public event LockContentedEventHandler LockContended;

		// Token: 0x0600338B RID: 13195 RVA: 0x0012E108 File Offset: 0x0012C508
		internal void ResolveListTypes(Type type, ref Type itemType, ref Type defaultType)
		{
			if (type == null)
			{
				return;
			}
			if (Helpers.GetTypeCode(type) != ProtoTypeCode.Unknown)
			{
				return;
			}
			if (this[type].IgnoreListHandling)
			{
				return;
			}
			if (type.IsArray)
			{
				if (type.GetArrayRank() != 1)
				{
					throw new NotSupportedException("Multi-dimension arrays are supported");
				}
				itemType = type.GetElementType();
				if (itemType == base.MapType(typeof(byte)))
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
				itemType = TypeModel.GetListItemType(this, type);
			}
			if (itemType != null)
			{
				Type type3 = null;
				Type type4 = null;
				ResolveListTypes(itemType, ref type3, ref type4);
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
					if (type.IsGenericType && type.GetGenericTypeDefinition() == base.MapType(typeof(IDictionary<, >)) && itemType == base.MapType(typeof(KeyValuePair<, >)).MakeGenericType(genericArguments = type.GetGenericArguments()))
					{
						defaultType = base.MapType(typeof(Dictionary<, >)).MakeGenericType(genericArguments);
					}
					else
					{
						defaultType = base.MapType(typeof(List<>)).MakeGenericType(new Type[]
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

		// Token: 0x0600338C RID: 13196 RVA: 0x0012E2AC File Offset: 0x0012C6AC
		internal string GetSchemaTypeName(Type effectiveType, DataFormat dataFormat, bool asReference, bool dynamicType, ref bool requiresBclImport)
		{
			Type underlyingType = Helpers.GetUnderlyingType(effectiveType);
			if (underlyingType != null)
			{
				effectiveType = underlyingType;
			}
			if (effectiveType == base.MapType(typeof(byte[])))
			{
				return "bytes";
			}
            IProtoSerializer protoSerializer = ValueMember.TryGetCoreSerializer(this, dataFormat, effectiveType, out WireType wireType, false, false, false, false);
            if (protoSerializer == null)
			{
				if (asReference || dynamicType)
				{
					requiresBclImport = true;
					return "bcl.NetObjectProxy";
				}
				return this[effectiveType].GetSurrogateOrBaseOrSelf(true).GetSchemaTypeName();
			}
			else
			{
				if (protoSerializer is ParseableSerializer)
				{
					if (asReference)
					{
						requiresBclImport = true;
					}
					return (!asReference) ? "string" : "bcl.NetObjectProxy";
				}
				ProtoTypeCode typeCode = Helpers.GetTypeCode(effectiveType);
				switch (typeCode)
				{
				case ProtoTypeCode.Boolean:
					return "bool";
				case ProtoTypeCode.Char:
				case ProtoTypeCode.Byte:
				case ProtoTypeCode.UInt16:
				case ProtoTypeCode.UInt32:
					if (dataFormat != DataFormat.FixedSize)
					{
						return "uint32";
					}
					return "fixed32";
				case ProtoTypeCode.SByte:
				case ProtoTypeCode.Int16:
				case ProtoTypeCode.Int32:
					if (dataFormat == DataFormat.ZigZag)
					{
						return "sint32";
					}
					if (dataFormat != DataFormat.FixedSize)
					{
						return "int32";
					}
					return "sfixed32";
				case ProtoTypeCode.Int64:
					if (dataFormat == DataFormat.ZigZag)
					{
						return "sint64";
					}
					if (dataFormat != DataFormat.FixedSize)
					{
						return "int64";
					}
					return "sfixed64";
				case ProtoTypeCode.UInt64:
					if (dataFormat != DataFormat.FixedSize)
					{
						return "uint64";
					}
					return "fixed64";
				case ProtoTypeCode.Single:
					return "float";
				case ProtoTypeCode.Double:
					return "double";
				case ProtoTypeCode.Decimal:
					requiresBclImport = true;
					return "bcl.Decimal";
				case ProtoTypeCode.DateTime:
					requiresBclImport = true;
					return "bcl.DateTime";
				default:
					switch (typeCode)
					{
					case ProtoTypeCode.TimeSpan:
						requiresBclImport = true;
						return "bcl.TimeSpan";
					case ProtoTypeCode.Guid:
						requiresBclImport = true;
						return "bcl.Guid";
					}
					throw new NotSupportedException("No .proto map found for: " + effectiveType.FullName);
				case ProtoTypeCode.String:
					if (asReference)
					{
						requiresBclImport = true;
					}
					return (!asReference) ? "string" : "bcl.NetObjectProxy";
				}
			}
		}

		// Token: 0x0600338D RID: 13197 RVA: 0x0012E4A1 File Offset: 0x0012C8A1
		public void SetDefaultFactory(MethodInfo methodInfo)
		{
			VerifyFactory(methodInfo, null);
			defaultFactory = methodInfo;
		}

		// Token: 0x0600338E RID: 13198 RVA: 0x0012E4B4 File Offset: 0x0012C8B4
		internal void VerifyFactory(MethodInfo factory, Type type)
		{
			if (factory != null)
			{
				if (type != null && Helpers.IsValueType(type))
				{
					throw new InvalidOperationException();
				}
				if (!factory.IsStatic)
				{
					throw new ArgumentException("A factory-method must be static", "factory");
				}
				if (type != null && factory.ReturnType != type && factory.ReturnType != base.MapType(typeof(object)))
				{
					throw new ArgumentException("The factory-method must return object" + ((type != null) ? (" or " + type.FullName) : string.Empty), "factory");
				}
				if (!CallbackSet.CheckCallbackParameters(this, factory))
				{
					throw new ArgumentException("Invalid factory signature in " + factory.DeclaringType.FullName + "." + factory.Name, "factory");
				}
			}
		}

		// Token: 0x0400240D RID: 9229
		private ushort options;

		// Token: 0x0400240E RID: 9230
		private const ushort OPTIONS_InferTagFromNameDefault = 1;

		// Token: 0x0400240F RID: 9231
		private const ushort OPTIONS_IsDefaultModel = 2;

		// Token: 0x04002410 RID: 9232
		private const ushort OPTIONS_Frozen = 4;

		// Token: 0x04002411 RID: 9233
		private const ushort OPTIONS_AutoAddMissingTypes = 8;

		// Token: 0x04002412 RID: 9234
		private const ushort OPTIONS_UseImplicitZeroDefaults = 32;

		// Token: 0x04002413 RID: 9235
		private const ushort OPTIONS_AllowParseableTypes = 64;

		// Token: 0x04002414 RID: 9236
		private const ushort OPTIONS_AutoAddProtoContractTypesOnly = 128;

		// Token: 0x04002415 RID: 9237
		private const ushort OPTIONS_IncludeDateTimeKind = 256;

		// Token: 0x04002416 RID: 9238
		private static readonly BasicList.MatchPredicate MetaTypeFinder = new BasicList.MatchPredicate(RuntimeTypeModel.MetaTypeFinderImpl);

		// Token: 0x04002417 RID: 9239
		private static readonly BasicList.MatchPredicate BasicTypeFinder = new BasicList.MatchPredicate(RuntimeTypeModel.BasicTypeFinderImpl);

		// Token: 0x04002418 RID: 9240
		private readonly BasicList basicTypes = new BasicList();

		// Token: 0x04002419 RID: 9241
		private readonly BasicList types = new BasicList();

		// Token: 0x0400241A RID: 9242
		private int metadataTimeoutMilliseconds = 5000;

		// Token: 0x0400241B RID: 9243
		private int contentionCounter = 1;

		// Token: 0x0400241D RID: 9245
		private MethodInfo defaultFactory;

		// Token: 0x020003DD RID: 989
		private sealed class Singleton
		{
			// Token: 0x06003390 RID: 13200 RVA: 0x0012E5B6 File Offset: 0x0012C9B6
			private Singleton()
			{
			}

			// Token: 0x0400241E RID: 9246
			internal static readonly RuntimeTypeModel Value = new RuntimeTypeModel(true);
		}

		// Token: 0x020003DE RID: 990
		private sealed class BasicType
		{
			// Token: 0x06003392 RID: 13202 RVA: 0x0012E5CB File Offset: 0x0012C9CB
			public BasicType(Type type, IProtoSerializer serializer)
			{
				this.type = type;
				this.serializer = serializer;
			}

            // Token: 0x170002E0 RID: 736
            // (get) Token: 0x06003393 RID: 13203 RVA: 0x0012E5E1 File Offset: 0x0012C9E1
            public Type Type => type;

            // Token: 0x170002E1 RID: 737
            // (get) Token: 0x06003394 RID: 13204 RVA: 0x0012E5E9 File Offset: 0x0012C9E9
            public IProtoSerializer Serializer => serializer;

            // Token: 0x0400241F RID: 9247
            private readonly Type type;

			// Token: 0x04002420 RID: 9248
			private readonly IProtoSerializer serializer;
		}
	}
}
