using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	// Token: 0x0200040D RID: 1037
	internal interface IProtoTypeSerializer : IProtoSerializer
	{
		// Token: 0x0600356C RID: 13676
		bool HasCallbacks(TypeModel.CallbackType callbackType);

		// Token: 0x0600356D RID: 13677
		bool CanCreateInstance();

		// Token: 0x0600356E RID: 13678
		object CreateInstance(ProtoReader source);

		// Token: 0x0600356F RID: 13679
		void Callback(object value, TypeModel.CallbackType callbackType, SerializationContext context);
	}
}
