using System;

namespace ProtoBuf.Serializers
{
    // Token: 0x02000419 RID: 1049
    internal abstract class ProtoDecoratorBase : IProtoSerializer
	{
		// Token: 0x060035BF RID: 13759 RVA: 0x001333D8 File Offset: 0x001317D8
		protected ProtoDecoratorBase(IProtoSerializer tail)
		{
			Tail = tail;
		}

		// Token: 0x17000373 RID: 883
		// (get) Token: 0x060035C0 RID: 13760
		public abstract Type ExpectedType { get; }

		// Token: 0x17000374 RID: 884
		// (get) Token: 0x060035C1 RID: 13761
		public abstract bool ReturnsValue { get; }

		// Token: 0x17000375 RID: 885
		// (get) Token: 0x060035C2 RID: 13762
		public abstract bool RequiresOldValue { get; }

		// Token: 0x060035C3 RID: 13763
		public abstract void Write(object value, ProtoWriter dest);

		// Token: 0x060035C4 RID: 13764
		public abstract object Read(object value, ProtoReader source);

		// Token: 0x040024E5 RID: 9445
		protected readonly IProtoSerializer Tail;
	}
}
