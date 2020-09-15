using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x020006C2 RID: 1730
	internal class FreshPlayerCopySceneManager
	{
		// Token: 0x060020CE RID: 8398 RVA: 0x001C2380 File Offset: 0x001C0580
		public static void AddFreshPlayerListCopyMap(int nID, CopyMap mapInfo)
		{
			lock (FreshPlayerCopySceneManager.m_FreshPlayerListCopyMaps)
			{
				FreshPlayerCopySceneManager.m_FreshPlayerListCopyMaps.Add(nID, mapInfo);
			}
		}

		// Token: 0x060020CF RID: 8399 RVA: 0x001C23D4 File Offset: 0x001C05D4
		public static void RemoveFreshPlayerListCopyMap(int nID, CopyMap mapInfo)
		{
			lock (FreshPlayerCopySceneManager.m_FreshPlayerListCopyMaps)
			{
				FreshPlayerCopySceneManager.m_FreshPlayerListCopyMaps.Remove(nID);
			}
		}

		// Token: 0x060020D0 RID: 8400 RVA: 0x001C2424 File Offset: 0x001C0624
		public static void HeartBeatFreshPlayerCopyMap()
		{
			long nowTicks = TimeUtil.NOW();
			if (nowTicks - FreshPlayerCopySceneManager.LastHeartBeatTicks >= 1000L)
			{
				FreshPlayerCopySceneManager.LastHeartBeatTicks = nowTicks;
				List<CopyMap> CopyMapList = new List<CopyMap>();
				lock (FreshPlayerCopySceneManager.m_FreshPlayerListCopyMaps)
				{
					CopyMap copyMap = null;
					foreach (CopyMap item in FreshPlayerCopySceneManager.m_FreshPlayerListCopyMaps.Values)
					{
						copyMap = item;
						if (item.FreshPlayerCreateGateFlag == 0)
						{
							FreshPlayerCopySceneManager.CreateGateMonster(item);
						}
						List<GameClient> clientsList = item.GetClientsList();
						if (clientsList != null && clientsList.Count <= 0)
						{
							CopyMapList.Add(item);
						}
					}
					if (null != copyMap)
					{
						GameManager.MonsterZoneMgr.ReloadCopyMapMonsters(copyMap.MapCode, -1);
					}
				}
				for (int i = 0; i < CopyMapList.Count; i++)
				{
					GameManager.CopyMapMgr.ProcessRemoveCopyMap(CopyMapList[i]);
				}
			}
		}

		// Token: 0x060020D1 RID: 8401 RVA: 0x001C257C File Offset: 0x001C077C
		public static void KillMonsterInFreshPlayerScene(GameClient client, Monster monster)
		{
			CopyMap copyMapInfo;
			lock (FreshPlayerCopySceneManager.m_FreshPlayerListCopyMaps)
			{
				if (!FreshPlayerCopySceneManager.m_FreshPlayerListCopyMaps.TryGetValue(client.ClientData.FuBenSeqID, out copyMapInfo) || copyMapInfo == null)
				{
					return;
				}
			}
			if (monster.MonsterInfo.VLevel >= Data.FreshPlayerSceneInfo.NeedKillMonster1Level)
			{
				copyMapInfo.FreshPlayerKillMonsterACount++;
				if (copyMapInfo.FreshPlayerKillMonsterACount >= Data.FreshPlayerSceneInfo.NeedKillMonster1Num)
				{
					string strcmd = string.Format("{0}", client.ClientData.RoleID);
					GameManager.ClientMgr.SendToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strcmd, 525);
				}
			}
			if (monster.MonsterInfo.ExtensionID == Data.FreshPlayerSceneInfo.NeedKillMonster2ID)
			{
				copyMapInfo.FreshPlayerKillMonsterBCount++;
				if (copyMapInfo.FreshPlayerKillMonsterBCount >= Data.FreshPlayerSceneInfo.NeedKillMonster2Num)
				{
					bool canAddMonster = false;
					TaskData taskData = Global.GetTaskData(client, 105);
					if (null != taskData)
					{
						canAddMonster = true;
					}
					if (canAddMonster)
					{
						copyMapInfo.HaveBirthShuiJingGuan = true;
						int monsterID = Data.FreshPlayerSceneInfo.CrystalID;
						string[] sfields = Data.FreshPlayerSceneInfo.CrystalPos.Split(new char[]
						{
							','
						});
						int nPosX = Global.SafeConvertToInt32(sfields[0]);
						int nPosY = Global.SafeConvertToInt32(sfields[1]);
						GameMap gameMap = null;
						if (!GameManager.MapMgr.DictMaps.TryGetValue(copyMapInfo.MapCode, out gameMap))
						{
							return;
						}
						int gridX = gameMap.CorrectWidthPointToGridPoint(nPosX) / gameMap.MapGridWidth;
						int gridY = gameMap.CorrectHeightPointToGridPoint(nPosY) / gameMap.MapGridHeight;
						GameManager.MonsterZoneMgr.AddDynamicMonsters(copyMapInfo.MapCode, monsterID, copyMapInfo.CopyMapID, 1, gridX, gridY, 0, 0, SceneUIClasses.Normal, null, null);
					}
				}
			}
			if (monster.MonsterInfo.ExtensionID == Data.FreshPlayerSceneInfo.GateID)
			{
				FreshPlayerCopySceneManager.CreateMonsterBFreshPlayerScene(copyMapInfo);
			}
			if (monster.MonsterInfo.ExtensionID == Data.FreshPlayerSceneInfo.CrystalID)
			{
				int monsterID = Data.FreshPlayerSceneInfo.DiaoXiangID;
				string[] sfields = Data.FreshPlayerSceneInfo.DiaoXiangPos.Split(new char[]
				{
					','
				});
				int nPosX = Global.SafeConvertToInt32(sfields[0]);
				int nPosY = Global.SafeConvertToInt32(sfields[1]);
				GameMap gameMap = null;
				if (GameManager.MapMgr.DictMaps.TryGetValue(copyMapInfo.MapCode, out gameMap))
				{
					int gridX = gameMap.CorrectWidthPointToGridPoint(nPosX) / gameMap.MapGridWidth;
					int gridY = gameMap.CorrectHeightPointToGridPoint(nPosY) / gameMap.MapGridHeight;
					GameManager.MonsterZoneMgr.AddDynamicMonsters(copyMapInfo.MapCode, monsterID, copyMapInfo.CopyMapID, 1, gridX, gridY, 0, 0, SceneUIClasses.Normal, null, null);
				}
			}
		}

		// Token: 0x060020D2 RID: 8402 RVA: 0x001C28B4 File Offset: 0x001C0AB4
		public static void AddShuiJingGuanCaiMonsters(GameClient client)
		{
			CopyMap copyMapInfo = null;
			lock (FreshPlayerCopySceneManager.m_FreshPlayerListCopyMaps)
			{
				if (!FreshPlayerCopySceneManager.m_FreshPlayerListCopyMaps.TryGetValue(client.ClientData.FuBenSeqID, out copyMapInfo) || copyMapInfo == null)
				{
					return;
				}
			}
			if (!copyMapInfo.HaveBirthShuiJingGuan)
			{
				if (copyMapInfo.FreshPlayerKillMonsterBCount >= Data.FreshPlayerSceneInfo.NeedKillMonster2Num)
				{
					bool canAddMonster = true;
					if (canAddMonster)
					{
						copyMapInfo.HaveBirthShuiJingGuan = true;
						int monsterID = Data.FreshPlayerSceneInfo.CrystalID;
						string[] sfields = Data.FreshPlayerSceneInfo.CrystalPos.Split(new char[]
						{
							','
						});
						int nPosX = Global.SafeConvertToInt32(sfields[0]);
						int nPosY = Global.SafeConvertToInt32(sfields[1]);
						GameMap gameMap = null;
						if (GameManager.MapMgr.DictMaps.TryGetValue(copyMapInfo.MapCode, out gameMap))
						{
							int gridX = gameMap.CorrectWidthPointToGridPoint(nPosX) / gameMap.MapGridWidth;
							int gridY = gameMap.CorrectHeightPointToGridPoint(nPosY) / gameMap.MapGridHeight;
							GameManager.MonsterZoneMgr.AddDynamicMonsters(copyMapInfo.MapCode, monsterID, copyMapInfo.CopyMapID, 1, gridX, gridY, 0, 0, SceneUIClasses.Normal, null, null);
						}
					}
				}
			}
		}

		// Token: 0x060020D3 RID: 8403 RVA: 0x001C2A28 File Offset: 0x001C0C28
		public static void CreateMonsterBFreshPlayerScene(CopyMap copyMapInfo)
		{
			int monsterID = Data.FreshPlayerSceneInfo.NeedKillMonster2ID;
			string[] sfields = Data.FreshPlayerSceneInfo.NeedCreateMonster2Pos.Split(new char[]
			{
				','
			});
			int nPosX = Global.SafeConvertToInt32(sfields[0]);
			int nPosY = Global.SafeConvertToInt32(sfields[1]);
			GameMap gameMap = null;
			if (GameManager.MapMgr.DictMaps.TryGetValue(copyMapInfo.MapCode, out gameMap))
			{
				int gridX = gameMap.CorrectWidthPointToGridPoint(nPosX) / gameMap.MapGridWidth;
				int gridY = gameMap.CorrectHeightPointToGridPoint(nPosY) / gameMap.MapGridHeight;
				int gridNum = gameMap.CorrectPointToGrid(Data.FreshPlayerSceneInfo.NeedCreateMonster2Radius);
				for (int i = 0; i < Data.FreshPlayerSceneInfo.NeedCreateMonster2Num; i++)
				{
					GameManager.MonsterZoneMgr.AddDynamicMonsters(copyMapInfo.MapCode, monsterID, copyMapInfo.CopyMapID, 1, gridX, gridY, gridNum, Data.FreshPlayerSceneInfo.NeedCreateMonster2PursuitRadius, SceneUIClasses.Normal, null, null);
				}
			}
		}

		// Token: 0x060020D4 RID: 8404 RVA: 0x001C2B20 File Offset: 0x001C0D20
		public static void CreateGateMonster(CopyMap copyMapInfo)
		{
			int monsterID = Data.FreshPlayerSceneInfo.GateID;
			string[] sfields = Data.FreshPlayerSceneInfo.GatePos.Split(new char[]
			{
				','
			});
			int nPosX = Global.SafeConvertToInt32(sfields[0]);
			int nPosY = Global.SafeConvertToInt32(sfields[1]);
			GameMap gameMap = null;
			if (GameManager.MapMgr.DictMaps.TryGetValue(copyMapInfo.MapCode, out gameMap))
			{
				int gridX = gameMap.CorrectWidthPointToGridPoint(nPosX) / gameMap.MapGridWidth;
				int gridY = gameMap.CorrectHeightPointToGridPoint(nPosY) / gameMap.MapGridHeight;
				GameManager.MonsterZoneMgr.AddDynamicMonsters(copyMapInfo.MapCode, monsterID, copyMapInfo.CopyMapID, 1, gridX, gridY, 0, 0, SceneUIClasses.Normal, null, null);
				copyMapInfo.FreshPlayerCreateGateFlag = 1;
			}
		}

		// Token: 0x0400368E RID: 13966
		public static Dictionary<int, CopyMap> m_FreshPlayerListCopyMaps = new Dictionary<int, CopyMap>();

		// Token: 0x0400368F RID: 13967
		private static long LastHeartBeatTicks = 0L;
	}
}
