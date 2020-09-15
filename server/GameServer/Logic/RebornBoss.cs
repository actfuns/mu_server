using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.Reborn;
using GameServer.Server;
using KF.Client;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	// Token: 0x020003DA RID: 986
	public class RebornBoss : IManager, IEventListener, IEventListenerEx, ICmdProcessorEx, ICmdProcessor
	{
		// Token: 0x06001116 RID: 4374 RVA: 0x0010C250 File Offset: 0x0010A450
		public static RebornBoss getInstance()
		{
			return RebornBoss.instance;
		}

		// Token: 0x06001117 RID: 4375 RVA: 0x0010C268 File Offset: 0x0010A468
		public bool initialize()
		{
			return this.InitConfig();
		}

		// Token: 0x06001118 RID: 4376 RVA: 0x0010C28C File Offset: 0x0010A48C
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1715, 2, 2, RebornBoss.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1717, 4, 4, RebornBoss.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(30, 54, RebornBoss.getInstance());
			GlobalEventSource.getInstance().registerListener(28, RebornBoss.getInstance());
			GlobalEventSource.getInstance().registerListener(11, RebornBoss.getInstance());
			return true;
		}

		// Token: 0x06001119 RID: 4377 RVA: 0x0010C308 File Offset: 0x0010A508
		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(30, 54, RebornBoss.getInstance());
			GlobalEventSource.getInstance().removeListener(28, RebornBoss.getInstance());
			GlobalEventSource.getInstance().removeListener(11, RebornBoss.getInstance());
			return true;
		}

		// Token: 0x0600111A RID: 4378 RVA: 0x0010C354 File Offset: 0x0010A554
		public bool destroy()
		{
			return true;
		}

		// Token: 0x0600111B RID: 4379 RVA: 0x0010C368 File Offset: 0x0010A568
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x0600111C RID: 4380 RVA: 0x0010C37C File Offset: 0x0010A57C
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			switch (nID)
			{
			case 1715:
				return this.ProcessRebornBossDataCmd(client, nID, bytes, cmdParams);
			case 1717:
				return this.ProcessRebornBossGetAwardCmd(client, nID, bytes, cmdParams);
			}
			return true;
		}

		// Token: 0x0600111D RID: 4381 RVA: 0x0010C3C8 File Offset: 0x0010A5C8
		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 11)
			{
				MonsterDeadEventObject e = eventObject as MonsterDeadEventObject;
				this.OnProcessMonsterDead(e.getAttacker(), e.getMonster());
			}
			else if (eventType == 28)
			{
				OnStartPlayGameEventObject e2 = eventObject as OnStartPlayGameEventObject;
				this.OnStartPlayGame(e2.Client);
			}
		}

		// Token: 0x0600111E RID: 4382 RVA: 0x0010C42C File Offset: 0x0010A62C
		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			int num = eventType;
			if (num == 30)
			{
				OnCreateMonsterEventObject e = eventObject as OnCreateMonsterEventObject;
				if (null != e)
				{
					RebornBossConfig bossConfig = e.Monster.Tag as RebornBossConfig;
					if (null != bossConfig)
					{
						RebornBossScene scene;
						if (this.SceneDict.TryGetValue(e.Monster.CurrentMapCode, out scene))
						{
							lock (RebornBoss.Mutex)
							{
								scene.scoreData.MonsterID = e.Monster.RoleID;
								scene.scoreData.VLife = e.Monster.VLife;
								scene.scoreData.VLifeMax = e.Monster.MonsterInfo.VLifeMax;
								this.BroadCastScoreInfo(scene);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600111F RID: 4383 RVA: 0x0010C5C0 File Offset: 0x0010A7C0
		public bool ProcessRebornBossDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = Convert.ToInt32(cmdParams[0]);
				int mapCodeID = Convert.ToInt32(cmdParams[1]);
				Dictionary<KeyValuePair<int, int>, KFRebornBossRefreshData> tempRefreshData = null;
				lock (RebornManager.getInstance().RebornSyncDataCache)
				{
					if (null != RebornManager.getInstance().RebornSyncDataCache.BossRefreshDict)
					{
						tempRefreshData = RebornManager.getInstance().RebornSyncDataCache.BossRefreshDict.V;
					}
				}
				List<RebornBossData> bossDataList = new List<RebornBossData>();
				KFRebornRoleData rebornRoleData = KuaFuWorldClient.getInstance().Reborn_GetRebornRoleData(client.ClientData.ServerPTID, client.ClientData.LocalRoleID);
				if (null == rebornRoleData)
				{
					client.sendCmd<List<RebornBossData>>(nID, bossDataList, false);
					return true;
				}
				List<KuaFuLineData> list = KuaFuWorldClient.getInstance().GetKuaFuLineDataList(mapCodeID) as List<KuaFuLineData>;
				if (list != null && list.Count > 0)
				{
					using (List<KuaFuLineData>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KuaFuLineData item = enumerator.Current;
							RebornBossData bossData = new RebornBossData();
							KFRebornBossRefreshData refreshData;
							if (tempRefreshData != null && tempRefreshData.TryGetValue(new KeyValuePair<int, int>(mapCodeID, item.Line), out refreshData))
							{
								bossData.ExtensionID = refreshData.ExtensionID;
								bossData.NextTime = refreshData.NextTime;
							}
							List<KFRebornBossAwardData> bossKillAwardList = this.GetRebornBossKillAwardList(client);
							KFRebornBossAwardData bossKillAwardData = bossKillAwardList.Find((KFRebornBossAwardData x) => x.MapCodeID == mapCodeID && x.LineID == item.Line);
							KFRebornBossAwardData bossAwardData = rebornRoleData.BossAwardList.Find((KFRebornBossAwardData x) => x.MapCodeID == mapCodeID && x.LineID == item.Line);
							if (null != bossAwardData)
							{
								bossData.AwardExtensionID = bossAwardData.ExtensionID;
								bossData.RankNum = bossAwardData.RankNum;
							}
							if (null != bossKillAwardData)
							{
								if (null != bossAwardData)
								{
									if (bossKillAwardData.ExtensionID == bossAwardData.ExtensionID)
									{
										bossData.BossKill = 1;
									}
								}
								else
								{
									bossData.AwardExtensionID = bossKillAwardData.ExtensionID;
									bossData.BossKill = 1;
								}
							}
							bossDataList.Add(bossData);
						}
					}
				}
				client.sendCmd<List<RebornBossData>>(nID, bossDataList, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06001120 RID: 4384 RVA: 0x0010C8EC File Offset: 0x0010AAEC
		public RebornBossAwardConfig GetKillAwardConfig(int ExtensionID)
		{
			RebornBossAwardConfig awardConfig = null;
			Dictionary<int, List<RebornBossAwardConfig>> tempRebornBossAwardConfigDict;
			lock (RebornBoss.Mutex)
			{
				tempRebornBossAwardConfigDict = this.RebornBossAwardConfigDict;
			}
			List<RebornBossAwardConfig> awardConfigList;
			if (tempRebornBossAwardConfigDict.TryGetValue(ExtensionID, out awardConfigList))
			{
				awardConfig = awardConfigList.Find((RebornBossAwardConfig x) => x.AwardType == 1);
			}
			return awardConfig;
		}

		// Token: 0x06001121 RID: 4385 RVA: 0x0010C980 File Offset: 0x0010AB80
		public RebornBossAwardConfig GetRankAwardConfig(int ExtensionID, int RankNum)
		{
			RebornBossAwardConfig awardConfig = null;
			Dictionary<int, List<RebornBossAwardConfig>> tempRebornBossAwardConfigDict;
			lock (RebornBoss.Mutex)
			{
				tempRebornBossAwardConfigDict = this.RebornBossAwardConfigDict;
			}
			List<RebornBossAwardConfig> awardConfigList;
			RebornBossAwardConfig result;
			if (!tempRebornBossAwardConfigDict.TryGetValue(ExtensionID, out awardConfigList))
			{
				result = awardConfig;
			}
			else
			{
				foreach (RebornBossAwardConfig item in awardConfigList)
				{
					if (item.AwardType <= 0)
					{
						if (item.BeginNum <= 0 || RankNum <= 0 || RankNum >= item.BeginNum)
						{
							if (item.EndNum <= 0 || RankNum <= item.EndNum)
							{
								awardConfig = item;
								break;
							}
						}
					}
				}
				result = awardConfig;
			}
			return result;
		}

		// Token: 0x06001122 RID: 4386 RVA: 0x0010CB44 File Offset: 0x0010AD44
		public bool ProcessRebornBossGetAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int roleID = Convert.ToInt32(cmdParams[0]);
				int mapCodeID = Convert.ToInt32(cmdParams[1]);
				int lineID = Convert.ToInt32(cmdParams[2]);
				int ExtensionID = Convert.ToInt32(cmdParams[3]);
				KFRebornRoleData rebornRoleData = KuaFuWorldClient.getInstance().Reborn_GetRebornRoleData(client.ClientData.ServerPTID, client.ClientData.LocalRoleID);
				if (null == rebornRoleData)
				{
					result = -12;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						result,
						mapCodeID,
						lineID,
						ExtensionID
					}), false);
					return true;
				}
				List<KFRebornBossAwardData> bossKillAwardList = this.GetRebornBossKillAwardList(client);
				KFRebornBossAwardData bossKillAwardData = bossKillAwardList.Find((KFRebornBossAwardData x) => x.MapCodeID == mapCodeID && x.LineID == lineID && x.ExtensionID == ExtensionID);
				KFRebornBossAwardData bossRankAwardData = rebornRoleData.BossAwardList.Find((KFRebornBossAwardData x) => x.MapCodeID == mapCodeID && x.LineID == lineID && x.ExtensionID == ExtensionID);
				if (bossKillAwardData == null && null == bossRankAwardData)
				{
					result = -12;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						result,
						mapCodeID,
						lineID,
						ExtensionID
					}), false);
					return true;
				}
				RebornBossAwardConfig awardKillConfig = null;
				RebornBossAwardConfig awardRankConfig = null;
				if (null != bossKillAwardData)
				{
					awardKillConfig = this.GetKillAwardConfig(bossKillAwardData.ExtensionID);
				}
				if (null != bossRankAwardData)
				{
					awardRankConfig = this.GetRankAwardConfig(bossRankAwardData.ExtensionID, bossRankAwardData.RankNum);
				}
				if (awardKillConfig == null && null == awardRankConfig)
				{
					result = -3;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						result,
						mapCodeID,
						lineID,
						ExtensionID
					}), false);
					return true;
				}
				List<AwardsItemData> awardsItemDataListOne = new List<AwardsItemData>();
				List<AwardsItemData> awardsItemDataListTwo = new List<AwardsItemData>();
				if (null != awardKillConfig)
				{
					awardsItemDataListOne.AddRange(awardKillConfig.AwardsItemListOne.Items);
					awardsItemDataListTwo.AddRange(awardKillConfig.AwardsItemListTwo.Items);
				}
				if (null != awardRankConfig)
				{
					awardsItemDataListOne.AddRange(awardRankConfig.AwardsItemListOne.Items);
					awardsItemDataListTwo.AddRange(awardRankConfig.AwardsItemListTwo.Items);
				}
				int awardCnt = 0;
				if (awardsItemDataListOne != null)
				{
					awardCnt += awardsItemDataListOne.Count;
				}
				if (awardsItemDataListTwo != null)
				{
					awardCnt += awardsItemDataListTwo.Count((AwardsItemData goods) => Global.IsRoleOccupationMatchGoods(client, goods.GoodsID));
				}
				int BagInt;
				if (!RebornEquip.MoreIsCanIntoRebornOrBaseBagAward(client, awardsItemDataListOne, out BagInt))
				{
					if (BagInt == 1)
					{
						result = -101;
					}
					else
					{
						result = -100;
					}
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						result,
						mapCodeID,
						lineID,
						ExtensionID
					}), false);
					return true;
				}
				if (null != awardKillConfig)
				{
					this.GiveBossAward(client, awardKillConfig, "重生Boss最后一击奖励");
					bossKillAwardList.Remove(bossKillAwardData);
					this.SaveRebornBossKillAwardList(client, bossKillAwardList);
				}
				if (null != awardRankConfig)
				{
					this.GiveBossAward(client, awardRankConfig, "重生Boss奖励");
					string param3 = string.Format("{0}", ExtensionID);
					KuaFuWorldClient.getInstance().Reborn_RebornOpt(client.ClientData.ServerPTID, client.ClientData.LocalRoleID, 5, mapCodeID, lineID, param3);
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					result,
					mapCodeID,
					lineID,
					ExtensionID
				}), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06001123 RID: 4387 RVA: 0x0010D088 File Offset: 0x0010B288
		public bool InitConfig()
		{
			bool result;
			if (!this.LoadRebornBossConfigFile())
			{
				result = false;
			}
			else if (!this.LoadRebornBossAwardConfigFile())
			{
				result = false;
			}
			else
			{
				this.RebornBossRankClearSec = (int)GameManager.systemParamsList.GetParamValueIntByName("RebornBossResumeTime", 120);
				result = true;
			}
			return result;
		}

		// Token: 0x06001124 RID: 4388 RVA: 0x0010D150 File Offset: 0x0010B350
		public bool LoadRebornBossConfigFile()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(RebornDataConst.RebornBoss));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(RebornDataConst.RebornBoss));
				if (null == xml)
				{
					return false;
				}
				Dictionary<int, List<RebornBossConfig>> tempRebornBossConfigDict = new Dictionary<int, List<RebornBossConfig>>();
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					int MapCodeID = (int)Global.GetSafeAttributeLong(xmlItem, "MapID");
					List<RebornBossConfig> bossList;
					if (!tempRebornBossConfigDict.TryGetValue(MapCodeID, out bossList))
					{
						bossList = new List<RebornBossConfig>();
						tempRebornBossConfigDict[MapCodeID] = bossList;
					}
					RebornBossConfig myData = new RebornBossConfig();
					myData.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
					myData.MapID = (int)Global.GetSafeAttributeLong(xmlItem, "MapID");
					myData.MonstersID = (int)Global.GetSafeAttributeLong(xmlItem, "MonstersID");
					myData.RebornLevel = (int)Global.GetSafeAttributeLong(xmlItem, "RebornLevel");
					myData.Site = Global.GetSafeAttributeIntArray(xmlItem, "Site", -1, '|');
					myData.Radius = (int)Global.GetSafeAttributeLong(xmlItem, "Radius");
					myData.PursuitRadius = (int)Global.GetSafeAttributeLong(xmlItem, "PursuitRadius");
					string str = Global.GetSafeAttributeStr(xmlItem, "Time");
					string[] rangeStr = str.Split(new char[]
					{
						'|'
					});
					foreach (string rangeItem in rangeStr)
					{
						TimeSpan time;
						if (TimeSpan.TryParse(rangeItem, out time))
						{
							myData.RefreshTimePoints.Add(time);
						}
					}
					myData.RefreshTimePoints.Sort(delegate(TimeSpan left, TimeSpan right)
					{
						int result;
						if (left < right)
						{
							result = -1;
						}
						else if (left > right)
						{
							result = 1;
						}
						else
						{
							result = 0;
						}
						return result;
					});
					if (!ConfigParser.ParserTimeRangeList(myData.TimePoints, Global.GetSafeAttributeStr(xmlItem, "EffectiveTime"), true, '|', '-'))
					{
						LogManager.WriteLog(LogTypes.Fatal, string.Format("读取{0}时间配置(TimePoints)出错", RebornDataConst.RebornBoss), null, true);
					}
					bossList.Add(myData);
				}
				foreach (List<RebornBossConfig> value in tempRebornBossConfigDict.Values)
				{
					value.Sort(delegate(RebornBossConfig left, RebornBossConfig right)
					{
						int result;
						if (left.ID < right.ID)
						{
							result = -1;
						}
						else if (left.ID > right.ID)
						{
							result = 1;
						}
						else
						{
							result = 0;
						}
						return result;
					});
				}
				lock (RebornBoss.Mutex)
				{
					this.RebornBossConfigDict = tempRebornBossConfigDict;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", RebornDataConst.RebornBoss, ex.Message), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x06001125 RID: 4389 RVA: 0x0010D4B4 File Offset: 0x0010B6B4
		public bool LoadRebornBossAwardConfigFile()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(RebornDataConst.RebornBossAward));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(RebornDataConst.RebornBossAward));
				if (null == xml)
				{
					return false;
				}
				Dictionary<int, List<RebornBossAwardConfig>> tempRebornBossAwardConfigDict = new Dictionary<int, List<RebornBossAwardConfig>>();
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					RebornBossAwardConfig myData = new RebornBossAwardConfig();
					myData.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
					myData.MonstersID = (int)Global.GetSafeAttributeLong(xmlItem, "MonstersID");
					myData.BeginNum = (int)Global.GetSafeAttributeLong(xmlItem, "BeginNum");
					myData.EndNum = (int)Global.GetSafeAttributeLong(xmlItem, "EndNum");
					myData.AwardType = (int)Global.GetSafeAttributeLong(xmlItem, "Type");
					ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xmlItem, "GoodsOne"), ref myData.AwardsItemListOne, '|', ',');
					ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xmlItem, "GoodsTwo"), ref myData.AwardsItemListTwo, '|', ',');
					List<RebornBossAwardConfig> bossList;
					if (!tempRebornBossAwardConfigDict.TryGetValue(myData.MonstersID, out bossList))
					{
						bossList = new List<RebornBossAwardConfig>();
						tempRebornBossAwardConfigDict[myData.MonstersID] = bossList;
					}
					bossList.Add(myData);
				}
				lock (RebornBoss.Mutex)
				{
					this.RebornBossAwardConfigDict = tempRebornBossAwardConfigDict;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", RebornDataConst.RebornBossAward, ex.Message), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x06001126 RID: 4390 RVA: 0x0010D6D0 File Offset: 0x0010B8D0
		private void OnStartPlayGame(GameClient client)
		{
			if (GameManager.IsKuaFuServer)
			{
				RebornBossScene scene;
				if (this.SceneDict.TryGetValue(client.ClientData.MapCode, out scene))
				{
					this.NotifyScoreInfo(client);
				}
			}
		}

		// Token: 0x06001127 RID: 4391 RVA: 0x0010D710 File Offset: 0x0010B910
		public void AddDelayCreateMonster(RebornBossScene scene, long ticks, object monster)
		{
			lock (RebornBoss.Mutex)
			{
				List<object> list = null;
				if (!scene.CreateMonsterQueue.TryGetValue(ticks, out list))
				{
					list = new List<object>();
					scene.CreateMonsterQueue.Add(ticks, list);
				}
				list.Add(monster);
			}
		}

		// Token: 0x06001128 RID: 4392 RVA: 0x0010D788 File Offset: 0x0010B988
		public void CheckCreateDynamicMonster(RebornBossScene scene, DateTime now)
		{
			long nowMs = now.Ticks / 10000L;
			while (scene.CreateMonsterQueue.Count > 0)
			{
				KeyValuePair<long, List<object>> pair = scene.CreateMonsterQueue.First<KeyValuePair<long, List<object>>>();
				if (nowMs < pair.Key)
				{
					break;
				}
				try
				{
					foreach (object obj in pair.Value)
					{
						if (obj is RebornBossConfig)
						{
							RebornBossConfig item = obj as RebornBossConfig;
							GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, item.MonstersID, -1, 1, item.Site[0] / 100, item.Site[1] / 100, item.Radius / 100, item.PursuitRadius, SceneUIClasses.ChongShengMap, item, null);
						}
					}
				}
				finally
				{
					scene.CreateMonsterQueue.RemoveAt(0);
				}
			}
		}

		// Token: 0x06001129 RID: 4393 RVA: 0x0010D928 File Offset: 0x0010BB28
		public void ResortBossAttackRank(RebornBossScene scene)
		{
			scene.BossRankList.Sort(delegate(RebornBossAttackLog left, RebornBossAttackLog right)
			{
				int result;
				if (left.InjureSum > right.InjureSum)
				{
					result = -1;
				}
				else if (left.InjureSum < right.InjureSum)
				{
					result = 1;
				}
				else if (left.RoleID > right.RoleID)
				{
					result = -1;
				}
				else if (left.RoleID < right.RoleID)
				{
					result = 1;
				}
				else
				{
					result = 0;
				}
				return result;
			});
			for (int loop = 0; loop < scene.BossRankList.Count; loop++)
			{
				int RankNumBefore = scene.BossRankList[loop].RankNum;
				if (loop >= RebornDataConst.RebornBossRankCal)
				{
					scene.BossRankList[loop].RankNum = 0;
				}
				else
				{
					scene.BossRankList[loop].RankNum = loop + 1;
				}
				scene.BossRankList[loop].NotifySelf |= (scene.BossRankList[loop].RankNum != RankNumBefore);
			}
		}

		// Token: 0x0600112A RID: 4394 RVA: 0x0010DA00 File Offset: 0x0010BC00
		public void OnInjureMonster(GameClient client, Monster monster, long injure)
		{
			if (401 == monster.MonsterType && monster.Tag is RebornBossConfig)
			{
				RebornBossScene scene;
				if (this.SceneDict.TryGetValue(client.ClientData.MapCode, out scene))
				{
					bool notifyAll = false;
					bool notifySelf = false;
					bool resort = false;
					lock (RebornBoss.Mutex)
					{
						scene.scoreData.VLife = monster.VLife;
						scene.scoreData.BossBeAttackTm = TimeUtil.NowDateTime();
						RebornBossAttackLog attackLog;
						if (!scene.BossRankDict.TryGetValue(client.ClientData.RoleID, out attackLog))
						{
							attackLog = new RebornBossAttackLog();
							attackLog.UserPtID = client.ClientData.UserPTID;
							attackLog.ServerPtID = client.ClientData.ServerPTID;
							attackLog.RoleID = client.ClientData.RoleID;
							attackLog.Param = client.ClientData.Channel;
							attackLog.RoleName = Global.FormatRoleNameWithZoneId2(client);
							attackLog.ServerID = client.ServerId;
							attackLog.LocalRoleID = client.ClientData.LocalRoleID;
							scene.BossRankDict[attackLog.RoleID] = attackLog;
							scene.BossRankList.Add(attackLog);
							resort = true;
						}
						attackLog.InjureSum += injure;
						int newPct = (int)((double)attackLog.InjureSum / scene.scoreData.VLifeMax * 100.0);
						if (attackLog.DamagePct != newPct)
						{
							resort = true;
							notifySelf = true;
						}
						attackLog.DamagePct = newPct;
						if (!resort)
						{
							RebornBossAttackLog clientLowest = scene.BossRankList[Math.Min(RebornDataConst.RebornBossRankShow, scene.BossRankList.Count) - 1];
							if (clientLowest.RoleID == attackLog.RoleID || attackLog.InjureSum > clientLowest.InjureSum)
							{
								resort = true;
							}
						}
						if (resort)
						{
							this.ResortBossAttackRank(scene);
						}
						if (attackLog.RankNum > 0 && attackLog.RankNum <= RebornDataConst.RebornBossRankShow)
						{
							notifyAll = true;
						}
						if (notifySelf || attackLog.NotifySelf)
						{
							this.NotifyScoreInfo(client);
						}
						if (notifyAll)
						{
							this.BroadCastScoreInfo(scene);
						}
					}
				}
			}
		}

		// Token: 0x0600112B RID: 4395 RVA: 0x0010DCA8 File Offset: 0x0010BEA8
		public void GiveBossAward(GameClient client, RebornBossAwardConfig awardConfig, string strFrom)
		{
			List<AwardsItemData> awardsItemDataListOne = awardConfig.AwardsItemListOne.Items;
			List<AwardsItemData> awardsItemDataListTwo = awardConfig.AwardsItemListTwo.Items;
			if (awardsItemDataListOne != null)
			{
				foreach (AwardsItemData item in awardsItemDataListOne)
				{
					if (RebornEquip.IsRebornType(item.GoodsID))
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, item.GoodsNum, 0, "", item.Level, item.Binding, 15000, "", true, 1, strFrom, "1900-01-01 12:00:00", 0, 0, item.IsHaveLuckyProp, 0, item.ExcellencePorpValue, item.AppendLev, 0, null, null, 0, true);
					}
					else
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, item.GoodsNum, 0, "", item.Level, item.Binding, 0, "", true, 1, strFrom, "1900-01-01 12:00:00", 0, 0, item.IsHaveLuckyProp, 0, item.ExcellencePorpValue, item.AppendLev, 0, null, null, 0, true);
					}
				}
			}
			if (awardsItemDataListTwo != null)
			{
				foreach (AwardsItemData item in awardsItemDataListTwo)
				{
					if (Global.IsCanGiveRewardByOccupation(client, item.GoodsID))
					{
						if (RebornEquip.IsRebornType(item.GoodsID))
						{
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, item.GoodsNum, 0, "", item.Level, item.Binding, 15000, "", true, 1, strFrom, "1900-01-01 12:00:00", 0, 0, item.IsHaveLuckyProp, 0, item.ExcellencePorpValue, item.AppendLev, 0, null, null, 0, true);
						}
						else
						{
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, item.GoodsNum, 0, "", item.Level, item.Binding, 0, "", true, 1, strFrom, "1900-01-01 12:00:00", 0, 0, item.IsHaveLuckyProp, 0, item.ExcellencePorpValue, item.AppendLev, 0, null, null, 0, true);
						}
					}
				}
			}
		}

		// Token: 0x0600112C RID: 4396 RVA: 0x0010DF38 File Offset: 0x0010C138
		public void OnProcessMonsterDead(GameClient client, Monster monster)
		{
			if (401 == monster.MonsterType)
			{
				RebornBossScene scene;
				if (this.SceneDict.TryGetValue(client.ClientData.MapCode, out scene))
				{
					lock (RebornBoss.Mutex)
					{
						Dictionary<int, List<RebornBossAwardConfig>> tempRebornBossAwardConfigDict = this.RebornBossAwardConfigDict;
					}
					RebornBossAwardConfig awardConfig = this.GetKillAwardConfig(monster.MonsterInfo.ExtensionID);
					if (null != awardConfig)
					{
						List<KFRebornBossAwardData> awardlist = this.GetRebornBossKillAwardList(client);
						KFRebornBossAwardData awarditem = new KFRebornBossAwardData
						{
							MapCodeID = scene.m_nMapCode,
							LineID = scene.m_nLineID,
							ExtensionID = monster.MonsterInfo.ExtensionID
						};
						awardlist.Add(awarditem);
						this.SaveRebornBossKillAwardList(client, awardlist);
					}
					lock (RebornBoss.Mutex)
					{
						scene.BossState = RebornBossState.RBS_Dead;
						this.ResortBossAttackRank(scene);
						for (int loop = 0; loop < scene.BossRankList.Count; loop++)
						{
							RebornBossAttackLog log = scene.BossRankList[loop];
							string param3 = string.Format("{0},{1}", monster.MonsterInfo.ExtensionID, log.RankNum);
							KuaFuWorldClient.getInstance().Reborn_RebornOpt(log.ServerPtID, log.LocalRoleID, 4, scene.m_nMapCode, scene.m_nLineID, param3);
						}
					}
					this.PrintBossInfoGM(client, 10, monster);
				}
			}
		}

		// Token: 0x0600112D RID: 4397 RVA: 0x0010E140 File Offset: 0x0010C340
		public RebornBossConfig GetBossConfigByExtensionID(RebornBossScene scene, bool peekNext = false)
		{
			RebornBossConfig config = null;
			Dictionary<int, List<RebornBossConfig>> tempRebornBossConfigDict = null;
			lock (RebornBoss.Mutex)
			{
				tempRebornBossConfigDict = this.RebornBossConfigDict;
			}
			List<RebornBossConfig> bossConfigList;
			RebornBossConfig result;
			if (!tempRebornBossConfigDict.TryGetValue(scene.m_nMapCode, out bossConfigList) || bossConfigList.Count == 0)
			{
				result = config;
			}
			else
			{
				int BossRefreshIdx = bossConfigList.FindIndex((RebornBossConfig x) => x.MonstersID == scene.scoreData.BossExtensionID);
				if (peekNext)
				{
					BossRefreshIdx++;
				}
				if (BossRefreshIdx >= 0 && BossRefreshIdx < bossConfigList.Count)
				{
					config = bossConfigList[BossRefreshIdx];
				}
				else
				{
					BossRefreshIdx = 0;
					config = bossConfigList[BossRefreshIdx];
				}
				result = config;
			}
			return result;
		}

		// Token: 0x0600112E RID: 4398 RVA: 0x0010E25C File Offset: 0x0010C45C
		public DateTime CalBossRefreshTime(RebornBossScene scene, RebornBossConfig config, DateTime now)
		{
			DateTime refreshTm = DateTime.MinValue;
			if (now.TimeOfDay < config.RefreshTimePoints[0])
			{
				refreshTm = new DateTime(now.Year, now.Month, now.Day).Add(config.RefreshTimePoints[0]);
			}
			else if (now.TimeOfDay >= config.RefreshTimePoints[config.RefreshTimePoints.Count - 1])
			{
				refreshTm = new DateTime(now.Year, now.Month, now.Day).AddDays(1.0).Add(config.RefreshTimePoints[0]);
			}
			else
			{
				TimeSpan refreshSpan = config.RefreshTimePoints.Find((TimeSpan x) => now.TimeOfDay < x);
				refreshTm = new DateTime(now.Year, now.Month, now.Day).Add(refreshSpan);
			}
			return refreshTm;
		}

		// Token: 0x0600112F RID: 4399 RVA: 0x0010E3C4 File Offset: 0x0010C5C4
		public void CheckCreateBossState(RebornBossScene scene, DateTime now)
		{
			long nowTicks = now.Ticks / 10000L;
			lock (RebornBoss.Mutex)
			{
				if (Math.Abs(nowTicks - RebornBoss.LastHeartBeatTicks_Boss) < 3000L)
				{
					return;
				}
				RebornBoss.LastHeartBeatTicks_Boss = nowTicks;
			}
			string param3;
			if (now >= scene.scoreData.BossRefreshTime)
			{
				param3 = string.Format("{0},{1}", scene.scoreData.BossExtensionID, "");
			}
			else
			{
				param3 = string.Format("{0},{1}", scene.scoreData.BossExtensionID, scene.scoreData.BossRefreshTime.ToString("yyyy-MM-dd HH:mm:ss"));
			}
			KuaFuWorldClient.getInstance().Reborn_RebornOpt(-1, -1, 6, scene.m_nMapCode, scene.m_nLineID, param3);
		}

		// Token: 0x06001130 RID: 4400 RVA: 0x0010E4D4 File Offset: 0x0010C6D4
		public void CheckCreateBoss(RebornBossScene scene, DateTime now)
		{
			long nowMs = now.Ticks / 10000L;
			if (!(now < scene.scoreData.BossRefreshTime))
			{
				bool notifyAll = false;
				if (scene.BossState == RebornBossState.RBS_None)
				{
					RebornBossConfig bossConfig = this.GetBossConfigByExtensionID(scene, false);
					if (null != bossConfig)
					{
						scene.BossRankDict.Clear();
						scene.BossRankList.Clear();
						scene.BossState = RebornBossState.RBS_Init;
						scene.scoreData.BossExtensionID = bossConfig.MonstersID;
						scene.scoreData.BossRefreshTime = now;
						scene.scoreData.BossBeAttackTm = now;
						scene.SaveSceneDBInfo();
						GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, bossConfig.MonstersID, -1, 1, bossConfig.Site[0] / 100, bossConfig.Site[1] / 100, bossConfig.Radius / 100, bossConfig.PursuitRadius, SceneUIClasses.ChongShengMap, bossConfig, null);
						notifyAll = true;
					}
				}
				else if (scene.BossState == RebornBossState.RBS_Init)
				{
					if ((now - scene.scoreData.BossBeAttackTm).TotalSeconds > (double)this.RebornBossRankClearSec)
					{
						scene.BossRankList.Clear();
						scene.BossRankDict.Clear();
						scene.scoreData.BossBeAttackTm = now;
						Monster BossObj = GameManager.MonsterMgr.FindMonster(scene.m_nMapCode, scene.scoreData.MonsterID);
						if (null != BossObj)
						{
							GameManager.MonsterMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, BossObj, scene.scoreData.VLifeMax);
							scene.scoreData.VLife = BossObj.VLife;
						}
						notifyAll = true;
					}
				}
				else if (scene.BossState == RebornBossState.RBS_Dead)
				{
					RebornBossConfig bossConfig = this.GetBossConfigByExtensionID(scene, true);
					if (null != bossConfig)
					{
						scene.BossState = RebornBossState.RBS_None;
						scene.scoreData.BossExtensionID = bossConfig.MonstersID;
						scene.scoreData.BossRefreshTime = this.CalBossRefreshTime(scene, bossConfig, now);
						scene.SaveSceneDBInfo();
						notifyAll = true;
					}
				}
				if (notifyAll)
				{
					this.BroadCastScoreInfo(scene);
				}
			}
		}

		// Token: 0x06001131 RID: 4401 RVA: 0x0010E720 File Offset: 0x0010C920
		public void NotifyScoreInfo(GameClient client)
		{
			lock (RebornBoss.Mutex)
			{
				RebornBossScene scene;
				if (this.SceneDict.TryGetValue(client.ClientData.MapCode, out scene))
				{
					if (scene.scoreData.VLife > 0.0 && scene.scoreData.VLifeMax > 0.0)
					{
						scene.scoreData.LeftLifePct = (int)(scene.scoreData.VLife / scene.scoreData.VLifeMax * 100.0);
						scene.scoreData.NextTime = "";
					}
					else
					{
						scene.scoreData.LeftLifePct = 0;
						scene.scoreData.NextTime = scene.scoreData.BossRefreshTime.ToString("yyyy-MM-dd HH:mm:ss");
					}
					scene.scoreData.SelfRankNum = 0;
					scene.scoreData.SelfDamagePct = 0;
					RebornBossAttackLog attackLog;
					if (scene.BossRankDict.TryGetValue(client.ClientData.RoleID, out attackLog))
					{
						scene.scoreData.SelfRankNum = attackLog.RankNum;
						scene.scoreData.SelfDamagePct = attackLog.DamagePct;
						attackLog.NotifySelf = false;
					}
					scene.scoreData.rankList = scene.BossRankList.GetRange(0, Math.Min(scene.BossRankList.Count, RebornDataConst.RebornBossRankShow));
					client.sendCmd<RebornBossScoreData>(1716, scene.scoreData, false);
				}
			}
		}

		// Token: 0x06001132 RID: 4402 RVA: 0x0010E8E4 File Offset: 0x0010CAE4
		public void BroadCastScoreInfo(RebornBossScene scene)
		{
			List<object> objsList = GameManager.ClientMgr.GetMapClients(scene.m_nMapCode);
			if (objsList != null && objsList.Count != 0)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					GameClient c = objsList[i] as GameClient;
					if (c != null)
					{
						this.NotifyScoreInfo(c);
					}
				}
			}
		}

		// Token: 0x06001133 RID: 4403 RVA: 0x0010E954 File Offset: 0x0010CB54
		public void BuildFakeBossInfoGM(GameClient client, int fakeNum)
		{
			lock (RebornBoss.Mutex)
			{
				RebornBossScene scene;
				if (this.SceneDict.TryGetValue(client.ClientData.MapCode, out scene))
				{
					for (int i = 0; i < fakeNum; i++)
					{
						RebornBossAttackLog attackLog = new RebornBossAttackLog();
						do
						{
							attackLog.RoleID = Global.GetRandomNumber(-2000000000, -1);
						}
						while (scene.BossRankDict.ContainsKey(attackLog.RoleID));
						attackLog.UserPtID = 1;
						attackLog.ServerPtID = 1;
						attackLog.Param = "FAKE";
						attackLog.RoleName = string.Format("FAKE{0}", attackLog.RoleID);
						attackLog.ServerID = -999;
						attackLog.LocalRoleID = attackLog.RoleID;
						attackLog.InjureSum = (long)Global.GetRandomNumber(1000000, 2000000);
						scene.BossRankDict[attackLog.RoleID] = attackLog;
						scene.BossRankList.Add(attackLog);
					}
					this.ResortBossAttackRank(scene);
					this.BroadCastScoreInfo(scene);
				}
			}
		}

		// Token: 0x06001134 RID: 4404 RVA: 0x0010EAA8 File Offset: 0x0010CCA8
		public void PrintBossInfoGM(GameClient client, int logNum = 2147483647, Monster deadBoss = null)
		{
			RebornBossScene scene;
			if (this.SceneDict.TryGetValue(client.ClientData.MapCode, out scene))
			{
				lock (RebornBoss.Mutex)
				{
					int printNum = 0;
					foreach (RebornBossAttackLog item in scene.BossRankList)
					{
						if (++printNum > logNum)
						{
							break;
						}
						LogManager.WriteLog(LogTypes.Analysis, string.Format("RebornBoss ranknum={0} userptid={1} channel={2} rname={3} localrid={4} rid={5} injure={6} serverptid={7}", new object[]
						{
							item.RankNum,
							item.UserPtID,
							item.Param,
							item.RoleName,
							item.LocalRoleID,
							item.RoleID,
							item.InjureSum,
							item.ServerPtID
						}), null, true);
					}
				}
				if (deadBoss != null && deadBoss.LastDeadTicks > 0L)
				{
					DateTime deadTm = new DateTime(deadBoss.LastDeadTicks);
					DateTime birthTm = new DateTime(deadBoss.GetMonsterBirthTick());
					long aliveTicks = deadBoss.LastDeadTicks - deadBoss.GetMonsterBirthTick();
					LogManager.WriteLog(LogTypes.Analysis, string.Format("RebornBoss birthtm={0} deadtm={1} aliveSeconds={2}", birthTm, deadTm, aliveTicks / 10000000L), null, true);
				}
			}
		}

		// Token: 0x06001135 RID: 4405 RVA: 0x0010EC78 File Offset: 0x0010CE78
		public List<KFRebornBossAwardData> GetRebornBossKillAwardList(GameClient client)
		{
			List<KFRebornBossAwardData> BossAwardList = new List<KFRebornBossAwardData>();
			string awardsInfo = Global.GetRoleParamByName(client, "157");
			List<KFRebornBossAwardData> result;
			if (string.IsNullOrEmpty(awardsInfo))
			{
				result = BossAwardList;
			}
			else
			{
				string[] fields = awardsInfo.Split(new char[]
				{
					'|'
				});
				foreach (string kvpItem in fields)
				{
					string[] kvpFields = kvpItem.Split(new char[]
					{
						','
					});
					if (kvpFields.Length >= 3)
					{
						BossAwardList.Add(new KFRebornBossAwardData
						{
							MapCodeID = Convert.ToInt32(kvpFields[0]),
							LineID = Convert.ToInt32(kvpFields[1]),
							ExtensionID = Convert.ToInt32(kvpFields[2])
						});
					}
				}
				result = BossAwardList;
			}
			return result;
		}

		// Token: 0x06001136 RID: 4406 RVA: 0x0010ED5C File Offset: 0x0010CF5C
		public void SaveRebornBossKillAwardList(GameClient client, List<KFRebornBossAwardData> awardlist)
		{
			string strResult = "";
			foreach (KFRebornBossAwardData value in awardlist)
			{
				strResult += string.Format("{0},{1},{2}|", value.MapCodeID, value.LineID, value.ExtensionID);
			}
			if (!string.IsNullOrEmpty(strResult) && strResult.Substring(strResult.Length - 1) == "|")
			{
				strResult = strResult.Substring(0, strResult.Length - 1);
			}
			Global.UpdateRoleParamByName(client, "157", strResult, true);
		}

		// Token: 0x06001137 RID: 4407 RVA: 0x0010EE50 File Offset: 0x0010D050
		public void TimerProc_fuBenWorker()
		{
			if (GameManager.IsKuaFuServer)
			{
				DateTime now = TimeUtil.NowDateTime();
				long nowTicks = now.Ticks / 10000L;
				Dictionary<int, List<RebornBossConfig>> tempRebornBossConfigDict;
				lock (RebornBoss.Mutex)
				{
					if (Math.Abs(nowTicks - RebornBoss.LastHeartBeatTicks) < 1000L)
					{
						return;
					}
					RebornBoss.LastHeartBeatTicks = nowTicks;
					tempRebornBossConfigDict = this.RebornBossConfigDict;
				}
				foreach (KeyValuePair<int, List<RebornBossConfig>> kvp in tempRebornBossConfigDict)
				{
					if (!this.KuaFuLineDataDict.ContainsKey(kvp.Key))
					{
						List<KuaFuLineData> list = KuaFuWorldClient.getInstance().GetKuaFuLineDataList(kvp.Key) as List<KuaFuLineData>;
						if (null != list)
						{
							this.KuaFuLineDataDict[kvp.Key] = list;
						}
					}
				}
				foreach (KeyValuePair<int, List<KuaFuLineData>> kvp2 in this.KuaFuLineDataDict)
				{
					KuaFuLineData lineData = kvp2.Value.Find((KuaFuLineData x) => x.ServerId == GameManager.KuaFuServerId);
					if (lineData != null && !this.SceneDict.ContainsKey(kvp2.Key))
					{
						RebornBossScene newScene = new RebornBossScene();
						newScene.m_nMapCode = kvp2.Key;
						newScene.m_nLineID = lineData.Line;
						newScene.LoadSceneDBInfo();
						this.SceneDict[kvp2.Key] = newScene;
					}
				}
				foreach (RebornBossScene scene in this.SceneDict.Values)
				{
					lock (RebornBoss.Mutex)
					{
						this.CheckCreateDynamicMonster(scene, now);
						this.CheckCreateBoss(scene, now);
						this.CheckCreateBossState(scene, now);
					}
				}
			}
		}

		// Token: 0x04001A2C RID: 6700
		private static object Mutex = new object();

		// Token: 0x04001A2D RID: 6701
		private static long LastHeartBeatTicks = 0L;

		// Token: 0x04001A2E RID: 6702
		private static long LastHeartBeatTicks_Boss = 0L;

		// Token: 0x04001A2F RID: 6703
		public ConcurrentDictionary<int, RebornBossScene> SceneDict = new ConcurrentDictionary<int, RebornBossScene>();

		// Token: 0x04001A30 RID: 6704
		public Dictionary<int, List<RebornBossData>> BossDataDict = new Dictionary<int, List<RebornBossData>>();

		// Token: 0x04001A31 RID: 6705
		public Dictionary<int, List<KuaFuLineData>> KuaFuLineDataDict = new Dictionary<int, List<KuaFuLineData>>();

		// Token: 0x04001A32 RID: 6706
		public Dictionary<int, List<RebornBossConfig>> RebornBossConfigDict = new Dictionary<int, List<RebornBossConfig>>();

		// Token: 0x04001A33 RID: 6707
		public Dictionary<int, List<RebornBossAwardConfig>> RebornBossAwardConfigDict = new Dictionary<int, List<RebornBossAwardConfig>>();

		// Token: 0x04001A34 RID: 6708
		public int RebornBossRankClearSec = 120;

		// Token: 0x04001A35 RID: 6709
		private static RebornBoss instance = new RebornBoss();
	}
}
