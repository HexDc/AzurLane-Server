using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
    // Token: 0x02000416 RID: 1046
    internal sealed class NullDecorator : ProtoDecoratorBase
	{
		// Token: 0x060035A8 RID: 13736 RVA: 0x00134C58 File Offset: 0x00133058
		public NullDecorator(TypeModel model, IProtoSerializer tail) : base(tail)
		{
			if (!tail.ReturnsValue)
			{
				throw new NotSupportedException("NullDecorator only supports implementations that return values");
			}
			Type type = tail.ExpectedType;
			if (Helpers.IsValueType(type))
			{
				expectedType = model.MapType(typeof(Nullable<>)).MakeGenericType(new Type[]
				{
					type
				});
			}
			else
			{
				expectedType = type;
			}
		}

        // Token: 0x1700036A RID: 874
        // (get) Token: 0x060035A9 RID: 13737 RVA: 0x00134CC5 File Offset: 0x001330C5
        public override Type ExpectedType => expectedType;

        // Token: 0x1700036B RID: 875
        // (get) Token: 0x060035AA RID: 13738 RVA: 0x00134CCD File Offset: 0x001330CD
        public override bool ReturnsValue => true;

        // Token: 0x1700036C RID: 876
        // (get) Token: 0x060035AB RID: 13739 RVA: 0x00134CD0 File Offset: 0x001330D0
        public override bool RequiresOldValue => true;

        // Token: 0x060035AC RID: 13740 RVA: 0x00134CD4 File Offset: 0x001330D4
        public override object Read(object value, ProtoReader source)
		{
			SubItemToken token = ProtoReader.StartSubItem(source);
			int num;
			while ((num = source.ReadFieldHeader()) > 0)
			{
				if (num == 1)
				{
					value = Tail.Read(value, source);
				}
				else
				{
					source.SkipField();
				}
			}
			ProtoReader.EndSubItem(token, source);
			return value;
		}

		// Token: 0x060035AD RID: 13741 RVA: 0x00134D24 File Offset: 0x00133124
		public override void Write(object value, ProtoWriter dest)
		{
			SubItemToken token = ProtoWriter.StartSubItem(null, dest);
			if (value != null)
			{
				Tail.Write(value, dest);
			}
			ProtoWriter.EndSubItem(token, dest);
		}

		// Token: 0x040024DE RID: 9438
		private readonly Type expectedType;

		// Token: 0x040024DF RID: 9439
		public const int Tag = 1;
	}
}
