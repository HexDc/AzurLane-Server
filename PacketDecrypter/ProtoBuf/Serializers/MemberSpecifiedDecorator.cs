using System;
using System.Reflection;

namespace ProtoBuf.Serializers
{
    // Token: 0x02000414 RID: 1044
    internal sealed class MemberSpecifiedDecorator : ProtoDecoratorBase
	{
		// Token: 0x0600359C RID: 13724 RVA: 0x00134ADB File Offset: 0x00132EDB
		public MemberSpecifiedDecorator(MethodInfo getSpecified, MethodInfo setSpecified, IProtoSerializer tail) : base(tail)
		{
			if (getSpecified == null && setSpecified == null)
			{
				throw new InvalidOperationException();
			}
			this.getSpecified = getSpecified;
			this.setSpecified = setSpecified;
		}

        // Token: 0x17000364 RID: 868
        // (get) Token: 0x0600359D RID: 13725 RVA: 0x00134B04 File Offset: 0x00132F04
        public override Type ExpectedType => Tail.ExpectedType;

        // Token: 0x17000365 RID: 869
        // (get) Token: 0x0600359E RID: 13726 RVA: 0x00134B11 File Offset: 0x00132F11
        public override bool RequiresOldValue => Tail.RequiresOldValue;

        // Token: 0x17000366 RID: 870
        // (get) Token: 0x0600359F RID: 13727 RVA: 0x00134B1E File Offset: 0x00132F1E
        public override bool ReturnsValue => Tail.ReturnsValue;

        // Token: 0x060035A0 RID: 13728 RVA: 0x00134B2B File Offset: 0x00132F2B
        public override void Write(object value, ProtoWriter dest)
		{
			if (getSpecified == null || (bool)getSpecified.Invoke(value, null))
			{
				Tail.Write(value, dest);
			}
		}

		// Token: 0x060035A1 RID: 13729 RVA: 0x00134B5C File Offset: 0x00132F5C
		public override object Read(object value, ProtoReader source)
		{
			object result = Tail.Read(value, source);
			if (setSpecified != null)
			{
				setSpecified.Invoke(value, new object[]
				{
					true
				});
			}
			return result;
		}

		// Token: 0x040024D9 RID: 9433
		private readonly MethodInfo getSpecified;

		// Token: 0x040024DA RID: 9434
		private readonly MethodInfo setSpecified;
	}
}
