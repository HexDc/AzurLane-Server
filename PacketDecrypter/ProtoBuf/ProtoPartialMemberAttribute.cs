using System;

namespace ProtoBuf
{
	// Token: 0x020003F7 RID: 1015
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public sealed class ProtoPartialMemberAttribute : ProtoMemberAttribute
	{
		// Token: 0x0600346D RID: 13421 RVA: 0x0012FD30 File Offset: 0x0012E130
		public ProtoPartialMemberAttribute(int tag, string memberName) : base(tag)
		{
			if (Helpers.IsNullOrEmpty(memberName))
			{
				throw new ArgumentNullException("memberName");
			}
			this.memberName = memberName;
		}

		// Token: 0x17000319 RID: 793
		// (get) Token: 0x0600346E RID: 13422 RVA: 0x0012FD56 File Offset: 0x0012E156
		public string MemberName
		{
			get
			{
				return this.memberName;
			}
		}

		// Token: 0x0400247E RID: 9342
		private readonly string memberName;
	}
}
