using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
    // Token: 0x02000400 RID: 1024
    internal sealed class BlobSerializer : IProtoSerializer
	{
		// Token: 0x0600351B RID: 13595 RVA: 0x001336A4 File Offset: 0x00131AA4
		public BlobSerializer(TypeModel model, bool overwriteList)
		{
			this.overwriteList = overwriteList;
		}

        // Token: 0x1700032F RID: 815
        // (get) Token: 0x0600351C RID: 13596 RVA: 0x001336B3 File Offset: 0x00131AB3
        public Type ExpectedType => expectedType;

        // Token: 0x0600351D RID: 13597 RVA: 0x001336BA File Offset: 0x00131ABA
        public object Read(object value, ProtoReader source)
		{
			return ProtoReader.AppendBytes((!overwriteList) ? ((byte[])value) : null, source);
		}

		// Token: 0x0600351E RID: 13598 RVA: 0x001336D9 File Offset: 0x00131AD9
		public void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteBytes((byte[])value, dest);
		}

        // Token: 0x1700032D RID: 813
        // (get) Token: 0x0600351F RID: 13599 RVA: 0x001336E7 File Offset: 0x00131AE7
        bool IProtoSerializer.RequiresOldValue => !overwriteList;

        // Token: 0x1700032E RID: 814
        // (get) Token: 0x06003520 RID: 13600 RVA: 0x001336F2 File Offset: 0x00131AF2
        bool IProtoSerializer.ReturnsValue => true;

        // Token: 0x040024B2 RID: 9394
        private static readonly Type expectedType = typeof(byte[]);

		// Token: 0x040024B3 RID: 9395
		private readonly bool overwriteList;
	}
}
