// Decompiled with JetBrains decompiler
// Type: JsonFx.Json.JsonSpecifiedPropertyAttribute
// Assembly: Assembly-CSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 18E1FC19-6B38-46B3-9D3F-DF1E9429ABBE
// Assembly location: C:\Users\Admin\Desktop\APK Easy Tool portable\1-Decompiled APKs\벽람항로_v1.4.61_apkpure.com\assets\bin\Data\Managed\Assembly-CSharp.dll

using System;
using System.Reflection;

namespace JsonFx.Json
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class JsonSpecifiedPropertyAttribute : Attribute
    {
        private string specifiedProperty;

        public JsonSpecifiedPropertyAttribute(string propertyName)
        {
            specifiedProperty = propertyName;
        }

        public string SpecifiedProperty
        {
            get => specifiedProperty;
            set => specifiedProperty = value;
        }

        public static string GetJsonSpecifiedProperty(MemberInfo memberInfo)
        {
            return memberInfo == null || !Attribute.IsDefined(memberInfo, typeof(JsonSpecifiedPropertyAttribute)) ? null : ((JsonSpecifiedPropertyAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(JsonSpecifiedPropertyAttribute))).SpecifiedProperty;
        }
    }
}
