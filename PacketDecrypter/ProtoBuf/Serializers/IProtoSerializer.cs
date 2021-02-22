using System;

namespace ProtoBuf.Serializers
{
	// Token: 0x0200040C RID: 1036
	internal interface IProtoSerializer
	{
		// Token: 0x1700034C RID: 844
		// (get) Token: 0x06003567 RID: 13671
		Type ExpectedType { get; }

		// Token: 0x06003568 RID: 13672
		void Write(object value, ProtoWriter dest);

		// Token: 0x06003569 RID: 13673
		object Read(object value, ProtoReader source);

		// Token: 0x1700034D RID: 845
		// (get) Token: 0x0600356A RID: 13674
		bool RequiresOldValue { get; }

		// Token: 0x1700034E RID: 846
		// (get) Token: 0x0600356B RID: 13675
		bool ReturnsValue { get; }
	}
}
