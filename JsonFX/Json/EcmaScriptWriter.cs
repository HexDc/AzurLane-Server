// Decompiled with JetBrains decompiler
// Type: JsonFx.Json.EcmaScriptWriter
// Assembly: Assembly-CSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 18E1FC19-6B38-46B3-9D3F-DF1E9429ABBE
// Assembly location: C:\Users\Admin\Desktop\APK Easy Tool portable\1-Decompiled APKs\벽람항로_v1.4.61_apkpure.com\assets\bin\Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace JsonFx.Json
{
    public class EcmaScriptWriter : JsonWriter
    {
        private static readonly DateTime EcmaScriptEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        private static readonly char[] NamespaceDelims = new char[1]
        {
      '.'
        };
        private static readonly IList<string> BrowserObjects = new List<string>(new string[10]
        {
      "console",
      "document",
      "event",
      "frames",
      "history",
      "location",
      "navigator",
      "opera",
      "screen",
      "window"
        });
        private const string EcmaScriptDateCtor1 = "new Date({0})";
        private const string EcmaScriptDateCtor7 = "new Date({0:0000},{1},{2},{3},{4},{5},{6})";
        private const string EmptyRegExpLiteral = "(?:)";
        private const char RegExpLiteralDelim = '/';
        private const char OperatorCharEscape = '\\';
        private const string NamespaceDelim = ".";
        private const string RootDeclarationDebug = "\n/* namespace {1} */\nvar {0};";
        private const string RootDeclaration = "var {0};";
        private const string NamespaceCheck = "if(\"undefined\"===typeof {0}){{{0}={{}};}}";
        private const string NamespaceCheckDebug = "\nif (\"undefined\" === typeof {0}) {{\n    {0} = {{}};\n}}";

        public EcmaScriptWriter(TextWriter output)
          : base(output)
        {
        }

        public EcmaScriptWriter(Stream output)
          : base(output)
        {
        }

        public EcmaScriptWriter(string outputFileName)
          : base(outputFileName)
        {
        }

        public EcmaScriptWriter(StringBuilder output)
          : base(output)
        {
        }

        public static new string Serialize(object value)
        {
            StringBuilder output = new StringBuilder();
            using (EcmaScriptWriter ecmaScriptWriter = new EcmaScriptWriter(output))
            {
                ecmaScriptWriter.Write(value);
            }

            return output.ToString();
        }

        public static bool WriteNamespaceDeclaration(
          TextWriter writer,
          string ident,
          List<string> namespaces,
          bool isDebug)
        {
            if (string.IsNullOrEmpty(ident))
            {
                return false;
            }

            if (namespaces == null)
            {
                namespaces = new List<string>();
            }

            string[] strArray = ident.Split(EcmaScriptWriter.NamespaceDelims, StringSplitOptions.RemoveEmptyEntries);
            string str = strArray[0];
            bool flag = false;
            for (int index = 0; index < strArray.Length - 1; ++index)
            {
                flag = true;
                if (index > 0)
                {
                    str = str + "." + strArray[index];
                }

                if (!namespaces.Contains(str) && !EcmaScriptWriter.BrowserObjects.Contains(str))
                {
                    namespaces.Add(str);
                    if (index == 0)
                    {
                        if (isDebug)
                        {
                            writer.Write("\n/* namespace {1} */\nvar {0};", str, string.Join(".", strArray, 0, strArray.Length - 1));
                        }
                        else
                        {
                            writer.Write("var {0};", str);
                        }
                    }
                    if (isDebug)
                    {
                        writer.WriteLine("\nif (\"undefined\" === typeof {0}) {{\n    {0} = {{}};\n}}", str);
                    }
                    else
                    {
                        writer.Write("if(\"undefined\"===typeof {0}){{{0}={{}};}}", str);
                    }
                }
            }
            if (isDebug && flag)
            {
                writer.WriteLine();
            }

            return flag;
        }

        public override void Write(DateTime value)
        {
            EcmaScriptWriter.WriteEcmaScriptDate(this, value);
        }

        public override void Write(float value)
        {
            TextWriter.Write(value.ToString("r"));
        }

        public override void Write(double value)
        {
            TextWriter.Write(value.ToString("r"));
        }

        protected override void Write(object value, bool isProperty)
        {
            if (value is Regex)
            {
                if (isProperty && Settings.PrettyPrint)
                {
                    TextWriter.Write(' ');
                }

                EcmaScriptWriter.WriteEcmaScriptRegExp(this, (Regex)value);
            }
            else
            {
                base.Write(value, isProperty);
            }
        }

        protected override void WriteObjectPropertyName(string name)
        {
            if (EcmaScriptIdentifier.IsValidIdentifier(name, false))
            {
                TextWriter.Write(name);
            }
            else
            {
                base.WriteObjectPropertyName(name);
            }
        }

        public static void WriteEcmaScriptDate(JsonWriter writer, DateTime value)
        {
            if (value.Kind == DateTimeKind.Unspecified)
            {
                writer.TextWriter.Write("new Date({0:0000},{1},{2},{3},{4},{5},{6})", value.Year, value.Month - 1, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond);
            }
            else
            {
                if (value.Kind == DateTimeKind.Local)
                {
                    value = value.ToUniversalTime();
                }

                long totalMilliseconds = (long)value.Subtract(EcmaScriptWriter.EcmaScriptEpoch).TotalMilliseconds;
                writer.TextWriter.Write("new Date({0})", totalMilliseconds);
            }
        }

        public static void WriteEcmaScriptRegExp(JsonWriter writer, Regex regex)
        {
            EcmaScriptWriter.WriteEcmaScriptRegExp(writer, regex, false);
        }

        public static void WriteEcmaScriptRegExp(JsonWriter writer, Regex regex, bool isGlobal)
        {
            if (regex == null)
            {
                writer.TextWriter.Write("null");
            }
            else
            {
                string str1 = regex.ToString();
                if (string.IsNullOrEmpty(str1))
                {
                    str1 = "(?:)";
                }

                string str2 = !isGlobal ? string.Empty : "g";
                switch (regex.Options & (RegexOptions.IgnoreCase | RegexOptions.Multiline))
                {
                    case RegexOptions.IgnoreCase:
                        str2 += "i";
                        break;
                    case RegexOptions.Multiline:
                        str2 += "m";
                        break;
                    case RegexOptions.IgnoreCase | RegexOptions.Multiline:
                        str2 += "im";
                        break;
                }
                writer.TextWriter.Write('/');
                int length = str1.Length;
                int startIndex = 0;
                for (int index = startIndex; index < length; ++index)
                {
                    if (str1[index] == '/')
                    {
                        writer.TextWriter.Write(str1.Substring(startIndex, index - startIndex));
                        startIndex = index + 1;
                        writer.TextWriter.Write('\\');
                        writer.TextWriter.Write(str1[index]);
                    }
                }
                writer.TextWriter.Write(str1.Substring(startIndex, length - startIndex));
                writer.TextWriter.Write('/');
                writer.TextWriter.Write(str2);
            }
        }
    }
}
