using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
    // Token: 0x02000411 RID: 1041
    internal sealed class Int32Serializer : IProtoSerializer
	{
		// Token: 0x0600357D RID: 13693 RVA: 0x00134A59 File Offset: 0x00132E59
		public Int32Serializer(TypeModel model)
		{
		}

        // Token: 0x17000356 RID: 854
        // (get) Token: 0x0600357E RID: 13694 RVA: 0x00134A61 File Offset: 0x00132E61
        public Type ExpectedType => Int32Serializer.expectedType;

        // Token: 0x17000354 RID: 852
        // (get) Token: 0x0600357F RID: 13695 RVA: 0x00134A68 File Offset: 0x00132E68
        bool IProtoSerializer.RequiresOldValue => false;

        // Token: 0x17000355 RID: 853
        // (get) Token: 0x06003580 RID: 13696 RVA: 0x00134A6B File Offset: 0x00132E6B
        bool IProtoSerializer.ReturnsValue => true;

        // Token: 0x06003581 RID: 13697 RVA: 0x00134A6E File Offset: 0x00132E6E
        public object Read(object value, ProtoReader source)
		{
			return source.ReadInt32();
		}

		// Token: 0x06003582 RID: 13698 RVA: 0x00134A7B File Offset: 0x00132E7B
		public void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteInt32((int)value, dest);
		}

		// Token: 0x040024C9 RID: 9417
		private static readonly Type expectedType = typeof(int);
	}
}
