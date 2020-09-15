using System;
using System.Collections.Generic;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	// Token: 0x02000316 RID: 790
	[TemplateMappingOptions(null, "LegionTasks", "ID")]
	public class JunTuanTaskInfo
	{
		// Token: 0x04001455 RID: 5205
		public int ID;

		// Token: 0x04001456 RID: 5206
		public int CompleteType;

		// Token: 0x04001457 RID: 5207
		public string Name;

		// Token: 0x04001458 RID: 5208
		[TemplateMappingField(SpliteChars = ",")]
		public List<int> TypeID;

		// Token: 0x04001459 RID: 5209
		public int NumInterval;

		// Token: 0x0400145A RID: 5210
		public int Exp;

		// Token: 0x0400145B RID: 5211
		public int ZhanGong;

		// Token: 0x0400145C RID: 5212
		public int Score;

		// Token: 0x0400145D RID: 5213
		[TemplateMappingField(ParseMethod = "AddNoRepeat")]
		public AwardsItemList Item;
	}
}
