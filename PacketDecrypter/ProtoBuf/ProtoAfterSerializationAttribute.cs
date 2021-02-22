using System;
using System.ComponentModel;

namespace ProtoBuf
{
	// Token: 0x020003C5 RID: 965
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	[ImmutableObject(true)]
	public sealed class ProtoAfterSerializationAttribute : Attribute
	{
	}
}
