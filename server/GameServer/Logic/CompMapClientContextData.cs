using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x0200026B RID: 619
	public class CompMapClientContextData
	{
		// Token: 0x04000F60 RID: 3936
		public int RoleId;

		// Token: 0x04000F61 RID: 3937
		public int ServerId;

		// Token: 0x04000F62 RID: 3938
		public int BattleWhichSide;

		// Token: 0x04000F63 RID: 3939
		public string RoleName;

		// Token: 0x04000F64 RID: 3940
		public int Occupation;

		// Token: 0x04000F65 RID: 3941
		public int RoleSex;

		// Token: 0x04000F66 RID: 3942
		public int ZoneID;

		// Token: 0x04000F67 RID: 3943
		public long TotalScore;

		// Token: 0x04000F68 RID: 3944
		public Dictionary<int, long> InjureBossDeltaDict = new Dictionary<int, long>();
	}
}
