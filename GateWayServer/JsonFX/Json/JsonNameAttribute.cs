// Decompiled with JetBrains decompiler
// Type: JsonFx.Json.JsonNameAttribute
// Assembly: Assembly-CSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 18E1FC19-6B38-46B3-9D3F-DF1E9429ABBE
// Assembly location: C:\Users\Admin\Desktop\APK Easy Tool portable\1-Decompiled APKs\벽람항로_v1.4.61_apkpure.com\assets\bin\Data\Managed\Assembly-CSharp.dll

using System;
using System.Reflection;

namespace JsonFx.Json
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class JsonNameAttribute : Attribute
    {
        private string jsonName;

        public JsonNameAttribute()
        {
        }

        public JsonNameAttribute(string jsonName)
        {
            this.jsonName = EcmaScriptIdentifier.EnsureValidIdentifier(jsonName, false);
        }

        public string Name
        {
            get => jsonName;
            set => jsonName = EcmaScriptIdentifier.EnsureValidIdentifier(value, false);
        }

        public static string GetJsonName(object value)
        {
            if (value == null)
            {
                return null;
            }

            Type type = value.GetType();
            MemberInfo element;
            if (type.IsEnum)
            {
                string name = Enum.GetName(type, value);
                if (string.IsNullOrEmpty(name))
                {
                    return null;
                }

                element = type.GetField(name);
            }
            else
            {
                element = value as MemberInfo;
            }

            if (element == null)
            {
                throw new ArgumentException();
            }

            return !Attribute.IsDefined(element, typeof(JsonNameAttribute)) ? null : ((JsonNameAttribute)Attribute.GetCustomAttribute(element, typeof(JsonNameAttribute))).Name;
        }
    }
}
