// Decompiled with JetBrains decompiler
// Type: JsonFx.Json.JsonWriterSettings
// Assembly: Assembly-CSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 18E1FC19-6B38-46B3-9D3F-DF1E9429ABBE
// Assembly location: C:\Users\Admin\Desktop\APK Easy Tool portable\1-Decompiled APKs\벽람항로_v1.4.61_apkpure.com\assets\bin\Data\Managed\Assembly-CSharp.dll

using System;

namespace JsonFx.Json
{
    public class JsonWriterSettings
    {
        private int maxDepth = 25;
        private string newLine = Environment.NewLine;
        private string tab = "\t";
        private WriteDelegate<DateTime> dateTimeSerializer;
        private bool prettyPrint;
        private string typeHintName;
        private bool useXmlSerializationAttributes;

        public virtual string TypeHintName
        {
            get => typeHintName;
            set => typeHintName = value;
        }

        public virtual bool PrettyPrint
        {
            get => prettyPrint;
            set => prettyPrint = value;
        }

        public virtual string Tab
        {
            get => tab;
            set => tab = value;
        }

        public virtual string NewLine
        {
            get => newLine;
            set => newLine = value;
        }

        public virtual int MaxDepth
        {
            get => maxDepth;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("MaxDepth must be a positive integer as it controls the maximum nesting level of serialized objects.");
                }

                maxDepth = value;
            }
        }

        public virtual bool UseXmlSerializationAttributes
        {
            get => useXmlSerializationAttributes;
            set => useXmlSerializationAttributes = value;
        }

        public virtual WriteDelegate<DateTime> DateTimeSerializer
        {
            get => dateTimeSerializer;
            set => dateTimeSerializer = value;
        }
    }
}
