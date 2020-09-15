using System;
using System.Collections.Generic;
using KF.Contract.Data;
using Server.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x020002F3 RID: 755
	public class HongBaoRuntimeData
	{
		// Token: 0x04001358 RID: 4952
		public object Mutex = new object();

		// Token: 0x04001359 RID: 4953
		public int RedPacketsTime;

		// Token: 0x0400135A RID: 4954
		public int RedPacketsRight;

		// Token: 0x0400135B RID: 4955
		public int RedPacketsNumMax;

		// Token: 0x0400135C RID: 4956
		public int RedPacketsMessage;

		// Token: 0x0400135D RID: 4957
		public int[] RedPacketsLimit;

		// Token: 0x0400135E RID: 4958
		public int[] RedPacketsInfomationLimit;

		// Token: 0x0400135F RID: 4959
		public int RedPacketsRequest;

		// Token: 0x04001360 RID: 4960
		public int LoadFromDBInterval1 = 60000;

		// Token: 0x04001361 RID: 4961
		public int LoadFromDBInterval2 = 60000;

		// Token: 0x04001362 RID: 4962
		public string RedPacketsQuanMinMessage;

		// Token: 0x04001363 RID: 4963
		public string RedPacketsChongZhiMessage;

		// Token: 0x04001364 RID: 4964
		public string RedPacketsTeQuanMessage;

		// Token: 0x04001365 RID: 4965
		public int RedPacketsAutomaticRecordMax;

		// Token: 0x04001366 RID: 4966
		public int RedPacketsRankLimit = 10;

		// Token: 0x04001367 RID: 4967
		public long AddChargeValue = 0L;

		// Token: 0x04001368 RID: 4968
		public Dictionary<int, int> BangHuiHongBaoIdDict = new Dictionary<int, int>();

		// Token: 0x04001369 RID: 4969
		public Dictionary<int, HongBaoSendData> OldHongBaoDict = new Dictionary<int, HongBaoSendData>();

		// Token: 0x0400136A RID: 4970
		public Dictionary<int, HongBaoSendData> HongBaoDict = new Dictionary<int, HongBaoSendData>();

		// Token: 0x0400136B RID: 4971
		public Dictionary<int, ZhanMengHongBaoData> ZhanMengHongBaoDict = new Dictionary<int, ZhanMengHongBaoData>();

		// Token: 0x0400136C RID: 4972
		public Dictionary<int, SystemHongBaoData> ChongZhiHongBaoDict = new Dictionary<int, SystemHongBaoData>();

		// Token: 0x0400136D RID: 4973
		public Dictionary<int, List<HongBaoRecvData>> ChongZhiHongBaoRecvDict = new Dictionary<int, List<HongBaoRecvData>>();

		// Token: 0x0400136E RID: 4974
		public Dictionary<int, HongBaoSendData> JieRiHongBaoDict = new Dictionary<int, HongBaoSendData>();

		// Token: 0x0400136F RID: 4975
		public Dictionary<int, HongBaoSendData> SpecPHongBaoDict = new Dictionary<int, HongBaoSendData>();

		// Token: 0x04001370 RID: 4976
		public KuaFuData<List<HongBaoRankItemData>> RecvRankList = new KuaFuData<List<HongBaoRankItemData>>();

		// Token: 0x04001371 RID: 4977
		public Dictionary<int, HongBaoSendData> DelayUpdateDict = new Dictionary<int, HongBaoSendData>();

		// Token: 0x04001372 RID: 4978
		public HashSet<int> DestoryBhIds = new HashSet<int>();

		// Token: 0x04001373 RID: 4979
		public bool Initialized = false;

		// Token: 0x04001374 RID: 4980
		public bool ZhanMengHongBaoInitialized = false;

		// Token: 0x04001375 RID: 4981
		public bool JieRiHongBaoInitialized = false;

		// Token: 0x04001376 RID: 4982
		public bool JieRiHongBaoBangInitialized = false;

		// Token: 0x04001377 RID: 4983
		public bool SpecPHongBaoInitialized = false;

		// Token: 0x04001378 RID: 4984
		public long NextCheckExpireTicks;

		// Token: 0x04001379 RID: 4985
		public long NextTicks1;

		// Token: 0x0400137A RID: 4986
		public long NextTicks3;
	}
}
