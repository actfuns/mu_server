using System;
using System.Collections.Generic;
using Server.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x020003C7 RID: 967
	public class LangHunLingYuScene
	{
		// Token: 0x060010E8 RID: 4328 RVA: 0x00107691 File Offset: 0x00105891
		public void CleanAllInfo()
		{
			this.m_lPrepareTime = 0L;
			this.m_lBeginTime = 0L;
			this.m_lEndTime = 0L;
			this.m_eStatus = GameSceneStatuses.STATUS_NULL;
		}

		// Token: 0x04001933 RID: 6451
		public long StartTimeTicks = 0L;

		// Token: 0x04001934 RID: 6452
		public long m_lPrepareTime = 0L;

		// Token: 0x04001935 RID: 6453
		public long m_lBeginTime = 0L;

		// Token: 0x04001936 RID: 6454
		public long m_lEndTime = 0L;

		// Token: 0x04001937 RID: 6455
		public long m_lLeaveTime = 0L;

		// Token: 0x04001938 RID: 6456
		public GameSceneStatuses m_eStatus = GameSceneStatuses.STATUS_NULL;

		// Token: 0x04001939 RID: 6457
		public int SuccessSide = 0;

		// Token: 0x0400193A RID: 6458
		public int GameId;

		// Token: 0x0400193B RID: 6459
		public Dictionary<int, CopyMap> CopyMapDict = new Dictionary<int, CopyMap>();

		// Token: 0x0400193C RID: 6460
		public bool HasGuangMu = true;

		// Token: 0x0400193D RID: 6461
		public LangHunLingYuSceneInfo SceneInfo;

		// Token: 0x0400193E RID: 6462
		public CityLevelInfo LevelInfo;

		// Token: 0x0400193F RID: 6463
		public List<BangHuiRoleCountData> LongTaBHRoleCountList = new List<BangHuiRoleCountData>();

		// Token: 0x04001940 RID: 6464
		public LangHunLingYuLongTaOwnerData LongTaOwnerData = new LangHunLingYuLongTaOwnerData();

		// Token: 0x04001941 RID: 6465
		public List<LangHunLingYuQiZhiBuffOwnerData> QiZhiBuffOwnerDataList = new List<LangHunLingYuQiZhiBuffOwnerData>();

		// Token: 0x04001942 RID: 6466
		public Dictionary<int, QiZhiConfig> NPCID2QiZhiConfigDict = new Dictionary<int, QiZhiConfig>();

		// Token: 0x04001943 RID: 6467
		public int LastTheOnlyOneBangHui = 0;

		// Token: 0x04001944 RID: 6468
		public int SuperQiZhiOwnerBhid;

		// Token: 0x04001945 RID: 6469
		public long BangHuiTakeHuangGongTicks;

		// Token: 0x04001946 RID: 6470
		public long LastAddBangZhanAwardsTicks = 0L;

		// Token: 0x04001947 RID: 6471
		public Dictionary<int, LangHunLingYuClientContextData> ClientContextDataDict = new Dictionary<int, LangHunLingYuClientContextData>();

		// Token: 0x04001948 RID: 6472
		public LangHunLingYuCityData CityData = new LangHunLingYuCityData();

		// Token: 0x04001949 RID: 6473
		public Dictionary<int, BangHuiMiniData> BHID2BangHuiMiniDataDict = new Dictionary<int, BangHuiMiniData>();

		// Token: 0x0400194A RID: 6474
		public int SuccessBangHuiId;

		// Token: 0x0400194B RID: 6475
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		// Token: 0x0400194C RID: 6476
		public SortedList<long, List<object>> CreateMonsterQueue = new SortedList<long, List<object>>();
	}
}
