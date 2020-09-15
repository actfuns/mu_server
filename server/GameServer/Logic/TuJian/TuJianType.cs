using System;
using System.Collections.Generic;

namespace GameServer.Logic.TuJian
{
	// Token: 0x02000495 RID: 1173
	internal class TuJianType
	{
		// Token: 0x04001F15 RID: 7957
		public int TypeID;

		// Token: 0x04001F16 RID: 7958
		public string Name;

		// Token: 0x04001F17 RID: 7959
		public int OpenChangeLife;

		// Token: 0x04001F18 RID: 7960
		public int OpenLevel;

		// Token: 0x04001F19 RID: 7961
		public int ItemCnt;

		// Token: 0x04001F1A RID: 7962
		public _AttrValue AttrValue = null;

		// Token: 0x04001F1B RID: 7963
		public List<int> ItemList = new List<int>();
	}
}
