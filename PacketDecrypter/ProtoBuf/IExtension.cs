using System;
using System.IO;

namespace ProtoBuf
{
	// Token: 0x020003CE RID: 974
	public interface IExtension
	{
		// Token: 0x060032B9 RID: 12985
		Stream BeginAppend();

		// Token: 0x060032BA RID: 12986
		void EndAppend(Stream stream, bool commit);

		// Token: 0x060032BB RID: 12987
		Stream BeginQuery();

		// Token: 0x060032BC RID: 12988
		void EndQuery(Stream stream);

		// Token: 0x060032BD RID: 12989
		int GetLength();
	}
}
