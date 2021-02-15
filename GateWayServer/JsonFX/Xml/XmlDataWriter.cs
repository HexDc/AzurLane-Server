// Decompiled with JetBrains decompiler
// Type: JsonFx.Xml.XmlDataWriter
// Assembly: Assembly-CSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 18E1FC19-6B38-46B3-9D3F-DF1E9429ABBE
// Assembly location: C:\Users\Admin\Desktop\APK Easy Tool portable\1-Decompiled APKs\벽람항로_v1.4.61_apkpure.com\assets\bin\Data\Managed\Assembly-CSharp.dll

using JsonFx.Json;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace JsonFx.Xml
{
    public class XmlDataWriter : IDataWriter
    {
        public const string XmlMimeType = "application/xml";
        public const string XmlFileExtension = ".xml";
        private readonly XmlWriterSettings Settings;
        private readonly XmlSerializerNamespaces Namespaces;

        public XmlDataWriter(XmlWriterSettings settings, XmlSerializerNamespaces namespaces)
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

        public Encoding ContentEncoding => Settings.Encoding ?? Encoding.UTF8;

        public string ContentType => "application/xml";

        public string FileExtension => ".xml";

        public void Serialize(TextWriter output, object data)
        {
            if (data == null)
            {
                return;
            }

            if (Settings.Encoding == null)
            {
                Settings.Encoding = ContentEncoding;
            }

            XmlWriter xmlWriter = XmlWriter.Create(output, Settings);
            new XmlSerializer(data.GetType()).Serialize(xmlWriter, data, Namespaces);
        }

        public static XmlWriterSettings CreateSettings(
          Encoding encoding,
          bool prettyPrint)
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
            {
                CheckCharacters = true,
                CloseOutput = false,
                ConformanceLevel = ConformanceLevel.Auto,
                Encoding = encoding,
                OmitXmlDeclaration = true
            };
            if (prettyPrint)
            {
                xmlWriterSettings.Indent = true;
                xmlWriterSettings.IndentChars = "\t";
            }
            else
            {
                xmlWriterSettings.Indent = false;
                xmlWriterSettings.NewLineChars = string.Empty;
            }
            xmlWriterSettings.NewLineHandling = NewLineHandling.Replace;
            return xmlWriterSettings;
        }
    }
}
