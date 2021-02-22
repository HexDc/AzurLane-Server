using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
    // Token: 0x02000415 RID: 1045
    internal sealed class NetObjectSerializer : IProtoSerializer
	{
		// Token: 0x060035A2 RID: 13730 RVA: 0x00134BA0 File Offset: 0x00132FA0
		public NetObjectSerializer(TypeModel model, Type type, int key, BclHelpers.NetObjectOptions options)
		{
			bool flag = (byte)(options & BclHelpers.NetObjectOptions.DynamicType) != 0;
			this.key = ((!flag) ? key : -1);
			this.type = ((!flag) ? type : model.MapType(typeof(object)));
			this.options = options;
		}

        // Token: 0x17000367 RID: 871
        // (get) Token: 0x060035A3 RID: 13731 RVA: 0x00134BFC File Offset: 0x00132FFC
        public Type ExpectedType => type;

        // Token: 0x17000368 RID: 872
        // (get) Token: 0x060035A4 RID: 13732 RVA: 0x00134C04 File Offset: 0x00133004
        public bool ReturnsValue => true;

        // Token: 0x17000369 RID: 873
        // (get) Token: 0x060035A5 RID: 13733 RVA: 0x00134C07 File Offset: 0x00133007
        public bool RequiresOldValue => true;

        // Token: 0x060035A6 RID: 13734 RVA: 0x00134C0A File Offset: 0x0013300A
        public object Read(object value, ProtoReader source)
		{
			return BclHelpers.ReadNetObject(value, source, key, (type != typeof(object)) ? type : null, options);
		}

		// Token: 0x060035A7 RID: 13735 RVA: 0x00134C40 File Offset: 0x00133040
		public void Write(object value, ProtoWriter dest)
		{
			BclHelpers.WriteNetObject(value, dest, key, options);
		}

		// Token: 0x040024DB RID: 9435
		private readonly int key;

		// Token: 0x040024DC RID: 9436
		private readonly Type type;

		// Token: 0x040024DD RID: 9437
		private readonly BclHelpers.NetObjectOptions options;
	}
}
