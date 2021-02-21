// Decompiled with JetBrains decompiler
// Type: JsonFx.Json.EcmaScriptIdentifier
// Assembly: Assembly-CSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 18E1FC19-6B38-46B3-9D3F-DF1E9429ABBE
// Assembly location: C:\Users\Admin\Desktop\APK Easy Tool portable\1-Decompiled APKs\벽람항로_v1.4.61_apkpure.com\assets\bin\Data\Managed\Assembly-CSharp.dll

using System;

namespace JsonFx.Json
{
    public class EcmaScriptIdentifier : IJsonSerializable
    {
        private readonly string identifier;

        public EcmaScriptIdentifier()
          : this(null)
        {
        }

        public EcmaScriptIdentifier(string ident)
        {
            identifier = !string.IsNullOrEmpty(ident) ? EcmaScriptIdentifier.EnsureValidIdentifier(ident, true) : string.Empty;
        }

        public string Identifier => identifier;

        public static string EnsureValidIdentifier(string varExpr, bool nested)
        {
            return EcmaScriptIdentifier.EnsureValidIdentifier(varExpr, nested, true);
        }

        public static string EnsureValidIdentifier(string varExpr, bool nested, bool throwOnEmpty)
        {
            if (string.IsNullOrEmpty(varExpr))
            {
                if (throwOnEmpty)
                {
                    throw new ArgumentException("Variable expression is empty.");
                }

                return string.Empty;
            }
            varExpr = varExpr.Replace(" ", string.Empty);
            if (!EcmaScriptIdentifier.IsValidIdentifier(varExpr, nested))
            {
                throw new ArgumentException("Variable expression \"" + varExpr + "\" is not supported.");
            }

            return varExpr;
        }

        public static bool IsValidIdentifier(string varExpr, bool nested)
        {
            if (string.IsNullOrEmpty(varExpr))
            {
                return false;
            }

            if (nested)
            {
                string str = varExpr;
                char[] chArray = new char[1] { '.' };
                foreach (string varExpr1 in str.Split(chArray))
                {
                    if (!EcmaScriptIdentifier.IsValidIdentifier(varExpr1, false))
                    {
                        return false;
                    }
                }
                return true;
            }
            if (EcmaScriptIdentifier.IsReservedWord(varExpr))
            {
                return false;
            }

            bool flag = false;
            foreach (char c in varExpr)
            {
                if (!flag || !char.IsDigit(c))
                {
                    if (!char.IsLetter(c) && c != '_' && c != '$')
                    {
                        return false;
                    }

                    flag = true;
                }
            }
            return true;
        }

        private static bool IsReservedWord(string varExpr)
        {
            // TODO: investigate doing this like Rhino does (switch on length check first letter or two)
            switch (varExpr)
            {
                // literals
                case "null":
                case "false":
                case "true":

                // ES5 Keywords
                case "break":
                case "case":
                case "catch":
                case "continue":
                case "debugger":
                case "default":
                case "delete":
                case "do":
                case "else":
                case "finally":
                case "for":
                case "function":
                case "if":
                case "in":
                case "instanceof":
                case "new":
                case "return":
                case "switch":
                case "this":
                case "throw":
                case "try":
                case "typeof":
                case "var":
                case "void":
                case "while":
                case "with":

                // ES5 Future Reserved Words
                case "abstract":
                case "boolean":
                case "byte":
                case "char":
                case "class":
                case "const":
                case "double":
                case "enum":
                case "export":
                case "extends":
                case "final":
                case "float":
                case "goto":
                case "implements":
                case "import":
                case "int":
                case "interface":
                case "long":
                case "native":
                case "package":
                case "private":
                case "protected":
                case "public":
                case "short":
                case "static":
                case "super":
                case "synchronized":
                case "throws":
                case "transient":
                case "volatile":

                // ES5 Possible Reserved Words
                case "let":
                case "yield":
                    {
                        return true;
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        public static EcmaScriptIdentifier Parse(string value)
        {
            return new EcmaScriptIdentifier(value);
        }

        public static implicit operator string(EcmaScriptIdentifier ident)
        {
            return ident == null ? string.Empty : ident.identifier;
        }

        public static implicit operator EcmaScriptIdentifier(string ident)
        {
            return new EcmaScriptIdentifier(ident);
        }

        void IJsonSerializable.ReadJson(JsonReader reader)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        void IJsonSerializable.WriteJson(JsonWriter writer)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                writer.TextWriter.Write("null");
            }
            else
            {
                writer.TextWriter.Write(identifier);
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is EcmaScriptIdentifier scriptIdentifier))
            {
                return base.Equals(obj);
            }

            return string.IsNullOrEmpty(identifier) && string.IsNullOrEmpty(scriptIdentifier.identifier) || StringComparer.Ordinal.Equals(identifier, scriptIdentifier.identifier);
        }

        public override string ToString()
        {
            return identifier;
        }

        public override int GetHashCode()
        {
            return identifier == null ? 0 : identifier.GetHashCode();
        }
    }
}
