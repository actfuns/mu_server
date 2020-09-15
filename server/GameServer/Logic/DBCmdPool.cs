using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x0200061C RID: 1564
	public class DBCmdPool
	{
		// Token: 0x06001FBB RID: 8123 RVA: 0x001B7909 File Offset: 0x001B5B09
		internal DBCmdPool(int capacity)
		{
			this.pool = new Stack<DBCommand>(capacity);
		}

		// Token: 0x17000209 RID: 521
		// (get) Token: 0x06001FBC RID: 8124 RVA: 0x001B7920 File Offset: 0x001B5B20
		internal int Count
		{
			get
			{
				int count = 0;
				lock (this.pool)
				{
					count = this.pool.Count;
				}
				return count;
			}
		}

		// Token: 0x06001FBD RID: 8125 RVA: 0x001B797C File Offset: 0x001B5B7C
		internal DBCommand Pop()
		{
			DBCommand result;
			lock (this.pool)
			{
				if (this.pool.Count <= 0)
				{
					result = null;
				}
				else
				{
					result = this.pool.Pop();
				}
			}
			return result;
		}

		// Token: 0x06001FBE RID: 8126 RVA: 0x001B79E4 File Offset: 0x001B5BE4
		internal void Push(DBCommand item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("添加到DBCommandPool 的item不能是空(null)");
			}
			lock (this.pool)
			{
				this.pool.Push(item);
			}
		}

		// Token: 0x04002CC0 RID: 11456
		private Stack<DBCommand> pool;
	}
}
