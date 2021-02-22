using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
    // Token: 0x0200041C RID: 1052
    internal sealed class StringSerializer : IProtoSerializer
	{
		// Token: 0x060035D3 RID: 13779 RVA: 0x00135146 File Offset: 0x00133546
		public StringSerializer(TypeModel model)
		{
		}

        // Token: 0x1700037E RID: 894
        // (get) Token: 0x060035D4 RID: 13780 RVA: 0x0013514E File Offset: 0x0013354E
        public Type ExpectedType => expectedType;

        // Token: 0x060035D5 RID: 13781 RVA: 0x00135155 File Offset: 0x00133555
        public void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteString((string)value, dest);
		}

        // Token: 0x1700037C RID: 892
        // (get) Token: 0x060035D6 RID: 13782 RVA: 0x00135163 File Offset: 0x00133563
        bool IProtoSerializer.RequiresOldValue => false;

        // Token: 0x1700037D RID: 893
        // (get) Token: 0x060035D7 RID: 13783 RVA: 0x00135166 File Offset: 0x00133566
        bool IProtoSerializer.ReturnsValue => true;

        // Token: 0x060035D8 RID: 13784 RVA: 0x00135169 File Offset: 0x00133569
        public object Read(object value, ProtoReader source)
		{
			return source.ReadString();
		}

		// Token: 0x040024E8 RID: 9448
		private static readonly Type expectedType = typeof(string);
	}
}
