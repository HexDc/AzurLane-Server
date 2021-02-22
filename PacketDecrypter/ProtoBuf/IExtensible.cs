using System;

namespace ProtoBuf
{
	// Token: 0x020003CD RID: 973
	public interface IExtensible
	{
		// Token: 0x060032B8 RID: 12984
		IExtension GetExtensionObject(bool createIfMissing);
	}
}
