using System;
using System.Collections.Generic;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	// Token: 0x02000315 RID: 789
	[TemplateMappingOptions(null, "LegionsManager", "ID")]
	public class JunTuanRolePermissionInfo
	{
		// Token: 0x0400144B RID: 5195
		public int ID;

		// Token: 0x0400144C RID: 5196
		public string Name;

		// Token: 0x0400144D RID: 5197
		public int Manager;

		// Token: 0x0400144E RID: 5198
		public int AppointLeader;

		// Token: 0x0400144F RID: 5199
		public int AppointElite;

		// Token: 0x04001450 RID: 5200
		public int Quit;

		// Token: 0x04001451 RID: 5201
		public int Dissolution;

		// Token: 0x04001452 RID: 5202
		public int BulletinCD;

		// Token: 0x04001453 RID: 5203
		[TemplateMappingField(SpliteChars = ",")]
		public List<int> TalkLevel;

		// Token: 0x04001454 RID: 5204
		public int TalkCD;
	}
}
