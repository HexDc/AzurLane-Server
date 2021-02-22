using ProtoBuf.Meta;
using System;
using System.Reflection;

namespace ProtoBuf.Serializers
{
    // Token: 0x02000417 RID: 1047
    internal sealed class ParseableSerializer : IProtoSerializer
	{
		// Token: 0x060035AE RID: 13742 RVA: 0x00134D53 File Offset: 0x00133153
		private ParseableSerializer(MethodInfo parse)
		{
			this.parse = parse;
		}

		// Token: 0x060035AF RID: 13743 RVA: 0x00134D64 File Offset: 0x00133164
		public static ParseableSerializer TryCreate(Type type, TypeModel model)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			MethodInfo method = type.GetMethod("Parse", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public, null, new Type[]
			{
				model.MapType(typeof(string))
			}, null);
			if (method != null && method.ReturnType == type)
			{
				if (Helpers.IsValueType(type))
				{
					MethodInfo customToString = ParseableSerializer.GetCustomToString(type);
					if (customToString == null || customToString.ReturnType != model.MapType(typeof(string)))
					{
						return null;
					}
				}
				return new ParseableSerializer(method);
			}
			return null;
		}

		// Token: 0x060035B0 RID: 13744 RVA: 0x00134DFA File Offset: 0x001331FA
		private static MethodInfo GetCustomToString(Type type)
		{
			return type.GetMethod("ToString", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public, null, Helpers.EmptyTypes, null);
		}

        // Token: 0x1700036F RID: 879
        // (get) Token: 0x060035B1 RID: 13745 RVA: 0x00134E10 File Offset: 0x00133210
        public Type ExpectedType => parse.DeclaringType;

        // Token: 0x1700036D RID: 877
        // (get) Token: 0x060035B2 RID: 13746 RVA: 0x00134E1D File Offset: 0x0013321D
        bool IProtoSerializer.RequiresOldValue => false;

        // Token: 0x1700036E RID: 878
        // (get) Token: 0x060035B3 RID: 13747 RVA: 0x00134E20 File Offset: 0x00133220
        bool IProtoSerializer.ReturnsValue => true;

        // Token: 0x060035B4 RID: 13748 RVA: 0x00134E23 File Offset: 0x00133223
        public object Read(object value, ProtoReader source)
		{
			return parse.Invoke(null, new object[]
			{
				source.ReadString()
			});
		}

		// Token: 0x060035B5 RID: 13749 RVA: 0x00134E40 File Offset: 0x00133240
		public void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteString(value.ToString(), dest);
		}

		// Token: 0x040024E0 RID: 9440
		private readonly MethodInfo parse;
	}
}
