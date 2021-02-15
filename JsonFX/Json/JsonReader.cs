// Decompiled with JetBrains decompiler
// Type: JsonFx.Json.JsonReader
// Assembly: Assembly-CSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 18E1FC19-6B38-46B3-9D3F-DF1E9429ABBE
// Assembly location: C:\Users\Admin\Desktop\APK Easy Tool portable\1-Decompiled APKs\벽람항로_v1.4.61_apkpure.com\assets\bin\Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace JsonFx.Json
{
    public class JsonReader
    {
        private static Type defaultObjectType = typeof(Dictionary<string, object>);
        private readonly JsonReaderSettings Settings = new JsonReaderSettings();
        internal const string LiteralFalse = "false";
        internal const string LiteralTrue = "true";
        internal const string LiteralNull = "null";
        internal const string LiteralUndefined = "undefined";
        internal const string LiteralNotANumber = "NaN";
        internal const string LiteralPositiveInfinity = "Infinity";
        internal const string LiteralNegativeInfinity = "-Infinity";
        internal const char OperatorNegate = '-';
        internal const char OperatorUnaryPlus = '+';
        internal const char OperatorArrayStart = '[';
        internal const char OperatorArrayEnd = ']';
        internal const char OperatorObjectStart = '{';
        internal const char OperatorObjectEnd = '}';
        internal const char OperatorStringDelim = '"';
        internal const char OperatorStringDelimAlt = '\'';
        internal const char OperatorValueDelim = ',';
        internal const char OperatorNameDelim = ':';
        internal const char OperatorCharEscape = '\\';
        private const string CommentStart = "/*";
        private const string CommentEnd = "*/";
        private const string CommentLine = "//";
        private const string LineEndings = "\r\n";
        internal const string TypeGenericIDictionary = "System.Collections.Generic.IDictionary`2";
        private const string ErrorUnrecognizedToken = "Illegal JSON sequence.";
        private const string ErrorUnterminatedComment = "Unterminated comment block.";
        private const string ErrorUnterminatedObject = "Unterminated JSON object.";
        private const string ErrorUnterminatedArray = "Unterminated JSON array.";
        private const string ErrorUnterminatedString = "Unterminated JSON string.";
        private const string ErrorIllegalNumber = "Illegal JSON number.";
        private const string ErrorExpectedString = "Expected JSON string.";
        private const string ErrorExpectedObject = "Expected JSON object.";
        private const string ErrorExpectedArray = "Expected JSON array.";
        private const string ErrorExpectedPropertyName = "Expected JSON object property name.";
        private const string ErrorExpectedPropertyNameDelim = "Expected JSON object property name delimiter.";
        private const string ErrorGenericIDictionary = "Types which implement Generic IDictionary<TKey, TValue> also need to implement IDictionary to be deserialized. ({0})";
        private const string ErrorGenericIDictionaryKeys = "Types which implement Generic IDictionary<TKey, TValue> need to have string keys to be deserialized. ({0})";
        private readonly string Source;
        private readonly int SourceLength;
        private int index;

        public JsonReader(TextReader input)
          : this(input, new JsonReaderSettings())
        {
        }

        public JsonReader(TextReader input, JsonReaderSettings settings)
        {
            Settings = settings;
            Source = input.ReadToEnd();
            SourceLength = Source.Length;
        }

        public JsonReader(Stream input)
          : this(input, new JsonReaderSettings())
        {
        }

        public JsonReader(Stream input, JsonReaderSettings settings)
        {
            Settings = settings;
            using (StreamReader streamReader = new StreamReader(input, true))
            {
                Source = streamReader.ReadToEnd();
            }

            SourceLength = Source.Length;
        }

        public JsonReader(string input)
          : this(input, new JsonReaderSettings())
        {
        }

        public JsonReader(string input, JsonReaderSettings settings)
        {
            Settings = settings;
            Source = input;
            SourceLength = Source.Length;
        }

        public JsonReader(StringBuilder input)
          : this(input, new JsonReaderSettings())
        {
        }

        public JsonReader(StringBuilder input, JsonReaderSettings settings)
        {
            Settings = settings;
            Source = input.ToString();
            SourceLength = Source.Length;
        }

        [Obsolete("This has been deprecated in favor of JsonReaderSettings object")]
        public bool AllowNullValueTypes
        {
            get => Settings.AllowNullValueTypes;
            set => Settings.AllowNullValueTypes = value;
        }

        [Obsolete("This has been deprecated in favor of JsonReaderSettings object")]
        public string TypeHintName
        {
            get => Settings.TypeHintName;
            set => Settings.TypeHintName = value;
        }

        public object Deserialize()
        {
            return Deserialize((Type)null);
        }

        public object Deserialize(int start)
        {
            index = start;
            return Deserialize((Type)null);
        }

        public object Deserialize(Type type)
        {
            return Read(type, false);
        }

        public T Deserialize<T>()
        {
            return (T)Read(typeof(T), false);
        }

        public object Deserialize(int start, Type type)
        {
            index = start;
            return Read(type, false);
        }

        public T Deserialize<T>(int start)
        {
            index = start;
            return (T)Read(typeof(T), false);
        }

        private object Read(Type expectedType, bool typeIsHint)
        {
            if (expectedType == typeof(object))
            {
                expectedType = null;
            }

            switch (Tokenize())
            {
                case JsonToken.Undefined:
                    index += "undefined".Length;
                    return null;
                case JsonToken.Null:
                    index += "null".Length;
                    return null;
                case JsonToken.False:
                    index += "false".Length;
                    return false;
                case JsonToken.True:
                    index += "true".Length;
                    return true;
                case JsonToken.NaN:
                    index += "NaN".Length;
                    return double.NaN;
                case JsonToken.PositiveInfinity:
                    index += "Infinity".Length;
                    return double.PositiveInfinity;
                case JsonToken.NegativeInfinity:
                    index += "-Infinity".Length;
                    return double.NegativeInfinity;
                case JsonToken.Number:
                    return ReadNumber(!typeIsHint ? expectedType : null);
                case JsonToken.String:
                    return ReadString(!typeIsHint ? expectedType : null);
                case JsonToken.ArrayStart:
                    return ReadArray(!typeIsHint ? expectedType : null);
                case JsonToken.ObjectStart:
                    return ReadObject(!typeIsHint ? expectedType : null);
                default:
                    return null;
            }
        }

        private object ReadObject(Type objectType)
        {
            if (Source[index] != '{')
            {
                throw new JsonDeserializationException("Expected JSON object.", index);
            }

            Type type1 = null;
            Dictionary<string, MemberInfo> memberMap = null;
            object result;
            if (objectType != null)
            {
                result = Settings.Coercion.InstantiateObject(objectType, out memberMap);
                if (memberMap == null)
                {
                    Type type2 = objectType.GetInterface("System.Collections.Generic.IDictionary`2");
                    if (type2 != null)
                    {
                        Type[] genericArguments = type2.GetGenericArguments();
                        if (genericArguments.Length == 2)
                        {
                            if (genericArguments[0] != typeof(string))
                            {
                                throw new JsonDeserializationException(string.Format("Types which implement Generic IDictionary<TKey, TValue> need to have string keys to be deserialized. ({0})", objectType), index);
                            }

                            if (genericArguments[1] != typeof(object))
                            {
                                type1 = genericArguments[1];
                            }
                        }
                    }
                }
            }
            else
            {
                result = Activator.CreateInstance(JsonReader.defaultObjectType) as IDictionary;
            }

            JsonToken jsonToken;
            do
            {
                ++index;
                if (index >= SourceLength)
                {
                    throw new JsonDeserializationException("Unterminated JSON object.", index);
                }

                jsonToken = Tokenize(Settings.AllowUnquotedObjectKeys);
                switch (jsonToken)
                {
                    case JsonToken.String:
                    case JsonToken.UnquotedName:
                        string str = jsonToken != JsonToken.String ? ReadUnquotedKey() : (string)ReadString(null);
                        MemberInfo memberInfo;
                        Type type2;
                        if (type1 == null && memberMap != null)
                        {
                            type2 = TypeCoercionUtility.GetMemberInfo(memberMap, str, out memberInfo);
                        }
                        else
                        {
                            type2 = type1;
                            memberInfo = null;
                        }
                        if (Tokenize() != JsonToken.NameDelim)
                        {
                            throw new JsonDeserializationException("Expected JSON object property name delimiter.", index);
                        }

                        ++index;
                        if (index >= SourceLength)
                        {
                            throw new JsonDeserializationException("Unterminated JSON object.", index);
                        }

                        object obj = Read(type2, false);
                        if (result is IDictionary)
                        {
                            if (objectType == null && Settings.IsTypeHintName(str))
                            {
                                result = Settings.Coercion.ProcessTypeHint((IDictionary)result, obj as string, out objectType, out memberMap);
                            }
                            else
                            {
                                ((IDictionary)result)[str] = obj;
                            }
                        }
                        else
                        {
                            if (objectType.GetInterface("System.Collections.Generic.IDictionary`2") != null)
                            {
                                throw new JsonDeserializationException(string.Format("Types which implement Generic IDictionary<TKey, TValue> also need to implement IDictionary to be deserialized. ({0})", objectType), index);
                            }

                            Settings.Coercion.SetMemberValue(result, type2, memberInfo, obj);
                        }
                        jsonToken = Tokenize();
                        continue;
                    case JsonToken.ObjectEnd:
                        goto label_30;
                    default:
                        throw new JsonDeserializationException("Expected JSON object property name.", index);
                }
            }
            while (jsonToken == JsonToken.ValueDelim);
        label_30:
            if (jsonToken != JsonToken.ObjectEnd)
            {
                new JsonDeserializationException("Unterminated JSON object.", index).GetLineAndColumn(Source, out int line, out int col);
                throw new JsonDeserializationException(string.Format("Unterminated JSON object.in line {0}, column {1}", line, col), index);
            }
            ++index;
            return result;
        }

        private IEnumerable ReadArray(Type arrayType)
        {
            if (Source[index] != '[')
            {
                throw new JsonDeserializationException("Expected JSON array.", index);
            }

            bool flag = arrayType != null;
            bool typeIsHint = !flag;
            Type type = null;
            if (flag)
            {
                if (arrayType.HasElementType)
                {
                    type = arrayType.GetElementType();
                }
                else if (arrayType.IsGenericType)
                {
                    Type[] genericArguments = arrayType.GetGenericArguments();
                    if (genericArguments.Length == 1)
                    {
                        type = genericArguments[0];
                    }
                }
            }
            ArrayList arrayList = new ArrayList();
            JsonToken jsonToken;
            do
            {
                ++index;
                if (index >= SourceLength)
                {
                    throw new JsonDeserializationException("Unterminated JSON array.", index);
                }

                jsonToken = Tokenize();
                if (jsonToken != JsonToken.ArrayEnd)
                {
                    object obj = Read(type, typeIsHint);
                    arrayList.Add(obj);
                    if (obj == null)
                    {
                        if (type != null && type.IsValueType)
                        {
                            type = null;
                        }

                        flag = true;
                    }
                    else if (type != null && !type.IsAssignableFrom(obj.GetType()))
                    {
                        if (obj.GetType().IsAssignableFrom(type))
                        {
                            type = obj.GetType();
                        }
                        else
                        {
                            type = null;
                            flag = true;
                        }
                    }
                    else if (!flag)
                    {
                        type = obj.GetType();
                        flag = true;
                    }
                    jsonToken = Tokenize();
                }
                else
                {
                    break;
                }
            }
            while (jsonToken == JsonToken.ValueDelim);
            if (jsonToken != JsonToken.ArrayEnd)
            {
                throw new JsonDeserializationException("Unterminated JSON array.", index);
            }

            ++index;
            return type != null && type != typeof(object) ? arrayList.ToArray(type) : arrayList.ToArray();
        }

        private string ReadUnquotedKey()
        {
            int index = this.index;
            do
            {
                ++this.index;
            }
            while (Tokenize(true) == JsonToken.UnquotedName);
            return Source.Substring(index, this.index - index);
        }

        private object ReadString(Type expectedType)
        {
            if (Source[this.index] != '"' && Source[this.index] != '\'')
            {
                throw new JsonDeserializationException("Expected JSON string.", this.index);
            }

            char ch1 = Source[this.index];
            ++this.index;
            if (this.index >= SourceLength)
            {
                throw new JsonDeserializationException("Unterminated JSON string.", this.index);
            }

            int index = this.index;
            StringBuilder stringBuilder = new StringBuilder();
            while (Source[this.index] != ch1)
            {
                if (Source[this.index] == '\\')
                {
                    stringBuilder.Append(Source, index, this.index - index);
                    ++this.index;
                    if (this.index >= SourceLength)
                    {
                        throw new JsonDeserializationException("Unterminated JSON string.", this.index);
                    }

                    char ch2 = Source[this.index];
                    switch (ch2)
                    {
                        case 'r':
                            stringBuilder.Append('\r');
                            break;
                        case 't':
                            stringBuilder.Append('\t');
                            break;
                        case 'u':
                            int result;
                            if (this.index + 4 < SourceLength && int.TryParse(Source.Substring(this.index + 1, 4), NumberStyles.AllowHexSpecifier, NumberFormatInfo.InvariantInfo, out result))
                            {
                                stringBuilder.Append(char.ConvertFromUtf32(result));
                                this.index += 4;
                                break;
                            }
                            stringBuilder.Append(Source[this.index]);
                            break;
                        default:
                            switch (ch2)
                            {
                                case '0':
                                    break;
                                case 'b':
                                    stringBuilder.Append('\b');
                                    break;
                                case 'f':
                                    stringBuilder.Append('\f');
                                    break;
                                case 'n':
                                    stringBuilder.Append('\n');
                                    break;
                                default:
                                    stringBuilder.Append(Source[this.index]);
                                    break;
                            }
                            break;
                    }
                    ++this.index;
                    if (this.index >= SourceLength)
                    {
                        throw new JsonDeserializationException("Unterminated JSON string.", this.index);
                    }

                    index = this.index;
                }
                else
                {
                    ++this.index;
                    if (this.index >= SourceLength)
                    {
                        throw new JsonDeserializationException("Unterminated JSON string.", this.index);
                    }
                }
            }
            stringBuilder.Append(Source, index, this.index - index);
            ++this.index;
            return expectedType != null && expectedType != typeof(string) ? Settings.Coercion.CoerceType(expectedType, stringBuilder.ToString()) : stringBuilder.ToString();
        }

        private object ReadNumber(Type expectedType)
        {
            bool hasDecimal = false;
            bool hasExponent = false;
            int start = index;
            int precision = 0;
            int exponent = 0;

            // optional minus part
            if (Source[index] == JsonReader.OperatorNegate)
            {
                // consume sign
                index++;
                if (index >= SourceLength || !char.IsDigit(Source[index]))
                {
                    throw new JsonDeserializationException(JsonReader.ErrorIllegalNumber, index);
                }
            }

            // integer part
            while ((index < SourceLength) && char.IsDigit(Source[index]))
            {
                // consume digit
                index++;
            }

            // optional decimal part
            if ((index < SourceLength) && (Source[index] == '.'))
            {
                hasDecimal = true;

                // consume decimal
                index++;
                if (index >= SourceLength || !char.IsDigit(Source[index]))
                {
                    throw new JsonDeserializationException(JsonReader.ErrorIllegalNumber, index);
                }

                // fraction part
                while (index < SourceLength && char.IsDigit(Source[index]))
                {
                    // consume digit
                    index++;
                }
            }

            // note the number of significant digits
            precision = index - start - (hasDecimal ? 1 : 0);

            // optional exponent part
            if (index < SourceLength && (Source[index] == 'e' || Source[index] == 'E'))
            {
                hasExponent = true;

                // consume 'e'
                index++;
                if (index >= SourceLength)
                {
                    throw new JsonDeserializationException(JsonReader.ErrorIllegalNumber, index);
                }

                int expStart = index;

                // optional minus/plus part
                if (Source[index] == JsonReader.OperatorNegate || Source[index] == JsonReader.OperatorUnaryPlus)
                {
                    // consume sign
                    index++;
                    if (index >= SourceLength || !char.IsDigit(Source[index]))
                    {
                        throw new JsonDeserializationException(JsonReader.ErrorIllegalNumber, index);
                    }
                }
                else
                {
                    if (!char.IsDigit(Source[index]))
                    {
                        throw new JsonDeserializationException(JsonReader.ErrorIllegalNumber, index);
                    }
                }

                // exp part
                while (index < SourceLength && char.IsDigit(Source[index]))
                {
                    // consume digit
                    index++;
                }

                int.TryParse(Source.Substring(expStart, index - expStart), NumberStyles.Integer,
                    NumberFormatInfo.InvariantInfo, out exponent);
            }

            // at this point, we have the full number string and know its characteristics
            string numberString = Source.Substring(start, index - start);

            if (!hasDecimal && !hasExponent && precision < 19)
            {
                // is Integer value

                // parse as most flexible
                decimal number = decimal.Parse(
                    numberString,
                    NumberStyles.Integer,
                    NumberFormatInfo.InvariantInfo);

                if (expectedType != null)
                {
                    return Settings.Coercion.CoerceType(expectedType, number);
                }

                if (number >= int.MinValue && number <= int.MaxValue)
                {
                    // use most common
                    return (int)number;
                }
                if (number >= long.MinValue && number <= long.MaxValue)
                {
                    // use more flexible
                    return (long)number;
                }

                // use most flexible
                return number;
            }
            else
            {
                // is Floating Point value

                if (expectedType == typeof(decimal))
                {
                    // special case since Double does not convert to Decimal
                    return decimal.Parse(
                        numberString,
                        NumberStyles.Float,
                        NumberFormatInfo.InvariantInfo);
                }

                // use native EcmaScript number (IEEE 754)
                double number = double.Parse(
                    numberString,
                    NumberStyles.Float,
                    NumberFormatInfo.InvariantInfo);

                if (expectedType != null)
                {
                    return Settings.Coercion.CoerceType(expectedType, number);
                }

                return number;
            }
        }

        public static object Deserialize(string value)
        {
            return JsonReader.Deserialize(value, 0, null);
        }

        public static T Deserialize<T>(string value)
        {
            return (T)JsonReader.Deserialize(value, 0, typeof(T));
        }

        public static object Deserialize(string value, int start)
        {
            return JsonReader.Deserialize(value, start, null);
        }

        public static T Deserialize<T>(string value, int start)
        {
            return (T)JsonReader.Deserialize(value, start, typeof(T));
        }

        public static object Deserialize(string value, Type type)
        {
            return JsonReader.Deserialize(value, 0, type);
        }

        public static object Deserialize(string value, int start, Type type)
        {
            JsonReader jsonReader = new JsonReader(value);
            object obj = jsonReader.Deserialize(start, type);
            return jsonReader.Settings.Coercion.CoerceType(type, obj);
        }

        public static void RegisterDefaultObjectType<T>() where T : IDictionary
        {
            JsonReader.defaultObjectType = typeof(T);
        }

        private JsonToken Tokenize()
        {
            return Tokenize(false);
        }

        private JsonToken Tokenize(bool allowUnquotedString)
        {
            if (index >= SourceLength)
            {
                return JsonToken.End;
            }

            // skip whitespace
            while (char.IsWhiteSpace(Source[index]))
            {
                index++;
                if (index >= SourceLength)
                {
                    return JsonToken.End;
                }
            }

            #region Skip Comments

            // skip block and line comments
            if (Source[index] == JsonReader.CommentStart[0])
            {
                if (index + 1 >= SourceLength)
                {
                    throw new JsonDeserializationException(JsonReader.ErrorUnrecognizedToken, index);
                }

                // skip over first char of comment start
                index++;

                bool isBlockComment = false;
                if (Source[index] == JsonReader.CommentStart[1])
                {
                    isBlockComment = true;
                }
                else if (Source[index] != JsonReader.CommentLine[1])
                {
                    throw new JsonDeserializationException(JsonReader.ErrorUnrecognizedToken, index);
                }
                // skip over second char of comment start
                index++;

                if (isBlockComment)
                {
                    // store index for unterminated case
                    int commentStart = index - 2;

                    if (index + 1 >= SourceLength)
                    {
                        throw new JsonDeserializationException(JsonReader.ErrorUnterminatedComment, commentStart);
                    }

                    // skip over everything until reach block comment ending
                    while (Source[index] != JsonReader.CommentEnd[0] ||
                        Source[index + 1] != JsonReader.CommentEnd[1])
                    {
                        index++;
                        if (index + 1 >= SourceLength)
                        {
                            throw new JsonDeserializationException(JsonReader.ErrorUnterminatedComment, commentStart);
                        }
                    }

                    // skip block comment end token
                    index += 2;
                    if (index >= SourceLength)
                    {
                        return JsonToken.End;
                    }
                }
                else
                {
                    // skip over everything until reach line ending
                    while (JsonReader.LineEndings.IndexOf(Source[index]) < 0)
                    {
                        index++;
                        if (index >= SourceLength)
                        {
                            return JsonToken.End;
                        }
                    }
                }

                // skip whitespace again
                while (char.IsWhiteSpace(Source[index]))
                {
                    index++;
                    if (index >= SourceLength)
                    {
                        return JsonToken.End;
                    }
                }
            }

            #endregion Skip Comments

            // consume positive signing (as is extraneous)
            if (Source[index] == JsonReader.OperatorUnaryPlus)
            {
                index++;
                if (index >= SourceLength)
                {
                    return JsonToken.End;
                }
            }

            switch (Source[index])
            {
                case JsonReader.OperatorArrayStart:
                    {
                        return JsonToken.ArrayStart;
                    }
                case JsonReader.OperatorArrayEnd:
                    {
                        return JsonToken.ArrayEnd;
                    }
                case JsonReader.OperatorObjectStart:
                    {
                        return JsonToken.ObjectStart;
                    }
                case JsonReader.OperatorObjectEnd:
                    {
                        return JsonToken.ObjectEnd;
                    }
                case JsonReader.OperatorStringDelim:
                case JsonReader.OperatorStringDelimAlt:
                    {
                        return JsonToken.String;
                    }
                case JsonReader.OperatorValueDelim:
                    {
                        return JsonToken.ValueDelim;
                    }
                case JsonReader.OperatorNameDelim:
                    {
                        return JsonToken.NameDelim;
                    }
                default:
                    {
                        break;
                    }
            }

            // number
            if (char.IsDigit(Source[index]) ||
                ((Source[index] == JsonReader.OperatorNegate) && (index + 1 < SourceLength) && char.IsDigit(Source[index + 1])))
            {
                return JsonToken.Number;
            }

            // "false" literal
            if (MatchLiteral(JsonReader.LiteralFalse))
            {
                return JsonToken.False;
            }

            // "true" literal
            if (MatchLiteral(JsonReader.LiteralTrue))
            {
                return JsonToken.True;
            }

            // "null" literal
            if (MatchLiteral(JsonReader.LiteralNull))
            {
                return JsonToken.Null;
            }

            // "NaN" literal
            if (MatchLiteral(JsonReader.LiteralNotANumber))
            {
                return JsonToken.NaN;
            }

            // "Infinity" literal
            if (MatchLiteral(JsonReader.LiteralPositiveInfinity))
            {
                return JsonToken.PositiveInfinity;
            }

            // "-Infinity" literal
            if (MatchLiteral(JsonReader.LiteralNegativeInfinity))
            {
                return JsonToken.NegativeInfinity;
            }

            // "undefined" literal
            if (MatchLiteral(JsonReader.LiteralUndefined))
            {
                return JsonToken.Undefined;
            }

            if (allowUnquotedString)
            {
                return JsonToken.UnquotedName;
            }

            throw new JsonDeserializationException(JsonReader.ErrorUnrecognizedToken, index);
        }

        private bool MatchLiteral(string literal)
        {
            int length = literal.Length;
            int index1 = 0;
            for (int index2 = index; index1 < length && index2 < SourceLength; ++index2)
            {
                if (literal[index1] != Source[index2])
                {
                    return false;
                }

                ++index1;
            }
            return true;
        }

        public static T CoerceType<T>(object value, T typeToMatch)
        {
            return (T)new TypeCoercionUtility().CoerceType(typeof(T), value);
        }

        public static T CoerceType<T>(object value)
        {
            return (T)new TypeCoercionUtility().CoerceType(typeof(T), value);
        }

        public static object CoerceType(Type targetType, object value)
        {
            return new TypeCoercionUtility().CoerceType(targetType, value);
        }
    }
}
