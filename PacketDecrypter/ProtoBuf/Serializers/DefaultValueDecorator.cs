using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
    // Token: 0x02000406 RID: 1030
    internal sealed class DefaultValueDecorator : ProtoDecoratorBase
	{
		// Token: 0x06003543 RID: 13635 RVA: 0x001338B8 File Offset: 0x00131CB8
		public DefaultValueDecorator(TypeModel model, object defaultValue, IProtoSerializer tail) : base(tail)
		{
			if (defaultValue == null)
			{
				throw new ArgumentNullException("defaultValue");
			}
			Type type = model.MapType(defaultValue.GetType());
			if (type != tail.ExpectedType)
			{
				throw new ArgumentException("Default value is of incorrect type", "defaultValue");
			}
			this.defaultValue = defaultValue;
		}

        // Token: 0x1700033D RID: 829
        // (get) Token: 0x06003544 RID: 13636 RVA: 0x0013390D File Offset: 0x00131D0D
        public override Type ExpectedType => Tail.ExpectedType;

        // Token: 0x1700033E RID: 830
        // (get) Token: 0x06003545 RID: 13637 RVA: 0x0013391A File Offset: 0x00131D1A
        public override bool RequiresOldValue => Tail.RequiresOldValue;

        // Token: 0x1700033F RID: 831
        // (get) Token: 0x06003546 RID: 13638 RVA: 0x00133927 File Offset: 0x00131D27
        public override bool ReturnsValue => Tail.ReturnsValue;

        // Token: 0x06003547 RID: 13639 RVA: 0x00133934 File Offset: 0x00131D34
        public override void Write(object value, ProtoWriter dest)
		{
			if (!object.Equals(value, defaultValue))
			{
				Tail.Write(value, dest);
			}
		}

		// Token: 0x06003548 RID: 13640 RVA: 0x00133954 File Offset: 0x00131D54
		public override object Read(object value, ProtoReader source)
		{
			return Tail.Read(value, source);
		}

		// Token: 0x040024BA RID: 9402
		private readonly object defaultValue;
	}
}
