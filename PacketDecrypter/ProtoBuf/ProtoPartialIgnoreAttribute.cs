using System;

namespace ProtoBuf
{
	// Token: 0x020003F3 RID: 1011
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public sealed class ProtoPartialIgnoreAttribute : ProtoIgnoreAttribute
	{
		// Token: 0x0600344C RID: 13388 RVA: 0x0012FA34 File Offset: 0x0012DE34
		public ProtoPartialIgnoreAttribute(string memberName)
		{
			if (Helpers.IsNullOrEmpty(memberName))
			{
				throw new ArgumentNullException("memberName");
			}
			this.memberName = memberName;
		}

		// Token: 0x1700030A RID: 778
		// (get) Token: 0x0600344D RID: 13389 RVA: 0x0012FA59 File Offset: 0x0012DE59
		public string MemberName
		{
			get
			{
				return this.memberName;
			}
		}

		// Token: 0x0400246C RID: 9324
		private readonly string memberName;
	}
}
