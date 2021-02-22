using System;
using System.ComponentModel;

namespace ProtoBuf
{
	// Token: 0x020003C4 RID: 964
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	[ImmutableObject(true)]
	public sealed class ProtoBeforeSerializationAttribute : Attribute
	{
	}
}
