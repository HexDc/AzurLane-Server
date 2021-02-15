// Decompiled with JetBrains decompiler
// Type: JsonFx.Json.JsonDataWriter
// Assembly: Assembly-CSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 18E1FC19-6B38-46B3-9D3F-DF1E9429ABBE
// Assembly location: C:\Users\Admin\Desktop\APK Easy Tool portable\1-Decompiled APKs\벽람항로_v1.4.61_apkpure.com\assets\bin\Data\Managed\Assembly-CSharp.dll

using System;
using System.IO;
using System.Text;

namespace JsonFx.Json
{
    public class JsonDataWriter : IDataWriter
    {
        public const string JsonMimeType = "application/json";
        public const string JsonFileExtension = ".json";
        private readonly JsonWriterSettings Settings;

        public JsonDataWriter(JsonWriterSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            Settings = settings;
        }

        public Encoding ContentEncoding => Encoding.UTF8;

        public string ContentType => "application/json";

        public string FileExtension => ".json";

        public void Serialize(TextWriter output, object data)
        {
            new JsonWriter(output, Settings).Write(data);
        }

        public static JsonWriterSettings CreateSettings(bool prettyPrint)
        {
            return new JsonWriterSettings()
            {
                PrettyPrint = prettyPrint
            };
        }
    }
}
