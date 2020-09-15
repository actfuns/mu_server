using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using KF.Remoting.Data;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting.KFBoCai
{
	// Token: 0x02000030 RID: 48
	internal class KFBoCaiCaiDaXiao : BocaiBase
	{
		// Token: 0x06000234 RID: 564 RVA: 0x000208D4 File Offset: 0x0001EAD4
		public static KFBoCaiCaiDaXiao GetInstance()
		{
			return KFBoCaiCaiDaXiao.instance;
		}

		// Token: 0x06000235 RID: 565 RVA: 0x000208EB File Offset: 0x0001EAEB
		private KFBoCaiCaiDaXiao()
		{
			this.StopBuyTime = 240;
			this.BoCaiType = BoCaiTypeEnum.Bocai_Dice;
			this.SelectOpenHisttory10 = " ORDER BY `DataPeriods` DESC LIMIT 10";
		}

		// Token: 0x06000236 RID: 566 RVA: 0x0002091C File Offset: 0x0001EB1C
		private void InitConfig()
		{
			try
			{
				CaiDaXiaoConfig cfg = KFBoCaiConfigManager.GetCaiDaXiaoConfig();
				if (null != cfg)
				{
					this.Config = new CaiDaXiaoConfig();
					this.Config.ID = cfg.ID;
					this.Config.HuoDongKaiQi = cfg.HuoDongKaiQi;
					this.Config.HuoDongJieSu = cfg.HuoDongJieSu;
					this.Config.MeiRiKaiQi = cfg.MeiRiKaiQi;
					this.Config.MeiRiJieSu = cfg.MeiRiJieSu;
					this.Config.ZhuShuShangXian = cfg.ZhuShuShangXian;
					this.OpenData.XiaoHaoDaiBi = cfg.XiaoHaoDaiBi;
				}
				else
				{
					this.Config = null;
					LogManager.WriteLog(LogTypes.Error, "[ljl_caidaxiao_猜大小] KFBoCaiConfigManager.GetCaiShuZiConfig() == null", null, true);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x06000237 RID: 567 RVA: 0x00020A0C File Offset: 0x0001EC0C
		private long GetNowPeriods(DateTime _time)
		{
			return Convert.ToInt64(string.Format("{0}001", TimeUtil.DataTimeToString(_time, "yyMMdd")));
		}

		// Token: 0x06000238 RID: 568 RVA: 0x00020A38 File Offset: 0x0001EC38
		private bool StartBuy(long Periods, DateTime time)
		{
			this.PeriodsStartTime = DateTime.Parse(TimeUtil.DataTimeToString(time, "yyyy-MM-dd HH:mm:ss"));
			this.OpenData.DataPeriods = Periods;
			this.OpenData.strWinNum = "";
			this.OpenData.WinInfo = "0,0,0";
			this.OpenData.BocaiType = (int)this.BoCaiType;
			this.OpenData.SurplusBalance = 0L;
			this.OpenData.AllBalance = 0L;
			this.SetUpToDBOpenData();
			bool result;
			if (!KFBoCaiDbManager.InserOpenLottery(this.OpenData))
			{
				LogManager.WriteLog(LogTypes.Error, "[ljl_caidaxiao_猜大小] 猜大小开始购买 KFBoCaiDbManager.InserOpenLottery(data) false", null, true);
				result = false;
			}
			else
			{
				this.InsertHistoryData();
				LogManager.WriteLog(LogTypes.Info, string.Format("[ljl_caidaxiao_猜大小] 猜大小开启新的一轮 Periods={0}", Periods), null, true);
				result = true;
			}
			return result;
		}

		// Token: 0x06000239 RID: 569 RVA: 0x00020B04 File Offset: 0x0001ED04
		private string SetWinInfo(string value, int buyNum)
		{
			try
			{
				lock (this.mutex)
				{
					int val = Convert.ToInt32(value);
					string[] files = this.OpenData.WinInfo.Split(new char[]
					{
						','
					});
					int value2 = Convert.ToInt32(files[0]);
					int value3 = Convert.ToInt32(files[1]);
					int value4 = Convert.ToInt32(files[2]);
					if (1 == val)
					{
						return string.Format("{0},{1},{2}", value2 + buyNum, value3, value4);
					}
					if (2 == val)
					{
						return string.Format("{0},{1},{2}", value2, value3 + buyNum, value4);
					}
					if (3 == val)
					{
						return string.Format("{0},{1},{2}", value2, value3, value4 + buyNum);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
			return "";
		}

		// Token: 0x0600023A RID: 570 RVA: 0x00020C80 File Offset: 0x0001EE80
		protected override void Init()
		{
			try
			{
				lock (this.mutex)
				{
					long delDataPeriods = this.GetNowPeriods(TimeUtil.NowDateTime().AddMonths(-6));
					KFBoCaiDbManager.DelTableData("t_bocai_open_lottery", string.Format("BocaiType={1} AND DataPeriods < {0}", delDataPeriods, (int)this.BoCaiType));
					KFBoCaiDbManager.DelTableData("t_bocai_buy_history", string.Format("BocaiType={1} AND DataPeriods < {0}", delDataPeriods, (int)this.BoCaiType));
					this.InitConfig();
					KFBoCaiDbManager.SelectOpenLottery((int)this.BoCaiType, this.SelectOpenHisttory10, out this.OpenHistory);
					List<KFBoCaoHistoryData> HistoryList = new List<KFBoCaoHistoryData>();
					KFBoCaiDbManager.LoadLotteryHistory(this.BoCaiType, out HistoryList, "LIMIT 50");
					this.addHistory(HistoryList);
					this.MaxPeriods = KFBoCaiDbManager.GetMaxPeriods((int)this.BoCaiType);
					if (this.MaxPeriods < 0L)
					{
						KFBoCaiDbManager.StopServer("[ljl_caidaxiao_猜大小] 猜大小 maxPeriods == -1");
					}
					else
					{
						if (null == this.Config)
						{
							LogManager.WriteLog(LogTypes.Error, "[ljl_caidaxiao_猜大小]猜大小配置文件错误", null, true);
						}
						this.Stage = BoCaiStageEnum.Stage_Ready;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
				KFBoCaiDbManager.StopServer("初始化 Exception");
			}
		}

		// Token: 0x0600023B RID: 571 RVA: 0x00020E20 File Offset: 0x0001F020
		public override void UpData(bool reload = false)
		{
			try
			{
				lock (this.mutex)
				{
					if (this.Config == null || BoCaiStageEnum.Stage_Init == this.Stage)
					{
						if (reload)
						{
							this.InitConfig();
						}
					}
					else
					{
						DateTime _time = TimeUtil.NowDateTime();
						if (BoCaiStageEnum.Stage_Ready == this.Stage)
						{
							if (DateTime.Parse(this.Config.HuoDongJieSu) <= _time)
							{
								this.InitConfig();
								base.KFSendStageData();
								return;
							}
							if (DateTime.Parse(this.Config.HuoDongKaiQi) > _time)
							{
								if (reload)
								{
									this.InitConfig();
								}
								return;
							}
							if (DateTime.Parse(this.Config.MeiRiKaiQi) > _time)
							{
								if (reload)
								{
									this.InitConfig();
								}
								return;
							}
							if (DateTime.Parse(this.Config.MeiRiJieSu) <= _time.AddSeconds(5.0))
							{
								if (reload)
								{
									this.InitConfig();
								}
								return;
							}
						}
						if (BoCaiStageEnum.Stage_Ready == this.Stage)
						{
							long Periods = this.GetNowPeriods(_time);
							if (this.MaxPeriods >= Periods)
							{
								Periods = this.MaxPeriods + 1L;
							}
							if (this.StartBuy(Periods, _time))
							{
								this.MaxPeriods = Periods;
								this.Stage = BoCaiStageEnum.Stage_Buy;
								this.CompensateRateTime = _time;
								base.KFSendPeriodsData();
								base.KFSendStageData();
							}
						}
						else if (BoCaiStageEnum.Stage_Buy == this.Stage && this.PeriodsStartTime.AddSeconds((double)this.StopBuyTime) <= _time)
						{
							this.Stage = BoCaiStageEnum.Stage_Stop;
							base.KFSendPeriodsData();
							base.KFSendStageData();
						}
						else if (BoCaiStageEnum.Stage_Buy == this.Stage && (_time - this.CompensateRateTime).TotalSeconds > 10.0)
						{
							this.CompensateRateTime = _time;
							base.KFSendPeriodsData();
						}
						else if (BoCaiStageEnum.Stage_Stop == this.Stage && this.PeriodsStartTime.AddSeconds(270.0) <= _time)
						{
							this.Stage = BoCaiStageEnum.Stage_Open;
							base.KFSendStageData();
						}
						else if (BoCaiStageEnum.Stage_End == this.Stage && this.PeriodsStartTime.AddMilliseconds(299960.0) <= _time)
						{
							this.RoleBuyDict.Clear();
							this.InitConfig();
							this.Stage = BoCaiStageEnum.Stage_Ready;
							base.KFSendStageData();
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x0600023C RID: 572 RVA: 0x00021194 File Offset: 0x0001F394
		private static int SortHistory(KFBoCaoHistoryData d1, KFBoCaoHistoryData d2)
		{
			int result;
			if (d1.DataPeriods > d2.DataPeriods)
			{
				result = -1;
			}
			else if (d1.DataPeriods < d2.DataPeriods)
			{
				result = -1;
			}
			else if (d1.WinMoney > d2.WinMoney)
			{
				result = -1;
			}
			else if (d1.WinMoney < d2.WinMoney)
			{
				result = -1;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x0600023D RID: 573 RVA: 0x00021264 File Offset: 0x0001F464
		private void addHistory(List<KFBoCaoHistoryData> History)
		{
			try
			{
				long longData = 0L;
				lock (this.mutex)
				{
					this.BoCaiWinHistoryList.AddRange(History);
					List<long> dataTime = new List<long>();
					using (List<KFBoCaoHistoryData>.Enumerator enumerator = this.BoCaiWinHistoryList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KFBoCaoHistoryData item = enumerator.Current;
							if (dataTime.Find((long x) => x == item.DataPeriods) <= 0L)
							{
								dataTime.Add(item.DataPeriods);
							}
						}
					}
					if (dataTime.Count > 10)
					{
						dataTime.Sort();
						dataTime.Reverse();
						longData = dataTime[9];
						this.BoCaiWinHistoryList = this.BoCaiWinHistoryList.FindAll((KFBoCaoHistoryData x) => x.DataPeriods >= longData);
					}
					this.BoCaiWinHistoryList.Sort(new Comparison<KFBoCaoHistoryData>(KFBoCaiCaiDaXiao.SortHistory));
				}
				if (longData >= 1L)
				{
					if (!KFBoCaiDbManager.DelTableData("t_bocai_lottery_history", string.Format("DataPeriods < {0} AND `BocaiType`={1}", longData, (int)this.BoCaiType)))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("[ljl_caidaxiao_猜大小] DelTableData  t_bocai_lottery_history false DataPeriods {0}", longData), null, true);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x0600023E RID: 574 RVA: 0x0002149C File Offset: 0x0001F69C
		private KFBoCaoHistoryData InsertHistoryData()
		{
			KFBoCaoHistoryData HistoryData = new KFBoCaoHistoryData();
			HistoryData.DataPeriods = this.OpenData.DataPeriods;
			HistoryData.RoleID = -1;
			HistoryData.ZoneID = -1;
			HistoryData.ServerID = -1;
			HistoryData.RoleName = "占位";
			HistoryData.BuyNum = -1;
			HistoryData.WinMoney = -1L;
			KFBoCaiDbManager.InsertLotteryHistory(this.BoCaiType, HistoryData);
			return HistoryData;
		}

		// Token: 0x0600023F RID: 575 RVA: 0x00021504 File Offset: 0x0001F704
		private void SetUpToDBOpenData()
		{
			try
			{
				lock (this.mutex)
				{
					this.UpToDBOpenData.AllBalance = this.OpenData.AllBalance;
					this.UpToDBOpenData.DataPeriods = this.OpenData.DataPeriods;
					this.UpToDBOpenData.strWinNum = this.OpenData.strWinNum;
					this.UpToDBOpenData.BocaiType = this.OpenData.BocaiType;
					this.UpToDBOpenData.SurplusBalance = this.OpenData.SurplusBalance;
					this.UpToDBOpenData.XiaoHaoDaiBi = this.OpenData.XiaoHaoDaiBi;
					this.UpToDBOpenData.WinInfo = this.OpenData.WinInfo;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x06000240 RID: 576 RVA: 0x00021618 File Offset: 0x0001F818
		public override void Thread()
		{
			try
			{
				lock (this.mutex)
				{
					if (this.Stage != BoCaiStageEnum.Stage_Init && !this.UpToDBOpenData.WinInfo.Equals(this.OpenData.WinInfo) && this.OpenData.DataPeriods > 1L)
					{
						this.SetUpToDBOpenData();
						if (!KFBoCaiDbManager.InserOpenLottery(this.OpenData))
						{
							this.UpToDBOpenData.WinInfo = "";
						}
						else
						{
							this.InsertHistoryData();
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
			if (BoCaiStageEnum.Stage_Open == this.Stage)
			{
				try
				{
					List<KFBoCaoHistoryData> History = new List<KFBoCaoHistoryData>();
					lock (this.mutex)
					{
						int Value = 0;
						List<int> openVal = new List<int>();
						for (int i = 0; i < 3; i++)
						{
							int num = Global.GetRandomNumber(1, 7);
							openVal.Add(num);
							Value += num;
						}
						this.OpenData.strWinNum = KFBoCaiDbManager.ListInt2String(openVal);
						LogManager.WriteLog(LogTypes.Info, string.Format("[ljl_caidaxiao_猜大小]猜大小 {0},winNum={1}", this.OpenData.DataPeriods, this.OpenData.strWinNum), null, true);
						if (Value > 3 && Value < 11)
						{
							Value = 1;
						}
						else if (Value >= 11 && Value < 18)
						{
							Value = 3;
						}
						else
						{
							Value = 2;
						}
						if (!KFBoCaiDbManager.InserOpenLottery(this.OpenData))
						{
							LogManager.WriteLog(LogTypes.Error, "[ljl_caidaxiao_猜大小] 猜大小 开始计算中奖了 KFBoCaiDbManager.InserOpenLottery(data) false", null, true);
							return;
						}
						double Rate = this.CompensateRate((DiceValueEnum)Value);
						foreach (List<KFBuyBocaiData> BuyDataList in this.RoleBuyDict.Values)
						{
							foreach (KFBuyBocaiData BuyData in BuyDataList)
							{
								if (Value == Convert.ToInt32(BuyData.BuyValue))
								{
									History.Add(new KFBoCaoHistoryData
									{
										DataPeriods = this.OpenData.DataPeriods,
										RoleID = BuyData.RoleID,
										ZoneID = BuyData.ZoneID,
										ServerID = BuyData.ServerID,
										RoleName = BuyData.RoleName,
										BuyNum = BuyData.BuyNum,
										WinMoney = (long)((int)(Rate * (double)BuyData.BuyNum))
									});
								}
							}
						}
					}
					if (History.Count > 5)
					{
						History.Sort(new Comparison<KFBoCaoHistoryData>(KFBoCaiCaiDaXiao.SortHistory));
					}
					History = History.GetRange(0, Math.Min(5, History.Count));
					foreach (KFBoCaoHistoryData item in History)
					{
						if (!KFBoCaiDbManager.InsertLotteryHistory(this.BoCaiType, item))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("[ljl_caidaxiao_猜大小]猜大小插入中奖历史 false DataPeriods ={0}", item.DataPeriods), null, true);
						}
					}
					if (History.Count < 1)
					{
						History.Add(this.InsertHistoryData());
					}
					this.addHistory(History);
					base.SetOpenHistory(this.GetOpenLottery());
					this.Stage = BoCaiStageEnum.Stage_End;
					base.KFSendPeriodsData();
					base.KFSendStageData();
				}
				catch (Exception ex)
				{
					this.Stage = BoCaiStageEnum.Stage_End;
					LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
				}
			}
		}

		// Token: 0x06000241 RID: 577 RVA: 0x00021B00 File Offset: 0x0001FD00
		public double CompensateRate(DiceValueEnum val)
		{
			try
			{
				lock (this.mutex)
				{
					long buyNun = Convert.ToInt64(this.OpenData.WinInfo.Split(new char[]
					{
						','
					})[val - DiceValueEnum.DiceMin]);
					if (this.OpenData.AllBalance > 0L || buyNun > 0L)
					{
						return Math.Truncate(100.0 * (double)this.OpenData.AllBalance / (double)buyNun) / 100.0;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
			return 1.0;
		}

		// Token: 0x06000242 RID: 578 RVA: 0x00021C00 File Offset: 0x0001FE00
		public override OpenLottery GetOpenLottery()
		{
			OpenLottery result;
			lock (this.mutex)
			{
				result = new OpenLottery
				{
					DataPeriods = this.OpenData.DataPeriods,
					strWinNum = this.OpenData.strWinNum,
					BocaiType = this.OpenData.BocaiType,
					SurplusBalance = this.OpenData.SurplusBalance,
					AllBalance = this.OpenData.AllBalance,
					XiaoHaoDaiBi = this.OpenData.XiaoHaoDaiBi,
					WinInfo = this.OpenData.WinInfo,
					IsAward = false
				};
			}
			return result;
		}

		// Token: 0x06000243 RID: 579 RVA: 0x00021CD0 File Offset: 0x0001FED0
		public override KFStageData GetKFStageData()
		{
			KFStageData result;
			lock (this.mutex)
			{
				KFStageData data = new KFStageData();
				data.Stage = (int)this.Stage;
				data.isOpen = false;
				data.OpenTime = -1L;
				data.isOpenDay = false;
				data.LastOpenTime = -1L;
				data.BoCaiType = this.BoCaiType;
				if (this.Config == null)
				{
					result = data;
				}
				else
				{
					if (data.Stage > 1)
					{
						data.isOpen = true;
						data.isOpenDay = true;
					}
					else
					{
						data.isOpen = (DateTime.Parse(this.Config.HuoDongKaiQi) <= TimeUtil.NowDateTime() && DateTime.Parse(this.Config.HuoDongJieSu) >= TimeUtil.NowDateTime());
						data.isOpenDay = (DateTime.Parse(this.Config.MeiRiKaiQi) <= TimeUtil.NowDateTime() && DateTime.Parse(this.Config.MeiRiJieSu) >= TimeUtil.NowDateTime() && data.isOpen);
					}
					if (!data.isOpen)
					{
						data.OpenTime = base.GetDiffTime(DateTime.Parse(this.Config.HuoDongKaiQi), TimeUtil.NowDateTime(), false);
					}
					else if (data.isOpenDay)
					{
						data.OpenTime = base.GetDiffTime(this.PeriodsStartTime.AddSeconds(270.0), TimeUtil.NowDateTime(), true);
						data.LastOpenTime = this.GetLastTime((BoCaiStageEnum)data.Stage);
					}
					else if (DateTime.Parse(this.Config.MeiRiKaiQi) >= TimeUtil.NowDateTime())
					{
						data.OpenTime = base.GetDiffTime(DateTime.Parse(this.Config.MeiRiKaiQi), TimeUtil.NowDateTime(), false);
					}
					else if (DateTime.Parse(this.Config.MeiRiJieSu) < TimeUtil.NowDateTime())
					{
						data.OpenTime = base.GetDiffTime(DateTime.Parse(this.Config.MeiRiKaiQi).AddDays(1.0), TimeUtil.NowDateTime(), false);
					}
					result = data;
				}
			}
			return result;
		}

		// Token: 0x06000244 RID: 580 RVA: 0x00021F54 File Offset: 0x00020154
		private long GetLastTime(BoCaiStageEnum stage)
		{
			try
			{
				if (null == this.Config)
				{
					return -1L;
				}
				DateTime _time = TimeUtil.NowDateTime();
				if (BoCaiStageEnum.Stage_Buy == stage)
				{
					return base.GetDiffTime(this.PeriodsStartTime.AddSeconds((double)this.StopBuyTime), _time, true);
				}
				if (BoCaiStageEnum.Stage_Stop == stage)
				{
					return base.GetDiffTime(this.PeriodsStartTime.AddSeconds(270.0), _time, true);
				}
				if (stage >= BoCaiStageEnum.Stage_Open)
				{
					return base.GetDiffTime(this.PeriodsStartTime.AddSeconds(300.0), _time, true);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_CaiShuZi_猜数字]{0}", ex.ToString()), null, true);
			}
			return 0L;
		}

		// Token: 0x06000245 RID: 581 RVA: 0x00022038 File Offset: 0x00020238
		public List<KFBoCaoHistoryData> GetWinHistory()
		{
			List<KFBoCaoHistoryData> result;
			lock (this.mutex)
			{
				List<KFBoCaoHistoryData> dList = new List<KFBoCaoHistoryData>();
				foreach (KFBoCaoHistoryData item in this.BoCaiWinHistoryList)
				{
					KFBoCaoHistoryData kfboCaoHistoryData = new KFBoCaoHistoryData();
					kfboCaoHistoryData.RoleID = item.RoleID;
					kfboCaoHistoryData.ZoneID = item.ZoneID;
					kfboCaoHistoryData.ServerID = item.ServerID;
					kfboCaoHistoryData.RoleName = item.RoleName;
					kfboCaoHistoryData.BuyNum = item.BuyNum;
					kfboCaoHistoryData.WinNo = item.WinNo;
					kfboCaoHistoryData.WinMoney = item.WinMoney;
					kfboCaoHistoryData.DataPeriods = item.DataPeriods;
					dList.Add(item);
				}
				result = dList;
			}
			return result;
		}

		// Token: 0x06000246 RID: 582 RVA: 0x00022148 File Offset: 0x00020348
		public bool IsCanBuy(string buyValue, int buyNum, long DataPeriods)
		{
			bool result;
			if (this.Stage != BoCaiStageEnum.Stage_Buy || DataPeriods != this.OpenData.DataPeriods)
			{
				result = false;
			}
			else if (buyNum > this.Config.ZhuShuShangXian)
			{
				result = false;
			}
			else
			{
				int value = Convert.ToInt32(buyValue);
				result = (1 <= value && value <= 3);
			}
			return result;
		}

		// Token: 0x06000247 RID: 583 RVA: 0x000221E8 File Offset: 0x000203E8
		public bool BuyBoCai(KFBuyBocaiData data)
		{
			bool result;
			lock (this.mutex)
			{
				string str = this.SetWinInfo(data.BuyValue, data.BuyNum);
				if (string.IsNullOrEmpty(str))
				{
					result = false;
				}
				else
				{
					bool flag = true;
					List<KFBuyBocaiData> itemList;
					if (this.RoleBuyDict.TryGetValue(data.GetKey(), out itemList))
					{
						KFBuyBocaiData temp = itemList.Find((KFBuyBocaiData x) => x.BuyValue.Equals(data.BuyValue));
						if (temp == null)
						{
							if (KFBoCaiDbManager.InserBuyBocai(this.OpenData.DataPeriods, data))
							{
								this.OpenData.WinInfo = str;
								itemList.Add(data);
							}
							else
							{
								flag = false;
							}
						}
						else
						{
							data.BuyNum += temp.BuyNum;
							if (KFBoCaiDbManager.InserBuyBocai(this.OpenData.DataPeriods, data))
							{
								this.OpenData.WinInfo = str;
								temp.BuyNum = data.BuyNum;
							}
							else
							{
								flag = false;
							}
						}
					}
					else if (KFBoCaiDbManager.InserBuyBocai(this.OpenData.DataPeriods, data))
					{
						itemList = new List<KFBuyBocaiData>();
						itemList.Add(data);
						this.OpenData.WinInfo = str;
						this.RoleBuyDict.Add(data.GetKey(), itemList);
					}
					else
					{
						flag = false;
					}
					result = flag;
				}
			}
			return result;
		}

		// Token: 0x04000137 RID: 311
		private const int RankNum = 5;

		// Token: 0x04000138 RID: 312
		private const int LastTime = 300;

		// Token: 0x04000139 RID: 313
		private const int OpenTime = 270;

		// Token: 0x0400013A RID: 314
		private const int UpCompensateRateTime = 10;

		// Token: 0x0400013B RID: 315
		private DateTime CompensateRateTime;

		// Token: 0x0400013C RID: 316
		private CaiDaXiaoConfig Config = null;

		// Token: 0x0400013D RID: 317
		private static KFBoCaiCaiDaXiao instance = new KFBoCaiCaiDaXiao();
	}
}
