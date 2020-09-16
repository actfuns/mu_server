using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Tools;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public static class ZhuanShengShiLian
	{
		
		public static void LoadZhuanShengShiLianXml()
		{
			string fileName = "";
			try
			{
				fileName = Global.GameResPath(ThemeDataConst.ThemeActivityZhuanSheng);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null != xml)
				{
					ZhuanShengShiLian.ZhuanShengRunTimeData.ThemeZSActivity.FromDate = "-1";
					ZhuanShengShiLian.ZhuanShengRunTimeData.ThemeZSActivity.ToDate = "-1";
					ZhuanShengShiLian.ZhuanShengRunTimeData.ThemeZSActivity.AwardStartDate = "-1";
					ZhuanShengShiLian.ZhuanShengRunTimeData.ThemeZSActivity.AwardEndDate = "-1";
					Dictionary<int, ZhuanShengMapInfo> mapInfoDict = new Dictionary<int, ZhuanShengMapInfo>();
					List<int> mapList = new List<int>();
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement xmlItem in nodes)
					{
						int mapCode = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "MapID", "0"));
						string[] minLevel = Global.GetDefAttributeStr(xmlItem, "MinLevel", "0").Split(new char[]
						{
							'|'
						});
						if (minLevel.Length >= 2)
						{
							string[] maxLevel = Global.GetDefAttributeStr(xmlItem, "MaxLevel", "0").Split(new char[]
							{
								'|'
							});
							if (maxLevel.Length >= 2)
							{
								ZhuanShengMapInfo mapInfo = new ZhuanShengMapInfo
								{
									MonstersID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "MonstersID", "0")),
									MapCode = mapCode,
									ReadyTime = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ReadyTime", "0")),
									FightSecs = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "FightSecs", "0")),
									ClearRolesSecs = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ClearRolesSecs", "0")),
									MaxEnterNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "MaxEnterNum", "0")),
									BornX = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "X", "0")),
									BornY = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Y", "0")),
									MinZhuanSheng = Convert.ToInt32(minLevel[0]),
									MinLevel = Convert.ToInt32(minLevel[1]),
									MaxZhuanSheng = Convert.ToInt32(maxLevel[0]),
									MaxLevel = Convert.ToInt32(maxLevel[1])
								};
								List<string> timePointsList = new List<string>();
								string timePoints = Global.GetDefAttributeStr(xmlItem, "TimePoints", "0");
								if (!string.IsNullOrEmpty(timePoints))
								{
									string[] sField = timePoints.Split(new char[]
									{
										'-'
									});
									for (int i = 0; i < sField.Length; i++)
									{
										timePointsList.Add(sField[i].Trim());
									}
								}
								mapInfo.TimePoints = timePointsList;
								mapInfoDict[mapCode] = mapInfo;
							}
						}
					}
					lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
					{
						ZhuanShengShiLian.ZhuanShengRunTimeData.ZhuanShengMapDict = mapInfoDict;
					}
					fileName = Global.GameResPath(ThemeDataConst.ThemeActivityZhuanShengReward);
					xml = CheckHelper.LoadXml(fileName, true);
					if (null != xml)
					{
						Dictionary<int, List<ShiLianReward>> shiLianDict = new Dictionary<int, List<ShiLianReward>>();
						nodes = xml.Elements();
						foreach (XElement xmlItem in nodes)
						{
							int mapCode = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "MapID", "0"));
							List<ShiLianReward> shiLianList;
							if (!shiLianDict.TryGetValue(mapCode, out shiLianList))
							{
								shiLianList = new List<ShiLianReward>();
								shiLianDict[mapCode] = shiLianList;
							}
							shiLianList.Add(new ShiLianReward
							{
								MinRank = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "MinRank", "0")),
								MaxRank = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "MaxRank", "0")),
								WinRewardItem = Global.GetDefAttributeStr(xmlItem, "WinRewardItem", ""),
								WinrewardExp = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "WinrewardExp", "0")),
								WinRewardMoney = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "WinRewardMoney", "0")),
								LoseRewardItem = Global.GetDefAttributeStr(xmlItem, "LoseRewardItem", "0"),
								LoseRewardExp = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "LoseRewardExp", "0")),
								LoseRewardMoney = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "LoseRewardMoney", "0"))
							});
						}
						List<int> goodsList = new List<int>();
						string goodsStr = GameManager.systemParamsList.GetParamValueByName("ThemeActivityZhuanShengGoods");
						if (!string.IsNullOrEmpty(goodsStr))
						{
							goodsList = Array.ConvertAll<string, int>(goodsStr.Split(new char[]
							{
								','
							}), (string _x) => Convert.ToInt32(_x)).ToList<int>();
						}
						lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
						{
							ZhuanShengShiLian.ZhuanShengRunTimeData.ShiLianRewardDict = shiLianDict;
							ZhuanShengShiLian.ZhuanShengRunTimeData.BroadGoodsIDList = goodsList;
						}
						ZhuanShengShiLian.ZhuanShengRunTimeData.ThemeZSActivity.ActivityType = 157;
						ZhuanShengShiLian.ZhuanShengRunTimeData.ThemeZSActivity.PredealDateTime();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
			}
		}

		
		public static int GetZhuanShengShiLianMapCodeIDForRole(GameClient client)
		{
			int mapCodeID = -1;
			lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
			{
				foreach (ZhuanShengMapInfo item in ZhuanShengShiLian.ZhuanShengRunTimeData.ZhuanShengMapDict.Values)
				{
					int maxUnionLev = Global.GetUnionLevel2(item.MaxZhuanSheng, item.MaxLevel);
					int minUnionLev = Global.GetUnionLevel2(item.MinZhuanSheng, item.MinLevel);
					int roleUnionLev = Global.GetUnionLevel2(client);
					if (roleUnionLev >= minUnionLev && roleUnionLev <= maxUnionLev)
					{
						mapCodeID = item.MapCode;
						break;
					}
				}
			}
			return mapCodeID;
		}

		
		public static bool EnterSceneCopyScene(GameClient client, out int nSeqID, int mapCode)
		{
			nSeqID = -1;
			ZhuanShengMapInfo mapInfo;
			lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
			{
				if (!ZhuanShengShiLian.ZhuanShengRunTimeData.ZhuanShengMapDict.TryGetValue(mapCode, out mapInfo))
				{
					return false;
				}
			}
			bool result;
			if (!ZhuanShengShiLian.JudgeCanEnterOnTime(mapInfo))
			{
				result = false;
			}
			else
			{
				int calEnterMapCode = ZhuanShengShiLian.GetZhuanShengShiLianMapCodeIDForRole(client);
				if (calEnterMapCode <= 0 || mapCode != calEnterMapCode)
				{
					result = false;
				}
				else
				{
					ZSSLScene sceneInfo = null;
					lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
					{
						foreach (KeyValuePair<int, ZSSLScene> kvp in ZhuanShengShiLian.SceneDict)
						{
							if (kvp.Value.SceneInfo.MapCode == mapCode)
							{
								sceneInfo = kvp.Value;
								nSeqID = kvp.Key;
								break;
							}
						}
						if (null == sceneInfo)
						{
							nSeqID = GameCoreInterface.getinstance().GetNewFuBenSeqId();
							sceneInfo = new ZSSLScene();
							sceneInfo.CleanAllInfo();
							sceneInfo.SceneInfo = mapInfo;
							ZhuanShengShiLian.SceneDict[nSeqID] = sceneInfo;
						}
					}
					if (null != sceneInfo.m_CopyMap)
					{
						if (sceneInfo.m_CopyMap.GetGameClientCount() >= mapInfo.MaxEnterNum)
						{
							return false;
						}
					}
					result = true;
				}
			}
			return result;
		}

		
		public static void AddCopyScenes(int nSequenceID, int nFubenID, int nMapCodeID, CopyMap mapInfo)
		{
			lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
			{
				ZSSLScene zsslScene;
				if (ZhuanShengShiLian.SceneDict.TryGetValue(nSequenceID, out zsslScene))
				{
					zsslScene.m_CopyMap = mapInfo;
				}
			}
		}

		
		public static void RemoveCopyScenes(CopyMap cmInfo, int nSqeID, int nCopyID)
		{
			lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
			{
				ZSSLScene zsslScene;
				ZhuanShengShiLian.SceneDict.TryRemove(nSqeID, out zsslScene);
			}
		}

		
		public static void OnEnterScene(GameClient client)
		{
			ZhuanShengShiLian.SendTimeInfoToClient(client);
		}

		
		public static void SendTimeInfoToAll(ZSSLScene scene, long ticks)
		{
			List<GameClient> objsList = scene.m_CopyMap.GetClientsList();
			if (objsList != null && objsList.Count > 0)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					GameClient client = objsList[i];
					if (client != null)
					{
						int nRemainSecs;
						int nStatus;
						lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
						{
							nRemainSecs = (int)((scene.StatusEndTime - ticks) / 1000L);
							nStatus = (int)scene.State;
						}
						string strcmd = string.Format("{0}:{1}", nStatus, nRemainSecs);
						client.sendCmd(1909, strcmd, false);
					}
				}
			}
		}

		
		public static void SendTimeInfoToClient(GameClient client)
		{
			ZSSLScene zsslScene;
			if (ZhuanShengShiLian.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out zsslScene))
			{
				long ticks = TimeUtil.NOW();
				int nRemainSecs;
				int nStatus;
				lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
				{
					nRemainSecs = (int)((zsslScene.StatusEndTime - ticks) / 1000L);
					nStatus = (int)zsslScene.State;
				}
				string strcmd = string.Format("{0}:{1}", nStatus, nRemainSecs);
				client.sendCmd(1909, strcmd, false);
			}
		}

		
		public static void GiveGoodsAward(GameClient client, string goods)
		{
			string[] goodList = goods.Split(new char[]
			{
				'|'
			});
			List<GoodsData> awardList = new List<GoodsData>();
			for (int i = 0; i < goodList.Length; i++)
			{
				if (!(goodList[i] == ""))
				{
					string[] goodItem = goodList[i].Split(new char[]
					{
						','
					});
					if (goodItem.Length == 7)
					{
						GoodsData goodsData = new GoodsData
						{
							Id = -1,
							GoodsID = Convert.ToInt32(goodItem[0]),
							Using = 0,
							Forge_level = Convert.ToInt32(goodItem[3]),
							Starttime = "1900-01-01 12:00:00",
							Endtime = "1900-01-01 12:00:00",
							Site = 0,
							GCount = Convert.ToInt32(goodItem[1]),
							Binding = Convert.ToInt32(goodItem[2]),
							BagIndex = 0,
							Lucky = Convert.ToInt32(goodItem[5]),
							ExcellenceInfo = Convert.ToInt32(goodItem[6]),
							AppendPropLev = Convert.ToInt32(goodItem[4])
						};
						SystemXmlItem systemGoods = null;
						if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoods))
						{
							string strinfo = string.Format("系统中不存在{0}", goodsData.GoodsID);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							awardList.Add(goodsData);
						}
					}
				}
			}
			if (!Global.CanAddGoodsNum(client, goodList.Length))
			{
				Global.UseMailGivePlayerAward2(client, awardList, GLang.GetLang(4011, new object[0]), GLang.GetLang(4011, new object[0]), 0, 0, 0);
			}
			else
			{
				foreach (GoodsData item in awardList)
				{
					SystemXmlItem systemXmlGood = null;
					GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(item.GoodsID, out systemXmlGood);
					string goodsName = systemXmlGood.GetStringValue("Title");
					LogManager.WriteLog(LogTypes.SQL, string.Format("转生试炼奖励{0} {1}", client.ClientData.RoleID, goodsName), null, true);
					Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, item.GCount, item.Quality, "", item.Forge_level, item.Binding, item.Site, "", true, 1, "转生试炼奖励", "1900-01-01 12:00:00", 0, 0, item.Lucky, 0, item.ExcellenceInfo, item.AppendPropLev, 0, null, null, 0, true);
				}
			}
		}

		
		public static int KillerRid(GameClient client, Monster monster)
		{
			ZSSLScene mapInfo;
			int result;
			if (!ZhuanShengShiLian.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out mapInfo))
			{
				result = 0;
			}
			else
			{
				int roleID = 0;
				long maxInjure = 0L;
				lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
				{
					if (mapInfo.AttackLog.BHAttackRank == null || mapInfo.AttackLog.BHAttackRank.Count < 1)
					{
						return 0;
					}
					BHAttackLog bhAttackLog = mapInfo.AttackLog.BHAttackRank[0];
					if (null == bhAttackLog.RoleInjure)
					{
						return 0;
					}
					foreach (KeyValuePair<int, long> item in bhAttackLog.RoleInjure)
					{
						if (item.Value > maxInjure)
						{
							roleID = item.Key;
							maxInjure = item.Value;
						}
					}
				}
				result = roleID;
			}
			return result;
		}

		
		public static bool IsShiLianGoods(int goodsID)
		{
			bool result;
			lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
			{
				result = ZhuanShengShiLian.ZhuanShengRunTimeData.BroadGoodsIDList.Contains(goodsID);
			}
			return result;
		}

		
		public static void BroadMsg(int mapCode, string broadMsg)
		{
			int minZhuanSheng = 0;
			int minLevel = 0;
			int maxZhuangSheng = 100;
			int maxLevel = 100;
			lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
			{
				ZhuanShengMapInfo mapInfo;
				if (!ZhuanShengShiLian.ZhuanShengRunTimeData.ZhuanShengMapDict.TryGetValue(mapCode, out mapInfo))
				{
					return;
				}
				minZhuanSheng = mapInfo.MinZhuanSheng;
				minLevel = mapInfo.MinLevel;
				maxZhuangSheng = mapInfo.MaxZhuanSheng;
				maxLevel = mapInfo.MaxLevel;
			}
			Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.HintMsg, broadMsg, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.HintAndBox, minZhuanSheng, minLevel, maxZhuangSheng, maxLevel);
		}

		
		public static void BroadBossLife(ZSSLScene mapInfo, GameClient client, bool Top5Chg)
		{
			if (null != mapInfo.AttackLog)
			{
				BossLifeLog bossLifeLog = new BossLifeLog();
				bossLifeLog.InjureSum = mapInfo.AttackLog.InjureSum;
				if (null != mapInfo.AttackLog.BHAttackRank)
				{
					int countLimit = Global.GMin(mapInfo.AttackLog.BHAttackRank.Count, 5);
					bossLifeLog.BHAttackRank = mapInfo.AttackLog.BHAttackRank.GetRange(0, countLimit);
				}
				List<GameClient> objsList = mapInfo.m_CopyMap.GetClientsList();
				if (objsList != null && objsList.Count > 0)
				{
					for (int i = 0; i < objsList.Count; i++)
					{
						GameClient c = objsList[i];
						if (c != null)
						{
							if (!Top5Chg && null != client)
							{
								if (client.ClientData.TeamID > 0 && client.ClientData.TeamID != c.ClientData.TeamID)
								{
									goto IL_189;
								}
								if (c.ClientData.RoleID != client.ClientData.RoleID)
								{
									goto IL_189;
								}
							}
							if (null != mapInfo.AttackLog.BHInjure)
							{
								long tID = ZhuanShengShiLian.GetGUID(c.ClientData.TeamID, c.ClientData.RoleID);
								mapInfo.AttackLog.BHInjure.TryGetValue(tID, out bossLifeLog.SelfBHAttack);
							}
							c.sendCmd<BossLifeLog>(1906, bossLifeLog, false);
						}
						IL_189:;
					}
				}
			}
		}

		
		public static void ProcessAttack(GameClient client, Monster monster, int injure)
		{
			try
			{
				if (injure > 0)
				{
					long tID = ZhuanShengShiLian.GetGUID(client.ClientData.TeamID, client.ClientData.RoleID);
					string tName = client.ClientData.RoleName;
					TeamData td = GameManager.TeamMgr.FindData(client.ClientData.TeamID);
					if (null != td)
					{
						lock (td)
						{
							TeamMemberData member = td.GetLeader();
							if (null != member)
							{
								tName = member.RoleName;
							}
						}
					}
					ZSSLScene mapInfo;
					if (ZhuanShengShiLian.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out mapInfo))
					{
						bool top5Chg = false;
						lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
						{
							BossAttackLog bossAttackLog = mapInfo.AttackLog;
							if (null == bossAttackLog)
							{
								bossAttackLog = new BossAttackLog
								{
									InjureSum = 0L,
									BHInjure = new Dictionary<long, BHAttackLog>(),
									BHAttackRank = new List<BHAttackLog>()
								};
							}
							BHAttackLog bhAttackLog;
							if (!bossAttackLog.BHInjure.TryGetValue(tID, out bhAttackLog))
							{
								bhAttackLog = new BHAttackLog
								{
									BHName = tName,
									BHInjure = 0L,
									RoleInjure = new Dictionary<int, long>()
								};
								bossAttackLog.BHInjure[tID] = bhAttackLog;
							}
							if (!bhAttackLog.RoleInjure.ContainsKey(client.ClientData.RoleID))
							{
								bhAttackLog.RoleInjure[client.ClientData.RoleID] = 0L;
							}
							Dictionary<int, long> roleInjure;
							int roleID;
							(roleInjure = bhAttackLog.RoleInjure)[roleID = client.ClientData.RoleID] = roleInjure[roleID] + (long)injure;
							bhAttackLog.BHInjure += (long)injure;
							top5Chg = ZhuanShengShiLian.TrySortAttackRank(bossAttackLog, bhAttackLog);
							bossAttackLog.InjureSum += (long)injure;
						}
						ZhuanShengShiLian.BroadBossLife(mapInfo, client, top5Chg);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ZhuanShengShiLian :: 处理攻击boss异常。", new object[0]), ex, true);
			}
		}

		
		private static bool TrySortAttackRank(BossAttackLog bossAttackLog, BHAttackLog myAttackLog)
		{
			bool result;
			if (bossAttackLog == null || null == myAttackLog)
			{
				result = false;
			}
			else
			{
				BHAttackLog rankMin = null;
				bool justReSort = false;
				bool rebuildRank = false;
				if (bossAttackLog.BHAttackRank.Count >= 20)
				{
					int countLimit = Global.GMin(bossAttackLog.BHAttackRank.Count, 20);
					rankMin = bossAttackLog.BHAttackRank[countLimit - 1];
					if (myAttackLog.BHInjure > rankMin.BHInjure)
					{
						rebuildRank = true;
					}
				}
				else if (bossAttackLog.BHAttackRank.Count < 20)
				{
					rebuildRank = true;
				}
				if (bossAttackLog.BHAttackRank.Exists((BHAttackLog x) => object.ReferenceEquals(x, myAttackLog)))
				{
					if (rankMin != null && myAttackLog.BHInjure > rankMin.BHInjure)
					{
						justReSort = true;
					}
					else
					{
						rebuildRank = true;
					}
				}
				int myRank = bossAttackLog.BHAttackRank.FindIndex((BHAttackLog x) => object.ReferenceEquals(x, myAttackLog));
				bool top5Chg = myRank >= 0 && myRank < 5;
				if (justReSort)
				{
					bossAttackLog.BHAttackRank.Sort((BHAttackLog x, BHAttackLog y) => (int)(y.BHInjure - x.BHInjure));
				}
				if (rebuildRank)
				{
					bossAttackLog.BHAttackRank = bossAttackLog.BHInjure.Values.ToList<BHAttackLog>();
					bossAttackLog.BHAttackRank.Sort((BHAttackLog x, BHAttackLog y) => (int)(y.BHInjure - x.BHInjure));
					int countLimit = Global.GMin(bossAttackLog.BHAttackRank.Count, 20);
					bossAttackLog.BHAttackRank = bossAttackLog.BHAttackRank.GetRange(0, countLimit);
				}
				myRank = bossAttackLog.BHAttackRank.FindIndex((BHAttackLog x) => object.ReferenceEquals(x, myAttackLog));
				top5Chg |= (myRank >= 0 && myRank < 5);
				result = top5Chg;
			}
			return result;
		}

		
		public static void ProcessBossDie(GameClient client, Monster monster)
		{
			try
			{
				ZSSLScene mapInfo;
				if (ZhuanShengShiLian.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out mapInfo))
				{
					lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
					{
						mapInfo.BossDie = true;
						mapInfo.State = BattleStates.EndFight;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ZhuanShengShiLian :: 处理boss死亡异常。", new object[0]), ex, true);
			}
		}

		
		public static long GetGUID(int teamID, int roleID)
		{
			long lKey = (long)((teamID > 0) ? teamID : 0);
			long lKey2 = (long)((teamID > 0) ? 0 : roleID);
			return lKey << 32 | lKey2;
		}

		
		public static int CheckInviteOrApplyTeam(GameClient client, GameClient otherClient)
		{
			int result;
			if (!ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(client.ClientData.MapCode) && !ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(otherClient.ClientData.MapCode))
			{
				result = 0;
			}
			else if (ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(client.ClientData.MapCode) && !ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(otherClient.ClientData.MapCode))
			{
				result = -101;
			}
			else if (!ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(client.ClientData.MapCode) && ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(otherClient.ClientData.MapCode))
			{
				result = -102;
			}
			else if (client.ClientData.FuBenSeqID != otherClient.ClientData.FuBenSeqID)
			{
				result = -101;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		
		public static void OnCreateTeamCopyRoleLog(GameClient client)
		{
			try
			{
				if (ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(client.ClientData.MapCode))
				{
					ZSSLScene mapInfo;
					if (ZhuanShengShiLian.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out mapInfo))
					{
						lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
						{
							BossAttackLog bossAttackLog = mapInfo.AttackLog;
							if (null != bossAttackLog)
							{
								long tID_Role = ZhuanShengShiLian.GetGUID(0, client.ClientData.RoleID);
								long tID_Team = ZhuanShengShiLian.GetGUID(client.ClientData.TeamID, client.ClientData.RoleID);
								BHAttackLog tAttackLog_Role = null;
								if (bossAttackLog.BHInjure.TryGetValue(tID_Role, out tAttackLog_Role))
								{
									bossAttackLog.BHInjure.Remove(tID_Role);
								}
								if (null != tAttackLog_Role)
								{
									bossAttackLog.BHInjure[tID_Team] = tAttackLog_Role;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ZhuanShengShiLian :: 处理拷贝角色伤害记录异常。", new object[0]), ex, true);
			}
		}

		
		public static void ProcessClearRoleLog(GameClient client)
		{
			try
			{
				if (ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(client.ClientData.MapCode))
				{
					ZSSLScene mapInfo;
					if (ZhuanShengShiLian.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out mapInfo))
					{
						long tID = ZhuanShengShiLian.GetGUID(client.ClientData.TeamID, client.ClientData.RoleID);
						int leader = -1;
						TeamData td = GameManager.TeamMgr.FindData(client.ClientData.TeamID);
						if (null != td)
						{
							leader = td.LeaderRoleID;
						}
						bool top5Chg = false;
						lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
						{
							BossAttackLog bossAttackLog = mapInfo.AttackLog;
							if (null == bossAttackLog)
							{
								return;
							}
							BHAttackLog tAttackLog;
							if (bossAttackLog.BHInjure.TryGetValue(tID, out tAttackLog))
							{
								if (leader == -1)
								{
									tAttackLog.BHInjure = 0L;
									bossAttackLog.BHInjure.Remove(tID);
								}
								else
								{
									long roleInjure = 0L;
									if (tAttackLog.RoleInjure.TryGetValue(client.ClientData.RoleID, out roleInjure))
									{
										tAttackLog.RoleInjure.Remove(client.ClientData.RoleID);
										tAttackLog.BHInjure -= roleInjure;
										if (tAttackLog.BHInjure <= 0L)
										{
											bossAttackLog.BHInjure.Remove(tID);
										}
									}
								}
								top5Chg = ZhuanShengShiLian.TrySortAttackRank(bossAttackLog, tAttackLog);
							}
						}
						ZhuanShengShiLian.BroadBossLife(mapInfo, client, top5Chg);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ZhuanShengShiLian :: 处理清除角色伤害记录异常。", new object[0]), ex, true);
			}
		}

		
		public static void ProcessChangeTeamName(GameClient client, bool needBroad = false)
		{
			try
			{
				if (ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(client.ClientData.MapCode))
				{
					ZSSLScene mapInfo;
					if (ZhuanShengShiLian.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out mapInfo))
					{
						TeamData td = GameManager.TeamMgr.FindData(client.ClientData.TeamID);
						if (null != td)
						{
							string newName = "";
							long tID = ZhuanShengShiLian.GetGUID(client.ClientData.TeamID, client.ClientData.RoleID);
							lock (td)
							{
								if (td.LeaderRoleID == client.ClientData.RoleID)
								{
									return;
								}
								TeamMemberData member = td.GetLeader();
								if (null == member)
								{
									return;
								}
								newName = member.RoleName;
							}
							BHAttackLog tAttackLog;
							lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
							{
								if (null == mapInfo.AttackLog)
								{
									return;
								}
								if (!mapInfo.AttackLog.BHInjure.TryGetValue(tID, out tAttackLog))
								{
									return;
								}
								tAttackLog.BHName = newName;
							}
							if (needBroad)
							{
								int myRank = mapInfo.AttackLog.BHAttackRank.FindIndex((BHAttackLog x) => object.ReferenceEquals(x, tAttackLog));
								bool top5Chg = myRank >= 0 && myRank < 5;
								ZhuanShengShiLian.BroadBossLife(mapInfo, client, top5Chg);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ZhuanShengShiLian :: 处理清除角色伤害记录异常。", new object[0]), ex, true);
			}
		}

		
		public static bool IsZhuanShengShiLianCopyScene(int mapCode)
		{
			bool result;
			lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
			{
				result = ZhuanShengShiLian.ZhuanShengRunTimeData.ZhuanShengMapDict.ContainsKey(mapCode);
			}
			return result;
		}

		
		public static bool JudgeCanEnterOnTime(ZhuanShengMapInfo mapInfo)
		{
			DateTime now = TimeUtil.NowDateTime();
			DateTime startTime = DateTime.Parse(mapInfo.TimePoints[0]);
			DateTime endTime = startTime.AddMinutes((double)(mapInfo.ReadyTime / 60));
			return now > startTime && now <= endTime;
		}

		
		public static bool CanFight(GameClient client)
		{
			ZSSLScene mapInfo;
			bool result;
			if (!ZhuanShengShiLian.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out mapInfo))
			{
				result = false;
			}
			else
			{
				lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
				{
					result = (mapInfo.State == BattleStates.StartFight);
				}
			}
			return result;
		}

		
		public static void TimerProc()
		{
			if (!GameManager.IsKuaFuServer)
			{
				long nowTicks = TimeUtil.NOW();
				lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
				{
					if (Math.Abs(nowTicks - ZhuanShengShiLian.LastHeartBeatTicks) < 1000L)
					{
						return;
					}
					ZhuanShengShiLian.LastHeartBeatTicks = nowTicks;
				}
				if (157 == ZhuanShengShiLian.ZhuanShengRunTimeData.ThemeZSActivity.ActivityType && ZhuanShengShiLian.ZhuanShengRunTimeData.ThemeZSActivity.InActivityTime())
				{
					foreach (KeyValuePair<int, ZSSLScene> scenes in ZhuanShengShiLian.SceneDict)
					{
						lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
						{
							switch (scenes.Value.State)
							{
							case BattleStates.NoBattle:
							{
								DateTime startTime = DateTime.Parse(scenes.Value.SceneInfo.TimePoints[0]).AddSeconds((double)scenes.Value.SceneInfo.ReadyTime);
								scenes.Value.StartTick = startTime.Ticks / 10000L;
								scenes.Value.EndTick = startTime.AddSeconds((double)scenes.Value.SceneInfo.FightSecs).Ticks / 10000L;
								scenes.Value.StatusEndTime = scenes.Value.StartTick;
								ZhuanShengShiLian.BroadMsg(scenes.Value.SceneInfo.MapCode, GLang.GetLang(4010, new object[0]));
								scenes.Value.State = BattleStates.WaitingFight;
								break;
							}
							case BattleStates.WaitingFight:
								if (nowTicks >= scenes.Value.StartTick && null != scenes.Value.m_CopyMap)
								{
									GameManager.MonsterZoneMgr.AddDynamicMonsters(scenes.Value.SceneInfo.MapCode, scenes.Value.SceneInfo.MonstersID, scenes.Value.m_CopyMap.FuBenSeqID, 1, scenes.Value.SceneInfo.BornX / 100, scenes.Value.SceneInfo.BornY / 100, 0, 0, SceneUIClasses.Normal, null, null);
									scenes.Value.State = BattleStates.StartFight;
									scenes.Value.StatusEndTime = scenes.Value.EndTick;
									ZhuanShengShiLian.SendTimeInfoToAll(scenes.Value, nowTicks);
								}
								break;
							case BattleStates.StartFight:
								if (nowTicks >= scenes.Value.EndTick)
								{
									scenes.Value.State = BattleStates.EndFight;
									scenes.Value.StatusEndTime = scenes.Value.EndTick;
									scenes.Value.BossDie = false;
									List<object> monsterList = GameManager.MonsterMgr.GetObjectsByMap(scenes.Value.SceneInfo.MapCode);
									foreach (object monster in monsterList)
									{
										if (monster is Monster)
										{
											GameManager.MonsterMgr.DeadMonsterImmediately(monster as Monster);
										}
									}
								}
								break;
							case BattleStates.EndFight:
								try
								{
									List<ShiLianReward> rewardList;
									if (ZhuanShengShiLian.ZhuanShengRunTimeData.ShiLianRewardDict.TryGetValue(scenes.Value.SceneInfo.MapCode, out rewardList))
									{
										List<BHAttackLog> bhAttackLogList = scenes.Value.AttackLog.BHInjure.Values.ToList<BHAttackLog>();
										int i;
										for (i = 0; i < bhAttackLogList.Count; i++)
										{
											if (bhAttackLogList[i].BHInjure > 0L)
											{
												int rank = scenes.Value.AttackLog.BHAttackRank.FindIndex((BHAttackLog x) => object.ReferenceEquals(x, bhAttackLogList[i]));
												rank++;
												ShiLianReward reward = rewardList.Find((ShiLianReward _x) => _x.MinRank <= rank && (rank <= _x.MaxRank || _x.MaxRank < 0));
												if (null != reward)
												{
													int exp = scenes.Value.BossDie ? reward.WinrewardExp : reward.LoseRewardExp;
													int money = scenes.Value.BossDie ? reward.WinRewardMoney : reward.LoseRewardMoney;
													string goods = scenes.Value.BossDie ? reward.WinRewardItem : reward.LoseRewardItem;
													foreach (KeyValuePair<int, long> role in bhAttackLogList[i].RoleInjure)
													{
														GameClient client = GameManager.ClientMgr.FindClient(role.Key);
														if (null != client)
														{
															if (client.ClientData.MapCode == scenes.Value.SceneInfo.MapCode)
															{
																GameManager.ClientMgr.ProcessRoleExperience(client, (long)exp, false, true, false, "none");
																GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, money, "转生试炼添加绑金", true);
																ZhuanShengShiLian.GiveGoodsAward(client, goods);
																client.sendCmd(1908, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
																{
																	scenes.Value.BossDie ? 1 : 0,
																	goods,
																	exp,
																	money,
																	rank
																}), false);
															}
														}
													}
												}
											}
										}
									}
									scenes.Value.AttackLog = null;
								}
								catch (Exception ex)
								{
									DataHelper.WriteExceptionLogEx(ex, "转生试炼调度异常");
								}
								scenes.Value.ClearTick = nowTicks + (long)(scenes.Value.SceneInfo.ClearRolesSecs * 1000);
								scenes.Value.StatusEndTime = scenes.Value.ClearTick;
								scenes.Value.State = BattleStates.ClearBattle;
								ZhuanShengShiLian.SendTimeInfoToAll(scenes.Value, nowTicks);
								break;
							case BattleStates.ClearBattle:
								if (nowTicks >= scenes.Value.ClearTick)
								{
									List<GameClient> objsList = scenes.Value.m_CopyMap.GetClientsList();
									if (objsList != null && objsList.Count > 0)
									{
										for (int j = 0; j < objsList.Count; j++)
										{
											GameClient client = objsList[j];
											if (client != null)
											{
												int toMapCode = GameManager.MainMapCode;
												int toPosX = -1;
												int toPosY = -1;
												if (client.ClientData.LastMapCode != -1 && client.ClientData.LastPosX != -1 && client.ClientData.LastPosY != -1)
												{
													if (MapTypes.Normal == Global.GetMapType(client.ClientData.LastMapCode))
													{
														toMapCode = client.ClientData.LastMapCode;
														toPosX = client.ClientData.LastPosX;
														toPosY = client.ClientData.LastPosY;
													}
												}
												GameMap gameMap = null;
												if (GameManager.MapMgr.DictMaps.TryGetValue(toMapCode, out gameMap))
												{
													GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, toMapCode, toPosX, toPosY, -1, 0);
												}
											}
										}
									}
									scenes.Value.State = BattleStates.NoBattle;
								}
								break;
							}
						}
					}
				}
			}
		}

		
		private static long LastHeartBeatTicks = 0L;

		
		public static ZhuanShengRunData ZhuanShengRunTimeData = new ZhuanShengRunData();

		
		public static ConcurrentDictionary<int, ZSSLScene> SceneDict = new ConcurrentDictionary<int, ZSSLScene>();
	}
}
