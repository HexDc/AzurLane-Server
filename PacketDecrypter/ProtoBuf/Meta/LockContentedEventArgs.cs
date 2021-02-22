using System;

namespace ProtoBuf.Meta
{
	// Token: 0x020003DF RID: 991
	public sealed class LockContentedEventArgs : EventArgs
	{
		// Token: 0x06003395 RID: 13205 RVA: 0x0012E5F1 File Offset: 0x0012C9F1
		internal LockContentedEventArgs(string ownerStackTrace)
		{
			this.ownerStackTrace = ownerStackTrace;
		}

		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x06003396 RID: 13206 RVA: 0x0012E600 File Offset: 0x0012CA00
		public string OwnerStackTrace
		{
			get
			{
				return this.ownerStackTrace;
			}
		}

		// Token: 0x04002421 RID: 9249
		private readonly string ownerStackTrace;
	}
}
