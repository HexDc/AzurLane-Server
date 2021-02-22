using System;
using System.Reflection;

namespace ProtoBuf.Serializers
{
    // Token: 0x0200040A RID: 1034
    internal sealed class FieldDecorator : ProtoDecoratorBase
	{
		// Token: 0x0600355A RID: 13658 RVA: 0x00133D54 File Offset: 0x00132154
		public FieldDecorator(Type forType, FieldInfo field, IProtoSerializer tail) : base(tail)
		{
			this.forType = forType;
			this.field = field;
		}

        // Token: 0x17000346 RID: 838
        // (get) Token: 0x0600355B RID: 13659 RVA: 0x00133D6B File Offset: 0x0013216B
        public override Type ExpectedType => forType;

        // Token: 0x17000347 RID: 839
        // (get) Token: 0x0600355C RID: 13660 RVA: 0x00133D73 File Offset: 0x00132173
        public override bool RequiresOldValue => true;

        // Token: 0x17000348 RID: 840
        // (get) Token: 0x0600355D RID: 13661 RVA: 0x00133D76 File Offset: 0x00132176
        public override bool ReturnsValue => false;

        // Token: 0x0600355E RID: 13662 RVA: 0x00133D79 File Offset: 0x00132179
        public override void Write(object value, ProtoWriter dest)
		{
			value = field.GetValue(value);
			if (value != null)
			{
				Tail.Write(value, dest);
			}
		}

		// Token: 0x0600355F RID: 13663 RVA: 0x00133D9C File Offset: 0x0013219C
		public override object Read(object value, ProtoReader source)
		{
			object obj = Tail.Read((!Tail.RequiresOldValue) ? null : field.GetValue(value), source);
			if (obj != null)
			{
				field.SetValue(value, obj);
			}
			return null;
		}

		// Token: 0x040024C1 RID: 9409
		private readonly FieldInfo field;

		// Token: 0x040024C2 RID: 9410
		private readonly Type forType;
	}
}
