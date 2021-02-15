// Decompiled with JetBrains decompiler
// Type: JsonFx.Json.JsonReaderSettings
// Assembly: Assembly-CSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 18E1FC19-6B38-46B3-9D3F-DF1E9429ABBE
// Assembly location: C:\Users\Admin\Desktop\APK Easy Tool portable\1-Decompiled APKs\벽람항로_v1.4.61_apkpure.com\assets\bin\Data\Managed\Assembly-CSharp.dll

using System;

namespace JsonFx.Json
{
    public class JsonReaderSettings
    {
        internal readonly TypeCoercionUtility Coercion = new TypeCoercionUtility();
        private bool allowUnquotedObjectKeys;
        private string typeHintName;

        public bool AllowNullValueTypes
        {
            get => Coercion.AllowNullValueTypes;
            set => Coercion.AllowNullValueTypes = value;
        }

        public bool AllowUnquotedObjectKeys
        {
            get => allowUnquotedObjectKeys;
            set => allowUnquotedObjectKeys = value;
        }

        public string TypeHintName
        {
            get => typeHintName;
            set => typeHintName = value;
        }

        internal bool IsTypeHintName(string name)
        {
            return !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(typeHintName) && StringComparer.Ordinal.Equals(typeHintName, name);
        }
    }
}
