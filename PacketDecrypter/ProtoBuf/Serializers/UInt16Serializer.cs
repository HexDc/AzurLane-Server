using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
    // Token: 0x02000424 RID: 1060
    internal class UInt16Serializer : IProtoSerializer
	{
		// Token: 0x06003624 RID: 13860 RVA: 0x00133788 File Offset: 0x00131B88
		public UInt16Serializer(TypeModel model)
		{
		}

        // Token: 0x17000398 RID: 920
        // (get) Token: 0x06003625 RID: 13861 RVA: 0x00133790 File Offset: 0x00131B90
        public virtual Type ExpectedType => expectedType;

        // Token: 0x17000396 RID: 918
        // (get) Token: 0x06003626 RID: 13862 RVA: 0x00133797 File Offset: 0x00131B97
        bool IProtoSerializer.RequiresOldValue => false;

        // Token: 0x17000397 RID: 919
        // (get) Token: 0x06003627 RID: 13863 RVA: 0x0013379A File Offset: 0x00131B9A
        bool IProtoSerializer.ReturnsValue => true;

        // Token: 0x06003628 RID: 13864 RVA: 0x0013379D File Offset: 0x00131B9D
        public virtual object Read(object value, ProtoReader source)
		{
			return source.ReadUInt16();
		}

		// Token: 0x06003629 RID: 13865 RVA: 0x001337AA File Offset: 0x00131BAA
		public virtual void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteUInt16((ushort)value, dest);
		}

		// Token: 0x04002506 RID: 9478
		private static readonly Type expectedType = typeof(ushort);
	}
}
