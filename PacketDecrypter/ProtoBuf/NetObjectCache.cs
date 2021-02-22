using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ProtoBuf.Meta;

namespace ProtoBuf
{
	// Token: 0x020003EB RID: 1003
	internal sealed class NetObjectCache
	{
		// Token: 0x170002FB RID: 763
		// (get) Token: 0x0600341E RID: 13342 RVA: 0x0012F557 File Offset: 0x0012D957
		private MutableList List
		{
			get
			{
				if (this.underlyingList == null)
				{
					this.underlyingList = new MutableList();
				}
				return this.underlyingList;
			}
		}

		// Token: 0x0600341F RID: 13343 RVA: 0x0012F578 File Offset: 0x0012D978
		internal object GetKeyedObject(int key)
		{
			if (key-- == 0)
			{
				if (this.rootObject == null)
				{
					throw new ProtoException("No root object assigned");
				}
				return this.rootObject;
			}
			else
			{
				BasicList list = this.List;
				if (key < 0 || key >= list.Count)
				{
					throw new ProtoException("Internal error; a missing key occurred");
				}
				object obj = list[key];
				if (obj == null)
				{
					throw new ProtoException("A deferred key does not have a value yet");
				}
				return obj;
			}
		}

		// Token: 0x06003420 RID: 13344 RVA: 0x0012F5EC File Offset: 0x0012D9EC
		internal void SetKeyedObject(int key, object value)
		{
			if (key-- == 0)
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (this.rootObject != null && this.rootObject != value)
				{
					throw new ProtoException("The root object cannot be reassigned");
				}
				this.rootObject = value;
			}
			else
			{
				MutableList list = this.List;
				if (key < list.Count)
				{
					object obj = list[key];
					if (obj == null)
					{
						list[key] = value;
					}
					else if (!object.ReferenceEquals(obj, value))
					{
						throw new ProtoException("Reference-tracked objects cannot change reference");
					}
				}
				else if (key != list.Add(value))
				{
					throw new ProtoException("Internal error; a key mismatch occurred");
				}
			}
		}

		// Token: 0x06003421 RID: 13345 RVA: 0x0012F6A8 File Offset: 0x0012DAA8
		internal int AddObjectKey(object value, out bool existing)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value == this.rootObject)
			{
				existing = true;
				return 0;
			}
			string text = value as string;
			BasicList list = this.List;
			int num;
			if (text == null)
			{
				if (this.objectKeys == null)
				{
					this.objectKeys = new Dictionary<object, int>(NetObjectCache.ReferenceComparer.Default);
					num = -1;
				}
				else if (!this.objectKeys.TryGetValue(value, out num))
				{
					num = -1;
				}
			}
			else if (this.stringKeys == null)
			{
				this.stringKeys = new Dictionary<string, int>();
				num = -1;
			}
			else if (!this.stringKeys.TryGetValue(text, out num))
			{
				num = -1;
			}
			if (!(existing = (num >= 0)))
			{
				num = list.Add(value);
				if (text == null)
				{
					this.objectKeys.Add(value, num);
				}
				else
				{
					this.stringKeys.Add(text, num);
				}
			}
			return num + 1;
		}

		// Token: 0x06003422 RID: 13346 RVA: 0x0012F79C File Offset: 0x0012DB9C
		internal void RegisterTrappedObject(object value)
		{
			if (this.rootObject == null)
			{
				this.rootObject = value;
			}
			else if (this.underlyingList != null)
			{
				for (int i = this.trapStartIndex; i < this.underlyingList.Count; i++)
				{
					this.trapStartIndex = i + 1;
					if (this.underlyingList[i] == null)
					{
						this.underlyingList[i] = value;
						break;
					}
				}
			}
		}

		// Token: 0x06003423 RID: 13347 RVA: 0x0012F818 File Offset: 0x0012DC18
		internal void Clear()
		{
			this.trapStartIndex = 0;
			this.rootObject = null;
			if (this.underlyingList != null)
			{
				this.underlyingList.Clear();
			}
			if (this.stringKeys != null)
			{
				this.stringKeys.Clear();
			}
			if (this.objectKeys != null)
			{
				this.objectKeys.Clear();
			}
		}

		// Token: 0x04002450 RID: 9296
		internal const int Root = 0;

		// Token: 0x04002451 RID: 9297
		private MutableList underlyingList;

		// Token: 0x04002452 RID: 9298
		private object rootObject;

		// Token: 0x04002453 RID: 9299
		private int trapStartIndex;

		// Token: 0x04002454 RID: 9300
		private Dictionary<string, int> stringKeys;

		// Token: 0x04002455 RID: 9301
		private Dictionary<object, int> objectKeys;

		// Token: 0x020003EC RID: 1004
		private sealed class ReferenceComparer : IEqualityComparer<object>
		{
			// Token: 0x06003424 RID: 13348 RVA: 0x0012F875 File Offset: 0x0012DC75
			private ReferenceComparer()
			{
			}

			// Token: 0x06003425 RID: 13349 RVA: 0x0012F87D File Offset: 0x0012DC7D
			bool IEqualityComparer<object>.Equals(object x, object y)
			{
				return x == y;
			}

			// Token: 0x06003426 RID: 13350 RVA: 0x0012F883 File Offset: 0x0012DC83
			int IEqualityComparer<object>.GetHashCode(object obj)
			{
				return RuntimeHelpers.GetHashCode(obj);
			}

			// Token: 0x04002456 RID: 9302
			public static readonly NetObjectCache.ReferenceComparer Default = new NetObjectCache.ReferenceComparer();
		}
	}
}
