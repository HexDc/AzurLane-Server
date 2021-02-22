using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
    // Token: 0x02000426 RID: 1062
    internal sealed class UInt64Serializer : IProtoSerializer
	{
		// Token: 0x06003632 RID: 13874 RVA: 0x001361A8 File Offset: 0x001345A8
		public UInt64Serializer(TypeModel model)
		{
		}

        // Token: 0x1700039E RID: 926
        // (get) Token: 0x06003633 RID: 13875 RVA: 0x001361B0 File Offset: 0x001345B0
        public Type ExpectedType => expectedType;

        // Token: 0x1700039C RID: 924
        // (get) Token: 0x06003634 RID: 13876 RVA: 0x001361B7 File Offset: 0x001345B7
        bool IProtoSerializer.RequiresOldValue => false;

        // Token: 0x1700039D RID: 925
        // (get) Token: 0x06003635 RID: 13877 RVA: 0x001361BA File Offset: 0x001345BA
        bool IProtoSerializer.ReturnsValue => true;

        // Token: 0x06003636 RID: 13878 RVA: 0x001361BD File Offset: 0x001345BD
        public object Read(object value, ProtoReader source)
		{
			return source.ReadUInt64();
		}

		// Token: 0x06003637 RID: 13879 RVA: 0x001361CA File Offset: 0x001345CA
		public void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteUInt64((ulong)value, dest);
		}

		// Token: 0x04002508 RID: 9480
		private static readonly Type expectedType = typeof(ulong);
	}
}
