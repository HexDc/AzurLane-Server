using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

//패치 완료
//https://github.com/RedpointGames/protobuf-net/blob/master/protobuf-net/Helpers.cs
namespace ProtoBuf
{
	// Token: 0x020003CB RID: 971
	internal sealed class Helpers
	{
		// Token: 0x06003294 RID: 12948 RVA: 0x001279EE File Offset: 0x00125DEE
		private Helpers()
		{
		}

		// Token: 0x06003295 RID: 12949 RVA: 0x001279F6 File Offset: 0x00125DF6
		public static StringBuilder AppendLine(StringBuilder builder)
		{
			return builder.AppendLine();
		}

		// Token: 0x06003296 RID: 12950 RVA: 0x001279FE File Offset: 0x00125DFE
		public static bool IsNullOrEmpty(string value)
		{
			return value == null || value.Length == 0;
		}

		// Token: 0x06003297 RID: 12951 RVA: 0x00127A12 File Offset: 0x00125E12
		[Conditional("DEBUG")]
		public static void DebugWriteLine(string message, object obj)
		{
		}

		// Token: 0x06003298 RID: 12952 RVA: 0x00127A14 File Offset: 0x00125E14
		[Conditional("DEBUG")]
		public static void DebugWriteLine(string message)
		{
		}

		// Token: 0x06003299 RID: 12953 RVA: 0x00127A16 File Offset: 0x00125E16
		[Conditional("TRACE")]
		public static void TraceWriteLine(string message)
		{
		}

		// Token: 0x0600329A RID: 12954 RVA: 0x00127A18 File Offset: 0x00125E18
		[Conditional("DEBUG")]
		public static void DebugAssert(bool condition, string message)
		{
		}

		// Token: 0x0600329B RID: 12955 RVA: 0x00127A1A File Offset: 0x00125E1A
		[Conditional("DEBUG")]
		public static void DebugAssert(bool condition, string message, params object[] args)
		{
		}

		// Token: 0x0600329C RID: 12956 RVA: 0x00127A1C File Offset: 0x00125E1C
		[Conditional("DEBUG")]
		public static void DebugAssert(bool condition)
		{
		}

		// Token: 0x0600329D RID: 12957 RVA: 0x00127A20 File Offset: 0x00125E20
		public static void Sort(int[] keys, object[] values)
		{
			bool flag;
			do
			{
				flag = false;
				for (int i = 1; i < keys.Length; i++)
				{
					if (keys[i - 1] > keys[i])
					{
						int num = keys[i];
						keys[i] = keys[i - 1];
						keys[i - 1] = num;
						object obj = values[i];
						values[i] = values[i - 1];
						values[i - 1] = obj;
						flag = true;
					}
				}
			}
			while (flag);
		}

		// Token: 0x0600329E RID: 12958 RVA: 0x00127A7C File Offset: 0x00125E7C
		public static void BlockCopy(byte[] from, int fromIndex, byte[] to, int toIndex, int count)
		{
			Buffer.BlockCopy(from, fromIndex, to, toIndex, count);
		}

		// Token: 0x0600329F RID: 12959 RVA: 0x00127A89 File Offset: 0x00125E89
		public static bool IsInfinity(float value)
		{
			return float.IsInfinity(value);
		}

		// Token: 0x060032A0 RID: 12960 RVA: 0x00127A91 File Offset: 0x00125E91
		internal static MethodInfo GetInstanceMethod(Type declaringType, string name)
		{
			return declaringType.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}

		// Token: 0x060032A1 RID: 12961 RVA: 0x00127A9C File Offset: 0x00125E9C
		internal static MethodInfo GetStaticMethod(Type declaringType, string name)
		{
			return declaringType.GetMethod(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}

		// Token: 0x060032A2 RID: 12962 RVA: 0x00127AA7 File Offset: 0x00125EA7
		internal static MethodInfo GetStaticMethod(Type declaringType, string name, Type[] parameterTypes)
		{
			return declaringType.GetMethod(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, parameterTypes, null);
		}

		// Token: 0x060032A3 RID: 12963 RVA: 0x00127AB5 File Offset: 0x00125EB5
		internal static MethodInfo GetInstanceMethod(Type declaringType, string name, Type[] types)
		{
			if (types == null)
			{
				types = Helpers.EmptyTypes;
			}
			return declaringType.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, types, null);
		}

		// Token: 0x060032A4 RID: 12964 RVA: 0x00127AD0 File Offset: 0x00125ED0
		internal static bool IsSubclassOf(Type type, Type baseClass)
		{
			return type.IsSubclassOf(baseClass);
		}

		// Token: 0x060032A5 RID: 12965 RVA: 0x00127AD9 File Offset: 0x00125ED9
		public static bool IsInfinity(double value)
		{
			return double.IsInfinity(value);
		}

		// Token: 0x060032A6 RID: 12966 RVA: 0x00127AE4 File Offset: 0x00125EE4
		public static ProtoTypeCode GetTypeCode(Type type)
		{
			TypeCode typeCode = Type.GetTypeCode(type);
			switch (typeCode)
			{
			case TypeCode.Empty:
			case TypeCode.Boolean:
			case TypeCode.Char:
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
			case TypeCode.DateTime:
			case TypeCode.String:
				return (ProtoTypeCode)typeCode;
			}
			switch (type.FullName)
			{
				case "System.TimeSpan": return ProtoTypeCode.TimeSpan;
				case "System.Guid": return ProtoTypeCode.Guid;
				case "System.Uri": return ProtoTypeCode.Uri;
				case "System.Byte[]": return ProtoTypeCode.ByteArray;
				case "System.Type": return ProtoTypeCode.Type;
			}
			return ProtoTypeCode.Unknown;
		}

