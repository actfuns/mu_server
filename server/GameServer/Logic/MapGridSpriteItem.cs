using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020005F1 RID: 1521
	public struct MapGridSpriteItem
	{
		// Token: 0x04002A9C RID: 10908
		public object GridLock;

		// Token: 0x04002A9D RID: 10909
		public List<object> ObjsList;

		// Token: 0x04002A9E RID: 10910
		public List<object> ObjsListReadOnly;

		// Token: 0x04002A9F RID: 10911
		public short RoleNum;

		// Token: 0x04002AA0 RID: 10912
		public short MonsterNum;

		// Token: 0x04002AA1 RID: 10913
		public short NPCNum;

		// Token: 0x04002AA2 RID: 10914
		public short BiaoCheNum;

		// Token: 0x04002AA3 RID: 10915
		public short JunQiNum;

		// Token: 0x04002AA4 RID: 10916
		public short GoodsPackNum;

		// Token: 0x04002AA5 RID: 10917
		public short DecoNum;
	}
}
