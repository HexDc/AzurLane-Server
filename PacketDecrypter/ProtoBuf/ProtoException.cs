using System;

namespace ProtoBuf
{
	// Token: 0x020003F1 RID: 1009
	public class ProtoException : Exception
	{
		// Token: 0x06003448 RID: 13384 RVA: 0x0012FA11 File Offset: 0x0012DE11
		public ProtoException()
		{
		}

		// Token: 0x06003449 RID: 13385 RVA: 0x0012FA19 File Offset: 0x0012DE19
		public ProtoException(string message) : base(message)
		{
		}

		// Token: 0x0600344A RID: 13386 RVA: 0x0012FA22 File Offset: 0x0012DE22
		public ProtoException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
