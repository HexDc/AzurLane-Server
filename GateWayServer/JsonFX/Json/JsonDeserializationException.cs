// Decompiled with JetBrains decompiler
// Type: JsonFx.Json.JsonDeserializationException
// Assembly: Assembly-CSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 18E1FC19-6B38-46B3-9D3F-DF1E9429ABBE
// Assembly location: C:\Users\Admin\Desktop\APK Easy Tool portable\1-Decompiled APKs\벽람항로_v1.4.61_apkpure.com\assets\bin\Data\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.Serialization;

namespace JsonFx.Json
{
    public class JsonDeserializationException : JsonSerializationException
    {
        private readonly int index = -1;

        public JsonDeserializationException(string message, int index)
          : base(message)
        {
            this.index = index;
        }

        public JsonDeserializationException(string message, Exception innerException, int index)
          : base(message, innerException)
        {
            this.index = index;
        }

        public JsonDeserializationException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }

        public int Index => index;

        public void GetLineAndColumn(string source, out int line, out int col)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }

            col = 1;
            line = 1;
            bool flag = false;
            for (int index = Math.Min(this.index, source.Length); index > 0; --index)
            {
                if (!flag)
                {
                    ++col;
                }

                if (source[index - 1] == '\n')
                {
                    ++line;
                    flag = true;
                }
            }
        }
    }
}
