// Decompiled with JetBrains decompiler
// Type: JsonFx.Json.JsonIgnoreAttribute
// Assembly: Assembly-CSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 18E1FC19-6B38-46B3-9D3F-DF1E9429ABBE
// Assembly location: C:\Users\Admin\Desktop\APK Easy Tool portable\1-Decompiled APKs\벽람항로_v1.4.61_apkpure.com\assets\bin\Data\Managed\Assembly-CSharp.dll

using System;
using System.Reflection;
using System.Xml.Serialization;

namespace JsonFx.Json
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public sealed class JsonIgnoreAttribute : Attribute
    {
        public static bool IsJsonIgnore(object value)
        {
            if (value == null)
                return false;
            Type type = value.GetType();
            ICustomAttributeProvider attributeProvider = !type.IsEnum ? value as ICustomAttributeProvider : (ICustomAttributeProvider)type.GetField(Enum.GetName(type, value));
            if (attributeProvider == null)
                throw new ArgumentException();
            return attributeProvider.IsDefined(typeof(JsonIgnoreAttribute), true);
        }

        public static bool IsXmlIgnore(object value)
        {
            if (value == null)
                return false;
            Type type = value.GetType();
            ICustomAttributeProvider attributeProvider = !type.IsEnum ? value as ICustomAttributeProvider : (ICustomAttributeProvider)type.GetField(Enum.GetName(type, value));
            if (attributeProvider == null)
                throw new ArgumentException();
            return attributeProvider.IsDefined(typeof(XmlIgnoreAttribute), true);
        }
    }
}
