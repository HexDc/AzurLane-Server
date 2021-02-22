using System;
using System.Collections;

namespace ProtoBuf.Meta
{
    // Token: 0x020003D3 RID: 979
    internal class BasicList : IEnumerable
	{
		// Token: 0x060032D0 RID: 13008 RVA: 0x00127F50 File Offset: 0x00126350
		public void CopyTo(Array array, int offset)
		{
			head.CopyTo(array, offset);
		}

		// Token: 0x060032D1 RID: 13009 RVA: 0x00127F60 File Offset: 0x00126360
		public int Add(object value)
		{
			return (head = head.Append(value)).Length - 1;
		}

        // Token: 0x170002B6 RID: 694
        public object this[int index] => head[index];

        // Token: 0x060032D3 RID: 13011 RVA: 0x00127F97 File Offset: 0x00126397
        public void Trim()
		{
			head = head.Trim();
		}

        // Token: 0x170002B7 RID: 695
        // (get) Token: 0x060032D4 RID: 13012 RVA: 0x00127FAA File Offset: 0x001263AA
        public int Count => head.Length;

        // Token: 0x060032D5 RID: 13013 RVA: 0x00127FB7 File Offset: 0x001263B7
        IEnumerator IEnumerable.GetEnumerator()
		{
			return new BasicList.NodeEnumerator(head);
		}

		// Token: 0x060032D6 RID: 13014 RVA: 0x00127FC9 File Offset: 0x001263C9
		public BasicList.NodeEnumerator GetEnumerator()
		{
			return new BasicList.NodeEnumerator(head);
		}

		// Token: 0x060032D7 RID: 13015 RVA: 0x00127FD6 File Offset: 0x001263D6
		internal int IndexOf(BasicList.MatchPredicate predicate, object ctx)
		{
			return head.IndexOf(predicate, ctx);
		}

		// Token: 0x060032D8 RID: 13016 RVA: 0x00127FE5 File Offset: 0x001263E5
		internal int IndexOfString(string value)
		{
			return head.IndexOfString(value);
		}

		// Token: 0x060032D9 RID: 13017 RVA: 0x00127FF3 File Offset: 0x001263F3
		internal int IndexOfReference(object instance)
		{
			return head.IndexOfReference(instance);
		}

