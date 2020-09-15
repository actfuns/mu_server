using System;
using System.Collections.Generic;

namespace GameServer.Logic.FluorescentGem
{
	// Token: 0x020002CF RID: 719
	internal class SoulStoneGroupConfig
	{
		// Token: 0x0400128D RID: 4749
		public int Group;

		// Token: 0x0400128E RID: 4750
		public List<int> NeedTypeList = new List<int>();

		// Token: 0x0400128F RID: 4751
		public Dictionary<int, double> AttrValue = new Dictionary<int, double>();
	}
}
