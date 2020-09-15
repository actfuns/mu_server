using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x0200032E RID: 814
	public class KingOfBattleClientContextData
	{
		// Token: 0x04001521 RID: 5409
		public int RoleId;

		// Token: 0x04001522 RID: 5410
		public int ServerId;

		// Token: 0x04001523 RID: 5411
		public int BattleWhichSide;

		// Token: 0x04001524 RID: 5412
		public string RoleName;

		// Token: 0x04001525 RID: 5413
		public int Occupation;

		// Token: 0x04001526 RID: 5414
		public int RoleSex;

		// Token: 0x04001527 RID: 5415
		public int ZoneID;

		// Token: 0x04001528 RID: 5416
		public int TotalScore;

		// Token: 0x04001529 RID: 5417
		public int KillNum;

		// Token: 0x0400152A RID: 5418
		public Dictionary<int, double> InjureBossDeltaDict = new Dictionary<int, double>();
	}
}
