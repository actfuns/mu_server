using System;
using System.Collections.Generic;
using KF.Contract.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x02000223 RID: 547
	public class KuaFuLueDuoScene
	{
		// Token: 0x04000CB9 RID: 3257
		public int FuBenSeqId = 0;

		// Token: 0x04000CBA RID: 3258
		public long StartTimeTicks = 0L;

		// Token: 0x04000CBB RID: 3259
		public long m_lPrepareTime = 0L;

		// Token: 0x04000CBC RID: 3260
		public long m_lBeginTime = 0L;

		// Token: 0x04000CBD RID: 3261
		public long m_lEndTime = 0L;

		// Token: 0x04000CBE RID: 3262
		public long m_lLeaveTime = 0L;

		// Token: 0x04000CBF RID: 3263
		public GameSceneStatuses m_eStatus = GameSceneStatuses.STATUS_NULL;

		// Token: 0x04000CC0 RID: 3264
		public bool m_bEndFlag = false;

		// Token: 0x04000CC1 RID: 3265
		public int GameId;

		// Token: 0x04000CC2 RID: 3266
		public CopyMap CopyMap;

		// Token: 0x04000CC3 RID: 3267
		public KuaFuLueDuoConfig SceneInfo;

		// Token: 0x04000CC4 RID: 3268
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		// Token: 0x04000CC5 RID: 3269
		public Dictionary<int, KuaFuLueDuoClientContextData> ClientContextDataDict = new Dictionary<int, KuaFuLueDuoClientContextData>();

		// Token: 0x04000CC6 RID: 3270
		public Dictionary<int, KuaFuLueDuoBangHuiContextData> BangHuiContextDataDict = new Dictionary<int, KuaFuLueDuoBangHuiContextData>();

		// Token: 0x04000CC7 RID: 3271
		public KuaFuLueDuoClientContextData ClientContextMVP = new KuaFuLueDuoClientContextData();

		// Token: 0x04000CC8 RID: 3272
		public Dictionary<int, KuaFuLueDuoMonsterItem> CollectMonsterXml = new Dictionary<int, KuaFuLueDuoMonsterItem>();

		// Token: 0x04000CC9 RID: 3273
		public KuaFuLueDuoStatisticalData GameStatisticalData = new KuaFuLueDuoStatisticalData();

		// Token: 0x04000CCA RID: 3274
		public KuaFuLueDuoFuBenData ThisFuBenData;

		// Token: 0x04000CCB RID: 3275
		public QiZhiConfig QiZhiItem;

		// Token: 0x04000CCC RID: 3276
		public int LeftZiYuan;

		// Token: 0x04000CCD RID: 3277
		public int TotalZiYuan;

		// Token: 0x04000CCE RID: 3278
		public int SmallZiYuanCount;

		// Token: 0x04000CCF RID: 3279
		public int BigZiYuanCount;

		// Token: 0x04000CD0 RID: 3280
		public int MapGridWidth = 100;

		// Token: 0x04000CD1 RID: 3281
		public int MapGridHeight = 100;
	}
}
