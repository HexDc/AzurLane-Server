// Decompiled with JetBrains decompiler
// Type: JsonFx.Xml.XmlDataReader
// Assembly: Assembly-CSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 18E1FC19-6B38-46B3-9D3F-DF1E9429ABBE
// Assembly location: C:\Users\Admin\Desktop\APK Easy Tool portable\1-Decompiled APKs\벽람항로_v1.4.61_apkpure.com\assets\bin\Data\Managed\Assembly-CSharp.dll

using JsonFx.Json;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JsonFx.Xml
{
    public class XmlDataReader : IDataReader
    {
        public const string XmlMimeType = "application/xml";
        private readonly XmlReaderSettings Settings;
        private readonly XmlSerializerNamespaces Namespaces;

        public XmlDataReader(XmlReaderSettings settings, XmlSerializerNamespaces namespaces)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            Settings = settings;
            if (namespaces == null)
            {
                namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);
            }
            Namespaces = namespaces;
        }

        public string ContentType => "application/xml";

        public object Deserialize(TextReader input, Type type)
        {
            XmlReader xmlReader = XmlReader.Create(input, Settings);
            int content = (int)xmlReader.MoveToContent();
            return new XmlSerializer(type).Deserialize(xmlReader);
        }

        public static XmlReaderSettings CreateSettings()
        {
            return new XmlReaderSettings()
            {
                CloseInput = false,
                ConformanceLevel = ConformanceLevel.Auto,
                IgnoreComments = true,
                IgnoreWhitespace = true,
                IgnoreProcessingInstructions = true
            };
        }
    }
}
