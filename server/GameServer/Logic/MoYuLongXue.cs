using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x02000436 RID: 1078
	public class MoYuLongXue
	{
		// Token: 0x060013C3 RID: 5059 RVA: 0x00137C14 File Offset: 0x00135E14
		public static void LoadMoYuXml()
		{
			string fileName = "";
			try
			{
				lock (MoYuLongXue.MoYuRunTimeData.Mutex)
				{
					fileName = Global.GameResPath(ThemeDataConst.ThemeActivityMoYu);
					XElement xml = CheckHelper.LoadXml(fileName, true);
					if (null != xml)
					{
						MoYuLongXue.MoYuRunTimeData.ThemeMoYuActivity.FromDate = "-1";
						MoYuLongXue.MoYuRunTimeData.ThemeMoYuActivity.ToDate = "-1";
						MoYuLongXue.MoYuRunTimeData.ThemeMoYuActivity.AwardStartDate = "-1";
						MoYuLongXue.MoYuRunTimeData.ThemeMoYuActivity.AwardEndDate = "-1";
						Dictionary<int, MoYuMonsterInfo> moYuMonsterInfoDict = new Dictionary<int, MoYuMonsterInfo>();
						List<int> mapList = new List<int>();
						IEnumerable<XElement> nodes = xml.Elements();
						foreach (XElement xmlItem in nodes)
						{
							int monstersID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "MonstersID", "0"));
							int mapCode = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "MapId", "0"));
							moYuMonsterInfoDict[monstersID] = new MoYuMonsterInfo
							{
								MonstersID = monstersID,
								NpcID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "NpcID", "0")),
								MapCode = mapCode,
								Chengjiu = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Chengjiu", "0")),
								Shengwang = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Shengwang", "0")),
								HurtMin = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "HurtMin", "0"))
							};
							if (!mapList.Contains(mapCode))
							{
								mapList.Add(mapCode);
							}
						}
						List<int> goodsList = new List<int>();
						string goodsStr = GameManager.systemParamsList.GetParamValueByName("ThemeActivityMoYuGoods");
						if (!string.IsNullOrEmpty(goodsStr))
						{
							goodsList = Array.ConvertAll<string, int>(goodsStr.Split(new char[]
							{
								','
							}), (string _x) => Convert.ToInt32(_x)).ToList<int>();
						}
						MoYuLongXue.MoYuRunTimeData.MonsterXmlDict.Clear();
						MoYuLongXue.MoYuRunTimeData.BroadGoodsIDList.Clear();
						MoYuLongXue.MoYuRunTimeData.MapCodeList.Clear();
						MoYuLongXue.MoYuRunTimeData.MonsterXmlDict = moYuMonsterInfoDict;
						MoYuLongXue.MoYuRunTimeData.BroadGoodsIDList = goodsList;
						MoYuLongXue.MoYuRunTimeData.MapCodeList = mapList;
						MoYuLongXue.MoYuRunTimeData.ThemeMoYuActivity.ActivityType = 156;
						MoYuLongXue.MoYuRunTimeData.ThemeMoYuActivity.PredealDateTime();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
			}
		}

		// Token: 0x060013C4 RID: 5060 RVA: 0x00137F54 File Offset: 0x00136154
		public static bool InActivityTime()
		{
			bool result;
			lock (MoYuLongXue.MoYuRunTimeData.Mutex)
			{
				if (null == MoYuLongXue.MoYuRunTimeData.ThemeMoYuActivity)
				{
					result = false;
				}
				else
				{
					result = MoYuLongXue.MoYuRunTimeData.ThemeMoYuActivity.InActivityTime();
				}
			}
			return result;
		}

		// Token: 0x060013C5 RID: 5061 RVA: 0x00137FC8 File Offset: 0x001361C8
		public static bool InMoYuMap(int mapCode)
		{
			bool result;
			lock (MoYuLongXue.MoYuRunTimeData.Mutex)
			{
				result = MoYuLongXue.MoYuRunTimeData.MapCodeList.Contains(mapCode);
			}
			return result;
		}

		// Token: 0x060013C6 RID: 5062 RVA: 0x00138024 File Offset: 0x00136224
		public static bool IsBHGoods(int goodsID)
		{
			bool result;
			lock (MoYuLongXue.MoYuRunTimeData.Mutex)
			{
				result = MoYuLongXue.MoYuRunTimeData.BroadGoodsIDList.Contains(goodsID);
			}
			return result;
		}

		// Token: 0x060013C7 RID: 5063 RVA: 0x00138080 File Offset: 0x00136280
		public static void OnBangHuiDestroy(int faction)
		{
			lock (MoYuLongXue.MoYuRunTimeData.Mutex)
			{
				foreach (BossAttackLog attackLog in MoYuLongXue.MoYuRunTimeData.BossAttackLogDict.Values)
				{
					if (null != attackLog.BHInjure)
					{
						BHAttackLog bhLog = null;
						if (attackLog.BHInjure.TryGetValue((long)faction, out bhLog))
						{
							bhLog.RoleInjure.Clear();
						}
					}
				}
			}
		}

		// Token: 0x060013C8 RID: 5064 RVA: 0x00138154 File Offset: 0x00136354
		public static void OnClientLeaveBangHui(int faction, int rid)
		{
			lock (MoYuLongXue.MoYuRunTimeData.Mutex)
			{
				foreach (BossAttackLog attackLog in MoYuLongXue.MoYuRunTimeData.BossAttackLogDict.Values)
				{
					if (null != attackLog.BHInjure)
					{
						BHAttackLog bhLog = null;
						if (attackLog.BHInjure.TryGetValue((long)faction, out bhLog))
						{
							bhLog.RoleInjure.Remove(rid);
						}
					}
				}
			}
		}

		// Token: 0x060013C9 RID: 5065 RVA: 0x00138228 File Offset: 0x00136428
		public static int KillerRid(Monster monster)
		{
			int roleID = 0;
			long maxInjure = 0L;
			lock (MoYuLongXue.MoYuRunTimeData.Mutex)
			{
				BossAttackLog bossAttackLog;
				if (!MoYuLongXue.MoYuRunTimeData.BossAttackLogDict.TryGetValue(monster.RoleID, out bossAttackLog))
				{
					return 0;
				}
				if (bossAttackLog.BHAttackRank == null || bossAttackLog.BHAttackRank.Count < 1)
				{
					return 0;
				}
				BHAttackLog bhAttackLog = bossAttackLog.BHAttackRank[0];
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
			return roleID;
		}

		// Token: 0x060013CA RID: 5066 RVA: 0x00138364 File Offset: 0x00136564
		public static int GetBossLeftCount()
		{
			int result;
			lock (MoYuLongXue.MoYuRunTimeData.Mutex)
			{
				if (null == MoYuLongXue.MoYuRunTimeData.BossAttackLogDict)
				{
					result = 0;
				}
				else
				{
					result = MoYuLongXue.MoYuRunTimeData.BossAttackLogDict.Count;
				}
			}
			return result;
		}

		// Token: 0x060013CB RID: 5067 RVA: 0x001383D8 File Offset: 0x001365D8
		public static void ProcessAddMonster(Monster monster)
		{
			try
			{
				DateTime now = TimeUtil.NowDateTime();
				if ((now - MoYuLongXue.MoYuRunTimeData.LastBirthTimePoint).TotalMinutes > 1.0)
				{
					Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.HintMsg, string.Format(GLang.GetLang(4004, new object[0]), monster.MonsterInfo.VSName), true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.HintAndBox, 0, 0, 100, 100);
					MoYuLongXue.MoYuRunTimeData.LastBirthTimePoint = now;
				}
				NPC npc = null;
				int mapCode = 0;
				int leftCount = 0;
				lock (MoYuLongXue.MoYuRunTimeData.Mutex)
				{
					MoYuMonsterInfo moYuInfo;
					if (!MoYuLongXue.MoYuRunTimeData.MonsterXmlDict.TryGetValue(monster.MonsterInfo.ExtensionID, out moYuInfo))
					{
						return;
					}
					mapCode = moYuInfo.MapCode;
					MoYuLongXue.MoYuRunTimeData.BossAttackLogDict[monster.RoleID] = new BossAttackLog
					{
						InjureSum = 0L,
						BHInjure = new Dictionary<long, BHAttackLog>(),
						BHAttackRank = new List<BHAttackLog>()
					};
					leftCount = MoYuLongXue.MoYuRunTimeData.BossAttackLogDict.Count;
					npc = NPCGeneralManager.FindNPC(moYuInfo.MapCode, moYuInfo.NpcID);
				}
				if (null != npc)
				{
					npc.ShowNpc = false;
					GameManager.ClientMgr.NotifyMySelfDelNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
				}
				List<GameClient> clientList = GameManager.ClientMgr.GetMapGameClients(mapCode);
				foreach (GameClient client in clientList)
				{
					client.sendCmd<int>(1907, leftCount, false);
				}
				MoYuLongXue.NotifyBossLogBy9Grid(monster);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("MoYuLongXue :: 处理场景刷新怪物异常。", new object[0]), ex, true);
			}
		}

		// Token: 0x060013CC RID: 5068 RVA: 0x0013866C File Offset: 0x0013686C
		public static void ProcessAttack(GameClient client, Monster monster, int injure)
		{
			try
			{
				lock (MoYuLongXue.MoYuRunTimeData.Mutex)
				{
					if (!MoYuLongXue.MoYuRunTimeData.MonsterXmlDict.ContainsKey(monster.MonsterInfo.ExtensionID))
					{
						return;
					}
					BossAttackLog bossAttackLog;
					if (!MoYuLongXue.MoYuRunTimeData.BossAttackLogDict.TryGetValue(monster.RoleID, out bossAttackLog))
					{
						bossAttackLog = new BossAttackLog
						{
							InjureSum = 0L,
							BHInjure = new Dictionary<long, BHAttackLog>(),
							BHAttackRank = new List<BHAttackLog>()
						};
						MoYuLongXue.MoYuRunTimeData.BossAttackLogDict[monster.RoleID] = bossAttackLog;
					}
					if (client.ClientData.Faction > 0)
					{
						BHAttackLog bhAttackLog;
						if (!bossAttackLog.BHInjure.TryGetValue((long)client.ClientData.Faction, out bhAttackLog))
						{
							bhAttackLog = new BHAttackLog
							{
								BHID = client.ClientData.Faction,
								BHName = client.ClientData.BHName,
								BHInjure = 0L,
								RoleInjure = new Dictionary<int, long>()
							};
							bossAttackLog.BHInjure[(long)client.ClientData.Faction] = bhAttackLog;
							bossAttackLog.BHAttackRank.Add(bhAttackLog);
						}
						if (!bhAttackLog.RoleInjure.ContainsKey(client.ClientData.RoleID))
						{
							bhAttackLog.RoleInjure[client.ClientData.RoleID] = 0L;
						}
						Dictionary<int, long> roleInjure;
						int roleID;
						(roleInjure = bhAttackLog.RoleInjure)[roleID = client.ClientData.RoleID] = roleInjure[roleID] + (long)injure;
						bhAttackLog.BHInjure += (long)injure;
						bossAttackLog.BHAttackRank.Sort(delegate(BHAttackLog x, BHAttackLog y)
						{
							int result;
							if (x.BHInjure > y.BHInjure)
							{
								result = -1;
							}
							else if (x.BHInjure < y.BHInjure)
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
					bossAttackLog.InjureSum += (long)injure;
				}
				MoYuLongXue.NotifyBossLogBy9Grid(monster);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("MoYuLongXue :: 处理攻击boss异常。", new object[0]), ex, true);
			}
		}

		// Token: 0x060013CD RID: 5069 RVA: 0x001388D8 File Offset: 0x00136AD8
		public static bool ProcessMonsterDie(Monster monster)
		{
			try
			{
				NPC npc = null;
				bool ret = true;
				int mapCode = 0;
				int leftCount = 0;
				lock (MoYuLongXue.MoYuRunTimeData.Mutex)
				{
					BossAttackLog attackLog;
					MoYuMonsterInfo monsterInfo;
					if (!MoYuLongXue.MoYuRunTimeData.BossAttackLogDict.TryGetValue(monster.RoleID, out attackLog))
					{
						ret = false;
					}
					else if (!MoYuLongXue.MoYuRunTimeData.MonsterXmlDict.TryGetValue(monster.MonsterInfo.ExtensionID, out monsterInfo))
					{
						ret = false;
					}
					else
					{
						mapCode = monsterInfo.MapCode;
						List<BHAttackLog> bhAttackLogList = attackLog.BHAttackRank;
						int countLimit = Global.GMin(bhAttackLogList.Count, 5);
						for (int i = 0; i < countLimit; i++)
						{
							foreach (KeyValuePair<int, long> role in bhAttackLogList[i].RoleInjure)
							{
								GameClient client = GameManager.ClientMgr.FindClient(role.Key);
								if (null != client)
								{
									if (client.ClientData.MapCode == monsterInfo.MapCode)
									{
										if (role.Value >= (long)monsterInfo.HurtMin)
										{
											GameManager.ClientMgr.ModifyChengJiuPointsValue(client, monsterInfo.Chengjiu, "魔域龙穴boss奖励", true, true);
											GameManager.ClientMgr.ModifyShengWangValue(client, monsterInfo.Shengwang, "魔域龙穴boss奖励", true, true);
										}
									}
								}
							}
						}
						npc = NPCGeneralManager.FindNPC(monsterInfo.MapCode, monsterInfo.NpcID);
					}
					MoYuLongXue.MoYuRunTimeData.BossAttackLogDict.Remove(monster.RoleID);
					leftCount = MoYuLongXue.MoYuRunTimeData.BossAttackLogDict.Count;
				}
				List<GameClient> clientList = GameManager.ClientMgr.GetMapGameClients(mapCode);
				foreach (GameClient client in clientList)
				{
                    client.sendCmd<int>(1907, leftCount, false);
				}
				if (null != npc)
				{
					npc.ShowNpc = true;
					GameManager.ClientMgr.NotifyMySelfNewNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
				}
				return ret;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("MoYuLongXue :: 处理boss被击杀异常。", new object[0]), ex, true);
			}
			return false;
		}

		// Token: 0x060013CE RID: 5070 RVA: 0x00138BE0 File Offset: 0x00136DE0
		public static BossLifeLog GetBossAttackLog(int factionID, int monsterID)
		{
			try
			{
				BossLifeLog ret;
				lock (MoYuLongXue.MoYuRunTimeData.Mutex)
				{
					BossAttackLog boss;
					if (!MoYuLongXue.MoYuRunTimeData.BossAttackLogDict.TryGetValue(monsterID, out boss))
					{
						return null;
					}
					int countLimit = Global.GMin(boss.BHAttackRank.Count, 5);
					ret = new BossLifeLog
					{
						InjureSum = boss.InjureSum,
						BHAttackRank = boss.BHAttackRank.GetRange(0, countLimit)
					};
					boss.BHInjure.TryGetValue((long)factionID, out ret.SelfBHAttack);
				}
				return ret;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("MoYuLongXue :: 处理获取boss攻打记录异常。", new object[0]), ex, true);
			}
			return null;
		}

		// Token: 0x060013CF RID: 5071 RVA: 0x00138CD8 File Offset: 0x00136ED8
		public static void NotifyBossLogBy9Grid(Monster monster)
		{
			lock (MoYuLongXue.MoYuRunTimeData.Mutex)
			{
				List<object> objsList = Global.GetAll9GridObjects(monster);
				for (int i = 0; i < objsList.Count; i++)
				{
					GameClient client = objsList[i] as GameClient;
					if (null != client)
					{
						BossLifeLog bossLifeLog = MoYuLongXue.GetBossAttackLog(client.ClientData.Faction, monster.RoleID);
						if (null != bossLifeLog)
						{
							(objsList[i] as GameClient).sendCmd<BossLifeLog>(1906, bossLifeLog, false);
						}
					}
				}
			}
		}

		// Token: 0x04001D21 RID: 7457
		public static MoYuRunData MoYuRunTimeData = new MoYuRunData();
	}
}
