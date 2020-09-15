using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.JingJiChang;
using GameServer.Server;
using GameServer.Tools;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x0200041D RID: 1053
	public class ZhengBaManager : SingletonTemplate<ZhengBaManager>, IManager, ICmdProcessorEx, ICmdProcessor, IEventListenerEx, IEventListener
	{
		// Token: 0x06001316 RID: 4886 RVA: 0x0012C178 File Offset: 0x0012A378
		private ZhengBaManager()
		{
		}

		// Token: 0x06001317 RID: 4887 RVA: 0x0012C268 File Offset: 0x0012A468
		public bool initialize()
		{
			bool result;
			if (!this._Config.Load(Global.GameResPath("Config\\Match.xml"), Global.GameResPath("Config\\Sustain.xml"), Global.GameResPath("Config\\MatchBirthPoint.xml")))
			{
				result = false;
			}
			else
			{
				XElement xml = XElement.Load(Global.GameResPath("Config\\MatchAward.xml"));
				foreach (XElement xmlItem in xml.Elements())
				{
					ZhengBaMatchAward award = new ZhengBaMatchAward();
					award.AwardId = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
					award.Name = Global.GetSafeAttributeStr(xmlItem, "Name");
					award.FinalPassDay = (int)Global.GetSafeAttributeLong(xmlItem, "FinalPassDay");
					award.GoodsList = GoodsHelper.ParseGoodsDataList(Global.GetSafeAttributeStr(xmlItem, "Award").Split(new char[]
					{
						'|'
					}), "Config\\MatchAward.xml");
					Debug.Assert(award.FinalPassDay >= 0 && award.FinalPassDay <= 7);
					Debug.Assert(award.GoodsList != null);
					this._MatchAwardList.Add(award);
				}
				foreach (ZhengBaSupportConfig support in this._Config.SupportConfigList)
				{
					string winAwardTag = (string)support.WinAwardTag;
					string failAwardTag = (string)support.FailAwardTag;
					List<GoodsData> winAwardGoodsList = GoodsHelper.ParseGoodsDataList(winAwardTag.Split(new char[]
					{
						'|'
					}), "Config\\Sustain.xml");
					List<GoodsData> failAwardGoodsList = GoodsHelper.ParseGoodsDataList(failAwardTag.Split(new char[]
					{
						'|'
					}), "Config\\Sustain.xml");
					support.FailAwardTag = failAwardGoodsList;
					support.WinAwardTag = winAwardGoodsList;
					int totalPoint = 0;
					foreach (GoodsData goods in winAwardGoodsList)
					{
						SystemXmlItem xmlItem2 = null;
						List<MagicActionItem> magicActionItemList = null;
						if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goods.GoodsID, out xmlItem2) || !GameManager.SystemMagicActionMgr.GoodsActionsDict.TryGetValue(goods.GoodsID, out magicActionItemList))
						{
							LogManager.WriteLog(LogTypes.Fatal, string.Format("众神争霸goods={0}找不到对应的action", new object[0]), null, true);
						}
						else
						{
							foreach (MagicActionItem action in magicActionItemList)
							{
								if (action.MagicActionID == MagicActionIDs.ADD_ZHENGBADIANSHU)
								{
									totalPoint += (int)action.MagicActionParams[0] * goods.GCount;
								}
							}
						}
					}
					support.WinPoint = totalPoint;
					totalPoint = 0;
					foreach (GoodsData goods in failAwardGoodsList)
					{
						SystemXmlItem xmlItem2 = null;
						List<MagicActionItem> magicActionItemList = null;
						if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goods.GoodsID, out xmlItem2) || !GameManager.SystemMagicActionMgr.GoodsActionsDict.TryGetValue(goods.GoodsID, out magicActionItemList))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("众神争霸goods={0}找不到对应的action", goods.GoodsID), null, true);
						}
						else
						{
							foreach (MagicActionItem action in magicActionItemList)
							{
								if (action.MagicActionID == MagicActionIDs.ADD_ZHENGBADIANSHU)
								{
									totalPoint += (int)action.MagicActionParams[0] * goods.GCount;
								}
							}
						}
					}
					support.FailPoint = totalPoint;
				}
				DateTime now = TimeUtil.NowDateTime();
				int month = ZhengBaUtils.MakeMonth(now);
				List<ZhengBaPkLogData> pkLogList = Global.sendToDB<List<ZhengBaPkLogData>, string>(14014, string.Format("{0}:{1}", month, 100), 0);
				Dictionary<int, List<ZhengBaSupportLogData>> supportLogs = Global.sendToDB<Dictionary<int, List<ZhengBaSupportLogData>>, string>(14013, string.Format("{0}:{1}", month, 30), 0);
				List<ZhengBaWaitYaZhuAwardData> waitAwardOfYaZhuList = Global.sendToDB<List<ZhengBaWaitYaZhuAwardData>, string>(14016, string.Format("{0}", month), 0);
				if (pkLogList != null)
				{
					pkLogList.RemoveAll((ZhengBaPkLogData _log) => !_log.UpGrade);
					foreach (ZhengBaPkLogData log in pkLogList)
					{
						this.PkLogQ.Enqueue(log);
					}
				}
				if (supportLogs != null)
				{
					foreach (KeyValuePair<int, List<ZhengBaSupportLogData>> kvp in supportLogs)
					{
						Queue<ZhengBaSupportLogData> logQ = new Queue<ZhengBaSupportLogData>();
						this.SupportLogs[kvp.Key] = logQ;
						foreach (ZhengBaSupportLogData log2 in kvp.Value)
						{
							logQ.Enqueue(log2);
						}
					}
				}
				if (waitAwardOfYaZhuList != null)
				{
					this.WaitAwardOfYaZhuList = waitAwardOfYaZhuList;
				}
				this.SyncData.Month = month;
				this.SyncData.RoleModTime = DateTime.MinValue;
				this.SyncData.SupportModTime = DateTime.MinValue;
				this.SyncData.RealActDay = -1;
				ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("ZhengBaManager.TimerProc", new EventHandler(this.SyncCenterData)), 20000, 10000);
				ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("ZhengBaManager.TimerProc", new EventHandler(this.CheckYaZhuAward)), 20000, 120000);
				ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("ZhengBaManager.UpdateCopyScene", new EventHandler(this.UpdateCopyScene)), 10000, 100);
				result = true;
			}
			return result;
		}

		// Token: 0x06001318 RID: 4888 RVA: 0x0012C9B8 File Offset: 0x0012ABB8
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1350, 1, 1, SingletonTemplate<ZhengBaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1351, 1, 1, SingletonTemplate<ZhengBaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1352, 1, 1, SingletonTemplate<ZhengBaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1353, 2, 2, SingletonTemplate<ZhengBaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1354, 4, 4, SingletonTemplate<ZhengBaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1355, 2, 2, SingletonTemplate<ZhengBaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1357, 2, 2, SingletonTemplate<ZhengBaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1359, 1, 1, SingletonTemplate<ZhengBaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1360, 1, 1, SingletonTemplate<ZhengBaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1361, 1, 1, SingletonTemplate<ZhengBaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1362, 2, 2, SingletonTemplate<ZhengBaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(10020, 36, SingletonTemplate<ZhengBaManager>.Instance());
			GlobalEventSource4Scene.getInstance().registerListener(10021, 36, SingletonTemplate<ZhengBaManager>.Instance());
			GlobalEventSource4Scene.getInstance().registerListener(10022, 36, SingletonTemplate<ZhengBaManager>.Instance());
			GlobalEventSource4Scene.getInstance().registerListener(10023, 36, SingletonTemplate<ZhengBaManager>.Instance());
			GlobalEventSource4Scene.getInstance().registerListener(10024, 36, SingletonTemplate<ZhengBaManager>.Instance());
			GlobalEventSource.getInstance().registerListener(10, SingletonTemplate<ZhengBaManager>.Instance());
			GlobalEventSource.getInstance().registerListener(11, SingletonTemplate<ZhengBaManager>.Instance());
			return true;
		}

		// Token: 0x06001319 RID: 4889 RVA: 0x0012CB6C File Offset: 0x0012AD6C
		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(10020, 36, SingletonTemplate<ZhengBaManager>.Instance());
			GlobalEventSource4Scene.getInstance().removeListener(10021, 36, SingletonTemplate<ZhengBaManager>.Instance());
			GlobalEventSource4Scene.getInstance().removeListener(10022, 36, SingletonTemplate<ZhengBaManager>.Instance());
			GlobalEventSource4Scene.getInstance().removeListener(10023, 36, SingletonTemplate<ZhengBaManager>.Instance());
			GlobalEventSource4Scene.getInstance().removeListener(10024, 36, SingletonTemplate<ZhengBaManager>.Instance());
			GlobalEventSource.getInstance().removeListener(10, SingletonTemplate<ZhengBaManager>.Instance());
			GlobalEventSource.getInstance().removeListener(11, SingletonTemplate<ZhengBaManager>.Instance());
			return true;
		}

		// Token: 0x0600131A RID: 4890 RVA: 0x0012CC18 File Offset: 0x0012AE18
		public bool destroy()
		{
			return true;
		}

		// Token: 0x0600131B RID: 4891 RVA: 0x0012CC2C File Offset: 0x0012AE2C
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1350:
					return this.HandleGetMainInfo(client, nID, bytes, cmdParams);
				case 1351:
					return this.HandleGetAllPkLog(client, nID, bytes, cmdParams);
				case 1352:
					return this.HandleGetAllPkState(client, nID, bytes, cmdParams);
				case 1353:
					return this.HandleGet16PkState(client, nID, bytes, cmdParams);
				case 1354:
					return this.HandleSupport(client, nID, bytes, cmdParams);
				case 1355:
					return false;
				case 1357:
					return this.HandleEnter(client, nID, bytes, cmdParams);
				case 1359:
					return this.HandleGetMiniState(client, nID, bytes, cmdParams);
				case 1360:
					return this.HandleQueryJoinHint(client, nID, bytes, cmdParams);
				case 1361:
					return this.ProcessGetAdmireDataCmd(client, nID, bytes, cmdParams);
				case 1362:
					return this.HandleAdmireStatue(client, nID, bytes, cmdParams);
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600131C RID: 4892 RVA: 0x0012CD34 File Offset: 0x0012AF34
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x0600131D RID: 4893 RVA: 0x0012CD64 File Offset: 0x0012AF64
		private bool HandleQueryJoinHint(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int hint = 0;
			DateTime now = TimeUtil.NowDateTime();
			int nowMonth = ZhengBaUtils.MakeMonth(TimeUtil.NowDateTime());
			int oldMonth = Global.GetRoleParamsInt32FromDB(client, "ZhengBaHintFlag");
			lock (this.Mutex)
			{
				if (this.SyncData.Month == nowMonth && oldMonth != nowMonth && this.SyncData.IsThisMonthInActivity && this.IsGongNengOpened() && this.RoleDataDict.ContainsKey(client.ClientData.RoleID))
				{
					if (this.SyncData.RealActDay <= 0)
					{
						hint = 1;
					}
					else
					{
						bool flag2;
						if (this.SyncData.RealActDay == 1)
						{
							flag2 = (now.TimeOfDay.Ticks >= this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == 1).DayBeginTick);
						}
						else
						{
							flag2 = true;
						}
						if (!flag2)
						{
							hint = 1;
						}
						else
						{
							hint = 0;
						}
					}
				}
			}
			client.sendCmd(nID, hint.ToString(), false);
			if (hint == 1)
			{
				Global.SaveRoleParamsInt32ValueToDB(client, "ZhengBaHintFlag", nowMonth, true);
			}
			return true;
		}

		// Token: 0x0600131E RID: 4894 RVA: 0x0012CF8C File Offset: 0x0012B18C
		private bool HandleGetMiniState(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int realDay = 0;
			DateTime nowTime = DateTime.MinValue;
			bool todayIsPking = false;
			bool thisMonthInActivity = false;
			lock (this.Mutex)
			{
				nowTime = TimeUtil.NowDateTime().Add(this.DiffKfCenter);
				realDay = this.SyncData.RealActDay;
				todayIsPking = this.SyncData.TodayIsPking;
				thisMonthInActivity = this.SyncData.IsThisMonthInActivity;
			}
			ZhengBaMiniStateData data = new ZhengBaMiniStateData();
			data.IsZhengBaOpened = this.IsGongNengOpened();
			data.IsThisMonthInActivity = thisMonthInActivity;
			bool result;
			if (!data.IsZhengBaOpened || realDay < 0)
			{
				client.sendCmd<ZhengBaMiniStateData>(nID, data, false);
				result = true;
			}
			else
			{
				if (!thisMonthInActivity)
				{
					DateTime nextMonth = nowTime.AddMonths(1);
					nextMonth = new DateTime(nextMonth.Year, nextMonth.Month, ZhengBaConsts.StartMonthDay, 0, 0, 0);
					nextMonth = nextMonth.AddTicks(this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == 1).DayBeginTick);
					data.PkStartWaitSec = (long)(nextMonth - nowTime).TotalSeconds;
				}
				else if (realDay == 0)
				{
					DateTime end = DateTime.MinValue;
					if (nowTime.AddDays(1.0).Month == nowTime.Month)
					{
						end = new DateTime(nowTime.Year, nowTime.Month, Math.Max(nowTime.Day + 1, ZhengBaConsts.StartMonthDay), 0, 0, 0);
					}
					else if (nowTime.AddMonths(1).Year == nowTime.Year)
					{
						end = new DateTime(nowTime.Year, nowTime.Month + 1, ZhengBaConsts.StartMonthDay, 0, 0, 0);
					}
					else
					{
						end = new DateTime(nowTime.Year + 1, 1, ZhengBaConsts.StartMonthDay, 0, 0, 0);
					}
					end = end.AddTicks(this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == 1).DayBeginTick);
					data.PkStartWaitSec = (long)(end - nowTime).TotalSeconds;
				}
				else if (realDay >= 1 && realDay <= 7)
				{
					ZhengBaMatchConfig todayMatchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == realDay);
					if (nowTime.TimeOfDay.Ticks <= todayMatchConfig.DayBeginTick)
					{
						data.PkStartWaitSec = (todayMatchConfig.DayBeginTick - nowTime.TimeOfDay.Ticks) / 10000000L;
					}
					else if (nowTime.TimeOfDay.Ticks >= todayMatchConfig.DayEndTick || (nowTime.TimeOfDay.Ticks - todayMatchConfig.DayBeginTick > 600000000L && !todayIsPking))
					{
						bool bCrossMonth = realDay == 7 || nowTime.AddDays(1.0).Month != nowTime.Month;
						DateTime nextDay = bCrossMonth ? new DateTime(nowTime.AddMonths(1).Year, nowTime.AddMonths(1).Month, ZhengBaConsts.StartMonthDay, 0, 0, 0) : new DateTime(nowTime.Year, nowTime.Month, nowTime.Day + 1, 0, 0, 0);
						long dayBeginTick;
						if (!bCrossMonth)
						{
							dayBeginTick = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == realDay + 1).DayBeginTick;
						}
						else
						{
							dayBeginTick = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == 1).DayBeginTick;
						}
						nextDay = nextDay.AddTicks(dayBeginTick);
						data.PkStartWaitSec = (long)(nextDay - nowTime).TotalSeconds;
					}
					else
					{
						int loopUseSec = todayMatchConfig.WaitSeconds + todayMatchConfig.FightSeconds + todayMatchConfig.ClearSeconds + todayMatchConfig.IntervalSeconds;
						long todayContinueSec = (nowTime.TimeOfDay.Ticks - todayMatchConfig.DayBeginTick) / 10000000L;
						long loopCurSec = todayContinueSec % (long)loopUseSec;
						if (loopCurSec < (long)(todayMatchConfig.WaitSeconds + todayMatchConfig.FightSeconds + todayMatchConfig.ClearSeconds))
						{
							data.LoopEndWaitSec = (long)(todayMatchConfig.WaitSeconds + todayMatchConfig.FightSeconds + todayMatchConfig.ClearSeconds) - loopCurSec;
						}
						else
						{
							data.NextLoopWaitSec = (long)loopUseSec - loopCurSec;
						}
					}
				}
				else
				{
					DateTime nextMonth = nowTime.AddMonths(1);
					nextMonth = new DateTime(nextMonth.Year, nextMonth.Month, ZhengBaConsts.StartMonthDay, 0, 0, 0);
					nextMonth = nextMonth.AddTicks(this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == 1).DayBeginTick);
					data.PkStartWaitSec = (long)(nextMonth - nowTime).TotalSeconds;
				}
				data.PkStartWaitSec = Math.Max(data.PkStartWaitSec, 0L);
				data.NextLoopWaitSec = Math.Max(data.NextLoopWaitSec, 0L);
				data.LoopEndWaitSec = Math.Max(data.LoopEndWaitSec, 0L);
				client.sendCmd<ZhengBaMiniStateData>(nID, data, false);
				result = true;
			}
			return result;
		}

		// Token: 0x0600131F RID: 4895 RVA: 0x0012D61C File Offset: 0x0012B81C
		private bool HandleGetMainInfo(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!this.IsGongNengOpened())
			{
				result = true;
			}
			else
			{
				DateTime now = TimeUtil.NowDateTime();
				ZhengBaMainInfoData mainInfo = new ZhengBaMainInfoData();
				lock (this.Mutex)
				{
					mainInfo.RealActDay = this.SyncData.RealActDay;
					mainInfo.RankResultOfDay = this.SyncData.RankResultOfDay;
					mainInfo.Top16List = this.Top16RoleList;
					mainInfo.MaxOpposeGroup = this.MaxOpposeGroup;
					mainInfo.MaxSupportGroup = this.MaxSupportGroup;
					mainInfo.CanGetAwardId = 0;
					bool bInAwardTime = false;
					bool flag2;
					if (this.SyncData.RealActDay >= 7)
					{
						flag2 = !this.Top16RoleList.Exists((TianTiPaiHangRoleData _r) => _r.ZhengBaGrade == 1);
					}
					else
					{
						flag2 = true;
					}
					if (!flag2)
					{
						bInAwardTime = true;
					}
					else if (now.AddDays(1.0).Month != now.Month)
					{
						ZhengBaMatchConfig _config = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == this.SyncData.RealActDay);
						if (_config != null && now.TimeOfDay.Ticks > _config.DayEndTick + (long)((ulong)-1294967296) && !this.SyncData.TodayIsPking)
						{
							bInAwardTime = true;
						}
					}
					if (bInAwardTime)
					{
						TianTiPaiHangRoleData roleData = null;
						if (this.RoleDataDict.TryGetValue(client.ClientData.RoleID, out roleData))
						{
							int awardFlag = Global.GetRoleParamsInt32FromDB(client, "ZhengBaAwardFlag");
							if (awardFlag <= 0 || awardFlag / 100 != this.SyncData.Month)
							{
								int day = ZhengBaUtils.WhichDayResultByGrade((EZhengBaGrade)roleData.ZhengBaGrade);
								ZhengBaMatchAward award = this._MatchAwardList.Find((ZhengBaMatchAward _m) => _m.FinalPassDay == day);
								if (award != null)
								{
									if (award.GoodsList.Count > 0)
									{
										if (Global.CanAddGoodsDataList(client, award.GoodsList))
										{
											foreach (GoodsData gd in award.GoodsList)
											{
												Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, gd.GoodsID, gd.GCount, gd.Quality, gd.Props, gd.Forge_level, gd.Binding, 0, gd.Jewellist, true, 1, "众神争霸", false, gd.Endtime, gd.AddPropIndex, gd.BornIndex, gd.Lucky, gd.Strong, gd.ExcellenceInfo, gd.AppendPropLev, gd.ChangeLifeLevForEquip, true, null, null, "1900-01-01 12:00:00", 0, true);
											}
										}
										else
										{
											Global.UseMailGivePlayerAward3(client.ClientData.RoleID, award.GoodsList, GLang.GetLang(539, new object[0]), string.Format(GLang.GetLang(540, new object[0]), award.Name), 0, 0, 0);
										}
									}
									Global.SaveRoleParamsInt32ValueToDB(client, "ZhengBaAwardFlag", this.SyncData.Month * 100 + award.AwardId, true);
									mainInfo.CanGetAwardId = award.AwardId;
								}
							}
						}
					}
					Global.SaveRoleParamsInt32ValueToDB(client, "ZhengBaJoinIconFlag", ZhengBaUtils.MakeMonth(now), true);
					this.CheckTipsIconState(client);
				}
				client.sendCmd<ZhengBaMainInfoData>(nID, mainInfo, false);
				result = true;
			}
			return result;
		}

		// Token: 0x06001320 RID: 4896 RVA: 0x0012DA34 File Offset: 0x0012BC34
		private bool HandleGetAllPkLog(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!this.IsGongNengOpened())
			{
				result = true;
			}
			else
			{
				List<ZhengBaPkLogData> retList = new List<ZhengBaPkLogData>();
				lock (this.Mutex)
				{
					retList.AddRange(this.PkLogQ);
				}
				retList.Reverse();
				client.sendCmd<List<ZhengBaPkLogData>>(nID, retList, false);
				result = true;
			}
			return result;
		}

		// Token: 0x06001321 RID: 4897 RVA: 0x0012DAB4 File Offset: 0x0012BCB4
		private bool HandleGetAllPkState(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!this.IsGongNengOpened())
			{
				result = true;
			}
			else
			{
				List<TianTiPaiHangRoleData> retList = new List<TianTiPaiHangRoleData>();
				lock (this.Mutex)
				{
					retList.AddRange(this.RoleDataList);
				}
				client.sendCmd<List<TianTiPaiHangRoleData>>(nID, retList, false);
				result = true;
			}
			return result;
		}

		// Token: 0x06001322 RID: 4898 RVA: 0x0012DB2C File Offset: 0x0012BD2C
		private bool HandleGet16PkState(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!this.IsGongNengOpened())
			{
				result = true;
			}
			else
			{
				int unionGroup = Convert.ToInt32(cmdParams[1]);
				client.sendCmd<ZhengBaUnionGroupData>(nID, this.Get16PkState(client, unionGroup), false);
				result = true;
			}
			return result;
		}

		// Token: 0x06001323 RID: 4899 RVA: 0x0012DC60 File Offset: 0x0012BE60
		private ZhengBaUnionGroupData Get16PkState(GameClient client, int unionGroup)
		{
			int group1 = 0;
			int group2 = 0;
			ZhengBaUtils.SplitUnionGroup(unionGroup, out group1, out group2);
			List<ZhengBaSupportAnalysisData> supportDatas = null;
			List<ZhengBaSupportLogData> supportLogs = null;
			lock (this.Mutex)
			{
				supportDatas = this.SupportDatas;
				Queue<ZhengBaSupportLogData> supportLogQ = null;
				if (this.SupportLogs.TryGetValue(unionGroup, out supportLogQ))
				{
					supportLogs = new List<ZhengBaSupportLogData>(supportLogQ);
					supportLogs.Reverse();
				}
			}
			ZhengBaUnionGroupData result = new ZhengBaUnionGroupData();
			result.UnionGroup = unionGroup;
			result.SupportLogs = supportLogs;
			result.SupportDatas = new List<ZhengBaSupportAnalysisData>();
			ZhengBaSupportAnalysisData data = supportDatas.Find((ZhengBaSupportAnalysisData _s) => _s.UnionGroup == unionGroup && _s.Group == group1);
			ZhengBaSupportAnalysisData data2 = supportDatas.Find((ZhengBaSupportAnalysisData _s) => _s.UnionGroup == unionGroup && _s.Group == group2);
			if (data != null)
			{
				result.SupportDatas.Add(data);
			}
			if (data2 != null)
			{
				result.SupportDatas.Add(data2);
			}
			result.SupportFlags = new List<ZhengBaSupportFlagData>();
			List<ZhengBaSupportFlagData> mySupports = client.ClientData.ZhengBaSupportFlags;
			ZhengBaSupportFlagData flag = mySupports.Find((ZhengBaSupportFlagData _s) => _s.UnionGroup == unionGroup && _s.Group == group1);
			ZhengBaSupportFlagData flag2 = mySupports.Find((ZhengBaSupportFlagData _s) => _s.UnionGroup == unionGroup && _s.Group == group2);
			if (flag != null)
			{
				result.SupportFlags.Add(flag);
			}
			if (flag2 != null)
			{
				result.SupportFlags.Add(flag2);
			}
			int supportDay = 0;
			for (int day = 7; day >= 1; day--)
			{
				if (ZhengBaUtils.IsValidPkGroup(group1, group2, day))
				{
					supportDay = day - 1;
					break;
				}
			}
			ZhengBaSupportConfig supportConfig = this._Config.SupportConfigList.Find((ZhengBaSupportConfig _s) => _s.RankOfDay == supportDay);
			if (supportConfig != null)
			{
				result.WinZhengBaPoint = supportConfig.WinPoint;
			}
			return result;
		}

		// Token: 0x06001324 RID: 4900 RVA: 0x0012E054 File Offset: 0x0012C254
		private bool HandleSupport(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!this.IsGongNengOpened())
			{
				result = true;
			}
			else
			{
				int unionGroup = Convert.ToInt32(cmdParams[1]);
				int group = Convert.ToInt32(cmdParams[2]);
				int supportType = Convert.ToInt32(cmdParams[3]);
				if (supportType != 1 && supportType != 2 && supportType != 3)
				{
					result = true;
				}
				else
				{
					int group1 = 0;
					int group2 = 0;
					ZhengBaUtils.SplitUnionGroup(unionGroup, out group1, out group2);
					if (group < 1 || group > 16)
					{
						result = true;
					}
					else if (group1 < 1 || group1 > 16)
					{
						result = true;
					}
					else if (group2 < 1 || group2 > 16)
					{
						result = true;
					}
					else if (group1 >= group2)
					{
						result = true;
					}
					else if (group != group1 && group != group2)
					{
						result = true;
					}
					else if (this.SyncData.RealActDay < 3 || this.SyncData.RealActDay > 7)
					{
						result = true;
					}
					else
					{
						DateTime now = TimeUtil.NowDateTime();
						ZhengBaSupportConfig supportConfig = this._Config.SupportConfigList.Find((ZhengBaSupportConfig _s) => _s.TimeList.Exists((ZhengBaSupportConfig.TimeConfig _t) => _t.RealDay == this.SyncData.RealActDay && _t.DayBeginTicks < now.TimeOfDay.Ticks && _t.DayEndTicks > now.TimeOfDay.Ticks));
						if (supportConfig == null)
						{
							client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
							{
								-2001,
								unionGroup,
								group,
								supportType
							}), false);
							result = true;
						}
						else if (Global.GetUnionLevel(client, false) < Global.GetUnionLevel(supportConfig.MinChangeLife, supportConfig.MinLevel, false))
						{
							client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
							{
								-19,
								unionGroup,
								group,
								supportType
							}), false);
							result = true;
						}
						else if (!ZhengBaUtils.IsValidPkGroup(group1, group2, supportConfig.RankOfDay + 1))
						{
							result = true;
						}
						else
						{
							lock (this.Mutex)
							{
								bool flag2;
								if (this.Top16RoleList.Exists((TianTiPaiHangRoleData _r) => _r.ZhengBaGroup == group1 && _r.ZhengBaState == 1))
								{
									flag2 = this.Top16RoleList.Exists((TianTiPaiHangRoleData _r) => _r.ZhengBaGroup == group2 && _r.ZhengBaState == 1);
								}
								else
								{
									flag2 = false;
								}
								if (!flag2)
								{
									client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
									{
										-12,
										unionGroup,
										group,
										supportType
									}), false);
									return true;
								}
							}
							if (supportType == 3)
							{
								int hadYaZhuCnt = client.ClientData.ZhengBaSupportFlags.Count((ZhengBaSupportFlagData _s) => _s.RankOfDay == supportConfig.RankOfDay && _s.IsYaZhu);
								if (hadYaZhuCnt >= supportConfig.MaxTimes)
								{
									client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
									{
										-16,
										unionGroup,
										group,
										supportType
									}), false);
									return true;
								}
							}
							ZhengBaSupportFlagData flagData = client.ClientData.ZhengBaSupportFlags.Find((ZhengBaSupportFlagData _f) => _f.UnionGroup == unionGroup && _f.Group == group);
							if (flagData != null)
							{
								if ((flagData.IsOppose || flagData.IsSupport) && (supportType == 2 || supportType == 1))
								{
									client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
									{
										-5,
										unionGroup,
										group,
										supportType
									}), false);
									return true;
								}
								if (supportType == 3)
								{
									bool flag3;
									if (!flagData.IsYaZhu)
									{
										flag3 = (client.ClientData.ZhengBaSupportFlags.Count((ZhengBaSupportFlagData _f) => _f.UnionGroup == unionGroup && _f.IsYaZhu) < 1);
									}
									else
									{
										flag3 = false;
									}
									if (!flag3)
									{
										client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
										{
											-5,
											unionGroup,
											group,
											supportType
										}), false);
										return true;
									}
								}
							}
							if (supportType == 3 && !Global.SubBindTongQianAndTongQian(client, supportConfig.CostJinBi, "众神争霸押注"))
							{
								client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
								{
									-9,
									unionGroup,
									group,
									supportType
								}), false);
								result = true;
							}
							else
							{
								ZhengBaSupportLogData log = new ZhengBaSupportLogData();
								log.FromRoleId = client.ClientData.RoleID;
								log.FromZoneId = client.ClientData.ZoneID;
								log.FromRolename = client.ClientData.RoleName;
								log.SupportType = supportType;
								log.ToUnionGroup = unionGroup;
								log.ToGroup = group;
								log.Time = now;
								log.FromServerId = GameCoreInterface.getinstance().GetLocalServerId();
								log.Month = ZhengBaUtils.MakeMonth(now);
								log.RankOfDay = supportConfig.RankOfDay;
								int ec = TianTiClient.getInstance().ZhengBaSupport(log);
								if (ec < 0)
								{
									client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
									{
										ec,
										unionGroup,
										group,
										supportType
									}), false);
									result = true;
								}
								else if (!Global.sendToDB<bool, ZhengBaSupportLogData>(14011, log, 0))
								{
									client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
									{
										-15,
										unionGroup,
										group,
										supportType
									}), false);
									result = true;
								}
								else
								{
									if (flagData == null)
									{
										flagData = new ZhengBaSupportFlagData();
										flagData.UnionGroup = unionGroup;
										flagData.Group = group;
										flagData.RankOfDay = supportConfig.RankOfDay;
										client.ClientData.ZhengBaSupportFlags.Add(flagData);
									}
									if (supportType == 1)
									{
										flagData.IsSupport = true;
									}
									else if (supportType == 2)
									{
										flagData.IsOppose = true;
									}
									else if (supportType == 3)
									{
										flagData.IsYaZhu = true;
									}
									lock (this.Mutex)
									{
										Queue<ZhengBaSupportLogData> supportLogQ = null;
										if (!this.SupportLogs.TryGetValue(unionGroup, out supportLogQ))
										{
											supportLogQ = (this.SupportLogs[unionGroup] = new Queue<ZhengBaSupportLogData>());
										}
										supportLogQ.Enqueue(log);
										while (supportLogQ.Count > 30)
										{
											supportLogQ.Dequeue();
										}
										if (supportType == 3)
										{
											ZhengBaWaitYaZhuAwardData waitYaZhuAward = new ZhengBaWaitYaZhuAwardData();
											waitYaZhuAward.Month = log.Month;
											waitYaZhuAward.FromRoleId = client.ClientData.RoleID;
											waitYaZhuAward.UnionGroup = unionGroup;
											waitYaZhuAward.Group = group;
											waitYaZhuAward.RankOfDay = log.RankOfDay;
											this.WaitAwardOfYaZhuList.Add(waitYaZhuAward);
										}
										ZhengBaSupportAnalysisData analysisData = this.SupportDatas.Find((ZhengBaSupportAnalysisData _s) => _s.UnionGroup == unionGroup && _s.Group == group);
										if (analysisData == null)
										{
											analysisData = new ZhengBaSupportAnalysisData
											{
												UnionGroup = unionGroup,
												Group = group
											};
											this.SupportDatas.Add(analysisData);
										}
										if (supportType == 1)
										{
											analysisData.TotalSupport++;
										}
										else if (supportType == 2)
										{
											analysisData.TotalOppose++;
										}
										else if (supportType == 3)
										{
											analysisData.TotalYaZhu++;
										}
									}
									client.sendCmd<ZhengBaUnionGroupData>(1353, this.Get16PkState(client, unionGroup), false);
									result = true;
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06001325 RID: 4901 RVA: 0x0012EADC File Offset: 0x0012CCDC
		private bool HandleEnter(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!this.IsGongNengOpened())
			{
				result = true;
			}
			else
			{
				int gameId = Convert.ToInt32(cmdParams[0]);
				int enter = Convert.ToInt32(cmdParams[1]);
				if ((long)gameId != client.ClientSocket.ClientKuaFuServerLoginData.GameId || client.ClientSocket.ClientKuaFuServerLoginData.GameType != 12)
				{
					client.sendCmd(nID, string.Format("{0}", -2001), false);
					result = true;
				}
				else if (enter != 1 && enter != 2)
				{
					client.sendCmd(nID, string.Format("{0}", -18), false);
					result = true;
				}
				else
				{
					int ec = TianTiClient.getInstance().ZhengBaRequestEnter(client.ClientData.RoleID, gameId, (EZhengBaEnterType)enter);
					if (ec < 0)
					{
						client.sendCmd(nID, string.Format("{0}", ec), false);
						result = true;
					}
					else
					{
						if (enter == 1)
						{
							GlobalNew.RecordSwitchKuaFuServerLog(client);
							client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
						}
						else if (enter == 2)
						{
							client.ClientSocket.ClientKuaFuServerLoginData.RoleId = 0;
							client.ClientSocket.ClientKuaFuServerLoginData.GameId = 0L;
							client.ClientSocket.ClientKuaFuServerLoginData.GameType = 0;
							client.ClientSocket.ClientKuaFuServerLoginData.ServerId = 0;
						}
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06001326 RID: 4902 RVA: 0x0012EC68 File Offset: 0x0012CE68
		public bool ProcessGetAdmireDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = Convert.ToInt32(cmdParams[0]);
				int nowDayId = TimeUtil.GetOffsetDayNow();
				long zhengBaKingMoBai = Global.GetRoleParamsInt64FromDB(client, "10151");
				int admireDayId = (int)(zhengBaKingMoBai % 10000L);
				int hadAdmireCount = (int)(zhengBaKingMoBai / 10000L);
				if (admireDayId != nowDayId)
				{
					hadAdmireCount = 0;
				}
				client.sendCmd<LangHunLingYuKingShowData>(nID, new LangHunLingYuKingShowData
				{
					AdmireCount = hadAdmireCount,
					RoleData4Selector = this.ZhengBaKingData
				}, false);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		// Token: 0x06001327 RID: 4903 RVA: 0x0012ED18 File Offset: 0x0012CF18
		public bool HandleAdmireStatue(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int result = 0;
			int roleId = Global.SafeConvertToInt32(cmdParams[0]);
			int admireType = Global.SafeConvertToInt32(cmdParams[1]);
			int nowDayId = TimeUtil.GetOffsetDayNow();
			MoBaiData MoBaiConfig = null;
			if (!Data.MoBaiDataInfoList.TryGetValue(3, out MoBaiConfig))
			{
				result = -3;
			}
			else if (client.ClientData.ChangeLifeCount < MoBaiConfig.MinZhuanSheng || (client.ClientData.ChangeLifeCount == MoBaiConfig.MinZhuanSheng && client.ClientData.Level < MoBaiConfig.MinLevel))
			{
				result = -19;
			}
			else
			{
				int maxAdmireNum = MoBaiConfig.AdrationMaxLimit;
				long zhengBaKingMoBai = Global.GetRoleParamsInt64FromDB(client, "10151");
				int admireDayId = (int)(zhengBaKingMoBai % 10000L);
				int hadAdmireCount = (int)(zhengBaKingMoBai / 10000L);
				if (admireDayId != nowDayId)
				{
					hadAdmireCount = 0;
				}
				int nVIPLev = client.ClientData.VipLevel;
				int[] nArrayVIPAdded = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPMoBaiNum", ',');
				if (nVIPLev > VIPEumValue.VIPENUMVALUE_MAXLEVEL || nArrayVIPAdded.Length < 1)
				{
					result = -3;
				}
				else
				{
					maxAdmireNum += nArrayVIPAdded[nVIPLev];
					double awardmuti = 0.0;
					JieRiMultAwardActivity activity = HuodongCachingMgr.GetJieRiMultAwardActivity();
					if (null != activity)
					{
						JieRiMultConfig config = activity.GetConfig(12);
						if (null != config)
						{
							awardmuti += config.GetMult();
						}
					}
					SpecPriorityActivity spAct = HuodongCachingMgr.GetSpecPriorityActivity();
					if (null != spAct)
					{
						awardmuti += spAct.GetMult(SpecPActivityBuffType.SPABT_Admire);
					}
					awardmuti = Math.Max(1.0, awardmuti);
					maxAdmireNum = (int)((double)maxAdmireNum * awardmuti);
					if (this.ZhengBaKingRoleId == roleId)
					{
						maxAdmireNum++;
					}
					if (hadAdmireCount >= maxAdmireNum)
					{
						result = -16;
					}
					else if (admireType == 1 && Global.GetTotalBindTongQianAndTongQianVal(client) < MoBaiConfig.NeedJinBi)
					{
						result = -9;
					}
					else if (admireType == 2 && client.ClientData.UserMoney < MoBaiConfig.NeedZuanShi)
					{
						result = -10;
					}
					else
					{
						double nRate = (client.ClientData.ChangeLifeCount == 0) ? 1.0 : Data.ChangeLifeEverydayExpRate[client.ClientData.ChangeLifeCount];
						if (admireType == 1)
						{
							Global.SubBindTongQianAndTongQian(client, MoBaiConfig.NeedJinBi, "膜拜众神之王");
							long nExp = (long)(nRate * (double)MoBaiConfig.JinBiExpAward);
							if (nExp > 0L)
							{
								GameManager.ClientMgr.ProcessRoleExperience(client, nExp, true, true, false, "none");
							}
							if (MoBaiConfig.JinBiZhanGongAward > 0)
							{
								GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref MoBaiConfig.JinBiZhanGongAward, AddBangGongTypes.CoupleWishMoBai, 0);
							}
							if (MoBaiConfig.LingJingAwardByJinBi > 0)
							{
								GameManager.ClientMgr.ModifyMUMoHeValue(client, MoBaiConfig.LingJingAwardByJinBi, "膜拜众神之王", true, true, false);
							}
						}
						if (admireType == 2)
						{
							GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, MoBaiConfig.NeedZuanShi, "膜拜众神之王", true, true, false, DaiBiSySType.None);
							int nExp2 = (int)(nRate * (double)MoBaiConfig.ZuanShiExpAward);
							if (nExp2 > 0)
							{
								GameManager.ClientMgr.ProcessRoleExperience(client, (long)nExp2, true, true, false, "none");
							}
							if (MoBaiConfig.ZuanShiZhanGongAward > 0)
							{
								GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref MoBaiConfig.ZuanShiZhanGongAward, AddBangGongTypes.CoupleWishMoBai, 0);
							}
							if (MoBaiConfig.LingJingAwardByZuanShi > 0)
							{
								GameManager.ClientMgr.ModifyMUMoHeValue(client, MoBaiConfig.LingJingAwardByZuanShi, "膜拜众神之王", true, true, false);
							}
						}
						hadAdmireCount++;
						zhengBaKingMoBai = (long)(hadAdmireCount * 10000 + nowDayId);
						Global.SaveRoleParamsInt64ValueToDB(client, "10151", zhengBaKingMoBai, true);
					}
				}
			}
			client.sendCmd<int>(nID, result, false);
			return true;
		}

		// Token: 0x06001328 RID: 4904 RVA: 0x0012F344 File Offset: 0x0012D544
		public void SyncCenterData(object sender, EventArgs e)
		{
			ZhengBaSyncData syncData = TianTiClient.getInstance().GetZhengBaRankData(this.SyncData);
			if (syncData != null)
			{
				if (syncData.LastKingData != null)
				{
					TianTiPaiHangRoleData KingRole = null;
					if (syncData.LastKingData != null && syncData.LastKingData.TianTiPaiHangRoleData != null)
					{
						ZhengBaRoleInfoData info = syncData.LastKingData;
						KingRole = DataHelper.BytesToObject<TianTiPaiHangRoleData>(info.TianTiPaiHangRoleData, 0, info.TianTiPaiHangRoleData.Length);
					}
					if (KingRole != null)
					{
						this.ZhengBaKingRoleId = KingRole.RoleId;
						this.ZhengBaKingData = KingRole.RoleData4Selector.Clone();
					}
					else
					{
						this.ZhengBaKingRoleId = 0;
						this.ZhengBaKingData = null;
					}
					NPC manNpc = NPCGeneralManager.FindNPC(GameManager.MainMapCode, FakeRoleNpcId.ZhengBaKing);
					if (null != manNpc)
					{
						if (this.ZhengBaKingData == null)
						{
							manNpc.ShowNpc = true;
							GameManager.ClientMgr.NotifyMySelfNewNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, manNpc);
							FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.ZhengBaKing, true);
						}
						else
						{
							manNpc.ShowNpc = false;
							GameManager.ClientMgr.NotifyMySelfDelNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, manNpc);
							FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.ZhengBaKing, true);
							FakeRoleManager.ProcessNewFakeRole(this.ZhengBaKingData, manNpc.MapCode, FakeRoleTypes.ZhengBaKing, (int)manNpc.CurrentDir, (int)manNpc.CurrentPos.X, (int)manNpc.CurrentPos.Y, FakeRoleNpcId.ZhengBaKing);
						}
					}
				}
				if (syncData.RoleList != null)
				{
					List<TianTiPaiHangRoleData> roleList = new List<TianTiPaiHangRoleData>();
					Dictionary<int, TianTiPaiHangRoleData> roleDict = new Dictionary<int, TianTiPaiHangRoleData>();
					Dictionary<int, PlayerJingJiData> mirrorData = new Dictionary<int, PlayerJingJiData>();
					foreach (ZhengBaRoleInfoData info in syncData.RoleList)
					{
						if (null != info.TianTiPaiHangRoleData)
						{
							TianTiPaiHangRoleData role = DataHelper.BytesToObject<TianTiPaiHangRoleData>(info.TianTiPaiHangRoleData, 0, info.TianTiPaiHangRoleData.Length);
							if (role != null)
							{
								role.RoleId = info.RoleId;
								role.DuanWeiRank = info.DuanWeiRank;
								role.ZhengBaGrade = info.Grade;
								role.ZhengBaGroup = info.Group;
								role.ZhengBaState = info.State;
								roleList.Add(role);
								roleDict.Add(role.RoleId, role);
								if (null != info.PlayerJingJiMirrorData)
								{
									PlayerJingJiData jingJiData = DataHelper.BytesToObject<PlayerJingJiData>(info.PlayerJingJiMirrorData, 0, info.PlayerJingJiMirrorData.Length);
									if (jingJiData != null)
									{
										mirrorData[role.RoleId] = jingJiData;
									}
								}
							}
						}
					}
					roleList.Sort(delegate(TianTiPaiHangRoleData _l, TianTiPaiHangRoleData _r)
					{
						int result;
						if (_l.ZhengBaGrade < _r.ZhengBaGrade)
						{
							result = -1;
						}
						else if (_l.ZhengBaGrade > _r.ZhengBaGrade)
						{
							result = 1;
						}
						else if (_l.ZhengBaState < _r.ZhengBaState)
						{
							result = -1;
						}
						else if (_l.ZhengBaState > _r.ZhengBaState)
						{
							result = 1;
						}
						else
						{
							result = _l.DuanWeiRank - _r.DuanWeiRank;
						}
						return result;
					});
					List<TianTiPaiHangRoleData> top16List = roleList.FindAll((TianTiPaiHangRoleData _r) => _r.ZhengBaGrade <= 16);
					TianTiPaiHangRoleData KingRole = top16List.Find((TianTiPaiHangRoleData _r) => _r.ZhengBaGrade == 1);
					if (KingRole != null)
					{
						this.SetZhongShengRole(KingRole.RoleId);
					}
					lock (this.Mutex)
					{
						foreach (TianTiPaiHangRoleData role in roleList)
						{
							if (role.RoleData4Selector != null)
							{
								role.RoleData4Selector.GoodsDataList = null;
								role.RoleData4Selector.MyWingData = null;
							}
						}
						this.RoleDataList = roleList;
						this.RoleDataDict = roleDict;
						this.Top16RoleList = top16List;
						this.MirrorDatas = mirrorData;
					}
				}
				if (syncData.SupportList != null)
				{
					List<ZhengBaSupportAnalysisData> supportDatas = syncData.SupportList;
					lock (this.Mutex)
					{
						this.SupportDatas = supportDatas;
						List<KeyValuePair<int, int>> _supportList = new List<KeyValuePair<int, int>>();
						List<KeyValuePair<int, int>> _opposeList = new List<KeyValuePair<int, int>>();
						using (List<ZhengBaSupportAnalysisData>.Enumerator enumerator3 = this.SupportDatas.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								ZhengBaSupportAnalysisData data = enumerator3.Current;
								if (data.RankOfDay == syncData.RankResultOfDay)
								{
									TianTiPaiHangRoleData roleData;
									if ((roleData = this.Top16RoleList.Find((TianTiPaiHangRoleData _r) => _r.ZhengBaGroup == data.Group)) != null && roleData.ZhengBaState == 1)
									{
										_supportList.RemoveAll((KeyValuePair<int, int> _kvp) => _kvp.Key == data.Group);
										_opposeList.RemoveAll((KeyValuePair<int, int> _kvp) => _kvp.Key == data.Group);
										_supportList.Add(new KeyValuePair<int, int>(data.Group, data.TotalSupport));
										_opposeList.Add(new KeyValuePair<int, int>(data.Group, data.TotalOppose));
									}
								}
							}
						}
						_supportList.RemoveAll((KeyValuePair<int, int> _kvp) => _kvp.Value <= 0);
						_opposeList.RemoveAll((KeyValuePair<int, int> _kvp) => _kvp.Value <= 0);
						_supportList.Sort((KeyValuePair<int, int> _l, KeyValuePair<int, int> _r) => _r.Value - _l.Value);
						_opposeList.Sort((KeyValuePair<int, int> _l, KeyValuePair<int, int> _r) => _r.Value - _l.Value);
						int _maxSupportGroup = 0;
						int _maxOpposeGroup = 0;
						if (_supportList.Count > 0)
						{
							_maxSupportGroup = _supportList[0].Key;
						}
						if (_opposeList.Count > 0)
						{
							_maxOpposeGroup = _opposeList[0].Key;
							if (_maxOpposeGroup == _maxSupportGroup)
							{
								_maxOpposeGroup = 0;
								if (_opposeList.Count > 1)
								{
									_maxOpposeGroup = _opposeList[1].Key;
								}
							}
						}
						this.MaxSupportGroup = _maxSupportGroup;
						this.MaxOpposeGroup = _maxOpposeGroup;
					}
				}
				lock (this.Mutex)
				{
					if (this.SyncData.Month != syncData.Month)
					{
						this.SupportLogs.Clear();
						this.PkLogQ.Clear();
						this.WaitAwardOfYaZhuList.Clear();
					}
					syncData.RoleList = null;
					syncData.SupportList = null;
					this.SyncData = syncData;
					this.DiffKfCenter = syncData.CenterTime - TimeUtil.NowDateTime();
				}
			}
		}

		// Token: 0x06001329 RID: 4905 RVA: 0x0012FB60 File Offset: 0x0012DD60
		public void SetZhongShengRole(int roleid)
		{
			int oldRoleId = GameManager.GameConfigMgr.GetGameConfigItemInt("ZhongShenZhiShenRole", 0);
			if (oldRoleId != roleid)
			{
				Global.UpdateDBGameConfigg("ZhongShenZhiShenRole", roleid.ToString());
				GameManager.GameConfigMgr.SetGameConfigItem("ZhongShenZhiShenRole", roleid.ToString());
				GameClient oldClient = GameManager.ClientMgr.FindClient(oldRoleId);
				if (oldClient != null)
				{
					this.CheckZhongShenChengHao(oldClient);
				}
				GameClient newClient = GameManager.ClientMgr.FindClient(roleid);
				if (newClient != null)
				{
					this.CheckZhongShenChengHao(newClient);
				}
			}
		}

		// Token: 0x0600132A RID: 4906 RVA: 0x0012FBEC File Offset: 0x0012DDEC
		private void CheckZhongShenChengHao(GameClient client)
		{
			if (client != null)
			{
				int kingRole = GameManager.GameConfigMgr.GetGameConfigItemInt("ZhongShenZhiShenRole", 0);
				BufferData bufferData = Global.GetBufferDataByID(client, 111);
				if (client.ClientData.RoleID != kingRole)
				{
					if (bufferData != null && bufferData.BufferVal != 0L)
					{
						double[] array = new double[1];
						double[] bufferParams = array;
						Global.UpdateBufferData(client, BufferItemTypes.ZhongShenZhiShen_ChengHao, bufferParams, 0, true);
					}
				}
				else if (bufferData == null || bufferData.BufferVal == 0L)
				{
					double[] bufferParams = new double[]
					{
						1.0
					};
					Global.UpdateBufferData(client, BufferItemTypes.ZhongShenZhiShen_ChengHao, bufferParams, 0, true);
				}
			}
		}

		// Token: 0x0600132B RID: 4907 RVA: 0x0012FD14 File Offset: 0x0012DF14
		private void CheckYaZhuAward(object sender, EventArgs e)
		{
			lock (this.Mutex)
			{
				if (this.WaitAwardOfYaZhuList.Count > 0)
				{
					using (List<ZhengBaWaitYaZhuAwardData>.Enumerator enumerator = this.WaitAwardOfYaZhuList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ZhengBaWaitYaZhuAwardData waitAward = enumerator.Current;
							TianTiPaiHangRoleData roleData = this.Top16RoleList.Find((TianTiPaiHangRoleData _r) => _r.ZhengBaGroup == waitAward.Group);
							if (roleData != null)
							{
								ZhengBaSupportConfig supportConfig = this._Config.SupportConfigList.Find((ZhengBaSupportConfig _m) => _m.RankOfDay == waitAward.RankOfDay);
								if (supportConfig != null)
								{
									string mailMsg = string.Empty;
									List<GoodsData> awardGoodsList;
									if (roleData.ZhengBaGrade <= (int)ZhengBaUtils.GetDayUpGrade(waitAward.RankOfDay + 1))
									{
										mailMsg = ((roleData.ZhengBaGrade == 1) ? GLang.GetLang(541, new object[0]) : GLang.GetLang(542, new object[0]));
										mailMsg = string.Format(mailMsg, roleData.RoleName, supportConfig.WinPoint);
										awardGoodsList = (supportConfig.WinAwardTag as List<GoodsData>);
									}
									else
									{
										if (roleData.ZhengBaState != 2)
										{
											continue;
										}
										mailMsg = GLang.GetLang(543, new object[0]);
										mailMsg = string.Format(mailMsg, roleData.RoleName, supportConfig.FailPoint);
										awardGoodsList = (supportConfig.FailAwardTag as List<GoodsData>);
									}
									if (Global.UseMailGivePlayerAward3(waitAward.FromRoleId, awardGoodsList, GLang.GetLang(539, new object[0]), mailMsg, 0, 0, 0))
									{
										Global.sendToDB<bool, string>(14017, string.Format("{0}:{1}:{2}:{3}", new object[]
										{
											waitAward.Month,
											waitAward.FromRoleId,
											waitAward.UnionGroup,
											waitAward.Group
										}), 0);
										waitAward.FromRoleId = -1;
									}
								}
							}
						}
					}
					this.WaitAwardOfYaZhuList.RemoveAll((ZhengBaWaitYaZhuAwardData _w) => _w.FromRoleId == -1);
				}
			}
		}

		// Token: 0x0600132C RID: 4908 RVA: 0x0013000C File Offset: 0x0012E20C
		public void CheckTipsIconState(GameClient client)
		{
			if (client != null)
			{
				DateTime now = TimeUtil.NowDateTime();
				int nowMonth = ZhengBaUtils.MakeMonth(TimeUtil.NowDateTime());
				int oldMonth = Global.GetRoleParamsInt32FromDB(client, "ZhengBaJoinIconFlag");
				bool bIconActive = false;
				lock (this.Mutex)
				{
					if (this.SyncData.Month == nowMonth && oldMonth != nowMonth && this.SyncData.IsThisMonthInActivity && this.IsGongNengOpened() && this.RoleDataDict.ContainsKey(client.ClientData.RoleID))
					{
						bIconActive = true;
					}
				}
				if (client._IconStateMgr.AddFlushIconState(15010, bIconActive))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
			}
		}

		// Token: 0x0600132D RID: 4909 RVA: 0x00130100 File Offset: 0x0012E300
		public void OnLogin(GameClient client)
		{
			if (client != null)
			{
				this.CheckZhongShenChengHao(client);
				this.CheckGongNengCanOpen(client);
				this.CheckTipsIconState(client);
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					DateTime now = TimeUtil.NowDateTime();
					if (now.Day > ZhengBaConsts.StartMonthDay)
					{
						int month = ZhengBaUtils.MakeMonth(now);
						List<ZhengBaSupportFlagData> mySupports = Global.sendToDB<List<ZhengBaSupportFlagData>, string>(14015, string.Format("{0}:{1}", client.ClientData.RoleID, month), client.ServerId);
						client.ClientData.ZhengBaSupportFlags.Clear();
						if (mySupports != null)
						{
							client.ClientData.ZhengBaSupportFlags.AddRange(mySupports);
						}
					}
				}
			}
		}

		// Token: 0x0600132E RID: 4910 RVA: 0x001301D0 File Offset: 0x0012E3D0
		public bool IsGongNengOpened()
		{
			return !GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System2Dot0) && GameManager.GameConfigMgr.GetGameConfigItemInt("ZhengBaOpenedFlag", 0) == 1;
		}

		// Token: 0x0600132F RID: 4911 RVA: 0x00130208 File Offset: 0x0012E408
		public void CheckGongNengCanOpen(GameClient client)
		{
			if (client != null)
			{
				if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System2Dot0))
				{
					int openFlag = 1;
					if (GameManager.GameConfigMgr.GetGameConfigItemInt("ZhengBaOpenedFlag", 0) != openFlag && TianTiManager.getInstance().IsGongNengOpened(client, false))
					{
						Global.UpdateDBGameConfigg("ZhengBaOpenedFlag", openFlag.ToString());
						GameManager.GameConfigMgr.SetGameConfigItem("ZhengBaOpenedFlag", openFlag.ToString());
						string broadcastMsg = GLang.GetLang(544, new object[0]);
						broadcastMsg = string.Format(broadcastMsg, client.ClientData.RoleName);
						Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.Bulletin, broadcastMsg, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlySysHint, 0, 0, 100, 100);
					}
				}
			}
		}

		// Token: 0x06001330 RID: 4912 RVA: 0x001302C8 File Offset: 0x0012E4C8
		public void OnNewDay(GameClient client)
		{
			if (client != null && !client.ClientSocket.IsKuaFuLogin)
			{
				if (TimeUtil.NowDateTime().Day == 1)
				{
					client.ClientData.ZhengBaSupportFlags.Clear();
				}
			}
		}

		// Token: 0x06001331 RID: 4913 RVA: 0x0013031C File Offset: 0x0012E51C
		public void processEvent(EventObjectEx eventObject)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System2Dot0))
			{
				if (eventObject.EventType == 10020)
				{
					this.HandleSupportLog((eventObject as KFZhengBaSupportEvent).Data);
				}
				else if (eventObject.EventType == 10021)
				{
					this.HandlePkLog((eventObject as KFZhengBaPkLogEvent).Log);
				}
				else if (eventObject.EventType == 10022)
				{
					this.HandleNtfEnter((eventObject as KFZhengBaNtfEnterEvent).Data);
				}
				else if (eventObject.EventType == 10023)
				{
					this.HandleMirrirFight((eventObject as KFZhengBaMirrorFightEvent).Data);
				}
				else if (eventObject.EventType == 10024)
				{
					this.HandleBulletinJoin((eventObject as KFZhengBaBulletinJoinEvent).Data);
				}
			}
			eventObject.Handled = true;
		}

		// Token: 0x06001332 RID: 4914 RVA: 0x00130470 File Offset: 0x0012E670
		private void HandleBulletinJoin(ZhengBaBulletinJoinData data)
		{
			this.SyncCenterData(null, null);
			if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System2Dot0))
			{
				if (data.NtfType == ZhengBaBulletinJoinData.ENtfType.BulletinServer)
				{
					ZhengBaMatchConfig matchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == 1);
					DateTime dtBegin = new DateTime(matchConfig.DayBeginTick);
					string broadcastMsg = GLang.GetLang(545, new object[0]);
					broadcastMsg = string.Format(broadcastMsg, new object[]
					{
						ZhengBaConsts.StartMonthDay,
						dtBegin.Hour,
						dtBegin.Minute,
						data.Args1
					});
					Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.Bulletin, broadcastMsg, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlySysHint, 0, 0, 100, 100);
				}
				else if (data.NtfType == ZhengBaBulletinJoinData.ENtfType.MailJoinRole)
				{
					ZhengBaMatchConfig matchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == 1);
					DateTime dtBegin = new DateTime(matchConfig.DayBeginTick);
					DateTime dtEnd = new DateTime(matchConfig.DayEndTick);
					string mailMsg = GLang.GetLang(546, new object[0]);
					mailMsg = string.Format(mailMsg, new object[]
					{
						ZhengBaConsts.StartMonthDay,
						dtBegin.Hour,
						dtBegin.Minute,
						dtEnd.Hour,
						dtEnd.Minute
					});
					List<int> roleIdList = new List<int>();
					lock (this.Mutex)
					{
						List<int> result = this.RoleDataDict.Keys.ToList<int>();
						if (result != null)
						{
							roleIdList.AddRange(result);
						}
					}
					roleIdList.ForEach(delegate(int _rid)
					{
						Global.UseMailGivePlayerAward3(_rid, null, GLang.GetLang(539, new object[0]), mailMsg, 0, 1, 0);
					});
				}
				else if (data.NtfType == ZhengBaBulletinJoinData.ENtfType.MailUpgradeRole)
				{
					string mailMsg2 = GLang.GetLang(547, new object[0]);
					Global.UseMailGivePlayerAward3(data.Args1, null, GLang.GetLang(539, new object[0]), mailMsg2, 0, 1, 0);
				}
				else if (data.NtfType == ZhengBaBulletinJoinData.ENtfType.DayLoopEnd)
				{
					if (data.Args1 >= 1 && data.Args1 < 7)
					{
						string mailMsg2 = string.Format(GLang.GetLang(548, new object[0]), data.Args1);
						Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.Bulletin, mailMsg2, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlySysHint, 0, 0, 100, 100);
					}
				}
			}
		}

		// Token: 0x06001333 RID: 4915 RVA: 0x001307B0 File Offset: 0x0012E9B0
		private void HandleMirrirFight(ZhengBaMirrorFightData data)
		{
			if (data.ToServerId == GameCoreInterface.getinstance().GetLocalServerId())
			{
				lock (this.Mutex)
				{
					ZhengBaMatchConfig matchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == this.SyncData.RealActDay);
					if (matchConfig != null)
					{
						PlayerJingJiData jingJiData = null;
						if (!this.MirrorDatas.TryGetValue(data.RoleId, out jingJiData))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("镜像出战，找不到镜像, server={0}, rid={1}, gameid={2}", data.ToServerId, data.RoleId, data.GameId), null, true);
						}
						else
						{
							ZhengBaManager.GameSideInfo sideInfo = null;
							if (!this.GameId2FuBenSeq.TryGetValue(data.GameId, out sideInfo))
							{
								sideInfo = new ZhengBaManager.GameSideInfo();
								sideInfo.FuBenSeq = GameCoreInterface.getinstance().GetNewFuBenSeqId();
								this.GameId2FuBenSeq[data.GameId] = sideInfo;
							}
							ZhengBaManager.ZhengBaCopyScene scene = null;
							if (!this.FuBenSeq2CopyScenes.TryGetValue(sideInfo.FuBenSeq, out scene))
							{
								scene = new ZhengBaManager.ZhengBaCopyScene();
								scene.FuBenSeq = sideInfo.FuBenSeq;
								scene.GameId = data.GameId;
								scene.MapCode = matchConfig.MapCode;
								this.FuBenSeq2CopyScenes[sideInfo.FuBenSeq] = scene;
							}
							if (scene.RoleId1 <= 0)
							{
								scene.RoleId1 = data.RoleId;
								scene.IsMirror1 = true;
								scene.JingJiData1 = jingJiData;
								scene.Robot1 = null;
							}
							else if (scene.RoleId2 <= 0)
							{
								scene.RoleId2 = data.RoleId;
								scene.IsMirror2 = true;
								scene.JingJiData2 = jingJiData;
								scene.Robot2 = null;
							}
						}
					}
				}
			}
		}

		// Token: 0x06001334 RID: 4916 RVA: 0x001309C0 File Offset: 0x0012EBC0
		private void HandleSupportLog(ZhengBaSupportLogData data)
		{
			if (Global.sendToDB<bool, ZhengBaSupportLogData>(14011, data, 0))
			{
				lock (this.Mutex)
				{
					Queue<ZhengBaSupportLogData> supportLogQ = null;
					if (!this.SupportLogs.TryGetValue(data.ToUnionGroup, out supportLogQ))
					{
						supportLogQ = (this.SupportLogs[data.ToUnionGroup] = new Queue<ZhengBaSupportLogData>());
					}
					supportLogQ.Enqueue(data);
					while (supportLogQ.Count > 30)
					{
						supportLogQ.Dequeue();
					}
				}
			}
		}

		// Token: 0x06001335 RID: 4917 RVA: 0x00130A6C File Offset: 0x0012EC6C
		private void HandlePkLog(ZhengBaPkLogData data)
		{
			if (data.PkResult != 0)
			{
				if (data.UpGrade)
				{
					if (Global.sendToDB<bool, ZhengBaPkLogData>(14012, data, 0))
					{
						lock (this.Mutex)
						{
							this.PkLogQ.Enqueue(data);
							while (this.PkLogQ.Count > 100)
							{
								this.PkLogQ.Dequeue();
							}
						}
					}
				}
			}
		}

		// Token: 0x06001336 RID: 4918 RVA: 0x00130B10 File Offset: 0x0012ED10
		private void HandleNtfEnter(ZhengBaNtfEnterData data)
		{
			GameClient client = GameManager.ClientMgr.FindClient(data.RoleId1);
			if (client != null && !client.ClientSocket.IsKuaFuLogin)
			{
				bool bHasMirror = false;
				lock (this.Mutex)
				{
					bHasMirror = this.MirrorDatas.ContainsKey(data.RoleId1);
				}
				client.ClientSocket.ClientKuaFuServerLoginData.RoleId = data.RoleId1;
				client.ClientSocket.ClientKuaFuServerLoginData.GameId = (long)data.GameId;
				client.ClientSocket.ClientKuaFuServerLoginData.GameType = 12;
				client.ClientSocket.ClientKuaFuServerLoginData.EndTicks = 0L;
				client.ClientSocket.ClientKuaFuServerLoginData.ServerId = GameCoreInterface.getinstance().GetLocalServerId();
				client.ClientSocket.ClientKuaFuServerLoginData.ServerIp = data.ToServerIp;
				client.ClientSocket.ClientKuaFuServerLoginData.ServerPort = data.ToServerPort;
				client.ClientSocket.ClientKuaFuServerLoginData.FuBenSeqId = 0;
				client.sendCmd(1356, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					data.GameId,
					data.Day,
					data.Loop,
					bHasMirror ? 1 : 0
				}), false);
			}
			GameClient client2 = GameManager.ClientMgr.FindClient(data.RoleId2);
			if (client2 != null && !client2.ClientSocket.IsKuaFuLogin)
			{
				bool bHasMirror = false;
				lock (this.Mutex)
				{
					bHasMirror = this.MirrorDatas.ContainsKey(data.RoleId2);
				}
				client2.ClientSocket.ClientKuaFuServerLoginData.RoleId = data.RoleId2;
				client2.ClientSocket.ClientKuaFuServerLoginData.GameId = (long)data.GameId;
				client2.ClientSocket.ClientKuaFuServerLoginData.GameType = 12;
				client2.ClientSocket.ClientKuaFuServerLoginData.EndTicks = 0L;
				client2.ClientSocket.ClientKuaFuServerLoginData.ServerId = GameCoreInterface.getinstance().GetLocalServerId();
				client2.ClientSocket.ClientKuaFuServerLoginData.ServerIp = data.ToServerIp;
				client2.ClientSocket.ClientKuaFuServerLoginData.ServerPort = data.ToServerPort;
				client2.ClientSocket.ClientKuaFuServerLoginData.FuBenSeqId = 0;
				client2.sendCmd(1356, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					data.GameId,
					data.Day,
					data.Loop,
					bHasMirror ? 1 : 0
				}), false);
			}
		}

		// Token: 0x06001337 RID: 4919 RVA: 0x00130E24 File Offset: 0x0012F024
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 10)
			{
				this.HandleClientDead(((PlayerDeadEventObject)eventObject).getPlayer());
			}
			if (eventObject.getEventType() == 11)
			{
				this.HandleMonsterDead(((MonsterDeadEventObject)eventObject).getAttacker(), ((MonsterDeadEventObject)eventObject).getMonster());
			}
		}

		// Token: 0x06001338 RID: 4920 RVA: 0x00130E87 File Offset: 0x0012F087
		private void HandleClientDead(GameClient player)
		{
			this.OnLogout(player);
		}

		// Token: 0x06001339 RID: 4921 RVA: 0x00130E94 File Offset: 0x0012F094
		private void HandleMonsterDead(GameClient player, Monster monster)
		{
			if (player != null)
			{
				if (monster != null)
				{
					Robot robot = monster as Robot;
					if (robot != null)
					{
						if (player.ClientData.CopyMapID > 0 && player.ClientData.FuBenSeqID > 0)
						{
							lock (this.Mutex)
							{
								ZhengBaManager.ZhengBaCopyScene scene = null;
								if (this.FuBenSeq2CopyScenes.TryGetValue(player.ClientData.FuBenSeqID, out scene))
								{
									if (player.ClientData.MapCode == scene.MapCode)
									{
										if (monster.CurrentMapCode == scene.MapCode)
										{
											if (scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
											{
												if (scene.Robot1 != null)
												{
													scene.Robot1.stopAttack();
												}
												if (scene.Robot2 != null)
												{
													scene.Robot2.stopAttack();
												}
												scene.Winner = player.ClientData.RoleID;
												scene.m_eStatus = GameSceneStatuses.STATUS_END;
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600133A RID: 4922 RVA: 0x00130FEC File Offset: 0x0012F1EC
		public bool CanKuaFuLogin(KuaFuServerLoginData kuaFuServerLoginData)
		{
			return TianTiClient.getInstance().ZhengBaKuaFuLogin(kuaFuServerLoginData.RoleId, (int)kuaFuServerLoginData.GameId) >= 0;
		}

		// Token: 0x0600133B RID: 4923 RVA: 0x00131048 File Offset: 0x0012F248
		public bool KuaFuInitGame(GameClient client)
		{
			bool result;
			lock (this.Mutex)
			{
				ZhengBaMatchConfig matchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == this.SyncData.RealActDay);
				if (matchConfig == null)
				{
					result = false;
				}
				else
				{
					int gameId = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
					if (gameId < 0)
					{
						result = false;
					}
					else
					{
						ZhengBaManager.GameSideInfo sideInfo = null;
						if (!this.GameId2FuBenSeq.TryGetValue(gameId, out sideInfo))
						{
							sideInfo = new ZhengBaManager.GameSideInfo();
							sideInfo.FuBenSeq = GameCoreInterface.getinstance().GetNewFuBenSeqId();
							this.GameId2FuBenSeq[gameId] = sideInfo;
						}
						int toX = 0;
						int toY = 0;
						if (!this.GetBirthPoint(matchConfig.MapCode, ++sideInfo.CurrSide, out toX, out toY))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("roleid={0},mapcode={1},side={2} 未找到出生点", client.ClientData.RoleID, matchConfig.MapCode, sideInfo.CurrSide), null, true);
							result = false;
						}
						else
						{
							Global.GetClientKuaFuServerLoginData(client).FuBenSeqId = sideInfo.FuBenSeq;
							client.ClientData.MapCode = matchConfig.MapCode;
							client.ClientData.PosX = toX;
							client.ClientData.PosY = toY;
							client.ClientData.FuBenSeqID = sideInfo.FuBenSeq;
							client.ClientData.BattleWhichSide = sideInfo.CurrSide;
							result = true;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600133C RID: 4924 RVA: 0x00131210 File Offset: 0x0012F410
		public void OnLogout(GameClient player)
		{
			if (player != null && player.ClientSocket.IsKuaFuLogin)
			{
				if (player.ClientData.CopyMapID > 0 && player.ClientData.FuBenSeqID > 0)
				{
					lock (this.Mutex)
					{
						ZhengBaManager.ZhengBaCopyScene scene = null;
						if (this.FuBenSeq2CopyScenes.TryGetValue(player.ClientData.FuBenSeqID, out scene))
						{
							if (player.ClientData.MapCode == scene.MapCode)
							{
								if (scene.m_eStatus < GameSceneStatuses.STATUS_BEGIN)
								{
									if (scene.FirstLeaveRoleId <= 0 && (scene.RoleId1 == player.ClientData.RoleID || scene.RoleId2 == player.ClientData.RoleID))
									{
										scene.FirstLeaveRoleId = player.ClientData.RoleID;
									}
								}
								else if (scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
								{
									if (scene.Robot1 != null)
									{
										scene.Robot1.stopAttack();
									}
									if (scene.Robot2 != null)
									{
										scene.Robot2.stopAttack();
									}
									scene.Winner = 0;
									if (player.ClientData.RoleID == scene.RoleId1 && scene.RoleId2 > 0)
									{
										scene.Winner = scene.RoleId2;
									}
									else if (player.ClientData.RoleID == scene.RoleId2 && scene.RoleId1 > 0)
									{
										scene.Winner = scene.RoleId1;
									}
									scene.m_eStatus = GameSceneStatuses.STATUS_END;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600133D RID: 4925 RVA: 0x00131410 File Offset: 0x0012F610
		private bool GetBirthPoint(int mapCode, int side, out int toPosX, out int toPosY)
		{
			toPosX = -1;
			toPosY = -1;
			GameMap gameMap = null;
			bool result;
			if (!GameManager.MapMgr.DictMaps.TryGetValue(mapCode, out gameMap))
			{
				result = false;
			}
			else
			{
				int defaultBirthPosX = this._Config.BirthPointList[side % this._Config.BirthPointList.Count].X;
				int defaultBirthPosY = this._Config.BirthPointList[side % this._Config.BirthPointList.Count].Y;
				int defaultBirthRadius = this._Config.BirthPointList[side % this._Config.BirthPointList.Count].Radius;
				Point newPos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, mapCode, defaultBirthPosX, defaultBirthPosY, defaultBirthRadius);
				toPosX = (int)newPos.X;
				toPosY = (int)newPos.Y;
				result = true;
			}
			return result;
		}

		// Token: 0x0600133E RID: 4926 RVA: 0x0013150C File Offset: 0x0012F70C
		public void AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			if (sceneType == SceneUIClasses.KFZhengBa)
			{
				ZhengBaMatchConfig matchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == this.SyncData.RealActDay);
				if (matchConfig != null)
				{
					int fuBenSeqId = copyMap.FuBenSeqID;
					int mapCode = copyMap.MapCode;
					lock (this.Mutex)
					{
						ZhengBaManager.ZhengBaCopyScene scene = null;
						if (!this.FuBenSeq2CopyScenes.TryGetValue(fuBenSeqId, out scene))
						{
							scene = new ZhengBaManager.ZhengBaCopyScene();
							scene.GameId = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
							scene.FuBenSeq = fuBenSeqId;
							scene.MapCode = mapCode;
							this.FuBenSeq2CopyScenes[fuBenSeqId] = scene;
						}
						if (scene.CopyMap == null)
						{
							copyMap.IsKuaFuCopy = true;
							copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)((matchConfig.WaitSeconds + matchConfig.FightSeconds + matchConfig.ClearSeconds) * 1000));
							scene.CopyMap = copyMap;
						}
						if (scene.RoleId1 <= 0)
						{
							scene.RoleId1 = client.ClientData.RoleID;
							scene.IsMirror1 = false;
						}
						else if (scene.RoleId2 <= 0)
						{
							scene.RoleId2 = client.ClientData.RoleID;
							scene.IsMirror2 = false;
						}
					}
				}
			}
		}

		// Token: 0x0600133F RID: 4927 RVA: 0x00131690 File Offset: 0x0012F890
		public void RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
		{
			if (copyMap != null && sceneType == SceneUIClasses.KFZhengBa)
			{
				lock (this.Mutex)
				{
					ZhengBaManager.ZhengBaCopyScene scene = null;
					if (this.FuBenSeq2CopyScenes.TryGetValue(copyMap.FuBenSeqID, out scene))
					{
						this.FuBenSeq2CopyScenes.Remove(copyMap.FuBenSeqID);
						this.GameId2FuBenSeq.Remove(scene.GameId);
					}
				}
			}
		}

		// Token: 0x06001340 RID: 4928 RVA: 0x0013172C File Offset: 0x0012F92C
		private void ProcessEnd(ZhengBaManager.ZhengBaCopyScene scene, DateTime now, long nowTicks, int clearSec)
		{
			scene.m_eStatus = GameSceneStatuses.STATUS_AWARD;
			scene.m_lEndTime = nowTicks;
			scene.m_lLeaveTime = scene.m_lEndTime + (long)(clearSec * 1000);
			scene.StateTimeData.GameType = 12;
			scene.StateTimeData.State = 3;
			scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
			if (scene.CopyMap != null)
			{
				GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
			}
			if (scene.Robot1 != null)
			{
				scene.Robot1.stopAttack();
			}
			if (scene.Robot2 != null)
			{
				scene.Robot2.stopAttack();
			}
			List<ZhengBaNtfPkResultData> pkResult = TianTiClient.getInstance().ZhengBaPkResult(scene.GameId, scene.Winner, scene.FirstLeaveRoleId);
			if (pkResult != null)
			{
				foreach (ZhengBaNtfPkResultData result in pkResult)
				{
					GameClient client = GameManager.ClientMgr.FindClient(result.RoleID);
					if (client != null && client.ClientData.MapCode == scene.MapCode)
					{
						client.sendCmd<ZhengBaNtfPkResultData>(1358, result, false);
					}
				}
			}
		}

		// Token: 0x06001341 RID: 4929 RVA: 0x001318B8 File Offset: 0x0012FAB8
		public void UpdateCopyScene(object sender, EventArgs e)
		{
			long nowTicks = TimeUtil.NOW();
			if (nowTicks >= this.NextHeartBeatMs)
			{
				this.NextHeartBeatMs = nowTicks + 100L;
				ZhengBaMatchConfig matchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == this.SyncData.RealActDay);
				if (matchConfig != null)
				{
					lock (this.Mutex)
					{
						foreach (ZhengBaManager.ZhengBaCopyScene scene in this.FuBenSeq2CopyScenes.Values.ToList<ZhengBaManager.ZhengBaCopyScene>())
						{
							DateTime now = TimeUtil.NowDateTime();
							long ticks = TimeUtil.NOW();
							if (scene.m_eStatus == GameSceneStatuses.STATUS_NULL)
							{
								scene.m_lPrepareTime = ticks;
								scene.m_lBeginTime = ticks + 30000L;
								scene.m_lEndTime = ticks + (long)(matchConfig.FightSeconds * 1000);
								scene.m_eStatus = GameSceneStatuses.STATUS_PREPARE;
								scene.StateTimeData.GameType = 12;
								scene.StateTimeData.State = (int)scene.m_eStatus;
								scene.StateTimeData.EndTicks = scene.m_lBeginTime;
								if (scene.CopyMap != null)
								{
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_PREPARE)
							{
								if (scene.RoleId1 > 0 && scene.RoleId2 > 0)
								{
									scene.m_eStatus = GameSceneStatuses.STATUS_BEGIN;
									scene.StateTimeData.GameType = 12;
									scene.StateTimeData.State = (int)scene.m_eStatus;
									scene.StateTimeData.EndTicks = scene.m_lEndTime;
									if (scene.CopyMap != null)
									{
										GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
										scene.CopyMap.AddGuangMuEvent(1, 0);
										GameManager.ClientMgr.BroadSpecialMapAIEvent(scene.CopyMap.MapCode, scene.CopyMap.CopyMapID, 1, 0);
										scene.CopyMap.AddGuangMuEvent(2, 0);
										GameManager.ClientMgr.BroadSpecialMapAIEvent(scene.CopyMap.MapCode, scene.CopyMap.CopyMapID, 2, 0);
									}
									if (!scene.IsMirror1 || !scene.IsMirror2)
									{
										if (scene.IsMirror1)
										{
											GameClient player2 = GameManager.ClientMgr.FindClient(scene.RoleId2);
											if (player2 != null && player2.ClientData.MapCode == scene.MapCode)
											{
												scene.Robot1 = JingJiChangManager.getInstance().createRobot(player2, scene.JingJiData1, scene.MapCode);
												GameMap gameMap = GameManager.MapMgr.DictMaps[scene.MapCode];
												int side = 0;
												ZhengBaManager.GameSideInfo sideInfo = null;
												if (this.GameId2FuBenSeq.TryGetValue(scene.FuBenSeq, out sideInfo))
												{
													side = ++sideInfo.CurrSide;
												}
												int RobotBothX;
												int RobotBothY;
												this.GetBirthPoint(scene.MapCode, side, out RobotBothX, out RobotBothY);
												int gridX = gameMap.CorrectWidthPointToGridPoint(RobotBothX) / gameMap.MapGridWidth;
												int gridY = gameMap.CorrectHeightPointToGridPoint(RobotBothY) / gameMap.MapGridHeight;
												GameManager.MonsterZoneMgr.AddDynamicRobot(scene.MapCode, scene.Robot1, scene.CopyMap.CopyMapID, 1, gridX, gridY, 1, 0, SceneUIClasses.KFZhengBa, scene.RoleId2);
											}
										}
										else if (scene.IsMirror2)
										{
											GameClient player3 = GameManager.ClientMgr.FindClient(scene.RoleId1);
											if (player3 != null && player3.ClientData.MapCode == scene.MapCode)
											{
												scene.Robot2 = JingJiChangManager.getInstance().createRobot(player3, scene.JingJiData2, scene.MapCode);
												GameMap gameMap = GameManager.MapMgr.DictMaps[scene.MapCode];
												int side = 0;
												ZhengBaManager.GameSideInfo sideInfo = null;
												if (this.GameId2FuBenSeq.TryGetValue(scene.FuBenSeq, out sideInfo))
												{
													side = ++sideInfo.CurrSide;
												}
												int RobotBothX;
												int RobotBothY;
												this.GetBirthPoint(scene.MapCode, side, out RobotBothX, out RobotBothY);
												int gridX = gameMap.CorrectWidthPointToGridPoint(RobotBothX) / gameMap.MapGridWidth;
												int gridY = gameMap.CorrectHeightPointToGridPoint(RobotBothY) / gameMap.MapGridHeight;
												GameManager.MonsterZoneMgr.AddDynamicRobot(scene.MapCode, scene.Robot2, scene.CopyMap.CopyMapID, 1, gridX, gridY, 1, 0, SceneUIClasses.KFZhengBa, scene.RoleId1);
											}
										}
									}
								}
								else if (ticks >= scene.m_lBeginTime)
								{
									scene.Winner = 0;
									if (scene.RoleId1 > 0 && scene.FirstLeaveRoleId != scene.RoleId1)
									{
										scene.Winner = scene.RoleId1;
									}
									else if (scene.RoleId2 > 0 && scene.FirstLeaveRoleId != scene.RoleId2)
									{
										scene.Winner = scene.RoleId2;
									}
									scene.m_eStatus = GameSceneStatuses.STATUS_END;
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
							{
								if (scene.FirstLeaveRoleId > 0)
								{
									scene.Winner = 0;
									scene.m_eStatus = GameSceneStatuses.STATUS_END;
								}
								else if (ticks >= scene.m_lEndTime)
								{
									scene.Winner = 0;
									if (scene.IsMirror1 && scene.IsMirror2)
									{
										TianTiPaiHangRoleData data = null;
										TianTiPaiHangRoleData data2 = null;
										if (this.RoleDataDict.TryGetValue(scene.RoleId1, out data) && this.RoleDataDict.TryGetValue(scene.RoleId2, out data2))
										{
											if (data.DuanWeiRank < data2.DuanWeiRank)
											{
												scene.Winner = data.RoleId;
											}
											else
											{
												scene.Winner = data2.RoleId;
											}
										}
									}
									else if (scene.IsMirror1 || scene.IsMirror2)
									{
										Robot robot = scene.IsMirror1 ? scene.Robot1 : scene.Robot2;
										GameClient client = GameManager.ClientMgr.FindClient(scene.IsMirror1 ? scene.RoleId2 : scene.RoleId1);
										if (client != null && robot != null)
										{
											int clientMaxLifeV = (int)RoleAlgorithm.GetMaxLifeV(client);
											if (clientMaxLifeV > 0 && robot.MonsterInfo.VLifeMax > 0.0)
											{
												if ((double)client.ClientData.CurrentLifeV * 1.0 / (double)clientMaxLifeV >= robot.VLife * 1.0 / robot.MonsterInfo.VLifeMax)
												{
													scene.Winner = client.ClientData.RoleID;
												}
												else
												{
													scene.Winner = robot.getRoleDataMini().RoleID;
												}
											}
											else
											{
												scene.Winner = client.ClientData.RoleID;
											}
										}
										else
										{
											scene.Winner = (scene.IsMirror1 ? scene.RoleId2 : scene.RoleId1);
										}
									}
									else
									{
										GameClient client2 = GameManager.ClientMgr.FindClient(scene.RoleId1);
										GameClient client3 = GameManager.ClientMgr.FindClient(scene.RoleId2);
										if (client2 != null && client3 != null)
										{
											int clientMaxLifeV2 = (int)RoleAlgorithm.GetMaxLifeV(client2);
											int clientMaxLifeV3 = (int)RoleAlgorithm.GetMaxLifeV(client3);
											if (clientMaxLifeV2 > 0 && clientMaxLifeV3 > 0)
											{
												if ((double)client2.ClientData.CurrentLifeV * 1.0 / (double)clientMaxLifeV2 >= (double)client3.ClientData.CurrentLifeV * 1.0 / (double)clientMaxLifeV3)
												{
													scene.Winner = client2.ClientData.RoleID;
												}
												else
												{
													scene.Winner = client3.ClientData.RoleID;
												}
											}
										}
									}
									scene.m_eStatus = GameSceneStatuses.STATUS_END;
								}
								else if (!scene.IsMirror1 || !scene.IsMirror2)
								{
									if (scene.Robot1 != null)
									{
										scene.Robot1.onUpdate();
									}
									else if (scene.Robot2 != null)
									{
										scene.Robot2.onUpdate();
									}
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_END)
							{
								this.ProcessEnd(scene, now, nowTicks, matchConfig.ClearSeconds);
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_AWARD)
							{
								if (ticks >= scene.m_lLeaveTime)
								{
									scene.m_eStatus = GameSceneStatuses.STATUS_CLEAR;
									if (scene.CopyMap != null)
									{
										scene.CopyMap.SetRemoveTicks(scene.m_lLeaveTime);
										try
										{
											List<GameClient> objsList = scene.CopyMap.GetClientsList();
											if (objsList != null && objsList.Count > 0)
											{
												for (int i = 0; i < objsList.Count; i++)
												{
													GameClient c = objsList[i];
													if (c != null)
													{
														KuaFuManager.getInstance().GotoLastMap(c);
													}
												}
											}
										}
										catch (Exception ex)
										{
											DataHelper.WriteExceptionLogEx(ex, "众神争霸系统清场调度异常");
										}
									}
									else
									{
										this.FuBenSeq2CopyScenes.Remove(scene.FuBenSeq);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x04001C2D RID: 7213
		private Dictionary<int, ZhengBaManager.ZhengBaCopyScene> FuBenSeq2CopyScenes = new Dictionary<int, ZhengBaManager.ZhengBaCopyScene>();

		// Token: 0x04001C2E RID: 7214
		private Dictionary<int, ZhengBaManager.GameSideInfo> GameId2FuBenSeq = new Dictionary<int, ZhengBaManager.GameSideInfo>();

		// Token: 0x04001C2F RID: 7215
		private long NextHeartBeatMs = TimeUtil.NOW();

		// Token: 0x04001C30 RID: 7216
		private ZhengBaConfig _Config = new ZhengBaConfig();

		// Token: 0x04001C31 RID: 7217
		private List<ZhengBaMatchAward> _MatchAwardList = new List<ZhengBaMatchAward>();

		// Token: 0x04001C32 RID: 7218
		private object Mutex = new object();

		// Token: 0x04001C33 RID: 7219
		private ZhengBaSyncData SyncData = new ZhengBaSyncData();

		// Token: 0x04001C34 RID: 7220
		private TimeSpan DiffKfCenter = TimeSpan.Zero;

		// Token: 0x04001C35 RID: 7221
		private Dictionary<int, TianTiPaiHangRoleData> RoleDataDict = new Dictionary<int, TianTiPaiHangRoleData>();

		// Token: 0x04001C36 RID: 7222
		private List<TianTiPaiHangRoleData> RoleDataList = new List<TianTiPaiHangRoleData>();

		// Token: 0x04001C37 RID: 7223
		private List<TianTiPaiHangRoleData> Top16RoleList = new List<TianTiPaiHangRoleData>();

		// Token: 0x04001C38 RID: 7224
		private List<ZhengBaSupportAnalysisData> SupportDatas = new List<ZhengBaSupportAnalysisData>();

		// Token: 0x04001C39 RID: 7225
		private Dictionary<int, PlayerJingJiData> MirrorDatas = new Dictionary<int, PlayerJingJiData>();

		// Token: 0x04001C3A RID: 7226
		private int MaxSupportGroup = 0;

		// Token: 0x04001C3B RID: 7227
		private int MaxOpposeGroup = 0;

		// Token: 0x04001C3C RID: 7228
		private Queue<ZhengBaPkLogData> PkLogQ = new Queue<ZhengBaPkLogData>();

		// Token: 0x04001C3D RID: 7229
		private Dictionary<int, Queue<ZhengBaSupportLogData>> SupportLogs = new Dictionary<int, Queue<ZhengBaSupportLogData>>();

		// Token: 0x04001C3E RID: 7230
		private List<ZhengBaWaitYaZhuAwardData> WaitAwardOfYaZhuList = new List<ZhengBaWaitYaZhuAwardData>();

		// Token: 0x04001C3F RID: 7231
		private int ZhengBaKingRoleId;

		// Token: 0x04001C40 RID: 7232
		private RoleData4Selector ZhengBaKingData;

		// Token: 0x0200041E RID: 1054
		private class ZhengBaCopyScene
		{
			// Token: 0x04001C52 RID: 7250
			public int FuBenSeq;

			// Token: 0x04001C53 RID: 7251
			public int GameId;

			// Token: 0x04001C54 RID: 7252
			public int MapCode;

			// Token: 0x04001C55 RID: 7253
			public CopyMap CopyMap;

			// Token: 0x04001C56 RID: 7254
			public int RoleId1;

			// Token: 0x04001C57 RID: 7255
			public bool IsMirror1;

			// Token: 0x04001C58 RID: 7256
			public PlayerJingJiData JingJiData1;

			// Token: 0x04001C59 RID: 7257
			public Robot Robot1;

			// Token: 0x04001C5A RID: 7258
			public int RoleId2;

			// Token: 0x04001C5B RID: 7259
			public bool IsMirror2;

			// Token: 0x04001C5C RID: 7260
			public PlayerJingJiData JingJiData2;

			// Token: 0x04001C5D RID: 7261
			public Robot Robot2;

			// Token: 0x04001C5E RID: 7262
			public long m_lPrepareTime = 0L;

			// Token: 0x04001C5F RID: 7263
			public long m_lBeginTime = 0L;

			// Token: 0x04001C60 RID: 7264
			public long m_lEndTime = 0L;

			// Token: 0x04001C61 RID: 7265
			public long m_lLeaveTime = 0L;

			// Token: 0x04001C62 RID: 7266
			public GameSceneStatuses m_eStatus = GameSceneStatuses.STATUS_NULL;

			// Token: 0x04001C63 RID: 7267
			public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

			// Token: 0x04001C64 RID: 7268
			public int Winner = 0;

			// Token: 0x04001C65 RID: 7269
			public int FirstLeaveRoleId = 0;
		}

		// Token: 0x0200041F RID: 1055
		private class GameSideInfo
		{
			// Token: 0x04001C66 RID: 7270
			public int FuBenSeq;

			// Token: 0x04001C67 RID: 7271
			public int CurrSide;
		}
	}
}
