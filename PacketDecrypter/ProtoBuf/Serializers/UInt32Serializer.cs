using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
    // Token: 0x02000425 RID: 1061
    internal sealed class UInt32Serializer : IProtoSerializer
	{
		// Token: 0x0600362B RID: 13867 RVA: 0x00136167 File Offset: 0x00134567
		public UInt32Serializer(TypeModel model)
		{
		}

        // Token: 0x1700039B RID: 923
        // (get) Token: 0x0600362C RID: 13868 RVA: 0x0013616F File Offset: 0x0013456F
        public Type ExpectedType => expectedType;

        // Token: 0x17000399 RID: 921
        // (get) Token: 0x0600362D RID: 13869 RVA: 0x00136176 File Offset: 0x00134576
        bool IProtoSerializer.RequiresOldValue => false;

        // Token: 0x1700039A RID: 922
        // (get) Token: 0x0600362E RID: 13870 RVA: 0x00136179 File Offset: 0x00134579
        bool IProtoSerializer.ReturnsValue => true;

        // Token: 0x0600362F RID: 13871 RVA: 0x0013617C File Offset: 0x0013457C
        public object Read(object value, ProtoReader source)
		{
			return source.ReadUInt32();
		}

		// Token: 0x06003630 RID: 13872 RVA: 0x00136189 File Offset: 0x00134589
		public void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteUInt32((uint)value, dest);
		}

		// Token: 0x04002507 RID: 9479
		private static readonly Type expectedType = typeof(uint);
	}
}