		// Token: 0x060032A7 RID: 12967 RVA: 0x00127BB1 File Offset: 0x00125FB1
		internal static Type GetUnderlyingType(Type type)
		{
			return Nullable.GetUnderlyingType(type);
		}

		// Token: 0x060032A8 RID: 12968 RVA: 0x00127BB9 File Offset: 0x00125FB9
		internal static bool IsValueType(Type type)
		{
			return type.IsValueType;
		}

		// Token: 0x060032A9 RID: 12969 RVA: 0x00127BC1 File Offset: 0x00125FC1
		internal static bool IsSealed(Type type)
		{
			return type.IsSealed;
		}

		// Token: 0x060032AA RID: 12970 RVA: 0x00127BC9 File Offset: 0x00125FC9
		internal static bool IsClass(Type type)
		{
			return type.IsClass;
		}

		// Token: 0x060032AB RID: 12971 RVA: 0x00127BD1 File Offset: 0x00125FD1
		internal static bool IsEnum(Type type)
		{
			return type.IsEnum;
		}

		// Token: 0x060032AC RID: 12972 RVA: 0x00127BDC File Offset: 0x00125FDC
		internal static MethodInfo GetGetMethod(PropertyInfo property, bool nonPublic, bool allowInternal)
		{
			if (property == null)
			{
				return null;
			}
			MethodInfo methodInfo = property.GetGetMethod(nonPublic);
			if (methodInfo == null && !nonPublic && allowInternal)
			{
				methodInfo = property.GetGetMethod(true);
				if (methodInfo == null && !methodInfo.IsAssembly && !methodInfo.IsFamilyOrAssembly)
				{
					methodInfo = null;
				}
			}
			return methodInfo;
		}

		// Token: 0x060032AD RID: 12973 RVA: 0x00127C34 File Offset: 0x00126034
		internal static MethodInfo GetSetMethod(PropertyInfo property, bool nonPublic, bool allowInternal)
		{
			if (property == null)
			{
				return null;
			}
			MethodInfo methodInfo = property.GetSetMethod(nonPublic);
			if (methodInfo == null && !nonPublic && allowInternal)
			{
				methodInfo = property.GetGetMethod(true);
				if (methodInfo == null && !methodInfo.IsAssembly && !methodInfo.IsFamilyOrAssembly)
				{
					methodInfo = null;
				}
			}
			return methodInfo;
		}

		// Token: 0x060032AE RID: 12974 RVA: 0x00127C8A File Offset: 0x0012608A
		internal static ConstructorInfo GetConstructor(Type type, Type[] parameterTypes, bool nonPublic)
		{
			return type.GetConstructor((!nonPublic) ? (BindingFlags.Instance | BindingFlags.Public) : (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic), null, parameterTypes, null);
		}

		// Token: 0x060032AF RID: 12975 RVA: 0x00127CA4 File Offset: 0x001260A4
		internal static ConstructorInfo[] GetConstructors(Type type, bool nonPublic)
		{
			return type.GetConstructors((!nonPublic) ? (BindingFlags.Instance | BindingFlags.Public) : (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
		}

		// Token: 0x060032B0 RID: 12976 RVA: 0x00127CBB File Offset: 0x001260BB
		internal static PropertyInfo GetProperty(Type type, string name, bool nonPublic)
		{
			return type.GetProperty(name, (!nonPublic) ? (BindingFlags.Instance | BindingFlags.Public) : (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
		}

		// Token: 0x060032B1 RID: 12977 RVA: 0x00127CD3 File Offset: 0x001260D3
		internal static object ParseEnum(Type type, string value)
		{
			return Enum.Parse(type, value, true);
		}

		// Token: 0x060032B2 RID: 12978 RVA: 0x00127CE0 File Offset: 0x001260E0
		internal static MemberInfo[] GetInstanceFieldsAndProperties(Type type, bool publicOnly)
		{
			BindingFlags bindingAttr = (!publicOnly) ? (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) : (BindingFlags.Instance | BindingFlags.Public);
			PropertyInfo[] properties = type.GetProperties(bindingAttr);
			FieldInfo[] fields = type.GetFields(bindingAttr);
			MemberInfo[] array = new MemberInfo[fields.Length + properties.Length];
			properties.CopyTo(array, 0);
			fields.CopyTo(array, properties.Length);
			return array;
		}

		// Token: 0x060032B3 RID: 12979 RVA: 0x00127D30 File Offset: 0x00126130
		internal static Type GetMemberType(MemberInfo member)
		{
			MemberTypes memberType = member.MemberType;
			if (memberType == MemberTypes.Field)
			{
				return ((FieldInfo)member).FieldType;
			}
			if (memberType != MemberTypes.Property)
			{
				return null;
			}
			return ((PropertyInfo)member).PropertyType;
		}

		// Token: 0x060032B4 RID: 12980 RVA: 0x00127D71 File Offset: 0x00126171
		internal static bool IsAssignableFrom(Type target, Type type)
		{
			return target.IsAssignableFrom(type);
		}

		// Token: 0x060032B5 RID: 12981 RVA: 0x00127D7A File Offset: 0x0012617A
		internal static Assembly GetAssembly(Type type)
		{
			return type.Assembly;
		}

		// Token: 0x060032B6 RID: 12982 RVA: 0x00127D82 File Offset: 0x00126182
		internal static byte[] GetBuffer(MemoryStream ms)
		{
			return ms.GetBuffer();
		}

		// Token: 0x040023C7 RID: 9159
		public static readonly Type[] EmptyTypes = Type.EmptyTypes;
	}
}
