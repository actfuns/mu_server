using System;
using System.Collections.Generic;
using KF.Contract.Data;

namespace GameServer.Logic
{
	// Token: 0x0200031F RID: 799
	public class KarenBattleDataBase
	{
		// Token: 0x040014B2 RID: 5298
		public object Mutex = new object();

		// Token: 0x040014B3 RID: 5299
		public Dictionary<int, KarenBattleBirthPoint> MapBirthPointDict = new Dictionary<int, KarenBattleBirthPoint>();

		// Token: 0x040014B4 RID: 5300
		public string RoleParamsAwardsDefaultString = "";

		// Token: 0x040014B5 RID: 5301
		public Dictionary<int, KarenFuBenData> FuBenItemData = new Dictionary<int, KarenFuBenData>();
	}
}
