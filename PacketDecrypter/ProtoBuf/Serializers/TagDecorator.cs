using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
    // Token: 0x02000420 RID: 1056
    internal sealed class TagDecorator : ProtoDecoratorBase, IProtoTypeSerializer, IProtoSerializer
	{
		// Token: 0x060035F7 RID: 13815 RVA: 0x00135534 File Offset: 0x00133934
		public TagDecorator(int fieldNumber, WireType wireType, bool strict, IProtoSerializer tail) : base(tail)
		{
			this.fieldNumber = fieldNumber;
			this.wireType = wireType;
			this.strict = strict;
		}

		// Token: 0x060035F8 RID: 13816 RVA: 0x00135554 File Offset: 0x00133954
		public bool HasCallbacks(TypeModel.CallbackType callbackType)
		{
			IProtoTypeSerializer protoTypeSerializer = Tail as IProtoTypeSerializer;
			return protoTypeSerializer != null && protoTypeSerializer.HasCallbacks(callbackType);
		}

		// Token: 0x060035F9 RID: 13817 RVA: 0x00135580 File Offset: 0x00133980
		public bool CanCreateInstance()
		{
			IProtoTypeSerializer protoTypeSerializer = Tail as IProtoTypeSerializer;
			return protoTypeSerializer != null && protoTypeSerializer.CanCreateInstance();
		}

		// Token: 0x060035FA RID: 13818 RVA: 0x001355A8 File Offset: 0x001339A8
		public object CreateInstance(ProtoReader source)
		{
			return ((IProtoTypeSerializer)Tail).CreateInstance(source);
		}

		// Token: 0x060035FB RID: 13819 RVA: 0x001355BC File Offset: 0x001339BC
		public void Callback(object value, TypeModel.CallbackType callbackType, SerializationContext context)
		{
			IProtoTypeSerializer protoTypeSerializer = Tail as IProtoTypeSerializer;
			if (protoTypeSerializer != null)
			{
				protoTypeSerializer.Callback(value, callbackType, context);
			}
		}

        // Token: 0x17000388 RID: 904
        // (get) Token: 0x060035FC RID: 13820 RVA: 0x001355E4 File Offset: 0x001339E4
        public override Type ExpectedType => Tail.ExpectedType;

        // Token: 0x17000389 RID: 905
        // (get) Token: 0x060035FD RID: 13821 RVA: 0x001355F1 File Offset: 0x001339F1
        public override bool RequiresOldValue => Tail.RequiresOldValue;

        // Token: 0x1700038A RID: 906
        // (get) Token: 0x060035FE RID: 13822 RVA: 0x001355FE File Offset: 0x001339FE
        public override bool ReturnsValue => Tail.ReturnsValue;

        // Token: 0x1700038B RID: 907
        // (get) Token: 0x060035FF RID: 13823 RVA: 0x0013560B File Offset: 0x00133A0B
        private bool NeedsHint => (wireType & (WireType)(-8)) != WireType.Variant;

        // Token: 0x06003600 RID: 13824 RVA: 0x0013561C File Offset: 0x00133A1C
        public override object Read(object value, ProtoReader source)
		{
			if (strict)
			{
				source.Assert(wireType);
			}
			else if (NeedsHint)
			{
				source.Hint(wireType);
			}
			return Tail.Read(value, source);
		}

		// Token: 0x06003601 RID: 13825 RVA: 0x00135669 File Offset: 0x00133A69
		public override void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteFieldHeader(fieldNumber, wireType, dest);
			Tail.Write(value, dest);
		}

		// Token: 0x040024F3 RID: 9459
		private readonly bool strict;

		// Token: 0x040024F4 RID: 9460
		private readonly int fieldNumber;

		// Token: 0x040024F5 RID: 9461
		private readonly WireType wireType;
	}
}
