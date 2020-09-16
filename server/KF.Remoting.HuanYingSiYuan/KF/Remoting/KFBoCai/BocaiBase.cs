using System;
using System.Collections.Generic;
using KF.Contract.Data;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting.KFBoCai
{
	
	public abstract class BocaiBase
	{
		
		public abstract void UpData(bool reload = false);

		
		public abstract void Thread();

		
		protected abstract void Init();

		
		public abstract KFStageData GetKFStageData();

		
		public abstract OpenLottery GetOpenLottery();

		
		public void InitData()
		{
			this.Init();
			this.UpData(false);
		}

		
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

		
		public void KFSendPeriodsData()
		{
			ClientAgentManager.Instance().BroadCastMsg(KFCallMsg.New<OpenLottery>(KuaFuEventTypes.BoCaiPeriodsData, this.GetOpenLottery()), 0);
		}

		
		public void KFSendStageData()
		{
			ClientAgentManager.Instance().BroadCastMsg(KFCallMsg.New<KFStageData>(KuaFuEventTypes.BoCaiStageChange, this.GetKFStageData()), 0);
		}

		
		public BoCaiTypeEnum BoCaiType;

		
		public BoCaiStageEnum Stage = BoCaiStageEnum.Stage_Init;

		
		protected int StopBuyTime;

		
		protected long MaxPeriods = 0L;

		
		protected DateTime PeriodsStartTime;

		
		protected OpenLottery OpenData = new OpenLottery();

		
		protected OpenLottery UpToDBOpenData = new OpenLottery();

		
		public object mutex = new object();

		
		protected List<OpenLottery> OpenHistory = new List<OpenLottery>();

		
		protected List<KFBoCaoHistoryData> BoCaiWinHistoryList = new List<KFBoCaoHistoryData>();

		
		protected Dictionary<string, List<KFBuyBocaiData>> RoleBuyDict = new Dictionary<string, List<KFBuyBocaiData>>();

		
		public string SelectOpenHisttory10 = string.Format(" AND `strWinNum`!='{0}' ORDER BY `DataPeriods` DESC LIMIT 10", "");
	}
}
