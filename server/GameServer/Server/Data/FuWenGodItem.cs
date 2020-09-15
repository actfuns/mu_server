using System;
using System.Collections.Generic;
using GameServer.Logic;

namespace Server.Data
{
	// Token: 0x02000411 RID: 1041
	public class FuWenGodItem
	{
		// Token: 0x04001BBA RID: 7098
		public int GodId;

		// Token: 0x04001BBB RID: 7099
		public int Type;

		// Token: 0x04001BBC RID: 7100
		public int Level;

		// Token: 0x04001BBD RID: 7101
		public int Num;

		// Token: 0x04001BBE RID: 7102
		public int NeedBlue;

		// Token: 0x04001BBF RID: 7103
		public int NeedRed;

		// Token: 0x04001BC0 RID: 7104
		public int NeedGreen;

		// Token: 0x04001BC1 RID: 7105
		public List<MagicActionItem> MagicItemList;
	}
}
