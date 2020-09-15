using System;
using System.Collections.Generic;
using KF.Contract.Data;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x02000221 RID: 545
	public class KuaFuLueDuoData
	{
		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600073A RID: 1850 RVA: 0x0006AF48 File Offset: 0x00069148
		public object Mutex
		{
			get
			{
				return this.CommonConfigData.MutexConfig;
			}
		}

		// Token: 0x04000C8D RID: 3213
		public KuaFuLueDuoCommonData CommonConfigData = new KuaFuLueDuoCommonData();

		// Token: 0x04000C8E RID: 3214
		public Dictionary<int, KuaFuLueDuoMonsterItem> CollectMonsterDict = new Dictionary<int, KuaFuLueDuoMonsterItem>();

		// Token: 0x04000C8F RID: 3215
		public Dictionary<int, QiZhiConfig> NPCID2QiZhiConfigDict = new Dictionary<int, QiZhiConfig>();

		// Token: 0x04000C90 RID: 3216
		public Dictionary<int, MapBirthPoint> MapBirthPointDict = new Dictionary<int, MapBirthPoint>();

		// Token: 0x04000C91 RID: 3217
		public HashSet<int> HideRankList = new HashSet<int>();

		// Token: 0x04000C92 RID: 3218
		public Dictionary<int, KuaFuLueDuoStoreConfig> KingOfBattleStoreDict = new Dictionary<int, KuaFuLueDuoStoreConfig>();

		// Token: 0x04000C93 RID: 3219
		public List<KuaFuLueDuoStoreConfig> KingOfBattleStoreList = new List<KuaFuLueDuoStoreConfig>();

		// Token: 0x04000C94 RID: 3220
		public int BeginNum;

		// Token: 0x04000C95 RID: 3221
		public int EndNum;

		// Token: 0x04000C96 RID: 3222
		public double[] CrusadeOrePercent;

		// Token: 0x04000C97 RID: 3223
		public int[] CrusadeUltraKill;

		// Token: 0x04000C98 RID: 3224
		public int[] CrusadeShutDown;

		// Token: 0x04000C99 RID: 3225
		public double[] CrusadeAwardAttacker;

		// Token: 0x04000C9A RID: 3226
		public double[] CrusadeAwardDefender;

		// Token: 0x04000C9B RID: 3227
		public int CrusadeSeason;

		// Token: 0x04000C9C RID: 3228
		public int[] CrusadeOre;

		// Token: 0x04000C9D RID: 3229
		public int CrusadeMinApply;

		// Token: 0x04000C9E RID: 3230
		public int CrusadeApplyCD;

		// Token: 0x04000C9F RID: 3231
		public int[] CrusadeEnterTime;

		// Token: 0x04000CA0 RID: 3232
		public int[] CrusadeEnterPrice;

		// Token: 0x04000CA1 RID: 3233
		public double CrusadePerfect;

		// Token: 0x04000CA2 RID: 3234
		public int CrusadeStoreCD;

		// Token: 0x04000CA3 RID: 3235
		public int CrusadeStorePrice;

		// Token: 0x04000CA4 RID: 3236
		public int CrusadeStoreRandomNum;

		// Token: 0x04000CA5 RID: 3237
		public int[] ZhanMengZiJin;

		// Token: 0x04000CA6 RID: 3238
		public int GoldWingGoodsID = 0;

		// Token: 0x04000CA7 RID: 3239
		public bool PrepareGame;

		// Token: 0x04000CA8 RID: 3240
		public Dictionary<long, KuaFuLueDuoFuBenData> FuBenItemData = new Dictionary<long, KuaFuLueDuoFuBenData>();

		// Token: 0x04000CA9 RID: 3241
		public Dictionary<int, KuaFuLueDuoBangHuiJingJiaData> JingJiaDict = new Dictionary<int, KuaFuLueDuoBangHuiJingJiaData>();

		// Token: 0x04000CAA RID: 3242
		public bool UpdateZiYuanData;

		// Token: 0x04000CAB RID: 3243
		public Dictionary<int, FightInfo> ServerZiYuanDict = new Dictionary<int, FightInfo>();

		// Token: 0x04000CAC RID: 3244
		public Dictionary<int, FightInfo> BhZiYuanDict = new Dictionary<int, FightInfo>();

		// Token: 0x04000CAD RID: 3245
		public Dictionary<int, int> CacheRole2KillDict = new Dictionary<int, int>();

		// Token: 0x04000CAE RID: 3246
		public Dictionary<int, int> CacheBh2LueDuoDict = new Dictionary<int, int>();

		// Token: 0x04000CAF RID: 3247
		public long ChengHaoBHid = 0L;

		// Token: 0x04000CB0 RID: 3248
		public long ChengHaoBHid_Week = 0L;
	}
}
