using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Logic.TuJian;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.Today
{
	// Token: 0x0200044E RID: 1102
	public class TodayManager : ICmdProcessorEx, ICmdProcessor, IManager
	{
		// Token: 0x0600142E RID: 5166 RVA: 0x0013E214 File Offset: 0x0013C414
		public static TodayManager getInstance()
		{
			return TodayManager.instance;
		}

		// Token: 0x0600142F RID: 5167 RVA: 0x0013E22C File Offset: 0x0013C42C
		public bool initialize()
		{
			TodayManager.InitConfig();
			return true;
		}

		// Token: 0x06001430 RID: 5168 RVA: 0x0013E248 File Offset: 0x0013C448
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1030, 1, 1, TodayManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1031, 2, 2, TodayManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			return true;
		}

		// Token: 0x06001431 RID: 5169 RVA: 0x0013E28C File Offset: 0x0013C48C
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06001432 RID: 5170 RVA: 0x0013E2A0 File Offset: 0x0013C4A0
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06001433 RID: 5171 RVA: 0x0013E2B4 File Offset: 0x0013C4B4
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return true;
		}

		// Token: 0x06001434 RID: 5172 RVA: 0x0013E2C8 File Offset: 0x0013C4C8
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			switch (nID)
			{
			case 1030:
				result = this.ProcessCmdTodayData(client, nID, bytes, cmdParams);
				break;
			case 1031:
				result = this.ProcessCmdTodayAward(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		// Token: 0x06001435 RID: 5173 RVA: 0x0013E310 File Offset: 0x0013C510
		private bool ProcessCmdTodayData(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				string result = this.GetTodayData(client);
				client.sendCmd(1030, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06001436 RID: 5174 RVA: 0x0013E380 File Offset: 0x0013C580
		private bool ProcessCmdTodayAward(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 2))
				{
					return false;
				}
				bool isAll = int.Parse(cmdParams[0]) > 0;
				int todayID = int.Parse(cmdParams[1]);
				string result = this.TodayAward(client, isAll, todayID);
				client.sendCmd(1031, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06001437 RID: 5175 RVA: 0x0013E410 File Offset: 0x0013C610
		private string GetTodayData(GameClient client)
		{
			string result = "{0}:{1}";
			string result2;
			if (!this.IsGongNengOpened())
			{
				result2 = string.Format(result, -11, 0);
			}
			else
			{
				List<TodayInfo> list = this.InitToday(client);
				if (list.IsNullOrEmpty<TodayInfo>())
				{
					result2 = string.Format(result, -11, 0);
				}
				else
				{
					StringBuilder sb = new StringBuilder();
					int i = 0;
					while (i < list.Count)
					{
						sb.Append(string.Format("{0}*{1}", list[i].ID, list[i].NumEnd));
						i++;
						if (i < list.Count)
						{
							sb.Append("|");
						}
					}
					result2 = string.Format(result, 1, sb.ToString());
				}
			}
			return result2;
		}

		// Token: 0x06001438 RID: 5176 RVA: 0x0013E598 File Offset: 0x0013C798
		private string TodayAward(GameClient client, bool isAll, int todayID)
		{
			string result = "{0}:{1}";
			string result2;
			if (!this.IsGongNengOpened())
			{
				result2 = string.Format(result, -11, 0);
			}
			else
			{
				TodayInfo oneInfo = null;
				if (!isAll)
				{
					oneInfo = this.GetTadayInfoByID(client, todayID);
					if (oneInfo == null)
					{
						return string.Format(result, -3, 0);
					}
					if (oneInfo.NumMax - oneInfo.NumEnd <= 0)
					{
						return string.Format(result, -4, 0);
					}
				}
				List<TodayInfo> listAll = new List<TodayInfo>();
				if (isAll)
				{
					listAll = this.InitToday(client);
				}
				else
				{
					listAll.Add(oneInfo);
				}
				if (listAll.IsNullOrEmpty<TodayInfo>())
				{
					result2 = string.Format(result, -3, 0);
				}
				else
				{
					IEnumerable<TodayInfo> fubenList = from info in listAll
					where info.FuBenID > 0 && client.ClientData.FuBenID > 0 && client.ClientData.FuBenID == info.FuBenID && info.NumMax - info.NumEnd > 0 && info.NumEnd >= 0
					select info;
					if (fubenList.Any<TodayInfo>())
					{
						result2 = string.Format(result, -6, 0);
					}
					else
					{
						IEnumerable<TodayInfo> awardList = from info in listAll
						where info.NumMax - info.NumEnd > 0
						select info;
						if (!awardList.Any<TodayInfo>())
						{
							result2 = string.Format(result, -5, 0);
						}
						else
						{
							int goodsCount = 0;
							foreach (TodayInfo info2 in awardList)
							{
								goodsCount += info2.AwardInfo.GoodsList.Count;
							}
							if (!Global.CanAddGoodsNum(client, goodsCount))
							{
								result2 = string.Format(result, -2, 0);
							}
							else
							{
								foreach (TodayInfo info2 in awardList)
								{
									SystemXmlItem fuBenInfo = null;
									if (info2.Type == 6)
									{
										TaskData taskData = TodayManager.GetTaoTask(client);
										if (taskData != null)
										{
											if (!Global.CancelTask(client, taskData.DbID, taskData.DoingTaskID))
											{
												return string.Format(result, -8, 0);
											}
										}
									}
									else if (info2.Type == 9)
									{
										BufferData bufferData = Global.GetBufferDataByID(client, 34);
										if (bufferData != null)
										{
											bufferData.BufferVal = 0L;
											bufferData.BufferSecs = 0;
											GameManager.ClientMgr.NotifyBufferData(client, bufferData);
										}
									}
									else if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(info2.FuBenID, out fuBenInfo))
									{
										return string.Format(result, -7, 0);
									}
									if (!this.SetFinishNum(client, info2, fuBenInfo))
									{
										return string.Format(result, -1, 0);
									}
								}
								TodayAwardInfo awardInfo = new TodayAwardInfo();
								foreach (TodayInfo info2 in awardList)
								{
									int num = info2.NumMax - info2.NumEnd;
									for (int i = 0; i < info2.AwardInfo.GoodsList.Count; i++)
									{
										GoodsData goods = info2.AwardInfo.GoodsList[i];
										Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goods.GoodsID, goods.GCount * num, goods.Quality, "", goods.Forge_level, goods.Binding, 0, "", true, 1, "每日专享", "1900-01-01 12:00:00", goods.AddPropIndex, goods.BornIndex, goods.Lucky, goods.Strong, goods.ExcellenceInfo, goods.AppendPropLev, 0, null, null, 0, true);
									}
									awardInfo.AddAward(info2.AwardInfo, num);
								}
								if (awardInfo.Exp > 0.0)
								{
									GameManager.ClientMgr.ProcessRoleExperience(client, (long)awardInfo.Exp, true, true, false, "none");
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(30, new object[0]), new object[]
									{
										awardInfo.Exp
									}), GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlyErr, 0);
								}
								if (awardInfo.GoldBind > 0.0)
								{
									GameManager.ClientMgr.AddMoney1(client, (int)awardInfo.GoldBind, "每日专享", true);
								}
								if (awardInfo.MoJing > 0.0)
								{
									GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, (int)awardInfo.MoJing, "每日专享", true, true, false);
								}
								if (awardInfo.ChengJiu > 0.0)
								{
									GameManager.ClientMgr.ModifyChengJiuPointsValue(client, (int)awardInfo.ChengJiu, "每日专享", true, true);
								}
								if (awardInfo.ShengWang > 0.0)
								{
									GameManager.ClientMgr.ModifyShengWangValue(client, (int)awardInfo.ShengWang, "每日专享", true, true);
								}
								if (awardInfo.ZhanGong > 0.0)
								{
									int zhanGong = (int)awardInfo.ZhanGong;
									GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref zhanGong, AddBangGongTypes.Today, 0);
								}
								if (awardInfo.DiamondBind > 0.0 || awardInfo.ExtDiamondBind > 0.0)
								{
									GameManager.ClientMgr.AddUserGold(client, (int)(awardInfo.DiamondBind + awardInfo.ExtDiamondBind), "每日专享");
								}
								if (awardInfo.XingHun > 0.0)
								{
									GameManager.ClientMgr.ModifyStarSoulValue(client, (int)awardInfo.XingHun, "每日专享", true, true);
								}
								if (awardInfo.YuanSuFenMo > 0.0)
								{
									GameManager.ClientMgr.ModifyYuanSuFenMoValue(client, (int)awardInfo.YuanSuFenMo, "每日专享", true, false);
								}
								if (awardInfo.ShouHuDianShu > 0.0)
								{
									SingletonTemplate<GuardStatueManager>.Instance().AddGuardPoint(client, (int)awardInfo.ShouHuDianShu, "每日专享");
								}
								if (awardInfo.ZaiZao > 0.0)
								{
									GameManager.ClientMgr.ModifyZaiZaoValue(client, (int)awardInfo.ZaiZao, "每日专享", true, true, false);
								}
								if (awardInfo.LingJing > 0.0)
								{
									GameManager.ClientMgr.ModifyMUMoHeValue(client, (int)awardInfo.LingJing, "每日专享", true, true, false);
								}
								if (awardInfo.RongYao > 0.0)
								{
									GameManager.ClientMgr.ModifyTianTiRongYaoValue(client, (int)awardInfo.RongYao, "每日专享", true);
								}
								result2 = this.GetTodayData(client);
							}
						}
					}
				}
			}
			return result2;
		}

		// Token: 0x06001439 RID: 5177 RVA: 0x0013EEAC File Offset: 0x0013D0AC
		private List<TodayInfo> InitToday(GameClient client)
		{
			List<TodayInfo> infoList = new List<TodayInfo>();
			int level = client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level;
			int taskID = client.ClientData.MainTaskID;
			IEnumerable<TodayInfo> tempList = from info in TodayManager._todayInfoList
			where level >= info.LevelMin && level <= info.LevelMax && taskID >= info.TaskMin
			select info;
			foreach (TodayInfo t in tempList)
			{
				TodayInfo info2 = new TodayInfo(t);
				info2.NumEnd = this.GetFinishNum(client, info2);
				info2.NumMax = this.GetMaxNum(client, info2);
				infoList.Add(info2);
			}
			return infoList;
		}

		// Token: 0x0600143A RID: 5178 RVA: 0x0013EFE8 File Offset: 0x0013D1E8
		private TodayInfo GetTadayInfoByID(GameClient client, int id)
		{
			TodayInfo result = null;
			int taskID = client.ClientData.MainTaskID;
			int level = client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level;
			IEnumerable<TodayInfo> temp = from info in TodayManager._todayInfoList
			where info.ID == id && level >= info.LevelMin && level <= info.LevelMax && taskID >= info.TaskMin
			select info;
			TodayInfo result2;
			if (!temp.Any<TodayInfo>())
			{
				result2 = null;
			}
			else
			{
				TodayInfo tempInfo = temp.First<TodayInfo>();
				if (tempInfo != null)
				{
					result = new TodayInfo(tempInfo);
					result.NumEnd = this.GetFinishNum(client, result);
					result.NumMax = this.GetMaxNum(client, result);
				}
				result2 = result;
			}
			return result2;
		}

		// Token: 0x0600143B RID: 5179 RVA: 0x0013F09C File Offset: 0x0013D29C
		private int GetMaxNum(GameClient client, TodayInfo todayInfo)
		{
			int num = 0;
			switch (todayInfo.Type)
			{
			case 1:
				num = todayInfo.NumMax;
				break;
			case 2:
				num = todayInfo.NumMax;
				break;
			case 3:
			case 4:
			case 5:
				num = todayInfo.NumMax;
				break;
			case 6:
			{
				DailyTaskData dailyTaskData = Global.FindDailyTaskDataByTaskClass(client, 9);
				if (null == dailyTaskData)
				{
					num = Global.MaxTaofaTaskNumForMU;
				}
				else
				{
					num = Global.GetMaxDailyTaskNum(client, 9, dailyTaskData);
				}
				break;
			}
			case 7:
			case 8:
			case 9:
				num = todayInfo.NumMax;
				break;
			case 10:
			case 11:
			case 12:
				num = todayInfo.NumMax;
				break;
			}
			return Math.Max(0, num);
		}

		// Token: 0x0600143C RID: 5180 RVA: 0x0013F154 File Offset: 0x0013D354
		private int GetFinishNum(GameClient client, TodayInfo todayInfo)
		{
			int num = 0;
			FuBenData fuBenData = this.GetFuBenData(client, todayInfo.FuBenID);
			switch (todayInfo.Type)
			{
			case 1:
			case 2:
				num = fuBenData.EnterNum;
				break;
			case 3:
			case 4:
			case 5:
				num = fuBenData.FinishNum;
				break;
			case 6:
			{
				DailyTaskData dailyTaskData = Global.FindDailyTaskDataByTaskClass(client, 9);
				num = ((dailyTaskData == null) ? 0 : dailyTaskData.RecNum);
				break;
			}
			case 7:
				num = Global.QueryDayActivityEnterCountToDB(client, client.ClientData.RoleID, TimeUtil.NowDateTime().DayOfYear, 2);
				break;
			case 8:
				num = Global.QueryDayActivityEnterCountToDB(client, client.ClientData.RoleID, TimeUtil.NowDateTime().DayOfYear, 1);
				break;
			case 9:
				num = Global.QueryDayActivityEnterCountToDB(client, client.ClientData.RoleID, TimeUtil.NowDateTime().DayOfYear, 6);
				break;
			case 10:
			case 11:
			case 12:
				num = fuBenData.EnterNum;
				break;
			}
			return num;
		}

		// Token: 0x0600143D RID: 5181 RVA: 0x0013F260 File Offset: 0x0013D460
		private FuBenData GetFuBenData(GameClient client, int fuBenID)
		{
			bool isNotify = false;
			FuBenData fuBenData = Global.GetFuBenData(client, fuBenID);
			int dayID = TimeUtil.NowDateTime().DayOfYear;
			if (null == fuBenData)
			{
				fuBenData = Global.AddFuBenData(client, fuBenID, dayID, 0, 0, 0);
			}
			if (fuBenData.DayID != dayID)
			{
				fuBenData.DayID = dayID;
				fuBenData.EnterNum = 0;
				fuBenData.FinishNum = 0;
				isNotify = true;
			}
			if (isNotify)
			{
				GameManager.ClientMgr.NotifyFuBenData(client, fuBenData);
			}
			return fuBenData;
		}

		// Token: 0x0600143E RID: 5182 RVA: 0x0013F2E4 File Offset: 0x0013D4E4
		private bool SetFinishNum(GameClient client, TodayInfo todayInfo, SystemXmlItem fuBenInfo)
		{
			int num = todayInfo.NumMax - todayInfo.NumEnd;
			switch (todayInfo.Type)
			{
			case 1:
			case 2:
				Global.UpdateFuBenData(client, todayInfo.FuBenID, num, num);
				break;
			case 3:
			case 4:
			case 5:
				Global.UpdateFuBenData(client, todayInfo.FuBenID, num, num);
				break;
			case 6:
			{
				DailyTaskData taoData = null;
				Global.GetDailyTaskData(client, 9, out taoData, true);
				taoData.RecNum = todayInfo.NumMax;
				Global.UpdateDBDailyTaskData(client, taoData, true);
				break;
			}
			case 7:
			{
				int nType = 2;
				Global.UpdateDayActivityEnterCountToDB(client, client.ClientData.RoleID, TimeUtil.NowDateTime().DayOfYear, nType, todayInfo.NumMax);
				break;
			}
			case 8:
			{
				int nType = 1;
				Global.UpdateDayActivityEnterCountToDB(client, client.ClientData.RoleID, TimeUtil.NowDateTime().DayOfYear, nType, todayInfo.NumMax);
				break;
			}
			case 9:
			{
				int nType = 6;
				Global.UpdateDayActivityEnterCountToDB(client, client.ClientData.RoleID, TimeUtil.NowDateTime().DayOfYear, nType, todayInfo.NumMax);
				break;
			}
			case 10:
			case 11:
			case 12:
				Global.UpdateFuBenData(client, todayInfo.FuBenID, num, num);
				break;
			}
			FuBenData fuBenData = Global.GetFuBenData(client, todayInfo.FuBenID);
			if (fuBenData != null && (fuBenData.EnterNum != 0 || fuBenData.FinishNum != 0))
			{
				int dayID = TimeUtil.NowDateTime().DayOfYear;
				RoleDailyData roleData = client.ClientData.MyRoleDailyData;
				if (roleData == null || dayID != roleData.FuBenDayID)
				{
					roleData.FuBenDayID = dayID;
					roleData.TodayFuBenNum = 0;
				}
				int count = todayInfo.NumMax - todayInfo.NumEnd;
				roleData.TodayFuBenNum += count;
				int level = fuBenInfo.GetIntValue("FuBenLevel", -1);
				DailyActiveManager.ProcessCompleteCopyMapForDailyActive(client, level, count);
				ChengJiuManager.ProcessCompleteCopyMapForChengJiu(client, level, count);
			}
			return true;
		}

		// Token: 0x0600143F RID: 5183 RVA: 0x0013F4F4 File Offset: 0x0013D6F4
		public static TaskData GetTaoTask(GameClient client)
		{
			TaskData result;
			if (null == client.ClientData.TaskDataList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.TaskDataList)
				{
					for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
					{
						TaskData taskData = client.ClientData.TaskDataList[i];
						SystemXmlItem systemTask = null;
						if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskData.DoingTaskID, out systemTask))
						{
							int taskClass = systemTask.GetIntValue("TaskClass", -1);
							if (taskClass == 9)
							{
								return taskData;
							}
						}
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x06001440 RID: 5184 RVA: 0x0013F5E4 File Offset: 0x0013D7E4
		public bool IsGongNengOpened()
		{
			return !GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot7) && GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("Today");
		}

		// Token: 0x06001441 RID: 5185 RVA: 0x0013F624 File Offset: 0x0013D824
		public static void InitConfig()
		{
			string fileName = Global.GameResPath("Config/JianFu.xml");
			XElement xml = CheckHelper.LoadXml(fileName, true);
			if (null != xml)
			{
				try
				{
					List<TodayInfo> todayInfoList = new List<TodayInfo>();
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (xmlItem != null)
						{
							TodayInfo info = new TodayInfo();
							info.ID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
							info.Type = info.ID / 100;
							info.Name = Global.GetDefAttributeStr(xmlItem, "Name", "0");
							info.FuBenID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "FuBenID", "0"));
							info.HuoDongID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "HuoDongID", "0"));
							info.LevelMin = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "MinZhuanSheng", "0")) * 100 + Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "MinLevel", "0"));
							info.LevelMax = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "MaxZhuanSheng", "0")) * 100 + Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "MaxLevel", "0"));
							info.TaskMin = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "MinTasks", "0"));
							info.NumMax = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Num", "0"));
							TodayAwardInfo awardInfo = new TodayAwardInfo();
							awardInfo.Exp = (double)Convert.ToInt64(Global.GetDefAttributeStr(xmlItem, "Exp", "0"));
							awardInfo.GoldBind = (double)Convert.ToInt64(Global.GetDefAttributeStr(xmlItem, "BandJinBi", "0"));
							awardInfo.MoJing = (double)Convert.ToInt64(Global.GetDefAttributeStr(xmlItem, "MoJing", "0"));
							awardInfo.ChengJiu = (double)Convert.ToInt64(Global.GetDefAttributeStr(xmlItem, "ChengJiu", "0"));
							awardInfo.ShengWang = (double)Convert.ToInt64(Global.GetDefAttributeStr(xmlItem, "ShengWang", "0"));
							awardInfo.ZhanGong = (double)Convert.ToInt64(Global.GetDefAttributeStr(xmlItem, "ZhanGong", "0"));
							awardInfo.DiamondBind = (double)Convert.ToInt64(Global.GetDefAttributeStr(xmlItem, "BandZuanShi", "0"));
							awardInfo.XingHun = (double)Convert.ToInt64(Global.GetDefAttributeStr(xmlItem, "XingHun", "0"));
							awardInfo.YuanSuFenMo = (double)Convert.ToInt64(Global.GetDefAttributeStr(xmlItem, "YuanSuFenMo", "0"));
							awardInfo.ShouHuDianShu = (double)Convert.ToInt64(Global.GetDefAttributeStr(xmlItem, "ShouHuDianShu", "0"));
							awardInfo.ZaiZao = (double)Convert.ToInt64(Global.GetDefAttributeStr(xmlItem, "ZaiZao", "0"));
							awardInfo.LingJing = (double)Convert.ToInt64(Global.GetDefAttributeStr(xmlItem, "LingJing", "0"));
							awardInfo.RongYao = (double)Convert.ToInt64(Global.GetDefAttributeStr(xmlItem, "RongYao", "0"));
							awardInfo.ExtDiamondBind = (double)Convert.ToInt64(Global.GetDefAttributeStr(xmlItem, "ExtraBandZuanShi", "0"));
							string goods = Global.GetDefAttributeStr(xmlItem, "Goods", "0");
							if (!string.IsNullOrEmpty(goods) && !goods.Equals("0"))
							{
								string[] fields = goods.Split(new char[]
								{
									'|'
								});
								awardInfo.GoodsList = GoodsHelper.ParseGoodsDataList(fields, fileName);
							}
							info.AwardInfo = awardInfo;
							todayInfoList.Add(info);
						}
					}
					TodayManager._todayInfoList = todayInfoList;
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("加载[{0}]时出错!!!", fileName), null, true);
				}
			}
		}

		// Token: 0x04001DC7 RID: 7623
		private static TodayManager instance = new TodayManager();

		// Token: 0x04001DC8 RID: 7624
		private static List<TodayInfo> _todayInfoList = new List<TodayInfo>();
	}
}
