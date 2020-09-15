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
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	// Token: 0x02000260 RID: 608
	public class CompBattleManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		// Token: 0x0600086D RID: 2157 RVA: 0x00081110 File Offset: 0x0007F310
		public static CompBattleManager getInstance()
		{
			return CompBattleManager.instance;
		}

		// Token: 0x0600086E RID: 2158 RVA: 0x00081128 File Offset: 0x0007F328
		public bool initialize()
		{
			return this.InitConfig();
		}

		// Token: 0x0600086F RID: 2159 RVA: 0x0008114C File Offset: 0x0007F34C
		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("CompBattleManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 2000);
			return true;
		}

		// Token: 0x06000870 RID: 2160 RVA: 0x0008118C File Offset: 0x0007F38C
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(2000, 1, 1, CompBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2001, 2, 2, CompBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2002, 2, 2, CompBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2003, 1, 1, CompBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2004, 1, 1, CompBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2006, 1, 1, CompBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2009, 1, 1, CompBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(33, 52, CompBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(27, 52, CompBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(30, 52, CompBattleManager.getInstance());
			GlobalEventSource.getInstance().registerListener(28, CompBattleManager.getInstance());
			GlobalEventSource.getInstance().registerListener(12, CompBattleManager.getInstance());
			GlobalEventSource.getInstance().registerListener(10, CompBattleManager.getInstance());
			GlobalEventSource.getInstance().registerListener(11, CompBattleManager.getInstance());
			return true;
		}

		// Token: 0x06000871 RID: 2161 RVA: 0x000812CC File Offset: 0x0007F4CC
		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(33, 52, CompBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(27, 52, CompBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(30, 52, CompBattleManager.getInstance());
			GlobalEventSource.getInstance().removeListener(28, CompBattleManager.getInstance());
			GlobalEventSource.getInstance().removeListener(12, CompBattleManager.getInstance());
			GlobalEventSource.getInstance().removeListener(10, CompBattleManager.getInstance());
			GlobalEventSource.getInstance().removeListener(11, CompBattleManager.getInstance());
			return true;
		}

		// Token: 0x06000872 RID: 2162 RVA: 0x00081364 File Offset: 0x0007F564
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06000873 RID: 2163 RVA: 0x00081378 File Offset: 0x0007F578
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x06000874 RID: 2164 RVA: 0x0008138C File Offset: 0x0007F58C
		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return GlobalNew.IsGongNengOpened(client, GongNengIDs.Comp, hint) && GameManager.VersionSystemOpenMgr.IsVersionSystemOpen(120402);
		}

		// Token: 0x06000875 RID: 2165 RVA: 0x000813C0 File Offset: 0x0007F5C0
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!this.IsGongNengOpened(client, false))
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 2000:
					return this.ProcessGetCompBattleBaseDataCmd(client, nID, bytes, cmdParams);
				case 2001:
					return this.ProcessGetCompBattleCityDataCmd(client, nID, bytes, cmdParams);
				case 2002:
					return this.ProcessCompBattleEnterCmd(client, nID, bytes, cmdParams);
				case 2003:
					return this.ProcessGetCompBattleAwardInfoCmd(client, nID, bytes, cmdParams);
				case 2004:
					return this.ProcessGetCompBattleStateCmd(client, nID, bytes, cmdParams);
				case 2006:
					return this.ProcessGetCompBattleSelfScoreCmd(client, nID, bytes, cmdParams);
				case 2009:
					return this.ProcessGetCompBattleAwardCmd(client, nID, bytes, cmdParams);
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06000876 RID: 2166 RVA: 0x000814BC File Offset: 0x0007F6BC
		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 28)
			{
				OnStartPlayGameEventObject e = eventObject as OnStartPlayGameEventObject;
				this.OnStartPlayGame(e.Client);
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
				}
			}
			else if (eventType == 11)
			{
				MonsterDeadEventObject e2 = eventObject as MonsterDeadEventObject;
				this.OnProcessMonsterDead(e2.getAttacker(), e2.getMonster());
			}
			else if (eventObject.getEventType() == 12)
			{
				PlayerLogoutEventObject eventObj = (PlayerLogoutEventObject)eventObject;
				this.OnLogout(eventObj.getPlayer());
			}
		}

		// Token: 0x06000877 RID: 2167 RVA: 0x000815A0 File Offset: 0x0007F7A0
		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			int num = eventType;
			if (num != 27)
			{
				if (num != 30)
				{
					if (num == 33)
					{
						PreMonsterInjureEventObject obj = eventObject as PreMonsterInjureEventObject;
						if (obj != null && obj.SceneType == 52)
						{
							Monster injureMonster = obj.Monster;
							if (injureMonster != null)
							{
								CompStrongholdConfig qiZhiConfig = obj.Monster.Tag as CompStrongholdConfig;
								if (null != qiZhiConfig)
								{
									bool setFlagDamage = false;
									CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(obj.Monster.CopyMapID);
									CompBattleScene scene = null;
									if (copyMap != null && this.SceneDict.TryGetValue(copyMap.FuBenSeqID, out scene) && scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
									{
										setFlagDamage = true;
									}
									if (setFlagDamage)
									{
										obj.Injure = this.RuntimeData.CompBattleFlagDamage;
									}
									else
									{
										obj.Injure = 0;
									}
									eventObject.Handled = true;
									eventObject.Result = true;
								}
							}
						}
					}
				}
				else
				{
					OnCreateMonsterEventObject e = eventObject as OnCreateMonsterEventObject;
					if (null != e)
					{
						CompStrongholdConfig qiZhiConfig = e.Monster.Tag as CompStrongholdConfig;
						if (null != qiZhiConfig)
						{
							e.Monster.Camp = qiZhiConfig.BattleWhichSide;
							e.Result = true;
							e.Handled = true;
						}
					}
				}
			}
			else
			{
				ProcessClickOnNpcEventObject e2 = eventObject as ProcessClickOnNpcEventObject;
				if (null != e2)
				{
					if (null != e2.Npc)
					{
						int npcId = e2.Npc.NpcID;
					}
					if (this.OnSpriteClickOnNpc(e2.Client, e2.NpcId, e2.ExtensionID))
					{
						e2.Result = false;
						e2.Handled = true;
					}
				}
			}
		}

		// Token: 0x06000878 RID: 2168 RVA: 0x0008178C File Offset: 0x0007F98C
		public bool InitConfig()
		{
			bool success = true;
			string fileName = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.CompBattleConfigDict.Clear();
					fileName = "Config/ForceCraft.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					XElement xml = XElement.Load(fullPathFileName);
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						CompBattleConfig item = new CompBattleConfig();
						item.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item.Name = Global.GetSafeAttributeStr(node, "Name");
						item.MapCode = (int)Global.GetSafeAttributeLong(node, "MapCode");
						item.ForceMax = (int)Global.GetSafeAttributeLong(node, "ForceMax");
						item.MaxEnterNum = (int)Global.GetSafeAttributeLong(node, "MaxEnterNum");
						item.EnterCD = (int)Global.GetSafeAttributeLong(node, "EnterCD");
						item.PrepareSecs = (int)Global.GetSafeAttributeLong(node, "PrepareSecs");
						item.FightingSecs = (int)Global.GetSafeAttributeLong(node, "FightingSecs");
						item.ClearRolesSecs = (int)Global.GetSafeAttributeLong(node, "ClearRolesSecs");
						item.DuiHuanType = (int)Global.GetSafeAttributeLong(node, "DuiHuanType");
						if (!ConfigParser.ParserTimeRangeListWithDay(item.TimePoints, Global.GetSafeAttributeStr(node, "TimePoints"), true, '|', '-', ','))
						{
							success = false;
							LogManager.WriteLog(LogTypes.Fatal, string.Format("读取{0}时间配置(TimePoints)出错", fileName), null, true);
						}
						for (int i = 0; i < item.TimePoints.Count; i++)
						{
							TimeSpan ts = new TimeSpan(item.TimePoints[i].Hours, item.TimePoints[i].Minutes, item.TimePoints[i].Seconds);
							item.SecondsOfDay.Add(ts.TotalSeconds);
						}
						this.RuntimeData.CompBattleConfigDict[item.ID] = item;
					}
					this.RuntimeData.CompStrongholdConfigDict.Clear();
					fileName = "Config/ForceStronghold.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						CompStrongholdConfig item2 = new CompStrongholdConfig();
						item2.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item2.MapCode = (int)Global.GetSafeAttributeLong(node, "MapCode");
						item2.QiZhiID = Global.GetSafeAttributeIntArray(node, "QiZhiID", -1, ',');
						item2.Name = Global.GetSafeAttributeStr(node, "Name");
						item2.QiZuoID = (int)Global.GetSafeAttributeLong(node, "QiZuoID");
						item2.Point = (int)Global.GetSafeAttributeLong(node, "Point");
						string[] strFields = Global.GetSafeAttributeStr(node, "QiZuoSite").Split(new char[]
						{
							'|'
						});
						if (strFields.Length == 2)
						{
							item2.PosX = Global.SafeConvertToInt32(strFields[0]);
							item2.PosY = Global.SafeConvertToInt32(strFields[1]);
						}
						item2.Rate = Global.GetSafeAttributeDouble(node, "Rate");
						this.RuntimeData.CompStrongholdConfigDict[item2.ID] = item2;
					}
					this.RuntimeData.CompBattleBirthConfigDict.Clear();
					fileName = "Config/ForceCraftBirth.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						CompBattleBirthConfig item3 = new CompBattleBirthConfig();
						item3.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item3.MapCode = (int)Global.GetSafeAttributeLong(node, "MapCode");
						item3.ForceID = (int)Global.GetSafeAttributeLong(node, "ForceID");
						item3.PosX = (int)Global.GetSafeAttributeLong(node, "PosX");
						item3.PosY = (int)Global.GetSafeAttributeLong(node, "PosY");
						item3.BirthRadius = (int)Global.GetSafeAttributeLong(node, "BirthRadius");
						this.RuntimeData.CompBattleBirthConfigDict[new KeyValuePair<int, int>(item3.MapCode, item3.ForceID)] = item3;
					}
					this.RuntimeData.CompBattleRewardConfigList.Clear();
					fileName = "Config/ForceCraftReward.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						CompBattleRewardConfig item4 = new CompBattleRewardConfig();
						item4.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item4.Rank = (int)Global.GetSafeAttributeLong(node, "Rank");
						item4.RankRate = Global.GetSafeAttributeDouble(node, "RankRate");
						item4.Grade = (int)Global.GetSafeAttributeLong(node, "Grade");
						item4.Contribution = (int)Global.GetSafeAttributeLong(node, "Contribution");
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(node, "GoodsOne"), ref item4.AwardsItemListOne, '|', ',');
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(node, "GoodsTwo"), ref item4.AwardsItemListTwo, '|', ',');
						this.RuntimeData.CompBattleRewardConfigList.Add(item4);
					}
					this.RuntimeData.CompBattleSingleIntegral = GameManager.systemParamsList.GetParamValueIntArrayByName("CraftSingleIntegral", ',');
					this.RuntimeData.CompBattleRewardRate = GameManager.systemParamsList.GetParamValueDoubleArrayByName("CraftRewardRate", ',');
					this.RuntimeData.CompBattleFlagDamage = (int)GameManager.systemParamsList.GetParamValueIntByName("FlagDamage", -1);
				}
				catch (Exception ex)
				{
					success = false;
					LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
				}
			}
			return success;
		}

		// Token: 0x06000879 RID: 2169 RVA: 0x00081EB4 File Offset: 0x000800B4
		private void TimerProc(object sender, EventArgs e)
		{
			DateTime now = TimeUtil.NowDateTime();
			lock (this.RuntimeData.Mutex)
			{
				CompBattleGameStates state = CompBattleGameStates.None;
				this.CheckCondition(null, ref state);
				if (CompBattleGameStates.None != state)
				{
					foreach (CompBattleScene scene in this.SceneDict.Values)
					{
						CompFuBenData fubenItem = null;
						if (this.RuntimeData.FuBenItemData.TryGetValue(scene.GameId, out fubenItem))
						{
							fubenItem.Init();
							List<GameClient> objsList = scene.CopyMap.GetClientsList();
							if (objsList != null && objsList.Count > 0)
							{
								for (int i = 0; i < objsList.Count; i++)
								{
									GameClient c = objsList[i];
									if (c != null)
									{
										int side = c.ClientData.BattleWhichSide;
										if (side > 0 && side <= fubenItem.RoleCountSideList.Count)
										{
											List<int> roleCountSideList;
											int index;
											(roleCountSideList = fubenItem.RoleCountSideList)[index = side - 1] = roleCountSideList[index] + 1;
											if (c.ClientData.CompZhiWu > 0)
											{
												fubenItem.ZhuJiangRoleDict[c.ClientData.CompType].Add(c.ClientData.RoleID);
											}
										}
									}
								}
							}
							TianTiClient.getInstance().Comp_UpdateKuaFuMapClientCount(30, fubenItem);
						}
					}
					foreach (KeyValuePair<KeyValuePair<int, int>, List<CompBattleWaitData>> kvp in this.RuntimeData.CompBattleWaitQueueDict)
					{
						KeyValuePair<int, int> kvpKey = kvp.Key;
						List<CompBattleWaitData> waitList = kvp.Value;
						if (waitList.Count > 0)
						{
							int cityId = kvpKey.Key;
							int compType = kvpKey.Value;
							CompBattleConfig compBattleConfig = null;
							if (this.RuntimeData.CompBattleConfigDict.TryGetValue(cityId, out compBattleConfig))
							{
								KuaFuServerInfo kfserverInfo = null;
								bool TryEnter = false;
								CompFuBenData fubenData = TianTiClient.getInstance().Comp_GetKuaFuFuBenData(30, cityId);
								if (fubenData != null && fubenData.GetRoleCountWithEnter(compType) < compBattleConfig.MaxEnterNum && KuaFuManager.getInstance().TryGetValue(fubenData.ServerId, out kfserverInfo))
								{
									TryEnter = true;
								}
								List<CompBattleWaitData> enterList = new List<CompBattleWaitData>();
								if (TryEnter)
								{
									for (int loop = 0; loop < waitList.Count; loop++)
									{
										CompBattleWaitData waitData = waitList[loop];
										GameClient client = GameManager.ClientMgr.FindClient(waitData.RoleId);
										if (null == client)
										{
											enterList.Add(waitData);
										}
										else
										{
											int result = TianTiClient.getInstance().Comp_GameFuBenRoleChangeState(30, client.ServerId, cityId, client.ClientData.RoleID, (int)client.ClientData.CompZhiWu, 4);
											if (result < 0)
											{
												break;
											}
											KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
											if (null != clientKuaFuServerLoginData)
											{
												clientKuaFuServerLoginData.RoleId = client.ClientData.RoleID;
												clientKuaFuServerLoginData.GameId = (long)fubenData.GameId;
												clientKuaFuServerLoginData.GameType = 30;
												clientKuaFuServerLoginData.EndTicks = fubenData.EndTime.Ticks;
												clientKuaFuServerLoginData.ServerId = client.ServerId;
												clientKuaFuServerLoginData.ServerIp = kfserverInfo.Ip;
												clientKuaFuServerLoginData.ServerPort = kfserverInfo.Port;
												clientKuaFuServerLoginData.FuBenSeqId = 0;
											}
											GlobalNew.RecordSwitchKuaFuServerLog(client);
											client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
											enterList.Add(waitData);
										}
									}
								}
								for (int loop = 0; loop < enterList.Count; loop++)
								{
									this.RemoveWait(enterList[loop].RoleId);
								}
								for (int loop = 0; loop < waitList.Count; loop++)
								{
									CompBattleWaitData waitData = waitList[loop];
									GameClient client = GameManager.ClientMgr.FindClient(waitData.RoleId);
									if (null != client)
									{
										int result = -22;
										client.sendCmd(2002, string.Format("{0}:{1}", result, loop + 1), false);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600087A RID: 2170 RVA: 0x000823EC File Offset: 0x000805EC
		public CompBattleBaseData GetCompBattleBaseData(int compType)
		{
			CompBattleBaseData cbBaseData = null;
			lock (this.RuntimeData.Mutex)
			{
				cbBaseData = (CompBattleBaseData)this.RuntimeData.compBattleBaseData.Clone();
			}
			return cbBaseData;
		}

		// Token: 0x0600087B RID: 2171 RVA: 0x00082458 File Offset: 0x00080658
		public void UpdateCompBattleBaseData(Dictionary<int, KFCompData> tempCompDataDict)
		{
			lock (this.RuntimeData.Mutex)
			{
				this.RuntimeData.compBattleBaseData.ClearAll();
				for (int compLoop = 1; compLoop <= 3; compLoop++)
				{
					CompBattleOwnCity ownCityData = new CompBattleOwnCity();
					KFCompData kfCompData = null;
					if (tempCompDataDict.TryGetValue(compLoop, out kfCompData))
					{
						foreach (KeyValuePair<int, CompStrongholdData> kvp in kfCompData.StrongholdDict)
						{
							if (kvp.Value.StrongholdSet.Count > 0 && 1 == kvp.Value.Rank)
							{
								ownCityData.OwnCityList.Add(kvp.Key);
							}
						}
					}
					this.RuntimeData.compBattleBaseData.CompBattleOwnCityList.Add(ownCityData);
				}
			}
		}

		// Token: 0x0600087C RID: 2172 RVA: 0x00082594 File Offset: 0x00080794
		public bool ProcessGetCompBattleBaseDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				CompBattleBaseData cbBaseData = null;
				lock (this.RuntimeData.Mutex)
				{
					cbBaseData = (CompBattleBaseData)this.RuntimeData.compBattleBaseData.Clone();
				}
				client.sendCmd<CompBattleBaseData>(nID, cbBaseData, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		// Token: 0x0600087D RID: 2173 RVA: 0x00082638 File Offset: 0x00080838
		public bool ProcessGetCompBattleCityDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				int cityID = Global.SafeConvertToInt32(cmdParams[1]);
				if (client.ClientData.CompType < 1 || client.ClientData.CompType > 3)
				{
					return true;
				}
				CompBattleConfig compBattleConfig = null;
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.CompBattleConfigDict.TryGetValue(cityID, out compBattleConfig))
					{
						return true;
					}
				}
				Dictionary<int, KFCompData> tempCompDataDict = null;
				lock (CompManager.getInstance().RuntimeData.Mutex)
				{
					tempCompDataDict = CompManager.getInstance().CompSyncDataCache.CompDataDict.V;
				}
				CompBattleCifyData cityData = new CompBattleCifyData();
				cityData.CityID = cityID;
				CompBattleGameStates state = CompBattleGameStates.None;
				this.CheckCondition(client, ref state);
				if (CompBattleGameStates.Start == state)
				{
					CompFuBenData fubenData = TianTiClient.getInstance().Comp_GetKuaFuFuBenData(30, cityID);
					if (null != fubenData)
					{
						cityData.RoleNum = fubenData.GetRoleCountWithEnter(client.ClientData.CompType);
						HashSet<int> zhujiangSet = null;
						if (fubenData.ZhuJiangRoleDict.TryGetValue(client.ClientData.CompType, out zhujiangSet) && null != zhujiangSet)
						{
							foreach (int rid in zhujiangSet)
							{
								KFCompRoleData compRoleData = TianTiClient.getInstance().Comp_GetCompRoleData(rid);
								if (compRoleData != null && null != compRoleData.RoleData4Selector)
								{
									RoleData4Selector OwnerRoleData = DataHelper.BytesToObject<RoleData4Selector>(compRoleData.RoleData4Selector, 0, compRoleData.RoleData4Selector.Length);
									if (null != OwnerRoleData)
									{
										CompBattleZhuJiangInfo zjInfo = new CompBattleZhuJiangInfo
										{
											RoleID = OwnerRoleData.RoleID,
											Name = OwnerRoleData.RoleName,
											Level = OwnerRoleData.Level,
											ZoneID = OwnerRoleData.ZoneId,
											Occupation = OwnerRoleData.Occupation,
											RoleSex = OwnerRoleData.RoleSex,
											CompZhiWu = compRoleData.ZhiWu
										};
										cityData.ZhuJiangList.Add(zjInfo);
									}
								}
							}
						}
					}
				}
				lock (this.RuntimeData.Mutex)
				{
					foreach (KFCompData compItem in tempCompDataDict.Values)
					{
						CompStrongholdData shData = null;
						if (compItem.StrongholdDict.TryGetValue(cityID, out shData))
						{
							foreach (int shId in shData.StrongholdSet)
							{
								cityData.StrongholdDict[shId] = compItem.CompType;
								if (1 == shData.Rank)
								{
									cityData.OwnCompType = compItem.CompType;
								}
							}
						}
					}
				}
				client.sendCmd<CompBattleCifyData>(nID, cityData, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		// Token: 0x0600087E RID: 2174 RVA: 0x00082AB4 File Offset: 0x00080CB4
		public bool ProcessCompBattleEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				int cityID = Global.SafeConvertToInt32(cmdParams[1]);
				int result = 0;
				CompBattleGameStates state = CompBattleGameStates.None;
				if (client.ClientData.CompType < 1 || client.ClientData.CompType > 3)
				{
					return true;
				}
				if (!this.CheckMap(client))
				{
					result = -21;
				}
				else
				{
					result = this.CheckCondition(client, ref state);
					if (state != CompBattleGameStates.Start)
					{
						result = -2001;
						client.sendCmd(nID, string.Format("{0}:{1}", result, 0), false);
						return true;
					}
				}
				lock (this.RuntimeData.Mutex)
				{
					CompBattleConfig compBattleConfig = null;
					if (!this.RuntimeData.CompBattleConfigDict.TryGetValue(cityID, out compBattleConfig))
					{
						this.RemoveWait(client.ClientData.RoleID);
						result = 0;
						client.sendCmd(nID, string.Format("{0}:{1}", result, 0), false);
						return true;
					}
					KuaFuServerInfo kfserverInfo = null;
					CompFuBenData fubenData = TianTiClient.getInstance().Comp_GetKuaFuFuBenData(30, cityID);
					if (fubenData == null || !KuaFuManager.getInstance().TryGetValue(fubenData.ServerId, out kfserverInfo))
					{
						result = -11000;
						client.sendCmd(nID, string.Format("{0}:{1}", result, 0), false);
						return true;
					}
					if (fubenData.GetRoleCountWithEnter(client.ClientData.CompType) >= compBattleConfig.MaxEnterNum)
					{
						if (this.AddToWait(cityID, client.ClientData.CompType, client.ClientData.RoleID))
						{
							result = -22;
							client.sendCmd(nID, string.Format("{0}:{1}", result, this.GetWaitingCount(cityID, client.ClientData.CompType)), false);
							return true;
						}
						result = -5;
						client.sendCmd(nID, string.Format("{0}:{1}", result, 0), false);
						return true;
					}
					else
					{
						KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
						if (null != clientKuaFuServerLoginData)
						{
							clientKuaFuServerLoginData.RoleId = client.ClientData.RoleID;
							clientKuaFuServerLoginData.GameId = (long)fubenData.GameId;
							clientKuaFuServerLoginData.GameType = 30;
							clientKuaFuServerLoginData.EndTicks = fubenData.EndTime.Ticks;
							clientKuaFuServerLoginData.ServerId = client.ServerId;
							clientKuaFuServerLoginData.ServerIp = kfserverInfo.Ip;
							clientKuaFuServerLoginData.ServerPort = kfserverInfo.Port;
							clientKuaFuServerLoginData.FuBenSeqId = 0;
						}
					}
				}
				if (result >= 0)
				{
					result = TianTiClient.getInstance().Comp_GameFuBenRoleChangeState(30, client.ServerId, cityID, client.ClientData.RoleID, (int)client.ClientData.CompZhiWu, 4);
					if (result >= 0)
					{
						GlobalNew.RecordSwitchKuaFuServerLog(client);
						client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
					}
					else
					{
						Global.GetClientKuaFuServerLoginData(client).RoleId = 0;
					}
				}
				client.sendCmd(nID, string.Format("{0}:{1}", result, 0), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		// Token: 0x0600087F RID: 2175 RVA: 0x00082E70 File Offset: 0x00081070
		public bool ProcessGetCompBattleAwardInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				KFCompRoleData compRoleData = TianTiClient.getInstance().Comp_GetCompRoleData(client.ClientData.RoleID);
				if (null == compRoleData)
				{
					return true;
				}
				CompBattleAwardsData awardsData = new CompBattleAwardsData();
				lock (this.RuntimeData.Mutex)
				{
					if (this.RuntimeData.compBattleBaseData.CompBattleOwnCityList.Count == 3)
					{
						awardsData.WinNum = this.RuntimeData.compBattleBaseData.CompBattleOwnCityList[compRoleData.CompType - 1].OwnCityList.Count;
					}
				}
				if (compRoleData.BattleJiFen > 0)
				{
					awardsData.RankNum = compRoleData.BattleRankNum;
					CompBattleRewardConfig awardConfig = this.CalBattleRewardConfig(client, compRoleData);
					awardsData.AwardID = ((awardConfig != null) ? awardConfig.ID : 0);
				}
				client.sendCmd<CompBattleAwardsData>(2003, awardsData, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		// Token: 0x06000880 RID: 2176 RVA: 0x00082FC0 File Offset: 0x000811C0
		public bool ProcessGetCompBattleStateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				CompBattleGameStates timeState = CompBattleGameStates.None;
				this.CheckCondition(client, ref timeState);
				int result = (int)timeState;
				KFCompRoleData compRoleData = TianTiClient.getInstance().Comp_GetCompRoleData(client.ClientData.RoleID);
				if (result == 0 && compRoleData != null && compRoleData.CompType == compRoleData.CompTypeBattle)
				{
					string awardsInfo = Global.GetRoleParamByName(client, "49");
					if (!string.IsNullOrEmpty(awardsInfo))
					{
						long LastStartTimeTicks = Global.SafeConvertToInt64(awardsInfo);
						if (TimeUtil.NOW() - LastStartTimeTicks < 604800000L)
						{
							client.sendCmd<int>(nID, 2, false);
							return true;
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
			return true;
		}

		// Token: 0x06000881 RID: 2177 RVA: 0x000830A8 File Offset: 0x000812A8
		public bool ProcessGetCompBattleSelfScoreCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (client.ClientData.CompType < 1 || client.ClientData.CompType > 3)
				{
					return true;
				}
				CompBattleScene scene = client.SceneObject as CompBattleScene;
				if (null == scene)
				{
					return true;
				}
				KFCompRoleData compRoleData = TianTiClient.getInstance().Comp_GetCompRoleData(client.ClientData.RoleID);
				if (null == compRoleData)
				{
					return true;
				}
				CompBattleSelfScore selfScore = new CompBattleSelfScore();
				CompBattleRewardConfig awardConfig = this.CalBattleRewardConfig(client, compRoleData);
				selfScore.RankNum = compRoleData.BattleRankNum;
				selfScore.AwardID = ((awardConfig != null) ? awardConfig.ID : 0);
				lock (CompManager.getInstance().RuntimeData.Mutex)
				{
					CompManager.getInstance().CompSyncDataCache.CompRankBattleJiFenDict.V.TryGetValue(client.ClientData.CompType, out selfScore.rankInfo2Client);
				}
				client.sendCmd<CompBattleSelfScore>(nID, selfScore, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		// Token: 0x06000882 RID: 2178 RVA: 0x0008322C File Offset: 0x0008142C
		public bool ProcessGetCompBattleAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				CompBattleGameStates timeState = CompBattleGameStates.None;
				this.CheckCondition(client, ref timeState);
				if (CompBattleGameStates.None != timeState)
				{
					int err = -2001;
					client.sendCmd<int>(nID, err, false);
					return true;
				}
				string awardsInfo = Global.GetRoleParamByName(client, "49");
				if (!string.IsNullOrEmpty(awardsInfo))
				{
					bool clear = true;
					long LastStartTimeTicks = Global.SafeConvertToInt64(awardsInfo);
					int err;
					if (TimeUtil.NOW() - LastStartTimeTicks < 604800000L)
					{
						err = this.GiveRoleAwards(client, LastStartTimeTicks);
						if (err < 0)
						{
							clear = false;
						}
					}
					else
					{
						err = -5;
					}
					if (clear)
					{
						Global.SaveRoleParamsStringToDB(client, "49", "", true);
					}
					client.sendCmd<int>(nID, err, false);
				}
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		// Token: 0x06000883 RID: 2179 RVA: 0x00083330 File Offset: 0x00081530
		public int GetWaitingCount(int cityId, int comptype)
		{
			int result;
			lock (this.RuntimeData.Mutex)
			{
				KeyValuePair<int, int> kvpKey = new KeyValuePair<int, int>(cityId, comptype);
				List<CompBattleWaitData> waitList = null;
				if (!this.RuntimeData.CompBattleWaitQueueDict.TryGetValue(kvpKey, out waitList))
				{
					result = 0;
				}
				else
				{
					result = waitList.Count;
				}
			}
			return result;
		}

		// Token: 0x06000884 RID: 2180 RVA: 0x000833B0 File Offset: 0x000815B0
		public bool IsInWait(int rid)
		{
			try
			{
				if (rid <= 0)
				{
					return false;
				}
				lock (this.RuntimeData.Mutex)
				{
					if (this.RuntimeData.CompBattleWaitAllDict.ContainsKey(rid))
					{
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("CompBattleManager::IsInWait roleID={0}", rid));
				return false;
			}
			return false;
		}

		// Token: 0x06000885 RID: 2181 RVA: 0x00083458 File Offset: 0x00081658
		public bool AddToWait(int cityId, int comptype, int rid)
		{
			try
			{
				if (this.IsInWait(rid))
				{
					return false;
				}
				KeyValuePair<int, int> kvpKey = new KeyValuePair<int, int>(cityId, comptype);
				List<CompBattleWaitData> waitList = null;
				if (!this.RuntimeData.CompBattleWaitQueueDict.TryGetValue(kvpKey, out waitList))
				{
					waitList = new List<CompBattleWaitData>();
					this.RuntimeData.CompBattleWaitQueueDict[kvpKey] = waitList;
				}
				waitList.Add(new CompBattleWaitData
				{
					CityID = cityId,
					CompType = comptype,
					RoleId = rid
				});
				this.RuntimeData.CompBattleWaitAllDict[rid] = kvpKey;
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("CompBattleManager::AddToWait roleID={0}", rid));
				return false;
			}
			return true;
		}

		// Token: 0x06000886 RID: 2182 RVA: 0x00083558 File Offset: 0x00081758
		public void RemoveWait(int rid)
		{
			try
			{
				lock (this.RuntimeData.Mutex)
				{
					if (this.IsInWait(rid))
					{
						KeyValuePair<int, int> kvpKey;
						this.RuntimeData.CompBattleWaitAllDict.TryGetValue(rid, out kvpKey);
						List<CompBattleWaitData> waitList = null;
						if (this.RuntimeData.CompBattleWaitQueueDict.TryGetValue(kvpKey, out waitList))
						{
							waitList.RemoveAll((CompBattleWaitData x) => x.RoleId == rid);
							this.RuntimeData.CompBattleWaitAllDict.Remove(rid);
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("CompBattleManager::RemoveWait roleID={0}", rid));
			}
		}

		// Token: 0x06000887 RID: 2183 RVA: 0x00083674 File Offset: 0x00081874
		public void OnCompBattleReset()
		{
			foreach (GameClient client in GameManager.ClientMgr.GetAllClients(true))
			{
				if (client != null && client.ClientData.CompType > 0)
				{
					int BattleJiFen = GameManager.ClientMgr.GetCompBattleJiFenValue(client);
					if (BattleJiFen > 0)
					{
						GameManager.ClientMgr.ModifyCompBattleJiFenValue(client, -BattleJiFen, "势力战KF", true, true, false);
					}
				}
			}
		}

		// Token: 0x06000888 RID: 2184 RVA: 0x00083714 File Offset: 0x00081914
		private bool CheckMap(GameClient client)
		{
			SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
			return sceneType == SceneUIClasses.Normal || sceneType == SceneUIClasses.Comp;
		}

		// Token: 0x06000889 RID: 2185 RVA: 0x00083750 File Offset: 0x00081950
		public int CheckCondition(GameClient client, ref CompBattleGameStates state)
		{
			int result = 0;
			CompBattleConfig sceneItem = null;
			if (client != null && !this.IsGongNengOpened(client, true))
			{
				result = -13;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					sceneItem = this.RuntimeData.CompBattleConfigDict.Values.FirstOrDefault<CompBattleConfig>();
					if (null == sceneItem)
					{
						return -12;
					}
				}
				result = -2001;
				DateTime now = TimeUtil.NowDateTime();
				lock (this.RuntimeData.Mutex)
				{
					for (int i = 0; i < sceneItem.TimePoints.Count - 1; i += 2)
					{
						if (now.DayOfWeek == (DayOfWeek)sceneItem.TimePoints[i].Days && now.TimeOfDay.TotalSeconds >= sceneItem.SecondsOfDay[i] && now.TimeOfDay.TotalSeconds <= sceneItem.SecondsOfDay[i + 1])
						{
							if (now.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i + 1] - (double)sceneItem.ClearRolesSecs)
							{
								state = CompBattleGameStates.Start;
								result = 1;
							}
							else if (now.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i + 1])
							{
								state = CompBattleGameStates.Analysis;
								result = 1;
							}
							else
							{
								state = CompBattleGameStates.None;
								result = -2001;
							}
							break;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600088A RID: 2186 RVA: 0x00083964 File Offset: 0x00081B64
		private TimeSpan GetStartTime(int sceneId)
		{
			CompBattleConfig sceneItem = null;
			TimeSpan startTime = TimeSpan.MinValue;
			DateTime now = TimeUtil.NowDateTime();
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.CompBattleConfigDict.TryGetValue(sceneId, out sceneItem))
				{
					goto IL_14B;
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < sceneItem.TimePoints.Count - 1; i += 2)
				{
					if (now.DayOfWeek == (DayOfWeek)sceneItem.TimePoints[i].Days && now.TimeOfDay.TotalSeconds >= sceneItem.SecondsOfDay[i] && now.TimeOfDay.TotalSeconds <= sceneItem.SecondsOfDay[i + 1])
					{
						startTime = TimeSpan.FromSeconds(sceneItem.SecondsOfDay[i]);
						break;
					}
				}
			}
			IL_14B:
			if (startTime < TimeSpan.Zero)
			{
				startTime = now.TimeOfDay;
			}
			return startTime;
		}

		// Token: 0x0600088B RID: 2187 RVA: 0x00083B00 File Offset: 0x00081D00
		public bool KuaFuLogin(KuaFuServerLoginData kuaFuServerLoginData)
		{
			CompFuBenData kfubenData = TianTiClient.getInstance().Comp_GetKuaFuFuBenData(30, (int)kuaFuServerLoginData.GameId);
			bool result;
			if (kfubenData == null || kfubenData.ServerId != GameManager.ServerId)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("{0}不具有进入跨服地图{1}的资格", kuaFuServerLoginData.RoleId, kuaFuServerLoginData.GameId), null, true);
				result = false;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					CompBattleConfig sceneInfo;
					if (!this.RuntimeData.CompBattleConfigDict.TryGetValue(kfubenData.GameId, out sceneInfo) || (long)sceneInfo.ID != kuaFuServerLoginData.GameId)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("{0}不具有进入跨服地图{1}的资格", kuaFuServerLoginData.RoleId, kuaFuServerLoginData.GameId), null, true);
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600088C RID: 2188 RVA: 0x00083C14 File Offset: 0x00081E14
		public bool OnInitGameKuaFu(GameClient client)
		{
			KuaFuServerLoginData kuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
			CompFuBenData fuBenData;
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.FuBenItemData.TryGetValue((int)kuaFuServerLoginData.GameId, out fuBenData))
				{
					fuBenData = null;
				}
				else if (fuBenData.State >= GameFuBenState.End)
				{
					return false;
				}
			}
			if (null == fuBenData)
			{
				CompFuBenData newFuBenData = TianTiClient.getInstance().Comp_GetKuaFuFuBenData(30, (int)kuaFuServerLoginData.GameId);
				if (newFuBenData == null || newFuBenData.State == GameFuBenState.End)
				{
					LogManager.WriteLog(LogTypes.Error, ("获取不到有效的副本数据," + newFuBenData == null) ? "fuBenData == null" : "fuBenData.State == GameFuBenState.End", null, true);
					return false;
				}
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.FuBenItemData.TryGetValue((int)kuaFuServerLoginData.GameId, out fuBenData))
					{
						fuBenData = newFuBenData;
						fuBenData.SequenceId = GameCoreInterface.getinstance().GetNewFuBenSeqId();
						fuBenData.Init();
						this.RuntimeData.FuBenItemData[fuBenData.GameId] = fuBenData;
					}
				}
			}
			CompBattleConfig sceneInfo;
			lock (this.RuntimeData.Mutex)
			{
				kuaFuServerLoginData.FuBenSeqId = fuBenData.SequenceId;
				if (!this.RuntimeData.CompBattleConfigDict.TryGetValue(fuBenData.GameId, out sceneInfo))
				{
					return false;
				}
			}
			client.ClientData.BattleWhichSide = client.ClientData.CompType;
			int posX;
			int posY;
			int side = this.GetBirthPoint(sceneInfo.MapCode, client, out posX, out posY);
			bool result;
			if (side <= 0)
			{
				LogManager.WriteLog(LogTypes.Error, "无法获取有效的阵营和出生点,进入跨服失败,side=" + side, null, true);
				result = false;
			}
			else
			{
				client.ClientData.MapCode = sceneInfo.MapCode;
				client.ClientData.PosX = posX;
				client.ClientData.PosY = posY;
				client.ClientData.FuBenSeqID = kuaFuServerLoginData.FuBenSeqId;
				result = true;
			}
			return result;
		}

		// Token: 0x0600088D RID: 2189 RVA: 0x00083EC0 File Offset: 0x000820C0
		private void OnLogout(GameClient client)
		{
			this.LeaveFuBen(client);
			this.RemoveWait(client.ClientData.RoleID);
		}

		// Token: 0x0600088E RID: 2190 RVA: 0x00083EE0 File Offset: 0x000820E0
		public bool ClientRelive(GameClient client)
		{
			int toPosX;
			int toPosY;
			int side = this.GetBirthPoint(client.ClientData.MapCode, client, out toPosX, out toPosY);
			bool result;
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
			return result;
		}

		// Token: 0x0600088F RID: 2191 RVA: 0x00083F8C File Offset: 0x0008218C
		public int GetBirthPoint(int mapCode, GameClient client, out int posX, out int posY)
		{
			int side = client.ClientData.BattleWhichSide;
			lock (this.RuntimeData.Mutex)
			{
				CompBattleBirthConfig birthPoint = null;
				KeyValuePair<int, int> kvpKey = new KeyValuePair<int, int>(mapCode, side);
				if (this.RuntimeData.CompBattleBirthConfigDict.TryGetValue(kvpKey, out birthPoint))
				{
					posX = birthPoint.PosX;
					posY = birthPoint.PosY;
					return side;
				}
			}
			posX = 0;
			posY = 0;
			return -1;
		}

		// Token: 0x06000890 RID: 2192 RVA: 0x000840E0 File Offset: 0x000822E0
		public void UpdateBattleSideScoreRank(CompBattleScene scene, bool update = true)
		{
			try
			{
				if (scene.ScoreData.Count != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("势力战分数信息异常 CityID={0}", scene.GameId), null, true);
				}
				else
				{
					foreach (CompBattleSideScore scoreData in scene.ScoreData)
					{
						scoreData.Rate = 0.0;
						foreach (int shId in scoreData.StrongholdSet)
						{
							CompStrongholdConfig shConfig = null;
							if (this.RuntimeData.CompStrongholdConfigDict.TryGetValue(shId, out shConfig))
							{
								scoreData.Rate += shConfig.Rate;
							}
						}
					}
					scene.ScoreData.Sort(delegate(CompBattleSideScore left, CompBattleSideScore right)
					{
						int result;
						if (left.Rate > right.Rate)
						{
							result = -1;
						}
						else if (left.Rate < right.Rate)
						{
							result = 1;
						}
						else if (left.Rank < right.Rank)
						{
							result = -1;
						}
						else if (left.Rank > right.Rank)
						{
							result = 1;
						}
						else if (left.CompType < right.CompType)
						{
							result = -1;
						}
						else if (left.CompType > right.CompType)
						{
							result = 1;
						}
						else
						{
							result = 0;
						}
						return result;
					});
					List<CompStrongholdData> shUpdateData = new List<CompStrongholdData>(new CompStrongholdData[3]);
					for (int loop = 0; loop < scene.ScoreData.Count; loop++)
					{
						CompBattleSideScore ssData = scene.ScoreData[loop];
						ssData.Rank = loop + 1;
						CompStrongholdData shData = new CompStrongholdData();
						shData.Rank = ssData.Rank;
						shData.StrongholdSet = new HashSet<int>(ssData.StrongholdSet);
						shUpdateData[ssData.CompType - 1] = shData;
					}
					if (update)
					{
						TianTiClient.getInstance().Comp_UpdateStrongholdData(scene.SceneInfo.ID, shUpdateData);
						GameManager.ClientMgr.BroadSpecialCopyMapMessage<List<CompBattleSideScore>>(2005, scene.ScoreData, scene.CopyMap);
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
		}

		// Token: 0x06000891 RID: 2193 RVA: 0x00084334 File Offset: 0x00082534
		public int CalTopOwnCityCompType()
		{
			int compType = 0;
			int cityCount = 0;
			lock (this.RuntimeData.Mutex)
			{
				if (this.RuntimeData.compBattleBaseData.CompBattleOwnCityList.Count != 3)
				{
					return compType;
				}
				for (int compLoop = 1; compLoop <= 3; compLoop++)
				{
					CompBattleOwnCity ownCity = this.RuntimeData.compBattleBaseData.CompBattleOwnCityList[compLoop - 1];
					if (ownCity.OwnCityList.Count == cityCount)
					{
						compType = 0;
					}
					if (ownCity.OwnCityList.Count > cityCount)
					{
						compType = compLoop;
						cityCount = ownCity.OwnCityList.Count;
					}
				}
			}
			return compType;
		}

		// Token: 0x06000892 RID: 2194 RVA: 0x00084428 File Offset: 0x00082628
		public bool CheckCompShopDuiHuanType(GameClient client, int nDuiHuanType)
		{
			try
			{
				CompBattleGameStates state = CompBattleGameStates.None;
				this.CheckCondition(null, ref state);
				if (CompBattleGameStates.None != state)
				{
					return false;
				}
				if (nDuiHuanType == CompManager.getInstance().GetCompShopDuiHuanType(CompShopDHTypeIndex.CSDH_CompBattleTop))
				{
					if (client.ClientData.CompType == this.CalTopOwnCityCompType())
					{
						return true;
					}
				}
				else
				{
					lock (this.RuntimeData.Mutex)
					{
						if (this.RuntimeData.compBattleBaseData.CompBattleOwnCityList.Count != 3)
						{
							return false;
						}
						CompBattleOwnCity ownCity = this.RuntimeData.compBattleBaseData.CompBattleOwnCityList[client.ClientData.CompType - 1];
						foreach (int cityId in ownCity.OwnCityList)
						{
							CompBattleConfig compBattleConfig = null;
							if (this.RuntimeData.CompBattleConfigDict.TryGetValue(cityId, out compBattleConfig))
							{
								if (compBattleConfig.DuiHuanType == nDuiHuanType)
								{
									return true;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			return false;
		}

		// Token: 0x06000893 RID: 2195 RVA: 0x00084618 File Offset: 0x00082818
		public void InstallJunQi(CompBattleScene scene, int comptype, CompStrongholdConfig item, bool updateScoreRank = true)
		{
			CopyMap copyMap = scene.CopyMap;
			GameMap gameMap = GameManager.MapMgr.GetGameMap(scene.m_nMapCode);
			if (copyMap != null && null != gameMap)
			{
				item.Alive = true;
				item.BattleWhichSide = comptype;
				int BattleQiZhiMonsterID = item.QiZhiID[comptype - 1];
				GameManager.MonsterZoneMgr.AddDynamicMonsters(copyMap.MapCode, BattleQiZhiMonsterID, copyMap.CopyMapID, 1, item.PosX / gameMap.MapGridWidth, item.PosY / gameMap.MapGridHeight, 0, 0, SceneUIClasses.CompBattle, item, null);
				CompBattleSideScore scoreData = scene.ScoreData.Find((CompBattleSideScore x) => x.CompType == comptype);
				if (null != scoreData)
				{
					scoreData.StrongholdSet.Add(item.ID);
				}
				if (updateScoreRank)
				{
					this.UpdateBattleSideScoreRank(scene, true);
				}
			}
		}

		// Token: 0x06000894 RID: 2196 RVA: 0x00084710 File Offset: 0x00082910
		public bool OnSpriteClickOnNpc(GameClient client, int npcID, int npcExtentionID)
		{
			CompStrongholdConfig item = null;
			bool isQiZuo = false;
			bool installJunQi = false;
			CompBattleScene scene = client.SceneObject as CompBattleScene;
			bool result;
			if (null == scene)
			{
				result = isQiZuo;
			}
			else
			{
				Dictionary<int, CompConfig> tempCompConfigDict = null;
				lock (CompManager.getInstance().RuntimeData.Mutex)
				{
					tempCompConfigDict = CompManager.getInstance().RuntimeData.CompConfigDict;
				}
				lock (this.RuntimeData.Mutex)
				{
					if (scene.CompStrongholdConfigDict.TryGetValue(npcExtentionID, out item))
					{
						isQiZuo = true;
						if (item.Alive)
						{
							return isQiZuo;
						}
						if (client.ClientData.BattleWhichSide != item.BattleWhichSide && Math.Abs(TimeUtil.NOW() - item.DeadTicks) < 3000L)
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(12, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						}
						else if (scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
						{
							if (Math.Abs(client.ClientData.PosX - item.PosX) <= 1000 && Math.Abs(client.ClientData.PosY - item.PosY) <= 1000)
							{
								installJunQi = true;
							}
						}
					}
					if (installJunQi)
					{
						if (item.BattleWhichSideLast != client.ClientData.CompType)
						{
							CompConfig comp = null;
							CompConfig comp2 = null;
							tempCompConfigDict.TryGetValue(item.BattleWhichSideLast, out comp);
							tempCompConfigDict.TryGetValue(client.ClientData.CompType, out comp2);
							if (comp != null && null != comp2)
							{
								string horseNoticeMsg = string.Format(GLang.GetLang(5005, new object[0]), comp.CompName, item.Name, comp2.CompName);
								BulletinMsgData bulletinMsgData = new BulletinMsgData
								{
									MsgID = "comp-battle-install-junqi",
									PlayMinutes = -1,
									ToPlayNum = -1,
									BulletinText = horseNoticeMsg,
									BulletinTicks = TimeUtil.NOW(),
									playingNum = 0
								};
								List<GameClient> objsList = scene.CopyMap.GetClientsList();
								if (objsList != null && objsList.Count > 0)
								{
									for (int i = 0; i < objsList.Count; i++)
									{
										GameClient c = objsList[i];
										if (c != null)
										{
											GameManager.ClientMgr.NotifyBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, c, bulletinMsgData);
										}
									}
								}
							}
						}
						this.InstallJunQi(scene, client.ClientData.CompType, item, true);
					}
				}
				result = isQiZuo;
			}
			return result;
		}

		// Token: 0x06000895 RID: 2197 RVA: 0x00084A80 File Offset: 0x00082C80
		private void InitScene(CompBattleScene scene, GameClient client, Dictionary<int, KFCompData> compDataDict)
		{
			foreach (CompStrongholdConfig item in this.RuntimeData.CompStrongholdConfigDict.Values)
			{
				if (scene.m_nMapCode == item.MapCode)
				{
					scene.CompStrongholdConfigDict.Add(item.QiZuoID, item.Clone() as CompStrongholdConfig);
				}
			}
			for (int compLoop = 1; compLoop <= 3; compLoop++)
			{
				KFCompData compData = null;
				CompStrongholdData shData = null;
				CompBattleSideScore scoreData = new CompBattleSideScore();
				scoreData.CompType = compLoop;
				if (compDataDict.TryGetValue(compLoop, out compData) && compData.StrongholdDict.TryGetValue(scene.SceneInfo.ID, out shData))
				{
					scoreData.Rank = shData.Rank;
					scoreData.StrongholdSet = shData.StrongholdSet;
					foreach (int shId in shData.StrongholdSet)
					{
						CompStrongholdConfig shConfig = null;
						if (this.RuntimeData.CompStrongholdConfigDict.TryGetValue(shId, out shConfig))
						{
							if (scene.CompStrongholdConfigDict.TryGetValue(shConfig.QiZuoID, out shConfig))
							{
								shConfig.BattleWhichSideLast = compLoop;
								this.InstallJunQi(scene, compLoop, shConfig, false);
							}
						}
					}
				}
				scene.ScoreData.Add(scoreData);
			}
			this.UpdateBattleSideScoreRank(scene, false);
		}

		// Token: 0x06000896 RID: 2198 RVA: 0x00084C40 File Offset: 0x00082E40
		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == SceneUIClasses.CompBattle)
			{
				GameMap gameMap = null;
				if (!GameManager.MapMgr.DictMaps.TryGetValue(client.ClientData.MapCode, out gameMap))
				{
					result = false;
				}
				else
				{
					Dictionary<int, KFCompData> tempCompDataDict = null;
					lock (CompManager.getInstance().RuntimeData.Mutex)
					{
						tempCompDataDict = CompManager.getInstance().CompSyncDataCache.CompDataDict.V;
					}
					int fuBenSeqId = copyMap.FuBenSeqID;
					int mapCode = copyMap.MapCode;
					int roleId = client.ClientData.RoleID;
					int gameId = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
					DateTime now = TimeUtil.NowDateTime();
					lock (this.RuntimeData.Mutex)
					{
						CompBattleScene scene = null;
						if (!this.SceneDict.TryGetValue(fuBenSeqId, out scene))
						{
							CompBattleConfig sceneInfo = null;
							CompFuBenData fuBenData;
							if (!this.RuntimeData.FuBenItemData.TryGetValue(gameId, out fuBenData))
							{
								LogManager.WriteLog(LogTypes.Error, "势力战场没有为副本找到对应的跨服副本数据,GameID:" + gameId, null, true);
							}
							if (!this.RuntimeData.CompBattleConfigDict.TryGetValue(fuBenData.GameId, out sceneInfo))
							{
								LogManager.WriteLog(LogTypes.Error, "势力战场没有为副本找到对应的档位数据,ID:" + fuBenData.GameId, null, true);
							}
							scene = new CompBattleScene();
							scene.CopyMap = copyMap;
							scene.CleanAllInfo();
							scene.GameId = gameId;
							scene.m_nMapCode = mapCode;
							scene.CopyMapId = copyMap.CopyMapID;
							scene.FuBenSeqId = fuBenSeqId;
							scene.SceneInfo = sceneInfo;
							scene.MapGridWidth = gameMap.MapGridWidth;
							scene.MapGridHeight = gameMap.MapGridHeight;
							DateTime startTime = now.Date.Add(this.GetStartTime(sceneInfo.ID));
							scene.StartTimeTicks = startTime.Ticks / 10000L;
							this.InitScene(scene, client, tempCompDataDict);
							this.SceneDict[fuBenSeqId] = scene;
						}
						CompBattleClientContextData clientContextData;
						if (!scene.ClientContextDataDict.TryGetValue(roleId, out clientContextData))
						{
							clientContextData = new CompBattleClientContextData
							{
								RoleId = roleId,
								ServerId = client.ServerId,
								BattleWhichSide = client.ClientData.BattleWhichSide,
								RoleName = client.ClientData.RoleName,
								Occupation = client.ClientData.Occupation,
								RoleSex = client.ClientData.RoleSex,
								ZoneID = client.ClientData.ZoneID,
								CompZhiWu = (int)client.ClientData.CompZhiWu
							};
							scene.ClientContextDataDict[roleId] = clientContextData;
						}
						else
						{
							clientContextData.KillNum = 0;
						}
						client.SceneObject = scene;
						client.SceneGameId = (long)scene.GameId;
						client.SceneContextData2 = clientContextData;
						copyMap.IsKuaFuCopy = true;
						copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(scene.SceneInfo.TotalSecs * 1000));
						TianTiClient.getInstance().Comp_GameFuBenRoleChangeState(30, client.ServerId, scene.SceneInfo.ID, roleId, (int)client.ClientData.CompZhiWu, 5);
					}
					result = true;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000897 RID: 2199 RVA: 0x00084FF8 File Offset: 0x000831F8
		public bool RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == SceneUIClasses.CompBattle)
			{
				lock (this.RuntimeData.Mutex)
				{
					CompBattleScene CompBattleScene;
					this.SceneDict.TryRemove(copyMap.FuBenSeqID, out CompBattleScene);
					this.RuntimeData.FuBenItemData.Remove(CompBattleScene.GameId);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000898 RID: 2200 RVA: 0x00085088 File Offset: 0x00083288
		public void TimerProc()
		{
			long nowTicks = TimeUtil.NOW();
			if (nowTicks >= CompBattleManager.NextHeartBeatTicks)
			{
				CompBattleManager.NextHeartBeatTicks = nowTicks + 1020L;
				foreach (CompBattleScene scene in this.SceneDict.Values)
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
							if (scene.m_eStatus == GameSceneStatuses.STATUS_PREPARE || scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
							{
							}
							if (scene.m_eStatus == GameSceneStatuses.STATUS_NULL)
							{
								if (ticks >= scene.StartTimeTicks)
								{
									scene.m_lPrepareTime = scene.StartTimeTicks;
									scene.m_lBeginTime = scene.m_lPrepareTime + (long)(scene.SceneInfo.PrepareSecs * 1000);
									scene.m_eStatus = GameSceneStatuses.STATUS_PREPARE;
									scene.StateTimeData.GameType = 30;
									scene.StateTimeData.State = (int)scene.m_eStatus;
									scene.StateTimeData.EndTicks = scene.m_lBeginTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_PREPARE)
							{
								if (ticks >= scene.m_lBeginTime)
								{
									scene.m_eStatus = GameSceneStatuses.STATUS_BEGIN;
									scene.m_lEndTime = scene.m_lBeginTime + (long)(scene.SceneInfo.FightingSecs * 1000);
									scene.StateTimeData.GameType = 30;
									scene.StateTimeData.State = (int)scene.m_eStatus;
									scene.StateTimeData.EndTicks = scene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
									for (int guangMuId = 1; guangMuId <= 3; guangMuId++)
									{
										GameManager.CopyMapMgr.AddGuangMuEvent(copyMap, guangMuId, 0);
									}
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
							{
								if (ticks >= scene.m_lEndTime)
								{
									this.ProcessEnd(scene, ticks);
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_END)
							{
								GameManager.CopyMapMgr.KillAllMonster(scene.CopyMap);
								scene.m_eStatus = GameSceneStatuses.STATUS_AWARD;
								TianTiClient.getInstance().Comp_GameFuBenRoleChangeState(30, -1, scene.SceneInfo.ID, -1, -1, 6);
								this.GiveAwards(scene);
								CompFuBenData fuBenData;
								if (this.RuntimeData.FuBenItemData.TryGetValue(scene.GameId, out fuBenData))
								{
									fuBenData.State = GameFuBenState.End;
									LogManager.WriteLog(LogTypes.Error, string.Format("势力战场跨服副本GameID={0},战斗结束", fuBenData.GameId), null, true);
								}
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
										DataHelper.WriteExceptionLogEx(ex, "势力战场系统清场调度异常");
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000899 RID: 2201 RVA: 0x0008550C File Offset: 0x0008370C
		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool sideScore = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				CompBattleScene scene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, scene.StateTimeData, false);
					}
					if (sideScore)
					{
						client.sendCmd<List<CompBattleSideScore>>(2005, scene.ScoreData, false);
					}
				}
			}
		}

		// Token: 0x0600089A RID: 2202 RVA: 0x000855B4 File Offset: 0x000837B4
		private CompBattleRewardConfig CalBattleRewardConfig(GameClient client, KFCompRoleData compRoleData)
		{
			CompBattleRewardConfig awardConfig = null;
			double rankPct = 0.0;
			lock (CompManager.getInstance().RuntimeData.Mutex)
			{
				if (CompManager.getInstance().CompSyncDataCache.CompBattleJoinRoleNum.Length != 0)
				{
					int joinNum = CompManager.getInstance().CompSyncDataCache.CompBattleJoinRoleNum[client.ClientData.CompType - 1];
					rankPct = ((joinNum > 0) ? ((double)compRoleData.BattleRankNum / (double)joinNum) : 1.0);
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				foreach (CompBattleRewardConfig item in this.RuntimeData.CompBattleRewardConfigList)
				{
					if (item.Rank > 0 && item.Rank == compRoleData.BattleRankNum)
					{
						awardConfig = item;
						break;
					}
					if (item.RankRate > 0.0 && rankPct <= item.RankRate)
					{
						awardConfig = item;
						break;
					}
				}
			}
			return awardConfig;
		}

		// Token: 0x0600089B RID: 2203 RVA: 0x00085778 File Offset: 0x00083978
		private int GiveRoleAwards(GameClient client, long LastStartTimeTicks)
		{
			KFCompRoleData compRoleData = TianTiClient.getInstance().Comp_GetCompRoleData(client.ClientData.RoleID);
			int result;
			if (null == compRoleData)
			{
				result = -11003;
			}
			else
			{
				CompBattleRewardConfig awardConfig = this.CalBattleRewardConfig(client, compRoleData);
				if (null == awardConfig)
				{
					result = -3;
				}
				else
				{
					int wincount = this.RuntimeData.compBattleBaseData.CompBattleOwnCityList[compRoleData.CompType - 1].OwnCityList.Count;
					double rewardRate = 1.0;
					if (wincount > 0 && wincount <= this.RuntimeData.CompBattleRewardRate.Length)
					{
						lock (this.RuntimeData.Mutex)
						{
							rewardRate = this.RuntimeData.CompBattleRewardRate[wincount - 1];
						}
					}
					int addGrade = (int)((double)awardConfig.Grade * rewardRate);
					int addContribution = (int)((double)awardConfig.Contribution * rewardRate);
					List<AwardsItemData> awardsItemDataListOne = awardConfig.AwardsItemListOne.Items;
					List<AwardsItemData> awardsItemDataListTwo = awardConfig.AwardsItemListTwo.Items;
					int awardCnt = 0;
					if (awardsItemDataListOne != null)
					{
						awardCnt += awardsItemDataListOne.Count;
					}
					if (awardsItemDataListTwo != null)
					{
						awardCnt += awardsItemDataListTwo.Count((AwardsItemData goods) => Global.IsRoleOccupationMatchGoods(client, goods.GoodsID));
					}
					if (!Global.CanAddGoodsNum(client, awardCnt))
					{
						result = -100;
					}
					else
					{
						if (addGrade > 0 && CompManager.getInstance().CheckCanAddJunXian(LastStartTimeTicks))
						{
							TianTiClient.getInstance().Comp_CompOpt(client.ClientData.CompType, 1, client.ClientData.RoleID, addGrade);
							string broadMsg = string.Format(GLang.GetLang(4017, new object[0]), addGrade);
							GameManager.ClientMgr.NotifyImportantMsg(client, broadMsg, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						}
						if (addContribution > 0)
						{
							GameManager.ClientMgr.ModifyCompDonateValue(client, addContribution, "势力战场奖励", true, true, false);
						}
						if (awardsItemDataListOne != null)
						{
							foreach (AwardsItemData item in awardsItemDataListOne)
							{
								int GoodsNum = (int)((double)item.GoodsNum * rewardRate);
								if (GoodsNum > 0)
								{
									Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, GoodsNum, 0, "", item.Level, item.Binding, 0, "", true, 1, "势力战场奖励", "1900-01-01 12:00:00", 0, 0, item.IsHaveLuckyProp, 0, item.ExcellencePorpValue, item.AppendLev, 0, null, null, 0, true);
								}
							}
						}
						if (awardsItemDataListTwo != null)
						{
							foreach (AwardsItemData item in awardsItemDataListTwo)
							{
								if (Global.IsCanGiveRewardByOccupation(client, item.GoodsID))
								{
									int GoodsNum = (int)((double)item.GoodsNum * rewardRate);
									if (GoodsNum > 0)
									{
										Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, GoodsNum, 0, "", item.Level, item.Binding, 0, "", true, 1, "势力战场奖励", "1900-01-01 12:00:00", 0, 0, item.IsHaveLuckyProp, 0, item.ExcellencePorpValue, item.AppendLev, 0, null, null, 0, true);
									}
								}
							}
						}
						result = 1;
					}
				}
			}
			return result;
		}

		// Token: 0x0600089C RID: 2204 RVA: 0x00085C60 File Offset: 0x00083E60
		private void UpdateMapBuffer(CompBattleScene scene, GameClient client, bool login)
		{
			if (client.ClientData.CompType > 0)
			{
				List<CompLevelConfig> tempCompLevelConfigList = null;
				lock (CompManager.getInstance().RuntimeData.Mutex)
				{
					tempCompLevelConfigList = CompManager.getInstance().RuntimeData.CompLevelConfigList;
				}
				if (login)
				{
					CompFuBenData fuBenData;
					if (!this.RuntimeData.FuBenItemData.TryGetValue(scene.GameId, out fuBenData))
					{
						return;
					}
					HashSet<int> zhujiangSet = null;
					if (fuBenData.ZhuJiangRoleDict.TryGetValue(client.ClientData.CompType, out zhujiangSet) && null != zhujiangSet)
					{
						foreach (int rid in zhujiangSet)
						{
							GameClient zhujiangClient = GameManager.ClientMgr.FindClient(rid);
							if (null != zhujiangClient)
							{
								CompLevelConfig levelConfig = tempCompLevelConfigList.Find((CompLevelConfig x) => x.CompID == client.ClientData.CompType && x.Level == (int)zhujiangClient.ClientData.CompZhiWu);
								if (null != levelConfig)
								{
									BufferItemTypes buffTypes = BufferItemTypes.CompBattle_Self + (int)zhujiangClient.ClientData.CompZhiWu;
									CompManager.getInstance().UpdateBuff4GameClient(client, buffTypes, levelConfig.CraftBuffID, true);
								}
							}
						}
					}
				}
				if (client.ClientData.CompZhiWu > 0)
				{
					CompLevelConfig levelConfig = tempCompLevelConfigList.Find((CompLevelConfig x) => x.CompID == client.ClientData.CompType && x.Level == (int)client.ClientData.CompZhiWu);
					if (null != levelConfig)
					{
						if (login)
						{
							CompManager.getInstance().UpdateBuff4GameClient(client, BufferItemTypes.CompBattle_Self, levelConfig.CraftSelfBuffID, true);
						}
						List<GameClient> objsList = scene.CopyMap.GetClientsList();
						if (objsList != null && objsList.Count > 0)
						{
							for (int i = 0; i < objsList.Count; i++)
							{
								GameClient c = objsList[i];
								if (c != null && c.ClientData.CompType == client.ClientData.CompType)
								{
									BufferItemTypes buffTypes = BufferItemTypes.CompBattle_Self + (int)client.ClientData.CompZhiWu;
									CompManager.getInstance().UpdateBuff4GameClient(c, buffTypes, levelConfig.CraftBuffID, login);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600089D RID: 2205 RVA: 0x00085F90 File Offset: 0x00084190
		public void OnStartPlayGame(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				CompBattleScene scene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					CompFuBenData fuBenData;
					if (this.RuntimeData.FuBenItemData.TryGetValue(scene.GameId, out fuBenData))
					{
						if (client.ClientData.CompZhiWu > 0)
						{
							CompBattleClientContextData clientContextData = client.SceneContextData2 as CompBattleClientContextData;
							clientContextData.CompZhiWu = (int)client.ClientData.CompZhiWu;
							clientContextData.Online = true;
							fuBenData.ZhuJiangRoleDict[client.ClientData.CompType].Add(client.ClientData.RoleID);
							CompBattleSideScore scoreData = scene.ScoreData.Find((CompBattleSideScore x) => x.CompType == client.ClientData.CompType);
							if (null != scoreData)
							{
								scoreData.ZhuJiangNum++;
							}
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<List<CompBattleSideScore>>(2005, scene.ScoreData, scene.CopyMap);
						}
						this.UpdateMapBuffer(scene, client, true);
						List<int> roleCountSideList;
						int index;
						(roleCountSideList = fuBenData.RoleCountSideList)[index = client.ClientData.CompType - 1] = roleCountSideList[index] + 1;
					}
				}
			}
		}

		// Token: 0x0600089E RID: 2206 RVA: 0x000861A0 File Offset: 0x000843A0
		public void LeaveFuBen(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				CompBattleScene scene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					CompFuBenData fuBenData;
					if (this.RuntimeData.FuBenItemData.TryGetValue(scene.GameId, out fuBenData))
					{
						if (client.ClientData.CompZhiWu > 0)
						{
							CompBattleClientContextData clientContextData = client.SceneContextData2 as CompBattleClientContextData;
							clientContextData.CompZhiWu = (int)client.ClientData.CompZhiWu;
							clientContextData.Online = false;
							bool exist = fuBenData.ZhuJiangRoleDict[client.ClientData.CompType].Remove(client.ClientData.RoleID);
							CompBattleSideScore scoreData = scene.ScoreData.Find((CompBattleSideScore x) => x.CompType == client.ClientData.CompType);
							if (exist && null != scoreData)
							{
								scoreData.ZhuJiangNum--;
							}
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<List<CompBattleSideScore>>(2005, scene.ScoreData, scene.CopyMap);
						}
						this.UpdateMapBuffer(scene, client, false);
						List<int> roleCountSideList;
						int index;
						(roleCountSideList = fuBenData.RoleCountSideList)[index = client.ClientData.CompType - 1] = roleCountSideList[index] - 1;
						TianTiClient.getInstance().Comp_GameFuBenRoleChangeState(30, client.ServerId, scene.SceneInfo.ID, client.ClientData.RoleID, (int)client.ClientData.CompZhiWu, 7);
					}
				}
			}
		}

		// Token: 0x0600089F RID: 2207 RVA: 0x000863CC File Offset: 0x000845CC
		public void OnKillRole(GameClient client, GameClient other)
		{
			lock (this.RuntimeData.Mutex)
			{
				CompBattleScene scene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					if (scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
					{
						int addScore = this.RuntimeData.CompBattleSingleIntegral[0];
						TianTiClient.getInstance().Comp_CompOpt(client.ClientData.CompType, 8, client.ClientData.RoleID, addScore);
						GameManager.ClientMgr.ModifyCompBattleJiFenValue(client, addScore, "势力战杀人", true, true, false);
						CompBattleClientContextData clientLianShaContextData = client.SceneContextData2 as CompBattleClientContextData;
						CompBattleClientContextData otherLianShaContextData = other.SceneContextData2 as CompBattleClientContextData;
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
							clientLianShaContextData.BattleJiFen += addScore;
							huanYingSiYuanAddScore.ByLianShaNum = 1;
							huanYingSiYuanLianSha = new HuanYingSiYuanLianSha();
							huanYingSiYuanLianSha.Name = huanYingSiYuanAddScore.Name;
							huanYingSiYuanLianSha.ZoneID = huanYingSiYuanAddScore.ZoneID;
							huanYingSiYuanLianSha.Occupation = huanYingSiYuanAddScore.Occupation;
							huanYingSiYuanLianSha.LianShaType = Math.Min(clientLianShaContextData.KillNum, 30) / 5;
							huanYingSiYuanLianSha.Side = huanYingSiYuanAddScore.Side;
							if (clientLianShaContextData.KillNum % 5 != 0)
							{
								huanYingSiYuanLianSha = null;
							}
						}
						if (null != otherLianShaContextData)
						{
							if (otherLianShaContextData.KillNum >= 10)
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
							}
							otherLianShaContextData.KillNum = 0;
						}
						if (null != huanYingSiYuanLianSha)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanLianSha>(2007, huanYingSiYuanLianSha, scene.CopyMap);
						}
						if (null != huanYingSiYuanLianshaOver)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanLianshaOver>(2008, huanYingSiYuanLianshaOver, scene.CopyMap);
						}
					}
				}
			}
		}

		// Token: 0x060008A0 RID: 2208 RVA: 0x000866F4 File Offset: 0x000848F4
		public void OnProcessMonsterDead(GameClient client, Monster monster)
		{
			CompBattleScene scene = client.SceneObject as CompBattleScene;
			CompStrongholdConfig tagInfo = monster.Tag as CompStrongholdConfig;
			if (tagInfo != null && null != scene)
			{
				lock (this.RuntimeData.Mutex)
				{
					CompBattleSideScore scoreData = scene.ScoreData.Find((CompBattleSideScore x) => x.CompType == tagInfo.BattleWhichSide);
					if (null != scoreData)
					{
						scoreData.StrongholdSet.Remove(tagInfo.ID);
					}
					tagInfo.DeadTicks = TimeUtil.NOW();
					tagInfo.Alive = false;
					tagInfo.BattleWhichSideLast = tagInfo.BattleWhichSide;
					tagInfo.BattleWhichSide = client.ClientData.BattleWhichSide;
					this.UpdateBattleSideScoreRank(scene, true);
				}
			}
		}

		// Token: 0x060008A1 RID: 2209 RVA: 0x00086820 File Offset: 0x00084A20
		public void OnInjureMonster(GameClient client, Monster monster, long injure)
		{
			CompStrongholdConfig tagInfo = monster.Tag as CompStrongholdConfig;
			if (null != tagInfo)
			{
				lock (this.RuntimeData.Mutex)
				{
					CompBattleScene scene = client.SceneObject as CompBattleScene;
					if (scene != null && scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
					{
						CompBattleClientContextData clientContextData = client.SceneContextData2 as CompBattleClientContextData;
						if (null != clientContextData)
						{
							clientContextData.BattleJiFen += tagInfo.Point;
							TianTiClient.getInstance().Comp_CompOpt(client.ClientData.CompType, 8, client.ClientData.RoleID, tagInfo.Point);
							GameManager.ClientMgr.ModifyCompBattleJiFenValue(client, tagInfo.Point, "势力战打旗", true, true, false);
						}
					}
				}
			}
		}

		// Token: 0x060008A2 RID: 2210 RVA: 0x00086920 File Offset: 0x00084B20
		public void CompleteScene(CompBattleScene scene, int successSide)
		{
			scene.SuccessSide = successSide;
		}

		// Token: 0x060008A3 RID: 2211 RVA: 0x0008692C File Offset: 0x00084B2C
		private void ProcessEnd(CompBattleScene scene, long nowTicks)
		{
			int successSide = (scene.ScoreData[0].Rate > 0.0) ? scene.ScoreData[0].CompType : 0;
			this.CompleteScene(scene, successSide);
			scene.m_eStatus = GameSceneStatuses.STATUS_END;
			scene.m_lLeaveTime = nowTicks + (long)(scene.SceneInfo.ClearRolesSecs * 1000);
			scene.StateTimeData.GameType = 30;
			scene.StateTimeData.State = 5;
			scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
			GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
		}

		// Token: 0x060008A4 RID: 2212 RVA: 0x000869DC File Offset: 0x00084BDC
		public void GiveAwards(CompBattleScene scene)
		{
			try
			{
				foreach (CompBattleClientContextData contextData in scene.ClientContextDataDict.Values)
				{
					if (contextData.BattleJiFen > 0)
					{
						GameClient client = GameManager.ClientMgr.FindClient(contextData.RoleId);
						string awardsInfo = string.Format("{0}", scene.StartTimeTicks);
						if (client != null)
						{
							Global.SaveRoleParamsStringToDB(client, "49", awardsInfo, true);
						}
						else
						{
							Global.UpdateRoleParamByNameOffline(contextData.RoleId, "49", awardsInfo, contextData.ServerId);
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "势力战场系统奖励异常");
			}
		}

		// Token: 0x04000EFF RID: 3839
		public const SceneUIClasses ManagerType = SceneUIClasses.CompBattle;

		// Token: 0x04000F00 RID: 3840
		private static CompBattleManager instance = new CompBattleManager();

		// Token: 0x04000F01 RID: 3841
		public CompBattleRuntimeData RuntimeData = new CompBattleRuntimeData();

		// Token: 0x04000F02 RID: 3842
		public ConcurrentDictionary<int, CompBattleScene> SceneDict = new ConcurrentDictionary<int, CompBattleScene>();

		// Token: 0x04000F03 RID: 3843
		private static long NextHeartBeatTicks = 0L;
	}
}
