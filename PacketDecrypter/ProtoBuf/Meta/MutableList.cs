namespace ProtoBuf.Meta
{
    // Token: 0x020003D2 RID: 978
    internal sealed class MutableList : BasicList
	{
		// Token: 0x170002B5 RID: 693
		public new object this[int index]
        {
            get => head[index];
            set => head[index] = value;
        }

        // Token: 0x060032CD RID: 13005 RVA: 0x00128404 File Offset: 0x00126804
        public void RemoveLast()
		{
			head.RemoveLastWithMutate();
		}

		// Token: 0x060032CE RID: 13006 RVA: 0x00128411 File Offset: 0x00126811
		public void Clear()
		{
			head.Clear();
		}
	}
}
