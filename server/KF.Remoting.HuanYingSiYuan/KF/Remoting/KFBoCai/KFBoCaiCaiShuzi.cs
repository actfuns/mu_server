using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;
using KF.Remoting.Data;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting.KFBoCai
{
	
	public class KFBoCaiCaiShuzi : BocaiBase
	{
		
		public static KFBoCaiCaiShuzi GetInstance()
		{
			return KFBoCaiCaiShuzi.instance;
		}

		
		private KFBoCaiCaiShuzi()
		{
			this.StopBuyTime = 1800;
			this.BoCaiType = BoCaiTypeEnum.Bocai_CaiShuzi;
		}

		
		private void InitConfig()
		{
			try
			{
				CaiShuZiConfig cfg = KFBoCaiConfigManager.GetCaiShuZiConfig();
				if (null != cfg)
				{
					this.Config = new CaiShuZiConfig();
					this.Config.ID = cfg.ID;
					this.Config.XiTongChouCheng = cfg.XiTongChouCheng;
					this.Config.BuChongTiaoJian = cfg.BuChongTiaoJian;
					this.Config.KaiQiShiJian = cfg.KaiQiShiJian;
					this.Config.JieShuShiJian = cfg.JieShuShiJian;
					this.Config.XiaoHaoDaiBi = cfg.XiaoHaoDaiBi;
					this.Config.KaiJiangShiJian = cfg.KaiJiangShiJian;
					this.Config.ChuFaBiZhong = cfg.ChuFaBiZhong;
					this.Config.AnNiuList = new List<CaiShuZiAnNiu>();
					foreach (CaiShuZiAnNiu item in cfg.AnNiuList)
					{
						CaiShuZiAnNiu temp = new CaiShuZiAnNiu();
						temp.NO = item.NO;
						temp.Percent = item.Percent;
						this.Config.AnNiuList.Add(temp);
					}
					this.OpenData.AllBalance = Math.Max(this.OpenData.AllBalance, (long)this.Config.BuChongTiaoJian);
					this.OpenData.XiaoHaoDaiBi = this.Config.XiaoHaoDaiBi;
				}
				else
				{
					this.Config = null;
					LogManager.WriteLog(LogTypes.Error, "[ljl_CaiShuZi_猜数字] KFBoCaiConfigManager.GetCaiShuZiConfig() == null", null, true);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_CaiShuZi_猜数字]{0}", ex.ToString()), null, true);
			}
		}

		
		private void StartServerSamePeriods(DateTime time)
		{
			try
			{
				OpenLottery data;
				KFBoCaiDbManager.SelectOpenLottery(this.MaxPeriods, (int)this.BoCaiType, out data);
				List<KFBuyBocaiData> HistoryList;
				if (null == data)
				{
					KFBoCaiDbManager.StopServer(string.Format("[ljl_CaiShuZi_猜数字] 开奖记录读取失败 BoCaiType={1},DataPeriods={0}", this.MaxPeriods, this.BoCaiType));
				}
				else if (!string.IsNullOrEmpty(data.strWinNum))
				{
					if (data.XiaoHaoDaiBi < 1)
					{
						data.XiaoHaoDaiBi = this.OpenData.XiaoHaoDaiBi;
					}
					this.OpenData = data;
					this.SetUpToDBOpenData();
					this.PeriodsStartTime = DateTime.Parse(TimeUtil.DataTimeToString(time, "yyyy-MM-dd HH:mm:ss"));
					this.Stage = BoCaiStageEnum.Stage_End;
					base.KFSendStageData();
					base.KFSendPeriodsData();
					LogManager.WriteLog(LogTypes.Info, string.Format("[ljl_CaiShuZi_猜数字] 和上期是一期 并且已经开奖 BoCaiType={1},DataPeriods={0}", this.MaxPeriods, this.BoCaiType), null, true);
				}
				else if (!KFBoCaiDbManager.LoadBuyHistory((int)this.BoCaiType, this.MaxPeriods, out HistoryList))
				{
					KFBoCaiDbManager.StopServer(string.Format("[ljl_CaiShuZi_猜数字]读取购买记录失败 BoCaiType={1},DataPeriods={0}", this.MaxPeriods, this.BoCaiType));
				}
				else
				{
					this.RoleBuyDict = new Dictionary<string, List<KFBuyBocaiData>>();
					using (List<KFBuyBocaiData>.Enumerator enumerator = HistoryList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KFBuyBocaiData item = enumerator.Current;
							List<KFBuyBocaiData> itemList;
							if (this.RoleBuyDict.TryGetValue(item.GetKey(), out itemList))
							{
								KFBuyBocaiData temp = itemList.Find((KFBuyBocaiData x) => x.BuyValue.Equals(item.BuyValue));
								if (temp == null)
								{
									itemList.Add(item);
								}
								else
								{
									temp.BuyNum += item.BuyNum;
								}
							}
							else
							{
								itemList = new List<KFBuyBocaiData>();
								itemList.Add(item);
								this.RoleBuyDict.Add(item.GetKey(), itemList);
							}
						}
					}
					if (data.XiaoHaoDaiBi < 1)
					{
						data.XiaoHaoDaiBi = this.OpenData.XiaoHaoDaiBi;
					}
					this.OpenData = data;
					this.PeriodsStartTime = DateTime.Parse(TimeUtil.DataTimeToString(time, "yyyy-MM-dd HH:mm:ss"));
					this.SetUpToDBOpenData();
					if (DateTime.Parse(this.Config.KaiJiangShiJian) >= time)
					{
						this.Stage = BoCaiStageEnum.Stage_Buy;
						LogManager.WriteLog(LogTypes.Info, string.Format("[ljl_CaiShuZi_猜数字] 和上期是一期 并且没开奖 BoCaiType={1},DataPeriods={0}", this.MaxPeriods, this.BoCaiType), null, true);
					}
					else if ((DateTime.Parse("23:59:59") - time).TotalMinutes < 2.0)
					{
						this.Stage = BoCaiStageEnum.Stage_Open;
						this.SetUpToDBOpenData();
						LogManager.WriteLog(LogTypes.Info, string.Format("[ljl_CaiShuZi_猜数字] 和上期是一期 状态设置开奖 &&强制开奖 不足2分钟 BoCaiType={1},DataPeriods={0}", this.MaxPeriods, this.BoCaiType), null, true);
						this.Thread();
					}
					else
					{
						this.Stage = BoCaiStageEnum.Stage_Open;
						LogManager.WriteLog(LogTypes.Info, string.Format("[ljl_CaiShuZi_猜数字] 和上期是一期 状态设置开奖 BoCaiType={1},DataPeriods={0}", this.MaxPeriods, this.BoCaiType), null, true);
					}
					base.KFSendStageData();
					base.KFSendPeriodsData();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_CaiShuZi_猜数字]{0}", ex.ToString()), null, true);
			}
		}

		
		private void GetOldBalance()
		{
			List<OpenLottery> dList;
			KFBoCaiDbManager.SelectOpenLottery((int)this.BoCaiType, string.Format(" AND `strWinNum`!='{0}' ORDER BY `DataPeriods` DESC LIMIT 1;", ""), out dList);
			if (null == dList)
			{
				KFBoCaiDbManager.StopServer("找上期余额失败");
			}
			if (dList.Count > 0)
			{
				this.OpenData.AllBalance = Math.Max(this.OpenData.AllBalance, dList[0].SurplusBalance);
			}
		}

		
		protected override void Init()
		{
			try
			{
				lock (this.mutex)
				{
					DateTime _time = TimeUtil.NowDateTime();
					long delDataPeriods = this.GetNowPeriods(_time.AddYears(-1));
					KFBoCaiDbManager.DelTableData("t_bocai_open_lottery", string.Format("BocaiType={1} AND DataPeriods < {0}", delDataPeriods, (int)this.BoCaiType));
					KFBoCaiDbManager.DelTableData("t_bocai_buy_history", string.Format("BocaiType={1} AND DataPeriods < {0}", delDataPeriods, (int)this.BoCaiType));
					this.InitConfig();
					KFBoCaiDbManager.LoadLotteryHistory(this.BoCaiType, out this.BoCaiWinHistoryList, "");
					KFBoCaiDbManager.SelectOpenLottery((int)this.BoCaiType, this.SelectOpenHisttory10, out this.OpenHistory);
					this.MaxPeriods = KFBoCaiDbManager.GetMaxPeriods((int)this.BoCaiType);
					if (this.MaxPeriods < 0L)
					{
						KFBoCaiDbManager.StopServer("[ljl_caidaxiao_猜数字] 猜数字 maxPeriods == -1");
					}
					else
					{
						if (null == this.Config)
						{
							LogManager.WriteLog(LogTypes.Error, "[ljl_CaiShuZi_猜数字]猜数字配置文件错误", null, true);
						}
						else if (DateTime.Parse(this.Config.KaiQiShiJian) < _time)
						{
							long Periods = this.GetNowPeriods(_time);
							if (this.MaxPeriods == Periods)
							{
								this.StartServerSamePeriods(_time);
								return;
							}
						}
						else
						{
							LogManager.WriteLog(LogTypes.Info, string.Format("[ljl_CaiShuZi_猜数字] 未开启 开启时间 {0}", this.Config.KaiQiShiJian), null, true);
						}
						this.GetOldBalance();
						this.Stage = BoCaiStageEnum.Stage_Ready;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_CaiShuZi_猜数字]{0}", ex.ToString()), null, true);
				KFBoCaiDbManager.StopServer("初始化 Exception");
			}
		}

		
		private bool StartBuy(DateTime time, long Periods, long SurplusBalance = 0L)
		{
			this.PeriodsStartTime = DateTime.Parse(TimeUtil.DataTimeToString(time, "yyyy-MM-dd HH:mm:ss"));
			this.OpenData.DataPeriods = Periods;
			this.OpenData.strWinNum = "";
			this.OpenData.WinInfo = "";
			this.OpenData.BocaiType = (int)this.BoCaiType;
			this.OpenData.SurplusBalance = SurplusBalance;
			this.SetUpToDBOpenData();
			bool result;
			if (!KFBoCaiDbManager.InserOpenLottery(this.OpenData))
			{
				LogManager.WriteLog(LogTypes.Error, "[ljl_CaiShuZi_猜数字]KFBoCaiDbManager.InserOpenLottery(data) false", null, true);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		
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
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
			}
		}

		
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
							if (DateTime.Parse(this.Config.KaiJiangShiJian).AddSeconds((double)(-(double)this.StopBuyTime)) <= _time)
							{
								return;
							}
							if (DateTime.Parse(this.Config.KaiQiShiJian) > _time)
							{
								if (reload)
								{
									this.InitConfig();
								}
								return;
							}
							if (DateTime.Parse(this.Config.JieShuShiJian) <= _time)
							{
								this.InitConfig();
								base.KFSendStageData();
								return;
							}
						}
						if (BoCaiStageEnum.Stage_Ready == this.Stage)
						{
							if (this.StartBuy(_time, this.GetNowPeriods(_time), 0L))
							{
								this.Stage = BoCaiStageEnum.Stage_Buy;
								base.KFSendPeriodsData();
								base.KFSendStageData();
								this.UpBalanceTime = _time;
							}
						}
						else if (BoCaiStageEnum.Stage_Buy == this.Stage && (DateTime.Parse(this.Config.KaiJiangShiJian).AddSeconds((double)(-(double)this.StopBuyTime)) <= _time || this.PeriodsStartTime.Day != _time.Day))
						{
							this.Stage = BoCaiStageEnum.Stage_Stop;
							base.KFSendPeriodsData();
							base.KFSendStageData();
						}
						else if (BoCaiStageEnum.Stage_Buy == this.Stage && (_time - this.UpBalanceTime).TotalSeconds > 600.0)
						{
							this.UpBalanceTime = _time;
							base.KFSendPeriodsData();
						}
						else if (BoCaiStageEnum.Stage_Stop == this.Stage && (DateTime.Parse(this.Config.KaiJiangShiJian) <= _time || this.PeriodsStartTime.Day != _time.Day))
						{
							this.Stage = BoCaiStageEnum.Stage_Open;
							base.KFSendStageData();
						}
						else if (BoCaiStageEnum.Stage_End == this.Stage && this.PeriodsStartTime.Day != _time.Day)
						{
							this.RoleBuyDict.Clear();
							this.OpenData.AllBalance = this.OpenData.SurplusBalance;
							this.InitConfig();
							this.Stage = BoCaiStageEnum.Stage_Ready;
							base.KFSendStageData();
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_CaiShuZi_猜数字]{0}", ex.ToString()), null, true);
			}
		}

		
		private void GetWinRoleNum(List<int> value, out int no1Num, out int no2Num, out int no3Num, out List<KFBoCaoHistoryData> Hsitory)
		{
			no1Num = 0;
			no2Num = 0;
			no3Num = 0;
			Hsitory = new List<KFBoCaoHistoryData>();
			lock (this.mutex)
			{
				foreach (List<KFBuyBocaiData> BuyDataList in this.RoleBuyDict.Values)
				{
					foreach (KFBuyBocaiData BuyData in BuyDataList)
					{
						int sameNum = 0;
						List<int> tempList;
						KFBoCaiDbManager.String2ListInt(BuyData.BuyValue, out tempList);
						if (tempList.Count == value.Count)
						{
							for (int i = 0; i < value.Count; i++)
							{
								if (value[i] == tempList[i])
								{
									sameNum++;
								}
							}
							KFBoCaoHistoryData HistoryData = new KFBoCaoHistoryData();
							HistoryData.DataPeriods = this.OpenData.DataPeriods;
							HistoryData.RoleID = BuyData.RoleID;
							HistoryData.ZoneID = BuyData.ZoneID;
							HistoryData.ServerID = BuyData.ServerID;
							HistoryData.RoleName = BuyData.RoleName;
							HistoryData.BuyNum = BuyData.BuyNum;
							if (5 == sameNum)
							{
								no1Num += BuyData.BuyNum;
								HistoryData.WinNo = 1;
							}
							else if (4 == sameNum)
							{
								no2Num += BuyData.BuyNum;
								HistoryData.WinNo = 2;
							}
							else
							{
								if (3 != sameNum)
								{
									continue;
								}
								no3Num += BuyData.BuyNum;
								HistoryData.WinNo = 3;
							}
							Hsitory.Add(HistoryData);
						}
					}
				}
			}
		}

		
		public override void Thread()
		{
			try
			{
				lock (this.mutex)
				{
					if (BoCaiStageEnum.Stage_Ready < this.Stage && this.UpToDBOpenData.AllBalance != this.OpenData.AllBalance && this.OpenData.DataPeriods > 1L)
					{
						this.SetUpToDBOpenData();
						if (!KFBoCaiDbManager.InserOpenLottery(this.OpenData))
						{
							this.UpToDBOpenData.AllBalance = 0L;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
			}
			if (BoCaiStageEnum.Stage_Open == this.Stage)
			{
				try
				{
					List<int> value = new List<int>();
					int no3Num = 0;
					if ((long)this.Config.ChuFaBiZhong <= this.OpenData.AllBalance && this.RoleBuyDict.Count > 0)
					{
						int index = Global.GetRandomNumber(0, this.RoleBuyDict.Count);
						List<string> keyList = this.RoleBuyDict.Keys.ToList<string>();
						List<KFBuyBocaiData> buyList = this.RoleBuyDict[keyList[index]];
						index = Global.GetRandomNumber(0, buyList.Count);
						KFBoCaiDbManager.String2ListInt(buyList[index].BuyValue, out value);
					}
					else
					{
						while (value.Count < 5)
						{
							value.Add(Global.GetRandomNumber(0, 10));
						}
					}
					int no1Num;
					int no2Num;
					List<KFBoCaoHistoryData> Hsitory;
					this.GetWinRoleNum(value, out no1Num, out no2Num, out no3Num, out Hsitory);
					LogManager.WriteLog(LogTypes.Info, string.Format("[ljl_CaiShuZi_猜数字]猜数1等奖人数={0}，二等奖={1}，3等奖={2}", no1Num, no2Num, no3Num), null, true);
					long No1Money = 0L;
					long No2Money = 0L;
					long No3Money = 0L;
					lock (this.mutex)
					{
						long no1Win = (long)((double)this.OpenData.AllBalance * this.Config.AnNiuList[0].Percent);
						long no2Win = (long)((double)this.OpenData.AllBalance * this.Config.AnNiuList[1].Percent);
						long no3Win = (long)((double)this.OpenData.AllBalance * this.Config.AnNiuList[2].Percent);
						this.OpenData.SurplusBalance = this.OpenData.AllBalance;
						if (no1Num > 0)
						{
							this.OpenData.SurplusBalance -= no1Win;
							No1Money = no1Win / (long)no1Num;
						}
						if (no2Num > 0)
						{
							this.OpenData.SurplusBalance -= no2Win;
							No2Money = no2Win / (long)no2Num;
						}
						if (no3Num > 0)
						{
							this.OpenData.SurplusBalance -= no3Win;
							No3Money = no3Win / (long)no3Num;
						}
						this.OpenData.WinInfo = string.Format("{0},{1},{2}", No1Money, No2Money, No3Money);
						this.OpenData.strWinNum = KFBoCaiDbManager.ListInt2String(value);
						if (!KFBoCaiDbManager.InserOpenLottery(this.OpenData))
						{
							LogManager.WriteLog(LogTypes.Error, "[ljl_CaiShuZi_猜数字]开始计算中奖了 KFBoCaiDbManager.InserOpenLottery(data) false", null, true);
							this.OpenData.SurplusBalance = 0L;
							this.OpenData.WinInfo = "";
							this.OpenData.strWinNum = "";
							return;
						}
						this.BoCaiWinHistoryList.Clear();
						this.BoCaiWinHistoryList.AddRange(Hsitory);
						base.SetOpenHistory(this.GetOpenLottery());
						this.Stage = BoCaiStageEnum.Stage_End;
					}
					foreach (KFBoCaoHistoryData item in Hsitory)
					{
						if (1 == item.WinNo)
						{
							item.WinMoney = (long)item.BuyNum * No1Money;
						}
						else if (2 == item.WinNo)
						{
							item.WinMoney = (long)item.BuyNum * No2Money;
						}
						else if (3 == item.WinNo)
						{
							item.WinMoney = (long)item.BuyNum * No3Money;
						}
						if (!KFBoCaiDbManager.InsertLotteryHistory(this.BoCaiType, item))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("[ljl_CaiShuZi_猜数字]插入中奖历史 false DataPeriods ={0}, name={1},id={2},WinNo={3},WinMoney={4}", new object[]
							{
								item.DataPeriods,
								item.RoleName,
								item.RoleID,
								item.WinNo,
								item.WinMoney
							}), null, true);
						}
					}
					if (!KFBoCaiDbManager.DelTableData("t_bocai_lottery_history", string.Format("DataPeriods < {0}", this.OpenData.DataPeriods)))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("[ljl_CaiShuZi_猜数字] DelTableData  t_bocai_lottery_history false DataPeriods ={0}", this.OpenData.DataPeriods), null, true);
					}
					base.KFSendPeriodsData();
					base.KFSendStageData();
				}
				catch (Exception ex)
				{
					this.Stage = BoCaiStageEnum.Stage_End;
					LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_CaiShuZi_猜数字]{0}", ex.ToString()), null, true);
				}
			}
		}

		
		private long GetNowPeriods(DateTime _time)
		{
			return Convert.ToInt64(string.Format("{0}1", TimeUtil.DataTimeToString(_time, "yyMMdd")));
		}

		
		public bool IsCanBuy(string buyValue, int buyNum, long DataPeriods)
		{
			bool result;
			if (this.Stage != BoCaiStageEnum.Stage_Buy || DataPeriods != this.OpenData.DataPeriods)
			{
				result = false;
			}
			else
			{
				List<int> value = new List<int>();
				KFBoCaiDbManager.String2ListInt(buyValue, out value);
				result = (value.Count == 5);
			}
			return result;
		}

		
		public bool BuyBoCai(KFBuyBocaiData data)
		{
			bool result;
			lock (this.mutex)
			{
				bool flag = false;
				List<KFBuyBocaiData> itemList;
				if (this.RoleBuyDict.TryGetValue(data.GetKey(), out itemList))
				{
					KFBuyBocaiData temp = itemList.Find((KFBuyBocaiData x) => x.BuyValue.Equals(data.BuyValue));
					if (temp == null)
					{
						if (KFBoCaiDbManager.InserBuyBocai(this.OpenData.DataPeriods, data))
						{
							itemList.Add(data);
							flag = true;
						}
					}
					else
					{
						data.BuyNum += temp.BuyNum;
						if (KFBoCaiDbManager.InserBuyBocai(this.OpenData.DataPeriods, data))
						{
							temp.BuyNum = data.BuyNum;
							flag = true;
						}
					}
				}
				else if (KFBoCaiDbManager.InserBuyBocai(this.OpenData.DataPeriods, data))
				{
					itemList = new List<KFBuyBocaiData>();
					itemList.Add(data);
					this.RoleBuyDict.Add(data.GetKey(), itemList);
					flag = true;
				}
				if (flag)
				{
					this.OpenData.AllBalance += (long)((double)(data.BuyNum * this.OpenData.XiaoHaoDaiBi) * (1.0 - this.Config.XiTongChouCheng));
				}
				result = flag;
			}
			return result;
		}

		
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
					data.isOpen = (DateTime.Parse(this.Config.KaiQiShiJian) <= TimeUtil.NowDateTime() && DateTime.Parse(this.Config.JieShuShiJian) >= TimeUtil.NowDateTime());
					data.isOpenDay = data.isOpen;
					if (data.isOpen)
					{
						data.OpenTime = base.GetDiffTime(DateTime.Parse(this.Config.KaiJiangShiJian), TimeUtil.NowDateTime(), true);
						data.LastOpenTime = this.GetLastTime((BoCaiStageEnum)data.Stage);
					}
					else
					{
						data.OpenTime = base.GetDiffTime(DateTime.Parse(this.Config.KaiQiShiJian), TimeUtil.NowDateTime(), false);
					}
					result = data;
				}
			}
			return result;
		}

		
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
					return base.GetDiffTime(DateTime.Parse(this.Config.KaiJiangShiJian).AddSeconds((double)(-(double)this.StopBuyTime)), _time, true);
				}
				if (BoCaiStageEnum.Stage_Stop == stage)
				{
					return base.GetDiffTime(DateTime.Parse(this.Config.KaiJiangShiJian), _time, true);
				}
				if (BoCaiStageEnum.Stage_Open <= stage)
				{
					if (this.PeriodsStartTime.Day == _time.Day)
					{
						return base.GetDiffTime(DateTime.Parse("23:59:59"), _time, true);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_CaiShuZi_猜数字]{0}", ex.ToString()), null, true);
			}
			return 0L;
		}

		
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

		
		private const int UpAllBalanceTime = 600;

		
		private static KFBoCaiCaiShuzi instance = new KFBoCaiCaiShuzi();

		
		private CaiShuZiConfig Config = null;

		
		private DateTime UpBalanceTime;
	}
}
