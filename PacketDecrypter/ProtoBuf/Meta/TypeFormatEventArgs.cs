using System;

namespace ProtoBuf.Meta
{
	// Token: 0x020003E3 RID: 995
	public class TypeFormatEventArgs : EventArgs
	{
		// Token: 0x060033A4 RID: 13220 RVA: 0x0012E72F File Offset: 0x0012CB2F
		internal TypeFormatEventArgs(string formattedName)
		{
			if (Helpers.IsNullOrEmpty(formattedName))
			{
				throw new ArgumentNullException("formattedName");
			}
			this.formattedName = formattedName;
		}

		// Token: 0x060033A5 RID: 13221 RVA: 0x0012E754 File Offset: 0x0012CB54
		internal TypeFormatEventArgs(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			this.type = type;
			this.typeFixed = true;
		}

		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x060033A6 RID: 13222 RVA: 0x0012E77B File Offset: 0x0012CB7B
		// (set) Token: 0x060033A7 RID: 13223 RVA: 0x0012E783 File Offset: 0x0012CB83
		public Type Type
		{
			get
			{
				return this.type;
			}
			set
			{
				if (this.type != value)
				{
					if (this.typeFixed)
					{
						throw new InvalidOperationException("The type is fixed and cannot be changed");
					}
					this.type = value;
				}
			}
		}

		// Token: 0x170002E7 RID: 743
		// (get) Token: 0x060033A8 RID: 13224 RVA: 0x0012E7AE File Offset: 0x0012CBAE
		// (set) Token: 0x060033A9 RID: 13225 RVA: 0x0012E7B6 File Offset: 0x0012CBB6
		public string FormattedName
		{
			get
			{
				return this.formattedName;
			}
			set
			{
				if (this.formattedName != value)
				{
					if (!this.typeFixed)
					{
						throw new InvalidOperationException("The formatted-name is fixed and cannot be changed");
					}
					this.formattedName = value;
				}
			}
		}

		// Token: 0x04002427 RID: 9255
		private Type type;

		// Token: 0x04002428 RID: 9256
		private string formattedName;

		// Token: 0x04002429 RID: 9257
		private readonly bool typeFixed;
	}
}
