using System;
using System.Collections.Generic;
using GameServer.Interface;

namespace GameServer.Logic.NewBufferExt
{
	
	public class BufferExtManager
	{
		
		public void AddBufferItem(int id, IBufferItem bufferItem)
		{
			lock (this.BufferItemDict)
			{
				this.BufferItemDict[id] = bufferItem;
			}
		}

		
		public IBufferItem FindBufferItem(int id)
		{
			IBufferItem bufferItem = null;
			lock (this.BufferItemDict)
			{
				this.BufferItemDict.TryGetValue(id, out bufferItem);
			}
			return bufferItem;
		}

		
		public void RemoveBufferItem(int id)
		{
			lock (this.BufferItemDict)
			{
				this.BufferItemDict.Remove(id);
			}
		}

		
		private Dictionary<int, IBufferItem> BufferItemDict = new Dictionary<int, IBufferItem>();
	}
}
