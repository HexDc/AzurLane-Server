using System;
using System.ComponentModel;

namespace ProtoBuf
{
	// Token: 0x020003C7 RID: 967
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	[ImmutableObject(true)]
	public sealed class ProtoAfterDeserializationAttribute : Attribute
	{
	}
}
