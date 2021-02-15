// Decompiled with JetBrains decompiler
// Type: JsonFx.Json.JsonDataReader
// Assembly: Assembly-CSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 18E1FC19-6B38-46B3-9D3F-DF1E9429ABBE
// Assembly location: C:\Users\Admin\Desktop\APK Easy Tool portable\1-Decompiled APKs\벽람항로_v1.4.61_apkpure.com\assets\bin\Data\Managed\Assembly-CSharp.dll

using System;
using System.IO;

namespace JsonFx.Json
{
  public class JsonDataReader : IDataReader
  {
    public const string JsonMimeType = "application/json";
    public const string JsonFileExtension = ".json";
    private readonly JsonReaderSettings Settings;

    public JsonDataReader(JsonReaderSettings settings)
    {
      if (settings == null)
        throw new ArgumentNullException(nameof (settings));
      this.Settings = settings;
    }

    public string ContentType
    {
      get
      {
        return "application/json";
      }
    }

    public object Deserialize(TextReader input, Type type)
    {
      return new JsonReader(input, this.Settings).Deserialize(type);
    }

    public static JsonReaderSettings CreateSettings(bool allowNullValueTypes)
    {
      return new JsonReaderSettings()
      {
        AllowNullValueTypes = allowNullValueTypes
      };
    }
  }
}
