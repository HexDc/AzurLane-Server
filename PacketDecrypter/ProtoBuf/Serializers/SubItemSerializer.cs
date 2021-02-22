using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
    // Token: 0x0200041D RID: 1053
    internal sealed class SubItemSerializer : IProtoTypeSerializer, IProtoSerializer
	{
		// Token: 0x060035DA RID: 13786 RVA: 0x00135184 File Offset: 0x00133584
		public SubItemSerializer(Type type, int key, ISerializerProxy proxy, bool recursionCheck)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (proxy == null)
			{
				throw new ArgumentNullException("proxy");
			}
			this.type = type;
			this.proxy = proxy;
			this.key = key;
			this.recursionCheck = recursionCheck;
		}

		// Token: 0x060035DB RID: 13787 RVA: 0x001351D6 File Offset: 0x001335D6
		bool IProtoTypeSerializer.HasCallbacks(TypeModel.CallbackType callbackType)
		{
			return ((IProtoTypeSerializer)proxy.Serializer).HasCallbacks(callbackType);
		}

		// Token: 0x060035DC RID: 13788 RVA: 0x001351EE File Offset: 0x001335EE
		bool IProtoTypeSerializer.CanCreateInstance()
		{
			return ((IProtoTypeSerializer)proxy.Serializer).CanCreateInstance();
		}

		// Token: 0x060035DD RID: 13789 RVA: 0x00135205 File Offset: 0x00133605
		void IProtoTypeSerializer.Callback(object value, TypeModel.CallbackType callbackType, SerializationContext context)
		{
			((IProtoTypeSerializer)proxy.Serializer).Callback(value, callbackType, context);
		}

		// Token: 0x060035DE RID: 13790 RVA: 0x0013521F File Offset: 0x0013361F
		object IProtoTypeSerializer.CreateInstance(ProtoReader source)
		{
			return ((IProtoTypeSerializer)proxy.Serializer).CreateInstance(source);
		}

        // Token: 0x1700037F RID: 895
        // (get) Token: 0x060035DF RID: 13791 RVA: 0x00135237 File Offset: 0x00133637
        Type IProtoSerializer.ExpectedType => type;

        // Token: 0x17000380 RID: 896
        // (get) Token: 0x060035E0 RID: 13792 RVA: 0x0013523F File Offset: 0x0013363F
        bool IProtoSerializer.RequiresOldValue => true;

        // Token: 0x17000381 RID: 897
        // (get) Token: 0x060035E1 RID: 13793 RVA: 0x00135242 File Offset: 0x00133642
        bool IProtoSerializer.ReturnsValue => true;

        // Token: 0x060035E2 RID: 13794 RVA: 0x00135245 File Offset: 0x00133645
        void IProtoSerializer.Write(object value, ProtoWriter dest)
		{
			if (recursionCheck)
			{
				ProtoWriter.WriteObject(value, key, dest);
			}
			else
			{
				ProtoWriter.WriteRecursionSafeObject(value, key, dest);
			}
		}

		// Token: 0x060035E3 RID: 13795 RVA: 0x00135271 File Offset: 0x00133671
		object IProtoSerializer.Read(object value, ProtoReader source)
		{
			return ProtoReader.ReadObject(value, key, source);
		}

		// Token: 0x040024E9 RID: 9449
		private readonly int key;

		// Token: 0x040024EA RID: 9450
		private readonly Type type;

		// Token: 0x040024EB RID: 9451
		private readonly ISerializerProxy proxy;

		// Token: 0x040024EC RID: 9452
		private readonly bool recursionCheck;
	}
}
