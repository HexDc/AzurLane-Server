// Decompiled with JetBrains decompiler
// Type: JsonFx.Json.DataWriterProvider
// Assembly: Assembly-CSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 18E1FC19-6B38-46B3-9D3F-DF1E9429ABBE
// Assembly location: C:\Users\Admin\Desktop\APK Easy Tool portable\1-Decompiled APKs\벽람항로_v1.4.61_apkpure.com\assets\bin\Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace JsonFx.Json
{
    public class DataWriterProvider : IDataWriterProvider
    {
        private readonly IDictionary<string, IDataWriter> WritersByExt = new Dictionary<string, IDataWriter>(StringComparer.OrdinalIgnoreCase);
        private readonly IDictionary<string, IDataWriter> WritersByMime = new Dictionary<string, IDataWriter>(StringComparer.OrdinalIgnoreCase);
        private readonly IDataWriter DefaultWriter;

        public DataWriterProvider(IEnumerable<IDataWriter> writers)
        {
            if (writers == null)
            {
                return;
            }

            foreach (IDataWriter writer in writers)
            {
                if (DefaultWriter == null)
                {
                    DefaultWriter = writer;
                }

                if (!string.IsNullOrEmpty(writer.ContentType))
                {
                    WritersByMime[writer.ContentType] = writer;
                }

                if (!string.IsNullOrEmpty(writer.ContentType))
                {
                    WritersByExt[DataWriterProvider.NormalizeExtension(writer.FileExtension)] = writer;
                }
            }
        }

        public IDataWriter DefaultDataWriter => DefaultWriter;

        public IDataWriter Find(string extension)
        {
            extension = DataWriterProvider.NormalizeExtension(extension);
            return WritersByExt.ContainsKey(extension) ? WritersByExt[extension] : null;
        }

        public IDataWriter Find(string acceptHeader, string contentTypeHeader)
        {
            foreach (string header in DataWriterProvider.ParseHeaders(acceptHeader, contentTypeHeader))
            {
                if (WritersByMime.ContainsKey(header))
                {
                    return WritersByMime[header];
                }
            }
            return null;
        }

        public static IEnumerable<string> ParseHeaders(string accept, string contentType)
        {
            string mime;

            // check for a matching accept type
            foreach (string type in DataWriterProvider.SplitTrim(accept, ','))
            {
                mime = DataWriterProvider.ParseMediaType(type);
                if (!string.IsNullOrEmpty(mime))
                {
                    yield return mime;
                }
            }

            // fallback on content-type
            mime = DataWriterProvider.ParseMediaType(contentType);
            if (!string.IsNullOrEmpty(mime))
            {
                yield return mime;
            }
        }

        public static string ParseMediaType(string type)
        {
            using (IEnumerator<string> enumerator = DataWriterProvider.SplitTrim(type, ';').GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    return enumerator.Current;
                }
            }
            return string.Empty;
        }

        private static IEnumerable<string> SplitTrim(string source, char ch)
        {
            if (string.IsNullOrEmpty(source))
            {
                yield break;
            }

            int length = source.Length;
            for (int prev = 0, next = 0; prev < length && next >= 0; prev = next + 1)
            {
                next = source.IndexOf(ch, prev);
                if (next < 0)
                {
                    next = length;
                }

                string part = source.Substring(prev, next - prev).Trim();
                if (part.Length > 0)
                {
                    yield return part;
                }
            }
        }

        private static string NormalizeExtension(string extension)
        {
            return string.IsNullOrEmpty(extension) ? string.Empty : Path.GetExtension(extension);
        }
    }
}
