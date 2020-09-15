using System;
using System.Collections.Generic;
using GameServer.Interface;

namespace GameServer.Logic.NewBufferExt
{
	// Token: 0x02000536 RID: 1334
	public class BufferExtManager
	{
		// Token: 0x0600196B RID: 6507 RVA: 0x0018D2A8 File Offset: 0x0018B4A8
		public void AddBufferItem(int id, IBufferItem bufferItem)
		{
			lock (this.BufferItemDict)
			{
				this.BufferItemDict[id] = bufferItem;
			}
		}

		// Token: 0x0600196C RID: 6508 RVA: 0x0018D2FC File Offset: 0x0018B4FC
		public IBufferItem FindBufferItem(int id)
		{
			IBufferItem bufferItem = null;
			lock (this.BufferItemDict)
			{
				this.BufferItemDict.TryGetValue(id, out bufferItem);
			}
			return bufferItem;
		}

		// Token: 0x0600196D RID: 6509 RVA: 0x0018D35C File Offset: 0x0018B55C
		public void RemoveBufferItem(int id)
		{
			lock (this.BufferItemDict)
			{
				this.BufferItemDict.Remove(id);
			}
		}

		// Token: 0x04002390 RID: 9104
		private Dictionary<int, IBufferItem> BufferItemDict = new Dictionary<int, IBufferItem>();
	}
}
