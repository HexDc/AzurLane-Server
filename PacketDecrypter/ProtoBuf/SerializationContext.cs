using System;

namespace ProtoBuf
{
	// Token: 0x020003FA RID: 1018
	public sealed class SerializationContext
	{
		// Token: 0x060034EE RID: 13550 RVA: 0x00133059 File Offset: 0x00131459
		static SerializationContext()
		{
			SerializationContext.@default.Freeze();
		}

		// Token: 0x060034F0 RID: 13552 RVA: 0x00133077 File Offset: 0x00131477
		internal void Freeze()
		{
			this.frozen = true;
		}

		// Token: 0x060034F1 RID: 13553 RVA: 0x00133080 File Offset: 0x00131480
		private void ThrowIfFrozen()
		{
			if (this.frozen)
			{
				throw new InvalidOperationException("The serialization-context cannot be changed once it is in use");
			}
		}

		// Token: 0x17000325 RID: 805
		// (get) Token: 0x060034F2 RID: 13554 RVA: 0x00133098 File Offset: 0x00131498
		// (set) Token: 0x060034F3 RID: 13555 RVA: 0x001330A0 File Offset: 0x001314A0
		public object Context
		{
			get
			{
				return this.context;
			}
			set
			{
				if (this.context != value)
				{
					this.ThrowIfFrozen();
					this.context = value;
				}
			}
		}

		// Token: 0x17000326 RID: 806
		// (get) Token: 0x060034F4 RID: 13556 RVA: 0x001330BB File Offset: 0x001314BB
		internal static SerializationContext Default
		{
			get
			{
				return SerializationContext.@default;
			}
		}

		// Token: 0x040024A5 RID: 9381
		private bool frozen;

		// Token: 0x040024A6 RID: 9382
		private object context;

		// Token: 0x040024A7 RID: 9383
		private static readonly SerializationContext @default = new SerializationContext();
	}
}
