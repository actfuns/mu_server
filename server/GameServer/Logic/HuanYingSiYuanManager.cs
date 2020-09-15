using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.Ornament;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x02000509 RID: 1289
	public class HuanYingSiYuanManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		// Token: 0x060017C1 RID: 6081 RVA: 0x00173838 File Offset: 0x00171A38
		public static HuanYingSiYuanManager getInstance()
		{
			return HuanYingSiYuanManager.instance;
		}

		// Token: 0x060017C2 RID: 6082 RVA: 0x00173850 File Offset: 0x00171A50
		public bool initialize()
		{
			return this.InitConfig();
		}

		// Token: 0x060017C3 RID: 6083 RVA: 0x00173874 File Offset: 0x00171A74
		public bool initialize(ICoreInterface coreInterface)
		{
			return true;
		}

		// Token: 0x060017C4 RID: 6084 RVA: 0x00173888 File Offset: 0x00171A88
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(820, 1, 1, HuanYingSiYuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(821, 1, 1, HuanYingSiYuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(824, 2, 2, HuanYingSiYuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(822, 1, 1, HuanYingSiYuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(828, 1, 1, HuanYingSiYuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(826, 1, 1, HuanYingSiYuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(10001, 25, HuanYingSiYuanManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10000, 25, HuanYingSiYuanManager.getInstance());
			GlobalEventSource.getInstance().registerListener(31, HuanYingSiYuanManager.getInstance());
			GlobalEventSource.getInstance().registerListener(10, HuanYingSiYuanManager.getInstance());
			return true;
		}

		// Token: 0x060017C5 RID: 6085 RVA: 0x00173980 File Offset: 0x00171B80
		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(10001, 25, HuanYingSiYuanManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(10000, 25, HuanYingSiYuanManager.getInstance());
			GlobalEventSource.getInstance().removeListener(31, HuanYingSiYuanManager.getInstance());
			GlobalEventSource.getInstance().removeListener(10, HuanYingSiYuanManager.getInstance());
			return true;
		}

		// Token: 0x060017C6 RID: 6086 RVA: 0x001739E8 File Offset: 0x00171BE8
		public bool destroy()
		{
			return true;
		}

		// Token: 0x060017C7 RID: 6087 RVA: 0x001739FC File Offset: 0x00171BFC
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x060017C8 RID: 6088 RVA: 0x00173A10 File Offset: 0x00171C10
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			switch (nID)
			{
			case 820:
				return this.ProcessHuanYingSiYuanEnqueueCmd(client, nID, bytes, cmdParams);
			case 821:
				return this.ProcessHuanYingSiYuanDequeueCmd(client, nID, bytes, cmdParams);
			case 822:
				return this.ProcessHuanYingSiYuanQueueRoleCountCmd(client, nID, bytes, cmdParams);
			case 824:
				return this.ProcessHuanYingSiYuanEnterRespondCmd(client, nID, bytes, cmdParams);
			case 826:
				return this.ProcessHuanYingSiYuanScoreInfoCmd(client, nID, bytes, cmdParams);
			case 828:
				return this.ProcessHuanYingSiYuanSuccessCountCmd(client, nID, bytes, cmdParams);
			}
			return true;
		}

		// Token: 0x060017C9 RID: 6089 RVA: 0x00173AAC File Offset: 0x00171CAC
		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 31)
			{
				ClientRegionEventObject e = eventObject as ClientRegionEventObject;
				if (null != e)
				{
					if (e.EventType == 1 && e.Flag == 1)
					{
						this.SubmitShengBei(e.Client);
					}
				}
			}
			else if (eventType == 10)
			{
				PlayerDeadEventObject playerDeadEvent = eventObject as PlayerDeadEventObject;
				if (null != playerDeadEvent)
				{
					if (playerDeadEvent.Type == PlayerDeadEventTypes.ByRole)
					{
						this.OnKillRole(playerDeadEvent.getAttackerRole(), playerDeadEvent.getPlayer());
					}
					else
					{
						this.TryLostShengBei(playerDeadEvent.getPlayer());
					}
				}
			}
		}

		// Token: 0x060017CA RID: 6090 RVA: 0x00173B64 File Offset: 0x00171D64
		public void processEvent(EventObjectEx eventObject)
		{
			switch (eventObject.EventType)
			{
			case 10000:
			{
				KuaFuFuBenRoleCountEvent e = eventObject as KuaFuFuBenRoleCountEvent;
				if (null != e)
				{
					GameClient client = GameManager.ClientMgr.FindClient(e.RoleId);
					if (null != client)
					{
						client.sendCmd<int>(822, e.RoleCount, false);
					}
					eventObject.Handled = true;
				}
				break;
			}
			case 10001:
			{
				KuaFuNotifyEnterGameEvent e2 = eventObject as KuaFuNotifyEnterGameEvent;
				if (null != e2)
				{
					KuaFuServerLoginData kuaFuServerLoginData = e2.Arg as KuaFuServerLoginData;
					if (null != kuaFuServerLoginData)
					{
						GameClient client = GameManager.ClientMgr.FindClient(kuaFuServerLoginData.RoleId);
						if (null != client)
						{
							KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
							if (null != clientKuaFuServerLoginData)
							{
								clientKuaFuServerLoginData.RoleId = kuaFuServerLoginData.RoleId;
								clientKuaFuServerLoginData.GameId = kuaFuServerLoginData.GameId;
								clientKuaFuServerLoginData.GameType = kuaFuServerLoginData.GameType;
								clientKuaFuServerLoginData.EndTicks = kuaFuServerLoginData.EndTicks;
								clientKuaFuServerLoginData.ServerId = kuaFuServerLoginData.ServerId;
								clientKuaFuServerLoginData.ServerIp = kuaFuServerLoginData.ServerIp;
								clientKuaFuServerLoginData.ServerPort = kuaFuServerLoginData.ServerPort;
								clientKuaFuServerLoginData.FuBenSeqId = kuaFuServerLoginData.FuBenSeqId;
								client.sendCmd<KuaFuServerLoginData>(823, clientKuaFuServerLoginData, false);
							}
						}
					}
					eventObject.Handled = true;
				}
				break;
			}
			}
		}

		// Token: 0x060017CB RID: 6091 RVA: 0x00173CE0 File Offset: 0x00171EE0
		public bool InitConfig()
		{
			bool success = true;
			string fileName = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.ShengBeiDataDict.Clear();
					fileName = "Config/HolyGrail.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					XElement xml = XElement.Load(fullPathFileName);
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						ShengBeiData item = new ShengBeiData();
						XElement node;
						item.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item.MonsterID = (int)Global.GetSafeAttributeLong(node, "MonsterID");
						item.Time = (int)Global.GetSafeAttributeLong(node, "Time");
						item.GoodsID = (int)Global.GetSafeAttributeLong(node, "GoodsID");
						item.Score = (int)Global.GetSafeAttributeLong(node, "Score");
						item.PosX = (int)Global.GetSafeAttributeLong(node, "PosX");
						item.PosY = (int)Global.GetSafeAttributeLong(node, "PosY");
						EquipPropItem propItem = GameManager.EquipPropsMgr.FindEquipPropItem(item.GoodsID);
						if (null != propItem)
						{
							item.BufferProps = propItem.ExtProps;
						}
						else
						{
							success = false;
							LogManager.WriteLog(LogTypes.Fatal, "幻影寺院的圣杯Buffer的GoodsID在物品表中找不到", null, true);
						}
						this.RuntimeData.ShengBeiDataDict[item.ID] = item;
					}
					this.RuntimeData.MapBirthPointDict.Clear();
					fileName = "Config/TempleMirageRebirth.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						HuanYingSiYuanBirthPoint item2 = new HuanYingSiYuanBirthPoint();
						XElement node;
						item2.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item2.PosX = (int)Global.GetSafeAttributeLong(node, "PosX");
						item2.PosY = (int)Global.GetSafeAttributeLong(node, "PosY");
						item2.BirthRadius = (int)Global.GetSafeAttributeLong(node, "BirthRadius");
						this.RuntimeData.MapBirthPointDict[item2.ID] = item2;
					}
					this.RuntimeData.ContinuityKillAwardDict.Clear();
					fileName = "Config/ContinuityKillAward.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						ContinuityKillAward item3 = new ContinuityKillAward();
						XElement node;
						item3.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item3.Num = (int)Global.GetSafeAttributeLong(node, "Num");
						item3.Score = (int)Global.GetSafeAttributeLong(node, "Score");
						this.RuntimeData.ContinuityKillAwardDict[item3.Num] = item3;
					}
					this.RuntimeData.MapCode = 0;
					fileName = "Config/TempleMirage.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					using (IEnumerator<XElement> enumerator = nodes.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							XElement node = enumerator.Current;
							this.RuntimeData.MapCode = (int)Global.GetSafeAttributeLong(node, "MapCode");
							this.RuntimeData.MinZhuanSheng = (int)Global.GetSafeAttributeLong(node, "MinZhuanSheng");
							this.RuntimeData.MinLevel = (int)Global.GetSafeAttributeLong(node, "MinLevel");
							this.RuntimeData.MinRequestNum = (int)Global.GetSafeAttributeLong(node, "MinRequestNum");
							this.RuntimeData.MaxEnterNum = (int)Global.GetSafeAttributeLong(node, "MaxEnterNum");
							this.RuntimeData.WaitingEnterSecs = (int)Global.GetSafeAttributeLong(node, "WaitingEnterSecs");
							this.RuntimeData.PrepareSecs = (int)Global.GetSafeAttributeLong(node, "PrepareSecs");
							this.RuntimeData.FightingSecs = (int)Global.GetSafeAttributeLong(node, "FightingSecs");
							this.RuntimeData.ClearRolesSecs = (int)Global.GetSafeAttributeLong(node, "ClearRolesSecs");
							if (!ConfigParser.ParserTimeRangeList(this.RuntimeData.TimePoints, Global.GetSafeAttributeStr(node, "TimePoints"), true, '|', '-'))
							{
								success = false;
								LogManager.WriteLog(LogTypes.Fatal, "读取幻影寺院时间配置(TimePoints)出错", null, true);
							}
							GameMap gameMap = null;
							if (!GameManager.MapMgr.DictMaps.TryGetValue(this.RuntimeData.MapCode, out gameMap))
							{
								LogManager.WriteLog(LogTypes.Fatal, string.Format("缺少幻影寺院地图 {0}", this.RuntimeData.MapCode), null, true);
							}
							this.RuntimeData.MapGridWidth = gameMap.MapGridWidth;
							this.RuntimeData.MapGridHeight = gameMap.MapGridHeight;
						}
					}
					this.RuntimeData.TempleMirageEXPAward = GameManager.systemParamsList.GetParamValueIntByName("TempleMirageEXPAward", -1);
					this.RuntimeData.TempleMirageWin = (int)GameManager.systemParamsList.GetParamValueIntByName("TempleMirageWin", -1);
					this.RuntimeData.TempleMiragePK = (int)GameManager.systemParamsList.GetParamValueIntByName("TempleMiragePK", -1);
					this.RuntimeData.TempleMirageMinJiFen = (int)GameManager.systemParamsList.GetParamValueIntByName("TempleMirageMinJiFen", -1);
					this.RuntimeData.AwardGoods = GameManager.systemParamsList.GetParamValueByName("TempleMirageGoodsAward");
					if (!ConfigParser.ParseStrInt2(GameManager.systemParamsList.GetParamValueByName("TempleMirageWinNum"), ref this.RuntimeData.TempleMirageWinExtraNum, ref this.RuntimeData.TempleMirageWinExtraRate, ','))
					{
						success = false;
						LogManager.WriteLog(LogTypes.Fatal, "读取幻影寺院多倍奖励配置(TempleMirageWin)出错", null, true);
					}
					if (!ConfigParser.ParseStrInt2(GameManager.systemParamsList.GetParamValueByName("TempleMirageAward"), ref this.RuntimeData.TempleMirageAwardChengJiu, ref this.RuntimeData.TempleMirageAwardShengWang, ','))
					{
						success = false;
						LogManager.WriteLog(LogTypes.Fatal, "读取幻影寺院多倍奖励配置(TempleMirageWin)出错", null, true);
					}
					List<List<int>> levelRanges = ConfigParser.ParserIntArrayList(GameManager.systemParamsList.GetParamValueByName("TempleMirageLevel"), true, '|', ',');
					if (levelRanges.Count == 0)
					{
						success = false;
						LogManager.WriteLog(LogTypes.Fatal, "读取幻影寺院等级分组配置(TempleMirageLevel)出错", null, true);
					}
					else
					{
						for (int i = 0; i < levelRanges.Count; i++)
						{
							List<int> range = levelRanges[i];
							this.RuntimeData.Range2GroupIndexDict.Add(new RangeKey(Global.GetUnionLevel(range[0], range[1], false), Global.GetUnionLevel(range[2], range[3], false), null), i + 1);
						}
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

		// Token: 0x060017CC RID: 6092 RVA: 0x001744B8 File Offset: 0x001726B8
		public void GMStartHuoDongNow()
		{
			try
			{
				lock (this.RuntimeData.Mutex)
				{
					ConfigParser.ParserTimeRangeList(this.RuntimeData.TimePoints, "00:00-23:59:59", true, '|', '-');
				}
			}
			catch (Exception ex)
			{
			}
		}

		// Token: 0x060017CD RID: 6093 RVA: 0x00174534 File Offset: 0x00172734
		public bool ProcessHuanYingSiYuanEnqueueCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
				if (sceneType != SceneUIClasses.Normal)
				{
					client.sendCmd<int>(nID, -21, false);
					return true;
				}
				if (!this.IsGongNengOpened(client, true))
				{
					client.sendCmd<int>(nID, -2001, false);
					return true;
				}
				int result = -2001;
				int gropuIndex = 1;
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
					if (result >= 1)
					{
						if (!this.RuntimeData.Range2GroupIndexDict.TryGetValue(new RangeKey(Global.GetUnionLevel(client, false)), out gropuIndex))
						{
							result = -12;
						}
					}
				}
				if (result >= 0)
				{
					result = HuanYingSiYuanClient.getInstance().HuanYingSiYuanSignUp(client.strUserID, client.ClientData.RoleID, client.ClientData.ZoneID, 1, gropuIndex, client.ClientData.CombatForce);
					if (result == 1)
					{
						client.ClientData.SignUpGameType = 1;
						GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 1, 0, 0, 0, 1);
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

		// Token: 0x060017CE RID: 6094 RVA: 0x00174774 File Offset: 0x00172974
		public bool ProcessHuanYingSiYuanQueueRoleCountCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpened(client, false))
				{
					client.sendCmd<int>(nID, 0, false);
					return true;
				}
				int result = -2001;
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
				if (result >= 0)
				{
					result = HuanYingSiYuanClient.getInstance().GetRoleKuaFuFuBenRoleCount(client.ClientData.RoleID);
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

		// Token: 0x060017CF RID: 6095 RVA: 0x001748DC File Offset: 0x00172ADC
		public bool ProcessHuanYingSiYuanSuccessCountCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int count = 0;
				int nowDayId = Global.GetOffsetDayNow();
				int dayid = Global.GetRoleParamsInt32FromDB(client, "HysySuccessDayId");
				if (dayid == nowDayId)
				{
					count = Global.GetRoleParamsInt32FromDB(client, "HysySuccessCount");
				}
				client.sendCmd<int>(nID, count, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x060017D0 RID: 6096 RVA: 0x0017495C File Offset: 0x00172B5C
		public bool ProcessHuanYingSiYuanScoreInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (client.ClientSocket.IsKuaFuLogin)
				{
					this.NotifyTimeStateInfoAndScoreInfo(client, true, true);
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		// Token: 0x060017D1 RID: 6097 RVA: 0x001749BC File Offset: 0x00172BBC
		public bool ProcessHuanYingSiYuanEnterRespondCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpened(client, false))
				{
					client.sendCmd<int>(nID, 0, false);
					return true;
				}
				int result = 1;
				int flag = Global.SafeConvertToInt32(cmdParams[1]);
				lock (this.RuntimeData.Mutex)
				{
					int gropuIndex;
					if (!this.RuntimeData.Range2GroupIndexDict.TryGetValue(new RangeKey(Global.GetUnionLevel(client, false)), out gropuIndex))
					{
						result = -12;
					}
				}
				client.ClientData.SignUpGameType = 0;
				if (result >= 0)
				{
					if (flag > 0)
					{
						result = HuanYingSiYuanClient.getInstance().ChangeRoleState(client.ClientData.RoleID, KuaFuRoleStates.EnterGame, false);
						if (result >= 0)
						{
							GlobalNew.RecordSwitchKuaFuServerLog(client);
							client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
						}
						else
						{
							Global.GetClientKuaFuServerLoginData(client).RoleId = 0;
							client.sendCmd<int>(nID, result, false);
							client.sendCmd<int>(821, 0, false);
						}
					}
					else
					{
						HuanYingSiYuanClient.getInstance().ChangeRoleState(client.ClientData.RoleID, KuaFuRoleStates.None, false);
						Global.GetClientKuaFuServerLoginData(client).RoleId = 0;
						client.sendCmd<int>(nID, 0, false);
						client.sendCmd<int>(821, 0, false);
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x060017D2 RID: 6098 RVA: 0x00174B80 File Offset: 0x00172D80
		public bool ProcessHuanYingSiYuanDequeueCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpened(client, false))
				{
					client.sendCmd<int>(nID, 0, false);
					return true;
				}
				int result = 1;
				lock (this.RuntimeData.Mutex)
				{
					int gropuIndex;
					if (!this.RuntimeData.Range2GroupIndexDict.TryGetValue(new RangeKey(Global.GetUnionLevel(client, false)), out gropuIndex))
					{
						result = -12;
					}
				}
				client.ClientData.SignUpGameType = 0;
				if (result >= 0)
				{
					result = HuanYingSiYuanClient.getInstance().ChangeRoleState(client.ClientData.RoleID, KuaFuRoleStates.None, false);
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

		// Token: 0x060017D3 RID: 6099 RVA: 0x00174C88 File Offset: 0x00172E88
		public int GetBirthPoint(GameClient client, out int posX, out int posY)
		{
			int side = client.ClientData.BattleWhichSide;
			if (side <= 0)
			{
				KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
				side = HuanYingSiYuanClient.getInstance().GetRoleBattleWhichSide((int)clientKuaFuServerLoginData.GameId, clientKuaFuServerLoginData.RoleId);
				if (side > 0)
				{
					client.ClientData.BattleWhichSide = side;
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				HuanYingSiYuanBirthPoint huanYingSiYuanBirthPoint = null;
				if (this.RuntimeData.MapBirthPointDict.TryGetValue(side, out huanYingSiYuanBirthPoint))
				{
					posX = huanYingSiYuanBirthPoint.PosX;
					posY = huanYingSiYuanBirthPoint.PosY;
					return side;
				}
			}
			posX = 0;
			posY = 0;
			return -1;
		}

		// Token: 0x060017D4 RID: 6100 RVA: 0x00174D6C File Offset: 0x00172F6C
		public void InitRoleDailyHYSYData(GameClient client)
		{
			if (this.IsGongNengOpened(client, false))
			{
				int dayid = Global.GetRoleParamsInt32FromDB(client, "HysySuccessDayId");
				int ytdid = Global.GetRoleParamsInt32FromDB(client, "HysyYTDSuccessDayId");
				int nowcount = Global.GetRoleParamsInt32FromDB(client, "HysySuccessCount");
				int currdayid = Global.GetOffsetDayNow();
				if (ytdid + 1 != currdayid)
				{
					if (dayid + 1 == currdayid)
					{
						ytdid = dayid;
						int ytdcount = nowcount;
						Global.SaveRoleParamsInt32ValueToDB(client, "HysyYTDSuccessDayId", ytdid, true);
						Global.SaveRoleParamsInt32ValueToDB(client, "HysyYTDSuccessCount", ytdcount, true);
					}
					else
					{
						Global.SaveRoleParamsInt32ValueToDB(client, "HysyYTDSuccessDayId", currdayid - 1, true);
						Global.SaveRoleParamsInt32ValueToDB(client, "HysyYTDSuccessCount", 0, true);
					}
				}
			}
		}

		// Token: 0x060017D5 RID: 6101 RVA: 0x00174E1C File Offset: 0x0017301C
		public int GetLeftCount(GameClient client)
		{
			int dayid = Global.GetRoleParamsInt32FromDB(client, "HysySuccessDayId");
			int nowcount = Global.GetRoleParamsInt32FromDB(client, "HysySuccessCount");
			int currdayid = Global.GetOffsetDayNow();
			int leftnum = 3;
			int[] nParams = GameManager.systemParamsList.GetParamValueIntArrayByName("TempleMirageWinNum", ',');
			if (nParams != null && nParams.Length == 2)
			{
				leftnum = nParams[0];
			}
			int result;
			if (dayid == currdayid)
			{
				result = Global.GMax(0, leftnum - nowcount);
			}
			else
			{
				result = leftnum;
			}
			return result;
		}

		// Token: 0x060017D6 RID: 6102 RVA: 0x00174EA0 File Offset: 0x001730A0
		public bool OnInitGame(GameClient client)
		{
			int posX;
			int posY;
			int side = this.GetBirthPoint(client, out posX, out posY);
			bool result;
			if (side <= 0)
			{
				result = false;
			}
			else
			{
				client.ClientData.MapCode = this.RuntimeData.MapCode;
				client.ClientData.PosX = posX;
				client.ClientData.PosY = posY;
				client.ClientData.BattleWhichSide = side;
				int fubenSeq = 0;
				lock (HuanYingSiYuanManager.Mutex)
				{
					if (!this.GameId2FuBenSeq.TryGetValue((int)Global.GetClientKuaFuServerLoginData(client).GameId, out fubenSeq))
					{
						fubenSeq = GameCoreInterface.getinstance().GetNewFuBenSeqId();
						this.GameId2FuBenSeq[(int)Global.GetClientKuaFuServerLoginData(client).GameId] = fubenSeq;
					}
				}
				Global.GetClientKuaFuServerLoginData(client).FuBenSeqId = fubenSeq;
				client.ClientData.FuBenSeqID = Global.GetClientKuaFuServerLoginData(client).FuBenSeqId;
				result = true;
			}
			return result;
		}

		// Token: 0x060017D7 RID: 6103 RVA: 0x00174FB8 File Offset: 0x001731B8
		public bool ClientRelive(GameClient client)
		{
			bool result;
			if (client.ClientData.MapCode == this.RuntimeData.MapCode)
			{
				int toPosX;
				int toPosY;
				int side = this.GetBirthPoint(client, out toPosX, out toPosY);
				if (side <= 0)
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

		// Token: 0x060017D8 RID: 6104 RVA: 0x00175084 File Offset: 0x00173284
		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("HuanYingSiYuan") && !GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot4Dot1) && GlobalNew.IsGongNengOpened(client, GongNengIDs.HuanYingSiYuan, hint);
		}

		// Token: 0x060017D9 RID: 6105 RVA: 0x001750CC File Offset: 0x001732CC
		public bool AddHuanYingSiYuanCopyScenes(GameClient client, CopyMap copyMap)
		{
			bool result;
			if (copyMap.MapCode == this.RuntimeData.MapCode)
			{
				int fuBenSeqId = copyMap.FuBenSeqID;
				int mapCode = copyMap.MapCode;
				lock (HuanYingSiYuanManager.Mutex)
				{
					HuanYingSiYuanScene huanYingSiYuanScene = null;
					if (!this.HuanYingSiYuanSceneDict.TryGetValue(fuBenSeqId, out huanYingSiYuanScene))
					{
						huanYingSiYuanScene = new HuanYingSiYuanScene();
						huanYingSiYuanScene.CopyMap = copyMap;
						huanYingSiYuanScene.CleanAllInfo();
						huanYingSiYuanScene.GameId = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
						huanYingSiYuanScene.m_nMapCode = mapCode;
						huanYingSiYuanScene.CopyMapId = copyMap.CopyMapID;
						huanYingSiYuanScene.FuBenSeqId = fuBenSeqId;
						huanYingSiYuanScene.m_nPlarerCount = 1;
						this.HuanYingSiYuanSceneDict[fuBenSeqId] = huanYingSiYuanScene;
					}
					else
					{
						huanYingSiYuanScene.m_nPlarerCount++;
					}
					if (client.ClientData.BattleWhichSide == 1)
					{
						huanYingSiYuanScene.ScoreInfoData.Count1 += 1L;
					}
					else
					{
						huanYingSiYuanScene.ScoreInfoData.Count2++;
					}
					copyMap.IsKuaFuCopy = true;
					copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(this.RuntimeData.TotalSecs * 1000));
					GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanScoreInfoData>(826, huanYingSiYuanScene.ScoreInfoData, huanYingSiYuanScene.CopyMap);
				}
				client.SceneContextData2 = new HuanYingSiYuanLianShaContextData();
				HuanYingSiYuanClient.getInstance().GameFuBenRoleChangeState(client.ClientData.RoleID, 5, 0, 0);
				GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 0, 1, 0, 0, 1);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060017DA RID: 6106 RVA: 0x001752B0 File Offset: 0x001734B0
		public bool RemoveHuanYingSiYuanListCopyScenes(CopyMap copyMap)
		{
			bool result;
			if (copyMap.MapCode == this.RuntimeData.MapCode)
			{
				lock (HuanYingSiYuanManager.Mutex)
				{
					HuanYingSiYuanScene huanYingSiYuanScene;
					if (this.HuanYingSiYuanSceneDict.TryRemove(copyMap.FuBenSeqID, out huanYingSiYuanScene))
					{
						this.GameId2FuBenSeq.Remove(huanYingSiYuanScene.GameId);
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

		// Token: 0x060017DB RID: 6107 RVA: 0x00175348 File Offset: 0x00173548
		public void TimerProc()
		{
			long nowTicks = TimeUtil.NOW();
			if (nowTicks >= HuanYingSiYuanManager.NextHeartBeatTicks)
			{
				HuanYingSiYuanManager.NextHeartBeatTicks = nowTicks + 1020L;
				foreach (HuanYingSiYuanScene huanYingSiYuanScene in this.HuanYingSiYuanSceneDict.Values)
				{
					lock (HuanYingSiYuanManager.Mutex)
					{
						int nID = huanYingSiYuanScene.FuBenSeqId;
						int nCopyID = huanYingSiYuanScene.CopyMapId;
						int nMapCodeID = huanYingSiYuanScene.m_nMapCode;
						if (nID >= 0 && nCopyID >= 0 && nMapCodeID >= 0)
						{
							CopyMap copyMap = huanYingSiYuanScene.CopyMap;
							DateTime now = TimeUtil.NowDateTime();
							long ticks = TimeUtil.NOW();
							if (huanYingSiYuanScene.m_eStatus == GameSceneStatuses.STATUS_NULL)
							{
								huanYingSiYuanScene.m_lPrepareTime = ticks;
								huanYingSiYuanScene.m_lBeginTime = ticks + (long)(this.RuntimeData.PrepareSecs * 1000);
								huanYingSiYuanScene.m_eStatus = GameSceneStatuses.STATUS_PREPARE;
								huanYingSiYuanScene.StateTimeData.GameType = 1;
								huanYingSiYuanScene.StateTimeData.State = (int)huanYingSiYuanScene.m_eStatus;
								huanYingSiYuanScene.StateTimeData.EndTicks = huanYingSiYuanScene.m_lBeginTime;
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, huanYingSiYuanScene.StateTimeData, huanYingSiYuanScene.CopyMap);
							}
							else if (huanYingSiYuanScene.m_eStatus == GameSceneStatuses.STATUS_PREPARE)
							{
								if (ticks >= huanYingSiYuanScene.m_lBeginTime)
								{
									huanYingSiYuanScene.m_eStatus = GameSceneStatuses.STATUS_BEGIN;
									huanYingSiYuanScene.m_lEndTime = ticks + (long)(this.RuntimeData.FightingSecs * 1000);
									huanYingSiYuanScene.StateTimeData.GameType = 1;
									huanYingSiYuanScene.StateTimeData.State = (int)huanYingSiYuanScene.m_eStatus;
									huanYingSiYuanScene.StateTimeData.EndTicks = huanYingSiYuanScene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, huanYingSiYuanScene.StateTimeData, huanYingSiYuanScene.CopyMap);
									foreach (ShengBeiData shengBeiData in this.RuntimeData.ShengBeiDataDict.Values)
									{
										HuanYingSiYuanShengBeiContextData contextData = new HuanYingSiYuanShengBeiContextData
										{
											UniqueId = this.GetInternalId(),
											FuBenSeqId = huanYingSiYuanScene.FuBenSeqId,
											ShengBeiId = shengBeiData.ID,
											BufferGoodsId = shengBeiData.GoodsID,
											MonsterId = shengBeiData.MonsterID,
											PosX = shengBeiData.PosX,
											PosY = shengBeiData.PosY,
											CopyMapID = huanYingSiYuanScene.CopyMapId,
											Score = shengBeiData.Score,
											Time = shengBeiData.Time,
											BufferProps = shengBeiData.BufferProps
										};
										this.CreateMonster(huanYingSiYuanScene, contextData);
									}
									copyMap.AddGuangMuEvent(1, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(copyMap.MapCode, copyMap.CopyMapID, 1, 0);
								}
							}
							else if (huanYingSiYuanScene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
							{
								if (ticks >= huanYingSiYuanScene.m_lEndTime)
								{
									int successSide = 0;
									if (huanYingSiYuanScene.ScoreInfoData.Score1 > huanYingSiYuanScene.ScoreInfoData.Score2)
									{
										successSide = 1;
									}
									else if (huanYingSiYuanScene.ScoreInfoData.Score2 > huanYingSiYuanScene.ScoreInfoData.Score1)
									{
										successSide = 2;
									}
									this.CompleteHuanYingSiYuanScene(huanYingSiYuanScene, successSide);
								}
								else
								{
									this.CheckShengBeiBufferTime(huanYingSiYuanScene, nowTicks);
								}
							}
							else if (huanYingSiYuanScene.m_eStatus == GameSceneStatuses.STATUS_END)
							{
								huanYingSiYuanScene.m_eStatus = GameSceneStatuses.STATUS_AWARD;
								huanYingSiYuanScene.m_lEndTime = nowTicks;
								huanYingSiYuanScene.m_lLeaveTime = huanYingSiYuanScene.m_lEndTime + (long)(this.RuntimeData.ClearRolesSecs * 1000);
								HuanYingSiYuanClient.getInstance().GameFuBenChangeState(huanYingSiYuanScene.GameId, GameFuBenState.End, now);
								huanYingSiYuanScene.StateTimeData.GameType = 1;
								huanYingSiYuanScene.StateTimeData.State = 3;
								huanYingSiYuanScene.StateTimeData.EndTicks = huanYingSiYuanScene.m_lLeaveTime;
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, huanYingSiYuanScene.StateTimeData, huanYingSiYuanScene.CopyMap);
								this.GiveAwards(huanYingSiYuanScene);
							}
							else if (huanYingSiYuanScene.m_eStatus == GameSceneStatuses.STATUS_AWARD)
							{
								if (ticks >= huanYingSiYuanScene.m_lLeaveTime)
								{
									copyMap.SetRemoveTicks(huanYingSiYuanScene.m_lLeaveTime);
									huanYingSiYuanScene.m_eStatus = GameSceneStatuses.STATUS_CLEAR;
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
										DataHelper.WriteExceptionLogEx(ex, "幻影寺院清场调度异常");
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060017DC RID: 6108 RVA: 0x001758F8 File Offset: 0x00173AF8
		public int GetInternalId()
		{
			int id = Interlocked.Increment(ref this.InternalShengBeiId);
			if (id < 0)
			{
				id = (this.InternalShengBeiId = 1);
			}
			return id;
		}

		// Token: 0x060017DD RID: 6109 RVA: 0x00175930 File Offset: 0x00173B30
		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool scoreInfo = true)
		{
			lock (HuanYingSiYuanManager.Mutex)
			{
				HuanYingSiYuanScene huanYingSiYuanScene;
				if (this.HuanYingSiYuanSceneDict.TryGetValue(client.ClientData.FuBenSeqID, out huanYingSiYuanScene))
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, huanYingSiYuanScene.StateTimeData, false);
					}
					if (scoreInfo)
					{
						client.sendCmd<HuanYingSiYuanScoreInfoData>(826, huanYingSiYuanScene.ScoreInfoData, false);
					}
				}
			}
		}

		// Token: 0x060017DE RID: 6110 RVA: 0x001759D0 File Offset: 0x00173BD0
		public void CreateMonster(HuanYingSiYuanScene scene, HuanYingSiYuanShengBeiContextData contextData = null)
		{
			int gridX = contextData.PosX / this.RuntimeData.MapGridWidth;
			int gridY = contextData.PosY / this.RuntimeData.MapGridHeight;
			GameManager.MonsterZoneMgr.AddDynamicMonsters(this.RuntimeData.MapCode, contextData.MonsterId, contextData.CopyMapID, 1, gridX, gridY, 0, 0, SceneUIClasses.HuanYingSiYuan, contextData, null);
		}

		// Token: 0x060017DF RID: 6111 RVA: 0x00175A30 File Offset: 0x00173C30
		public int GetCaiJiMonsterTime(GameClient client, Monster monster)
		{
			HuanYingSiYuanShengBeiContextData contextData = client.SceneContextData as HuanYingSiYuanShengBeiContextData;
			if (null != contextData)
			{
				lock (HuanYingSiYuanManager.Mutex)
				{
					HuanYingSiYuanScene huanYingSiYuanScene;
					if (this.HuanYingSiYuanSceneDict.TryGetValue(contextData.FuBenSeqId, out huanYingSiYuanScene))
					{
						if (huanYingSiYuanScene.ShengBeiContextDict.ContainsKey(contextData.UniqueId))
						{
							return -300;
						}
					}
				}
			}
			contextData = (monster.Tag as HuanYingSiYuanShengBeiContextData);
			int result;
			if (contextData != null)
			{
				result = contextData.Time;
			}
			else
			{
				result = -302;
			}
			return result;
		}

		// Token: 0x060017E0 RID: 6112 RVA: 0x00175AFC File Offset: 0x00173CFC
		public void OnCaiJiFinish(GameClient client, Monster monster)
		{
			HuanYingSiYuanShengBeiContextData contextData = monster.Tag as HuanYingSiYuanShengBeiContextData;
			HuanYingSiYuanScene huanYingSiYuanScene = null;
			if (null != contextData)
			{
				long endTicks = TimeUtil.NOW() + (long)(this.RuntimeData.HoldShengBeiSecs * 1000);
				lock (HuanYingSiYuanManager.Mutex)
				{
					contextData.OwnerRoleId = client.ClientData.RoleID;
					contextData.EndTicks = endTicks;
					if (this.HuanYingSiYuanSceneDict.TryGetValue(contextData.FuBenSeqId, out huanYingSiYuanScene))
					{
						if (huanYingSiYuanScene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
						{
							this.GetShengBei(huanYingSiYuanScene, client, contextData);
						}
					}
				}
			}
		}

		// Token: 0x060017E1 RID: 6113 RVA: 0x00175BD4 File Offset: 0x00173DD4
		public void CompleteHuanYingSiYuanScene(HuanYingSiYuanScene huanYingSiYuanScene, int successSide)
		{
			huanYingSiYuanScene.m_eStatus = GameSceneStatuses.STATUS_END;
			huanYingSiYuanScene.SuccessSide = successSide;
		}

		// Token: 0x060017E2 RID: 6114 RVA: 0x00175BE8 File Offset: 0x00173DE8
		public void OnKillRole(GameClient client, GameClient other)
		{
			this.TryLostShengBei(other);
			lock (HuanYingSiYuanManager.Mutex)
			{
				HuanYingSiYuanScene huanYingSiYuanScene;
				if (this.HuanYingSiYuanSceneDict.TryGetValue(client.ClientData.FuBenSeqID, out huanYingSiYuanScene))
				{
					if (huanYingSiYuanScene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
					{
						int addScore = 0;
						HuanYingSiYuanLianShaContextData clientLianShaContextData = client.SceneContextData2 as HuanYingSiYuanLianShaContextData;
						HuanYingSiYuanLianShaContextData otherLianShaContextData = other.SceneContextData2 as HuanYingSiYuanLianShaContextData;
						HuanYingSiYuanLianSha huanYingSiYuanLianSha = null;
						HuanYingSiYuanLianshaOver huanYingSiYuanLianshaOver = null;
						HuanYingSiYuanAddScore huanYingSiYuanAddScore = new HuanYingSiYuanAddScore();
						huanYingSiYuanAddScore.Name = Global.FormatRoleName4(client);
						huanYingSiYuanAddScore.ZoneID = client.ClientData.ZoneID;
						huanYingSiYuanAddScore.Side = client.ClientData.BattleWhichSide;
						huanYingSiYuanAddScore.ByLianShaNum = 1;
						huanYingSiYuanAddScore.RoleId = client.ClientData.RoleID;
						huanYingSiYuanAddScore.Occupation = client.ClientData.Occupation;
						if (null != clientLianShaContextData)
						{
							clientLianShaContextData.KillNum++;
							ContinuityKillAward continuityKillAward;
							if (this.RuntimeData.ContinuityKillAwardDict.TryGetValue(clientLianShaContextData.KillNum, out continuityKillAward))
							{
								huanYingSiYuanAddScore.ByLianShaNum = 1;
								huanYingSiYuanLianSha = new HuanYingSiYuanLianSha();
								huanYingSiYuanLianSha.Name = huanYingSiYuanAddScore.Name;
								huanYingSiYuanLianSha.ZoneID = huanYingSiYuanAddScore.ZoneID;
								huanYingSiYuanLianSha.Occupation = huanYingSiYuanAddScore.Occupation;
								huanYingSiYuanLianSha.LianShaType = continuityKillAward.ID;
								huanYingSiYuanLianSha.ExtScore = continuityKillAward.Score;
								huanYingSiYuanLianSha.Side = huanYingSiYuanAddScore.Side;
								addScore += huanYingSiYuanLianSha.ExtScore;
							}
						}
						if (null != otherLianShaContextData)
						{
							if (otherLianShaContextData.KillNum >= 2)
							{
								huanYingSiYuanLianshaOver = new HuanYingSiYuanLianshaOver();
								huanYingSiYuanLianshaOver.KillerName = huanYingSiYuanAddScore.Name;
								huanYingSiYuanLianshaOver.KillerZoneID = huanYingSiYuanAddScore.ZoneID;
								huanYingSiYuanLianshaOver.KillerOccupation = client.ClientData.Occupation;
								huanYingSiYuanLianshaOver.KillerSide = huanYingSiYuanAddScore.Side;
								huanYingSiYuanLianshaOver.KilledName = Global.FormatRoleName4(other);
								huanYingSiYuanLianshaOver.KilledZoneID = other.ClientData.ZoneID;
								huanYingSiYuanLianshaOver.KilledOccupation = other.ClientData.Occupation;
								huanYingSiYuanLianshaOver.KilledSide = other.ClientData.BattleWhichSide;
								huanYingSiYuanLianshaOver.ExtScore = otherLianShaContextData.KillNum * 5;
								addScore += huanYingSiYuanLianshaOver.ExtScore;
							}
							otherLianShaContextData.KillNum = 0;
						}
						addScore += this.RuntimeData.TempleMiragePK;
						huanYingSiYuanAddScore.Score = addScore;
						if (client.ClientData.BattleWhichSide == 1)
						{
							huanYingSiYuanScene.ScoreInfoData.Score1 += addScore;
							if (huanYingSiYuanScene.ScoreInfoData.Score1 >= this.RuntimeData.TempleMirageWin)
							{
								this.CompleteHuanYingSiYuanScene(huanYingSiYuanScene, 1);
							}
						}
						else
						{
							huanYingSiYuanScene.ScoreInfoData.Score2 += addScore;
							if (huanYingSiYuanScene.ScoreInfoData.Score2 >= this.RuntimeData.TempleMirageWin)
							{
								this.CompleteHuanYingSiYuanScene(huanYingSiYuanScene, 2);
							}
						}
						if (null != clientLianShaContextData)
						{
							clientLianShaContextData.TotalScore += addScore;
						}
						GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanScoreInfoData>(826, huanYingSiYuanScene.ScoreInfoData, huanYingSiYuanScene.CopyMap);
						if (null != huanYingSiYuanLianSha)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanLianSha>(818, huanYingSiYuanLianSha, huanYingSiYuanScene.CopyMap);
						}
						if (null != huanYingSiYuanLianshaOver)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanLianshaOver>(819, huanYingSiYuanLianshaOver, huanYingSiYuanScene.CopyMap);
						}
					}
				}
			}
		}

		// Token: 0x060017E3 RID: 6115 RVA: 0x00175FA8 File Offset: 0x001741A8
		private void GetShengBei(HuanYingSiYuanScene huanYingSiYuanScene, GameClient client, HuanYingSiYuanShengBeiContextData contextData)
		{
			if (null != contextData)
			{
				lock (HuanYingSiYuanManager.Mutex)
				{
					client.SceneContextData = contextData;
					huanYingSiYuanScene.ShengBeiContextDict[contextData.UniqueId] = contextData;
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.HysyShengBei,
						contextData.BufferProps
					});
					double[] actionParams = new double[]
					{
						(double)contextData.BufferGoodsId,
						(double)this.RuntimeData.HoldShengBeiSecs
					};
					Global.UpdateBufferData(client, BufferItemTypes.HysyShengBei, actionParams, 0, true);
				}
			}
		}

		// Token: 0x060017E4 RID: 6116 RVA: 0x00176074 File Offset: 0x00174274
		private HuanYingSiYuanShengBeiContextData LostShengBei(GameClient client)
		{
			HuanYingSiYuanShengBeiContextData contextData = null;
			if (null != client.SceneContextData)
			{
				contextData = (client.SceneContextData as HuanYingSiYuanShengBeiContextData);
				if (null != contextData)
				{
					lock (HuanYingSiYuanManager.Mutex)
					{
						PropsCacheManager propsCacheManager = client.ClientData.PropsCacheManager;
						object[] array = new object[2];
						array[0] = PropsSystemTypes.HysyShengBei;
						propsCacheManager.SetExtProps(array);
						double[] array2 = new double[2];
						double[] actionParams = array2;
						Global.UpdateBufferData(client, BufferItemTypes.HysyShengBei, actionParams, 0, true);
						client.SceneContextData = null;
					}
				}
			}
			return contextData;
		}

		// Token: 0x060017E5 RID: 6117 RVA: 0x00176138 File Offset: 0x00174338
		private void SubmitShengBei(GameClient client)
		{
			if (null != client.SceneContextData)
			{
				HuanYingSiYuanShengBeiContextData contextData = client.SceneContextData as HuanYingSiYuanShengBeiContextData;
				if (null != contextData)
				{
					long nowTicks = TimeUtil.NOW();
					if (contextData.EndTicks - nowTicks <= (long)((this.RuntimeData.HoldShengBeiSecs - this.RuntimeData.MinSubmitShengBeiSecs) * 1000))
					{
						Point clientPoint = new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY);
						lock (this.RuntimeData.Mutex)
						{
							HuanYingSiYuanBirthPoint huanYingSiYuanBirthPoint;
							if (!this.RuntimeData.MapBirthPointDict.TryGetValue(client.ClientData.BattleWhichSide, out huanYingSiYuanBirthPoint))
							{
								return;
							}
							Point targetPoint = new Point((double)huanYingSiYuanBirthPoint.PosX, (double)huanYingSiYuanBirthPoint.PosY);
							if (Global.GetTwoPointDistance(clientPoint, targetPoint) > 1000.0)
							{
								return;
							}
						}
						lock (HuanYingSiYuanManager.Mutex)
						{
							HuanYingSiYuanScene huanYingSiYuanScene;
							if (this.HuanYingSiYuanSceneDict.TryGetValue(client.ClientData.FuBenSeqID, out huanYingSiYuanScene) && huanYingSiYuanScene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
							{
								if (huanYingSiYuanScene.ShengBeiContextDict.Remove(contextData.UniqueId))
								{
									this.LostShengBei(client);
									this.CreateMonster(huanYingSiYuanScene, contextData);
									contextData.OwnerRoleId = 0;
									if (client.ClientData.BattleWhichSide == 1)
									{
										huanYingSiYuanScene.ScoreInfoData.Score1 += contextData.Score;
										if (huanYingSiYuanScene.ScoreInfoData.Score1 >= this.RuntimeData.TempleMirageWin)
										{
											this.CompleteHuanYingSiYuanScene(huanYingSiYuanScene, 1);
										}
									}
									else
									{
										huanYingSiYuanScene.ScoreInfoData.Score2 += contextData.Score;
										if (huanYingSiYuanScene.ScoreInfoData.Score2 >= this.RuntimeData.TempleMirageWin)
										{
											this.CompleteHuanYingSiYuanScene(huanYingSiYuanScene, 2);
										}
									}
									HuanYingSiYuanLianShaContextData clientLianShaContextData = client.SceneContextData2 as HuanYingSiYuanLianShaContextData;
									if (null != clientLianShaContextData)
									{
										clientLianShaContextData.TotalScore += contextData.Score;
									}
									HuanYingSiYuanAddScore huanYingSiYuanAddScore = new HuanYingSiYuanAddScore();
									huanYingSiYuanAddScore.Name = Global.FormatRoleName4(client);
									huanYingSiYuanAddScore.ZoneID = client.ClientData.ZoneID;
									huanYingSiYuanAddScore.Side = client.ClientData.BattleWhichSide;
									huanYingSiYuanAddScore.Score = contextData.Score;
									huanYingSiYuanAddScore.RoleId = client.ClientData.RoleID;
									huanYingSiYuanAddScore.Occupation = client.ClientData.Occupation;
									CopyMap copyMap = huanYingSiYuanScene.CopyMap;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanAddScore>(829, huanYingSiYuanAddScore, copyMap);
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanScoreInfoData>(826, huanYingSiYuanScene.ScoreInfoData, copyMap);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060017E6 RID: 6118 RVA: 0x001764B4 File Offset: 0x001746B4
		private void CheckShengBeiBufferTime(HuanYingSiYuanScene huanYingSiYuanScene, long nowTicks)
		{
			List<HuanYingSiYuanShengBeiContextData> shengBeiList = new List<HuanYingSiYuanShengBeiContextData>();
			lock (HuanYingSiYuanManager.Mutex)
			{
				if (huanYingSiYuanScene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
				{
					if (huanYingSiYuanScene.ShengBeiContextDict.Count > 0)
					{
						foreach (HuanYingSiYuanShengBeiContextData contextData in huanYingSiYuanScene.ShengBeiContextDict.Values)
						{
							if (contextData.EndTicks < nowTicks)
							{
								shengBeiList.Add(contextData);
								if (contextData.OwnerRoleId != 0)
								{
									GameClient client = GameManager.ClientMgr.FindClient(contextData.OwnerRoleId);
									if (null != client)
									{
										this.LostShengBei(client);
									}
								}
								contextData.OwnerRoleId = 0;
								this.CreateMonster(huanYingSiYuanScene, contextData);
							}
						}
					}
					if (shengBeiList.Count > 0)
					{
						foreach (HuanYingSiYuanShengBeiContextData contextData in shengBeiList)
						{
							huanYingSiYuanScene.ShengBeiContextDict.Remove(contextData.UniqueId);
						}
					}
				}
			}
		}

		// Token: 0x060017E7 RID: 6119 RVA: 0x0017666C File Offset: 0x0017486C
		private void TryLostShengBei(GameClient client)
		{
			lock (HuanYingSiYuanManager.Mutex)
			{
				HuanYingSiYuanScene huanYingSiYuanScene = null;
				if (this.HuanYingSiYuanSceneDict.TryGetValue(client.ClientData.FuBenSeqID, out huanYingSiYuanScene))
				{
					HuanYingSiYuanShengBeiContextData contextData = this.LostShengBei(client);
					if (null != contextData)
					{
						contextData.OwnerRoleId = 0;
						huanYingSiYuanScene.ShengBeiContextDict.Remove(contextData.UniqueId);
						this.CreateMonster(huanYingSiYuanScene, contextData);
					}
				}
			}
		}

		// Token: 0x060017E8 RID: 6120 RVA: 0x00176710 File Offset: 0x00174910
		public void GiveAwards(HuanYingSiYuanScene huanYingSiYuanScene)
		{
			try
			{
				List<GameClient> objsList = huanYingSiYuanScene.CopyMap.GetClientsList();
				if (objsList != null && objsList.Count > 0)
				{
					int nowDayId = Global.GetOffsetDayNow();
					for (int i = 0; i < objsList.Count; i++)
					{
						GameClient client = objsList[i];
						if (client != null && client == GameManager.ClientMgr.FindClient(client.ClientData.RoleID))
						{
							bool success = false;
							double nMultiple = 0.5;
							int awardsRate = 1;
							int count = 0;
							string awardGoods = null;
							HuanYingSiYuanLianShaContextData clientLianShaContextData = client.SceneContextData2 as HuanYingSiYuanLianShaContextData;
							if (clientLianShaContextData != null && clientLianShaContextData.TotalScore >= this.RuntimeData.TempleMirageMinJiFen)
							{
								if (client.ClientData.BattleWhichSide == huanYingSiYuanScene.SuccessSide)
								{
									success = true;
									nMultiple = 1.0;
									int dayid = Global.GetRoleParamsInt32FromDB(client, "HysySuccessDayId");
									if (dayid == nowDayId)
									{
										count = Global.GetRoleParamsInt32FromDB(client, "HysySuccessCount");
										if (count < this.RuntimeData.TempleMirageWinExtraNum)
										{
											awardsRate = this.RuntimeData.TempleMirageWinExtraRate;
											awardGoods = this.RuntimeData.AwardGoods;
										}
									}
									else
									{
										awardsRate = this.RuntimeData.TempleMirageWinExtraRate;
										awardGoods = this.RuntimeData.AwardGoods;
									}
								}
							}
							else
							{
								nMultiple = 0.0;
								awardsRate = 0;
							}
							long nExp = (long)((double)this.RuntimeData.TempleMirageEXPAward * nMultiple * (double)client.ClientData.ChangeLifeCount);
							int chengJiuaward = (int)((double)this.RuntimeData.TempleMirageAwardChengJiu * nMultiple);
							int shengWangaward = (int)((double)this.RuntimeData.TempleMirageAwardShengWang * nMultiple);
							if (nExp > 0L)
							{
								GameManager.ClientMgr.ProcessRoleExperience(client, nExp * (long)awardsRate, false, true, false, "none");
							}
							if (chengJiuaward > 0)
							{
								ChengJiuManager.AddChengJiuPoints(client, "幻影寺院获得成就", chengJiuaward * awardsRate, true, true);
							}
							if (shengWangaward > 0)
							{
								GameManager.ClientMgr.ModifyShengWangValue(client, shengWangaward * awardsRate, "幻影寺院奖励", false, true);
							}
							if (!string.IsNullOrEmpty(awardGoods))
							{
								AwardsItemList awardsItemList = new AwardsItemList();
								awardsItemList.Add(awardGoods);
								List<GoodsData> goodsDataList = Global.ConvertToGoodsDataList(awardsItemList.Items, -1);
								if (!Global.CanAddGoodsDataList(client, goodsDataList))
								{
									GameManager.ClientMgr.SendMailWhenPacketFull(client, goodsDataList, GLang.GetLang(385, new object[0]), GLang.GetLang(385, new object[0]));
								}
								else
								{
									for (int j = 0; j < goodsDataList.Count; j++)
									{
										GoodsData goodsData = goodsDataList[j];
										if (null != goodsData)
										{
											goodsData.Id = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, goodsData.GCount, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 0, goodsData.Jewellist, true, 1, "幻影寺院奖励", goodsData.Endtime, goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, 0, 0, 0, null, null, 0, true);
										}
									}
								}
							}
							HuanYingSiYuanAwardsData awardsData = new HuanYingSiYuanAwardsData
							{
								SuccessSide = huanYingSiYuanScene.SuccessSide,
								Exp = nExp,
								ShengWang = shengWangaward,
								ChengJiuAward = chengJiuaward,
								AwardsRate = awardsRate,
								AwardGoods = awardGoods
							};
							if (success)
							{
								if (nMultiple > 0.0)
								{
									Global.SaveRoleParamsInt32ValueToDB(client, "HysySuccessDayId", nowDayId, true);
									Global.SaveRoleParamsInt32ValueToDB(client, "HysySuccessCount", count + 1, true);
									GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 0, 0, 1, 0, 1);
									if (huanYingSiYuanScene.ScoreInfoData.Score1 >= 1000 || huanYingSiYuanScene.ScoreInfoData.Score2 >= 1000)
									{
										GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 0, 0, 0, 1, 1);
									}
									GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_HuanYingSiYuan, new int[0]));
								}
							}
							client.sendCmd<HuanYingSiYuanAwardsData>(825, awardsData, false);
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "幻影寺院清场调度异常");
			}
		}

		// Token: 0x060017E9 RID: 6121 RVA: 0x00176C0C File Offset: 0x00174E0C
		public void LeaveFuBen(GameClient client)
		{
			lock (HuanYingSiYuanManager.Mutex)
			{
				this.TryLostShengBei(client);
				HuanYingSiYuanScene huanYingSiYuanScene = null;
				if (this.HuanYingSiYuanSceneDict.TryGetValue(client.ClientData.FuBenSeqID, out huanYingSiYuanScene))
				{
					huanYingSiYuanScene.m_nPlarerCount--;
					if (client.ClientData.BattleWhichSide == 1)
					{
						huanYingSiYuanScene.ScoreInfoData.Count1 -= 1L;
					}
					else
					{
						huanYingSiYuanScene.ScoreInfoData.Count2--;
					}
					GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanScoreInfoData>(826, huanYingSiYuanScene.ScoreInfoData, huanYingSiYuanScene.CopyMap);
				}
			}
			HuanYingSiYuanClient.getInstance().GameFuBenRoleChangeState(client.ClientData.RoleID, 0, 0, 0);
		}

		// Token: 0x060017EA RID: 6122 RVA: 0x00176D00 File Offset: 0x00174F00
		public void OnLogout(GameClient client)
		{
			this.LeaveFuBen(client);
		}

		// Token: 0x04002232 RID: 8754
		public const SceneUIClasses ManagerType = SceneUIClasses.HuanYingSiYuan;

		// Token: 0x04002233 RID: 8755
		private static HuanYingSiYuanManager instance = new HuanYingSiYuanManager();

		// Token: 0x04002234 RID: 8756
		public HuanYingSiYuanData RuntimeData = new HuanYingSiYuanData();

		// Token: 0x04002235 RID: 8757
		public static object Mutex = new object();

		// Token: 0x04002236 RID: 8758
		private int InternalShengBeiId = 0;

		// Token: 0x04002237 RID: 8759
		public ConcurrentDictionary<int, HuanYingSiYuanScene> HuanYingSiYuanSceneDict = new ConcurrentDictionary<int, HuanYingSiYuanScene>();

		// Token: 0x04002238 RID: 8760
		public Dictionary<int, int> GameId2FuBenSeq = new Dictionary<int, int>();

		// Token: 0x04002239 RID: 8761
		private static long NextHeartBeatTicks = 0L;
	}
}
