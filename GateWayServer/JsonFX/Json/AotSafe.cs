// Decompiled with JetBrains decompiler
// Type: JsonFx.Json.AotSafe
// Assembly: Assembly-CSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 18E1FC19-6B38-46B3-9D3F-DF1E9429ABBE
// Assembly location: C:\Users\Admin\Desktop\APK Easy Tool portable\1-Decompiled APKs\벽람항로_v1.4.61_apkpure.com\assets\bin\Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JsonFx.Json
{
    public class AotSafe
    {
        public static void ForEach<T>(object enumerable, Action<T> action)
        {
            if (enumerable == null)
            {
                return;
            }

            Type type = ((IEnumerable<Type>)enumerable.GetType().GetInterfaces()).First<Type>(x => !x.IsGenericType && x == typeof(IEnumerable));
            if (type == null)
            {
                throw new ArgumentException("Object does not implement IEnumerable interface", nameof(enumerable));
            }

            MethodInfo method = type.GetMethod("GetEnumerator");
            if (method == null)
            {
                throw new InvalidOperationException("Failed to get 'GetEnumberator()' method info from IEnumerable type");
            }

            IEnumerator enumerator = null;
            try
            {
                enumerator = (IEnumerator)method.Invoke(enumerable, null);
                if (enumerator == null)
                {
                    return;
                }

                while (enumerator.MoveNext())
                {
                    action((T)enumerator.Current);
                }
            }
            finally
            {
                if (enumerator is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}
