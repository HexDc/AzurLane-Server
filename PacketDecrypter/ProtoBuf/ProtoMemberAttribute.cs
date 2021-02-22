using System;
using System.Reflection;

namespace ProtoBuf
{
	// Token: 0x020003F5 RID: 1013
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class ProtoMemberAttribute : Attribute, IComparable, IComparable<ProtoMemberAttribute>
	{
		// Token: 0x06003455 RID: 13397 RVA: 0x0012FB03 File Offset: 0x0012DF03
		public ProtoMemberAttribute(int tag) : this(tag, false)
		{
		}

		// Token: 0x06003456 RID: 13398 RVA: 0x0012FB0D File Offset: 0x0012DF0D
		internal ProtoMemberAttribute(int tag, bool forced)
		{
			if (tag <= 0 && !forced)
			{
				throw new ArgumentOutOfRangeException("tag");
			}
			this.tag = tag;
		}

		// Token: 0x06003457 RID: 13399 RVA: 0x0012FB34 File Offset: 0x0012DF34
		public int CompareTo(object other)
		{
			return this.CompareTo(other as ProtoMemberAttribute);
		}

		// Token: 0x06003458 RID: 13400 RVA: 0x0012FB44 File Offset: 0x0012DF44
		public int CompareTo(ProtoMemberAttribute other)
		{
			if (other == null)
			{
				return -1;
			}
			if (this == other)
			{
				return 0;
			}
			int num = this.tag.CompareTo(other.tag);
			if (num == 0)
			{
				num = string.CompareOrdinal(this.name, other.name);
			}
			return num;
		}

		// Token: 0x1700030F RID: 783
		// (get) Token: 0x06003459 RID: 13401 RVA: 0x0012FB8D File Offset: 0x0012DF8D
		// (set) Token: 0x0600345A RID: 13402 RVA: 0x0012FB95 File Offset: 0x0012DF95
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

		// Token: 0x17000310 RID: 784
		// (get) Token: 0x0600345B RID: 13403 RVA: 0x0012FB9E File Offset: 0x0012DF9E
		// (set) Token: 0x0600345C RID: 13404 RVA: 0x0012FBA6 File Offset: 0x0012DFA6
		public DataFormat DataFormat
		{
			get
			{
				return this.dataFormat;
			}
			set
			{
				this.dataFormat = value;
			}
		}

		// Token: 0x17000311 RID: 785
		// (get) Token: 0x0600345D RID: 13405 RVA: 0x0012FBAF File Offset: 0x0012DFAF
		public int Tag
		{
			get
			{
				return this.tag;
			}
		}

		// Token: 0x0600345E RID: 13406 RVA: 0x0012FBB7 File Offset: 0x0012DFB7
		internal void Rebase(int tag)
		{
			this.tag = tag;
		}

		// Token: 0x17000312 RID: 786
		// (get) Token: 0x0600345F RID: 13407 RVA: 0x0012FBC0 File Offset: 0x0012DFC0
		// (set) Token: 0x06003460 RID: 13408 RVA: 0x0012FBCD File Offset: 0x0012DFCD
		public bool IsRequired
		{
			get
			{
				return (this.options & MemberSerializationOptions.Required) == MemberSerializationOptions.Required;
			}
			set
			{
				if (value)
				{
					this.options |= MemberSerializationOptions.Required;
				}
				else
				{
					this.options &= ~MemberSerializationOptions.Required;
				}
			}
		}

		// Token: 0x17000313 RID: 787
		// (get) Token: 0x06003461 RID: 13409 RVA: 0x0012FBF7 File Offset: 0x0012DFF7
		// (set) Token: 0x06003462 RID: 13410 RVA: 0x0012FC04 File Offset: 0x0012E004
		public bool IsPacked
		{
			get
			{
				return (this.options & MemberSerializationOptions.Packed) == MemberSerializationOptions.Packed;
			}
			set
			{
				if (value)
				{
					this.options |= MemberSerializationOptions.Packed;
				}
				else
				{
					this.options &= ~MemberSerializationOptions.Packed;
				}
			}
		}

		// Token: 0x17000314 RID: 788
		// (get) Token: 0x06003463 RID: 13411 RVA: 0x0012FC2E File Offset: 0x0012E02E
		// (set) Token: 0x06003464 RID: 13412 RVA: 0x0012FC3D File Offset: 0x0012E03D
		public bool OverwriteList
		{
			get
			{
				return (this.options & MemberSerializationOptions.OverwriteList) == MemberSerializationOptions.OverwriteList;
			}
			set
			{
				if (value)
				{
					this.options |= MemberSerializationOptions.OverwriteList;
				}
				else
				{
					this.options &= ~MemberSerializationOptions.OverwriteList;
				}
			}
		}

		// Token: 0x17000315 RID: 789
		// (get) Token: 0x06003465 RID: 13413 RVA: 0x0012FC68 File Offset: 0x0012E068
		// (set) Token: 0x06003466 RID: 13414 RVA: 0x0012FC75 File Offset: 0x0012E075
		public bool AsReference
		{
			get
			{
				return (this.options & MemberSerializationOptions.AsReference) == MemberSerializationOptions.AsReference;
			}
			set
			{
				if (value)
				{
					this.options |= MemberSerializationOptions.AsReference;
				}
				else
				{
					this.options &= ~MemberSerializationOptions.AsReference;
				}
				this.options |= MemberSerializationOptions.AsReferenceHasValue;
			}
		}

		// Token: 0x17000316 RID: 790
		// (get) Token: 0x06003467 RID: 13415 RVA: 0x0012FCAE File Offset: 0x0012E0AE
		// (set) Token: 0x06003468 RID: 13416 RVA: 0x0012FCBD File Offset: 0x0012E0BD
		internal bool AsReferenceHasValue
		{
			get
			{
				return (this.options & MemberSerializationOptions.AsReferenceHasValue) == MemberSerializationOptions.AsReferenceHasValue;
			}
			set
			{
				if (value)
				{
					this.options |= MemberSerializationOptions.AsReferenceHasValue;
				}
				else
				{
					this.options &= ~MemberSerializationOptions.AsReferenceHasValue;
				}
			}
		}

		// Token: 0x17000317 RID: 791
		// (get) Token: 0x06003469 RID: 13417 RVA: 0x0012FCE8 File Offset: 0x0012E0E8
		// (set) Token: 0x0600346A RID: 13418 RVA: 0x0012FCF5 File Offset: 0x0012E0F5
		public bool DynamicType
		{
			get
			{
				return (this.options & MemberSerializationOptions.DynamicType) == MemberSerializationOptions.DynamicType;
			}
			set
			{
				if (value)
				{
					this.options |= MemberSerializationOptions.DynamicType;
				}
				else
				{
					this.options &= ~MemberSerializationOptions.DynamicType;
				}
			}
		}

		// Token: 0x17000318 RID: 792
		// (get) Token: 0x0600346B RID: 13419 RVA: 0x0012FD1F File Offset: 0x0012E11F
		// (set) Token: 0x0600346C RID: 13420 RVA: 0x0012FD27 File Offset: 0x0012E127
		public MemberSerializationOptions Options
		{
			get
			{
				return this.options;
			}
			set
			{
				this.options = value;
			}
		}

		// Token: 0x04002470 RID: 9328
		internal MemberInfo Member;

		// Token: 0x04002471 RID: 9329
		internal bool TagIsPinned;

		// Token: 0x04002472 RID: 9330
		private string name;

		// Token: 0x04002473 RID: 9331
		private DataFormat dataFormat;

		// Token: 0x04002474 RID: 9332
		private int tag;

		// Token: 0x04002475 RID: 9333
		private MemberSerializationOptions options;
	}
}
