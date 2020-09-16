using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class DBCmdPool
	{
		
		internal DBCmdPool(int capacity)
		{
			this.pool = new Stack<DBCommand>(capacity);
		}

		
		
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

		
		private Stack<DBCommand> pool;
	}
}
