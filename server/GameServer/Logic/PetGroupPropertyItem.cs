using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000293 RID: 659
	public class PetGroupPropertyItem
	{
		// Token: 0x04001051 RID: 4177
		public int Id;

		// Token: 0x04001052 RID: 4178
		public string Name;

		// Token: 0x04001053 RID: 4179
		public List<List<int>> PetGoodsList = new List<List<int>>();

		// Token: 0x04001054 RID: 4180
		public EquipPropItem PropItem;
	}
}
