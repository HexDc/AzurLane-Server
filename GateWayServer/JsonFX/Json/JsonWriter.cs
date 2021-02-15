// Decompiled with JetBrains decompiler
// Type: JsonFx.Json.JsonWriter
// Assembly: Assembly-CSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 18E1FC19-6B38-46B3-9D3F-DF1E9429ABBE
// Assembly location: C:\Users\Admin\Desktop\APK Easy Tool portable\1-Decompiled APKs\벽람항로_v1.4.61_apkpure.com\assets\bin\Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace JsonFx.Json
{
    public class JsonWriter : IDisposable
    {
        public const string JsonMimeType = "application/json";
        public const string JsonFileExtension = ".json";
        private const string AnonymousTypePrefix = "<>f__AnonymousType";
        private const string ErrorMaxDepth = "The maxiumum depth of {0} was exceeded. Check for cycles in object graph.";
        private const string ErrorIDictionaryEnumerator = "Types which implement Generic IDictionary<TKey, TValue> must have an IEnumerator which implements IDictionaryEnumerator. ({0})";
        private readonly TextWriter Writer;
        private JsonWriterSettings settings;
        private int depth;

        public JsonWriter(TextWriter output)
          : this(output, new JsonWriterSettings())
        {
        }

        public JsonWriter(TextWriter output, JsonWriterSettings settings)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            Writer = output;
            this.settings = settings;
            Writer.NewLine = this.settings.NewLine;
        }

        public JsonWriter(Stream output)
          : this(output, new JsonWriterSettings())
        {
        }

        public JsonWriter(Stream output, JsonWriterSettings settings)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            Writer = new StreamWriter(output, Encoding.UTF8);
            this.settings = settings;
            Writer.NewLine = this.settings.NewLine;
        }

        public JsonWriter(string outputFileName)
          : this(outputFileName, new JsonWriterSettings())
        {
        }

        public JsonWriter(string outputFileName, JsonWriterSettings settings)
        {
            if (outputFileName == null)
            {
                throw new ArgumentNullException(nameof(outputFileName));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            Writer = new StreamWriter(new FileStream(outputFileName, FileMode.Create, FileAccess.Write, FileShare.Read), Encoding.UTF8);
            this.settings = settings;
            Writer.NewLine = this.settings.NewLine;
        }

        public JsonWriter(StringBuilder output)
          : this(output, new JsonWriterSettings())
        {
        }

        public JsonWriter(StringBuilder output, JsonWriterSettings settings)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            Writer = new StringWriter(output, CultureInfo.InvariantCulture);
            this.settings = settings;
            Writer.NewLine = this.settings.NewLine;
        }

        [Obsolete("This has been deprecated in favor of JsonWriterSettings object")]
        public string TypeHintName
        {
            get => settings.TypeHintName;
            set => settings.TypeHintName = value;
        }

        [Obsolete("This has been deprecated in favor of JsonWriterSettings object")]
        public bool PrettyPrint
        {
            get => settings.PrettyPrint;
            set => settings.PrettyPrint = value;
        }

        [Obsolete("This has been deprecated in favor of JsonWriterSettings object")]
        public string Tab
        {
            get => settings.Tab;
            set => settings.Tab = value;
        }

        [Obsolete("This has been deprecated in favor of JsonWriterSettings object")]
        public string NewLine
        {
            get => settings.NewLine;
            set
            {
                TextWriter writer = Writer;
                string str1 = value;
                settings.NewLine = str1;
                string str2 = str1;
                writer.NewLine = str2;
            }
        }

        protected int Depth => depth;

        [Obsolete("This has been deprecated in favor of JsonWriterSettings object")]
        public int MaxDepth
        {
            get => settings.MaxDepth;
            set => settings.MaxDepth = value;
        }

        [Obsolete("This has been deprecated in favor of JsonWriterSettings object")]
        public bool UseXmlSerializationAttributes
        {
            get => settings.UseXmlSerializationAttributes;
            set => settings.UseXmlSerializationAttributes = value;
        }

        [Obsolete("This has been deprecated in favor of JsonWriterSettings object")]
        public WriteDelegate<DateTime> DateTimeSerializer
        {
            get => settings.DateTimeSerializer;
            set => settings.DateTimeSerializer = value;
        }

        public TextWriter TextWriter => Writer;

        public JsonWriterSettings Settings
        {
            get => settings;
            set
            {
                if (value == null)
                {
                    value = new JsonWriterSettings();
                }

                settings = value;
            }
        }

        public static string Serialize(object value)
        {
            StringBuilder output = new StringBuilder();
            using (JsonWriter jsonWriter = new JsonWriter(output))
            {
                jsonWriter.Write(value);
            }

            return output.ToString();
        }

        public void Write(object value)
        {
            Write(value, false);
        }

        protected virtual void Write(object value, bool isProperty)
        {
            if (isProperty && settings.PrettyPrint)
            {
                Writer.Write(' ');
            }

            switch (value)
            {
                case null:
                    Writer.Write("null");
                    break;
                case IJsonSerializable _:
                    try
                    {
                        if (isProperty)
                        {
                            ++depth;
                            if (depth > settings.MaxDepth)
                            {
                                throw new JsonSerializationException(string.Format("The maxiumum depth of {0} was exceeded. Check for cycles in object graph.", settings.MaxDepth));
                            }

                            WriteLine();
                        }
                      ((IJsonSerializable)value).WriteJson(this);
                        break;
                    }
                    finally
                    {
                        if (isProperty)
                        {
                            --depth;
                        }
                    }
                case Enum _:
                    Write((Enum)value);
                    break;
                default:
                    Type type = value.GetType();
                    switch (Type.GetTypeCode(type))
                    {
                        case TypeCode.Empty:
                        case TypeCode.DBNull:
                            Writer.Write("null");
                            return;
                        case TypeCode.Boolean:
                            Write((bool)value);
                            return;
                        case TypeCode.Char:
                            Write((char)value);
                            return;
                        case TypeCode.SByte:
                            Write((sbyte)value);
                            return;
                        case TypeCode.Byte:
                            Write((byte)value);
                            return;
                        case TypeCode.Int16:
                            Write((short)value);
                            return;
                        case TypeCode.UInt16:
                            Write((ushort)value);
                            return;
                        case TypeCode.Int32:
                            Write((int)value);
                            return;
                        case TypeCode.UInt32:
                            Write((uint)value);
                            return;
                        case TypeCode.Int64:
                            Write((long)value);
                            return;
                        case TypeCode.UInt64:
                            Write((ulong)value);
                            return;
                        case TypeCode.Single:
                            Write((float)value);
                            return;
                        case TypeCode.Double:
                            Write((double)value);
                            return;
                        case TypeCode.Decimal:
                            Write((decimal)value);
                            return;
                        case TypeCode.DateTime:
                            Write((DateTime)value);
                            return;
                        case TypeCode.String:
                            Write((string)value);
                            return;
                        default:
                            if (value is Guid guid)
                            {
                                Write(guid);
                                return;
                            }
                            if ((object)(value as Uri) != null)
                            {
                                Write((Uri)value);
                                return;
                            }
                            if (value is TimeSpan timeSpan)
                            {
                                Write(timeSpan);
                                return;
                            }
                            if ((object)(value as Version) != null)
                            {
                                Write((Version)value);
                                return;
                            }
                            if (value is IDictionary)
                            {
                                WriteObject((IDictionary)value);
                                return;
                            }
                            if (type.GetInterface("System.Collections.Generic.IDictionary`2") != null)
                            {
                                try
                                {
                                    if (isProperty)
                                    {
                                        ++depth;
                                        if (depth > settings.MaxDepth)
                                        {
                                            throw new JsonSerializationException(string.Format("The maxiumum depth of {0} was exceeded. Check for cycles in object graph.", settings.MaxDepth));
                                        }

                                        WriteLine();
                                    }
                                    WriteDictionary((IEnumerable)value);
                                    return;
                                }
                                finally
                                {
                                    if (isProperty)
                                    {
                                        --depth;
                                    }
                                }
                            }
                            else
                            {
                                if (value is IEnumerable)
                                {
                                    if (value is XmlNode)
                                    {
                                        Write((XmlNode)value);
                                        return;
                                    }
                                    WriteArray((IEnumerable)value);
                                    return;
                                }
                                WriteObject(value, type);
                                return;
                            }
                    }
            }
        }

        public virtual void WriteBase64(byte[] value)
        {
            Write(Convert.ToBase64String(value));
        }

        public virtual void WriteHexString(byte[] value)
        {
            if (value == null || value.Length == 0)
            {
                Write(string.Empty);
            }
            else
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int index = 0; index < value.Length; ++index)
                {
                    stringBuilder.Append(value[index].ToString("x2"));
                }

                Write(stringBuilder.ToString());
            }
        }

        public virtual void Write(DateTime value)
        {
            if (settings.DateTimeSerializer != null)
            {
                settings.DateTimeSerializer(this, value);
            }
            else
            {
                switch (value.Kind)
                {
                    case DateTimeKind.Utc:
                        Write(string.Format("{0:s}Z", value));
                        break;
                    case DateTimeKind.Local:
                        value = value.ToUniversalTime();
                        goto case DateTimeKind.Utc;
                    default:
                        Write(string.Format("{0:s}", value));
                        break;
                }
            }
        }

        public virtual void Write(Guid value)
        {
            Write(value.ToString("D"));
        }

        public virtual void Write(Enum value)
        {
            Type type = value.GetType();
            string str;
            if (type.IsDefined(typeof(FlagsAttribute), true) && !Enum.IsDefined(type, value))
            {
                Enum[] flagList = JsonWriter.GetFlagList(type, value);
                string[] strArray = new string[flagList.Length];
                for (int index = 0; index < flagList.Length; ++index)
                {
                    strArray[index] = JsonNameAttribute.GetJsonName(flagList[index]);
                    if (string.IsNullOrEmpty(strArray[index]))
                    {
                        strArray[index] = flagList[index].ToString("f");
                    }
                }
                str = string.Join(", ", strArray);
            }
            else
            {
                str = JsonNameAttribute.GetJsonName(value);
                if (string.IsNullOrEmpty(str))
                {
                    str = value.ToString("f");
                }
            }
            Write(str);
        }

        public virtual void Write(string value)
        {
            if (value == null)
            {
                Writer.Write("null");
            }
            else
            {
                int startIndex = 0;
                int length = value.Length;
                Writer.Write('"');
                for (int index = startIndex; index < length; ++index)
                {
                    char ch = value[index];
                    if (ch <= '\x001F' || ch == '<' || (ch == '"' || ch == '\\'))
                    {
                        if (index > startIndex)
                        {
                            Writer.Write(value.Substring(startIndex, index - startIndex));
                        }

                        startIndex = index + 1;
                        switch (ch)
                        {
                            case '\b':
                                Writer.Write("\\b");
                                continue;
                            case '\t':
                                Writer.Write("\\t");
                                continue;
                            case '\n':
                                Writer.Write("\\n");
                                continue;
                            case '\f':
                                Writer.Write("\\f");
                                continue;
                            case '\r':
                                Writer.Write("\\r");
                                continue;
                            default:
                                if (ch == '"' || ch == '\\')
                                {
                                    Writer.Write('\\');
                                    Writer.Write(ch);
                                    continue;
                                }
                                Writer.Write("\\u");
                                Writer.Write(char.ConvertToUtf32(value, index).ToString("X4"));
                                continue;
                        }
                    }
                }
                if (length > startIndex)
                {
                    Writer.Write(value.Substring(startIndex, length - startIndex));
                }

                Writer.Write('"');
            }
        }

        public virtual void Write(bool value)
        {
            Writer.Write(!value ? "false" : "true");
        }

        public virtual void Write(byte value)
        {
            Writer.Write(value.ToString("g", CultureInfo.InvariantCulture));
        }

        public virtual void Write(sbyte value)
        {
            Writer.Write(value.ToString("g", CultureInfo.InvariantCulture));
        }

        public virtual void Write(short value)
        {
            Writer.Write(value.ToString("g", CultureInfo.InvariantCulture));
        }

        public virtual void Write(ushort value)
        {
            Writer.Write(value.ToString("g", CultureInfo.InvariantCulture));
        }

        public virtual void Write(int value)
        {
            Writer.Write(value.ToString("g", CultureInfo.InvariantCulture));
        }

        public virtual void Write(uint value)
        {
            if (InvalidIeee754(value))
            {
                Write(value.ToString("g", CultureInfo.InvariantCulture));
            }
            else
            {
                Writer.Write(value.ToString("g", CultureInfo.InvariantCulture) + 'U');
            }
        }

        public virtual void Write(long value)
        {
            if (InvalidIeee754(value))
            {
                Write(value.ToString("g", CultureInfo.InvariantCulture));
            }
            else
            {
                Writer.Write(value.ToString("g", CultureInfo.InvariantCulture) + 'L');
            }
        }

        public virtual void Write(ulong value)
        {
            if (InvalidIeee754(value))
            {
                Write(value.ToString("g", CultureInfo.InvariantCulture));
            }
            else
            {
                Writer.Write(value.ToString("g", CultureInfo.InvariantCulture));
            }
        }

        public virtual void Write(float value)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                Writer.Write("null");
            }
            else
            {
                Writer.Write(value.ToString("r", CultureInfo.InvariantCulture) + 'F');
            }
        }

        public virtual void Write(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                Writer.Write("null");
            }
            else
            {
                Writer.Write(value.ToString("r", CultureInfo.InvariantCulture) + 'D');
            }
        }

        public virtual void Write(decimal value)
        {
            if (InvalidIeee754(value))
            {
                Write(value.ToString("g", CultureInfo.InvariantCulture));
            }
            else
            {
                Writer.Write(value.ToString("g", CultureInfo.InvariantCulture));
            }
        }

        public virtual void Write(char value)
        {
            Write(new string(value, 1));
        }

        public virtual void Write(TimeSpan value)
        {
            Write(value.Ticks);
        }

        public virtual void Write(Uri value)
        {
            Write(value.ToString());
        }

        public virtual void Write(Version value)
        {
            Write(value.ToString());
        }

        public virtual void Write(XmlNode value)
        {
            Write(value.OuterXml);
        }

        protected internal virtual void WriteArray(IEnumerable value)
        {
            bool appendDelim = false;
            bool simple = true;
            Writer.Write('[');
            ++depth;
            if (depth > settings.MaxDepth)
            {
                throw new JsonSerializationException(string.Format("The maxiumum depth of {0} was exceeded. Check for cycles in object graph.", settings.MaxDepth));
            }

            try
            {
                AotSafe.ForEach<object>(value, item =>
              {
                  if (appendDelim)
                  {
                      WriteArrayItemDelim();
                  }
                  else
                  {
                      appendDelim = true;
                  }

                  if (simple && Type.GetTypeCode(item.GetType()) == TypeCode.Object)
                  {
                      simple = false;
                  }

                  if (!simple)
                  {
                      WriteLine();
                  }

                  WriteArrayItem(item);
              });
            }
            finally
            {
                --depth;
            }
            if (appendDelim && !simple)
            {
                WriteLine();
            }

            Writer.Write(']');
        }

        protected virtual void WriteArrayItem(object item)
        {
            Write(item, false);
        }

        protected virtual void WriteObject(IDictionary value)
        {
            WriteDictionary(value);
        }

        protected virtual void WriteDictionary(IEnumerable value)
        {
            if (!(value.GetEnumerator() is IDictionaryEnumerator enumerator))
            {
                throw new JsonSerializationException(string.Format("Types which implement Generic IDictionary<TKey, TValue> must have an IEnumerator which implements IDictionaryEnumerator. ({0})", value.GetType()));
            }

            bool flag = false;
            Writer.Write('{');
            ++depth;
            if (depth > settings.MaxDepth)
            {
                throw new JsonSerializationException(string.Format("The maxiumum depth of {0} was exceeded. Check for cycles in object graph.", settings.MaxDepth));
            }

            try
            {
                while (enumerator.MoveNext())
                {
                    if (flag)
                    {
                        WriteObjectPropertyDelim();
                    }
                    else
                    {
                        flag = true;
                    }

                    WriteObjectProperty(Convert.ToString(enumerator.Entry.Key), enumerator.Entry.Value);
                }
            }
            finally
            {
                --depth;
            }
            if (flag)
            {
                WriteLine();
            }

            Writer.Write('}');
        }

        private void WriteObjectProperty(string key, object value)
        {
            WriteLine();
            WriteObjectPropertyName(key);
            Writer.Write(':');
            WriteObjectPropertyValue(value);
        }

        protected virtual void WriteObjectPropertyName(string name)
        {
            Write(name);
        }

        protected virtual void WriteObjectPropertyValue(object value)
        {
            Write(value, true);
        }

        protected virtual void WriteObject(object value, Type type)
        {
            bool flag1 = false;
            Writer.Write('{');
            ++depth;
            if (depth > settings.MaxDepth)
            {
                throw new JsonSerializationException(string.Format("The maxiumum depth of {0} was exceeded. Check for cycles in object graph.", settings.MaxDepth));
            }

            try
            {
                if (!string.IsNullOrEmpty(settings.TypeHintName))
                {
                    if (flag1)
                    {
                        WriteObjectPropertyDelim();
                    }
                    else
                    {
                        flag1 = true;
                    }

                    WriteObjectProperty(settings.TypeHintName, type.FullName + ", " + type.Assembly.GetName().Name);
                }
                bool flag2 = type.IsGenericType && type.Name.StartsWith("<>f__AnonymousType");
                foreach (PropertyInfo property in type.GetProperties())
                {
                    if (property.CanRead && (property.CanWrite || flag2) && !IsIgnored(type, property, value))
                    {
                        object obj = property.GetValue(value, null);
                        if (!IsDefaultValue(property, obj))
                        {
                            if (flag1)
                            {
                                WriteObjectPropertyDelim();
                            }
                            else
                            {
                                flag1 = true;
                            }

                            string key = JsonNameAttribute.GetJsonName(property);
                            if (string.IsNullOrEmpty(key))
                            {
                                key = property.Name;
                            }

                            WriteObjectProperty(key, obj);
                        }
                    }
                }
                foreach (FieldInfo field in type.GetFields())
                {
                    if (field.IsPublic && !field.IsStatic && !IsIgnored(type, field, value))
                    {
                        object obj = field.GetValue(value);
                        if (!IsDefaultValue(field, obj))
                        {
                            if (flag1)
                            {
                                WriteObjectPropertyDelim();
                            }
                            else
                            {
                                flag1 = true;
                            }

                            string key = JsonNameAttribute.GetJsonName(field);
                            if (string.IsNullOrEmpty(key))
                            {
                                key = field.Name;
                            }

                            WriteObjectProperty(key, obj);
                        }
                    }
                }
            }
            finally
            {
                --depth;
            }
            if (flag1)
            {
                WriteLine();
            }

            Writer.Write('}');
        }

        protected virtual void WriteArrayItemDelim()
        {
            Writer.Write(',');
        }

        protected virtual void WriteObjectPropertyDelim()
        {
            Writer.Write(',');
        }

        protected virtual void WriteLine()
        {
            if (!settings.PrettyPrint)
            {
                return;
            }

            Writer.WriteLine();
            for (int index = 0; index < depth; ++index)
            {
                Writer.Write(settings.Tab);
            }
        }

        private bool IsIgnored(Type objType, MemberInfo member, object obj)
        {
            if (JsonIgnoreAttribute.IsJsonIgnore(member))
            {
                return true;
            }

            string specifiedProperty = JsonSpecifiedPropertyAttribute.GetJsonSpecifiedProperty(member);
            if (!string.IsNullOrEmpty(specifiedProperty))
            {
                PropertyInfo property = objType.GetProperty(specifiedProperty);
                if (property != null)
                {
                    object obj1 = property.GetValue(obj, null);
                    if (obj1 is bool && !Convert.ToBoolean(obj1))
                    {
                        return true;
                    }
                }
            }
            if (settings.UseXmlSerializationAttributes)
            {
                if (JsonIgnoreAttribute.IsXmlIgnore(member))
                {
                    return true;
                }

                PropertyInfo property = objType.GetProperty(member.Name + "Specified");
                if (property != null)
                {
                    object obj1 = property.GetValue(obj, null);
                    if (obj1 is bool && !Convert.ToBoolean(obj1))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsDefaultValue(MemberInfo member, object value)
        {
            if (!(Attribute.GetCustomAttribute(member, typeof(DefaultValueAttribute)) is DefaultValueAttribute customAttribute))
            {
                return false;
            }

            return customAttribute.Value == null ? value == null : customAttribute.Value.Equals(value);
        }

        private static Enum[] GetFlagList(Type enumType, object value)
        {
            ulong uint64_1 = Convert.ToUInt64(value);
            Array values = Enum.GetValues(enumType);
            List<Enum> enumList = new List<Enum>(values.Length);
            if (uint64_1 == 0UL)
            {
                enumList.Add((Enum)Convert.ChangeType(value, enumType));
                return enumList.ToArray();
            }
            for (int index = values.Length - 1; index >= 0; --index)
            {
                ulong uint64_2 = Convert.ToUInt64(values.GetValue(index));
                if ((index != 0 || uint64_2 != 0UL) && ((long)uint64_1 & (long)uint64_2) == (long)uint64_2)
                {
                    uint64_1 -= uint64_2;
                    enumList.Add(values.GetValue(index) as Enum);
                }
            }
            if (uint64_1 != 0UL)
            {
                enumList.Add(Enum.ToObject(enumType, uint64_1) as Enum);
            }

            return enumList.ToArray();
        }

        protected virtual bool InvalidIeee754(decimal value)
        {
            try
            {
                return (decimal)(double)value != value;
            }
            catch
            {
                return true;
            }
        }

        void IDisposable.Dispose()
        {
            if (Writer == null)
            {
                return;
            }

            Writer.Dispose();
        }
    }
}
