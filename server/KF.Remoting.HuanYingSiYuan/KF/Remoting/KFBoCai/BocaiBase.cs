using System;
using System.Collections.Generic;
using KF.Contract.Data;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting.KFBoCai
{
	// Token: 0x0200002A RID: 42
	public abstract class BocaiBase
	{
		// Token: 0x060001E7 RID: 487
		public abstract void UpData(bool reload = false);

		// Token: 0x060001E8 RID: 488
		public abstract void Thread();

		// Token: 0x060001E9 RID: 489
		protected abstract void Init();

		// Token: 0x060001EA RID: 490
		public abstract KFStageData GetKFStageData();

		// Token: 0x060001EB RID: 491
		public abstract OpenLottery GetOpenLottery();

		// Token: 0x060001EC RID: 492 RVA: 0x0001CB59 File Offset: 0x0001AD59
		public void InitData()
		{
			this.Init();
			this.UpData(false);
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0001CB70 File Offset: 0x0001AD70
		public long GetDiffTime(DateTime d1, DateTime d2, bool isMilliseconds = true)
		{
			double time;
			if (isMilliseconds)
			{
				time = (d1 - d2).TotalMilliseconds;
			}
			else
			{
				time = (d1 - d2).TotalSeconds;
			}
			if (time - (double)((long)time) > 0.2)
			{
				time += 1.0;
			}
			return (long)time;
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0001CC04 File Offset: 0x0001AE04
		public void SetOpenHistory(OpenLottery dOpen)
		{
			if (null == this.OpenHistory)
			{
				KFBoCaiDbManager.SelectOpenLottery((int)this.BoCaiType, this.SelectOpenHisttory10, out this.OpenHistory);
				if (null == this.OpenHistory)
				{
					return;
				}
			}
			if (!string.IsNullOrEmpty(dOpen.strWinNum))
			{
				if (null == this.OpenHistory.Find((OpenLottery x) => x.DataPeriods == dOpen.DataPeriods))
				{
					this.OpenHistory.Insert(0, dOpen);
					while (this.OpenHistory.Count > 10)
					{
						this.OpenHistory.RemoveAt(this.OpenHistory.Count - 1);
					}
				}
			}
		}

		// Token: 0x060001EF RID: 495 RVA: 0x0001CCE0 File Offset: 0x0001AEE0
		public List<OpenLottery> GetOpenHistory()
		{
			if (null == this.OpenHistory)
			{
				KFBoCaiDbManager.SelectOpenLottery((int)this.BoCaiType, this.SelectOpenHisttory10, out this.OpenHistory);
				if (null == this.OpenHistory)
				{
					return null;
				}
			}
			List<OpenLottery> dList = new List<OpenLottery>();
			foreach (OpenLottery item in this.OpenHistory)
			{
				dList.Add(new OpenLottery
				{
					DataPeriods = item.DataPeriods,
					strWinNum = item.strWinNum,
					BocaiType = item.BocaiType,
					SurplusBalance = item.SurplusBalance,
					AllBalance = item.AllBalance,
					XiaoHaoDaiBi = item.XiaoHaoDaiBi,
					WinInfo = item.WinInfo,
					IsAward = false
				});
			}
			return dList;
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0001CDF4 File Offset: 0x0001AFF4
		public void KFSendPeriodsData()
		{
			ClientAgentManager.Instance().BroadCastMsg(KFCallMsg.New<OpenLottery>(KuaFuEventTypes.BoCaiPeriodsData, this.GetOpenLottery()), 0);
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x0001CE15 File Offset: 0x0001B015
		public void KFSendStageData()
		{
			ClientAgentManager.Instance().BroadCastMsg(KFCallMsg.New<KFStageData>(KuaFuEventTypes.BoCaiStageChange, this.GetKFStageData()), 0);
		}

		// Token: 0x0400011D RID: 285
		public BoCaiTypeEnum BoCaiType;

		// Token: 0x0400011E RID: 286
		public BoCaiStageEnum Stage = BoCaiStageEnum.Stage_Init;

		// Token: 0x0400011F RID: 287
		protected int StopBuyTime;

		// Token: 0x04000120 RID: 288
		protected long MaxPeriods = 0L;

		// Token: 0x04000121 RID: 289
		protected DateTime PeriodsStartTime;

		// Token: 0x04000122 RID: 290
		protected OpenLottery OpenData = new OpenLottery();

		// Token: 0x04000123 RID: 291
		protected OpenLottery UpToDBOpenData = new OpenLottery();

		// Token: 0x04000124 RID: 292
		public object mutex = new object();

		// Token: 0x04000125 RID: 293
		protected List<OpenLottery> OpenHistory = new List<OpenLottery>();

		// Token: 0x04000126 RID: 294
		protected List<KFBoCaoHistoryData> BoCaiWinHistoryList = new List<KFBoCaoHistoryData>();

		// Token: 0x04000127 RID: 295
		protected Dictionary<string, List<KFBuyBocaiData>> RoleBuyDict = new Dictionary<string, List<KFBuyBocaiData>>();

		// Token: 0x04000128 RID: 296
		public string SelectOpenHisttory10 = string.Format(" AND `strWinNum`!='{0}' ORDER BY `DataPeriods` DESC LIMIT 10", "");
	}
}
