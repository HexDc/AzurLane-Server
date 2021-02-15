// Decompiled with JetBrains decompiler
// Type: JsonFx.Json.TypeCoercionUtility
// Assembly: Assembly-CSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 18E1FC19-6B38-46B3-9D3F-DF1E9429ABBE
// Assembly location: C:\Users\Admin\Desktop\APK Easy Tool portable\1-Decompiled APKs\벽람항로_v1.4.61_apkpure.com\assets\bin\Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace JsonFx.Json
{
    internal class TypeCoercionUtility
    {
        private bool allowNullValueTypes = true;
        private const string ErrorNullValueType = "{0} does not accept null as a value";
        private const string ErrorDefaultCtor = "Only objects with default constructors can be deserialized. ({0})";
        private const string ErrorCannotInstantiate = "Interfaces, Abstract classes, and unsupported ValueTypes cannot be deserialized. ({0})";
        private Dictionary<Type, Dictionary<string, MemberInfo>> memberMapCache;

        private Dictionary<Type, Dictionary<string, MemberInfo>> MemberMapCache
        {
            get
            {
                if (memberMapCache == null)
                {
                    memberMapCache = new Dictionary<Type, Dictionary<string, MemberInfo>>();
                }

                return memberMapCache;
            }
        }

        public bool AllowNullValueTypes
        {
            get => allowNullValueTypes;
            set => allowNullValueTypes = value;
        }

        internal object ProcessTypeHint(
      IDictionary result,
      string typeInfo,
      out Type objectType,
      out Dictionary<string, MemberInfo> memberMap)
        {
            if (string.IsNullOrEmpty(typeInfo))
            {
                objectType = null;
                memberMap = null;
                return result;
            }
            Type type = Type.GetType(typeInfo, false);
            if (type == null)
            {
                objectType = null;
                memberMap = null;
                return result;
            }
            objectType = type;
            return CoerceType(type, result, out memberMap);
        }

        internal object InstantiateObject(Type objectType, out Dictionary<string, MemberInfo> memberMap)
        {
            if (objectType.IsInterface || objectType.IsAbstract || objectType.IsValueType)
            {
                throw new JsonTypeCoercionException(string.Format("Interfaces, Abstract classes, and unsupported ValueTypes cannot be deserialized. ({0})", objectType.FullName));
            }

            ConstructorInfo constructor = objectType.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
            {
                throw new JsonTypeCoercionException(string.Format("Only objects with default constructors can be deserialized. ({0})", objectType.FullName));
            }

            object obj;
            try
            {
                obj = constructor.Invoke(null);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw new JsonTypeCoercionException(ex.InnerException.Message, ex.InnerException);
                }

                throw new JsonTypeCoercionException("Error instantiating " + objectType.FullName, ex);
            }
            memberMap = !typeof(IDictionary).IsAssignableFrom(objectType) ? CreateMemberMap(objectType) : null;
            return obj;
        }

        private Dictionary<string, MemberInfo> CreateMemberMap(Type objectType)
        {
            if (MemberMapCache.ContainsKey(objectType))
            {
                return MemberMapCache[objectType];
            }

            Dictionary<string, MemberInfo> dictionary = new Dictionary<string, MemberInfo>();
            foreach (PropertyInfo property in objectType.GetProperties())
            {
                if (property.CanRead && property.CanWrite && !JsonIgnoreAttribute.IsJsonIgnore(property))
                {
                    string jsonName = JsonNameAttribute.GetJsonName(property);
                    if (string.IsNullOrEmpty(jsonName))
                    {
                        dictionary[property.Name] = property;
                    }
                    else
                    {
                        dictionary[jsonName] = property;
                    }
                }
            }
            foreach (FieldInfo field in objectType.GetFields())
            {
                if (field.IsPublic && !JsonIgnoreAttribute.IsJsonIgnore(field))
                {
                    string jsonName = JsonNameAttribute.GetJsonName(field);
                    if (string.IsNullOrEmpty(jsonName))
                    {
                        dictionary[field.Name] = field;
                    }
                    else
                    {
                        dictionary[jsonName] = field;
                    }
                }
            }
            MemberMapCache[objectType] = dictionary;
            return dictionary;
        }

        internal static Type GetMemberInfo(
          Dictionary<string, MemberInfo> memberMap,
          string memberName,
          out MemberInfo memberInfo)
        {
            if (memberMap != null && memberMap.ContainsKey(memberName))
            {
                memberInfo = memberMap[memberName];
                if (memberInfo is PropertyInfo)
                {
                    return ((PropertyInfo)memberInfo).PropertyType;
                }

                if (memberInfo is FieldInfo)
                {
                    return ((FieldInfo)memberInfo).FieldType;
                }
            }
            memberInfo = null;
            return null;
        }

        internal void SetMemberValue(
          object result,
          Type memberType,
          MemberInfo memberInfo,
          object value)
        {
            switch (memberInfo)
            {
                case PropertyInfo _:
                    ((PropertyInfo)memberInfo).SetValue(result, CoerceType(memberType, value), null);
                    break;
                case FieldInfo _:
                    ((FieldInfo)memberInfo).SetValue(result, CoerceType(memberType, value));
                    break;
            }
        }

        internal object CoerceType(Type targetType, object value)
        {
            bool flag = TypeCoercionUtility.IsNullable(targetType);
            if (value == null)
            {
                if (!allowNullValueTypes && targetType.IsValueType && !flag)
                {
                    throw new JsonTypeCoercionException(string.Format("{0} does not accept null as a value", targetType.FullName));
                }

                return value;
            }
            if (flag)
            {
                Type[] genericArguments = targetType.GetGenericArguments();
                if (genericArguments.Length == 1)
                {
                    targetType = genericArguments[0];
                }
            }
            Type type = value.GetType();
            if (targetType.IsAssignableFrom(type))
            {
                return value;
            }

            if (targetType.IsEnum)
            {
                if (value is string)
                {
                    if (!Enum.IsDefined(targetType, value))
                    {
                        foreach (FieldInfo field in targetType.GetFields())
                        {
                            string jsonName = JsonNameAttribute.GetJsonName(field);
                            if (((string)value).Equals(jsonName))
                            {
                                value = field.Name;
                                break;
                            }
                        }
                    }
                    return Enum.Parse(targetType, (string)value);
                }
                value = CoerceType(Enum.GetUnderlyingType(targetType), value);
                return Enum.ToObject(targetType, value);
            }
            if (value is IDictionary)
            {
                return CoerceType(targetType, (IDictionary)value, out Dictionary<string, MemberInfo> _);
            }

            if (typeof(IEnumerable).IsAssignableFrom(targetType) && typeof(IEnumerable).IsAssignableFrom(type))
            {
                return CoerceList(targetType, type, (IEnumerable)value);
            }

            if (value is string)
            {
                if (targetType == typeof(DateTime))
                {
                    if (DateTime.TryParse((string)value, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.RoundtripKind, out DateTime result))
                    {
                        return result;
                    }
                }
                else
                {
                    if (targetType == typeof(Guid))
                    {
                        return new Guid((string)value);
                    }

                    if (targetType == typeof(char))
                    {
                        if (((string)value).Length == 1)
                        {
                            return ((string)value)[0];
                        }
                    }
                    else if (targetType == typeof(Uri))
                    {
                        if (Uri.TryCreate((string)value, UriKind.RelativeOrAbsolute, out Uri result))
                        {
                            return result;
                        }
                    }
                    else if (targetType == typeof(Version))
                    {
                        return new Version((string)value);
                    }
                }
            }
            else if (targetType == typeof(TimeSpan))
            {
                return new TimeSpan((long)CoerceType(typeof(long), value));
            }

            TypeConverter converter1 = TypeDescriptor.GetConverter(targetType);
            if (converter1.CanConvertFrom(type))
            {
                return converter1.ConvertFrom(value);
            }

            TypeConverter converter2 = TypeDescriptor.GetConverter(type);
            if (converter2.CanConvertTo(targetType))
            {
                return converter2.ConvertTo(value, targetType);
            }

            try
            {
                return Convert.ChangeType(value, targetType);
            }
            catch (Exception ex)
            {
                throw new JsonTypeCoercionException(string.Format("Error converting {0} to {1}", value.GetType().FullName, targetType.FullName), ex);
            }
        }

        private object CoerceType(
          Type targetType,
          IDictionary value,
          out Dictionary<string, MemberInfo> memberMap)
        {
            object result = InstantiateObject(targetType, out memberMap);
            if (memberMap != null)
            {
                foreach (object key in value.Keys)
                {
                    Type memberInfo2 = TypeCoercionUtility.GetMemberInfo(memberMap, key as string, out MemberInfo memberInfo1);
                    SetMemberValue(result, memberInfo2, memberInfo1, value[key]);
                }
            }
            return result;
        }

        private object CoerceList(Type targetType, Type arrayType, IEnumerable value)
        {
            if (targetType.IsArray)
            {
                return CoerceArray(targetType.GetElementType(), value);
            }

            ConstructorInfo[] constructors = targetType.GetConstructors();
            ConstructorInfo constructorInfo1 = null;
            foreach (ConstructorInfo constructorInfo2 in constructors)
            {
                ParameterInfo[] parameters = constructorInfo2.GetParameters();
                if (parameters.Length == 0)
                {
                    constructorInfo1 = constructorInfo2;
                }
                else if (parameters.Length == 1)
                {
                    if (parameters[0].ParameterType.IsAssignableFrom(arrayType))
                    {
                        try
                        {
                            return constructorInfo2.Invoke(new object[1]
                            {
                 value
                            });
                        }
                        catch
                        {
                        }
                    }
                }
            }
            if (constructorInfo1 == null)
            {
                throw new JsonTypeCoercionException(string.Format("Only objects with default constructors can be deserialized. ({0})", targetType.FullName));
            }

            object obj1;
            try
            {
                obj1 = constructorInfo1.Invoke(null);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw new JsonTypeCoercionException(ex.InnerException.Message, ex.InnerException);
                }

                throw new JsonTypeCoercionException("Error instantiating " + targetType.FullName, ex);
            }
            MethodInfo method1 = targetType.GetMethod("AddRange");
            ParameterInfo[] parameters1 = method1?.GetParameters();
            Type type = parameters1 == null || parameters1.Length != 1 ? null : parameters1[0].ParameterType;
            if (type != null)
            {
                if (type.IsAssignableFrom(arrayType))
                {
                    try
                    {
                        method1.Invoke(obj1, new object[1]
                        {
               value
                        });
                    }
                    catch (TargetInvocationException ex)
                    {
                        if (ex.InnerException != null)
                        {
                            throw new JsonTypeCoercionException(ex.InnerException.Message, ex.InnerException);
                        }

                        throw new JsonTypeCoercionException("Error calling AddRange on " + targetType.FullName, ex);
                    }
                    return obj1;
                }
            }
            MethodInfo method2 = targetType.GetMethod("Add");
            ParameterInfo[] parameters2 = method2?.GetParameters();
            Type targetType1 = parameters2 == null || parameters2.Length != 1 ? null : parameters2[0].ParameterType;
            if (targetType1 != null)
            {
                foreach (object obj2 in value)
                {
                    try
                    {
                        method2.Invoke(obj1, new object[1]
                        {
              CoerceType(targetType1, obj2)
                        });
                    }
                    catch (TargetInvocationException ex)
                    {
                        if (ex.InnerException != null)
                        {
                            throw new JsonTypeCoercionException(ex.InnerException.Message, ex.InnerException);
                        }

                        throw new JsonTypeCoercionException("Error calling Add on " + targetType.FullName, ex);
                    }
                }
                return obj1;
            }
            try
            {
                return Convert.ChangeType(value, targetType);
            }
            catch (Exception ex)
            {
                throw new JsonTypeCoercionException(string.Format("Error converting {0} to {1}", value.GetType().FullName, targetType.FullName), ex);
            }
        }

        private Array CoerceArray(Type elementType, IEnumerable value)
        {
            ArrayList arrayList = new ArrayList();
            foreach (object obj in value)
            {
                arrayList.Add(CoerceType(elementType, obj));
            }

            return arrayList.ToArray(elementType);
        }

        private static bool IsNullable(Type type)
        {
            return type.IsGenericType && typeof(Nullable<>) == type.GetGenericTypeDefinition();
        }
    }
}
