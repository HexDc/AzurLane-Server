using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
    // Token: 0x02000427 RID: 1063
    internal sealed class UriDecorator : ProtoDecoratorBase
	{
		// Token: 0x06003639 RID: 13881 RVA: 0x001361E9 File Offset: 0x001345E9
		public UriDecorator(TypeModel model, IProtoSerializer tail) : base(tail)
		{
		}

        // Token: 0x1700039F RID: 927
        // (get) Token: 0x0600363A RID: 13882 RVA: 0x001361F2 File Offset: 0x001345F2
        public override Type ExpectedType => expectedType;

        // Token: 0x170003A0 RID: 928
        // (get) Token: 0x0600363B RID: 13883 RVA: 0x001361F9 File Offset: 0x001345F9
        public override bool RequiresOldValue => false;

        // Token: 0x170003A1 RID: 929
        // (get) Token: 0x0600363C RID: 13884 RVA: 0x001361FC File Offset: 0x001345FC
        public override bool ReturnsValue => true;

        // Token: 0x0600363D RID: 13885 RVA: 0x001361FF File Offset: 0x001345FF
        public override void Write(object value, ProtoWriter dest)
		{
			this.Tail.Write(((Uri)value).AbsoluteUri, dest);
		}

		// Token: 0x0600363E RID: 13886 RVA: 0x00136218 File Offset: 0x00134618
		public override object Read(object value, ProtoReader source)
		{
			string text = (string)Tail.Read(null, source);
			return (text.Length != 0) ? new Uri(text) : null;
		}

		// Token: 0x04002509 RID: 9481
		private static readonly Type expectedType = typeof(Uri);
	}
}
