using System;
using System.Collections.Generic;

namespace GameServer.Logic.MoRi
{
	// Token: 0x0200037A RID: 890
	public class MoRiMonster
	{
		// Token: 0x0400177F RID: 6015
		public int Id;

		// Token: 0x04001780 RID: 6016
		public string Name;

		// Token: 0x04001781 RID: 6017
		public int MonsterId;

		// Token: 0x04001782 RID: 6018
		public int BirthX;

		// Token: 0x04001783 RID: 6019
		public int BirthY;

		// Token: 0x04001784 RID: 6020
		public int KillLimitSecond;

		// Token: 0x04001785 RID: 6021
		public Dictionary<int, float> ExtPropDict = new Dictionary<int, float>();
	}
}
