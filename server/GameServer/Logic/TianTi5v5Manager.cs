using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using ProtoBuf;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x020000B8 RID: 184
	public class TianTi5v5Manager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2, ICopySceneManager
	{
		// Token: 0x060002F3 RID: 755 RVA: 0x00033BD8 File Offset: 0x00031DD8
		public static TianTi5v5Manager getInstance()
		{
			return TianTi5v5Manager.instance;
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x00033BF0 File Offset: 0x00031DF0
		public bool initialize()
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("TianTi5v5Manager.TimerProc", new EventHandler(this.TimerProc)), 20000, 10000);
			return this.InitConfig(false);
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x00033C40 File Offset: 0x00031E40
		public bool initialize(ICoreInterface coreInterface)
		{
			return true;
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x00033C54 File Offset: 0x00031E54
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(3693, 1, 1, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3697, 1, 1, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3680, 1, 1, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3687, 2, 3, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3690, 2, 2, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3685, 2, 3, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3686, 2, 2, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3688, 1, 1, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3691, 2, 2, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3695, 1, 1, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3694, 1, 1, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3698, 2, 2, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3700, 1, 1, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3699, 1, 1, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3704, 1, 1, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3709, 1, 1, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3711, 1, 1, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3713, 1, 1, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3714, 2, 2, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3712, 0, 5, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3679, 0, 5, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3719, 2, 2, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3721, 2, 2, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(60, 55, TianTi5v5Manager.getInstance());
			GlobalEventSource.getInstance().registerListener(10, TianTi5v5Manager.getInstance());
			GlobalEventSource.getInstance().registerListener(14, TianTi5v5Manager.getInstance());
			GlobalEventSource.getInstance().registerListener(13, TianTi5v5Manager.getInstance());
			GlobalEventSource.getInstance().registerListener(59, TianTi5v5Manager.getInstance());
			GlobalEventSource.getInstance().registerListener(28, TianTi5v5Manager.getInstance());
			return true;
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x00033F00 File Offset: 0x00032100
		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(60, 55, TianTi5v5Manager.getInstance());
			GlobalEventSource.getInstance().removeListener(10, TianTi5v5Manager.getInstance());
			GlobalEventSource.getInstance().removeListener(14, TianTi5v5Manager.getInstance());
			GlobalEventSource.getInstance().removeListener(13, TianTi5v5Manager.getInstance());
			GlobalEventSource.getInstance().registerListener(59, TianTi5v5Manager.getInstance());
			GlobalEventSource.getInstance().registerListener(28, TianTi5v5Manager.getInstance());
			return true;
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x00033F84 File Offset: 0x00032184
		public bool destroy()
		{
			return true;
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x00033F98 File Offset: 0x00032198
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x060002FA RID: 762 RVA: 0x00033FAC File Offset: 0x000321AC
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			switch (nID)
			{
			case 3679:
				return this.ProcessTianTi5v5GetPaiHangAwardsCmd(client, nID, bytes, cmdParams);
			case 3680:
				return this.ProcessGetTianTi5v5DataAndDayPaiHangCmd(client, nID, bytes, cmdParams);
			case 3685:
				return this.ProcessCreateZhanDui(client, nID, bytes, cmdParams);
			case 3686:
				return this.ProcessDeleteZhanDuiMember(client, nID, bytes, cmdParams);
			case 3687:
				return this.ProcessInviteOther2MyZhanDui(client, nID, bytes, cmdParams);
			case 3688:
				return this.ProcessGetZhanDuiList(client, nID, bytes, cmdParams);
			case 3690:
				return this.ProcessAgreeZhanDuiInvite(client, nID, bytes, cmdParams);
			case 3691:
				return this.ProcessUpdateZhanDuiXuanYan(client, nID, bytes, cmdParams);
			case 3693:
				return this.ProcessTianTi5v5QuitCmd(client, nID, bytes, cmdParams);
			case 3695:
				return this.ProcessGetMyZhanDuiInfo(client, nID, bytes, cmdParams);
			case 3697:
				return this.ProcessTianTi5v5EnterCmd(client, nID, bytes, cmdParams);
			case 3698:
				return this.ProcessChangeZhanDuiLeader(client, nID, bytes, cmdParams);
			case 3699:
				return this.ProcessDeleteZhanDui(client, nID, bytes, cmdParams);
			case 3700:
				return this.ProcessZhanDuiKF5V5JingJiData(client, nID, bytes, cmdParams);
			case 3704:
				return this.ProcessGetDayPaiHangCmd(client, nID, bytes, cmdParams);
			case 3709:
				return this.ProcessTianTi5v5GeLogCmd(client, nID, bytes, cmdParams);
			case 3711:
				return this.ProcessQuitFromZhanDui(client, nID, bytes, cmdParams);
			case 3713:
				return this.ProcessConfirmBattleCmd(client, nID, bytes, cmdParams);
			case 3714:
				return this.ProcessAcceptConfirmBattleCmd(client, nID, bytes, cmdParams);
			case 3719:
				return this.ProcessRequestToZhanDui(client, nID, bytes, cmdParams);
			case 3721:
				return this.ProcessAgreeZhanDuiRequest(client, nID, bytes, cmdParams);
			}
			return true;
		}

		// Token: 0x060002FB RID: 763 RVA: 0x000341D0 File Offset: 0x000323D0
		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 10)
			{
				PlayerDeadEventObject playerDeadEvent = eventObject as PlayerDeadEventObject;
				if (null != playerDeadEvent)
				{
					if (playerDeadEvent.Type == PlayerDeadEventTypes.ByRole)
					{
						this.OnKillRole(playerDeadEvent.getAttackerRole(), playerDeadEvent.getPlayer());
					}
				}
			}
			else if (eventType == 14)
			{
				PlayerInitGameEventObject e = eventObject as PlayerInitGameEventObject;
				if (null != e)
				{
					this.InitRoleTianTi5v5Data(e.getPlayer());
				}
			}
			else if (eventType == 28)
			{
				OnStartPlayGameEventObject e2 = eventObject as OnStartPlayGameEventObject;
				if (null != e2)
				{
					this.SendZhanDuiInviteData(e2.Client);
					this.SendZhanDuiRequestData(e2.Client);
				}
			}
			else if (eventType == 59)
			{
				OnClientMapChangedEventObject e3 = eventObject as OnClientMapChangedEventObject;
				if (null != e3)
				{
					this.SendZhanDuiInviteData(e3.Client);
					this.SendZhanDuiRequestData(e3.Client);
				}
			}
			else if (eventObject.getEventType() == 13)
			{
				PlayerLeaveFuBenEventObject eventObj = (PlayerLeaveFuBenEventObject)eventObject;
				this.RoleLeaveFuBen(eventObj.getPlayer());
			}
		}

		// Token: 0x060002FC RID: 764 RVA: 0x00034318 File Offset: 0x00032518
		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			int num = eventType;
			if (num == 60)
			{
				this.NotifyTimeStateInfoAndScoreInfo(eventObject.Sender as GameClient);
			}
		}

		// Token: 0x060002FD RID: 765 RVA: 0x0003434C File Offset: 0x0003254C
		public bool InitConfig(bool reload = false)
		{
			bool success = true;
			string fileName = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.TianTi5v5CD = (int)GameManager.systemParamsList.GetParamValueIntByName("TianTi5v5CD", -1);
					Dictionary<int, TianTiDuanWei> duanWeiDict = new Dictionary<int, TianTiDuanWei>();
					Dictionary<RangeKey, int> rangeDict = new Dictionary<RangeKey, int>(RangeKey.Comparer);
					int preJiFen = 0;
					int perDuanWeiId = 0;
					int maxDuanWeiId = 0;
					fileName = "Config/TeamDuanWei.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					XElement xml = XElement.Load(fullPathFileName);
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						TianTiDuanWei item = new TianTiDuanWei();
						item.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item.NeedDuanWeiJiFen = (int)Global.GetSafeAttributeLong(node, "NeedDuanWeiJiFen");
						item.WinJiFen = (int)Global.GetSafeAttributeLong(node, "WinJiFen");
						item.LoseJiFen = (int)Global.GetSafeAttributeLong(node, "LoseJiFen");
						item.RongYaoNum = (int)Global.GetSafeAttributeLong(node, "RongYaoNum");
						item.WinRongYu = (int)Global.GetSafeAttributeLong(node, "WinRongYu");
						item.LoseRongYu = (int)Global.GetSafeAttributeLong(node, "LoseRongYu");
						if (perDuanWeiId > 0)
						{
							rangeDict[new RangeKey(preJiFen, item.NeedDuanWeiJiFen - 1, null)] = perDuanWeiId;
						}
						preJiFen = item.NeedDuanWeiJiFen;
						perDuanWeiId = item.ID;
						maxDuanWeiId = item.ID;
						duanWeiDict[item.ID] = item;
					}
					if (maxDuanWeiId > 0 && preJiFen > 0)
					{
						rangeDict[new RangeKey(preJiFen, int.MaxValue, null)] = maxDuanWeiId;
					}
					this.RuntimeData.TianTi5v5DuanWeiDict = duanWeiDict;
					this.RuntimeData.DuanWeiJiFenRangeDuanWeiIdDict = rangeDict;
					this.RuntimeData.MapBirthPointDict.Clear();
					fileName = "Config/TeamBattleBirthPoint.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						TianTiBirthPoint item2 = new TianTiBirthPoint();
						item2.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item2.PosX = (int)Global.GetSafeAttributeLong(node, "PosX");
						item2.PosY = (int)Global.GetSafeAttributeLong(node, "PosY");
						item2.BirthRadius = (int)Global.GetSafeAttributeLong(node, "BirthRadius");
						this.RuntimeData.MapBirthPointDict[item2.ID] = item2;
					}
					this.RuntimeData.DuanWeiRankAwardDict.Clear();
					fileName = "Config/TeamDuanWeiAward.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						DuanWeiRankAward item3 = new DuanWeiRankAward();
						item3.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item3.StarRank = (int)Global.GetSafeAttributeLong(node, "StarRank");
						item3.EndRank = (int)Global.GetSafeAttributeLong(node, "EndRank");
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(node, "Award"), ref item3.Award, '|', ',');
						if (item3.EndRank < 0)
						{
							item3.EndRank = int.MaxValue;
						}
						this.RuntimeData.DuanWeiRankAwardDict[new RangeKey(item3.StarRank, item3.EndRank, null)] = item3;
					}
					this.RuntimeData.MapCodeDict.Clear();
					fileName = "Config/TeamBattle.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						int mapCode = (int)Global.GetSafeAttributeLong(node, "MapCode");
						if (!this.RuntimeData.MapCodeDict.ContainsKey(mapCode))
						{
							this.RuntimeData.MapCodeDict[mapCode] = 1;
							this.RuntimeData.MapCodeList.Add(mapCode);
						}
						this.RuntimeData.WaitingEnterSecs = (int)Global.GetSafeAttributeLong(node, "PrepareSecs");
						this.RuntimeData.FightingSecs = (int)Global.GetSafeAttributeLong(node, "FightingSecs");
						this.RuntimeData.ClearRolesSecs = (int)Global.GetSafeAttributeLong(node, "ClearRolesSecs");
						if (!ConfigParser.ParserTimeRangeList(this.RuntimeData.TimePoints, Global.GetSafeAttributeStr(node, "TimePoints"), true, '|', '-'))
						{
							success = false;
							LogManager.WriteLog(LogTypes.Fatal, "读取跨服组队竞技时间配置(TimePoints)出错", null, true);
						}
						GameMap gameMap = null;
						if (!GameManager.MapMgr.DictMaps.TryGetValue(mapCode, out gameMap))
						{
							success = false;
							LogManager.WriteLog(LogTypes.Fatal, string.Format("缺少跨服组队竞技地图 {0}", mapCode), null, true);
						}
					}
					this.RuntimeData.TeamBattleMap = GameManager.systemParamsList.GetParamValueIntArrayByName("TeamBattleMap", ',');
					this.RuntimeData.ZhanDuiJinZuan = (int)GameManager.systemParamsList.GetParamValueIntByName("TeamNeedZuan", 50);
					this.RuntimeData.TeamConfirmTime = (int)GameManager.systemParamsList.GetParamValueIntByName("TeamConfirmTime", -1);
					this.RuntimeData.TeamAwardLimit = (int)GameManager.systemParamsList.GetParamValueIntByName("TeamAwardLimit", -1);
					int[] dengjiArr = GameManager.systemParamsList.GetParamValueIntArrayByName("TeamLevelLimit", ',');
					if (dengjiArr != null && dengjiArr.Length >= 2)
					{
						this.RuntimeData.ZhanDuiDengJiTp = new Tuple<int, int>(dengjiArr[0], dengjiArr[1]);
					}
					this.RuntimeData.MaxZhanDuiNum = (int)GameManager.systemParamsList.GetParamValueIntByName("MaxZhanDuiNum", 1000);
					this.RuntimeData.MaxTianTi5v5JiFen = (int)GameManager.systemParamsList.GetParamValueIntByName("MaxTianTi5v5JiFen", -1);
					this.RuntimeData.TeamBattleWatch = GameManager.systemParamsList.GetParamValueIntArrayByName("TeamBattleWatch", ',');
					this.RuntimeData.MinConfirmFightTeamCnt = (int)GameManager.systemParamsList.GetParamValueIntByName("MinConfirmFightTeamCnt", 1);
					this.RuntimeData.TeamBattleNameRange = GameManager.systemParamsList.GetParamValueIntArrayByName("TeamBattleNameRange", ',');
					if (this.RuntimeData.TeamBattleNameRange == null)
					{
						this.RuntimeData.TeamBattleNameRange = new int[]
						{
							2,
							7
						};
					}
					this.RuntimeData.TeamApply = GameManager.systemParamsList.GetParamValueIntArrayByName("TeamApply", ',');
					if (this.RuntimeData.TeamApply == null)
					{
						this.RuntimeData.TeamApply = new int[]
						{
							1,
							2
						};
					}
				}
				catch (Exception ex)
				{
					success = false;
					LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
				}
			}
			return success;
		}

		// Token: 0x060002FE RID: 766 RVA: 0x00034B68 File Offset: 0x00032D68
		public void GMStartHuoDongNow(int v)
		{
			try
			{
				lock (this.RuntimeData.Mutex)
				{
					if (v == 0)
					{
						ConfigParser.ParserTimeRangeList(this.RuntimeData.TimePoints, this.RuntimeData.TimePointsStr, true, '|', '-');
					}
					else
					{
						ConfigParser.ParserTimeRangeList(this.RuntimeData.TimePoints, "00:00-23:59:59", true, '|', '-');
					}
				}
			}
			catch (Exception ex)
			{
			}
		}

		// Token: 0x060002FF RID: 767 RVA: 0x00034C30 File Offset: 0x00032E30
		private TianTi5v5ZhanDuiData NewZhanDuiData(GameClient client)
		{
			int duanWeiId = this.RuntimeData.DuanWeiJiFenRangeDuanWeiIdDict.Values.Min();
			TianTi5v5ZhanDuiData zhanDuiData = new TianTi5v5ZhanDuiData
			{
				ZhanDuiID = 0,
				DuanWeiId = duanWeiId
			};
			TianTi5v5ZhanDuiRoleData roleTianTi5v5Data = new TianTi5v5ZhanDuiRoleData();
			roleTianTi5v5Data.RoleID = client.ClientData.LocalRoleID;
			roleTianTi5v5Data.RoleName = client.ClientData.RoleName;
			roleTianTi5v5Data.RoleOcc = client.ClientData.Occupation;
			roleTianTi5v5Data.ZhanLi = (long)client.ClientData.CombatForce;
			roleTianTi5v5Data.Level = client.ClientData.Level;
			roleTianTi5v5Data.ZhuanSheng = client.ClientData.ChangeLifeCount;
			roleTianTi5v5Data.ZoneID = client.ClientData.ZoneID;
			roleTianTi5v5Data.RebornLevel = client.ClientData.RebornLevel;
			zhanDuiData.teamerList.Add(roleTianTi5v5Data);
			zhanDuiData.ZhanDouLi = zhanDuiData.teamerList.Sum((TianTi5v5ZhanDuiRoleData x) => x.ZhanLi);
			RoleData4Selector roleInfo = Global.sendToDB<RoleData4Selector, string>(512, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
			if (roleInfo != null || roleInfo.RoleID < 0)
			{
				zhanDuiData.teamerList[0].ModelData = DataHelper.ObjectToBytes<RoleData4Selector>(roleInfo);
			}
			return zhanDuiData;
		}

		// Token: 0x06000300 RID: 768 RVA: 0x00034DA0 File Offset: 0x00032FA0
		private TianTi5v5ZhanDuiRoleData NewRoleData(GameClient client)
		{
			TianTi5v5ZhanDuiRoleData roleTianTi5v5Data = new TianTi5v5ZhanDuiRoleData();
			roleTianTi5v5Data.RoleID = client.ClientData.LocalRoleID;
			roleTianTi5v5Data.RoleName = client.ClientData.RoleName;
			roleTianTi5v5Data.RoleOcc = client.ClientData.Occupation;
			roleTianTi5v5Data.ZhanLi = (long)client.ClientData.CombatForce;
			roleTianTi5v5Data.Level = client.ClientData.Level;
			roleTianTi5v5Data.ZhuanSheng = client.ClientData.ChangeLifeCount;
			roleTianTi5v5Data.RebornLevel = client.ClientData.RebornLevel;
			RoleData4Selector roleInfo = Global.sendToDB<RoleData4Selector, string>(512, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
			if (roleInfo != null || roleInfo.RoleID < 0)
			{
				roleTianTi5v5Data.ModelData = DataHelper.ObjectToBytes<RoleData4Selector>(roleInfo);
			}
			return roleTianTi5v5Data;
		}

		// Token: 0x06000301 RID: 769 RVA: 0x00034E84 File Offset: 0x00033084
		public void GMSetRoleData(GameClient client, int duanWeiId, int duanWeiJiFen, int rongYao, int monthDuanWeiRank, int lianSheng, int successCount, int fightCount)
		{
			TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId) ?? this.NewZhanDuiData(client);
			if (null != zhanDuiData)
			{
				zhanDuiData.DuanWeiJiFen = duanWeiJiFen;
				zhanDuiData.MonthDuanWeiRank = monthDuanWeiRank;
				zhanDuiData.LianSheng = lianSheng;
				zhanDuiData.SuccessCount = successCount;
				zhanDuiData.FightCount = fightCount;
				int selfDuanWeiId;
				if (this.RuntimeData.DuanWeiJiFenRangeDuanWeiIdDict.TryGetValue(zhanDuiData.ZhanDuiID, out selfDuanWeiId))
				{
					zhanDuiData.DuanWeiId = selfDuanWeiId;
				}
				RoleData4Selector roleInfo = Global.sendToDB<RoleData4Selector, string>(512, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
				if (roleInfo != null || roleInfo.RoleID < 0)
				{
					zhanDuiData.teamerList[0].ModelData = DataHelper.ObjectToBytes<RoleData4Selector>(roleInfo);
				}
				GameManager.ClientMgr.ModifyTeamRongYaoValue(client, rongYao - client.ClientData.TeamRongYao, "GM添加", false);
				TianTiClient.getInstance().UpdateZhanDuiData(zhanDuiData, ZhanDuiDataModeTypes.ZhanDuiInfo);
			}
		}

		// Token: 0x06000302 RID: 770 RVA: 0x00034FA0 File Offset: 0x000331A0
		public void TimerProc(object sender, EventArgs e)
		{
			bool modify = false;
			TianTi5v5RankData tianTiRankData = TianTiClient.getInstance().ZhanDuiGetRankingData(this.RuntimeData.RankData.ModifyTime);
			lock (this.RuntimeData.Mutex)
			{
				if (tianTiRankData != null && tianTiRankData.ModifyTime > this.RuntimeData.ModifyTime)
				{
					modify = true;
				}
			}
			if (modify)
			{
				Dictionary<int, TianTi5v5ZhanDuiData> tianTiPaiHangRoleDataDict = new Dictionary<int, TianTi5v5ZhanDuiData>();
				List<TianTi5v5ZhanDuiData> tianTiPaiHangRoleDataList = new List<TianTi5v5ZhanDuiData>();
				Dictionary<int, TianTi5v5ZhanDuiData> tianTiMonthPaiHangRoleDataDict = new Dictionary<int, TianTi5v5ZhanDuiData>();
				List<TianTi5v5ZhanDuiData> tianTiMonthPaiHangRoleDataList = new List<TianTi5v5ZhanDuiData>();
				if (null != tianTiRankData.DayPaiHangList)
				{
					foreach (TianTi5v5ZhanDuiData data in tianTiRankData.DayPaiHangList)
					{
						tianTiPaiHangRoleDataDict[data.ZhanDuiID] = data;
						if (tianTiPaiHangRoleDataList.Count < this.RuntimeData.MaxDayPaiMingListCount)
						{
							tianTiPaiHangRoleDataList.Add(data);
						}
					}
				}
				if (null != tianTiRankData.MonthPaiHangList)
				{
					foreach (TianTi5v5ZhanDuiData data in tianTiRankData.MonthPaiHangList)
					{
						tianTiMonthPaiHangRoleDataDict[data.ZhanDuiID] = data;
						if (tianTiMonthPaiHangRoleDataList.Count < this.RuntimeData.MaxMonthPaiMingListCount)
						{
							tianTiMonthPaiHangRoleDataList.Add(data);
						}
					}
				}
				lock (this.RuntimeData.Mutex)
				{
					this.RuntimeData.ModifyTime = tianTiRankData.ModifyTime;
					this.RuntimeData.MaxPaiMingRank = tianTiRankData.MaxPaiMingRank;
					this.RuntimeData.ZhanDuiDataPaiHangDict = tianTiPaiHangRoleDataDict;
					this.RuntimeData.ZhanDuiDataPaiHangList = tianTiPaiHangRoleDataList;
					this.RuntimeData.ZhanDuiDataMonthPaiHangDict = tianTiMonthPaiHangRoleDataDict;
					this.RuntimeData.ZhanDuiDataMonthPaiHangList = tianTiMonthPaiHangRoleDataList;
				}
			}
		}

		// Token: 0x06000303 RID: 771 RVA: 0x00035214 File Offset: 0x00033414
		public bool ProcessTianTi5v5JoinCmd(int zhanDuiID, TianTi5v5PiPeiState piPeiState)
		{
			if (piPeiState.State == 0)
			{
				piPeiState.EndTicks += (long)this.RuntimeData.MaxSignUp;
				piPeiState.State = 1;
				int result = TianTiClient.getInstance().ZhanDuiRoleSignUp(GameManager.ServerId, 34, zhanDuiID, piPeiState.ZhanLi, piPeiState.GroupIndex);
				if (result > 0)
				{
					GlobalNew.UpdateKuaFuRoleDayLogData(GameManager.ServerId, zhanDuiID, TimeUtil.NowDateTime(), GameManager.ServerId, 1, 0, 0, 0, 34);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000304 RID: 772 RVA: 0x000352A4 File Offset: 0x000334A4
		public bool ProcessGetTianTi5v5DataAndDayPaiHangCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					this.GetMainInfo(client);
				}
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000305 RID: 773 RVA: 0x00035300 File Offset: 0x00033500
		public bool ProcessGetDayPaiHangCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					TianTi5v5Manager.TianTi5v5ZhanDuiDataList list = null;
					lock (this.RuntimeData.Mutex)
					{
						if (!this.RuntimeData.ZhanDuiDataPaiHangList.IsNullOrEmpty<TianTi5v5ZhanDuiData>())
						{
							int count = Math.Min(this.RuntimeData.MaxDayPaiMingListCount, this.RuntimeData.ZhanDuiDataPaiHangList.Count);
							list = new TianTi5v5Manager.TianTi5v5ZhanDuiDataList(this.RuntimeData.ZhanDuiDataPaiHangList.GetRange(0, count));
						}
					}
					client.sendCmd<TianTi5v5Manager.TianTi5v5ZhanDuiDataList>(nID, list, false);
				}
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000306 RID: 774 RVA: 0x000353F8 File Offset: 0x000335F8
		public bool ProcessTianTi5v5GetPaiHangAwardsCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = -20;
				DuanWeiRankAward duanWeiRankAward = null;
				if (this.CanGetMonthRankAwards(client, out duanWeiRankAward))
				{
					List<GoodsData> goodsDataList = Global.ConvertToGoodsDataList(duanWeiRankAward.Award.Items, -1);
					if (!Global.CanAddGoodsDataList(client, goodsDataList))
					{
						result = -100;
					}
					else
					{
						result = 0;
						Global.SaveRoleParamsDateTimeToDB(client, "10220", TimeUtil.NowDateTime(), true);
						for (int i = 0; i < goodsDataList.Count; i++)
						{
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsDataList[i].GoodsID, goodsDataList[i].GCount, goodsDataList[i].Quality, "", goodsDataList[i].Forge_level, goodsDataList[i].Binding, 0, "", true, 1, "天梯月段位排名奖励", "1900-01-01 12:00:00", 0, goodsDataList[i].BornIndex, goodsDataList[i].Lucky, 0, goodsDataList[i].ExcellenceInfo, goodsDataList[i].AppendPropLev, 0, null, null, 0, true);
						}
					}
				}
				client.sendCmd<int>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000307 RID: 775 RVA: 0x0003556C File Offset: 0x0003376C
		public bool ProcessTianTi5v5GeLogCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				List<TianTi5v5LogItemData> logList = new List<TianTi5v5LogItemData>();
				logList = Global.sendToDB<List<TianTi5v5LogItemData>, int>(3709, client.ClientData.ZhanDuiID, client.ServerId);
				client.sendCmd<List<TianTi5v5LogItemData>>(nID, logList, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000308 RID: 776 RVA: 0x00035620 File Offset: 0x00033820
		public bool ProcessConfirmBattleCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 1;
				if (this.IsGongNengOpened(client, false))
				{
					int zhanDuiID = client.ClientData.ZhanDuiID;
					if (zhanDuiID <= 0)
					{
						result = -4013;
					}
					else if (client.ClientData.ZhanDuiZhiWu != 1)
					{
						result = -4016;
					}
					else
					{
						bool allAccept = false;
						TianTi5v5PiPeiState piPeiState = new TianTi5v5PiPeiState();
						piPeiState.RoleList = new List<TianTi5v5PiPeiRoleState>();
						lock (this.RuntimeData.Mutex)
						{
							result = -2001;
							TimeSpan time = TimeUtil.NowDateTime().TimeOfDay;
							lock (this.RuntimeData.Mutex)
							{
								for (int i = 0; i < this.RuntimeData.TimePoints.Count - 1; i += 2)
								{
									if (time >= this.RuntimeData.TimePoints[i] && time < this.RuntimeData.TimePoints[i + 1])
									{
										result = 1;
										break;
									}
								}
							}
							if (result < 0)
							{
								goto IL_437;
							}
							if (!this.RuntimeData.TeamBattleMap.Contains(client.ClientData.MapCode))
							{
								result = -21;
								goto IL_437;
							}
							TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(zhanDuiID, client.ServerId);
							if (zhanDuiData.teamerList.Count < this.RuntimeData.MinConfirmFightTeamCnt)
							{
								result = -4026;
								goto IL_437;
							}
							foreach (TianTi5v5ZhanDuiRoleData role in zhanDuiData.teamerList)
							{
								TianTi5v5PiPeiRoleState rd = new TianTi5v5PiPeiRoleState
								{
									RoleID = role.RoleID,
									RoleName = role.RoleName,
									Occupation = role.RoleOcc
								};
								GameClient c = GameManager.ClientMgr.FindClient(role.RoleID);
								if (null == c)
								{
									rd.State = 4;
								}
								else if (!this.RuntimeData.TeamBattleMap.Contains(c.ClientData.MapCode))
								{
									rd.State = 3;
								}
								else if (role.RoleID == client.ClientData.RoleID)
								{
									rd.State = 1;
								}
								else
								{
									rd.State = 0;
								}
								piPeiState.RoleList.Add(rd);
							}
							piPeiState.ZhanLi = zhanDuiData.ZhanDouLi;
							piPeiState.GroupIndex = zhanDuiData.DuanWeiId;
							piPeiState.EndTicks = TimeUtil.NOW() + (long)(this.RuntimeData.TeamConfirmTime * 1000);
							this.RuntimeData.ConfirmBattleDict[zhanDuiID] = piPeiState;
							allAccept = piPeiState.RoleList.All((TianTi5v5PiPeiRoleState x) => x.State == 1);
							bool allInline = piPeiState.RoleList.All((TianTi5v5PiPeiRoleState x) => x.State == 1 || x.State == 0);
						}
						foreach (TianTi5v5PiPeiRoleState rd in piPeiState.RoleList)
						{
							TianTi5v5PiPeiRoleState rd;
							GameClient c = GameManager.ClientMgr.FindClient(rd.RoleID);
							if ((c != null && rd.State == 1) || rd.State == 0)
							{
								c.sendCmd<TianTi5v5PiPeiState>(3712, piPeiState, false);
							}
						}
						if (allAccept)
						{
							this.ProcessTianTi5v5JoinCmd(zhanDuiID, piPeiState);
						}
					}
				}
				IL_437:
				client.sendCmd<int>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000309 RID: 777 RVA: 0x00035B4C File Offset: 0x00033D4C
		public bool ProcessAcceptConfirmBattleCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 1;
				int accept = Global.SafeConvertToInt32(cmdParams[1]);
				int zhanDuiID = client.ClientData.ZhanDuiID;
				if (zhanDuiID <= 0)
				{
					result = -4013;
				}
				else
				{
					bool allAccept = false;
					TianTi5v5PiPeiState piPeiState;
					lock (this.RuntimeData.Mutex)
					{
						if (!this.RuntimeData.ConfirmBattleDict.TryGetValue(zhanDuiID, out piPeiState) || piPeiState.EndTicks < TimeUtil.NOW())
						{
							result = -2003;
							goto IL_25E;
						}
						foreach (TianTi5v5PiPeiRoleState rd in piPeiState.RoleList)
						{
							if (rd.RoleID == client.ClientData.RoleID)
							{
								if (!this.RuntimeData.TeamBattleMap.Contains(client.ClientData.MapCode))
								{
									rd.State = 3;
								}
								else if (accept == 1)
								{
									rd.State = 1;
								}
								else
								{
									rd.State = 2;
								}
							}
						}
						allAccept = piPeiState.RoleList.All((TianTi5v5PiPeiRoleState x) => x.State == 1);
						bool anyRefuse = piPeiState.RoleList.Any((TianTi5v5PiPeiRoleState x) => x.State == 2);
						if (anyRefuse)
						{
							piPeiState.State = 2;
						}
					}
					foreach (TianTi5v5PiPeiRoleState rd in piPeiState.RoleList)
					{
						GameClient c = GameManager.ClientMgr.FindClient(rd.RoleID);
						if ((c != null && rd.State == 1) || rd.State == 0)
						{
							c.sendCmd<TianTi5v5PiPeiState>(3712, piPeiState, false);
						}
					}
					if (allAccept)
					{
						this.ProcessTianTi5v5JoinCmd(zhanDuiID, piPeiState);
					}
				}
				IL_25E:
				client.sendCmd<int>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x0600030A RID: 778 RVA: 0x00035E50 File Offset: 0x00034050
		public bool ProcessTianTi5v5EnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x0600030B RID: 779 RVA: 0x00035E94 File Offset: 0x00034094
		public bool ProcessTianTi5v5QuitCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 1;
				int zhanDuiID = client.ClientData.ZhanDuiID;
				if (zhanDuiID <= 0)
				{
					result = -4013;
				}
				else if (client.ClientData.ZhanDuiZhiWu != 1)
				{
					result = -4016;
				}
				else
				{
					TianTi5v5PiPeiState piPeiState;
					lock (this.RuntimeData.Mutex)
					{
						if (!this.RuntimeData.ConfirmBattleDict.TryGetValue(zhanDuiID, out piPeiState) || piPeiState.EndTicks < TimeUtil.NOW())
						{
							result = -2003;
							goto IL_15D;
						}
					}
					result = TianTiClient.getInstance().ZhanDuiRoleChangeState(GameManager.ServerId, zhanDuiID, client.ClientData.RoleID, 0, 0);
					if (result != 3)
					{
						piPeiState.State = 2;
						foreach (TianTi5v5PiPeiRoleState rd in piPeiState.RoleList)
						{
							GameClient c = GameManager.ClientMgr.FindClient(rd.RoleID);
							if (null != c)
							{
								c.sendCmd<int>(nID, 0, false);
							}
						}
						client.ClientData.SignUpGameType = 0;
						return true;
					}
				}
				IL_15D:
				client.sendCmd<int>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x0600030C RID: 780 RVA: 0x00036080 File Offset: 0x00034280
		public bool ProcessQuitFromZhanDui(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					int ret = this.QuitFromZhanDui(client);
					client.sendCmd<int>(nID, ret, false);
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		// Token: 0x0600030D RID: 781 RVA: 0x000360E4 File Offset: 0x000342E4
		public bool ProcessDeleteZhanDui(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					int ret = this.DeleteZhanDui(client);
					client.sendCmd<int>(nID, ret, false);
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		// Token: 0x0600030E RID: 782 RVA: 0x00036148 File Offset: 0x00034348
		public bool ProcessDeleteZhanDuiMember(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					int ret = this.DeleteZhanDuiMember(client, Convert.ToInt32(cmdParams[1]));
					client.sendCmd<int>(nID, ret, false);
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		// Token: 0x0600030F RID: 783 RVA: 0x000361B8 File Offset: 0x000343B8
		public bool ProcessCreateZhanDui(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					string xuanyan = (cmdParams.Length >= 3) ? cmdParams[2] : null;
					int ret = this.CreateZhanDui(client, cmdParams[1], xuanyan);
					client.sendCmd<int>(nID, ret, false);
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		// Token: 0x06000310 RID: 784 RVA: 0x00036234 File Offset: 0x00034434
		public int CreateZhanDui(GameClient client, string teamName, string xuanyan)
		{
			int result;
			if (!this.IsGongNengOpened(client, false))
			{
				result = -400;
			}
			else
			{
				teamName = teamName.Trim();
				if (NameServerNamager.CheckInvalidCharacters(teamName, false) <= 0)
				{
					result = -4027;
				}
				else if (teamName.Length < this.RuntimeData.TeamBattleNameRange[0] || teamName.Length > this.RuntimeData.TeamBattleNameRange[1])
				{
					result = -4027;
				}
				else if (!string.IsNullOrEmpty(xuanyan) && (NameServerNamager.CheckInvalidCharacters(xuanyan, false) <= 0 || xuanyan.Length > 255))
				{
					result = -40;
				}
				else
				{
					lock (this.RuntimeData.Mutex)
					{
						if (0 != Global.AvalidLevel(client, this.RuntimeData.ZhanDuiDengJiTp.Item1, this.RuntimeData.ZhanDuiDengJiTp.Item2, -1, -1))
						{
							return -19;
						}
						if (Global.IsRoleHasEnoughMoney(client, this.RuntimeData.ZhanDuiJinZuan, 40) <= 0)
						{
							return -10;
						}
						TianTi5v5ZhanDuiData pData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
						if (pData != null)
						{
							return -4014;
						}
						pData = this.NewZhanDuiData(client);
						pData.ZhanDuiName = teamName;
						pData.XuanYan = xuanyan;
						pData.LeaderRoleName = Global.FormatRoleName4(client);
						pData.LeaderRoleID = client.ClientData.LocalRoleID;
						pData.ZoneID = client.ClientData.ZoneID;
						int ret = TianTiClient.getInstance().CreateZhanDui(pData);
						if (ret <= 0)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("CreateZhanDui  ErrCode= {0} ,rleid= {1}", ret, client.ClientData.RoleID), null, true);
							return ret;
						}
						int zhanDuiID = ret;
						string strLog = "";
						if (Global.SubRoleMoneyForGoods(client, this.RuntimeData.ZhanDuiJinZuan, 40, "创建战队") <= 0)
						{
							return -10;
						}
						pData.ZhanDuiID = zhanDuiID;
						this.UpdateZhanDuiData2DB(pData, client.ServerId, ZhanDuiDataModeTypes.ZhanDuiInfo);
						client.ClientData.ZhanDuiID = zhanDuiID;
						client.ClientData.ZhanDuiZhiWu = 1;
						this.ChangeRoleZhanDuiID2DB(client.ClientData.RoleID, zhanDuiID, client.ClientData.ZhanDuiZhiWu, client.ServerId);
						EventLogManager.AddCreateZhanDuiEvent(client, (long)zhanDuiID, teamName, strLog);
					}
					this.BroadcastZhanDuiDataChanged(client.ClientData.ZhanDuiID, client.ServerId);
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x06000311 RID: 785 RVA: 0x0003651C File Offset: 0x0003471C
		public bool ProcessAgreeZhanDuiInvite(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					int zhanDuiID = Convert.ToInt32(cmdParams[1]);
					int ret = this.AddMe2ZhanDui(client, zhanDuiID);
					client.sendCmd<int>(nID, ret, false);
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		// Token: 0x06000312 RID: 786 RVA: 0x00036590 File Offset: 0x00034790
		public bool ProcessAgreeZhanDuiRequest(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int otherRoleID = Convert.ToInt32(cmdParams[1]);
				int ret = this.ZhanDuiAddMember(client, otherRoleID);
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					client.sendCmd<int>(nID, ret, false);
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		// Token: 0x06000313 RID: 787 RVA: 0x00036604 File Offset: 0x00034804
		public List<TianTi5v5ZhanDuiMiniData> GetZhanDuiMiniDataList(int maxCount, int serverID)
		{
			AgeDataT<int> requestData = new AgeDataT<int>(this.RuntimeData.ZhanDuiSimpleList.Age, maxCount);
			lock (this.RuntimeData.Mutex)
			{
				requestData.Age = this.RuntimeData.ZhanDuiSimpleList.Age;
			}
			AgeDataT<List<TianTi5v5ZhanDuiMiniData>> result = Global.sendToDB<AgeDataT<List<TianTi5v5ZhanDuiMiniData>>, AgeDataT<int>>(3688, requestData, serverID);
			List<TianTi5v5ZhanDuiMiniData> result2;
			if (result == null || result.V == null)
			{
				result2 = null;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					if (this.RuntimeData.ZhanDuiSimpleList.Age < result.Age)
					{
						this.RuntimeData.ZhanDuiSimpleList.V = result.V;
					}
					result2 = this.RuntimeData.ZhanDuiSimpleList.V;
				}
			}
			return result2;
		}

		// Token: 0x06000314 RID: 788 RVA: 0x00036734 File Offset: 0x00034934
		public TianTi5v5ZhanDuiData GetZhanDuiData(int zhanDuiID, int serverID)
		{
			TianTi5v5ZhanDuiData result2;
			if (zhanDuiID <= 0)
			{
				result2 = null;
			}
			else
			{
				AgeDataT<int> requestData = new AgeDataT<int>(0L, zhanDuiID);
				lock (this.RuntimeData.Mutex)
				{
					AgeDataT<TianTi5v5ZhanDuiData> ageZhanDuiData;
					if (this.RuntimeData.ZhanDuiDataAgeDict.TryGetValue(zhanDuiID, out ageZhanDuiData))
					{
						requestData.Age = ageZhanDuiData.Age;
					}
				}
				AgeDataT<TianTi5v5ZhanDuiData> result = Global.sendToDB<AgeDataT<TianTi5v5ZhanDuiData>, AgeDataT<int>>(3715, requestData, serverID);
				if (result == null)
				{
					result2 = null;
				}
				else
				{
					AgeDataT<TianTi5v5ZhanDuiData> ageZhanDuiData;
					lock (this.RuntimeData.Mutex)
					{
						if (!this.RuntimeData.ZhanDuiDataAgeDict.TryGetValue(zhanDuiID, out ageZhanDuiData) || ageZhanDuiData.Age < result.Age)
						{
							ageZhanDuiData = result;
							this.RuntimeData.ZhanDuiDataAgeDict[zhanDuiID] = result;
						}
					}
					result2 = ageZhanDuiData.V;
				}
			}
			return result2;
		}

		// Token: 0x06000315 RID: 789 RVA: 0x0003687C File Offset: 0x00034A7C
		public TianTi5v5ZhanDuiData UpdateZhanDuiData2DB(TianTi5v5ZhanDuiData zhanDuiData, int serverID, ZhanDuiDataModeTypes ZhanDuiDataModeType)
		{
			int zhanDuiID = zhanDuiData.ZhanDuiID;
			TianTi5v5ZhanDuiData result2;
			if (zhanDuiID <= 0)
			{
				result2 = null;
			}
			else
			{
				AgeDataT<TianTi5v5ZhanDuiData> requestData = new AgeDataT<TianTi5v5ZhanDuiData>(0L, zhanDuiData);
				lock (this.RuntimeData.Mutex)
				{
					AgeDataT<TianTi5v5ZhanDuiData> ageZhanDuiData;
					if (this.RuntimeData.ZhanDuiDataAgeDict.TryGetValue(zhanDuiID, out ageZhanDuiData))
					{
						requestData.Age = ageZhanDuiData.Age;
					}
				}
				zhanDuiData.ZhanDuiDataModeType = (int)ZhanDuiDataModeType;
				AgeDataT<TianTi5v5ZhanDuiData> result = Global.sendToDB<AgeDataT<TianTi5v5ZhanDuiData>, AgeDataT<TianTi5v5ZhanDuiData>>(3716, requestData, serverID);
				if (result == null)
				{
					result2 = null;
				}
				else
				{
					lock (this.RuntimeData.Mutex)
					{
						AgeDataT<TianTi5v5ZhanDuiData> ageZhanDuiData;
						if (!this.RuntimeData.ZhanDuiDataAgeDict.TryGetValue(zhanDuiID, out ageZhanDuiData) || ageZhanDuiData.Age < result.Age)
						{
							this.RuntimeData.ZhanDuiDataAgeDict[zhanDuiID] = result;
						}
						result2 = result.V;
					}
				}
			}
			return result2;
		}

		// Token: 0x06000316 RID: 790 RVA: 0x000369CC File Offset: 0x00034BCC
		public TianTi5v5ZhanDuiData UpdateZorkZhanDuiData2DB(TianTi5v5ZhanDuiData zhanDuiData, int serverID)
		{
			int zhanDuiID = zhanDuiData.ZhanDuiID;
			TianTi5v5ZhanDuiData result2;
			if (zhanDuiID <= 0)
			{
				result2 = null;
			}
			else
			{
				AgeDataT<TianTi5v5ZhanDuiData> requestData = new AgeDataT<TianTi5v5ZhanDuiData>(0L, zhanDuiData);
				lock (this.RuntimeData.Mutex)
				{
					AgeDataT<TianTi5v5ZhanDuiData> ageZhanDuiData;
					if (this.RuntimeData.ZhanDuiDataAgeDict.TryGetValue(zhanDuiID, out ageZhanDuiData))
					{
						requestData.Age = ageZhanDuiData.Age;
					}
				}
				AgeDataT<TianTi5v5ZhanDuiData> result = Global.sendToDB<AgeDataT<TianTi5v5ZhanDuiData>, AgeDataT<TianTi5v5ZhanDuiData>>(3722, requestData, serverID);
				if (result == null)
				{
					result2 = null;
				}
				else
				{
					lock (this.RuntimeData.Mutex)
					{
						AgeDataT<TianTi5v5ZhanDuiData> ageZhanDuiData;
						if (!this.RuntimeData.ZhanDuiDataAgeDict.TryGetValue(zhanDuiID, out ageZhanDuiData) || ageZhanDuiData.Age < result.Age)
						{
							this.RuntimeData.ZhanDuiDataAgeDict[zhanDuiID] = result;
						}
						result2 = result.V;
					}
				}
			}
			return result2;
		}

		// Token: 0x06000317 RID: 791 RVA: 0x00036B18 File Offset: 0x00034D18
		public TianTi5v5ZhanDuiData UpdateEscapeZhanDuiData2DB(TianTi5v5ZhanDuiData zhanDuiData, int serverID)
		{
			int zhanDuiID = zhanDuiData.ZhanDuiID;
			TianTi5v5ZhanDuiData result2;
			if (zhanDuiID <= 0)
			{
				result2 = null;
			}
			else
			{
				AgeDataT<TianTi5v5ZhanDuiData> requestData = new AgeDataT<TianTi5v5ZhanDuiData>(0L, zhanDuiData);
				lock (this.RuntimeData.Mutex)
				{
					AgeDataT<TianTi5v5ZhanDuiData> ageZhanDuiData;
					if (this.RuntimeData.ZhanDuiDataAgeDict.TryGetValue(zhanDuiID, out ageZhanDuiData))
					{
						requestData.Age = ageZhanDuiData.Age;
					}
				}
				AgeDataT<TianTi5v5ZhanDuiData> result = Global.sendToDB<AgeDataT<TianTi5v5ZhanDuiData>, AgeDataT<TianTi5v5ZhanDuiData>>(3723, requestData, serverID);
				if (result == null)
				{
					result2 = null;
				}
				else
				{
					lock (this.RuntimeData.Mutex)
					{
						AgeDataT<TianTi5v5ZhanDuiData> ageZhanDuiData;
						if (!this.RuntimeData.ZhanDuiDataAgeDict.TryGetValue(zhanDuiID, out ageZhanDuiData) || ageZhanDuiData.Age < result.Age)
						{
							this.RuntimeData.ZhanDuiDataAgeDict[zhanDuiID] = result;
						}
						result2 = result.V;
					}
				}
			}
			return result2;
		}

		// Token: 0x06000318 RID: 792 RVA: 0x00036C64 File Offset: 0x00034E64
		public bool DeleteZhanDuiData2DB(int zhanDuiID, int serverID)
		{
			int result = Global.sendToDB<int, int>(3699, zhanDuiID, serverID);
			if (result >= 0)
			{
				lock (this.RuntimeData.Mutex)
				{
					AgeDataT<TianTi5v5ZhanDuiData> ageZhanDuiData;
					if (this.RuntimeData.ZhanDuiDataAgeDict.TryGetValue(zhanDuiID, out ageZhanDuiData))
					{
						ageZhanDuiData.V = null;
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000319 RID: 793 RVA: 0x00036CF8 File Offset: 0x00034EF8
		public void ChangeRoleZhanDuiID2DB(int roleID, int zhanDuiID, int zhiWu, int serverID)
		{
			GameClient client = GameManager.ClientMgr.FindClient(roleID);
			if (client != null)
			{
				client.ClientData.ZhanDuiID = zhanDuiID;
				client.ClientData.ZhanDuiZhiWu = zhiWu;
				client.sendCmd<int[]>(3717, new int[]
				{
					roleID,
					zhanDuiID,
					zhiWu
				}, false);
				GlobalEventSource.getInstance().fireEvent(new EventObject(64, new object[]
				{
					client
				}));
			}
			int result = Global.sendToDB<int, int[]>(3717, new int[]
			{
				roleID,
				zhanDuiID,
				zhiWu
			}, serverID);
		}

		// Token: 0x0600031A RID: 794 RVA: 0x00036D9C File Offset: 0x00034F9C
		public void BroadcastZhanDuiDataChanged(int zhanDuiID, int serverID)
		{
			List<int> list = new List<int>();
			TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(zhanDuiID, serverID);
			if (zhanDuiData != null && null != zhanDuiData.teamerList)
			{
				foreach (TianTi5v5ZhanDuiRoleData role in zhanDuiData.teamerList)
				{
					GameClient client = GameManager.ClientMgr.FindClient(role.RoleID);
					if (null != client)
					{
						client.sendCmd<int>(3718, zhanDuiID, false);
					}
				}
			}
		}

		// Token: 0x0600031B RID: 795 RVA: 0x00036E48 File Offset: 0x00035048
		private void GetZhanDuiRoleState(TianTi5v5ZhanDuiData data)
		{
			if (data != null && null != data.teamerList)
			{
				foreach (TianTi5v5ZhanDuiRoleData role in data.teamerList)
				{
					GameClient c = GameManager.ClientMgr.FindClient(role.RoleID);
					if (null != c)
					{
						role.OnlineState = 1;
					}
					else
					{
						role.OnlineState = 0;
					}
				}
			}
		}

		// Token: 0x0600031C RID: 796 RVA: 0x00036EE0 File Offset: 0x000350E0
		public bool GetMainInfo(GameClient client)
		{
			DateTime now = TimeUtil.NowDateTime();
			this.InitRoleTianTi5v5Data(client);
			TianTi5v5DataAndDayPaiHang data = new TianTi5v5DataAndDayPaiHang();
			lock (this.RuntimeData.Mutex)
			{
				data.TianTi5v5Data = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
				if (!this.RuntimeData.ZhanDuiDataPaiHangList.IsNullOrEmpty<TianTi5v5ZhanDuiData>())
				{
					int count = Math.Min(this.RuntimeData.MinDayPaiMingListCount, this.RuntimeData.ZhanDuiDataPaiHangList.Count);
					data.PaiHangRoleDataList = this.RuntimeData.ZhanDuiDataPaiHangList.GetRange(0, count);
				}
			}
			if (null != data.TianTi5v5Data)
			{
				DuanWeiRankAward duanWeiRankAward = null;
				if (this.CanGetMonthRankAwards(client, out duanWeiRankAward))
				{
					data.HaveMonthPaiHangAwards = 1;
				}
				long lastFightDayCount = Global.GetRoleParamsInt64FromDB(client, "10219");
				int lastFightDayID = (int)(lastFightDayCount / 100000L);
				int lastFightCount = (int)(lastFightDayCount % 100000L);
				int dayId = Global.GetOffsetDay(now);
				if (dayId != lastFightDayID)
				{
					data.TodayFightCount = 0;
				}
				else
				{
					data.TodayFightCount = lastFightCount;
					TianTiDuanWei tianTiDuanWei;
					if (this.RuntimeData.TianTi5v5DuanWeiDict.TryGetValue(data.TianTi5v5Data.DuanWeiId, out tianTiDuanWei))
					{
						data.TodayFightCount = Math.Min(tianTiDuanWei.RongYaoNum, lastFightCount);
					}
				}
			}
			client.sendCmd<TianTi5v5DataAndDayPaiHang>(3680, data, false);
			return true;
		}

		// Token: 0x0600031D RID: 797 RVA: 0x000370AC File Offset: 0x000352AC
		public bool ProcessInviteOther2MyZhanDui(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					List<int> rids = new List<int>();
					if (!this.KuaFuServerOK())
					{
						result = -11000;
					}
					else if (!GlobalEventSource4Scene.getInstance().fireEvent(new PreZhanDuiChangeMemberEventObject(client, client.ClientData.ZhanDuiID, 4), 10000))
					{
						result = -12;
					}
					else if ((string.IsNullOrEmpty(cmdParams[1]) || cmdParams[1] == "0") && cmdParams.Length >= 3)
					{
						int rid = RoleManager.getInstance().GetRoleIDByRoleName(cmdParams[2], client.ServerId);
						if (rid <= 0)
						{
							result = -4030;
						}
						else
						{
							rids.Add(rid);
						}
					}
					else
					{
						foreach (string p in cmdParams[1].Split(new char[]
						{
							'|'
						}))
						{
							int rid = Convert.ToInt32(p);
							if (rid > 0)
							{
								rids.Add(rid);
							}
						}
					}
					using (List<int>.Enumerator enumerator = rids.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							int roleid = enumerator.Current;
							TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
							if (zhanDuiData == null)
							{
								result = -4013;
								break;
							}
							GameClient pOtherClient = GameManager.ClientMgr.FindClient(roleid);
							lock (this.RuntimeData.Mutex)
							{
								if (zhanDuiData.LeaderRoleID != client.ClientData.RoleID)
								{
									result = -4016;
									break;
								}
								if (null != pOtherClient)
								{
									if (pOtherClient.ServerId != client.ServerId)
									{
										result = -4025;
										continue;
									}
									if (0 != Global.AvalidLevel(pOtherClient, this.RuntimeData.LvLimit.Item1, this.RuntimeData.LvLimit.Item2, -1, -1))
									{
										result = -19;
										continue;
									}
									if (null != this.GetZhanDuiData(pOtherClient.ClientData.ZhanDuiID, pOtherClient.ServerId))
									{
										result = -4024;
										continue;
									}
								}
								if (null == zhanDuiData.teamerList)
								{
									result = -3;
									break;
								}
								if (zhanDuiData.teamerList.Count >= this.RuntimeData.MaxTeamCnt)
								{
									result = -4028;
									break;
								}
								if (zhanDuiData.teamerList.Any((TianTi5v5ZhanDuiRoleData x) => x.RoleID == roleid))
								{
									result = -4014;
									continue;
								}
								List<int> inviteList;
								if (!this.RuntimeData.ZhanDuiInviteListDict.TryGetValue(roleid, out inviteList))
								{
									inviteList = new List<int>();
									this.RuntimeData.ZhanDuiInviteListDict[roleid] = inviteList;
								}
								if (inviteList.Contains(zhanDuiData.ZhanDuiID))
								{
									continue;
								}
								inviteList.Add(zhanDuiData.ZhanDuiID);
							}
							if (null != pOtherClient)
							{
								this.SendZhanDuiInviteData(pOtherClient);
							}
						}
					}
				}
				client.sendCmd<int>(nID, result, false);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		// Token: 0x0600031E RID: 798 RVA: 0x00037518 File Offset: 0x00035718
		public bool ProcessRequestToZhanDui(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					int roleID = client.ClientData.RoleID;
					int zhanDuiID = Convert.ToInt32(cmdParams[1]);
					if (!this.KuaFuServerOK())
					{
						result = -11000;
					}
					else
					{
						long nowTicks = TimeUtil.NOW();
						long key = ((long)client.ClientData.RoleID << 32) + (long)zhanDuiID;
						lock (this.RuntimeData.Mutex)
						{
							long lastTicks;
							if (this.RuntimeData.RoleRequestZhanDuiTicksDict.TryGetValue(key, out lastTicks) && lastTicks + 86400000L > nowTicks)
							{
								result = -2007;
								goto IL_324;
							}
						}
						if (!GlobalEventSource4Scene.getInstance().fireEvent(new PreZhanDuiChangeMemberEventObject(client, client.ClientData.ZhanDuiID, 4), 10000))
						{
							result = -12;
						}
						else
						{
							TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
							if (zhanDuiData != null)
							{
								result = -4012;
							}
							else
							{
								zhanDuiData = this.GetZhanDuiData(zhanDuiID, client.ServerId);
								if (zhanDuiData == null)
								{
									result = -4025;
								}
								else if (0 != Global.AvalidLevel(client, this.RuntimeData.LvLimit.Item1, this.RuntimeData.LvLimit.Item2, -1, -1))
								{
									result = -19;
								}
								else if (null == zhanDuiData.teamerList)
								{
									result = -3;
								}
								else if (zhanDuiData.teamerList.Count >= this.RuntimeData.MaxTeamCnt)
								{
									result = -4028;
								}
								else
								{
									TianTi5v5ZhanDuiRoleData roleData = this.NewRoleData(client);
									if (null == roleData)
									{
										result = -15;
									}
									else
									{
										lock (this.RuntimeData.Mutex)
										{
											List<TianTi5v5ZhanDuiRoleData> requestList;
											if (!this.RuntimeData.ZhanDuiRequestListDict.TryGetValue(zhanDuiID, out requestList))
											{
												requestList = new List<TianTi5v5ZhanDuiRoleData>();
												this.RuntimeData.ZhanDuiRequestListDict[zhanDuiID] = requestList;
											}
											if (requestList.Any((TianTi5v5ZhanDuiRoleData x) => x.RoleID == client.ClientData.RoleID))
											{
												goto IL_324;
											}
											requestList.Add(roleData);
											this.RuntimeData.RoleRequestZhanDuiTicksDict[key] = nowTicks;
										}
										GameClient pOtherClient = GameManager.ClientMgr.FindClient(zhanDuiData.LeaderRoleID);
										if (null != pOtherClient)
										{
											result = this.SendZhanDuiRequestData(pOtherClient);
										}
									}
								}
							}
						}
					}
					IL_324:;
				}
				client.sendCmd<int>(nID, result, false);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		// Token: 0x0600031F RID: 799 RVA: 0x000378D4 File Offset: 0x00035AD4
		public int SendZhanDuiInviteData(GameClient client)
		{
			int result;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				result = 0;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.TeamApply.Contains(client.ClientData.MapCode))
					{
						return -4019;
					}
					List<int> inviteList;
					if (!this.RuntimeData.ZhanDuiInviteListDict.TryGetValue(client.ClientData.RoleID, out inviteList))
					{
						return 0;
					}
					try
					{
						if (0 != Global.AvalidLevel(client, this.RuntimeData.LvLimit.Item1, this.RuntimeData.LvLimit.Item2, -1, -1))
						{
							this.RuntimeData.ZhanDuiInviteListDict.Remove(client.ClientData.RoleID);
							return -19;
						}
						if (null != this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId))
						{
							return -4024;
						}
						foreach (int zhanDuiID in inviteList)
						{
							TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(zhanDuiID, client.ServerId);
							client.sendCmd(3689, string.Format("{0}:{1}:{2}", zhanDuiData.ZhanDuiID, zhanDuiData.LeaderRoleName, zhanDuiData.ZhanDuiName), false);
						}
					}
					finally
					{
						inviteList.Clear();
					}
				}
				result = 0;
			}
			return result;
		}

		// Token: 0x06000320 RID: 800 RVA: 0x00037AE0 File Offset: 0x00035CE0
		public int SendZhanDuiRequestData(GameClient client)
		{
			int result;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				result = 0;
			}
			else if (client.ClientData.ZhanDuiID <= 0 || client.ClientData.ZhanDuiZhiWu == 0)
			{
				result = 0;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.TeamApply.Contains(client.ClientData.MapCode))
					{
						return -4019;
					}
					List<TianTi5v5ZhanDuiRoleData> requestList;
					if (!this.RuntimeData.ZhanDuiRequestListDict.TryGetValue(client.ClientData.ZhanDuiID, out requestList))
					{
						return 0;
					}
					TianTi5v5ZhanDuiData pZhanDui = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
					if (pZhanDui == null || pZhanDui.LeaderRoleID != client.ClientData.RoleID)
					{
						return -4013;
					}
					if (pZhanDui.teamerList.Count >= this.RuntimeData.MaxTeamCnt)
					{
						requestList.Clear();
						return -4028;
					}
					foreach (TianTi5v5ZhanDuiRoleData role in requestList)
					{
						if (role.OnlineState == 0)
						{
							role.OnlineState = 1;
							client.sendCmd(3720, string.Format("{0}:{1}:{2}", role.RoleID, role.RoleName, role.RoleOcc), false);
						}
					}
				}
				result = 0;
			}
			return result;
		}

		// Token: 0x06000321 RID: 801 RVA: 0x00037CFC File Offset: 0x00035EFC
		public bool ProcessGetZhanDuiList(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					List<TianTi5v5ZhanDuiMiniData> list = this.GetZhanDuiMiniDataList(this.RuntimeData.MaxZhanDuiNum, client.ServerId);
					client.sendCmd<List<TianTi5v5ZhanDuiMiniData>>(nID, list, false);
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		// Token: 0x06000322 RID: 802 RVA: 0x00037DA0 File Offset: 0x00035FA0
		public int AddMe2ZhanDui(GameClient client, int nTeamID)
		{
			int result;
			if (!GlobalEventSource4Scene.getInstance().fireEvent(new PreZhanDuiChangeMemberEventObject(client, nTeamID, 4), 10000))
			{
				result = -12;
			}
			else if (!this.KuaFuServerOK())
			{
				result = -11000;
			}
			else if (0 != Global.AvalidLevel(client, this.RuntimeData.ZhanDuiDengJiTp.Item1, this.RuntimeData.ZhanDuiDengJiTp.Item2, -1, -1))
			{
				result = -19;
			}
			else
			{
				TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
				if (zhanDuiData != null)
				{
					if (zhanDuiData.ZhanDuiID == nTeamID)
					{
						result = -4014;
					}
					else
					{
						result = -4024;
					}
				}
				else
				{
					zhanDuiData = this.GetZhanDuiData(nTeamID, client.ServerId);
					if (null == zhanDuiData)
					{
						result = -4013;
					}
					else
					{
						TianTi5v5ZhanDuiRoleData roleData = this.NewRoleData(client);
						lock (this.RuntimeData.Mutex)
						{
							zhanDuiData = this.GetZhanDuiData(nTeamID, client.ServerId);
							if (zhanDuiData.teamerList.Count >= this.RuntimeData.MaxTeamCnt)
							{
								return -4028;
							}
							zhanDuiData.teamerList.Add(roleData);
							zhanDuiData.ZhanDouLi = zhanDuiData.teamerList.Sum((TianTi5v5ZhanDuiRoleData x) => x.ZhanLi);
							int ret = TianTiClient.getInstance().UpdateZhanDuiData(zhanDuiData, ZhanDuiDataModeTypes.ZhanDuiInfo);
							if (ret < 0)
							{
								zhanDuiData.teamerList.Remove(roleData);
								LogManager.WriteLog(LogTypes.Error, string.Format("AddMe2ZhanDui ErrCode={0} ,roleid ={1} teamID={2}", ret, client.ClientData.RoleID, nTeamID), null, true);
								return -11000;
							}
							client.ClientData.ZhanDuiID = zhanDuiData.ZhanDuiID;
							client.ClientData.ZhanDuiZhiWu = 0;
							this.ChangeRoleZhanDuiID2DB(client.ClientData.RoleID, nTeamID, 0, client.ServerId);
							this.UpdateZhanDuiData2DB(zhanDuiData, client.ServerId, ZhanDuiDataModeTypes.ZhanDuiInfo);
						}
						if (null != zhanDuiData)
						{
							EventLogManager.AddAttendZhanDuiEvent(client, (long)nTeamID, zhanDuiData.ZhanDuiName, zhanDuiData.LeaderRoleID, string.Join("|", zhanDuiData.teamerList.ConvertAll<string>((TianTi5v5ZhanDuiRoleData x) => x.RoleName)));
						}
						this.BroadcastZhanDuiDataChanged(client.ClientData.ZhanDuiID, client.ServerId);
						result = 0;
					}
				}
			}
			return result;
		}

		// Token: 0x06000323 RID: 803 RVA: 0x00038100 File Offset: 0x00036300
		public int ZhanDuiAddMember(GameClient client, int otherRoleID)
		{
			int result;
			if (!GlobalEventSource4Scene.getInstance().fireEvent(new PreZhanDuiChangeMemberEventObject(client, client.ClientData.ZhanDuiID, 4), 10000))
			{
				result = -12;
			}
			else
			{
				int nTeamID = client.ClientData.ZhanDuiID;
				if (!this.KuaFuServerOK())
				{
					result = -11000;
				}
				else
				{
					TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
					if (null == zhanDuiData)
					{
						result = -4013;
					}
					else
					{
						TianTi5v5ZhanDuiRoleData roleData = null;
						GameClient otherClient = GameManager.ClientMgr.FindClient(otherRoleID);
						if (null != otherClient)
						{
							if (otherClient.ClientData.ZhanDuiID > 0)
							{
								return -4014;
							}
							roleData = this.NewRoleData(otherClient);
						}
						else
						{
							RoleDataEx dbRd = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, otherRoleID), 0);
							if (null == dbRd)
							{
								return -15;
							}
							if (dbRd.ZhanDuiID > 0)
							{
								return -4014;
							}
						}
						lock (this.RuntimeData.Mutex)
						{
							if (client.ClientData.RoleID != zhanDuiData.LeaderRoleID)
							{
								return -4016;
							}
							List<TianTi5v5ZhanDuiRoleData> requestList;
							if (this.RuntimeData.ZhanDuiRequestListDict.TryGetValue(client.ClientData.ZhanDuiID, out requestList))
							{
								if (null == roleData)
								{
									roleData = requestList.Find((TianTi5v5ZhanDuiRoleData x) => x.RoleID == otherRoleID);
								}
								if (null != roleData)
								{
									requestList.RemoveAll((TianTi5v5ZhanDuiRoleData x) => x.RoleID == otherRoleID);
								}
							}
							if (null == roleData)
							{
								return -11003;
							}
							zhanDuiData = this.GetZhanDuiData(nTeamID, client.ServerId);
							if (zhanDuiData.teamerList.Count >= this.RuntimeData.MaxTeamCnt)
							{
								return -4028;
							}
							zhanDuiData.teamerList.Add(roleData);
							zhanDuiData.ZhanDouLi = zhanDuiData.teamerList.Sum((TianTi5v5ZhanDuiRoleData x) => x.ZhanLi);
							int ret = TianTiClient.getInstance().UpdateZhanDuiData(zhanDuiData, ZhanDuiDataModeTypes.ZhanDuiInfo);
							if (ret < 0)
							{
								zhanDuiData.teamerList.Remove(roleData);
								LogManager.WriteLog(LogTypes.Error, string.Format("ZhanDuiAddMember ErrCode={0} ,roleid ={1} teamID={2}", ret, client.ClientData.RoleID, nTeamID), null, true);
								return -11000;
							}
							this.ChangeRoleZhanDuiID2DB(otherRoleID, nTeamID, 0, client.ServerId);
							this.UpdateZhanDuiData2DB(zhanDuiData, client.ServerId, ZhanDuiDataModeTypes.ZhanDuiInfo);
						}
						if (null != zhanDuiData)
						{
							EventLogManager.AddAttendZhanDuiEvent(client, (long)nTeamID, zhanDuiData.ZhanDuiName, zhanDuiData.LeaderRoleID, string.Join("|", zhanDuiData.teamerList.ConvertAll<string>((TianTi5v5ZhanDuiRoleData x) => x.RoleName)));
						}
						this.BroadcastZhanDuiDataChanged(client.ClientData.ZhanDuiID, client.ServerId);
						result = 0;
					}
				}
			}
			return result;
		}

		// Token: 0x06000324 RID: 804 RVA: 0x00038524 File Offset: 0x00036724
		public int QuitFromZhanDui(GameClient client)
		{
			int result;
			if (!this.KuaFuServerOK())
			{
				result = -11000;
			}
			else if (!GlobalEventSource4Scene.getInstance().fireEvent(new PreZhanDuiChangeMemberEventObject(client, client.ClientData.ZhanDuiID, 4), 10000))
			{
				result = -12;
			}
			else
			{
				TianTi5v5ZhanDuiData zhanDuiData = null;
				lock (this.RuntimeData.Mutex)
				{
					zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
					if (zhanDuiData == null)
					{
						return -4013;
					}
					if (zhanDuiData.LeaderRoleID == client.ClientData.RoleID)
					{
						return -4017;
					}
					TianTi5v5ZhanDuiRoleData roleData = zhanDuiData.teamerList.Find((TianTi5v5ZhanDuiRoleData x) => x.RoleID == client.ClientData.RoleID);
					if (null == roleData)
					{
						return -4017;
					}
					this.BroadcastZhanDuiDataChanged(client.ClientData.ZhanDuiID, client.ServerId);
					zhanDuiData.teamerList.Remove(roleData);
					zhanDuiData.ZhanDouLi = zhanDuiData.teamerList.Sum((TianTi5v5ZhanDuiRoleData x) => x.ZhanLi);
					int ret = TianTiClient.getInstance().UpdateZhanDuiData(zhanDuiData, ZhanDuiDataModeTypes.ZhanDuiInfo);
					if (ret < 0)
					{
						zhanDuiData.teamerList.Add(roleData);
						LogManager.WriteLog(LogTypes.Error, string.Format("UpdateZhanDuiData ErrCode={0} ,roleid ={1} teamID={2}", ret, client.ClientData.RoleID, zhanDuiData.ZhanDuiID), null, true);
						return -11000;
					}
					client.ClientData.ZhanDuiID = 0;
					client.ClientData.ZhanDuiZhiWu = 0;
					this.ChangeRoleZhanDuiID2DB(client.ClientData.RoleID, 0, 0, client.ServerId);
					this.UpdateZhanDuiData2DB(zhanDuiData, client.ServerId, ZhanDuiDataModeTypes.ZhanDuiInfo);
				}
				EventLogManager.QuitZhanDuiEvent(client, (long)zhanDuiData.ZhanDuiID, zhanDuiData.ZhanDuiName, zhanDuiData.LeaderRoleID, "");
				result = 0;
			}
			return result;
		}

		// Token: 0x06000325 RID: 805 RVA: 0x00038830 File Offset: 0x00036A30
		public int DeleteZhanDuiMember(GameClient client, int targetRoleID)
		{
			int result;
			if (!this.KuaFuServerOK())
			{
				result = -11000;
			}
			else if (!GlobalEventSource4Scene.getInstance().fireEvent(new PreZhanDuiChangeMemberEventObject(client, client.ClientData.ZhanDuiID, 4), 10000))
			{
				result = -12;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
					if (zhanDuiData == null)
					{
						return -4013;
					}
					if (zhanDuiData.LeaderRoleID != client.ClientData.RoleID)
					{
						return -4016;
					}
					TianTi5v5ZhanDuiRoleData roleData = zhanDuiData.teamerList.Find((TianTi5v5ZhanDuiRoleData x) => x.RoleID == targetRoleID);
					if (null == roleData)
					{
						return -4017;
					}
					this.BroadcastZhanDuiDataChanged(client.ClientData.ZhanDuiID, client.ServerId);
					zhanDuiData.teamerList.Remove(roleData);
					zhanDuiData.ZhanDouLi = zhanDuiData.teamerList.Sum((TianTi5v5ZhanDuiRoleData x) => x.ZhanLi);
					int ret = TianTiClient.getInstance().UpdateZhanDuiData(zhanDuiData, ZhanDuiDataModeTypes.ZhanDuiInfo);
					if (ret < 0)
					{
						zhanDuiData.teamerList.Add(roleData);
						LogManager.WriteLog(LogTypes.Error, string.Format("UpdateZhanDuiData ErrCode={0} ,roleid ={1} teamID={2}", ret, client.ClientData.RoleID, zhanDuiData.ZhanDuiID), null, true);
						return -11000;
					}
					GameClient otherClient = GameManager.ClientMgr.FindClient(targetRoleID);
					if (null != otherClient)
					{
						otherClient.ClientData.ZhanDuiID = 0;
						otherClient.ClientData.ZhanDuiZhiWu = 0;
					}
					this.UpdateZhanDuiData2DB(zhanDuiData, client.ServerId, ZhanDuiDataModeTypes.ZhanDuiInfo);
					this.ChangeRoleZhanDuiID2DB(targetRoleID, 0, 0, client.ServerId);
				}
				result = 0;
			}
			return result;
		}

		// Token: 0x06000326 RID: 806 RVA: 0x00038AA0 File Offset: 0x00036CA0
		public int DeleteZhanDui(GameClient client)
		{
			int result;
			if (!this.KuaFuServerOK())
			{
				result = -11000;
			}
			else if (!GlobalEventSource4Scene.getInstance().fireEvent(new PreZhanDuiChangeMemberEventObject(client, client.ClientData.ZhanDuiID, 0), 10000))
			{
				result = -12;
			}
			else
			{
				TianTi5v5ZhanDuiData zhanDuiData = null;
				lock (this.RuntimeData.Mutex)
				{
					zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
					if (zhanDuiData == null)
					{
						return -4013;
					}
					if (zhanDuiData.LeaderRoleID != client.ClientData.RoleID)
					{
						return -4016;
					}
					int ret = TianTiClient.getInstance().DeleteZhanDui(GameManager.ServerId, client.ClientData.RoleID, zhanDuiData.ZhanDuiID);
					if (ret < 0)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("DeleteZhanDui ErrCode={0} ,roleid ={1} ZhanDuiID={2}", ret, client.ClientData.RoleID, zhanDuiData.ZhanDuiID), null, true);
						return -11000;
					}
					this.BroadcastZhanDuiDataChanged(client.ClientData.ZhanDuiID, client.ServerId);
					if (!this.DeleteZhanDuiData2DB(zhanDuiData.ZhanDuiID, client.ServerId))
					{
						LogManager.WriteLog(LogTypes.Error, "解散战队,本地数据库删除战队数据时失败,zhanduiid=" + zhanDuiData.ZhanDuiID, null, true);
					}
					foreach (TianTi5v5ZhanDuiRoleData role in zhanDuiData.teamerList)
					{
						this.ChangeRoleZhanDuiID2DB(role.RoleID, 0, 0, client.ServerId);
						GameClient c = GameManager.ClientMgr.FindClient(role.RoleID);
						if (null != c)
						{
							c.ClientData.ZhanDuiID = 0;
							c.ClientData.ZhanDuiZhiWu = 0;
						}
					}
				}
				EventLogManager.DeleteZhanDuiEvent(client, (long)zhanDuiData.ZhanDuiID, zhanDuiData.ZhanDuiName, client.ClientData.RoleID, "");
				result = 0;
			}
			return result;
		}

		// Token: 0x06000327 RID: 807 RVA: 0x00038D28 File Offset: 0x00036F28
		public bool ProcessUpdateZhanDuiXuanYan(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					int ret = this.UpdateZhanDuiXuanYan(client, cmdParams[1]);
					client.sendCmd<int>(nID, ret, false);
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		// Token: 0x06000328 RID: 808 RVA: 0x00038D90 File Offset: 0x00036F90
		public bool ProcessGetMyZhanDuiInfo(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					lock (this.RuntimeData.Mutex)
					{
						TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
						if (null != zhanDuiData)
						{
							zhanDuiData = zhanDuiData.Clone();
							this.GetZhanDuiRoleState(zhanDuiData);
						}
						client.sendCmd<TianTi5v5ZhanDuiData>(nID, zhanDuiData, false);
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		// Token: 0x06000329 RID: 809 RVA: 0x00038E88 File Offset: 0x00037088
		public bool ProcessChangeZhanDuiLeader(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int newLeaderId = Convert.ToInt32(cmdParams[1]);
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					int result = 0;
					if (!GlobalEventSource4Scene.getInstance().fireEvent(new PreZhanDuiChangeMemberEventObject(client, client.ClientData.ZhanDuiID, 4), 10000))
					{
						result = -12;
					}
					else
					{
						lock (this.RuntimeData.Mutex)
						{
							TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
							if (zhanDuiData == null)
							{
								result = -4013;
								goto IL_1EF;
							}
							if (zhanDuiData.LeaderRoleID != client.ClientData.RoleID)
							{
								result = -4016;
								goto IL_1EF;
							}
							TianTi5v5ZhanDuiRoleData roleData = zhanDuiData.teamerList.Find((TianTi5v5ZhanDuiRoleData x) => x.RoleID == newLeaderId);
							if (null == roleData)
							{
								result = -4017;
								goto IL_1EF;
							}
							result = TianTiClient.getInstance().UpdateZhanDuiData(zhanDuiData, ZhanDuiDataModeTypes.ZhanDuiInfo);
							if (result < 0)
							{
								goto IL_1EF;
							}
							zhanDuiData.LeaderRoleID = newLeaderId;
							zhanDuiData.LeaderRoleName = Global.FormatRoleName4(client);
							EventLogManager.ChangeZhanDuiLeaderEvent(client, (long)zhanDuiData.ZhanDuiID, zhanDuiData.ZhanDuiName, client.ClientData.RoleID, newLeaderId, "");
							this.UpdateZhanDuiData2DB(zhanDuiData, client.ServerId, ZhanDuiDataModeTypes.ZhanDuiInfo);
							this.ChangeRoleZhanDuiID2DB(newLeaderId, zhanDuiData.ZhanDuiID, 1, client.ServerId);
							this.ChangeRoleZhanDuiID2DB(client.ClientData.RoleID, zhanDuiData.ZhanDuiID, 0, client.ServerId);
						}
						this.BroadcastZhanDuiDataChanged(client.ClientData.ZhanDuiID, client.ServerId);
					}
					IL_1EF:
					client.sendCmd<int>(nID, result, false);
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		// Token: 0x0600032A RID: 810 RVA: 0x000390EC File Offset: 0x000372EC
		public bool ProcessZhanDuiKF5V5JingJiData(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!client.ClientSocket.IsKuaFuLogin)
				{
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		// Token: 0x0600032B RID: 811 RVA: 0x00039140 File Offset: 0x00037340
		public bool InitRoleTianTi5v5Data(GameClient client)
		{
			bool result;
			if (KuaFuManager.KuaFuWorldKuaFuGameServer)
			{
				result = true;
			}
			else
			{
				TianTi5v5ZhanDuiData selfZhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
				if (null == selfZhanDuiData)
				{
					result = true;
				}
				else
				{
					bool rankChanged = false;
					DateTime lastMonth = TimeUtil.NowDateTime().AddMonths(-1);
					lastMonth = new DateTime(lastMonth.Year, lastMonth.Month, 1);
					lock (this.RuntimeData.Mutex)
					{
						this.UpdateByMonth(selfZhanDuiData);
						if (this.RuntimeData.ModifyTime > lastMonth)
						{
							int newRank = this.RuntimeData.MaxPaiMingRank + 1;
							TianTi5v5ZhanDuiData zhanDuiData;
							if (this.RuntimeData.ZhanDuiDataPaiHangDict.TryGetValue(selfZhanDuiData.ZhanDuiID, out zhanDuiData))
							{
								newRank = zhanDuiData.DuanWeiRank;
							}
							if (selfZhanDuiData.DuanWeiRank != newRank)
							{
								rankChanged = true;
								selfZhanDuiData.DuanWeiRank = newRank;
							}
							newRank = this.RuntimeData.MaxPaiMingRank + 1;
							if (this.RuntimeData.ZhanDuiDataMonthPaiHangDict.TryGetValue(selfZhanDuiData.ZhanDuiID, out zhanDuiData))
							{
								newRank = zhanDuiData.DuanWeiRank;
							}
							if (selfZhanDuiData.MonthDuanWeiRank != newRank)
							{
								rankChanged = true;
								selfZhanDuiData.MonthDuanWeiRank = newRank;
							}
						}
						int selfDuanWeiId;
						if (this.RuntimeData.DuanWeiJiFenRangeDuanWeiIdDict.TryGetValue(selfZhanDuiData.DuanWeiJiFen, out selfDuanWeiId))
						{
							selfZhanDuiData.DuanWeiId = selfDuanWeiId;
						}
					}
					result = rankChanged;
				}
			}
			return result;
		}

		// Token: 0x0600032C RID: 812 RVA: 0x00039314 File Offset: 0x00037514
		public bool UpdateByMonth(TianTi5v5ZhanDuiData data)
		{
			DateTime now = DateTime.Now;
			DateTime monthStartDateTime = new DateTime(now.Year, now.Month, 1);
			DateTime lastMonthStartDateTime = monthStartDateTime.AddMonths(-1);
			bool result;
			if (data.LastFightTime < monthStartDateTime)
			{
				data.DuanWeiJiFen = 0;
				data.DuanWeiId = 1;
				data.DuanWeiRank = this.RuntimeData.RankData.MaxPaiMingRank;
				data.LianSheng = 0;
				data.FightCount = 0;
				data.SuccessCount = 0;
				if (data.LastFightTime < lastMonthStartDateTime)
				{
					data.MonthDuanWeiRank = 0;
				}
				data.LastFightTime = monthStartDateTime;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600032D RID: 813 RVA: 0x000393C8 File Offset: 0x000375C8
		public int GetBirthPoint(GameClient client, out int posX, out int posY)
		{
			int side = client.ClientData.BattleWhichSide;
			lock (this.RuntimeData.Mutex)
			{
				TianTiBirthPoint TianTiBirthPoint = null;
				if (this.RuntimeData.MapBirthPointDict.TryGetValue(side, out TianTiBirthPoint))
				{
					posX = TianTiBirthPoint.PosX;
					posY = TianTiBirthPoint.PosY;
					return side;
				}
			}
			posX = 0;
			posY = 0;
			return -1;
		}

		// Token: 0x0600032E RID: 814 RVA: 0x00039464 File Offset: 0x00037664
		public bool CanKuaFuLogin(KuaFuServerLoginData kuaFuServerLoginData)
		{
			int gameID = (int)kuaFuServerLoginData.GameId;
			KuaFu5v5FuBenData fuBenData;
			lock (this.RuntimeData.Mutex)
			{
				TianTi5v5FuBenItem tianTiFuBenItem;
				if (!this.RuntimeData.TianTi5v5FuBenItemDict.TryGetValue(gameID, out tianTiFuBenItem))
				{
					tianTiFuBenItem = new TianTi5v5FuBenItem
					{
						GameId = gameID
					};
					tianTiFuBenItem.FuBenSeqId = GameCoreInterface.getinstance().GetNewFuBenSeqId();
					this.RuntimeData.TianTi5v5FuBenItemDict[tianTiFuBenItem.GameId] = tianTiFuBenItem;
				}
				kuaFuServerLoginData.FuBenSeqId = tianTiFuBenItem.FuBenSeqId;
				this.RuntimeData.FuBenDataDict.TryGetValue(gameID, out fuBenData);
			}
			if (null == fuBenData)
			{
				fuBenData = TianTiClient.getInstance().ZhanDuiGetFuBenData(gameID);
				if (null != fuBenData)
				{
					lock (this.RuntimeData.Mutex)
					{
						this.RuntimeData.FuBenDataDict[gameID] = fuBenData;
					}
				}
			}
			return fuBenData != null && fuBenData.LoginInfo.KuaFuServerId == GameManager.ServerId && fuBenData.RoleDict.ContainsKey(kuaFuServerLoginData.RoleId);
		}

		// Token: 0x0600032F RID: 815 RVA: 0x000395E4 File Offset: 0x000377E4
		public bool OnInitGame(GameClient client)
		{
			KuaFuServerLoginData kuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
			int gameID = (int)kuaFuServerLoginData.GameId;
			KuaFu5v5FuBenData fuBenData;
			lock (this.RuntimeData.Mutex)
			{
				TianTi5v5FuBenItem tianTiFuBenItem;
				if (!this.RuntimeData.TianTi5v5FuBenItemDict.TryGetValue(gameID, out tianTiFuBenItem))
				{
					tianTiFuBenItem = new TianTi5v5FuBenItem
					{
						GameId = gameID
					};
					tianTiFuBenItem.FuBenSeqId = GameCoreInterface.getinstance().GetNewFuBenSeqId();
					this.RuntimeData.TianTi5v5FuBenItemDict[tianTiFuBenItem.GameId] = tianTiFuBenItem;
				}
				kuaFuServerLoginData.FuBenSeqId = tianTiFuBenItem.FuBenSeqId;
				this.RuntimeData.FuBenDataDict.TryGetValue(gameID, out fuBenData);
			}
			if (null == fuBenData)
			{
				fuBenData = TianTiClient.getInstance().ZhanDuiGetFuBenData(gameID);
				if (null != fuBenData)
				{
					lock (this.RuntimeData.Mutex)
					{
						this.RuntimeData.FuBenDataDict[gameID] = fuBenData;
					}
				}
			}
			bool result;
			if (fuBenData == null || fuBenData.State >= GameFuBenState.End)
			{
				result = false;
			}
			else
			{
				KuaFuFuBenRoleData role;
				if (fuBenData.RoleDict.TryGetValue(client.ClientData.RoleID, out role))
				{
					client.ClientData.BattleWhichSide = role.Side;
				}
				int posX;
				int posY;
				int side = this.GetBirthPoint(client, out posX, out posY);
				if (side <= 0)
				{
					result = false;
				}
				else
				{
					int index = (int)kuaFuServerLoginData.GameId % this.RuntimeData.MapCodeList.Count;
					client.ClientData.MapCode = this.RuntimeData.MapCodeList[index];
					client.ClientData.PosX = posX;
					client.ClientData.PosY = posY;
					int fuBenSeq = 0;
					lock (this.RuntimeData.Mutex)
					{
						if (!this.RuntimeData.GameId2FuBenSeq.TryGetValue((int)kuaFuServerLoginData.GameId, out fuBenSeq))
						{
							fuBenSeq = GameCoreInterface.getinstance().GetNewFuBenSeqId();
							this.RuntimeData.GameId2FuBenSeq[(int)kuaFuServerLoginData.GameId] = fuBenSeq;
						}
					}
					kuaFuServerLoginData.FuBenSeqId = fuBenSeq;
					client.ClientData.FuBenSeqID = kuaFuServerLoginData.FuBenSeqId;
					client.SceneType = 55;
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06000330 RID: 816 RVA: 0x000398B8 File Offset: 0x00037AB8
		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return GlobalNew.IsGongNengOpened(client, GongNengIDs.TianTi5v5, hint);
		}

		// Token: 0x06000331 RID: 817 RVA: 0x00039908 File Offset: 0x00037B08
		public bool CanGetMonthRankAwards(GameClient client, out DuanWeiRankAward duanWeiRankAward)
		{
			duanWeiRankAward = null;
			bool result;
			if (TimeUtil.GetOffsetMonth(this.RuntimeData.RankData.ModifyTime) != TimeUtil.GetOffsetMonth(TimeUtil.NowDateTime()))
			{
				result = false;
			}
			else
			{
				TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
				lock (this.RuntimeData.Mutex)
				{
					if (zhanDuiData != null && this.RuntimeData.ZhanDuiDataMonthPaiHangDict != null)
					{
						int monthDuanWeiRank = this.RuntimeData.RankData.MaxPaiMingRank;
						TianTi5v5ZhanDuiData zhanDuiMonthData;
						if (this.RuntimeData.ZhanDuiDataMonthPaiHangDict.TryGetValue(zhanDuiData.ZhanDuiID, out zhanDuiMonthData))
						{
							monthDuanWeiRank = zhanDuiMonthData.MonthDuanWeiRank;
						}
						if (this.RuntimeData.DuanWeiRankAwardDict.TryGetValue(monthDuanWeiRank, out duanWeiRankAward))
						{
							DateTime fetchTime = Global.GetRoleParamsDateTimeFromDB(client, "10220");
							DateTime now = TimeUtil.NowDateTime();
							if (fetchTime.Month == now.Month && fetchTime.Year == now.Year)
							{
								return false;
							}
							if (new DateTime(fetchTime.Year, fetchTime.Month, 1) >= new DateTime(this.RuntimeData.ModifyTime.Year, this.RuntimeData.ModifyTime.Month, 1))
							{
								return false;
							}
							TianTi5v5ZhanDuiRoleData role = zhanDuiData.teamerList.Find((TianTi5v5ZhanDuiRoleData x) => x.RoleID == client.ClientData.RoleID);
							if (role == null)
							{
								return false;
							}
							int monthDayID = TimeUtil.GetOffsetMonthDayID(TimeUtil.NowDateTime().AddMonths(-1));
							int[] arr = role.MonthFightCounts;
							if (arr == null || arr.Length != 4)
							{
								return false;
							}
							int lastMonthFightCount = 0;
							if (arr[0] == monthDayID)
							{
								lastMonthFightCount = arr[1];
							}
							else if (arr[2] == monthDayID)
							{
								lastMonthFightCount = arr[3];
							}
							if (lastMonthFightCount < this.RuntimeData.TeamAwardLimit)
							{
								return false;
							}
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06000332 RID: 818 RVA: 0x00039BCC File Offset: 0x00037DCC
		public bool KuaFuServerOK()
		{
			return TianTiClient.getInstance().IsKuaFuServerOK();
		}

		// Token: 0x06000333 RID: 819 RVA: 0x00039BE8 File Offset: 0x00037DE8
		public int UpdateZhanDuiXuanYan(GameClient client, string xuanYan)
		{
			int result2;
			if (!this.KuaFuServerOK())
			{
				result2 = -11000;
			}
			else
			{
				xuanYan = xuanYan.Trim();
				if (NameServerNamager.CheckInvalidCharacters(xuanYan, false) <= 0)
				{
					result2 = -40;
				}
				else
				{
					TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
					lock (this.RuntimeData.Mutex)
					{
						if (zhanDuiData == null)
						{
							return -4013;
						}
						if (zhanDuiData.LeaderRoleID != client.ClientData.RoleID)
						{
							return -4016;
						}
						int result = TianTiClient.getInstance().UpdateZhanDuiXuanYan((long)zhanDuiData.ZhanDuiID, xuanYan);
						if (result < 0)
						{
							return result;
						}
						zhanDuiData.XuanYan = xuanYan;
						this.UpdateZhanDuiData2DB(zhanDuiData, client.ServerId, ZhanDuiDataModeTypes.ZhanDuiInfo);
					}
					result2 = 0;
				}
			}
			return result2;
		}

		// Token: 0x06000334 RID: 820 RVA: 0x00039D38 File Offset: 0x00037F38
		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == SceneUIClasses.TianTi5v5)
			{
				int fuBenSeqId = copyMap.FuBenSeqID;
				int mapCode = copyMap.MapCode;
				lock (this.RuntimeData.Mutex)
				{
					TianTi5v5Scene scene = null;
					if (!this.TianTi5v5SceneDict.TryGetValue(fuBenSeqId, out scene))
					{
						scene = new TianTi5v5Scene();
						scene.CopyMap = copyMap;
						scene.CleanAllInfo();
						scene.GameId = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
						scene.m_nMapCode = mapCode;
						scene.CopyMapId = copyMap.CopyMapID;
						scene.FuBenSeqId = fuBenSeqId;
						scene.m_nPlarerCount = 1;
						KuaFu5v5FuBenData fuBenData;
						if (this.RuntimeData.FuBenDataDict.TryGetValue(scene.GameId, out fuBenData))
						{
							scene.FuBenData = fuBenData;
						}
						this.TianTi5v5SceneDict[fuBenSeqId] = scene;
					}
					else
					{
						scene.m_nPlarerCount++;
					}
					copyMap.IsKuaFuCopy = true;
					this.SaveClientBattleSide(scene, client);
					copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(this.RuntimeData.TotalSecs * 1000));
					scene.RoleSideStateDict[client.ClientData.RoleID] = new Tuple<int, bool>(client.ClientData.BattleWhichSide, true);
					scene.ClientDict[client.ClientData.RoleID] = client;
					if (!scene.ZhanDuiDataDict.Any((Tuple<TianTi5v5ZhanDuiData, int> x) => x.Item1.ZhanDuiID == client.ClientData.ZhanDuiID))
					{
						TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
						this.UpdateByMonth(zhanDuiData);
						scene.ZhanDuiDataDict.Add(new Tuple<TianTi5v5ZhanDuiData, int>(zhanDuiData, client.ServerId));
					}
				}
				TianTiClient.getInstance().ZhanDuiRoleChangeState(GameManager.ServerId, client.ClientData.ZhanDuiID, client.ClientData.RoleID, 5, 0);
				GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 0, 1, 0, 0, 34);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000335 RID: 821 RVA: 0x00039FFC File Offset: 0x000381FC
		public bool RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == SceneUIClasses.TianTi5v5)
			{
				lock (this.RuntimeData.Mutex)
				{
					TianTi5v5Scene scene;
					if (this.TianTi5v5SceneDict.TryRemove(copyMap.FuBenSeqID, out scene))
					{
						this.RuntimeData.GameId2FuBenSeq.Remove(scene.GameId);
					}
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000336 RID: 822 RVA: 0x0003A094 File Offset: 0x00038294
		private void SaveClientBattleSide(TianTi5v5Scene scene, GameClient client)
		{
			TianTi5v5RoleMiniData tianTiRoleMiniData;
			if (!scene.RoleIdDuanWeiIdDict.TryGetValue(client.ClientData.ZhanDuiID, out tianTiRoleMiniData))
			{
				tianTiRoleMiniData = new TianTi5v5RoleMiniData();
				scene.RoleIdDuanWeiIdDict[client.ClientData.ZhanDuiID] = tianTiRoleMiniData;
				tianTiRoleMiniData.RoleId = client.ClientData.ZhanDuiID;
			}
			tianTiRoleMiniData.BattleWitchSide = client.ClientData.BattleWhichSide;
			TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
			if (null != zhanDuiData)
			{
				tianTiRoleMiniData.DuanWeiId = zhanDuiData.DuanWeiId;
				tianTiRoleMiniData.RoleName = zhanDuiData.ZhanDuiName;
				if (client.ClientData.RoleID == zhanDuiData.LeaderRoleID)
				{
					tianTiRoleMiniData.ZoneId = client.ClientData.ZoneID;
				}
			}
		}

		// Token: 0x06000337 RID: 823 RVA: 0x0003A168 File Offset: 0x00038368
		private TianTi5v5RoleMiniData GetEnemyBattleSide(TianTi5v5Scene scene, GameClient client)
		{
			foreach (KeyValuePair<int, TianTi5v5RoleMiniData> kv in scene.RoleIdDuanWeiIdDict)
			{
				if (client.ClientData.ZhanDuiID != kv.Key)
				{
					return kv.Value;
				}
			}
			return scene.RoleIdDuanWeiIdDict.Values.FirstOrDefault<TianTi5v5RoleMiniData>();
		}

		// Token: 0x06000338 RID: 824 RVA: 0x0003A214 File Offset: 0x00038414
		public void TimerProc()
		{
			long nowTicks = TimeUtil.NOW();
			if (nowTicks >= TianTi5v5Manager.NextHeartBeatTicks)
			{
				TianTi5v5Manager.NextHeartBeatTicks = nowTicks + 1020L;
				foreach (TianTi5v5Scene scene in this.TianTi5v5SceneDict.Values)
				{
					lock (this.RuntimeData.Mutex)
					{
						int nID = scene.FuBenSeqId;
						int nCopyID = scene.CopyMapId;
						int nMapCodeID = scene.m_nMapCode;
						if (nID >= 0 && nCopyID >= 0 && nMapCodeID >= 0)
						{
							CopyMap copyMap = scene.CopyMap;
							DateTime now = TimeUtil.NowDateTime();
							long ticks = TimeUtil.NOW();
							scene.ScoreInfoData.Count1 = 0L;
							scene.ScoreInfoData.Count2 = 0;
							List<KeyValuePair<int, Tuple<int, bool>>> updateList = new List<KeyValuePair<int, Tuple<int, bool>>>();
							foreach (int rid in scene.RoleSideStateDict.Keys)
							{
								Tuple<int, bool> v = scene.RoleSideStateDict[rid];
								GameClient gc = GameManager.ClientMgr.FindClient(rid);
								if (null == gc)
								{
									if (v.Item2)
									{
										updateList.Add(new KeyValuePair<int, Tuple<int, bool>>(rid, new Tuple<int, bool>(v.Item1, false)));
									}
								}
								else if (!gc.ClientData.FirstPlayStart)
								{
									if (v.Item1 == 1)
									{
										scene.ScoreInfoData.Count1 += 1L;
									}
									else if (v.Item1 == 2)
									{
										scene.ScoreInfoData.Count2++;
									}
								}
							}
							foreach (KeyValuePair<int, Tuple<int, bool>> kv in updateList)
							{
								scene.RoleSideStateDict[kv.Key] = kv.Value;
							}
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<KuaFu5v5ScoreInfoData>(3708, scene.ScoreInfoData, copyMap);
							if (scene.m_eStatus == GameSceneStatuses.STATUS_NULL)
							{
								scene.m_lPrepareTime = ticks;
								scene.m_lBeginTime = ticks + (long)(this.RuntimeData.WaitingEnterSecs * 1000);
								scene.m_eStatus = GameSceneStatuses.STATUS_PREPARE;
								scene.StateTimeData.GameType = 34;
								scene.StateTimeData.State = (int)scene.m_eStatus;
								scene.StateTimeData.EndTicks = scene.m_lBeginTime;
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_PREPARE)
							{
								bool gotoNextStep = false;
								if (ticks >= scene.m_lBeginTime)
								{
									gotoNextStep = true;
								}
								else
								{
									bool flag2;
									if (scene.RoleSideStateDict.Count >= scene.FuBenData.RoleDict.Count)
									{
										flag2 = !copyMap.GetClientsList().All((GameClient x) => !x.ClientData.FirstPlayStart);
									}
									else
									{
										flag2 = true;
									}
									if (!flag2)
									{
										gotoNextStep = true;
									}
								}
								if (gotoNextStep)
								{
									scene.m_lBeginTime = ticks;
									scene.m_eStatus = GameSceneStatuses.STATUS_BEGIN;
									scene.m_lEndTime = ticks + (long)(this.RuntimeData.FightingSecs * 1000);
									scene.StateTimeData.GameType = 34;
									scene.StateTimeData.State = (int)scene.m_eStatus;
									scene.StateTimeData.EndTicks = scene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
									copyMap.AddGuangMuEvent(1, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(copyMap.MapCode, copyMap.CopyMapID, 1, 0);
									copyMap.AddGuangMuEvent(2, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(copyMap.MapCode, copyMap.CopyMapID, 2, 0);
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
							{
								if (ticks >= scene.m_lEndTime)
								{
									this.CompleteTianTi5v5Scene(scene, 0);
								}
								else if (ticks - scene.m_lBeginTime > 1000L)
								{
									this.SceneCheckComplete(scene, true);
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_END)
							{
								this.ProcessEnd(scene, now, nowTicks);
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_AWARD)
							{
								if (ticks >= scene.m_lLeaveTime)
								{
									copyMap.SetRemoveTicks(scene.m_lLeaveTime);
									scene.m_eStatus = GameSceneStatuses.STATUS_CLEAR;
									try
									{
										List<GameClient> objsList = copyMap.GetClientsList();
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
										DataHelper.WriteExceptionLogEx(ex, "跨服组队竞技清场调度异常");
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000339 RID: 825 RVA: 0x0003A854 File Offset: 0x00038A54
		public void NotifyTimeStateInfoAndScoreInfo(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				TianTi5v5Scene scene;
				if (this.TianTi5v5SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					client.sendCmd<GameSceneStateTimeData>(827, scene.StateTimeData, false);
				}
			}
		}

		// Token: 0x0600033A RID: 826 RVA: 0x0003A8D4 File Offset: 0x00038AD4
		public void CompleteTianTi5v5Scene(TianTi5v5Scene scene, int successSide)
		{
			scene.m_eStatus = GameSceneStatuses.STATUS_END;
			scene.SuccessSide = successSide;
		}

		// Token: 0x0600033B RID: 827 RVA: 0x0003A8E8 File Offset: 0x00038AE8
		public void CancleTianTi5v5Scene(int gameId)
		{
			lock (this.RuntimeData.Mutex)
			{
				this.CancledGameIdDict.Add(gameId);
			}
		}

		// Token: 0x0600033C RID: 828 RVA: 0x0003A974 File Offset: 0x00038B74
		private int SceneCheckComplete(TianTi5v5Scene scene, bool complete = true)
		{
			int side = 0;
			if (scene.RoleSideStateDict.Count > 0)
			{
				int existsSide = scene.RoleSideStateDict.First<KeyValuePair<int, Tuple<int, bool>>>().Value.Item1;
				if (scene.RoleSideStateDict.All((KeyValuePair<int, Tuple<int, bool>> x) => x.Value.Item1 == existsSide))
				{
					side = existsSide;
				}
			}
			else
			{
				side = scene.LastLeaveZhanDuiID;
			}
			if (side != 0 && complete)
			{
				this.CompleteTianTi5v5Scene(scene, side);
			}
			return side;
		}

		// Token: 0x0600033D RID: 829 RVA: 0x0003AA14 File Offset: 0x00038C14
		private void SceneRemoveRole(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				TianTi5v5Scene scene;
				if (this.TianTi5v5SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					if (scene.m_eStatus < GameSceneStatuses.STATUS_END)
					{
						scene.RoleSideStateDict.Remove(client.ClientData.RoleID);
						if (scene.RoleSideStateDict.Count == 0)
						{
							scene.LastLeaveZhanDuiID = client.ClientData.BattleWhichSide;
						}
					}
				}
			}
		}

		// Token: 0x0600033E RID: 830 RVA: 0x0003AAD0 File Offset: 0x00038CD0
		public void OnKillRole(GameClient client, GameClient other)
		{
			if (client.SceneType == 55)
			{
				this.SceneRemoveRole(other);
				GameManager.ClientMgr.ChangePosition(TCPManager.getInstance().MySocketListener, TCPManager.getInstance().TcpOutPacketPool, other, this.RuntimeData.TeamBattleWatch[0], this.RuntimeData.TeamBattleWatch[1], 4, 159, 0);
			}
		}

		// Token: 0x0600033F RID: 831 RVA: 0x0003AB3C File Offset: 0x00038D3C
		public void RoleLeaveFuBen(GameClient client)
		{
			if (client.SceneType == 55)
			{
				this.SceneRemoveRole(client);
			}
		}

		// Token: 0x06000340 RID: 832 RVA: 0x0003AB68 File Offset: 0x00038D68
		private void ProcessEnd(TianTi5v5Scene scene, DateTime now, long nowTicks)
		{
			scene.m_eStatus = GameSceneStatuses.STATUS_AWARD;
			scene.m_lEndTime = nowTicks;
			scene.m_lLeaveTime = scene.m_lEndTime + (long)(this.RuntimeData.ClearRolesSecs * 1000);
			TianTiClient.getInstance().ZhanDuiRoleChangeState(GameManager.ServerId, 0, 0, 3, scene.GameId);
			scene.StateTimeData.GameType = 34;
			scene.StateTimeData.State = 3;
			scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
			GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
			if (scene.SuccessSide == -1)
			{
				this.GameCanceled(scene);
			}
			else
			{
				this.GiveAwards(scene);
			}
		}

		// Token: 0x06000341 RID: 833 RVA: 0x0003ACD4 File Offset: 0x00038ED4
		public void GiveAwards(TianTi5v5Scene scene)
		{
			try
			{
				DateTime now = TimeUtil.NowDateTime();
				DateTime startTime = now.Subtract(this.RuntimeData.RefreshTime);
				List<GameClient> objsList = scene.ClientDict.Values.ToList<GameClient>();
				HashSet<int> processedZhanDuiHashSet = new HashSet<int>();
				if (objsList != null && objsList.Count > 0)
				{
					int nowDayId = Global.GetOffsetDayNow();
					int i = 0;
					while (i < objsList.Count)
					{
						GameClient client = objsList[i];
						if (client != null)
						{
							bool online = false;
							GameClient c = GameManager.ClientMgr.FindClient(client.ClientData.RoleID);
							if (c != null && c.SceneType == 55)
							{
								online = true;
							}
							Tuple<TianTi5v5ZhanDuiData, int> tp = scene.ZhanDuiDataDict.Find((Tuple<TianTi5v5ZhanDuiData, int> x) => x.Item1.ZhanDuiID == client.ClientData.ZhanDuiID);
							if (tp != null)
							{
								TianTi5v5ZhanDuiData zhanDuiData = tp.Item1;
								zhanDuiData = this.GetZhanDuiData(zhanDuiData.ZhanDuiID, tp.Item2);
								if (null != zhanDuiData)
								{
									int index = scene.ZhanDuiDataDict.FindIndex((Tuple<TianTi5v5ZhanDuiData, int> x) => x.Item1.ZhanDuiID == client.ClientData.ZhanDuiID);
									scene.ZhanDuiDataDict[index] = new Tuple<TianTi5v5ZhanDuiData, int>(zhanDuiData, tp.Item2);
									bool success = client.ClientData.BattleWhichSide == scene.SuccessSide;
									if (success)
									{
										int successZhanDuiID = client.ClientData.ZhanDuiID;
									}
									int selfDuanWeiId = zhanDuiData.DuanWeiId;
									TianTi5v5RoleMiniData enemyMiniData = this.GetEnemyBattleSide(scene, client);
									int addDuanWeiJiFen = 0;
									int addLianShengJiFen = 0;
									int addRongYao = 0;
									bool processZhanDuiData = processedZhanDuiHashSet.Add(client.ClientData.ZhanDuiID);
									long lastFightDayCount = Global.GetRoleParamsInt64FromDB(client, "10219");
									int lastFightDayID = (int)(lastFightDayCount / 100000L);
									int lastFightCount = (int)(lastFightDayCount % 100000L);
									int dayId = Global.GetOffsetDay(startTime);
									if (dayId != lastFightDayID)
									{
										lastFightDayID = dayId;
										lastFightCount = 1;
									}
									else
									{
										lastFightCount++;
									}
									lastFightDayCount = (long)lastFightCount + 100000L * (long)lastFightDayID;
									Global.SaveRoleParamsInt64ValueToDB(client, "10219", lastFightDayCount, true);
									if (success)
									{
										if (processZhanDuiData)
										{
											zhanDuiData.LianSheng++;
											zhanDuiData.SuccessCount++;
										}
										TianTiDuanWei tianTiDuanWei;
										if (this.RuntimeData.TianTi5v5DuanWeiDict.TryGetValue(enemyMiniData.DuanWeiId, out tianTiDuanWei))
										{
											addDuanWeiJiFen = tianTiDuanWei.WinJiFen;
											addLianShengJiFen = (int)((double)tianTiDuanWei.WinJiFen * Math.Min(2.0, (double)(zhanDuiData.LianSheng - 1) * 0.2));
											if (lastFightCount <= tianTiDuanWei.RongYaoNum)
											{
												addRongYao = tianTiDuanWei.WinRongYu;
											}
										}
									}
									else
									{
										if (processZhanDuiData)
										{
											zhanDuiData.LianSheng = 0;
										}
										TianTiDuanWei tianTiDuanWei;
										if (this.RuntimeData.TianTi5v5DuanWeiDict.TryGetValue(zhanDuiData.DuanWeiId, out tianTiDuanWei))
										{
											addDuanWeiJiFen = tianTiDuanWei.LoseJiFen;
											if (lastFightCount <= tianTiDuanWei.RongYaoNum)
											{
												addRongYao = tianTiDuanWei.LoseRongYu;
											}
										}
									}
									if (addDuanWeiJiFen != 0 && processZhanDuiData)
									{
										zhanDuiData.DuanWeiJiFen += addDuanWeiJiFen + addLianShengJiFen;
										zhanDuiData.DuanWeiJiFen = Math.Max(0, zhanDuiData.DuanWeiJiFen);
									}
									if (processZhanDuiData)
									{
										zhanDuiData.FightCount++;
										if (this.RuntimeData.DuanWeiJiFenRangeDuanWeiIdDict.TryGetValue(zhanDuiData.DuanWeiJiFen, out selfDuanWeiId))
										{
											zhanDuiData.DuanWeiId = selfDuanWeiId;
										}
										TianTi5v5LogItemData tianTiLogItemData = new TianTi5v5LogItemData
										{
											Success = (success ? 1 : 0),
											ZoneId1 = zhanDuiData.ZoneID,
											RoleName1 = zhanDuiData.ZhanDuiName,
											ZoneId2 = enemyMiniData.ZoneId,
											RoleName2 = enemyMiniData.RoleName,
											DuanWeiJiFenAward = addDuanWeiJiFen + addLianShengJiFen,
											RongYaoAward = addRongYao,
											RoleId = zhanDuiData.ZhanDuiID,
											EndTime = now
										};
										Global.sendToDB<int, TianTi5v5LogItemData>(3670, tianTiLogItemData, client.ServerId);
									}
									TianTi5v5AwardsData awardsData = new TianTi5v5AwardsData();
									awardsData.DuanWeiJiFen = addDuanWeiJiFen;
									awardsData.LianShengJiFen = addLianShengJiFen;
									awardsData.RongYao = addRongYao;
									awardsData.DuanWeiId = zhanDuiData.DuanWeiId;
									if (success)
									{
										awardsData.Success = 1;
										GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 0, 0, 1, 0, 34);
									}
									else
									{
										GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 0, 0, 0, 1, 34);
									}
									if (online)
									{
										if (addRongYao != 0)
										{
											GameManager.ClientMgr.ModifyTeamRongYaoValue(c, addRongYao, "组队竞技获得荣耀", true);
										}
										c.sendCmd<TianTi5v5AwardsData>(3710, awardsData, false);
									}
									TianTi5v5ZhanDuiRoleData role = zhanDuiData.teamerList.Find((TianTi5v5ZhanDuiRoleData x) => x.RoleID == client.ClientData.RoleID);
									if (null != role)
									{
										int monthDayID0 = TimeUtil.GetOffsetMonthDayID(TimeUtil.NowDateTime());
										int monthDayID = TimeUtil.GetOffsetMonthDayID(TimeUtil.NowDateTime().AddMonths(-1));
										int[] arr = role.MonthFightCounts;
										if (arr == null || arr.Length != 4)
										{
											arr = new int[4];
											role.MonthFightCounts = arr;
										}
										if (arr[0] == monthDayID)
										{
											arr[2] = arr[0];
											arr[3] = arr[1];
											arr[0] = monthDayID0;
											arr[1] = 1;
										}
										else
										{
											arr[0] = monthDayID0;
											arr[1]++;
										}
										role.MonthFigntCount++;
										role.ZhanLi = (long)client.ClientData.CombatForce;
										role.RoleName = client.ClientData.RoleName;
										role.RoleOcc = client.ClientData.Occupation;
										role.ZhuanSheng = client.ClientData.ChangeLifeCount;
										role.Level = client.ClientData.Level;
										role.RebornLevel = client.ClientData.RebornLevel;
										RoleData4Selector roleInfo = Global.sendToDB<RoleData4Selector, string>(512, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
										if (roleInfo != null || roleInfo.RoleID < 0)
										{
											role.ModelData = DataHelper.ObjectToBytes<RoleData4Selector>(roleInfo);
										}
									}
								}
							}
						}
						IL_788:
						i++;
						continue;
						goto IL_788;
					}
				}
				foreach (Tuple<TianTi5v5ZhanDuiData, int> tp in scene.ZhanDuiDataDict)
				{
					Tuple<TianTi5v5ZhanDuiData, int> tp;
					TianTi5v5ZhanDuiData data = tp.Item1;
					data.ZhanDouLi = data.teamerList.Sum((TianTi5v5ZhanDuiRoleData x) => x.ZhanLi);
					data.LastFightTime = TimeUtil.NowDateTime();
					this.UpdateZhanDuiData2DB(data, tp.Item2, ZhanDuiDataModeTypes.TianTiFightData);
					TianTiClient.getInstance().UpdateZhanDuiData(data, ZhanDuiDataModeTypes.TianTiFightData);
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "组队竞技清场调度异常");
			}
		}

		// Token: 0x06000342 RID: 834 RVA: 0x0003B570 File Offset: 0x00039770
		public void GameCanceled(TianTi5v5Scene scene)
		{
			try
			{
				List<GameClient> objsList = scene.CopyMap.GetClientsList();
				if (objsList != null && objsList.Count > 0)
				{
					for (int i = 0; i < objsList.Count; i++)
					{
						GameClient client = objsList[i];
						if (client != null && client == GameManager.ClientMgr.FindClient(client.ClientData.RoleID))
						{
							client.sendCmd<TianTi5v5AwardsData>(3710, new TianTi5v5AwardsData
							{
								Success = -1
							}, false);
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "组队竞技清场调度异常");
			}
		}

		// Token: 0x0400045B RID: 1115
		public const SceneUIClasses ManagerType = SceneUIClasses.TianTi5v5;

		// Token: 0x0400045C RID: 1116
		private static TianTi5v5Manager instance = new TianTi5v5Manager();

		// Token: 0x0400045D RID: 1117
		public TianTi5v5Data RuntimeData = new TianTi5v5Data();

		// Token: 0x0400045E RID: 1118
		public ConcurrentDictionary<int, TianTi5v5Scene> TianTi5v5SceneDict = new ConcurrentDictionary<int, TianTi5v5Scene>();

		// Token: 0x0400045F RID: 1119
		public HashSet<int> CancledGameIdDict = new HashSet<int>();

		// Token: 0x04000460 RID: 1120
		private static long NextHeartBeatTicks = 0L;

		// Token: 0x020000B9 RID: 185
		[ProtoContract]
		private class TianTi5v5ZhanDuiDataList : List<TianTi5v5ZhanDuiData>, ICompressed
		{
			// Token: 0x06000352 RID: 850 RVA: 0x0003B671 File Offset: 0x00039871
			public TianTi5v5ZhanDuiDataList()
			{
			}

			// Token: 0x06000353 RID: 851 RVA: 0x0003B67C File Offset: 0x0003987C
			public TianTi5v5ZhanDuiDataList(List<TianTi5v5ZhanDuiData> list) : base(list)
			{
			}
		}
	}
}
