// Decompiled with JetBrains decompiler
// Type: JsonFx.Json.IJsonSerializable
// Assembly: Assembly-CSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 18E1FC19-6B38-46B3-9D3F-DF1E9429ABBE
// Assembly location: C:\Users\Admin\Desktop\APK Easy Tool portable\1-Decompiled APKs\벽람항로_v1.4.61_apkpure.com\assets\bin\Data\Managed\Assembly-CSharp.dll

namespace JsonFx.Json
{
  public interface IJsonSerializable
  {
    void ReadJson(JsonReader reader);

    void WriteJson(JsonWriter writer);
  }
}
