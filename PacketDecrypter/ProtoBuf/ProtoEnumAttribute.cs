using System;

namespace ProtoBuf
{
	// Token: 0x020003F0 RID: 1008
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class ProtoEnumAttribute : Attribute
	{
		// Token: 0x17000308 RID: 776
		// (get) Token: 0x06003443 RID: 13379 RVA: 0x0012F9E0 File Offset: 0x0012DDE0
		// (set) Token: 0x06003444 RID: 13380 RVA: 0x0012F9E8 File Offset: 0x0012DDE8
		public int Value
		{
			get
			{
				return this.enumValue;
			}
			set
			{
				this.enumValue = value;
				this.hasValue = true;
			}
		}

		// Token: 0x06003445 RID: 13381 RVA: 0x0012F9F8 File Offset: 0x0012DDF8
		public bool HasValue()
		{
			return this.hasValue;
		}

		// Token: 0x17000309 RID: 777
		// (get) Token: 0x06003446 RID: 13382 RVA: 0x0012FA00 File Offset: 0x0012DE00
		// (set) Token: 0x06003447 RID: 13383 RVA: 0x0012FA08 File Offset: 0x0012DE08
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		// Token: 0x04002469 RID: 9321
		private bool hasValue;

		// Token: 0x0400246A RID: 9322
		private int enumValue;

		// Token: 0x0400246B RID: 9323
		private string name;
	}
}
