using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x0200070F RID: 1807
	public class HeFuActivityConfig
	{
		// Token: 0x06002B54 RID: 11092 RVA: 0x00267E48 File Offset: 0x00266048
		public bool InList(int type)
		{
			foreach (int item in this.openList)
			{
				if (item == type)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04003A45 RID: 14917
		public List<int> openList = new List<int>();
	}
}
