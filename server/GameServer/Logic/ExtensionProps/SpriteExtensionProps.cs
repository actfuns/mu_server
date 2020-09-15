using System;
using System.Collections.Generic;

namespace GameServer.Logic.ExtensionProps
{
	// Token: 0x020006BE RID: 1726
	public class SpriteExtensionProps
	{
		// Token: 0x06002076 RID: 8310 RVA: 0x001BF428 File Offset: 0x001BD628
		public void AddID(int id)
		{
			lock (this.Mutex)
			{
				this.ExtensionPropIDsList.Add(id);
			}
		}

		// Token: 0x06002077 RID: 8311 RVA: 0x001BF47C File Offset: 0x001BD67C
		public void RemoveID(int id)
		{
			lock (this.Mutex)
			{
				for (int i = 0; i < this.ExtensionPropIDsList.Count; i++)
				{
					if (this.ExtensionPropIDsList[i] == id)
					{
						this.ExtensionPropIDsList.RemoveAt(i);
						break;
					}
				}
			}
		}

		// Token: 0x06002078 RID: 8312 RVA: 0x001BF504 File Offset: 0x001BD704
		public List<int> GetIDs()
		{
			List<int> list = null;
			lock (this.Mutex)
			{
				list = this.ExtensionPropIDsList.GetRange(0, this.ExtensionPropIDsList.Count);
			}
			return list;
		}

		// Token: 0x04003677 RID: 13943
		private object Mutex = new object();

		// Token: 0x04003678 RID: 13944
		private List<int> ExtensionPropIDsList = new List<int>();
	}
}
