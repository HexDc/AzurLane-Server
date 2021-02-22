using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
    // Token: 0x0200041F RID: 1055
    internal sealed class SystemTypeSerializer : IProtoSerializer
	{
		// Token: 0x060035F0 RID: 13808 RVA: 0x001354F8 File Offset: 0x001338F8
		public SystemTypeSerializer(TypeModel model)
		{
		}

        // Token: 0x17000387 RID: 903
        // (get) Token: 0x060035F1 RID: 13809 RVA: 0x00135500 File Offset: 0x00133900
        public Type ExpectedType => SystemTypeSerializer.expectedType;

        // Token: 0x060035F2 RID: 13810 RVA: 0x00135507 File Offset: 0x00133907
        void IProtoSerializer.Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteType((Type)value, dest);
		}

		// Token: 0x060035F3 RID: 13811 RVA: 0x00135515 File Offset: 0x00133915
		object IProtoSerializer.Read(object value, ProtoReader source)
		{
			return source.ReadType();
		}

        // Token: 0x17000385 RID: 901
        // (get) Token: 0x060035F4 RID: 13812 RVA: 0x0013551D File Offset: 0x0013391D
        bool IProtoSerializer.RequiresOldValue => false;

        // Token: 0x17000386 RID: 902
        // (get) Token: 0x060035F5 RID: 13813 RVA: 0x00135520 File Offset: 0x00133920
        bool IProtoSerializer.ReturnsValue => true;

        // Token: 0x040024F2 RID: 9458
        private static readonly Type expectedType = typeof(Type);
	}
}
