using System;
using System.Collections.Generic;
using AutoCSer.Net.TcpServer;
using GameServer.Core.Executor;
using KF.TcpCall;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic.BocaiSys
{
	
	public class BoCaiCaiDaXiao
	{
		
		private BoCaiCaiDaXiao()
		{
		}

		
		public static BoCaiCaiDaXiao GetInstance()
		{
			return BoCaiCaiDaXiao.instance;
		}

		
		private void GetRank()
		{
			try
			{
				List<OpenLottery> openHistory = BoCaiManager.getInstance().GetNewOpenLottery10(this.BoCaiType);
				if (null != openHistory)
				{
					ReturnValue<List<KFBoCaoHistoryData>> msgData = TcpCall.KFBoCaiManager.GetWinHistory(this.BoCaiType);
					if (!msgData.IsReturn)
					{
						LogManager.WriteLog(LogTypes.Error, "[ljl_caidaxiao_猜大小]猜大小获取排行 失败", null, true);
					}
					else
					{
						List<KFBoCaoHistoryData> History = msgData.Value;
						lock (this.mutex)
						{
							this.OpenHistory = openHistory;
							this.WinHistory.Clear();
							if (null != History)
							{
								using (List<KFBoCaoHistoryData>.Enumerator enumerator = History.GetEnumerator())
								{
									while (enumerator.MoveNext())
									{
										KFBoCaoHistoryData item = enumerator.Current;
										if (item.RoleID >= 0)
										{
											OpenLottery data = this.OpenHistory.Find((OpenLottery x) => x.DataPeriods == item.DataPeriods);
											if (data != null && !string.IsNullOrEmpty(data.strWinNum))
											{
												item.OpenData = data.strWinNum;
												this.WinHistory.Add(item);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		
		private bool GetStageData()
		{
			try
			{
				ReturnValue<KFStageData> msgData = TcpCall.KFBoCaiManager.GetKFStageData(this.BoCaiType);
				if (!msgData.IsReturn)
				{
					return false;
				}
				KFStageData data = msgData.Value;
				return this.SetStageData(data, false);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		
		private bool GetOpenLotteryData(bool init = false)
		{
			try
			{
				ReturnValue<OpenLottery> msgData = TcpCall.KFBoCaiManager.GetOpenLottery(this.BoCaiType);
				if (!msgData.IsReturn)
				{
					return false;
				}
				OpenLottery OpenData = msgData.Value;
				return this.SetOpenLotteryData(OpenData, init, false);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		
		private void startNewGame(OpenLottery OpenData = null)
		{
			try
			{
				lock (this.mutex)
				{
					this.BoCaiBaseList.Clear();
					this.ServerOpenData.XiaoHaoDaiBi = -1;
					this.ServerData = new CenterServerCaiDaXiao();
					this.GetRank();
					if (null != OpenData)
					{
						this.SetOpenLotteryData(OpenData, true, false);
						this.GetStageData();
					}
					else
					{
						this.GetStageData();
						if (this.StageData.isOpenDay && this.StageData.Stage >= 2)
						{
							if (!this.GetOpenLotteryData(true))
							{
								LogManager.WriteLog(LogTypes.Error, "[ljl_caidaxiao_猜大小]本期开奖数据 失败", null, true);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		
		public void Init()
		{
			try
			{
				lock (this.mutex)
				{
					this.StageData.isOpen = false;
					this.StageData.isOpenDay = false;
					this.startNewGame(null);
					this.ServerData.UpOldOpenTime = TimeUtil.NowDateTime();
					BoCaiManager.getInstance().OldtterySet(this.BoCaiType, this.ServerOpenData.DataPeriods);
					if (!this.StageData.isOpen || !this.StageData.isOpenDay)
					{
						LogManager.WriteLog(LogTypes.Info, string.Format("[ljl_caidaxiao_猜大小] 初始化猜大小 博彩猜大小 暂未开启活动 isOpen={0}, OpenTime={1}", this.StageData.isOpen, this.StageData.OpenTime), null, true);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		
		private void CheckStageData(DateTime _time, bool isReload)
		{
			try
			{
				lock (this.mutex)
				{
					bool isCheck = false;
					double last = (_time - this.ServerData.GetStageDataTime).TotalSeconds;
					if (isReload)
					{
						isCheck = true;
					}
					else if (this.StageData.isOpenDay)
					{
						if (last > 60.0)
						{
							isCheck = true;
						}
						else if (this.StageData.Stage == 1 && last > 5.0)
						{
							isCheck = true;
						}
						else if (this.StageData.Stage == 4 && last > 5.0)
						{
							isCheck = true;
						}
						else if (this.StageData.Stage >= 1 && this.StageData.LastOpenTime >= 0L && (last + 1.0) * 1000.0 > (double)this.StageData.LastOpenTime)
						{
							isCheck = true;
						}
						else if (this.StageData.LastOpenTime < 0L && last > 3.0)
						{
							isCheck = true;
						}
					}
					else if (this.StageData.OpenTime >= 0L && (this.ServerData.GetStageDataTime.AddSeconds((double)this.StageData.OpenTime) - _time).TotalSeconds < 1.0)
					{
						isCheck = true;
					}
					else if (this.StageData.OpenTime < 0L && last > 30.0)
					{
						isCheck = true;
					}
					if (isCheck)
					{
						this.GetStageData();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		
		public void BigTimeUpData(bool reload = false)
		{
			try
			{
				DateTime _time = TimeUtil.NowDateTime();
				this.CheckStageData(_time, reload);
				if (this.StageData.isOpenDay)
				{
					if (this.StageData.Stage < 5 && this.StageData.Stage >= 2 && this.ServerOpenData.DataPeriods < 1L)
					{
						this.GetOpenLotteryData(false);
					}
					else if (this.StageData.Stage >= 2 && this.StageData.Stage < 4 && (_time - this.ServerData.UpRateTime).TotalSeconds > 15.0)
					{
						this.GetOpenLotteryData(false);
					}
					else if (this.StageData.LastOpenTime < 0L && (_time - this.ServerData.UpRateTime).TotalSeconds > 3.0)
					{
						this.GetOpenLotteryData(false);
					}
				}
				else if (this.StageData.OpenTime > 60L && (long)(_time - this.ServerData.UpOldOpenTime).TotalMinutes > 30L)
				{
					this.ServerData.UpOldOpenTime = TimeUtil.NowDateTime();
					if (string.IsNullOrEmpty(this.ServerOpenData.strWinNum))
					{
						BoCaiManager.getInstance().OldtterySet(this.BoCaiType, this.ServerOpenData.DataPeriods);
					}
					else
					{
						BoCaiManager.getInstance().OldtterySet(this.BoCaiType, this.ServerOpenData.DataPeriods + 1L);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		
		public double CompensateRate(string winNum, string info, out string winType)
		{
			winType = "";
			try
			{
				lock (this.mutex)
				{
					int Value = BoCaiHelper.String2Int(winNum);
					if (Value < 0)
					{
						return 0.0;
					}
					DiceValueEnum EnumValue;
					if (Value > 3 && Value < 11)
					{
						EnumValue = DiceValueEnum.DiceMin;
					}
					else if (Value >= 11 && Value < 18)
					{
						EnumValue = DiceValueEnum.DiceMax;
					}
					else
					{
						EnumValue = DiceValueEnum.DiceLeopard;
					}
					int num = (int)EnumValue;
					winType = num.ToString();
					string[] files = info.Split(new char[]
					{
						','
					});
					long buyNum = Convert.ToInt64(files[EnumValue - DiceValueEnum.DiceMin]);
					long AllBalance = Convert.ToInt64(files[0]) + Convert.ToInt64(files[1]) + Convert.ToInt64(files[2]);
					if (AllBalance > 0L && buyNum > 0L)
					{
						return Math.Truncate(100.0 * (double)AllBalance / (double)buyNum) / 100.0;
					}
					return 1.0;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
			return 0.0;
		}

		
		private void OpenLotterySetWin()
		{
			try
			{
				if (this.StageData.Stage == 5 && !this.ServerData.IsAward)
				{
					if (string.IsNullOrEmpty(this.ServerOpenData.strWinNum) || string.IsNullOrEmpty(this.ServerOpenData.WinInfo))
					{
						this.GetOpenLotteryData(false);
					}
					if (this.ServerOpenData.DataPeriods >= 1L && this.ServerOpenData.XiaoHaoDaiBi >= 1 && !string.IsNullOrEmpty(this.ServerOpenData.strWinNum) && !string.IsNullOrEmpty(this.ServerOpenData.WinInfo))
					{
						LogManager.WriteLog(LogTypes.Info, string.Format("[ljl_caidaxiao_猜大小]猜大小 开奖 GetOpenLottery su DataPeriods={0}", this.ServerOpenData.DataPeriods), null, true);
						string winType;
						double Rate = this.CompensateRate(this.ServerOpenData.strWinNum, this.ServerOpenData.WinInfo, out winType);
						if (Rate < 1.0)
						{
							LogManager.WriteLog(LogTypes.Info, "[ljl_caidaxiao_猜大小]猜大小 开奖 赔率 < 1 ", null, true);
						}
						else
						{
							List<BuyBoCai2SDB> BuyList = new List<BuyBoCai2SDB>();
							lock (this.mutex)
							{
								foreach (PlayerBuyBoCaiData PlayerBuyData in this.BoCaiBaseList)
								{
									BuyBoCai2SDB buyItem = new BuyBoCai2SDB
									{
										ZoneID = PlayerBuyData.ZoneID,
										m_RoleID = PlayerBuyData.RoleID,
										ServerId = PlayerBuyData.ServerId,
										strUserID = PlayerBuyData.strUserID,
										m_RoleName = PlayerBuyData.RoleName,
										DataPeriods = this.ServerOpenData.DataPeriods,
										BocaiType = this.BoCaiType,
										IsSend = false,
										IsWin = false
									};
									foreach (BoCaiBuyItem item in PlayerBuyData.BuyItemList)
									{
										BuyBoCai2SDB temp = new BuyBoCai2SDB();
										GlobalNew.Copy<BuyBoCai2SDB>(buyItem, ref temp);
										temp.BuyNum = item.BuyNum;
										temp.strBuyValue = item.strBuyValue;
										BuyList.Add(temp);
									}
								}
							}
							this.ServerOpenData.IsAward = true;
							this.ServerData.IsAward = true;
							foreach (BuyBoCai2SDB buyItem in BuyList)
							{
                                if (!BoCaiManager.getInstance().SendWinItem(this.ServerOpenData, buyItem, Rate, false, winType))
								{
									this.ServerOpenData.IsAward = false;
								}
							}
							if (this.ServerOpenData.IsAward)
							{
								Global.Send2DB<OpenLottery>(2084, this.ServerOpenData, 0);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		
		public bool SetStageData(KFStageData data, bool isKF = true)
		{
			try
			{
				bool isOpen = false;
				bool isActivity = false;
				bool isChangeStage = false;
				lock (this.mutex)
				{
					if (null == data)
					{
						return false;
					}
					isActivity = (this.StageData.isOpen != data.isOpen);
					if (data.Stage != this.StageData.Stage)
					{
						isOpen = (this.StageData.isOpenDay && data.Stage == 5);
						isChangeStage = true;
					}
					this.StageData = data;
					this.ServerData.GetStageDataTime = TimeUtil.NowDateTime();
				}
				if (isOpen)
				{
					this.OpenLotterySetWin();
					this.GetRank();
				}
				if (isChangeStage && (this.StageData.Stage == 5 || this.StageData.Stage == 3 || this.StageData.Stage == 2))
				{
					if (this.ServerOpenData.DataPeriods < 0L)
					{
						this.GetOpenLotteryData(false);
					}
					this.UpdateBoCai();
				}
				if (isActivity)
				{
					this.PriorityActivity(null);
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		
		public bool SetOpenLotteryData(OpenLottery OpenData, bool init = false, bool isKF = false)
		{
			try
			{
				lock (this.mutex)
				{
					if (null != OpenData)
					{
						if (OpenData.DataPeriods < 1L || OpenData.XiaoHaoDaiBi < 1)
						{
							if (this.StageData.Stage > 1)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("[ljl_caidaxiao_猜大小] DataPeriods = {0},XiaoHaoDaiBi={1} ", OpenData.DataPeriods, OpenData.XiaoHaoDaiBi), null, true);
							}
							return false;
						}
						if (this.ServerOpenData.DataPeriods != OpenData.DataPeriods && this.ServerOpenData.DataPeriods > 0L && !init)
						{
							this.startNewGame(OpenData);
							return true;
						}
						if (this.ServerOpenData.DataPeriods != OpenData.DataPeriods)
						{
						}
						this.ServerOpenData = OpenData;
						this.ServerData.UpRateTime = TimeUtil.NowDateTime();
						Global.Send2DB<OpenLottery>(2084, OpenData, 0);
						if (isKF && this.StageData.Stage == 2)
						{
							this.UpdateBoCai();
						}
						return true;
					}
					else
					{
						LogManager.WriteLog(LogTypes.Error, "[ljl_caidaxiao_猜大小] 猜大小 TcpCall.KFBoCaiManager.GetOpenLottery = null", null, true);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		
		public bool IsCanBuy()
		{
			bool result;
			lock (this.mutex)
			{
				result = (this.StageData.isOpenDay && this.StageData.Stage == 2 && this.ServerOpenData.DataPeriods > 1L);
			}
			return result;
		}

		
		public long GetDataPeriods()
		{
			long dataPeriods;
			lock (this.mutex)
			{
				dataPeriods = this.ServerOpenData.DataPeriods;
			}
			return dataPeriods;
		}

		
		public int GetXiaoHaoDaiBi()
		{
			int xiaoHaoDaiBi;
			lock (this.mutex)
			{
				xiaoHaoDaiBi = this.ServerOpenData.XiaoHaoDaiBi;
			}
			return xiaoHaoDaiBi;
		}

		
		public int GetBuyNum(int roleID)
		{
			int result;
			lock (this.mutex)
			{
				int num = 0;
				PlayerBuyBoCaiData playerBoCai = this.BoCaiBaseList.Find((PlayerBuyBoCaiData x) => x.RoleID == roleID);
				if (null != playerBoCai)
				{
					foreach (BoCaiBuyItem item in playerBoCai.BuyItemList)
					{
						num += item.BuyNum;
					}
				}
				result = num;
			}
			return result;
		}

		
		public BuyBoCai2SDB BuyBocai(GameClient client, int buyNum, string BuyVal, ref int allNum)
		{
			BuyBoCai2SDB DbData = null;
			try
			{
				lock (this.mutex)
				{
					PlayerBuyBoCaiData playerBoCai = this.BoCaiBaseList.Find((PlayerBuyBoCaiData x) => x.RoleID == client.ClientData.RoleID);
					if (null == playerBoCai)
					{
						playerBoCai = new PlayerBuyBoCaiData();
						playerBoCai.RoleID = client.ClientData.RoleID;
						playerBoCai.RoleName = client.ClientData.RoleName;
						playerBoCai.ZoneID = client.ClientData.ZoneID;
						playerBoCai.strUserID = client.strUserID;
						playerBoCai.ServerId = client.ServerId;
						playerBoCai.BuyItemList = new List<BoCaiBuyItem>();
						BoCaiBuyItem item = new BoCaiBuyItem
						{
							BuyNum = buyNum,
							strBuyValue = BuyVal
						};
						playerBoCai.BuyItemList.Add(item);
						this.BoCaiBaseList.Add(playerBoCai);
					}
					else
					{
						BoCaiBuyItem item = playerBoCai.BuyItemList.Find((BoCaiBuyItem x) => x.strBuyValue.Equals(BuyVal));
						if (null == item)
						{
							item = new BoCaiBuyItem
							{
								BuyNum = buyNum,
								strBuyValue = BuyVal,
								DataPeriods = this.ServerOpenData.DataPeriods
							};
							playerBoCai.BuyItemList.Add(item);
						}
						else
						{
							item.BuyNum += buyNum;
							allNum = item.BuyNum;
						}
					}
					DbData = new BuyBoCai2SDB();
					DbData.m_RoleID = playerBoCai.RoleID;
					DbData.m_RoleName = playerBoCai.RoleName;
					DbData.ZoneID = playerBoCai.ZoneID;
					DbData.strUserID = playerBoCai.strUserID;
					DbData.ServerId = playerBoCai.ServerId;
					DbData.BuyNum = buyNum;
					DbData.strBuyValue = BuyVal;
					DbData.BocaiType = this.BoCaiType;
					DbData.DataPeriods = this.ServerOpenData.DataPeriods;
					DbData.IsSend = false;
					DbData.IsWin = false;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
			return DbData;
		}

		
		public void CopyBuyList(out List<BoCaiBuyItem> itemList, int roleID)
		{
			itemList = new List<BoCaiBuyItem>();
			try
			{
				lock (this.mutex)
				{
					PlayerBuyBoCaiData playerBoCai = this.BoCaiBaseList.Find((PlayerBuyBoCaiData x) => x.RoleID == roleID);
					if (null != playerBoCai)
					{
						foreach (BoCaiBuyItem item in playerBoCai.BuyItemList)
						{
							BoCaiBuyItem temp = new BoCaiBuyItem
							{
								BuyNum = item.BuyNum,
								strBuyValue = item.strBuyValue,
								DataPeriods = item.DataPeriods
							};
							itemList.Add(temp);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		
		public void OpenGetBoCai(int roleid, ref GetBoCaiResult mgsData)
		{
			try
			{
				lock (this.mutex)
				{
					this.CopyBuyList(out mgsData.ItemList, roleid);
					mgsData.NowPeriods = this.ServerOpenData.DataPeriods;
					mgsData.IsOpen = (this.StageData.Stage > 1);
					mgsData.Value1 = this.ServerOpenData.WinInfo;
					mgsData.Stage = this.StageData.Stage;
					mgsData.OpenHistory = new List<BoCaiOpenHistory>();
					if (this.StageData.isOpenDay)
					{
						mgsData.OpenTime = TimeUtil.GetDiffTimeSeconds(this.ServerData.GetStageDataTime.AddMilliseconds((double)this.StageData.OpenTime), TimeUtil.NowDateTime(), true);
					}
					else
					{
						mgsData.OpenTime = TimeUtil.GetDiffTimeSeconds(this.ServerData.GetStageDataTime.AddSeconds((double)this.StageData.OpenTime), TimeUtil.NowDateTime(), true);
					}
					if (mgsData.Stage == 5)
					{
						mgsData.OpenTime = this.StageData.LastOpenTime;
					}
					BoCaiHelper.CopyHistoryData(this.WinHistory, out mgsData.WinLotteryRoleList);
					if (null != this.OpenHistory)
					{
						foreach (OpenLottery item in this.OpenHistory)
						{
							BoCaiOpenHistory data = new BoCaiOpenHistory();
							data.DataPeriods = item.DataPeriods;
							data.OpenValue = item.strWinNum;
							mgsData.OpenHistory.Add(data);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		
		public void UpdateBoCai()
		{
			try
			{
				lock (this.mutex)
				{
					BoCaiUpdate data = new BoCaiUpdate();
					data.BocaiType = this.BoCaiType;
					data.Value1 = this.ServerOpenData.WinInfo;
					data.DataPeriods = this.ServerOpenData.DataPeriods;
					data.Stage = this.StageData.Stage;
					if (this.StageData.isOpenDay)
					{
						data.OpenTime = TimeUtil.GetDiffTimeSeconds(this.ServerData.GetStageDataTime.AddMilliseconds((double)this.StageData.OpenTime), TimeUtil.NowDateTime(), true);
					}
					else
					{
						data.OpenTime = TimeUtil.GetDiffTimeSeconds(this.ServerData.GetStageDataTime.AddSeconds((double)this.StageData.OpenTime), TimeUtil.NowDateTime(), true);
					}
					if (data.Stage == 5)
					{
						data.OpenTime = this.StageData.LastOpenTime;
					}
					FunctionSendManager.GetInstance().SendMsg<BoCaiUpdate>(FunctionType.CaiDaXiao, 2084, data);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		
		public void PriorityActivity(GameClient client = null)
		{
			try
			{
				int Activity = 0;
				lock (this.mutex)
				{
					Activity = Convert.ToInt32(this.StageData.isOpen);
				}
				if (null == client)
				{
					GameManager.ClientMgr.NotifyAllActivityState(19, Activity, "", "", 0);
				}
				else
				{
					string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						19,
						Activity,
						"",
						0,
						0
					});
					client.sendCmd(770, strcmd, false);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		
		private static BoCaiCaiDaXiao instance = new BoCaiCaiDaXiao();

		
		private object mutex = new object();

		
		private int BoCaiType = 1;

		
		public KFStageData StageData = new KFStageData();

		
		private OpenLottery ServerOpenData = new OpenLottery();

		
		private CenterServerCaiDaXiao ServerData = new CenterServerCaiDaXiao();

		
		private List<OpenLottery> OpenHistory = new List<OpenLottery>();

		
		private List<KFBoCaoHistoryData> WinHistory = new List<KFBoCaoHistoryData>();

		
		private List<PlayerBuyBoCaiData> BoCaiBaseList = new List<PlayerBuyBoCaiData>();
	}
}
