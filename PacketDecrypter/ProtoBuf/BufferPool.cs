using System;
using System.Threading;

namespace ProtoBuf
{
	// Token: 0x020003C3 RID: 963
	internal sealed class BufferPool
	{
		// Token: 0x06003277 RID: 12919 RVA: 0x001270C4 File Offset: 0x001254C4
		private BufferPool()
		{
		}

		// Token: 0x06003278 RID: 12920 RVA: 0x001270CC File Offset: 0x001254CC
		internal static void Flush()
		{
			for (int i = 0; i < BufferPool.pool.Length; i++)
			{
				Interlocked.Exchange(ref BufferPool.pool[i], null);
			}
		}

		// Token: 0x06003279 RID: 12921 RVA: 0x00127104 File Offset: 0x00125504
		internal static byte[] GetBuffer()
		{
			for (int i = 0; i < BufferPool.pool.Length; i++)
			{
				object obj;
				if ((obj = Interlocked.Exchange(ref BufferPool.pool[i], null)) != null)
				{
					return (byte[])obj;
				}
			}
			return new byte[1024];
		}

		// Token: 0x0600327A RID: 12922 RVA: 0x00127154 File Offset: 0x00125554
		internal static void ResizeAndFlushLeft(ref byte[] buffer, int toFitAtLeastBytes, int copyFromIndex, int copyBytes)
		{
			int num = buffer.Length * 2;
			if (num < toFitAtLeastBytes)
			{
				num = toFitAtLeastBytes;
			}
			byte[] array = new byte[num];
			if (copyBytes > 0)
			{
				Helpers.BlockCopy(buffer, copyFromIndex, array, 0, copyBytes);
			}
			if (buffer.Length == 1024)
			{
				BufferPool.ReleaseBufferToPool(ref buffer);
			}
			buffer = array;
		}

		// Token: 0x0600327B RID: 12923 RVA: 0x001271A4 File Offset: 0x001255A4
		internal static void ReleaseBufferToPool(ref byte[] buffer)
		{
			if (buffer == null)
			{
				return;
			}
			if (buffer.Length == 1024)
			{
				for (int i = 0; i < BufferPool.pool.Length; i++)
				{
					if (Interlocked.CompareExchange(ref BufferPool.pool[i], buffer, null) == null)
					{
						break;
					}
				}
			}
			buffer = null;
		}

		// Token: 0x040023BD RID: 9149
		private const int PoolSize = 20;

		// Token: 0x040023BE RID: 9150
		internal const int BufferLength = 1024;

		// Token: 0x040023BF RID: 9151
		private static readonly object[] pool = new object[20];
	}
}
