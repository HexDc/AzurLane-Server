using System;
//패치
//https://github.com/RedpointGames/protobuf-net/blob/master/protobuf-net/ProtoContractAttribute.cs
namespace ProtoBuf
{
	// Token: 0x020003EE RID: 1006
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
	public sealed class ProtoContractAttribute : Attribute
	{
		// Token: 0x170002FC RID: 764
		// (get) Token: 0x06003429 RID: 13353 RVA: 0x0012F89F File Offset: 0x0012DC9F
		// (set) Token: 0x0600342A RID: 13354 RVA: 0x0012F8A7 File Offset: 0x0012DCA7
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

		// Token: 0x170002FD RID: 765
		// (get) Token: 0x0600342B RID: 13355 RVA: 0x0012F8B0 File Offset: 0x0012DCB0
		// (set) Token: 0x0600342C RID: 13356 RVA: 0x0012F8B8 File Offset: 0x0012DCB8
		public int ImplicitFirstTag
		{
			get
			{
				return this.implicitFirstTag;
			}
			set
			{
				if (value < 1)
				{
					throw new ArgumentOutOfRangeException("ImplicitFirstTag");
				}
				this.implicitFirstTag = value;
			}
		}

		// Token: 0x170002FE RID: 766
		// (get) Token: 0x0600342D RID: 13357 RVA: 0x0012F8D3 File Offset: 0x0012DCD3
		// (set) Token: 0x0600342E RID: 13358 RVA: 0x0012F8DC File Offset: 0x0012DCDC
		public bool UseProtoMembersOnly
		{
			get
			{
				return this.HasFlag(4);
			}
			set
			{
				this.SetFlag(4, value);
			}
		}

		// Token: 0x170002FF RID: 767
		// (get) Token: 0x0600342F RID: 13359 RVA: 0x0012F8E6 File Offset: 0x0012DCE6
		// (set) Token: 0x06003430 RID: 13360 RVA: 0x0012F8F0 File Offset: 0x0012DCF0
		public bool IgnoreListHandling
		{
			get
			{
				return this.HasFlag(16);
			}
			set
			{
				this.SetFlag(16, value);
			}
		}

		// Token: 0x17000300 RID: 768
		// (get) Token: 0x06003431 RID: 13361 RVA: 0x0012F8FB File Offset: 0x0012DCFB
		// (set) Token: 0x06003432 RID: 13362 RVA: 0x0012F903 File Offset: 0x0012DD03
		public ImplicitFields ImplicitFields
		{
			get
			{
				return this.implicitFields;
			}
			set
			{
				this.implicitFields = value;
			}
		}

		// Token: 0x17000301 RID: 769
		// (get) Token: 0x06003433 RID: 13363 RVA: 0x0012F90C File Offset: 0x0012DD0C
		// (set) Token: 0x06003434 RID: 13364 RVA: 0x0012F915 File Offset: 0x0012DD15
		public bool InferTagFromName
		{
			get
			{
				return this.HasFlag(1);
			}
			set
			{
				this.SetFlag(1, value);
				this.SetFlag(2, true);
			}
		}

		// Token: 0x17000302 RID: 770
		// (get) Token: 0x06003435 RID: 13365 RVA: 0x0012F927 File Offset: 0x0012DD27
		internal bool InferTagFromNameHasValue
		{
			get
			{
				return this.HasFlag(2);
			}
		}

		// Token: 0x17000303 RID: 771
		// (get) Token: 0x06003436 RID: 13366 RVA: 0x0012F930 File Offset: 0x0012DD30
		// (set) Token: 0x06003437 RID: 13367 RVA: 0x0012F938 File Offset: 0x0012DD38
		public int DataMemberOffset
		{
			get
			{
				return this.dataMemberOffset;
			}
			set
			{
				this.dataMemberOffset = value;
			}
		}

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x06003438 RID: 13368 RVA: 0x0012F941 File Offset: 0x0012DD41
		// (set) Token: 0x06003439 RID: 13369 RVA: 0x0012F94A File Offset: 0x0012DD4A
		public bool SkipConstructor
		{
			get
			{
				return this.HasFlag(8);
			}
			set
			{
				this.SetFlag(8, value);
			}
		}

		// Token: 0x17000305 RID: 773
		// (get) Token: 0x0600343A RID: 13370 RVA: 0x0012F954 File Offset: 0x0012DD54
		// (set) Token: 0x0600343B RID: 13371 RVA: 0x0012F95E File Offset: 0x0012DD5E
		public bool AsReferenceDefault
		{
			get
			{
				return this.HasFlag(32);
			}
			set
			{
				this.SetFlag(32, value);
			}
		}

		// Token: 0x0600343C RID: 13372 RVA: 0x0012F969 File Offset: 0x0012DD69
		private bool HasFlag(byte flag)
		{
			return (this.flags & flag) == flag;
		}

		// Token: 0x0600343D RID: 13373 RVA: 0x0012F976 File Offset: 0x0012DD76

		private void SetFlag(byte flag, bool value)
		{
			if (value) flags |= flag;
			else flags = (byte)(flags & ~flag);
		}

		// Token: 0x17000306 RID: 774
		// (get) Token: 0x0600343E RID: 13374 RVA: 0x0012F9A2 File Offset: 0x0012DDA2
		// (set) Token: 0x0600343F RID: 13375 RVA: 0x0012F9AC File Offset: 0x0012DDAC
		public bool EnumPassthru
		{
			get
			{
				return this.HasFlag(64);
			}
			set
			{
				this.SetFlag(64, value);
				this.SetFlag(128, true);
			}
		}

		// Token: 0x17000307 RID: 775
		// (get) Token: 0x06003440 RID: 13376 RVA: 0x0012F9C3 File Offset: 0x0012DDC3
		internal bool EnumPassthruHasValue
		{
			get
			{
				return this.HasFlag(128);
			}
		}

		// Token: 0x0400245C RID: 9308
		private string name;

		// Token: 0x0400245D RID: 9309
		private int implicitFirstTag;

		// Token: 0x0400245E RID: 9310
		private ImplicitFields implicitFields;

		// Token: 0x0400245F RID: 9311
		private int dataMemberOffset;

		// Token: 0x04002460 RID: 9312
		private byte flags;

		// Token: 0x04002461 RID: 9313
		private const byte OPTIONS_InferTagFromName = 1;

		// Token: 0x04002462 RID: 9314
		private const byte OPTIONS_InferTagFromNameHasValue = 2;

		// Token: 0x04002463 RID: 9315
		private const byte OPTIONS_UseProtoMembersOnly = 4;

		// Token: 0x04002464 RID: 9316
		private const byte OPTIONS_SkipConstructor = 8;

		// Token: 0x04002465 RID: 9317
		private const byte OPTIONS_IgnoreListHandling = 16;

		// Token: 0x04002466 RID: 9318
		private const byte OPTIONS_AsReferenceDefault = 32;

		// Token: 0x04002467 RID: 9319
		private const byte OPTIONS_EnumPassthru = 64;

		// Token: 0x04002468 RID: 9320
		private const byte OPTIONS_EnumPassthruHasValue = 128;
	}
}
