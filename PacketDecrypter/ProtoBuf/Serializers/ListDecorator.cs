using ProtoBuf.Meta;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ProtoBuf.Serializers
{
    // Token: 0x02000413 RID: 1043
    internal class ListDecorator : ProtoDecoratorBase
	{
		// Token: 0x0600358B RID: 13707 RVA: 0x00133E30 File Offset: 0x00132230
		protected ListDecorator(TypeModel model, Type declaredType, Type concreteType, IProtoSerializer tail, int fieldNumber, bool writePacked, WireType packedWireType, bool returnList, bool overwriteList, bool supportNull) : base(tail)
		{
			if (returnList) options |= OPTIONS_ReturnList;
			if (overwriteList) options |= OPTIONS_OverwriteList;
			if (supportNull) options |= OPTIONS_SupportNull;
			if ((writePacked || packedWireType != WireType.None) && fieldNumber <= 0) throw new ArgumentOutOfRangeException("fieldNumber");
			if (!CanPack(packedWireType))
			{
				if (writePacked) throw new InvalidOperationException("Only simple data-types can use packed encoding");
				packedWireType = WireType.None;
			}
			this.fieldNumber = fieldNumber;
			if (writePacked) options |= OPTIONS_WritePacked;
			this.packedWireType = packedWireType;
			if (declaredType == null) throw new ArgumentNullException("declaredType");
			if (declaredType.IsArray) throw new ArgumentException("Cannot treat arrays as lists", "declaredType");
			this.declaredType = declaredType;
			this.concreteType = concreteType;
			if (RequireAdd)
			{
                add = TypeModel.ResolveListAdd(model, declaredType, tail.ExpectedType, out bool flag);
                if (flag)
				{
					options |= 1;
					string fullName = declaredType.FullName;
					if (fullName != null && fullName.StartsWith("System.Data.Linq.EntitySet`1[["))
					{
						options |= 2;
					}
				}
				if (add == null)
				{
					throw new InvalidOperationException("Unable to resolve a suitable Add method for " + declaredType.FullName);
				}
			}
		}

		// Token: 0x0600358C RID: 13708 RVA: 0x00133FAF File Offset: 0x001323AF
		internal static bool CanPack(WireType wireType)
		{
			switch (wireType)
			{
			case WireType.Variant:
			case WireType.Fixed64:
			case WireType.Fixed32:
				break;
			default:
				if (wireType != WireType.SignedVariant)
				{
					return false;
				}
				break;
			}
			return true;
		}

        // Token: 0x1700035A RID: 858
        // (get) Token: 0x0600358D RID: 13709 RVA: 0x00133FDE File Offset: 0x001323DE
        private bool IsList => (options & 1) != 0;

        // Token: 0x1700035B RID: 859
        // (get) Token: 0x0600358E RID: 13710 RVA: 0x00133FEE File Offset: 0x001323EE
        private bool SuppressIList => (options & 2) != 0;

        // Token: 0x1700035C RID: 860
        // (get) Token: 0x0600358F RID: 13711 RVA: 0x00133FFE File Offset: 0x001323FE
        private bool WritePacked => (options & 4) != 0;

        // Token: 0x1700035D RID: 861
        // (get) Token: 0x06003590 RID: 13712 RVA: 0x0013400E File Offset: 0x0013240E
        private bool SupportNull => (options & 32) != 0;

        // Token: 0x1700035E RID: 862
        // (get) Token: 0x06003591 RID: 13713 RVA: 0x0013401F File Offset: 0x0013241F
        private bool ReturnList => (options & 8) != 0;

        // Token: 0x06003592 RID: 13714 RVA: 0x00134030 File Offset: 0x00132430
        internal static ListDecorator Create(TypeModel model, Type declaredType, Type concreteType, IProtoSerializer tail, int fieldNumber, bool writePacked, WireType packedWireType, bool returnList, bool overwriteList, bool supportNull)
		{
            if (returnList && ImmutableCollectionDecorator.IdentifyImmutable(model, declaredType, out MethodInfo builderFactory, out MethodInfo methodInfo, out MethodInfo addRange, out MethodInfo finish))
            {
                return new ImmutableCollectionDecorator(model, declaredType, concreteType, tail, fieldNumber, writePacked, packedWireType, returnList, overwriteList, supportNull, builderFactory, methodInfo, addRange, finish);
            }
            return new ListDecorator(model, declaredType, concreteType, tail, fieldNumber, writePacked, packedWireType, returnList, overwriteList, supportNull);
		}

        // Token: 0x1700035F RID: 863
        // (get) Token: 0x06003593 RID: 13715 RVA: 0x00134087 File Offset: 0x00132487
        protected virtual bool RequireAdd => true;

        // Token: 0x17000360 RID: 864
        // (get) Token: 0x06003594 RID: 13716 RVA: 0x0013408A File Offset: 0x0013248A
        public override Type ExpectedType => declaredType;

        // Token: 0x17000361 RID: 865
        // (get) Token: 0x06003595 RID: 13717 RVA: 0x00134092 File Offset: 0x00132492
        public override bool RequiresOldValue => AppendToCollection;

        // Token: 0x17000362 RID: 866
        // (get) Token: 0x06003596 RID: 13718 RVA: 0x0013409A File Offset: 0x0013249A
        public override bool ReturnsValue => ReturnList;

        // Token: 0x17000363 RID: 867
        // (get) Token: 0x06003597 RID: 13719 RVA: 0x001340A2 File Offset: 0x001324A2
        protected bool AppendToCollection => (options & 16) == 0;

        // Token: 0x06003598 RID: 13720 RVA: 0x001340B0 File Offset: 0x001324B0
        protected MethodInfo GetEnumeratorInfo(TypeModel model, out MethodInfo moveNext, out MethodInfo current)
		{
			Type type = null;
			Type expectedType = ExpectedType;
			MethodInfo instanceMethod = Helpers.GetInstanceMethod(expectedType, "GetEnumerator", null);
			Type expectedType2 = Tail.ExpectedType;
			Type returnType;
			Type type2;
			if (instanceMethod != null)
			{
				returnType = instanceMethod.ReturnType;
				type2 = returnType;
				moveNext = Helpers.GetInstanceMethod(type2, "MoveNext", null);
				PropertyInfo property = Helpers.GetProperty(type2, "Current", false);
				current = ((property != null) ? Helpers.GetGetMethod(property, false, false) : null);
				if (moveNext == null && model.MapType(ListDecorator.ienumeratorType).IsAssignableFrom(type2))
				{
					moveNext = Helpers.GetInstanceMethod(model.MapType(ListDecorator.ienumeratorType), "MoveNext", null);
				}
				if (moveNext != null && moveNext.ReturnType == model.MapType(typeof(bool)) && current != null && current.ReturnType == expectedType2)
				{
					return instanceMethod;
				}
				MethodInfo methodInfo;
				current = (methodInfo = null);
				moveNext = methodInfo;
			}
			Type type3 = model.MapType(typeof(IEnumerable<>), false);
			if (type3 != null)
			{
				type3 = type3.MakeGenericType(new Type[]
				{
					expectedType2
				});
				type = type3;
			}
			if (type != null && type.IsAssignableFrom(expectedType))
			{
				instanceMethod = Helpers.GetInstanceMethod(type, "GetEnumerator");
				returnType = instanceMethod.ReturnType;
				type2 = returnType;
				moveNext = Helpers.GetInstanceMethod(model.MapType(ListDecorator.ienumeratorType), "MoveNext");
				current = Helpers.GetGetMethod(Helpers.GetProperty(type2, "Current", false), false, false);
				return instanceMethod;
			}
			type = model.MapType(ListDecorator.ienumerableType);
			instanceMethod = Helpers.GetInstanceMethod(type, "GetEnumerator");
			returnType = instanceMethod.ReturnType;
			type2 = returnType;
			moveNext = Helpers.GetInstanceMethod(type2, "MoveNext");
			current = Helpers.GetGetMethod(Helpers.GetProperty(type2, "Current", false), false, false);
			return instanceMethod;
		}

		// Token: 0x06003599 RID: 13721 RVA: 0x00134274 File Offset: 0x00132674
		public override void Write(object value, ProtoWriter dest)
		{
			bool writePacked = WritePacked;
			SubItemToken token;
			if (writePacked)
			{
				ProtoWriter.WriteFieldHeader(fieldNumber, WireType.String, dest);
				token = ProtoWriter.StartSubItem(value, dest);
				ProtoWriter.SetPackedField(fieldNumber, dest);
			}
			else
			{
				token = default(SubItemToken);
			}
			bool flag = !SupportNull;
			IEnumerator enumerator = ((IEnumerable)value).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					if (flag && obj == null)
					{
						throw new NullReferenceException();
					}
					Tail.Write(obj, dest);
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
			if (writePacked)
			{
				ProtoWriter.EndSubItem(token, dest);
			}
		}

		// Token: 0x0600359A RID: 13722 RVA: 0x00134344 File Offset: 0x00132744
		public override object Read(object value, ProtoReader source)
		{
			object result;
			try
			{
				int field = source.FieldNumber;
				object obj = value;
				if (value == null)
				{
					value = Activator.CreateInstance(concreteType);
				}
				bool flag = IsList && !SuppressIList;
				if (packedWireType != WireType.None && source.WireType == WireType.String)
				{
					SubItemToken token = ProtoReader.StartSubItem(source);
					if (flag)
					{
						IList list = (IList)value;
						while (ProtoReader.HasSubValue(packedWireType, source))
						{
							list.Add(Tail.Read(null, source));
						}
					}
					else
					{
						object[] array = new object[1];
						while (ProtoReader.HasSubValue(packedWireType, source))
						{
							array[0] = Tail.Read(null, source);
							add.Invoke(value, array);
						}
					}
					ProtoReader.EndSubItem(token, source);
				}
				else if (flag)
				{
					IList list2 = (IList)value;
					do
					{
						list2.Add(Tail.Read(null, source));
					}
					while (source.TryReadFieldHeader(field));
				}
				else
				{
					object[] array2 = new object[1];
					do
					{
						array2[0] = Tail.Read(null, source);
						add.Invoke(value, array2);
					}
					while (source.TryReadFieldHeader(field));
				}
				result = ((obj != value) ? value : null);
			}
			catch (TargetInvocationException ex)
			{
				if (ex.InnerException != null)
				{
					throw ex.InnerException;
				}
				throw;
			}
			return result;
		}

		// Token: 0x040024CB RID: 9419
		private readonly byte options;

		// Token: 0x040024CC RID: 9420
		private const byte OPTIONS_IsList = 1;

		// Token: 0x040024CD RID: 9421
		private const byte OPTIONS_SuppressIList = 2;

		// Token: 0x040024CE RID: 9422
		private const byte OPTIONS_WritePacked = 4;

		// Token: 0x040024CF RID: 9423
		private const byte OPTIONS_ReturnList = 8;

		// Token: 0x040024D0 RID: 9424
		private const byte OPTIONS_OverwriteList = 16;

		// Token: 0x040024D1 RID: 9425
		private const byte OPTIONS_SupportNull = 32;

		// Token: 0x040024D2 RID: 9426
		private readonly Type declaredType;

		// Token: 0x040024D3 RID: 9427
		private readonly Type concreteType;

		// Token: 0x040024D4 RID: 9428
		private readonly MethodInfo add;

		// Token: 0x040024D5 RID: 9429
		private readonly int fieldNumber;

		// Token: 0x040024D6 RID: 9430
		protected readonly WireType packedWireType;

		// Token: 0x040024D7 RID: 9431
		private static readonly Type ienumeratorType = typeof(IEnumerator);

		// Token: 0x040024D8 RID: 9432
		private static readonly Type ienumerableType = typeof(IEnumerable);
	}
}
