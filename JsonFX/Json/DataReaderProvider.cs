﻿// Decompiled with JetBrains decompiler
// Type: JsonFx.Json.DataReaderProvider
// Assembly: Assembly-CSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 18E1FC19-6B38-46B3-9D3F-DF1E9429ABBE
// Assembly location: C:\Users\Admin\Desktop\APK Easy Tool portable\1-Decompiled APKs\벽람항로_v1.4.61_apkpure.com\assets\bin\Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

namespace JsonFx.Json
{
    public class DataReaderProvider : IDataReaderProvider
    {
        private readonly IDictionary<string, IDataReader> ReadersByMime = new Dictionary<string, IDataReader>(StringComparer.OrdinalIgnoreCase);

        public DataReaderProvider(IEnumerable<IDataReader> readers)
        {
            if (readers == null)
            {
                return;
            }

            foreach (IDataReader reader in readers)
            {
                if (!string.IsNullOrEmpty(reader.ContentType))
                {
                    ReadersByMime[reader.ContentType] = reader;
                }
            }
        }

        public IDataReader Find(string contentTypeHeader)
        {
            string mediaType = DataWriterProvider.ParseMediaType(contentTypeHeader);
            return ReadersByMime.ContainsKey(mediaType) ? ReadersByMime[mediaType] : null;
        }
    }
}
