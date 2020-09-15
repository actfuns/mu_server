using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Interface;
using GameServer.Logic.Ornament;
using GameServer.Server;
using GameServer.Tools;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic.Marriage.CoupleArena
{
	// Token: 0x0200035F RID: 863
	public class CoupleArenaManager : SingletonTemplate<CoupleArenaManager>, IManager, ICmdProcessorEx, ICmdProcessor, IEventListenerEx, IEventListener
	{
		// Token: 0x06000EB4 RID: 3764 RVA: 0x000E6D04 File Offset: 0x000E4F04
		public void InitSystenParams()
		{
			try
			{
				this.ZhenAiBuffHoldWinSec = (int)GameManager.systemParamsList.GetParamValueIntByName("CoupleVictoryNeedTime", -1);
				this.YongQiBuff2ZhenAiBuffHurt = GameManager.systemParamsList.GetParamValueDoubleByName("CoupleBuffSpecificHurt", 0.0);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				this.ZhenAiBuffHoldWinSec = 60;
				this.YongQiBuff2ZhenAiBuffHurt = 0.2;
			}
		}

		// Token: 0x06000EB5 RID: 3765 RVA: 0x000E6E28 File Offset: 0x000E5028
		private bool LoadConfig()
		{
			bool result;
			try
			{
				XElement xml = XElement.Load(Global.GameResPath(CoupleAreanConsts.WarCfgFile));
				if (xml.Elements().Count<XElement>() < 1)
				{
					throw new Exception(CoupleAreanConsts.WarCfgFile + " need at least 1 elements");
				}
				using (IEnumerator<XElement> enumerator = xml.Elements().GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						XElement xmlItem = enumerator.Current;
						this.WarCfg.Id = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						this.WarCfg.MapCode = (int)Global.GetSafeAttributeLong(xmlItem, "MapCode");
						this.WarCfg.WaitSec = (int)Global.GetSafeAttributeLong(xmlItem, "WaitingEnterSecs");
						this.WarCfg.FightSec = (int)Global.GetSafeAttributeLong(xmlItem, "FightingSecs");
						this.WarCfg.ClearSec = (int)Global.GetSafeAttributeLong(xmlItem, "ClearRolesSecs");
						this.WarCfg.TimePoints = new List<CoupleAreanWarCfg.TimePoint>();
						string[] weekPoints = Global.GetSafeAttributeStr(xmlItem, "TimePoints").Split(new char[]
						{
							',',
							'-',
							'|'
						});
						for (int i = 0; i < weekPoints.Length; i += 3)
						{
							CoupleAreanWarCfg.TimePoint tp = new CoupleAreanWarCfg.TimePoint();
							tp.Weekday = Convert.ToInt32(weekPoints[i]);
							if (tp.Weekday < 1 || tp.Weekday > 7)
							{
								throw new Exception("weekday error!");
							}
							tp.DayStartTicks = DateTime.Parse(weekPoints[i + 1]).TimeOfDay.Ticks;
							tp.DayEndTicks = DateTime.Parse(weekPoints[i + 2]).TimeOfDay.Ticks;
							this.WarCfg.TimePoints.Add(tp);
						}
						this.WarCfg.TimePoints.Sort((CoupleAreanWarCfg.TimePoint _l, CoupleAreanWarCfg.TimePoint _r) => _l.Weekday - _r.Weekday);
					}
				}
				xml = XElement.Load(Global.GameResPath(CoupleAreanConsts.DuanWeiCfgFile));
				foreach (XElement xmlItem in xml.Elements())
				{
					CoupleAreanDuanWeiCfg duanweiCfg = new CoupleAreanDuanWeiCfg();
					XElement xmlItem;
					duanweiCfg.Id = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
					duanweiCfg.Type = (int)Global.GetSafeAttributeLong(xmlItem, "Type");
					duanweiCfg.Level = (int)Global.GetSafeAttributeLong(xmlItem, "Level");
					duanweiCfg.NeedJiFen = (int)Global.GetSafeAttributeLong(xmlItem, "NeedCoupleDuanWeiJiFen");
					duanweiCfg.WinJiFen = (int)Global.GetSafeAttributeLong(xmlItem, "WinJiFen");
					duanweiCfg.LoseJiFen = (int)Global.GetSafeAttributeLong(xmlItem, "LoseJiFen");
					duanweiCfg.WeekGetRongYaoTimes = (int)Global.GetSafeAttributeLong(xmlItem, "WeekRongYaoNum");
					duanweiCfg.WinRongYao = (int)Global.GetSafeAttributeLong(xmlItem, "WinRongYu");
					duanweiCfg.LoseRongYao = (int)Global.GetSafeAttributeLong(xmlItem, "LoseRongYu");
					this.DuanWeiCfgList.Add(duanweiCfg);
				}
				this.DuanWeiCfgList.Sort(delegate(CoupleAreanDuanWeiCfg _l, CoupleAreanDuanWeiCfg _r)
				{
					int result2;
					if (_l.Type < _r.Type)
					{
						result2 = -1;
					}
					else if (_l.Type > _r.Type)
					{
						result2 = 1;
					}
					else if (_l.Level > _r.Level)
					{
						result2 = -1;
					}
					else if (_l.Level < _r.Level)
					{
						result2 = 1;
					}
					else
					{
						result2 = 0;
					}
					return result2;
				});
				for (int i = 1; i < this.DuanWeiCfgList.Count; i++)
				{
					CoupleAreanDuanWeiCfg curr = this.DuanWeiCfgList[i];
					CoupleAreanDuanWeiCfg left = this.DuanWeiCfgList[i - 1];
					if (curr.NeedJiFen <= left.NeedJiFen)
					{
						throw new Exception(string.Format("段位积分配置有问题{0}", curr.Id));
					}
				}
				if (this.DuanWeiCfgList[0].NeedJiFen != 0)
				{
					throw new Exception(string.Format("段位积分配置有问题{0}", this.DuanWeiCfgList[0].Id));
				}
				xml = XElement.Load(Global.GameResPath(CoupleAreanConsts.WeekRankAwardCfgFile));
				foreach (XElement xmlItem in xml.Elements())
				{
					CoupleAreanWeekRankAwardCfg weekAwardCfg = new CoupleAreanWeekRankAwardCfg();
					XElement xmlItem;
					weekAwardCfg.Id = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
					weekAwardCfg.Name = Global.GetSafeAttributeStr(xmlItem, "Name");
					weekAwardCfg.StartRank = (int)Global.GetSafeAttributeLong(xmlItem, "StarRank");
					weekAwardCfg.EndRank = (int)Global.GetSafeAttributeLong(xmlItem, "EndRank");
					weekAwardCfg.AwardGoods = GoodsHelper.ParseGoodsDataList(Global.GetSafeAttributeStr(xmlItem, "Award").Split(new char[]
					{
						'|'
					}), CoupleAreanConsts.WeekRankAwardCfgFile);
					this.WeekAwardCfgList.Add(weekAwardCfg);
				}
				xml = XElement.Load(Global.GameResPath(CoupleAreanConsts.BuffCfgFile));
				foreach (XElement xmlItem in xml.Elements())
				{
					CoupleArenaBuffCfg buffCfg = new CoupleArenaBuffCfg();
					XElement xmlItem;
					buffCfg.Type = (int)Global.GetSafeAttributeLong(xmlItem, "TypeID");
					buffCfg.Name = Global.GetSafeAttributeStr(xmlItem, "Name");
					buffCfg.MonsterId = (int)Global.GetSafeAttributeLong(xmlItem, "MonstersID");
					buffCfg.RandPosList = new List<CoupleArenaBuffCfg.RandPos>();
					string[] szBuffPos = Global.GetSafeAttributeStr(xmlItem, "Site").Split(new char[]
					{
						'|',
						','
					});
					for (int i = 0; i < szBuffPos.Length - 2; i += 3)
					{
						CoupleArenaBuffCfg.RandPos randPos = new CoupleArenaBuffCfg.RandPos();
						randPos.X = Convert.ToInt32(szBuffPos[i]);
						randPos.Y = Convert.ToInt32(szBuffPos[i + 1]);
						randPos.R = Convert.ToInt32(szBuffPos[i + 2]);
						buffCfg.RandPosList.Add(randPos);
					}
					buffCfg.FlushSecList = new List<int>();
					string[] szFlushSec = Global.GetSafeAttributeStr(xmlItem, "Time").Split(new char[]
					{
						'|'
					});
					for (int i = 0; i < szFlushSec.Length; i++)
					{
						buffCfg.FlushSecList.Add(Convert.ToInt32(szFlushSec[i]));
					}
					buffCfg.ExtProps = new Dictionary<ExtPropIndexes, double>();
					string[] szExtProps = Global.GetSafeAttributeStr(xmlItem, "Property").Split(new char[]
					{
						'|',
						','
					});
					for (int i = 0; i < szExtProps.Length - 1; i += 2)
					{
						buffCfg.ExtProps.Add((ExtPropIndexes)Enum.Parse(typeof(ExtPropIndexes), szExtProps[i]), Convert.ToDouble(szExtProps[i + 1]));
					}
					this.BuffCfgList.Add(buffCfg);
				}
				xml = XElement.Load(Global.GameResPath(CoupleAreanConsts.BirthPointCfgFile));
				foreach (XElement xmlItem in xml.Elements())
				{
					TianTiBirthPoint bp = new TianTiBirthPoint();
					XElement xmlItem;
					bp.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
					bp.PosX = (int)Global.GetSafeAttributeLong(xmlItem, "PosX");
					bp.PosY = (int)Global.GetSafeAttributeLong(xmlItem, "PosY");
					bp.BirthRadius = (int)Global.GetSafeAttributeLong(xmlItem, "BirthRadius");
					this.BirthPointList.Add(bp);
				}
				if (this.BirthPointList.Count != 2)
				{
					throw new Exception(CoupleAreanConsts.BirthPointCfgFile);
				}
				result = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteException("CoupleArenaManager loadconfig. " + ex.Message);
				result = false;
			}
			return result;
		}

		// Token: 0x06000EB6 RID: 3766 RVA: 0x000E76B0 File Offset: 0x000E58B0
		public bool initialize()
		{
			bool result;
			if (!this.LoadConfig())
			{
				result = false;
			}
			else
			{
				ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("CoupleArenaManager.TimerProc", new EventHandler(this.TimerProc)), 20000, 10000);
				result = true;
			}
			return result;
		}

		// Token: 0x06000EB7 RID: 3767 RVA: 0x000E76FC File Offset: 0x000E58FC
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1370, 1, 1, SingletonTemplate<CoupleArenaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1372, 1, 1, SingletonTemplate<CoupleArenaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1371, 1, 1, SingletonTemplate<CoupleArenaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1373, 2, 2, SingletonTemplate<CoupleArenaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1374, 1, 1, SingletonTemplate<CoupleArenaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1377, 2, 2, SingletonTemplate<CoupleArenaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1375, 1, 1, SingletonTemplate<CoupleArenaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1380, 2, 2, SingletonTemplate<CoupleArenaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(10025, 38, SingletonTemplate<CoupleArenaManager>.Instance());
			GlobalEventSource.getInstance().registerListener(10, SingletonTemplate<CoupleArenaManager>.Instance());
			GlobalEventSource.getInstance().registerListener(11, SingletonTemplate<CoupleArenaManager>.Instance());
			return true;
		}

		// Token: 0x06000EB8 RID: 3768 RVA: 0x000E780C File Offset: 0x000E5A0C
		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(10025, 38, SingletonTemplate<CoupleArenaManager>.Instance());
			GlobalEventSource.getInstance().removeListener(10, SingletonTemplate<CoupleArenaManager>.Instance());
			GlobalEventSource.getInstance().removeListener(11, SingletonTemplate<CoupleArenaManager>.Instance());
			return true;
		}

		// Token: 0x06000EB9 RID: 3769 RVA: 0x000E785C File Offset: 0x000E5A5C
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06000EBA RID: 3770 RVA: 0x000E7870 File Offset: 0x000E5A70
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
				case 1370:
					return this.HandleGetMainDataCommand(client, nID, bytes, cmdParams);
				case 1371:
					return this.HandleGetZhanBaoCommand(client, nID, bytes, cmdParams);
				case 1372:
					return this.HandleGetPaiHangCommand(client, nID, bytes, cmdParams);
				case 1373:
					return this.HandleSetReadyCommand(client, nID, bytes, cmdParams);
				case 1374:
					return this.HandleSingleJoinCommand(client, nID, bytes, cmdParams);
				case 1375:
					return this.HandleQuitCommand(client, nID, bytes, cmdParams);
				case 1377:
					return this.HandleEnterCommand(client, nID, bytes, cmdParams);
				case 1380:
					return this.HandleRegStateWatcherCommand(client, nID, bytes, cmdParams);
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06000EBB RID: 3771 RVA: 0x000E7948 File Offset: 0x000E5B48
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x06000EBC RID: 3772 RVA: 0x000E795C File Offset: 0x000E5B5C
		public void processEvent(EventObjectEx eventObject)
		{
			if (eventObject.EventType == 10025)
			{
				this.HandleCanEnterEvent((eventObject as CoupleArenaCanEnterEvent).Data);
			}
			eventObject.Handled = true;
		}

		// Token: 0x06000EBD RID: 3773 RVA: 0x000E799C File Offset: 0x000E5B9C
		private void HandleCanEnterEvent(CoupleArenaCanEnterData data)
		{
			string dbIp;
			int dbPort;
			string logIp;
			int logPort;
			string gsIp;
			int gsPort;
			if (!KuaFuManager.getInstance().GetKuaFuDbServerInfo(data.KfServerId, out dbIp, out dbPort, out logIp, out logPort, out gsIp, out gsPort))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("夫妻竞技被分配到服务器ServerId={0}, 但是找不到该跨服活动服务器", data.KfServerId), null, true);
			}
			else
			{
				lock (this.Mutex)
				{
					GameClient client = GameManager.ClientMgr.FindClient(data.RoleId1);
					if (client != null && this.GetMatchState(data.RoleId1) == ECoupleArenaMatchState.Ready)
					{
						client.ClientSocket.ClientKuaFuServerLoginData.RoleId = data.RoleId1;
						client.ClientSocket.ClientKuaFuServerLoginData.GameId = data.GameId;
						client.ClientSocket.ClientKuaFuServerLoginData.GameType = 13;
						client.ClientSocket.ClientKuaFuServerLoginData.EndTicks = 0L;
						client.ClientSocket.ClientKuaFuServerLoginData.ServerId = GameCoreInterface.getinstance().GetLocalServerId();
						client.ClientSocket.ClientKuaFuServerLoginData.ServerIp = gsIp;
						client.ClientSocket.ClientKuaFuServerLoginData.ServerPort = gsPort;
						client.ClientSocket.ClientKuaFuServerLoginData.FuBenSeqId = 0;
						client.sendCmd(1376, "1", false);
						this.SetMatchState(data.RoleId1, ECoupleArenaMatchState.OnLine);
						this.NtfCoupleMatchState(client.ClientData.RoleID);
						if (MarryLogic.IsMarried(client.ClientData.RoleID))
						{
							this.NtfCoupleMatchState(client.ClientData.MyMarriageData.nSpouseID);
						}
					}
					GameClient client2 = GameManager.ClientMgr.FindClient(data.RoleId2);
					if (client2 != null && this.GetMatchState(data.RoleId2) == ECoupleArenaMatchState.Ready)
					{
						client2.ClientSocket.ClientKuaFuServerLoginData.RoleId = data.RoleId2;
						client2.ClientSocket.ClientKuaFuServerLoginData.GameId = data.GameId;
						client2.ClientSocket.ClientKuaFuServerLoginData.GameType = 13;
						client2.ClientSocket.ClientKuaFuServerLoginData.EndTicks = 0L;
						client2.ClientSocket.ClientKuaFuServerLoginData.ServerId = GameCoreInterface.getinstance().GetLocalServerId();
						client2.ClientSocket.ClientKuaFuServerLoginData.ServerIp = gsIp;
						client2.ClientSocket.ClientKuaFuServerLoginData.ServerPort = gsPort;
						client2.ClientSocket.ClientKuaFuServerLoginData.FuBenSeqId = 0;
						client2.sendCmd(1376, "1", false);
						this.SetMatchState(data.RoleId2, ECoupleArenaMatchState.OnLine);
						this.NtfCoupleMatchState(client2.ClientData.RoleID);
						if (MarryLogic.IsMarried(client2.ClientData.RoleID))
						{
							this.NtfCoupleMatchState(client2.ClientData.MyMarriageData.nSpouseID);
						}
					}
				}
			}
		}

		// Token: 0x06000EBE RID: 3774 RVA: 0x000E7CC0 File Offset: 0x000E5EC0
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 11)
			{
				MonsterDeadEventObject deadEv = eventObject as MonsterDeadEventObject;
				if (deadEv.getAttacker().ClientData.CopyMapID > 0 && deadEv.getAttacker().ClientData.FuBenSeqID > 0 && deadEv.getAttacker().ClientData.MapCode == this.WarCfg.MapCode && deadEv.getMonster().CurrentMapCode == this.WarCfg.MapCode)
				{
					lock (this.Mutex)
					{
						CoupleArenaCopyScene scene;
						if (this.FuBenSeq2CopyScenes.TryGetValue(deadEv.getAttacker().ClientData.FuBenSeqID, out scene))
						{
							this.OnMonsterDead(scene, deadEv.getAttacker(), deadEv.getMonster());
						}
					}
				}
			}
			else if (eventObject.getEventType() == 10)
			{
				PlayerDeadEventObject deadEv2 = eventObject as PlayerDeadEventObject;
				if (deadEv2.getPlayer().ClientData.CopyMapID > 0 && deadEv2.getPlayer().ClientData.FuBenSeqID > 0 && deadEv2.getPlayer().ClientData.MapCode == this.WarCfg.MapCode)
				{
					lock (this.Mutex)
					{
						CoupleArenaCopyScene scene;
						if (this.FuBenSeq2CopyScenes.TryGetValue(deadEv2.getPlayer().ClientData.FuBenSeqID, out scene))
						{
							this.OnPlayerDead(scene, deadEv2.getPlayer(), deadEv2.getAttackerRole());
						}
					}
				}
			}
		}

		// Token: 0x06000EBF RID: 3775 RVA: 0x000E7EB8 File Offset: 0x000E60B8
		private bool HandleRegStateWatcherCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int roleid = Convert.ToInt32(cmdParams[0]);
			int watch = Convert.ToInt32(cmdParams[1]);
			this.RegStateWatcher(client.ClientData.RoleID, watch > 0);
			this.NtfCoupleMatchState(client.ClientData.RoleID);
			return true;
		}

		// Token: 0x06000EC0 RID: 3776 RVA: 0x000E7F0C File Offset: 0x000E610C
		private bool HandleEnterCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int roleid = Convert.ToInt32(cmdParams[0]);
			int enter = Convert.ToInt32(cmdParams[1]);
			if (enter <= 0)
			{
				Global.GetClientKuaFuServerLoginData(client).RoleId = 0;
				client.sendCmd(nID, 1.ToString(), false);
			}
			else
			{
				GlobalNew.RecordSwitchKuaFuServerLog(client);
				client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
			}
			return true;
		}

		// Token: 0x06000EC1 RID: 3777 RVA: 0x000E7F7C File Offset: 0x000E617C
		private bool HandleQuitCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!this.IsGongNengOpened(client) || !MarryLogic.IsMarried(client.ClientData.RoleID))
			{
				result = true;
			}
			else
			{
				lock (this.Mutex)
				{
					if (this.GetMatchState(client.ClientData.RoleID) != ECoupleArenaMatchState.Ready)
					{
						client.sendCmd(nID, 1.ToString(), false);
						result = true;
					}
					else
					{
						TianTiClient.getInstance().CoupleArenaQuit(client.ClientData.RoleID, client.ClientData.MyMarriageData.nSpouseID);
						this.SetMatchState(client.ClientData.RoleID, ECoupleArenaMatchState.OnLine);
						if (this.GetMatchState(client.ClientData.MyMarriageData.nSpouseID) == ECoupleArenaMatchState.Ready)
						{
							this.SetMatchState(client.ClientData.MyMarriageData.nSpouseID, ECoupleArenaMatchState.OnLine);
							GameClient spouseClient = GameManager.ClientMgr.FindClient(client.ClientData.MyMarriageData.nSpouseID);
							if (spouseClient != null)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, spouseClient, GLang.GetLang(475, new object[0]), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.ErrAndBox, 0);
								spouseClient.sendCmd(1375, 1.ToString(), false);
							}
						}
						this.NtfCoupleMatchState(client.ClientData.RoleID);
						this.NtfCoupleMatchState(client.ClientData.MyMarriageData.nSpouseID);
						client.sendCmd(nID, 1.ToString(), false);
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06000EC2 RID: 3778 RVA: 0x000E8150 File Offset: 0x000E6350
		private bool HandleSingleJoinCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!client.ClientData.IsMainOccupation)
			{
				client.sendCmd(nID, -35.ToString(), false);
				result = true;
			}
			else
			{
				lock (this.Mutex)
				{
					if (!this.IsGongNengOpened(client))
					{
						client.sendCmd(nID, -12.ToString(), false);
						result = true;
					}
					else if (!this.IsInWeekOnceActTimes(TimeUtil.NowDateTime()))
					{
						client.sendCmd(nID, -2001.ToString(), false);
						result = true;
					}
					else if (MarryLogic.IsMarried(client.ClientData.RoleID) && this.GetMatchState(client.ClientData.MyMarriageData.nSpouseID) != ECoupleArenaMatchState.Offline && this.GetMatchState(client.ClientData.MyMarriageData.nSpouseID) != ECoupleArenaMatchState.NotOpen)
					{
						client.sendCmd(nID, -12.ToString(), false);
						result = true;
					}
					else if (this.GetMatchState(client.ClientData.RoleID) != ECoupleArenaMatchState.OnLine)
					{
						client.sendCmd(nID, -12.ToString(), false);
						result = true;
					}
					else
					{
						int ret = TianTiClient.getInstance().CoupleArenaJoin(client.ClientData.RoleID, client.ClientData.MyMarriageData.nSpouseID, GameCoreInterface.getinstance().GetLocalServerId());
						if (ret >= 0)
						{
							this.SetMatchState(client.ClientData.RoleID, ECoupleArenaMatchState.Ready);
							this.NtfCoupleMatchState(client.ClientData.RoleID);
							this.NtfCoupleMatchState(client.ClientData.MyMarriageData.nSpouseID);
							GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 1, 0, 0, 0, 13);
						}
						client.sendCmd(nID, ret.ToString(), false);
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06000EC3 RID: 3779 RVA: 0x000E8374 File Offset: 0x000E6574
		private bool HandleSetReadyCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int roleid = Convert.ToInt32(cmdParams[0]);
			bool bReady = Convert.ToInt32(cmdParams[1]) > 0;
			bool result;
			if (!client.ClientData.IsMainOccupation)
			{
				client.sendCmd(nID, -35.ToString(), false);
				result = true;
			}
			else if (!this.IsGongNengOpened(client))
			{
				client.sendCmd(nID, -12.ToString(), false);
				result = true;
			}
			else if (!this.IsInWeekOnceActTimes(TimeUtil.NowDateTime()))
			{
				client.sendCmd(nID, -2001.ToString(), false);
				result = true;
			}
			else
			{
				lock (this.Mutex)
				{
					ECoupleArenaMatchState oldState = this.GetMatchState(client.ClientData.RoleID);
					ECoupleArenaMatchState newState = bReady ? ECoupleArenaMatchState.Ready : ECoupleArenaMatchState.OnLine;
					if (oldState == newState)
					{
						result = true;
					}
					else
					{
						this.SetMatchState(client.ClientData.RoleID, newState);
						this.NtfCoupleMatchState(client.ClientData.RoleID);
						this.NtfCoupleMatchState(client.ClientData.MyMarriageData.nSpouseID);
						if (oldState != ECoupleArenaMatchState.Ready && newState == ECoupleArenaMatchState.Ready && this.GetMatchState(client.ClientData.MyMarriageData.nSpouseID) == ECoupleArenaMatchState.Ready)
						{
							CoupleArenaJoinData req = new CoupleArenaJoinData();
							req.RoleId1 = client.ClientData.RoleID;
							req.RoleId2 = client.ClientData.MyMarriageData.nSpouseID;
							int ret = TianTiClient.getInstance().CoupleArenaJoin(client.ClientData.RoleID, client.ClientData.MyMarriageData.nSpouseID, GameCoreInterface.getinstance().GetLocalServerId());
							if (ret >= 0)
							{
								client.sendCmd(1374, ret.ToString(), false);
								GameClient spouseClient = GameManager.ClientMgr.FindClient(client.ClientData.MyMarriageData.nSpouseID);
								if (spouseClient != null)
								{
									spouseClient.sendCmd(1374, ret.ToString(), false);
								}
							}
						}
						if (newState == ECoupleArenaMatchState.Ready)
						{
							GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 1, 0, 0, 0, 13);
						}
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06000EC4 RID: 3780 RVA: 0x000E8610 File Offset: 0x000E6810
		private bool HandleGetZhanBaoCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			List<CoupleArenaZhanBaoItemData> items = null;
			if (this.IsGongNengOpened(client))
			{
				items = Global.sendToDB<List<CoupleArenaZhanBaoItemData>, string>(nID, string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.MyMarriageData.nSpouseID), client.ServerId);
			}
			client.sendCmd<List<CoupleArenaZhanBaoItemData>>(nID, items, false);
			return true;
		}

		// Token: 0x06000EC5 RID: 3781 RVA: 0x000E867C File Offset: 0x000E687C
		private bool HandleGetPaiHangCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			CoupleArenaPaiHangData data = new CoupleArenaPaiHangData();
			lock (this.Mutex)
			{
				data.PaiHang = this.SyncRankList.GetRange(0, Math.Min(10, this.SyncRankList.Count));
			}
			client.sendCmd<CoupleArenaPaiHangData>(nID, data, false);
			return true;
		}

		// Token: 0x06000EC6 RID: 3782 RVA: 0x000E86FC File Offset: 0x000E68FC
		private bool HandleGetMainDataCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			DateTime now = TimeUtil.NowDateTime();
			CoupleArenaMainData data = new CoupleArenaMainData();
			data.JingJiData = null;
			data.WeekGetRongYaoTimes = 0;
			data.CanGetAwardId = 0;
			CoupleArenaCoupleJingJiData jingJiData = new CoupleArenaCoupleJingJiData();
			jingJiData.ManRoleId = client.ClientData.RoleID;
			jingJiData.ManZoneId = client.ClientData.ZoneID;
			jingJiData.ManSelector = Global.sendToDB<RoleData4Selector, int>(10232, client.ClientData.RoleID, client.ServerId);
			if (MarryLogic.IsMarried(client.ClientData.RoleID))
			{
				jingJiData.WifeSelector = Global.sendToDB<RoleData4Selector, int>(10232, client.ClientData.MyMarriageData.nSpouseID, client.ServerId);
				if (jingJiData.WifeSelector != null)
				{
					jingJiData.WifeRoleId = jingJiData.WifeSelector.RoleID;
					jingJiData.WifeZoneId = jingJiData.WifeSelector.ZoneId;
				}
			}
			if ((!MarryLogic.IsMarried(client.ClientData.RoleID) && client.ClientData.RoleSex == 1) || client.ClientData.MyMarriageData.byMarrytype == 2)
			{
				int tmpRoleId = jingJiData.ManRoleId;
				int tmpZoneId = jingJiData.ManZoneId;
				RoleData4Selector tmpSelector = jingJiData.ManSelector;
				jingJiData.ManRoleId = jingJiData.WifeRoleId;
				jingJiData.ManZoneId = jingJiData.WifeZoneId;
				jingJiData.ManSelector = jingJiData.WifeSelector;
				jingJiData.WifeRoleId = tmpRoleId;
				jingJiData.WifeZoneId = tmpZoneId;
				jingJiData.WifeSelector = tmpSelector;
			}
			jingJiData.DuanWeiType = this.DuanWeiCfgList[0].Type;
			jingJiData.DuanWeiLevel = this.DuanWeiCfgList[0].Level;
			jingJiData.JiFen = 0;
			if (MarryLogic.IsMarried(client.ClientData.RoleID))
			{
				CoupleArenaCoupleJingJiData coupleData = this.GetCachedCoupleData(client.ClientData.RoleID);
				if (coupleData != null)
				{
					jingJiData.TotalFightTimes = coupleData.TotalFightTimes;
					jingJiData.WinFightTimes = coupleData.WinFightTimes;
					jingJiData.LianShengTimes = coupleData.LianShengTimes;
					jingJiData.DuanWeiType = coupleData.DuanWeiType;
					jingJiData.DuanWeiLevel = coupleData.DuanWeiLevel;
					jingJiData.JiFen = coupleData.JiFen;
					jingJiData.Rank = coupleData.Rank;
				}
				GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_CoupleArenaDuanWei, new int[0]));
			}
			data.JingJiData = jingJiData;
			string szWeekRongYao = Global.GetRoleParamByName(client, "27");
			if (!string.IsNullOrEmpty(szWeekRongYao))
			{
				string[] fields = szWeekRongYao.Split(new char[]
				{
					','
				});
				if (fields != null && fields.Length == 2 && Convert.ToInt32(fields[0]) == this.CurrRankWeek(now))
				{
					data.WeekGetRongYaoTimes = Convert.ToInt32(fields[1]);
				}
			}
			int willAwardWeek;
			if (this.IsInWeekAwardTime(now, out willAwardWeek))
			{
				CoupleArenaCoupleJingJiData coupleData = this.GetCachedCoupleData(client.ClientData.RoleID);
				if (coupleData != null)
				{
					string szWeekAward = Global.GetRoleParamByName(client, "28");
					string[] fields = (szWeekAward == null) ? null : szWeekAward.Split(new char[]
					{
						','
					});
					if (fields == null || fields.Length != 2 || Convert.ToInt32(fields[0]) != willAwardWeek)
					{
						foreach (CoupleAreanWeekRankAwardCfg award in this.WeekAwardCfgList)
						{
							if (coupleData.Rank >= award.StartRank && (award.EndRank == -1 || coupleData.Rank <= award.EndRank))
							{
								Global.SaveRoleParamsStringToDB(client, "28", string.Format("{0},{1}", willAwardWeek, award.Id), true);
								data.CanGetAwardId = award.Id;
								if (Global.CanAddGoodsDataList(client, award.AwardGoods))
								{
									foreach (GoodsData goodsData in award.AwardGoods)
									{
										Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, goodsData.GCount, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 0, goodsData.Jewellist, true, 1, "情侣竞技场", false, goodsData.Endtime, goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, goodsData.ExcellenceInfo, goodsData.AppendPropLev, goodsData.ChangeLifeLevForEquip, true, null, null, "1900-01-01 12:00:00", 0, true);
									}
								}
								else
								{
									Global.UseMailGivePlayerAward3(client.ClientData.RoleID, award.AwardGoods, GLang.GetLang(476, new object[0]), string.Format(GLang.GetLang(477, new object[0]), coupleData.Rank), 0, 0, 0);
								}
								break;
							}
						}
					}
					this.CheckTipsIconState(client);
				}
			}
			client.sendCmd<CoupleArenaMainData>(nID, data, false);
			return true;
		}

		// Token: 0x06000EC7 RID: 3783 RVA: 0x000E8CA8 File Offset: 0x000E6EA8
		public void OnClientLogin(GameClient client)
		{
			if (client != null)
			{
				this.CheckFengHuoJiaRenChengHao(client);
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					lock (this.Mutex)
					{
						this.RegStateWatcher(client.ClientData.RoleID, false);
						this.SetMatchState(client.ClientData.RoleID, (this.IsGongNengOpened(client) && MarryLogic.IsMarried(client.ClientData.RoleID)) ? ECoupleArenaMatchState.OnLine : ECoupleArenaMatchState.NotOpen);
						if (this.GetMatchState(client.ClientData.RoleID) == ECoupleArenaMatchState.OnLine)
						{
							if (this.GetMatchState(client.ClientData.MyMarriageData.nSpouseID) == ECoupleArenaMatchState.Ready)
							{
								TianTiClient.getInstance().CoupleArenaQuit(client.ClientData.RoleID, client.ClientData.MyMarriageData.nSpouseID);
								this.SetMatchState(client.ClientData.MyMarriageData.nSpouseID, ECoupleArenaMatchState.OnLine);
								GameClient spouseClient = GameManager.ClientMgr.FindClient(client.ClientData.MyMarriageData.nSpouseID);
								if (spouseClient != null)
								{
									if (this.RoleStartReadyMs.ContainsKey(spouseClient.ClientData.RoleID) && this.RoleStartReadyMs[spouseClient.ClientData.RoleID] + 60000L > TimeUtil.NOW())
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, spouseClient, "情侣双方均在线，无法单人匹配", GameInfoTypeIndexes.Normal, ShowGameInfoTypes.ErrAndBox, 0);
									}
									spouseClient.sendCmd(1375, 1.ToString(), false);
								}
							}
							this.NtfCoupleMatchState(client.ClientData.MyMarriageData.nSpouseID);
						}
						this.CheckTipsIconState(client);
						GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_CoupleArenaDuanWei, new int[0]));
					}
				}
			}
		}

		// Token: 0x06000EC8 RID: 3784 RVA: 0x000E8ED8 File Offset: 0x000E70D8
		public void OnClientLogout(GameClient client)
		{
			try
			{
				if (client != null)
				{
					if (!client.ClientSocket.IsKuaFuLogin)
					{
						lock (this.Mutex)
						{
							this.RegStateWatcher(client.ClientData.RoleID, false);
							if (this.GetMatchState(client.ClientData.RoleID) == ECoupleArenaMatchState.Ready || this.GetMatchState(client.ClientData.RoleID) == ECoupleArenaMatchState.OnLine)
							{
								TianTiClient.getInstance().CoupleArenaQuit(client.ClientData.RoleID, client.ClientData.MyMarriageData.nSpouseID);
								if (this.GetMatchState(client.ClientData.MyMarriageData.nSpouseID) == ECoupleArenaMatchState.Ready)
								{
									this.SetMatchState(client.ClientData.MyMarriageData.nSpouseID, ECoupleArenaMatchState.OnLine);
									GameClient spouseClient = GameManager.ClientMgr.FindClient(client.ClientData.MyMarriageData.nSpouseID);
									if (spouseClient != null)
									{
										spouseClient.sendCmd(1375, 1.ToString(), false);
									}
								}
							}
							this.SetMatchState(client.ClientData.RoleID, ECoupleArenaMatchState.Offline);
							this.NtfCoupleMatchState(client.ClientData.MyMarriageData.nSpouseID);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
		}

		// Token: 0x06000EC9 RID: 3785 RVA: 0x000E9090 File Offset: 0x000E7290
		public void CheckGongNengOpen(GameClient client)
		{
			lock (this.Mutex)
			{
				if (this.GetMatchState(client.ClientData.RoleID) == ECoupleArenaMatchState.NotOpen && this.IsGongNengOpened(client))
				{
					this.SetMatchState(client.ClientData.RoleID, ECoupleArenaMatchState.OnLine);
					this.NtfCoupleMatchState(client.ClientData.RoleID);
					if (this.GetMatchState(client.ClientData.MyMarriageData.nSpouseID) == ECoupleArenaMatchState.Ready)
					{
						GameClient spouseClient = GameManager.ClientMgr.FindClient(client.ClientData.MyMarriageData.nSpouseID);
						if (spouseClient != null)
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, spouseClient, StringUtil.substitute(GLang.GetLang(478, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						}
						TianTiClient.getInstance().CoupleArenaQuit(client.ClientData.RoleID, client.ClientData.MyMarriageData.nSpouseID);
						this.SetMatchState(client.ClientData.MyMarriageData.nSpouseID, ECoupleArenaMatchState.OnLine);
					}
					this.NtfCoupleMatchState(client.ClientData.MyMarriageData.nSpouseID);
				}
			}
		}

		// Token: 0x06000ECA RID: 3786 RVA: 0x000E9210 File Offset: 0x000E7410
		public void OnMarry(GameClient client1, GameClient client2)
		{
			lock (this.Mutex)
			{
				this.SetMatchState(client1.ClientData.RoleID, this.IsGongNengOpened(client1) ? ECoupleArenaMatchState.OnLine : ECoupleArenaMatchState.NotOpen);
				this.SetMatchState(client2.ClientData.RoleID, this.IsGongNengOpened(client2) ? ECoupleArenaMatchState.OnLine : ECoupleArenaMatchState.NotOpen);
				this.NtfCoupleMatchState(client1.ClientData.RoleID);
				this.NtfCoupleMatchState(client2.ClientData.RoleID);
			}
		}

		// Token: 0x06000ECB RID: 3787 RVA: 0x000E92BC File Offset: 0x000E74BC
		public void OnDivorce(int roleId1, int roleId2)
		{
			lock (this.Mutex)
			{
				if (this.GetMatchState(roleId1) == ECoupleArenaMatchState.Ready || this.GetMatchState(roleId2) == ECoupleArenaMatchState.Ready)
				{
					TianTiClient.getInstance().CoupleArenaQuit(roleId1, roleId2);
				}
				ECoupleArenaMatchState oldState = this.GetMatchState(roleId1);
				ECoupleArenaMatchState oldState2 = this.GetMatchState(roleId2);
				this.SetMatchState(roleId1, (oldState == ECoupleArenaMatchState.OnLine || oldState == ECoupleArenaMatchState.Ready) ? ECoupleArenaMatchState.NotOpen : oldState);
				this.SetMatchState(roleId2, (oldState2 == ECoupleArenaMatchState.OnLine || oldState2 == ECoupleArenaMatchState.Ready) ? ECoupleArenaMatchState.NotOpen : oldState2);
				this.NtfCoupleMatchState(roleId1);
				this.NtfCoupleMatchState(roleId2);
				if (oldState == ECoupleArenaMatchState.Ready)
				{
					GameClient client = GameManager.ClientMgr.FindClient(roleId1);
					if (client != null)
					{
						client.sendCmd(1375, 1.ToString(), false);
					}
				}
				if (oldState2 == ECoupleArenaMatchState.Ready)
				{
					GameClient client2 = GameManager.ClientMgr.FindClient(roleId2);
					if (client2 != null)
					{
						client2.sendCmd(1375, 1.ToString(), false);
					}
				}
				this.CheckFengHuoJiaRenChengHao(GameManager.ClientMgr.FindClient(roleId1));
				this.CheckFengHuoJiaRenChengHao(GameManager.ClientMgr.FindClient(roleId2));
				Global.sendToDB<bool, string>(1383, string.Format("{0}:{1}", roleId1, roleId2), 0);
			}
		}

		// Token: 0x06000ECC RID: 3788 RVA: 0x000E944C File Offset: 0x000E764C
		public bool PreClearDivorceData(int roleId1, int roleId2)
		{
			return TianTiClient.getInstance().CoupleArenaPreDivorce(roleId1, roleId2) >= 0;
		}

		// Token: 0x06000ECD RID: 3789 RVA: 0x000E9478 File Offset: 0x000E7678
		public void OnSpouseRequestDivorce(GameClient client, GameClient spouseClient)
		{
			if (client != null)
			{
				if (spouseClient != null)
				{
					lock (this.Mutex)
					{
						if (this.GetMatchState(client.ClientData.RoleID) == ECoupleArenaMatchState.Ready)
						{
							TianTiClient.getInstance().CoupleArenaQuit(client.ClientData.RoleID, spouseClient.ClientData.RoleID);
							this.SetMatchState(client.ClientData.RoleID, ECoupleArenaMatchState.OnLine);
							this.NtfCoupleMatchState(client.ClientData.RoleID);
							client.sendCmd(1375, 1.ToString(), false);
						}
					}
				}
			}
		}

		// Token: 0x06000ECE RID: 3790 RVA: 0x000E9558 File Offset: 0x000E7758
		public double CalcBuffHurt(IObject obj, IObject objTarget)
		{
			double result;
			try
			{
				if (obj == null || objTarget == null)
				{
					result = 0.0;
				}
				else
				{
					GameClient fromClient = obj as GameClient;
					GameClient toClient = objTarget as GameClient;
					if (fromClient == null || toClient == null)
					{
						result = 0.0;
					}
					else
					{
						BufferData yongqiData = Global.GetBufferDataByID(fromClient, 2080011);
						BufferData zhenaiData = Global.GetBufferDataByID(toClient, 2080010);
						if (yongqiData != null && !Global.IsBufferDataOver(yongqiData, 0L) && zhenaiData != null && !Global.IsBufferDataOver(zhenaiData, 0L))
						{
							result = this.YongQiBuff2ZhenAiBuffHurt;
						}
						else
						{
							result = 0.0;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				result = 0.0;
			}
			return result;
		}

		// Token: 0x06000ECF RID: 3791 RVA: 0x000E963C File Offset: 0x000E783C
		public void SetFengHuoJiaRenCouple(int roleid1, int roleid2)
		{
			int min = Math.Min(roleid1, roleid2);
			int max = Math.Max(roleid1, roleid2);
			string[] roles = GameManager.GameConfigMgr.GetGameConfigItemStr("CoupleArenaFengHuo", "0,0").Split(new char[]
			{
				','
			});
			if (roles == null || roles.Length != 2 || Convert.ToInt32(roles[0]) != min || Convert.ToInt32(roles[1]) != max)
			{
				int oldRole = Convert.ToInt32(roles[0]);
				int oldRole2 = Convert.ToInt32(roles[1]);
				Global.UpdateDBGameConfigg("CoupleArenaFengHuo", string.Format("{0},{1}", min, max));
				GameManager.GameConfigMgr.SetGameConfigItem("CoupleArenaFengHuo", string.Format("{0},{1}", min, max));
				this.CheckFengHuoJiaRenChengHao(GameManager.ClientMgr.FindClient(oldRole));
				this.CheckFengHuoJiaRenChengHao(GameManager.ClientMgr.FindClient(oldRole2));
				this.CheckFengHuoJiaRenChengHao(GameManager.ClientMgr.FindClient(roleid1));
				this.CheckFengHuoJiaRenChengHao(GameManager.ClientMgr.FindClient(roleid2));
			}
		}

		// Token: 0x06000ED0 RID: 3792 RVA: 0x000E9758 File Offset: 0x000E7958
		public void CheckFengHuoJiaRenChengHao(GameClient client)
		{
			if (client != null)
			{
				lock (this.Mutex)
				{
					DateTime now = TimeUtil.NowDateTime();
					string[] roles = GameManager.GameConfigMgr.GetGameConfigItemStr("CoupleArenaFengHuo", "0,0").Split(new char[]
					{
						','
					});
					bool isMe = false;
					int i = 0;
					while (roles != null && i < roles.Length && !isMe)
					{
						isMe = (Convert.ToInt32(roles[i]) == client.ClientData.RoleID);
						i++;
					}
					isMe &= MarryLogic.IsMarried(client.ClientData.RoleID);
					isMe &= this.IsInFengHuoJiaRenChengHaoTime(now);
					if (isMe)
					{
						CoupleAreanWarCfg.TimePoint weekFirst = this.WarCfg.TimePoints.First<CoupleAreanWarCfg.TimePoint>();
						int todayWeek = TimeUtil.GetWeekDay1To7(now);
						DateTime endTime = now.AddTicks(-now.TimeOfDay.Ticks);
						endTime = endTime.AddDays((double)(-(double)TimeUtil.GetWeekDay1To7(endTime)));
						endTime = endTime.AddDays((double)weekFirst.Weekday);
						endTime = endTime.AddTicks(weekFirst.DayStartTicks);
						if (todayWeek > weekFirst.Weekday || (todayWeek == weekFirst.Weekday && now.TimeOfDay.Ticks > weekFirst.DayStartTicks))
						{
							endTime = endTime.AddDays(7.0);
						}
						FashionManager.getInstance().GetFashionByMagic(client, FashionIdConsts.CoupleArenaFengHuoJiaRen, endTime.ToString("yyyy-MM-dd HH:mm:ss"));
					}
					else
					{
						FashionManager.getInstance().DelFashionByMagic(client, FashionIdConsts.CoupleArenaFengHuoJiaRen);
					}
				}
			}
		}

		// Token: 0x06000ED1 RID: 3793 RVA: 0x000E9944 File Offset: 0x000E7B44
		public void CheckTipsIconState(GameClient client)
		{
			if (client != null)
			{
				bool bCanGetAward = false;
				lock (this.Mutex)
				{
					int willAwardWeek = 0;
					if (this.IsInWeekAwardTime(TimeUtil.NowDateTime(), out willAwardWeek))
					{
						CoupleArenaCoupleJingJiData coupleData = this.GetCachedCoupleData(client.ClientData.RoleID);
						if (coupleData != null)
						{
							string szWeekAward = Global.GetRoleParamByName(client, "28");
							string[] fields = (szWeekAward == null) ? null : szWeekAward.Split(new char[]
							{
								','
							});
							if (fields == null || fields.Length != 2 || Convert.ToInt32(fields[0]) != willAwardWeek)
							{
								bCanGetAward = true;
							}
						}
					}
				}
				if (client._IconStateMgr.AddFlushIconState(15011, bCanGetAward))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
			}
		}

		// Token: 0x06000ED2 RID: 3794 RVA: 0x000E9A54 File Offset: 0x000E7C54
		private bool IsGongNengOpened(GameClient client)
		{
			return client != null && GlobalNew.IsGongNengOpened(client, GongNengIDs.Marriage, false) && MarryLogic.IsMarried(client.ClientData.RoleID) && GlobalNew.IsGongNengOpened(client, GongNengIDs.CoupleArena, false);
		}

		// Token: 0x06000ED3 RID: 3795 RVA: 0x000E9AB0 File Offset: 0x000E7CB0
		private bool IsInFengHuoJiaRenChengHaoTime(DateTime now)
		{
			int week;
			return this.IsInWeekAwardTime(now, out week);
		}

		// Token: 0x06000ED4 RID: 3796 RVA: 0x000E9AD8 File Offset: 0x000E7CD8
		private bool IsInWeekOnceActTimes(DateTime time)
		{
			int wd = TimeUtil.GetWeekDay1To7(time);
			foreach (CoupleAreanWarCfg.TimePoint tp in this.WarCfg.TimePoints)
			{
				if (tp.Weekday == wd && time.TimeOfDay.Ticks >= tp.DayStartTicks && time.TimeOfDay.Ticks <= tp.DayEndTicks)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000ED5 RID: 3797 RVA: 0x000E9B88 File Offset: 0x000E7D88
		public bool IsNowCanDivorce(DateTime time)
		{
			int wd = TimeUtil.GetWeekDay1To7(time);
			foreach (CoupleAreanWarCfg.TimePoint tp in this.WarCfg.TimePoints)
			{
				if (tp.Weekday == wd && time.TimeOfDay.Ticks >= tp.DayStartTicks - (long)((ulong)-1294967296) && time.TimeOfDay.Ticks <= tp.DayEndTicks + (long)((ulong)-1294967296))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000ED6 RID: 3798 RVA: 0x000E9C48 File Offset: 0x000E7E48
		private bool IsInWeekAwardTime(DateTime time, out int week)
		{
			week = 0;
			CoupleAreanWarCfg.TimePoint weekFirst = this.WarCfg.TimePoints.First<CoupleAreanWarCfg.TimePoint>();
			CoupleAreanWarCfg.TimePoint weekLast = this.WarCfg.TimePoints.Last<CoupleAreanWarCfg.TimePoint>();
			int todayWeek = TimeUtil.GetWeekDay1To7(time);
			bool result;
			if (todayWeek < weekFirst.Weekday || (todayWeek == weekFirst.Weekday && time.TimeOfDay.Ticks < weekFirst.DayStartTicks))
			{
				week = TimeUtil.MakeFirstWeekday(time.AddDays(-7.0));
				result = true;
			}
			else if (todayWeek < weekLast.Weekday || (todayWeek == weekLast.Weekday && time.TimeOfDay.Ticks < weekLast.DayEndTicks + 6000000000L))
			{
				week = 0;
				result = false;
			}
			else
			{
				week = TimeUtil.MakeFirstWeekday(time);
				result = true;
			}
			return result;
		}

		// Token: 0x06000ED7 RID: 3799 RVA: 0x000E9D34 File Offset: 0x000E7F34
		private int CurrRankWeek(DateTime time)
		{
			int currWeekDay = TimeUtil.GetWeekDay1To7(time);
			CoupleAreanWarCfg.TimePoint first = this.WarCfg.TimePoints.First<CoupleAreanWarCfg.TimePoint>();
			int result;
			if (currWeekDay < first.Weekday || (currWeekDay == first.Weekday && time.TimeOfDay.Ticks < first.DayStartTicks))
			{
				result = TimeUtil.MakeFirstWeekday(time.AddDays(-7.0));
			}
			else
			{
				result = TimeUtil.MakeFirstWeekday(time);
			}
			return result;
		}

		// Token: 0x06000ED8 RID: 3800 RVA: 0x000E9DB8 File Offset: 0x000E7FB8
		private bool IsStateWatcher(int roleId)
		{
			bool result;
			lock (this.Mutex)
			{
				result = this.coupleStateWatchers.Contains(roleId);
			}
			return result;
		}

		// Token: 0x06000ED9 RID: 3801 RVA: 0x000E9E0C File Offset: 0x000E800C
		private void RegStateWatcher(int roleId, bool bWatch)
		{
			lock (this.Mutex)
			{
				if (bWatch && !this.coupleStateWatchers.Contains(roleId))
				{
					this.coupleStateWatchers.Add(roleId);
				}
				if (!bWatch)
				{
					this.coupleStateWatchers.Remove(roleId);
				}
			}
		}

		// Token: 0x06000EDA RID: 3802 RVA: 0x000E9E88 File Offset: 0x000E8088
		private ECoupleArenaMatchState GetMatchState(int roleId)
		{
			ECoupleArenaMatchState result;
			lock (this.Mutex)
			{
				ECoupleArenaMatchState state;
				if (!this.RoleMatchStateDict.TryGetValue(roleId, out state))
				{
					state = ECoupleArenaMatchState.Offline;
				}
				result = state;
			}
			return result;
		}

		// Token: 0x06000EDB RID: 3803 RVA: 0x000E9EE8 File Offset: 0x000E80E8
		private void SetMatchState(int roleId, ECoupleArenaMatchState state)
		{
			lock (this.Mutex)
			{
				if (state == ECoupleArenaMatchState.Offline)
				{
					this.RoleMatchStateDict.Remove(roleId);
				}
				else
				{
					this.RoleMatchStateDict[roleId] = state;
				}
				if (state == ECoupleArenaMatchState.Ready)
				{
					this.RoleStartReadyMs[roleId] = TimeUtil.NOW();
				}
				else
				{
					this.RoleStartReadyMs.Remove(roleId);
				}
			}
		}

		// Token: 0x06000EDC RID: 3804 RVA: 0x000E9F84 File Offset: 0x000E8184
		private void NtfCoupleMatchState(int roleId)
		{
			if (this.IsStateWatcher(roleId))
			{
				GameClient client = GameManager.ClientMgr.FindClient(roleId);
				if (client != null)
				{
					CoupleArenaRoleStateData myState = new CoupleArenaRoleStateData
					{
						RoleId = client.ClientData.RoleID
					};
					CoupleArenaRoleStateData spouseState = null;
					if (MarryLogic.IsMarried(client.ClientData.RoleID))
					{
						spouseState = new CoupleArenaRoleStateData
						{
							RoleId = client.ClientData.MyMarriageData.nSpouseID
						};
					}
					lock (this.Mutex)
					{
						myState.MatchState = (int)this.GetMatchState(myState.RoleId);
						if (spouseState != null)
						{
							spouseState.MatchState = (int)this.GetMatchState(spouseState.RoleId);
						}
					}
					List<CoupleArenaRoleStateData> stateList = new List<CoupleArenaRoleStateData>();
					stateList.Add(myState);
					if (spouseState != null)
					{
						stateList.Add(spouseState);
					}
					client.sendCmd<List<CoupleArenaRoleStateData>>(1379, stateList, false);
				}
			}
		}

		// Token: 0x06000EDD RID: 3805 RVA: 0x000EA0B8 File Offset: 0x000E82B8
		public CoupleArenaCoupleJingJiData GetCachedCoupleData(int roleId)
		{
			CoupleArenaCoupleJingJiData result;
			lock (this.Mutex)
			{
				CoupleArenaCoupleJingJiData coupleData = null;
				if (!this.SyncRoleDict.TryGetValue(roleId, out coupleData))
				{
					coupleData = null;
				}
				result = coupleData;
			}
			return result;
		}

		// Token: 0x06000EDE RID: 3806 RVA: 0x000EA11C File Offset: 0x000E831C
		private CoupleArenaCoupleJingJiData ConvertToJiJiData(CoupleArenaCoupleDataK kData)
		{
			return new CoupleArenaCoupleJingJiData
			{
				ManRoleId = kData.ManRoleId,
				ManZoneId = kData.ManZoneId,
				ManSelector = ((kData.ManSelectorData != null) ? DataHelper.BytesToObject<RoleData4Selector>(kData.ManSelectorData, 0, kData.ManSelectorData.Length) : null),
				WifeRoleId = kData.WifeRoleId,
				WifeZoneId = kData.WifeZoneId,
				WifeSelector = ((kData.WifeSelectorData != null) ? DataHelper.BytesToObject<RoleData4Selector>(kData.WifeSelectorData, 0, kData.WifeSelectorData.Length) : null),
				TotalFightTimes = kData.TotalFightTimes,
				WinFightTimes = kData.WinFightTimes,
				LianShengTimes = kData.LianShengTimes,
				DuanWeiType = kData.DuanWeiType,
				DuanWeiLevel = kData.DuanWeiLevel,
				JiFen = kData.JiFen,
				Rank = kData.Rank,
				IsDivorced = kData.IsDivorced
			};
		}

		// Token: 0x06000EDF RID: 3807 RVA: 0x000EA22C File Offset: 0x000E842C
		private void TimerProc(object sender, EventArgs e)
		{
			CoupleArenaSyncData data = TianTiClient.getInstance().CoupleArenaSync(this.SyncDateTime);
			if (data != null)
			{
				lock (this.Mutex)
				{
					this.SyncRankList.Clear();
					this.SyncRoleDict.Clear();
					if (data.RankList != null)
					{
						this.SyncRankList.AddRange(from _r in data.RankList
						select this.ConvertToJiJiData(_r));
						foreach (CoupleArenaCoupleJingJiData r in this.SyncRankList)
						{
							this.SyncRoleDict[r.ManRoleId] = r;
							this.SyncRoleDict[r.WifeRoleId] = r;
						}
					}
					if (this.SyncRankList.Count > 0 && this.SyncRankList[0].Rank == 1 && this.SyncRankList[0].IsDivorced == 0)
					{
						this.SetFengHuoJiaRenCouple(this.SyncRankList[0].ManRoleId, this.SyncRankList[0].WifeRoleId);
					}
					else
					{
						this.SetFengHuoJiaRenCouple(0, 0);
					}
					this.SyncDateTime = data.ModifyTime;
				}
			}
		}

		// Token: 0x06000EE0 RID: 3808 RVA: 0x000EA3FC File Offset: 0x000E85FC
		public bool CanKuaFuLogin(KuaFuServerLoginData kuaFuServerLoginData)
		{
			return true;
		}

		// Token: 0x06000EE1 RID: 3809 RVA: 0x000EA444 File Offset: 0x000E8644
		public bool KuaFuInitGame(GameClient client)
		{
			long gameId = Global.GetClientKuaFuServerLoginData(client).GameId;
			lock (this.Mutex)
			{
				CoupleArenaFuBenData fubenData = null;
				if (!this.GameId2FuBenData.TryGetValue(gameId, out fubenData))
				{
					fubenData = TianTiClient.getInstance().GetFuBenData(gameId);
					if (fubenData != null)
					{
						if (fubenData.FuBenSeq == 0)
						{
							fubenData.FuBenSeq = GameCoreInterface.getinstance().GetNewFuBenSeqId();
						}
						this.GameId2FuBenData.Add(gameId, fubenData);
					}
				}
				if (fubenData == null)
				{
					return false;
				}
				if (fubenData.KfServerId != GameCoreInterface.getinstance().GetLocalServerId())
				{
					return false;
				}
				KuaFuFuBenRoleData roleData = null;
				bool flag2;
				if (fubenData.RoleList != null)
				{
					flag2 = ((roleData = fubenData.RoleList.Find((KuaFuFuBenRoleData _r) => _r.RoleId == client.ClientData.RoleID)) != null);
				}
				else
				{
					flag2 = false;
				}
				if (!flag2)
				{
					return false;
				}
				client.ClientData.MapCode = this.WarCfg.MapCode;
				client.ClientData.BattleWhichSide = roleData.Side;
				int _posx = 0;
				int _posy = 0;
				if (!this.GetBirthPoint(client.ClientData.MapCode, client.ClientData.BattleWhichSide, out _posx, out _posy))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("找不到出生点mapcode={0},side={1}", client.ClientData.MapCode, client.ClientData.BattleWhichSide), null, true);
					return false;
				}
				client.ClientData.PosX = _posx;
				client.ClientData.PosY = _posy;
				Global.GetClientKuaFuServerLoginData(client).FuBenSeqId = fubenData.FuBenSeq;
				client.ClientData.FuBenSeqID = fubenData.FuBenSeq;
			}
			return true;
		}

		// Token: 0x06000EE2 RID: 3810 RVA: 0x000EA6AC File Offset: 0x000E88AC
		public bool ClientRelive(GameClient client)
		{
			bool result;
			if (client.ClientData.MapCode == this.WarCfg.MapCode)
			{
				int toPosX;
				int toPosY;
				if (!this.GetBirthPoint(this.WarCfg.MapCode, client.ClientData.BattleWhichSide, out toPosX, out toPosY))
				{
					result = false;
				}
				else
				{
					client.ClientData.CurrentLifeV = client.ClientData.LifeV;
					client.ClientData.CurrentMagicV = client.ClientData.MagicV;
					client.ClientData.MoveAndActionNum = 0;
					GameManager.ClientMgr.NotifyTeamRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client.ClientData.RoleID, toPosX, toPosY, -1);
					Global.ClientRealive(client, toPosX, toPosY, -1);
					result = true;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000EE3 RID: 3811 RVA: 0x000EA784 File Offset: 0x000E8984
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
				int defaultBirthPosX = this.BirthPointList[side % this.BirthPointList.Count].PosX;
				int defaultBirthPosY = this.BirthPointList[side % this.BirthPointList.Count].PosY;
				int defaultBirthRadius = this.BirthPointList[side % this.BirthPointList.Count].BirthRadius;
				Point newPos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, mapCode, defaultBirthPosX, defaultBirthPosY, defaultBirthRadius);
				toPosX = (int)newPos.X;
				toPosY = (int)newPos.Y;
				toPosX = defaultBirthPosX;
				toPosY = defaultBirthPosY;
				result = true;
			}
			return result;
		}

		// Token: 0x06000EE4 RID: 3812 RVA: 0x000EA848 File Offset: 0x000E8A48
		public void NotifyTimeStateInfoAndScoreInfo(GameClient client)
		{
			lock (this.Mutex)
			{
				CoupleArenaCopyScene copyScene;
				if (this.FuBenSeq2CopyScenes.TryGetValue(client.ClientData.FuBenSeqID, out copyScene))
				{
					client.sendCmd<GameSceneStateTimeData>(827, copyScene.StateTimeData, false);
				}
			}
		}

		// Token: 0x06000EE5 RID: 3813 RVA: 0x000EA930 File Offset: 0x000E8B30
		public void OnLeaveFuBen(GameClient client)
		{
			lock (this.Mutex)
			{
				CoupleArenaCopyScene scene = null;
				if (this.FuBenSeq2CopyScenes.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					if (scene.m_eStatus < GameSceneStatuses.STATUS_BEGIN)
					{
						scene.EnterRoleSide.Remove(client.ClientData.RoleID);
					}
					else if (scene.m_eStatus < GameSceneStatuses.STATUS_END)
					{
						CoupleArenaBuffCfg zhenAiBuffCfg = this.BuffCfgList.Find((CoupleArenaBuffCfg _b) => _b.Type == CoupleAreanConsts.ZhenAiBuffCfgType);
						CoupleArenaBuffCfg yongQiBuffCfg = this.BuffCfgList.Find((CoupleArenaBuffCfg _b) => _b.Type == CoupleAreanConsts.YongQiBuffCfgType);
						this.ModifyBuff(scene, client, BufferItemTypes.CoupleArena_YongQi_Buff, yongQiBuffCfg, false);
						this.ModifyBuff(scene, client, BufferItemTypes.CoupleArena_ZhenAi_Buff, zhenAiBuffCfg, false);
						scene.EnterRoleSide.Remove(client.ClientData.RoleID);
						List<int> leftSide = scene.EnterRoleSide.Values.ToList<int>();
						if (leftSide.Count((int _s) => _s == client.ClientData.BattleWhichSide) <= 0)
						{
							scene.WinSide = ((leftSide.Count > 0) ? leftSide[0] : 0);
							this.ProcessEnd(scene, TimeUtil.NowDateTime(), TimeUtil.NOW());
						}
					}
				}
			}
		}

		// Token: 0x06000EE6 RID: 3814 RVA: 0x000EAB1C File Offset: 0x000E8D1C
		public void AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			if (sceneType == SceneUIClasses.CoupleArena)
			{
				int fuBenSeqId = copyMap.FuBenSeqID;
				int mapCode = copyMap.MapCode;
				lock (this.Mutex)
				{
					CoupleArenaCopyScene scene = null;
					if (!this.FuBenSeq2CopyScenes.TryGetValue(fuBenSeqId, out scene))
					{
						scene = new CoupleArenaCopyScene();
						scene.GameId = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
						scene.FuBenSeq = fuBenSeqId;
						scene.MapCode = mapCode;
						scene.CopyMap = copyMap;
						this.FuBenSeq2CopyScenes[fuBenSeqId] = scene;
					}
					scene.EnterRoleSide[client.ClientData.RoleID] = client.ClientData.BattleWhichSide;
					copyMap.IsKuaFuCopy = true;
					copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)((this.WarCfg.WaitSec + this.WarCfg.FightSec + this.WarCfg.ClearSec + 120) * 1000));
					GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 0, 1, 0, 0, 13);
				}
			}
		}

		// Token: 0x06000EE7 RID: 3815 RVA: 0x000EAC68 File Offset: 0x000E8E68
		public void RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
		{
			if (copyMap != null && sceneType == SceneUIClasses.CoupleArena)
			{
				lock (this.Mutex)
				{
					CoupleArenaCopyScene scene = null;
					if (this.FuBenSeq2CopyScenes.TryGetValue(copyMap.FuBenSeqID, out scene))
					{
						this.FuBenSeq2CopyScenes.Remove(copyMap.FuBenSeqID);
						this.GameId2FuBenData.Remove((long)scene.GameId);
					}
				}
			}
		}

		// Token: 0x06000EE8 RID: 3816 RVA: 0x000EAD04 File Offset: 0x000E8F04
		public void UpdateCopyScene()
		{
			DateTime now = TimeUtil.NowDateTime();
			long nowTicks = now.Ticks / 10000L;
			if (nowTicks >= CoupleArenaManager.NextHeartBeatTicks)
			{
				CoupleArenaManager.NextHeartBeatTicks = nowTicks + 1020L;
				lock (this.Mutex)
				{
					foreach (CoupleArenaCopyScene scene in this.FuBenSeq2CopyScenes.Values.ToList<CoupleArenaCopyScene>())
					{
						scene.m_lPrevUpdateTime = scene.m_lCurrUpdateTime;
						scene.m_lCurrUpdateTime = nowTicks;
						if (scene.m_eStatus == GameSceneStatuses.STATUS_NULL)
						{
							this.NtfBuffHoldData(scene);
							scene.m_lPrepareTime = nowTicks;
							scene.m_lBeginTime = nowTicks + (long)(this.WarCfg.WaitSec * 1000);
							scene.m_eStatus = GameSceneStatuses.STATUS_PREPARE;
							scene.StateTimeData.GameType = 13;
							scene.StateTimeData.State = (int)scene.m_eStatus;
							scene.StateTimeData.EndTicks = scene.m_lBeginTime;
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
						}
						else if (scene.m_eStatus == GameSceneStatuses.STATUS_PREPARE)
						{
							if (nowTicks >= scene.m_lBeginTime)
							{
								if (scene.EnterRoleSide.Values.ToList<int>().Distinct<int>().Count<int>() <= 1)
								{
									scene.WinSide = 0;
									scene.m_eStatus = GameSceneStatuses.STATUS_END;
								}
								else
								{
									this.NtfBuffHoldData(scene);
									scene.m_eStatus = GameSceneStatuses.STATUS_BEGIN;
									scene.m_lEndTime = nowTicks + (long)(this.WarCfg.FightSec * 1000);
									scene.StateTimeData.GameType = 13;
									scene.StateTimeData.State = (int)scene.m_eStatus;
									scene.StateTimeData.EndTicks = scene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
									scene.CopyMap.AddGuangMuEvent(1, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(scene.CopyMap.MapCode, scene.CopyMap.CopyMapID, 1, 0);
									scene.CopyMap.AddGuangMuEvent(2, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(scene.CopyMap.MapCode, scene.CopyMap.CopyMapID, 2, 0);
								}
							}
						}
						else if (scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
						{
							if (nowTicks >= scene.m_lEndTime)
							{
								scene.m_eStatus = GameSceneStatuses.STATUS_END;
								if (!scene.EnterRoleSide.TryGetValue(scene.ZhenAiBuff_Role, out scene.WinSide))
								{
									scene.WinSide = 0;
								}
							}
							else if (scene.EnterRoleSide.ContainsKey(scene.ZhenAiBuff_Role) && nowTicks - scene.ZhenAiBuff_StartMs >= (long)(this.ZhenAiBuffHoldWinSec * 1000))
							{
								scene.m_eStatus = GameSceneStatuses.STATUS_END;
								scene.WinSide = scene.EnterRoleSide[scene.ZhenAiBuff_Role];
							}
							else
							{
								this.CheckFlushZhenAiMonster(scene);
								this.CheckFlushYongQiMonster(scene);
							}
						}
						else if (scene.m_eStatus == GameSceneStatuses.STATUS_END)
						{
							this.ProcessEnd(scene, now, nowTicks);
						}
						else if (scene.m_eStatus == GameSceneStatuses.STATUS_AWARD)
						{
							if (nowTicks >= scene.m_lLeaveTime)
							{
								scene.m_eStatus = GameSceneStatuses.STATUS_CLEAR;
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
									DataHelper.WriteExceptionLogEx(ex, "情侣竞技系统清场调度异常");
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000EE9 RID: 3817 RVA: 0x000EB1C0 File Offset: 0x000E93C0
		private void CheckFlushZhenAiMonster(CoupleArenaCopyScene scene)
		{
			if (!scene.IsZhenAiMonsterExist && !scene.EnterRoleSide.ContainsKey(scene.ZhenAiBuff_Role))
			{
				GameMap gameMap = null;
				if (!GameManager.MapMgr.DictMaps.TryGetValue(scene.MapCode, out gameMap))
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("缺少情侣竞技地图 {0}", scene.MapCode), null, true);
				}
				else
				{
					CoupleArenaBuffCfg buffCfg = this.BuffCfgList.Find((CoupleArenaBuffCfg _b) => _b.Type == CoupleAreanConsts.ZhenAiBuffCfgType);
					if (buffCfg != null)
					{
						CoupleArenaBuffCfg.RandPos pos = buffCfg.RandPosList[Global.GetRandomNumber(0, buffCfg.RandPosList.Count)];
						GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.MapCode, buffCfg.MonsterId, scene.CopyMap.CopyMapID, 1, pos.X / gameMap.MapGridWidth, pos.Y / gameMap.MapGridHeight, pos.R, 0, SceneUIClasses.CoupleArena, null, null);
						scene.IsZhenAiMonsterExist = true;
					}
				}
			}
		}

		// Token: 0x06000EEA RID: 3818 RVA: 0x000EB300 File Offset: 0x000E9500
		private void CheckFlushYongQiMonster(CoupleArenaCopyScene scene)
		{
			CoupleArenaBuffCfg buffCfg = this.BuffCfgList.Find((CoupleArenaBuffCfg _b) => _b.Type == CoupleAreanConsts.YongQiBuffCfgType);
			if (buffCfg != null)
			{
				bool isInFlusTime = false;
				foreach (int sec in buffCfg.FlushSecList)
				{
					if (scene.m_lPrevUpdateTime - scene.m_lBeginTime <= (long)(sec * 1000) && scene.m_lCurrUpdateTime - scene.m_lBeginTime >= (long)(sec * 1000))
					{
						isInFlusTime = true;
						break;
					}
				}
				if (isInFlusTime && !scene.IsYongQiMonsterExist && !scene.EnterRoleSide.ContainsKey(scene.YongQiBuff_Role))
				{
					GameMap gameMap = null;
					if (!GameManager.MapMgr.DictMaps.TryGetValue(scene.MapCode, out gameMap))
					{
						LogManager.WriteLog(LogTypes.Fatal, string.Format("缺少情侣竞技地图 {0}", scene.MapCode), null, true);
					}
					else
					{
						CoupleArenaBuffCfg.RandPos pos = buffCfg.RandPosList[Global.GetRandomNumber(0, buffCfg.RandPosList.Count)];
						GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.MapCode, buffCfg.MonsterId, scene.CopyMap.CopyMapID, 1, pos.X / gameMap.MapGridWidth, pos.Y / gameMap.MapGridHeight, pos.R, 0, SceneUIClasses.CoupleArena, null, null);
						scene.IsYongQiMonsterExist = true;
					}
				}
			}
		}

		// Token: 0x06000EEB RID: 3819 RVA: 0x000EB4B4 File Offset: 0x000E96B4
		private void ProcessEnd(CoupleArenaCopyScene scene, DateTime now, long nowTicks)
		{
			GameManager.CopyMapMgr.KillAllMonster(scene.CopyMap);
			scene.m_eStatus = GameSceneStatuses.STATUS_AWARD;
			scene.m_lEndTime = nowTicks;
			scene.m_lLeaveTime = scene.m_lEndTime + (long)(this.WarCfg.ClearSec * 1000);
			scene.StateTimeData.GameType = 13;
			scene.StateTimeData.State = 3;
			scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
			GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
			CoupleArenaFuBenData fubenData = null;
			if (this.GameId2FuBenData.TryGetValue((long)scene.GameId, out fubenData))
			{
				List<RoleData4Selector> selectorList = new List<RoleData4Selector>();
				foreach (KuaFuFuBenRoleData roledata in fubenData.RoleList)
				{
					RoleData4Selector _roleInfo = Global.sendToDB<RoleData4Selector, int>(10232, roledata.RoleId, roledata.ServerId);
					if (_roleInfo == null || _roleInfo.RoleID <= 0)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("加载RoleData4Selector失败, serverid={0}, roleid={1}", roledata.ServerId, roledata.RoleId), null, true);
						return;
					}
					selectorList.Add(_roleInfo);
				}
				if (!MarryLogic.SameSexMarry(false))
				{
					if (selectorList[0].RoleSex == 1)
					{
						RoleData4Selector tmp = selectorList[0];
						selectorList[0] = selectorList[1];
						selectorList[1] = tmp;
					}
					if (selectorList[2].RoleSex == 1)
					{
						RoleData4Selector tmp = selectorList[2];
						selectorList[2] = selectorList[3];
						selectorList[3] = tmp;
					}
				}
				CoupleArenaPkResultReq req = new CoupleArenaPkResultReq();
				req.GameId = (long)scene.GameId;
				req.winSide = scene.WinSide;
				req.ManRole1 = selectorList[0].RoleID;
				req.ManZoneId1 = selectorList[0].ZoneId;
				req.ManSelector1 = DataHelper.ObjectToBytes<RoleData4Selector>(selectorList[0]);
				req.WifeRole1 = selectorList[1].RoleID;
				req.WifeZoneId1 = selectorList[1].ZoneId;
				req.WifeSelector1 = DataHelper.ObjectToBytes<RoleData4Selector>(selectorList[1]);
				req.ManRole2 = selectorList[2].RoleID;
				req.ManZoneId2 = selectorList[2].ZoneId;
				req.ManSelector2 = DataHelper.ObjectToBytes<RoleData4Selector>(selectorList[2]);
				req.WifeRole2 = selectorList[3].RoleID;
				req.WifeZoneId2 = selectorList[3].ZoneId;
				req.WifeSelector2 = DataHelper.ObjectToBytes<RoleData4Selector>(selectorList[3]);
				CoupleArenaPkResultRsp rsp = TianTiClient.getInstance().CoupleArenaPkResult(req);
				if (rsp != null)
				{
					if (rsp.Couple1RetData != null)
					{
						if (rsp.Couple1RetData.Result != 0)
						{
							Global.sendToDB<bool, CoupleArenaZhanBaoSaveDbData>(1382, new CoupleArenaZhanBaoSaveDbData
							{
								FromMan = req.ManRole1,
								FromWife = req.WifeRole1,
								FirstWeekday = TimeUtil.MakeFirstWeekday(now),
								ZhanBao = new CoupleArenaZhanBaoItemData
								{
									TargetManZoneId = req.ManZoneId2,
									TargetManRoleName = selectorList[2].RoleName,
									TargetWifeZoneId = req.WifeZoneId2,
									TargetWifeRoleName = selectorList[3].RoleName,
									IsWin = (rsp.Couple1RetData.Result == 1),
									GetJiFen = rsp.Couple1RetData.GetJiFen
								}
							}, fubenData.RoleList[0].ServerId);
						}
						this.NtfAwardData(req.ManRole1, rsp.Couple1RetData);
						this.NtfAwardData(req.WifeRole1, rsp.Couple1RetData);
					}
					if (rsp.Couple2RetData != null)
					{
						if (rsp.Couple2RetData.Result != 0)
						{
							Global.sendToDB<bool, CoupleArenaZhanBaoSaveDbData>(1382, new CoupleArenaZhanBaoSaveDbData
							{
								FirstWeekday = TimeUtil.MakeFirstWeekday(now),
								FromMan = req.ManRole2,
								FromWife = req.WifeRole2,
								ZhanBao = new CoupleArenaZhanBaoItemData
								{
									TargetManZoneId = req.ManZoneId1,
									TargetManRoleName = selectorList[0].RoleName,
									TargetWifeZoneId = req.WifeZoneId1,
									TargetWifeRoleName = selectorList[1].RoleName,
									IsWin = (rsp.Couple2RetData.Result == 1),
									GetJiFen = rsp.Couple2RetData.GetJiFen
								}
							}, fubenData.RoleList[2].ServerId);
						}
						this.NtfAwardData(req.ManRole2, rsp.Couple2RetData);
						this.NtfAwardData(req.WifeRole2, rsp.Couple2RetData);
					}
				}
			}
		}

		// Token: 0x06000EEC RID: 3820 RVA: 0x000EBA50 File Offset: 0x000E9C50
		private void NtfAwardData(int roleid, CoupleArenaPkResultItem retItem)
		{
			GameClient client = GameManager.ClientMgr.FindClient(roleid);
			if (client != null && client.ClientData.MapCode == this.WarCfg.MapCode)
			{
				CoupleAreanDuanWeiCfg cfg = this.DuanWeiCfgList.Find((CoupleAreanDuanWeiCfg _d) => _d.Type == retItem.OldDuanWeiType && _d.Level == retItem.OldDuanWeiLevel);
				if (cfg == null)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("NtfAwardData 段位配置找不到 type={0},level={1}", retItem.OldDuanWeiType, retItem.OldDuanWeiLevel), null, true);
				}
				else
				{
					DateTime now = TimeUtil.NowDateTime();
					int getRongYaoTimes = 0;
					bool canGetRongYao = false;
					string szWeekRongYao = Global.GetRoleParamByName(client, "27");
					if (!string.IsNullOrEmpty(szWeekRongYao))
					{
						string[] fields = szWeekRongYao.Split(new char[]
						{
							','
						});
						if (fields != null && fields.Length == 2 && Convert.ToInt32(fields[0]) == TimeUtil.MakeFirstWeekday(now))
						{
							getRongYaoTimes = Convert.ToInt32(fields[1]);
						}
					}
					CoupleArenaPkResultData data = new CoupleArenaPkResultData();
					data.PKResult = retItem.Result;
					data.DuanWeiType = retItem.NewDuanWeiType;
					data.DuanWeiLevel = retItem.NewDuanWeiLevel;
					data.GetJiFen = retItem.GetJiFen;
					if (retItem.Result != 0 && getRongYaoTimes < cfg.WeekGetRongYaoTimes)
					{
						data.GetRongYao = ((retItem.Result == 1) ? cfg.WinRongYao : cfg.LoseRongYao);
						canGetRongYao = true;
					}
					if (canGetRongYao)
					{
						GameManager.ClientMgr.ModifyTianTiRongYaoValue(client, data.GetRongYao, "情侣竞技系统获得荣耀", true);
						Global.SaveRoleParamsStringToDB(client, "27", string.Format("{0},{1}", TimeUtil.MakeFirstWeekday(now), getRongYaoTimes + 1), true);
					}
					client.sendCmd<CoupleArenaPkResultData>(1378, data, false);
				}
			}
		}

		// Token: 0x06000EED RID: 3821 RVA: 0x000EBCBC File Offset: 0x000E9EBC
		private void OnMonsterDead(CoupleArenaCopyScene scene, GameClient client, Monster monster)
		{
			lock (this.Mutex)
			{
				if (scene.m_eStatus >= GameSceneStatuses.STATUS_BEGIN && scene.m_eStatus < GameSceneStatuses.STATUS_END)
				{
					if (scene.EnterRoleSide.ContainsKey(client.ClientData.RoleID))
					{
						CoupleArenaBuffCfg zhenAiBuffCfg = this.BuffCfgList.Find((CoupleArenaBuffCfg _b) => _b.Type == CoupleAreanConsts.ZhenAiBuffCfgType);
						CoupleArenaBuffCfg yongQiBuffCfg = this.BuffCfgList.Find((CoupleArenaBuffCfg _b) => _b.Type == CoupleAreanConsts.YongQiBuffCfgType);
						if (scene.IsZhenAiMonsterExist && monster.MonsterInfo.ExtensionID == zhenAiBuffCfg.MonsterId)
						{
							scene.IsZhenAiMonsterExist = false;
							this.ModifyBuff(scene, client, BufferItemTypes.CoupleArena_YongQi_Buff, yongQiBuffCfg, false);
							this.ModifyBuff(scene, client, BufferItemTypes.CoupleArena_ZhenAi_Buff, zhenAiBuffCfg, true);
						}
						if (scene.IsYongQiMonsterExist && monster.MonsterInfo.ExtensionID == yongQiBuffCfg.MonsterId)
						{
							scene.IsYongQiMonsterExist = false;
							this.ModifyBuff(scene, client, BufferItemTypes.CoupleArena_YongQi_Buff, yongQiBuffCfg, true);
						}
					}
				}
			}
		}

		// Token: 0x06000EEE RID: 3822 RVA: 0x000EBE7C File Offset: 0x000EA07C
		private void OnPlayerDead(CoupleArenaCopyScene scene, GameClient deader, GameClient killer)
		{
			lock (this.Mutex)
			{
				if (scene.m_eStatus >= GameSceneStatuses.STATUS_BEGIN && scene.m_eStatus < GameSceneStatuses.STATUS_END)
				{
					if (scene.EnterRoleSide.ContainsKey(deader.ClientData.RoleID))
					{
						if (killer == null || scene.EnterRoleSide.ContainsKey(killer.ClientData.RoleID))
						{
							CoupleArenaBuffCfg zhenAiBuffCfg = this.BuffCfgList.Find((CoupleArenaBuffCfg _b) => _b.Type == CoupleAreanConsts.ZhenAiBuffCfgType);
							CoupleArenaBuffCfg yongQiBuffCfg = this.BuffCfgList.Find((CoupleArenaBuffCfg _b) => _b.Type == CoupleAreanConsts.YongQiBuffCfgType);
							if (scene.ZhenAiBuff_Role == deader.ClientData.RoleID)
							{
								this.ModifyBuff(scene, deader, BufferItemTypes.CoupleArena_ZhenAi_Buff, zhenAiBuffCfg, false);
								this.ModifyBuff(scene, killer, BufferItemTypes.CoupleArena_YongQi_Buff, yongQiBuffCfg, false);
								this.ModifyBuff(scene, killer, BufferItemTypes.CoupleArena_ZhenAi_Buff, zhenAiBuffCfg, true);
							}
							if (scene.YongQiBuff_Role == deader.ClientData.RoleID)
							{
								this.ModifyBuff(scene, deader, BufferItemTypes.CoupleArena_YongQi_Buff, yongQiBuffCfg, false);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000EEF RID: 3823 RVA: 0x000EC00C File Offset: 0x000EA20C
		private void ModifyBuff(CoupleArenaCopyScene scene, GameClient client, BufferItemTypes buffType, CoupleArenaBuffCfg buffCfg, bool bAdd)
		{
			if (scene != null && client != null && buffCfg != null)
			{
				lock (this.Mutex)
				{
					bool bChanged = false;
					BufferData buffData = Global.GetBufferDataByID(client, (int)buffType);
					int noSaveDbBuffType = 1;
					if (bAdd && (buffData == null || Global.IsBufferDataOver(buffData, 0L)))
					{
						if (buffType != BufferItemTypes.CoupleArena_YongQi_Buff || scene.ZhenAiBuff_Role != client.ClientData.RoleID)
						{
							double[] bufferParams = new double[]
							{
								1.0
							};
							Global.UpdateBufferData(client, buffType, bufferParams, noSaveDbBuffType, true);
							foreach (KeyValuePair<ExtPropIndexes, double> prop in buffCfg.ExtProps)
							{
								client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
								{
									21,
									(int)prop.Key,
									prop.Value
								});
							}
							bChanged = true;
						}
					}
					if (!bAdd && buffData != null && !Global.IsBufferDataOver(buffData, 0L))
					{
						double[] array = new double[1];
						double[] bufferParams = array;
						Global.UpdateBufferData(client, buffType, bufferParams, noSaveDbBuffType, true);
						foreach (KeyValuePair<ExtPropIndexes, double> prop in buffCfg.ExtProps)
						{
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								21,
								(int)prop.Key,
								0
							});
						}
						bChanged = true;
					}
					if (bChanged)
					{
						if (buffType == BufferItemTypes.CoupleArena_ZhenAi_Buff)
						{
							if (bAdd)
							{
								scene.ZhenAiBuff_Role = client.ClientData.RoleID;
								scene.ZhenAiBuff_StartMs = TimeUtil.NOW();
							}
							else
							{
								scene.ZhenAiBuff_Role = 0;
							}
						}
						else if (buffType == BufferItemTypes.CoupleArena_YongQi_Buff)
						{
							if (bAdd)
							{
								scene.YongQiBuff_Role = client.ClientData.RoleID;
							}
							else
							{
								scene.YongQiBuff_Role = 0;
							}
						}
						this.NtfBuffHoldData(scene);
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					}
				}
			}
		}

		// Token: 0x06000EF0 RID: 3824 RVA: 0x000EC33C File Offset: 0x000EA53C
		private void NtfBuffHoldData(CoupleArenaCopyScene scene)
		{
			lock (this.Mutex)
			{
				CoupleArenaFuBenData fuben;
				if (this.GameId2FuBenData.TryGetValue((long)scene.GameId, out fuben))
				{
					CoupleArenaBuffHoldData holdData = new CoupleArenaBuffHoldData();
					GameClient client = GameManager.ClientMgr.FindClient(scene.ZhenAiBuff_Role);
					if (client != null && scene.EnterRoleSide.ContainsKey(client.ClientData.RoleID))
					{
						holdData.IsZhenAiBuffValid = true;
						holdData.ZhenAiHolderZoneId = client.ClientData.ZoneID;
						holdData.ZhenAiHolderRname = client.ClientData.RoleName;
					}
					else
					{
						holdData.IsZhenAiBuffValid = false;
					}
					client = GameManager.ClientMgr.FindClient(scene.YongQiBuff_Role);
					if (client != null && scene.EnterRoleSide.ContainsKey(scene.YongQiBuff_Role))
					{
						holdData.IsYongQiBuffValid = true;
						holdData.YongQiHolderZoneId = client.ClientData.ZoneID;
						holdData.YongQiHolderRname = client.ClientData.RoleName;
					}
					else
					{
						holdData.IsYongQiBuffValid = false;
					}
					GameManager.ClientMgr.BroadSpecialCopyMapMessage<CoupleArenaBuffHoldData>(1381, holdData, scene.CopyMap);
				}
			}
		}

		// Token: 0x040016C7 RID: 5831
		private object Mutex = new object();

		// Token: 0x040016C8 RID: 5832
		private DateTime SyncDateTime = DateTime.MinValue;

		// Token: 0x040016C9 RID: 5833
		private List<CoupleArenaCoupleJingJiData> SyncRankList = new List<CoupleArenaCoupleJingJiData>();

		// Token: 0x040016CA RID: 5834
		private Dictionary<int, CoupleArenaCoupleJingJiData> SyncRoleDict = new Dictionary<int, CoupleArenaCoupleJingJiData>();

		// Token: 0x040016CB RID: 5835
		private Dictionary<int, ECoupleArenaMatchState> RoleMatchStateDict = new Dictionary<int, ECoupleArenaMatchState>();

		// Token: 0x040016CC RID: 5836
		private Dictionary<int, long> RoleStartReadyMs = new Dictionary<int, long>();

		// Token: 0x040016CD RID: 5837
		private Dictionary<int, int> RoleMatchKeyDict = new Dictionary<int, int>();

		// Token: 0x040016CE RID: 5838
		private HashSet<int> coupleStateWatchers = new HashSet<int>();

		// Token: 0x040016CF RID: 5839
		private CoupleAreanWarCfg WarCfg = new CoupleAreanWarCfg();

		// Token: 0x040016D0 RID: 5840
		private List<CoupleAreanDuanWeiCfg> DuanWeiCfgList = new List<CoupleAreanDuanWeiCfg>();

		// Token: 0x040016D1 RID: 5841
		private List<CoupleAreanWeekRankAwardCfg> WeekAwardCfgList = new List<CoupleAreanWeekRankAwardCfg>();

		// Token: 0x040016D2 RID: 5842
		private List<CoupleArenaBuffCfg> BuffCfgList = new List<CoupleArenaBuffCfg>();

		// Token: 0x040016D3 RID: 5843
		private List<TianTiBirthPoint> BirthPointList = new List<TianTiBirthPoint>();

		// Token: 0x040016D4 RID: 5844
		private int ZhenAiBuffHoldWinSec = 60;

		// Token: 0x040016D5 RID: 5845
		private double YongQiBuff2ZhenAiBuffHurt = 0.2;

		// Token: 0x040016D6 RID: 5846
		private Dictionary<long, CoupleArenaFuBenData> GameId2FuBenData = new Dictionary<long, CoupleArenaFuBenData>();

		// Token: 0x040016D7 RID: 5847
		private Dictionary<int, CoupleArenaCopyScene> FuBenSeq2CopyScenes = new Dictionary<int, CoupleArenaCopyScene>();

		// Token: 0x040016D8 RID: 5848
		private static long NextHeartBeatTicks = 0L;
	}
}
