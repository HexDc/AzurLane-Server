using System;
using System.IO;

namespace ProtoBuf
{
	// Token: 0x020003C2 RID: 962
	public sealed class BufferExtension : IExtension
	{
		// Token: 0x06003272 RID: 12914 RVA: 0x00126FA3 File Offset: 0x001253A3
		int IExtension.GetLength()
		{
			return (this.buffer != null) ? this.buffer.Length : 0;
		}

		// Token: 0x06003273 RID: 12915 RVA: 0x00126FBE File Offset: 0x001253BE
		Stream IExtension.BeginAppend()
		{
			return new MemoryStream();
		}

		// Token: 0x06003274 RID: 12916 RVA: 0x00126FC8 File Offset: 0x001253C8
		void IExtension.EndAppend(Stream stream, bool commit)
		{
			try
			{
				int num;
				if (commit && (num = (int)stream.Length) > 0)
				{
					MemoryStream memoryStream = (MemoryStream)stream;
					if (this.buffer == null)
					{
						this.buffer = memoryStream.ToArray();
					}
					else
					{
						int num2 = this.buffer.Length;
						byte[] to = new byte[num2 + num];
						Helpers.BlockCopy(this.buffer, 0, to, 0, num2);
						Helpers.BlockCopy(Helpers.GetBuffer(memoryStream), 0, to, num2, num);
						this.buffer = to;
					}
				}
			}
			finally
			{
				if (stream != null)
				{
					((IDisposable)stream).Dispose();
				}
			}
		}

		// Token: 0x06003275 RID: 12917 RVA: 0x0012706C File Offset: 0x0012546C
		Stream IExtension.BeginQuery()
		{
			return (this.buffer != null) ? new MemoryStream(this.buffer) : Stream.Null;
		}

		// Token: 0x06003276 RID: 12918 RVA: 0x00127090 File Offset: 0x00125490
		void IExtension.EndQuery(Stream stream)
		{
			try
			{
			}
			finally
			{
				if (stream != null)
				{
					((IDisposable)stream).Dispose();
				}
			}
		}

		// Token: 0x040023BC RID: 9148
		private byte[] buffer;
	}
}
