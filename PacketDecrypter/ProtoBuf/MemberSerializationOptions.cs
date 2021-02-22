using System;

namespace ProtoBuf
{
	// Token: 0x020003F6 RID: 1014
	[Flags]
	public enum MemberSerializationOptions
	{
		// Token: 0x04002477 RID: 9335
		None = 0,
		// Token: 0x04002478 RID: 9336
		Packed = 1,
		// Token: 0x04002479 RID: 9337
		Required = 2,
		// Token: 0x0400247A RID: 9338
		AsReference = 4,
		// Token: 0x0400247B RID: 9339
		DynamicType = 8,
		// Token: 0x0400247C RID: 9340
		OverwriteList = 16,
		// Token: 0x0400247D RID: 9341
		AsReferenceHasValue = 32
	}
}
