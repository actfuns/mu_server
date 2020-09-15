using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using KF.Contract.Data;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	// Token: 0x0200042B RID: 1067
	public class ZhengDuoRuntimeData
	{
		// Token: 0x04001CB5 RID: 7349
		public object Mutex = new object();

		// Token: 0x04001CB6 RID: 7350
		public Dictionary<int, ZhengDuoSignUpData> FightDataDict = new Dictionary<int, ZhengDuoSignUpData>();

		// Token: 0x04001CB7 RID: 7351
		public ZhengDuoRankData[] ZhengDuoRankDatas = new ZhengDuoRankData[16];

		// Token: 0x04001CB8 RID: 7352
		public Dictionary<int, ZhengDuoRankData> bhid2ZhengDuoRankDataDict = new Dictionary<int, ZhengDuoRankData>();

		// Token: 0x04001CB9 RID: 7353
		public int Rank1Bhid;

		// Token: 0x04001CBA RID: 7354
		public long Age;

		// Token: 0x04001CBB RID: 7355
		public int ZhengDuoStep = 0;

		// Token: 0x04001CBC RID: 7356
		public int State = 0;

		// Token: 0x04001CBD RID: 7357
		public int WeekDay = 0;

		// Token: 0x04001CBE RID: 7358
		public ConcurrentDictionary<int, ZhengDuoScene> SceneDict = new ConcurrentDictionary<int, ZhengDuoScene>();

		// Token: 0x04001CBF RID: 7359
		public Dictionary<long, ZhengDuoFuBenData> FuBenItemData = new Dictionary<long, ZhengDuoFuBenData>();

		// Token: 0x04001CC0 RID: 7360
		public Dictionary<int, ZhengDuoFuBenData> FuBenItemDataByBhid = new Dictionary<int, ZhengDuoFuBenData>();

		// Token: 0x04001CC1 RID: 7361
		public long NextHeartBeatTicks = 0L;
	}
}
