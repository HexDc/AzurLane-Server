using System;
using System.ComponentModel;

namespace ProtoBuf
{
	// Token: 0x020003C6 RID: 966
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	[ImmutableObject(true)]
	public sealed class ProtoBeforeDeserializationAttribute : Attribute
	{
	}
}