		// Token: 0x060032DA RID: 13018 RVA: 0x00128004 File Offset: 0x00126404
		internal bool Contains(object value)
		{
			foreach (object objA in this)
			{
				if (object.Equals(objA, value))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060032DB RID: 13019 RVA: 0x00128040 File Offset: 0x00126440
		internal static BasicList GetContiguousGroups(int[] keys, object[] values)
		{
			if (keys == null) throw new ArgumentNullException("keys");
			if (values == null) throw new ArgumentNullException("values");
			if (values.Length < keys.Length) throw new ArgumentException("Not all keys are covered by values", "values");
			BasicList outer = new BasicList();
			Group group = null;
			for (int i = 0; i < keys.Length; i++)
			{
				if (i == 0 || keys[i] != keys[i - 1]) { group = null; }
				if (group == null)
				{
					group = new Group(keys[i]);
					outer.Add(group);
				}
				group.Items.Add(values[i]);
			}
			return outer;
		}

		// Token: 0x040023E4 RID: 9188
		private static readonly BasicList.Node nil = new BasicList.Node(null, 0);

		// Token: 0x040023E5 RID: 9189
		protected BasicList.Node head = BasicList.nil;

		// Token: 0x020003D4 RID: 980
		public struct NodeEnumerator : IEnumerator
		{
			// Token: 0x060032DD RID: 13021 RVA: 0x001280F0 File Offset: 0x001264F0
			internal NodeEnumerator(BasicList.Node node)
			{
				position = -1;
				this.node = node;
			}

			// Token: 0x060032DE RID: 13022 RVA: 0x00128100 File Offset: 0x00126500
			void IEnumerator.Reset()
			{
				position = -1;
			}

            // Token: 0x170002B8 RID: 696
            // (get) Token: 0x060032DF RID: 13023 RVA: 0x00128109 File Offset: 0x00126509
            public object Current => node[position];

            // Token: 0x060032E0 RID: 13024 RVA: 0x0012811C File Offset: 0x0012651C
            public bool MoveNext()
			{
				int length = node.Length;
				return position <= length && ++position < length;
			}

			// Token: 0x040023E6 RID: 9190
			private int position;

			// Token: 0x040023E7 RID: 9191
			private readonly BasicList.Node node;
		}

		// Token: 0x020003D5 RID: 981
		internal sealed class Node
		{
			// Token: 0x060032E1 RID: 13025 RVA: 0x00128158 File Offset: 0x00126558
			internal Node(object[] data, int length)
			{
				this.data = data;
				this.length = length;
			}

			// Token: 0x170002B9 RID: 697
			public object this[int index]
			{
				get
				{
					if (index >= 0 && index < length)
					{
						return data[index];
					}
					throw new ArgumentOutOfRangeException("index");
				}
				set
				{
					if (index >= 0 && index < length)
					{
						data[index] = value;
						return;
					}
					throw new ArgumentOutOfRangeException("index");
				}
			}

            // Token: 0x170002BA RID: 698
            // (get) Token: 0x060032E4 RID: 13028 RVA: 0x001281C4 File Offset: 0x001265C4
            public int Length => length;

            // Token: 0x060032E5 RID: 13029 RVA: 0x001281CC File Offset: 0x001265CC
            public void RemoveLastWithMutate()
			{
				if (length == 0)
				{
					throw new InvalidOperationException();
				}
				length--;
			}

			// Token: 0x060032E6 RID: 13030 RVA: 0x001281F0 File Offset: 0x001265F0
			public BasicList.Node Append(object value)
			{
				int num = length + 1;
				object[] array;
				if (data == null)
				{
					array = new object[10];
				}
				else if (length == data.Length)
				{
					array = new object[data.Length * 2];
					Array.Copy(data, array, length);
				}
				else
				{
					array = data;
				}
				array[length] = value;
				return new BasicList.Node(array, num);
			}

			// Token: 0x060032E7 RID: 13031 RVA: 0x00128270 File Offset: 0x00126670
			public BasicList.Node Trim()
			{
				if (length == 0 || length == data.Length)
				{
					return this;
				}
				object[] destinationArray = new object[length];
				Array.Copy(data, destinationArray, length);
				return new BasicList.Node(destinationArray, length);
			}

			// Token: 0x060032E8 RID: 13032 RVA: 0x001282C8 File Offset: 0x001266C8
			internal int IndexOfString(string value)
			{
				for (int i = 0; i < length; i++)
				{
					if (value == (string)data[i])
					{
						return i;
					}
				}
				return -1;
			}

			// Token: 0x060032E9 RID: 13033 RVA: 0x00128308 File Offset: 0x00126708
			internal int IndexOfReference(object instance)
			{
				for (int i = 0; i < length; i++)
				{
					if (instance == data[i])
					{
						return i;
					}
				}
				return -1;
			}

			// Token: 0x060032EA RID: 13034 RVA: 0x00128340 File Offset: 0x00126740
			internal int IndexOf(BasicList.MatchPredicate predicate, object ctx)
			{
				for (int i = 0; i < length; i++)
				{
					if (predicate(data[i], ctx))
					{
						return i;
					}
				}
				return -1;
			}

			// Token: 0x060032EB RID: 13035 RVA: 0x0012837B File Offset: 0x0012677B
			internal void CopyTo(Array array, int offset)
			{
				if (length > 0)
				{
					Array.Copy(data, 0, array, offset, length);
				}
			}

			// Token: 0x060032EC RID: 13036 RVA: 0x0012839D File Offset: 0x0012679D
			internal void Clear()
			{
				if (data != null)
				{
					Array.Clear(data, 0, data.Length);
				}
				length = 0;
			}

			// Token: 0x040023E8 RID: 9192
			private readonly object[] data;

			// Token: 0x040023E9 RID: 9193
			private int length;
		}

		// Token: 0x020003D6 RID: 982
		// (Invoke) Token: 0x060032EE RID: 13038
		internal delegate bool MatchPredicate(object value, object ctx);

		// Token: 0x020003D7 RID: 983
		internal sealed class Group
		{
			// Token: 0x060032F1 RID: 13041 RVA: 0x001283C5 File Offset: 0x001267C5
			public Group(int first)
			{
				First = first;
				Items = new BasicList();
			}

			// Token: 0x040023EA RID: 9194
			public readonly int First;

			// Token: 0x040023EB RID: 9195
			public readonly BasicList Items;
		}
	}
}
