using System;
using System.ComponentModel;
using ProtoBuf.Meta;

namespace ProtoBuf
{
	// Token: 0x020003F4 RID: 1012
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
	public sealed class ProtoIncludeAttribute : Attribute
	{
		// Token: 0x0600344E RID: 13390 RVA: 0x0012FA61 File Offset: 0x0012DE61
		public ProtoIncludeAttribute(int tag, Type knownType) : this(tag, (knownType != null) ? knownType.AssemblyQualifiedName : string.Empty)
		{
		}

		// Token: 0x0600344F RID: 13391 RVA: 0x0012FA80 File Offset: 0x0012DE80
		public ProtoIncludeAttribute(int tag, string knownTypeName)
		{
			if (tag <= 0)
			{
				throw new ArgumentOutOfRangeException("tag", "Tags must be positive integers");
			}
			if (Helpers.IsNullOrEmpty(knownTypeName))
			{
				throw new ArgumentNullException("knownTypeName", "Known type cannot be blank");
			}
			this.tag = tag;
			this.knownTypeName = knownTypeName;
		}

		// Token: 0x1700030B RID: 779
		// (get) Token: 0x06003450 RID: 13392 RVA: 0x0012FAD3 File Offset: 0x0012DED3
		public int Tag
		{
			get
			{
				return this.tag;
			}
		}

		// Token: 0x1700030C RID: 780
		// (get) Token: 0x06003451 RID: 13393 RVA: 0x0012FADB File Offset: 0x0012DEDB
		public string KnownTypeName
		{
			get
			{
				return this.knownTypeName;
			}
		}

		// Token: 0x1700030D RID: 781
		// (get) Token: 0x06003452 RID: 13394 RVA: 0x0012FAE3 File Offset: 0x0012DEE3
		public Type KnownType
		{
			get
			{
				return TypeModel.ResolveKnownType(this.KnownTypeName, null, null);
			}
		}

		// Token: 0x1700030E RID: 782
		// (get) Token: 0x06003453 RID: 13395 RVA: 0x0012FAF2 File Offset: 0x0012DEF2
		// (set) Token: 0x06003454 RID: 13396 RVA: 0x0012FAFA File Offset: 0x0012DEFA
		[DefaultValue(DataFormat.Default)]
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

		// Token: 0x0400246D RID: 9325
		private readonly int tag;

		// Token: 0x0400246E RID: 9326
		private readonly string knownTypeName;

		// Token: 0x0400246F RID: 9327
		private DataFormat dataFormat;
	}
}
