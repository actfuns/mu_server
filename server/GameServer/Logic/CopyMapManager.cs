using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GameServer.Core.Executor;
using GameServer.Logic.Copy;
using GameServer.Logic.Marriage.CoupleArena;
using GameServer.Logic.MoRi;
using GameServer.Logic.WanMota;
using Server.Data;
using Server.Protocol;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x02000612 RID: 1554
	public class CopyMapManager
	{
		// Token: 0x06001F22 RID: 7970 RVA: 0x001AE120 File Offset: 0x001AC320
		public int GetNewCopyMapID()
		{
			int id = 1;
			lock (this)
			{
				id = this.BaseCopyMapID++;
			}
			return id;
		}

		// Token: 0x06001F23 RID: 7971 RVA: 0x001AE180 File Offset: 0x001AC380
		private void AddCopyID(int fuBenSeqID, int mapCode, int copyMapID)
		{
			string key = string.Format("{0}_{1}", fuBenSeqID, mapCode);
			lock (this.FuBenSeqID2CopyIDDict)
			{
				this.FuBenSeqID2CopyIDDict[key] = copyMapID;
			}
		}

		// Token: 0x06001F24 RID: 7972 RVA: 0x001AE1EC File Offset: 0x001AC3EC
		public void RemoveCopyID(int fuBenSeqID, int mapCode)
		{
			string key = string.Format("{0}_{1}", fuBenSeqID, mapCode);
			lock (this.FuBenSeqID2CopyIDDict)
			{
				this.FuBenSeqID2CopyIDDict.Remove(key);
			}
		}

		// Token: 0x06001F25 RID: 7973 RVA: 0x001AE258 File Offset: 0x001AC458
		public int FindCopyID(int fuBenSeqID, int mapCode)
		{
			int copyMapID = -1;
			string key = string.Format("{0}_{1}", fuBenSeqID, mapCode);
			lock (this.FuBenSeqID2CopyIDDict)
			{
				if (!this.FuBenSeqID2CopyIDDict.TryGetValue(key, out copyMapID))
				{
					copyMapID = -1;
				}
			}
			return copyMapID;
		}

		// Token: 0x06001F26 RID: 7974 RVA: 0x001AE2D8 File Offset: 0x001AC4D8
		private void AddMonsterState(int fuBenSeqID, int mapCode, int monsterState)
		{
			string key = string.Format("{0}_{1}", fuBenSeqID, mapCode);
			lock (this.FuBenSeqID2MonsterStateDict)
			{
				this.FuBenSeqID2MonsterStateDict[key] = monsterState;
			}
		}

		// Token: 0x06001F27 RID: 7975 RVA: 0x001AE344 File Offset: 0x001AC544
		private int FindMonsterState(int fuBenSeqID, int mapCode)
		{
			int monsterState = 0;
			string key = string.Format("{0}_{1}", fuBenSeqID, mapCode);
			lock (this.FuBenSeqID2MonsterStateDict)
			{
				if (!this.FuBenSeqID2MonsterStateDict.TryGetValue(key, out monsterState))
				{
					monsterState = 0;
				}
			}
			return monsterState;
		}

		// Token: 0x06001F28 RID: 7976 RVA: 0x001AE3C4 File Offset: 0x001AC5C4
		public void AddAwardState(int roleID, int fuBenSeqID, int mapCode, int awardState)
		{
			string key = string.Format("{0}_{1}_{2}", roleID, fuBenSeqID, mapCode);
			lock (this.RoleIDFuBenSeqID2AwardStateDict)
			{
				this.RoleIDFuBenSeqID2AwardStateDict[key] = awardState;
			}
		}

		// Token: 0x06001F29 RID: 7977 RVA: 0x001AE438 File Offset: 0x001AC638
		public int FindAwardState(int roleID, int fuBenSeqID, int mapCode)
		{
			int awardState = 0;
			string key = string.Format("{0}_{1}_{2}", roleID, fuBenSeqID, mapCode);
			lock (this.RoleIDFuBenSeqID2AwardStateDict)
			{
				if (!this.RoleIDFuBenSeqID2AwardStateDict.TryGetValue(key, out awardState))
				{
					awardState = 0;
				}
			}
			return awardState;
		}

		// Token: 0x06001F2A RID: 7978 RVA: 0x001AE4C0 File Offset: 0x001AC6C0
		public CopyMap FindCopyMap(int mapCode, int fuBenSeqID)
		{
			CopyMap copyMap = null;
			int copyMapID = this.FindCopyID(fuBenSeqID, mapCode);
			if (copyMapID > 0)
			{
				copyMap = this.FindCopyMap(copyMapID);
			}
			return copyMap;
		}

		// Token: 0x06001F2B RID: 7979 RVA: 0x001AE4F8 File Offset: 0x001AC6F8
		public CopyMap GetCopyMap(GameClient client, MapTypes mapType)
		{
			CopyMap copyMap = null;
			int totalMonsterNum = GameManager.MonsterZoneMgr.GetMapTotalMonsterNum(client.ClientData.MapCode, MonsterTypes.None, true);
			int totalNormalNum = GameManager.MonsterZoneMgr.GetMapTotalMonsterNum(client.ClientData.MapCode, MonsterTypes.Noraml, true);
			int totalBossNum = totalMonsterNum - totalNormalNum;
			SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
			lock (this)
			{
				int copyMapID = this.FindCopyID(client.ClientData.FuBenSeqID, client.ClientData.MapCode);
				if (copyMapID > 0)
				{
					copyMap = this.FindCopyMap(copyMapID);
				}
				int monsterState = this.FindMonsterState(client.ClientData.FuBenSeqID, client.ClientData.MapCode);
				if (null == copyMap)
				{
					copyMap = new CopyMap
					{
						CopyMapID = this.GetNewCopyMapID(),
						FuBenSeqID = client.ClientData.FuBenSeqID,
						MapCode = client.ClientData.MapCode,
						FubenMapID = client.ClientData.FuBenID,
						CopyMapType = mapType,
						IsInitMonster = (monsterState > 0),
						InitTicks = TimeUtil.NOW(),
						TotalNormalNum = totalNormalNum,
						TotalBossNum = totalBossNum,
						bStoryCopyMapFinishStatus = false
					};
					if (copyMap.FubenMapID < 0)
					{
						copyMap.FubenMapID = FuBenManager.FindFuBenIDByMapCode(client.ClientData.MapCode);
						client.ClientData.FuBenID = copyMap.FubenMapID;
					}
					this.AddCopyID(client.ClientData.FuBenSeqID, client.ClientData.MapCode, copyMap.CopyMapID);
					this.AddCopyMap(copyMap);
					this.AddTeamCopyMap(copyMap);
					if (!copyMap.IsInitMonster)
					{
						copyMap.IsInitMonster = true;
						GameManager.MonsterZoneMgr.AddCopyMapMonsters(client.ClientData.MapCode, copyMap.CopyMapID);
					}
				}
				copyMap.AddGameClient(client);
				if (client.ClientData.MapCode == 6090)
				{
					copyMap.FreshPlayerCreateGateFlag = 0;
					FreshPlayerCopySceneManager.AddFreshPlayerListCopyMap(client.ClientData.FuBenSeqID, copyMap);
				}
				else if (Global.IsInExperienceCopyScene(client.ClientData.MapCode))
				{
					ExperienceCopySceneManager.AddExperienceListCopyMap(client.ClientData.FuBenSeqID, copyMap);
				}
				else if (client.ClientData.MapCode == 5100)
				{
					GlodCopySceneManager.AddGlodCopySceneList(client.ClientData.FuBenSeqID, copyMap);
				}
				else if (client.ClientData.MapCode == EMoLaiXiCopySceneManager.EMoLaiXiCopySceneMapCode)
				{
					EMoLaiXiCopySceneManager.AddEMoLaiXiCopySceneList(client.ClientData.FuBenSeqID, copyMap);
				}
				else if (GameManager.BloodCastleCopySceneMgr.IsBloodCastleCopyScene2(client.ClientData.MapCode))
				{
					GameManager.BloodCastleCopySceneMgr.AddBloodCastleCopyScenes(copyMap.FuBenSeqID, copyMap.FubenMapID, client.ClientData.MapCode, copyMap);
				}
				else if (GameManager.DaimonSquareCopySceneMgr.IsDaimonSquareCopyScene2(client.ClientData.MapCode))
				{
					GameManager.DaimonSquareCopySceneMgr.AddDaimonSquareCopyScenes(copyMap.FuBenSeqID, copyMap.FubenMapID, client.ClientData.MapCode, copyMap);
				}
				else if (ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(client.ClientData.MapCode))
				{
					ZhuanShengShiLian.AddCopyScenes(copyMap.FuBenSeqID, copyMap.FubenMapID, client.ClientData.MapCode, copyMap);
				}
				else if (Global.IsStoryCopyMapScene(client.ClientData.MapCode))
				{
					SystemXmlItem systemFuBenItem = null;
					if (GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(copyMap.FubenMapID, out systemFuBenItem) && systemFuBenItem != null)
					{
						int nBossID = systemFuBenItem.GetIntValue("BossID", -1);
						int nNum = GameManager.MonsterZoneMgr.GetMapMonsterNum(client.ClientData.MapCode, nBossID);
						if (nNum == 0)
						{
							Global.NotifyClientStoryCopyMapInfo(copyMap.CopyMapID, 1);
						}
						else
						{
							Global.NotifyClientStoryCopyMapInfo(copyMap.CopyMapID, 2);
						}
					}
				}
				if (client.ClientSocket.IsKuaFuLogin)
				{
					FuBenManager.AddFuBenSeqID(client.ClientData.RoleID, copyMap.FuBenSeqID, 0, copyMap.FubenMapID);
				}
				switch (sceneType)
				{
				case SceneUIClasses.HuanYingSiYuan:
					HuanYingSiYuanManager.getInstance().AddHuanYingSiYuanCopyScenes(client, copyMap);
					goto IL_6B5;
				case SceneUIClasses.TianTi:
					TianTiManager.getInstance().AddTianTiCopyScenes(client, copyMap, sceneType);
					goto IL_6B5;
				case SceneUIClasses.YongZheZhanChang:
					YongZheZhanChangManager.getInstance().AddCopyScenes(client, copyMap, sceneType);
					goto IL_6B5;
				case SceneUIClasses.ElementWar:
					ElementWarManager.getInstance().AddCopyScene(client, copyMap);
					goto IL_6B5;
				case SceneUIClasses.MoRiJudge:
					SingletonTemplate<MoRiJudgeManager>.Instance().AddCopyScene(client, copyMap, sceneType);
					goto IL_6B5;
				case SceneUIClasses.KuaFuBoss:
					KuaFuBossManager.getInstance().AddCopyScenes(client, copyMap, sceneType);
					goto IL_6B5;
				case SceneUIClasses.CopyWolf:
					CopyWolfManager.getInstance().AddCopyScene(client, copyMap);
					goto IL_6B5;
				case SceneUIClasses.LangHunLingYu:
					LangHunLingYuManager.getInstance().AddCopyScenes(client, copyMap, sceneType);
					goto IL_6B5;
				case SceneUIClasses.KFZhengBa:
					SingletonTemplate<ZhengBaManager>.Instance().AddCopyScenes(client, copyMap, sceneType);
					goto IL_6B5;
				case SceneUIClasses.CoupleArena:
					SingletonTemplate<CoupleArenaManager>.Instance().AddCopyScenes(client, copyMap, sceneType);
					goto IL_6B5;
				case SceneUIClasses.KingOfBattle:
					KingOfBattleManager.getInstance().AddCopyScenes(client, copyMap, sceneType);
					goto IL_6B5;
				case SceneUIClasses.KarenWest:
					KarenBattleManager_MapWest.getInstance().AddCopyScenes(client, copyMap, sceneType);
					goto IL_6B5;
				case SceneUIClasses.KarenEast:
					KarenBattleManager_MapEast.getInstance().AddCopyScenes(client, copyMap, sceneType);
					goto IL_6B5;
				case SceneUIClasses.BangHuiMatch:
					BangHuiMatchManager.getInstance().AddCopyScenes(client, copyMap, sceneType);
					goto IL_6B5;
				case SceneUIClasses.KuaFuLueDuo:
					KuaFuLueDuoManager.getInstance().AddCopyScenes(client, copyMap, sceneType);
					goto IL_6B5;
				case SceneUIClasses.CompBattle:
					CompBattleManager.getInstance().AddCopyScenes(client, copyMap, sceneType);
					goto IL_6B5;
				case SceneUIClasses.CompMine:
					CompMineManager.getInstance().AddCopyScenes(client, copyMap, sceneType);
					goto IL_6B5;
				case SceneUIClasses.ZorkBattle:
					ZorkBattleManager.getInstance().AddCopyScenes(client, copyMap, sceneType);
					goto IL_6B5;
				}
				if (SingletonTemplate<CopyTeamManager>.Instance().IsKuaFuCopy(copyMap.FubenMapID))
				{
					SingletonTemplate<CopyTeamManager>.Instance().AddCopyScenes(client, copyMap, sceneType);
				}
				IL_6B5:;
			}
			GlobalServiceManager.AddCopyScenes(client, copyMap, sceneType);
			return copyMap;
		}

		// Token: 0x06001F2C RID: 7980 RVA: 0x001AEC00 File Offset: 0x001ACE00
		private void AddCopyMap(CopyMap copyMap)
		{
			lock (this._ListCopyMaps)
			{
				this._ListCopyMaps.Add(copyMap);
			}
			lock (this._DictCopyMaps)
			{
				this._DictCopyMaps.Add(copyMap.CopyMapID, copyMap);
			}
		}

		// Token: 0x06001F2D RID: 7981 RVA: 0x001AEC9C File Offset: 0x001ACE9C
		public void RemoveCopyMap(CopyMap copyMap)
		{
			lock (this._ListCopyMaps)
			{
				this._ListCopyMaps.Remove(copyMap);
			}
			lock (this._DictCopyMaps)
			{
				this._DictCopyMaps.Remove(copyMap.CopyMapID);
			}
		}

		// Token: 0x06001F2E RID: 7982 RVA: 0x001AED38 File Offset: 0x001ACF38
		public CopyMap FindCopyMap(int copyMapID)
		{
			CopyMap copyMap = null;
			lock (this._DictCopyMaps)
			{
				this._DictCopyMaps.TryGetValue(copyMapID, out copyMap);
			}
			return copyMap;
		}

		// Token: 0x06001F2F RID: 7983 RVA: 0x001AED98 File Offset: 0x001ACF98
		public CopyMap GetNextCopyMap(int index)
		{
			CopyMap copyMap = null;
			lock (this._ListCopyMaps)
			{
				if (index < this._ListCopyMaps.Count)
				{
					copyMap = this._ListCopyMaps[index];
				}
			}
			return copyMap;
		}

		// Token: 0x06001F30 RID: 7984 RVA: 0x001AEE0C File Offset: 0x001AD00C
		public int GetCopyMapCount()
		{
			int count = 0;
			lock (this._ListCopyMaps)
			{
				count = this._ListCopyMaps.Count;
			}
			return count;
		}

		// Token: 0x06001F31 RID: 7985 RVA: 0x001AEE68 File Offset: 0x001AD068
		private bool CopyMapOverTime(CopyMap copyMap, long nowTicks, List<GameClient> clientsList)
		{
			int fuBenID = FuBenManager.FindFuBenIDByMapCode(copyMap.MapCode);
			FuBenMapItem fuBenMapItem = FuBenManager.FindMapCodeByFuBenID(fuBenID, copyMap.MapCode);
			bool result;
			if (null == fuBenMapItem)
			{
				result = false;
			}
			else
			{
				FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(copyMap.FuBenSeqID);
				if (null == fuBenInfoItem)
				{
				}
				if (GameManager.GuildCopyMapMgr.IsGuildCopyMap(fuBenID))
				{
					if (clientsList == null || 0 == clientsList.Count)
					{
						if (nowTicks - copyMap.GetLastLeaveClientTicks() >= 600000L)
						{
							return true;
						}
					}
				}
				if (fuBenMapItem.MaxTime <= 0)
				{
					result = false;
				}
				else if (nowTicks - copyMap.InitTicks < (long)fuBenMapItem.MaxTime * 60L * 1000L)
				{
					result = false;
				}
				else
				{
					if (null != clientsList)
					{
						int toMapCode = GameManager.MainMapCode;
						GameMap gameMap = null;
						if (GameManager.MapMgr.DictMaps.TryGetValue(toMapCode, out gameMap))
						{
							for (int i = 0; i < clientsList.Count; i++)
							{
								if (copyMap.MapCode == 6090)
								{
									int nID = 543;
									string strcmd = string.Format("{0}", clientsList[i].ClientData.RoleID);
									TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strcmd, nID);
									Global._TCPManager.MySocketListener.SendData(clientsList[i].ClientSocket, tcpOutPacket, true);
								}
								else
								{
									GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, clientsList[i], toMapCode, -1, -1, -1, 0);
									GameManager.LuaMgr.Error(clientsList[i], GLang.GetLang(99, new object[0]), 0);
								}
							}
						}
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06001F32 RID: 7986 RVA: 0x001AF084 File Offset: 0x001AD284
		private bool CanRemoveCopyMap(CopyMap copyMap, long nowTicks)
		{
			bool result;
			if (copyMap.bNeedRemove)
			{
				result = true;
			}
			else
			{
				List<GameClient> clientsList = copyMap.GetClientsList();
				long maxKeepAliveTicks = 180000L;
				if (copyMap.IsKuaFuCopy)
				{
					if (copyMap.CanRemoveTicks > 0L)
					{
						return nowTicks > copyMap.CanRemoveTicks;
					}
					maxKeepAliveTicks = 30000L;
				}
				if (this.CopyMapOverTime(copyMap, nowTicks, clientsList))
				{
					result = true;
				}
				else if (clientsList != null && clientsList.Count > 0)
				{
					result = false;
				}
				else
				{
					long lastLeaveClientTicks = copyMap.GetLastLeaveClientTicks();
					result = (nowTicks - lastLeaveClientTicks >= maxKeepAliveTicks);
				}
			}
			return result;
		}

		// Token: 0x06001F33 RID: 7987 RVA: 0x001AF154 File Offset: 0x001AD354
		public void ProcessRemoveCopyMap(CopyMap copyMap)
		{
			int monsterCount = Global.GetLeftMonsterByCopyMapID(copyMap.CopyMapID);
			int monsterState = 0;
			if (copyMap.IsInitMonster)
			{
				monsterState = ((monsterCount <= 0) ? 1 : 0);
			}
			this.AddMonsterState(copyMap.FuBenSeqID, copyMap.MapCode, monsterState);
			GameManager.MonsterZoneMgr.DestroyCopyMapMonsters(copyMap.MapCode, copyMap.CopyMapID);
			this.RemoveCopyID(copyMap.FuBenSeqID, copyMap.MapCode);
			this.RemoveCopyMap(copyMap);
			this.RemoveTeamCopyMap(copyMap);
			SceneUIClasses sceneType = Global.GetMapSceneType(copyMap.MapCode);
			if (copyMap.MapCode == 6090)
			{
				FreshPlayerCopySceneManager.RemoveFreshPlayerListCopyMap(copyMap.FuBenSeqID, copyMap);
			}
			if (Global.IsInExperienceCopyScene(copyMap.MapCode))
			{
				ExperienceCopySceneManager.RemoveExperienceListCopyMap(copyMap.FuBenSeqID);
			}
			if (copyMap.MapCode == 5100)
			{
				GlodCopySceneManager.RemoveGlodCopySceneList(copyMap.FuBenSeqID);
			}
			else if (copyMap.MapCode == EMoLaiXiCopySceneManager.EMoLaiXiCopySceneMapCode)
			{
				EMoLaiXiCopySceneManager.RemoveEMoLaiXiCopySceneList(copyMap.FuBenSeqID, copyMap.CopyMapID);
			}
			else if (GameManager.BloodCastleCopySceneMgr.IsBloodCastleCopyScene(copyMap.FubenMapID))
			{
				GameManager.BloodCastleCopySceneMgr.RemoveBloodCastleListCopyScenes(copyMap, copyMap.FuBenSeqID, copyMap.FubenMapID);
			}
			else if (GameManager.DaimonSquareCopySceneMgr.IsDaimonSquareCopyScene(copyMap.FubenMapID))
			{
				GameManager.DaimonSquareCopySceneMgr.RemoveDaimonSquareListCopyScenes(copyMap, copyMap.FuBenSeqID, copyMap.FubenMapID);
			}
			else if (ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(copyMap.MapCode))
			{
				ZhuanShengShiLian.RemoveCopyScenes(copyMap, copyMap.FuBenSeqID, copyMap.FubenMapID);
			}
			switch (sceneType)
			{
			case SceneUIClasses.HuanYingSiYuan:
				HuanYingSiYuanManager.getInstance().RemoveHuanYingSiYuanListCopyScenes(copyMap);
				goto IL_3A0;
			case SceneUIClasses.TianTi:
				TianTiManager.getInstance().RemoveTianTiCopyScene(copyMap, sceneType);
				goto IL_3A0;
			case SceneUIClasses.YongZheZhanChang:
				YongZheZhanChangManager.getInstance().RemoveCopyScene(copyMap, sceneType);
				goto IL_3A0;
			case SceneUIClasses.ElementWar:
				ElementWarManager.getInstance().RemoveCopyScene(copyMap);
				goto IL_3A0;
			case SceneUIClasses.MoRiJudge:
				SingletonTemplate<MoRiJudgeManager>.Instance().DelCopyScene(copyMap);
				goto IL_3A0;
			case SceneUIClasses.KuaFuBoss:
				KuaFuBossManager.getInstance().RemoveCopyScene(copyMap, sceneType);
				goto IL_3A0;
			case SceneUIClasses.CopyWolf:
				CopyWolfManager.getInstance().RemoveCopyScene(copyMap);
				goto IL_3A0;
			case SceneUIClasses.LangHunLingYu:
				LangHunLingYuManager.getInstance().RemoveCopyScene(copyMap, sceneType);
				goto IL_3A0;
			case SceneUIClasses.KFZhengBa:
				SingletonTemplate<ZhengBaManager>.Instance().RemoveCopyScene(copyMap, sceneType);
				goto IL_3A0;
			case SceneUIClasses.CoupleArena:
				SingletonTemplate<CoupleArenaManager>.Instance().RemoveCopyScene(copyMap, sceneType);
				goto IL_3A0;
			case SceneUIClasses.KingOfBattle:
				KingOfBattleManager.getInstance().RemoveCopyScene(copyMap, sceneType);
				goto IL_3A0;
			case SceneUIClasses.KarenWest:
				KarenBattleManager_MapWest.getInstance().RemoveCopyScene(copyMap, sceneType);
				goto IL_3A0;
			case SceneUIClasses.KarenEast:
				KarenBattleManager_MapEast.getInstance().RemoveCopyScene(copyMap, sceneType);
				goto IL_3A0;
			case SceneUIClasses.BangHuiMatch:
				BangHuiMatchManager.getInstance().RemoveCopyScene(copyMap, sceneType);
				goto IL_3A0;
			case SceneUIClasses.CompBattle:
				CompBattleManager.getInstance().RemoveCopyScene(copyMap, sceneType);
				goto IL_3A0;
			case SceneUIClasses.CompMine:
				CompMineManager.getInstance().RemoveCopyScene(copyMap, sceneType);
				goto IL_3A0;
			case SceneUIClasses.ZorkBattle:
				ZorkBattleManager.getInstance().RemoveCopyScene(copyMap, sceneType);
				goto IL_3A0;
			}
			if (SingletonTemplate<CopyTeamManager>.Instance().IsKuaFuCopy(copyMap.FubenMapID))
			{
				SingletonTemplate<CopyTeamManager>.Instance().RemoveCopyScene(copyMap, sceneType);
			}
			IL_3A0:
			GlobalServiceManager.RemoveCopyScene(copyMap, sceneType);
			SingletonTemplate<CopyTeamManager>.Instance().OnCopyRemove(copyMap.FuBenSeqID);
			FuBenManager.RemoveFuBenInfoBySeqID(copyMap.FuBenSeqID);
			copyMap.bNeedRemove = false;
		}

		// Token: 0x06001F34 RID: 7988 RVA: 0x001AF530 File Offset: 0x001AD730
		public void ProcessRemoveCopyMaps(List<CopyMap> listCopyMap, int FuBenSeqID, int FubenMapID)
		{
			foreach (CopyMap copyMap in listCopyMap)
			{
				int monsterCount = Global.GetLeftMonsterByCopyMapID(copyMap.CopyMapID);
				int monsterState = 0;
				if (copyMap.IsInitMonster)
				{
					monsterState = ((monsterCount <= 0) ? 1 : 0);
				}
				this.AddMonsterState(copyMap.FuBenSeqID, copyMap.MapCode, monsterState);
				GameManager.MonsterZoneMgr.DestroyCopyMapMonsters(copyMap.MapCode, copyMap.CopyMapID);
				this.RemoveCopyID(copyMap.FuBenSeqID, copyMap.MapCode);
				this.RemoveCopyMap(copyMap);
				this.RemoveTeamCopyMap(copyMap);
				SingletonTemplate<CopyTeamManager>.Instance().OnCopyRemove(copyMap.FuBenSeqID);
			}
			if (LuoLanFaZhenCopySceneManager.IsLuoLanFaZhen(FubenMapID))
			{
				LuoLanFaZhenCopySceneManager.OnFubenOver(FuBenSeqID);
			}
			FuBenManager.RemoveFuBenInfoBySeqID(FuBenSeqID);
		}

		// Token: 0x06001F35 RID: 7989 RVA: 0x001AF630 File Offset: 0x001AD830
		public void ProcessEndCopyMap()
		{
			long nowTicks = TimeUtil.NOW();
			int index = 0;
			CopyMap copyMap = this.GetNextCopyMap(index);
			int nDelCount = 0;
			while (null != copyMap)
			{
				if (LuoLanFaZhenCopySceneManager.IsLuoLanFaZhen(copyMap.FubenMapID))
				{
					List<CopyMap> listMultiCopyMap = null;
					bool bCanRemoveFuBen = true;
					List<int> mapCodeList = FuBenManager.FindMapCodeListByFuBenID(copyMap.FubenMapID);
					if (null != mapCodeList)
					{
						foreach (int mapcode in mapCodeList)
						{
							int copyMapID = this.FindCopyID(copyMap.FuBenSeqID, mapcode);
							if (copyMapID >= 0)
							{
								CopyMap child_map = this.FindCopyMap(copyMapID);
								if (null != child_map)
								{
									if (!this.CanRemoveCopyMap(child_map, nowTicks))
									{
										bCanRemoveFuBen = false;
										break;
									}
									if (null == listMultiCopyMap)
									{
										listMultiCopyMap = new List<CopyMap>();
									}
									listMultiCopyMap.Add(child_map);
								}
							}
						}
					}
					if (!bCanRemoveFuBen)
					{
						index++;
						copyMap = this.GetNextCopyMap(index);
						continue;
					}
					if (bCanRemoveFuBen && listMultiCopyMap != null && listMultiCopyMap.Count > 0)
					{
						this.ProcessRemoveCopyMaps(listMultiCopyMap, copyMap.FuBenSeqID, copyMap.FubenMapID);
						break;
					}
				}
				if (this.CanRemoveCopyMap(copyMap, nowTicks))
				{
					this.ProcessRemoveCopyMap(copyMap);
					GuildCopyMap mapData = GameManager.GuildCopyMapMgr.FindGuildCopyMapBySeqID(copyMap.FuBenSeqID);
					if (null != mapData)
					{
						GameManager.GuildCopyMapMgr.RemoveGuildCopyMap(mapData.GuildID);
					}
					nDelCount++;
					if (nDelCount >= GameManager.OnceDestroyCopyMapNum)
					{
						break;
					}
				}
				else
				{
					index++;
				}
				copyMap = this.GetNextCopyMap(index);
			}
		}

		// Token: 0x06001F36 RID: 7990 RVA: 0x001AF814 File Offset: 0x001ADA14
		public void ProcessEndGuildCopyMapFlag()
		{
			if (GameManager.GuildCopyMapMgr.IsPrepareResetTime())
			{
				GameManager.GuildCopyMapMgr.ProcessEndFlag = true;
			}
		}

		// Token: 0x06001F37 RID: 7991 RVA: 0x001AF840 File Offset: 0x001ADA40
		public void ProcessEndGuildCopyMap(long ticks)
		{
			if (ticks - GameManager.GuildCopyMapMgr.lastProcessEndTicks >= 1000L)
			{
				GameManager.GuildCopyMapMgr.lastProcessEndTicks = ticks;
				if (!GameManager.GuildCopyMapMgr.IsPrepareResetTime())
				{
					if (GameManager.GuildCopyMapMgr.ProcessEndFlag)
					{
						GuildCopyMap mapData = GameManager.GuildCopyMapMgr.FindActiveGuildCopyMap();
						if (null == mapData)
						{
							GameManager.GuildCopyMapMgr.ProcessEndFlag = false;
						}
						else
						{
							GameManager.GuildCopyMapMgr.RemoveGuildCopyMap(mapData.GuildID);
							this.CloseGuildCopyMap(mapData.SeqID, mapData.MapCode);
						}
					}
				}
			}
		}

		// Token: 0x06001F38 RID: 7992 RVA: 0x001AF8E8 File Offset: 0x001ADAE8
		public void CloseGuildCopyMap(int fuBenSeqID, int mapCode)
		{
			int copyMapID = this.FindCopyID(fuBenSeqID, mapCode);
			if (copyMapID > 0)
			{
				CopyMap copyMap = this.FindCopyMap(copyMapID);
				if (null != copyMap)
				{
					if (GameManager.GuildCopyMapMgr.IsGuildCopyMap(copyMap.FubenMapID))
					{
						this.RemoveCopyMapAllPlayer(copyMap);
						this.ProcessRemoveCopyMap(copyMap);
					}
				}
			}
		}

		// Token: 0x06001F39 RID: 7993 RVA: 0x001AF944 File Offset: 0x001ADB44
		public void RemoveCopyMapAllPlayer(CopyMap copyMap)
		{
			List<GameClient> objsList = copyMap.GetClientsList();
			if (objsList != null)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					GameClient c = objsList[i];
					if (c != null)
					{
						if (c.ClientData.MapCode == copyMap.MapCode)
						{
							int toMapCode = GameManager.MainMapCode;
							int toPosX = -1;
							int toPosY = -1;
							if (MapTypes.Normal == Global.GetMapType(c.ClientData.LastMapCode))
							{
								if (GameManager.BattleMgr.BattleMapCode != c.ClientData.LastMapCode || GameManager.ArenaBattleMgr.BattleMapCode != c.ClientData.LastMapCode)
								{
									toMapCode = c.ClientData.LastMapCode;
									toPosX = c.ClientData.LastPosX;
									toPosY = c.ClientData.LastPosY;
								}
							}
							GameMap gameMap = null;
							if (GameManager.MapMgr.DictMaps.TryGetValue(toMapCode, out gameMap))
							{
								c.ClientData.bIsInAngelTempleMap = false;
								GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, c, toMapCode, toPosX, toPosY, -1, 0);
							}
						}
					}
				}
			}
		}

		// Token: 0x06001F3A RID: 7994 RVA: 0x001AFAA0 File Offset: 0x001ADCA0
		public void AddTeamCopyMap(CopyMap copyMap)
		{
			if (SingletonTemplate<CopyTeamManager>.Instance().NeedRecordDamageInfoFuBenID(copyMap.FubenMapID))
			{
				lock (this._RoleDamageDict_TeamCopyMapDict_Mutex)
				{
					if (!this.TeamCopyMapDict.Contains(copyMap))
					{
						this.RoleDamageDict.Add(copyMap.CopyMapID, new Dictionary<int, RoleDamage>());
						this.TeamCopyMapDict.Add(copyMap);
					}
				}
			}
		}

		// Token: 0x06001F3B RID: 7995 RVA: 0x001AFB34 File Offset: 0x001ADD34
		public void RemoveTeamCopyMap(CopyMap copyMap)
		{
			if (SingletonTemplate<CopyTeamManager>.Instance().NeedRecordDamageInfoFuBenID(copyMap.FubenMapID))
			{
				lock (this._RoleDamageDict_TeamCopyMapDict_Mutex)
				{
					this.RoleDamageDict.Remove(copyMap.CopyMapID);
					this.TeamCopyMapDict.Remove(copyMap);
				}
			}
		}

		// Token: 0x06001F3C RID: 7996 RVA: 0x001AFBB4 File Offset: 0x001ADDB4
		public List<RoleDamage> GetCopyMapAllRoleDamages(int copyMapID)
		{
			List<RoleDamage> roleDamages = null;
			lock (this._RoleDamageDict_TeamCopyMapDict_Mutex)
			{
				Dictionary<int, RoleDamage> dict;
				if (this.RoleDamageDict.TryGetValue(copyMapID, out dict))
				{
					roleDamages = new List<RoleDamage>();
					foreach (RoleDamage rd in dict.Values)
					{
						roleDamages.Add(rd);
					}
				}
			}
			return roleDamages;
		}

		// Token: 0x06001F3D RID: 7997 RVA: 0x001AFC74 File Offset: 0x001ADE74
		public void BroadcastCopyMapDamageInfo(CopyMap copyMap, int sendtoRoleId = -1)
		{
			lock (this._RoleDamageDict_TeamCopyMapDict_Mutex)
			{
				Dictionary<int, RoleDamage> dict;
				if (this.RoleDamageDict.TryGetValue(copyMap.CopyMapID, out dict))
				{
					List<GameClient> clientList = copyMap.GetClientsList();
					foreach (GameClient client in clientList)
					{
						long damage = Interlocked.Exchange(ref client.SumDamageForCopyTeam, 0L);
						int roleID = client.ClientData.RoleID;
						RoleDamage rd;
						if (dict.TryGetValue(roleID, out rd))
						{
							rd.Damage += damage;
						}
						else
						{
							dict.Add(roleID, new RoleDamage(roleID, damage, Global.FormatRoleName(client, client.ClientData.RoleName), new int[0]));
						}
					}
					List<RoleDamage> roleDamages = this.GetCopyMapAllRoleDamages(copyMap.CopyMapID);
					foreach (GameClient client in clientList)
					{
						if (sendtoRoleId < 0 || sendtoRoleId == client.ClientData.RoleID)
						{
							client.sendCmd<List<RoleDamage>>(626, roleDamages, false);
						}
					}
				}
			}
		}

		// Token: 0x06001F3E RID: 7998 RVA: 0x001AFE54 File Offset: 0x001AE054
		public void SendCopyMapMaxDamageInfo(GameClient client, CopyMap copyMap, int MaxCount)
		{
			if (MaxCount > 0)
			{
				lock (this._RoleDamageDict_TeamCopyMapDict_Mutex)
				{
					Dictionary<int, RoleDamage> dict;
					if (this.RoleDamageDict.TryGetValue(copyMap.CopyMapID, out dict))
					{
						List<GameClient> clientList = copyMap.GetClientsList();
						foreach (GameClient gc in clientList)
						{
							long damage = Interlocked.Exchange(ref gc.SumDamageForCopyTeam, 0L);
							int roleID = gc.ClientData.RoleID;
							RoleDamage rd;
							if (dict.TryGetValue(roleID, out rd))
							{
								rd.Damage += damage;
							}
							else
							{
								dict.Add(roleID, new RoleDamage(roleID, damage, Global.FormatRoleName(gc, gc.ClientData.RoleName), new int[0]));
							}
						}
						List<RoleDamage> roleDamages = this.GetCopyMapAllRoleDamages(copyMap.CopyMapID);
						IEnumerable<RoleDamage> query = from items in roleDamages
						orderby items.Damage descending
						select items;
						List<RoleDamage> tempList = new List<RoleDamage>();
						int count = 0;
						foreach (RoleDamage item in query)
						{
							tempList.Add(item);
							count++;
							if (count >= GameManager.GuildCopyMapMgr.MaxDamageSendCount)
							{
								break;
							}
						}
						roleDamages = tempList;
						client.sendCmd<List<RoleDamage>>(626, roleDamages, false);
					}
				}
			}
		}

		// Token: 0x06001F3F RID: 7999 RVA: 0x001B0088 File Offset: 0x001AE288
		public void CheckCopyTeamDamage(long ticks, bool force = false)
		{
			if (ticks - this.LastNotifyTeamDamageTicks >= 2000L)
			{
				this.LastNotifyTeamDamageTicks = ticks;
				lock (this._RoleDamageDict_TeamCopyMapDict_Mutex)
				{
					foreach (CopyMap copyMap in this.TeamCopyMapDict)
					{
						Dictionary<int, RoleDamage> dict;
						if (this.RoleDamageDict.TryGetValue(copyMap.CopyMapID, out dict))
						{
							List<GameClient> clientList = copyMap.GetClientsList();
							long sumdamage = 0L;
							foreach (GameClient client in clientList)
							{
								long damage = Interlocked.Exchange(ref client.SumDamageForCopyTeam, 0L);
								if (damage > 0L)
								{
									int roleID = client.ClientData.RoleID;
									RoleDamage rd;
									if (dict.TryGetValue(roleID, out rd))
									{
										rd.Damage += damage;
									}
									else
									{
										dict.Add(roleID, new RoleDamage(roleID, damage, Global.FormatRoleName(client, client.ClientData.RoleName), new int[0]));
									}
									sumdamage += damage;
								}
							}
							if (sumdamage > 0L || force)
							{
								List<RoleDamage> roleDamages = this.GetCopyMapAllRoleDamages(copyMap.CopyMapID);
								if (GameManager.GuildCopyMapMgr.IsGuildCopyMap(copyMap.FubenMapID))
								{
									IEnumerable<RoleDamage> query = from items in roleDamages
									orderby items.Damage descending
									select items;
									List<RoleDamage> tempList = new List<RoleDamage>();
									int count = 0;
									foreach (RoleDamage item in query)
									{
										tempList.Add(item);
										count++;
										if (count >= GameManager.GuildCopyMapMgr.MaxDamageSendCount)
										{
											break;
										}
									}
									roleDamages = tempList;
								}
								foreach (GameClient client in clientList)
								{
									client.sendCmd<List<RoleDamage>>(626, roleDamages, false);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06001F40 RID: 8000 RVA: 0x001B03B8 File Offset: 0x001AE5B8
		private bool IsHeroMapCode(int mapCode)
		{
			if (null == CopyMapManager.HeroMapCodeFileds)
			{
				string heroMapCodes = GameManager.systemParamsList.GetParamValueByName("HeroMapCodes");
				if (!string.IsNullOrEmpty(heroMapCodes))
				{
					CopyMapManager.HeroMapCodeFileds = heroMapCodes.Split(new char[]
					{
						','
					});
				}
			}
			bool result;
			if (CopyMapManager.HeroMapCodeFileds == null || CopyMapManager.HeroMapCodeFileds.Length <= 0)
			{
				result = false;
			}
			else
			{
				string strMapCode = mapCode.ToString();
				for (int i = 0; i < CopyMapManager.HeroMapCodeFileds.Length; i++)
				{
					if (CopyMapManager.HeroMapCodeFileds[i] == strMapCode)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06001F41 RID: 8001 RVA: 0x001B0474 File Offset: 0x001AE674
		public void CopyMapPassAward(GameClient client, CopyMap copyMap, bool anyAlive)
		{
			int fuBenSeqID = FuBenManager.FindFuBenSeqIDByRoleID(client.ClientData.RoleID);
			FuBenTongGuanData fubenTongGuanData = null;
			if (fuBenSeqID > 0)
			{
				FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(fuBenSeqID);
				if (null != fuBenInfoItem)
				{
					fuBenInfoItem.EndTicks = TimeUtil.NOW();
					int addFuBenNum = 1;
					if (fuBenInfoItem.nDayOfYear != TimeUtil.NowDateTime().DayOfYear)
					{
						addFuBenNum = 0;
					}
					int fuBenID = FuBenManager.FindFuBenIDByMapCode(client.ClientData.MapCode);
					if (fuBenID > 0)
					{
						int usedSecs = (int)((fuBenInfoItem.EndTicks - fuBenInfoItem.StartTicks) / 1000L);
						int nLev = -1;
						string strName = null;
						SystemXmlItem systemFuBenItem = null;
						if (GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(fuBenID, out systemFuBenItem))
						{
							nLev = systemFuBenItem.GetIntValue("FuBenLevel", -1);
							strName = systemFuBenItem.GetStringValue("CopyName");
						}
						Global.UpdateFuBenDataForQuickPassTimer(client, fuBenID, usedSecs, addFuBenNum);
						FuBenMapItem fuBenMapItem = FuBenManager.FindMapCodeByFuBenID(fuBenID, client.ClientData.MapCode);
						if (fuBenMapItem != null && fuBenMapItem.Experience > 0 && fuBenMapItem.Money1 > 0)
						{
							int nMaxTime = fuBenMapItem.MaxTime * 60;
							long startTicks = fuBenInfoItem.StartTicks;
							long endTicks = fuBenInfoItem.EndTicks;
							int nFinishTimer = (int)(endTicks - startTicks) / 1000;
							int killedNum = 0;
							int nDieCount = fuBenInfoItem.nDieCount;
							fubenTongGuanData = Global.GiveCopyMapGiftForScore(client, fuBenID, copyMap.MapCode, nMaxTime, nFinishTimer, killedNum, nDieCount, (int)((double)fuBenMapItem.Experience * fuBenInfoItem.AwardRate), (int)((double)fuBenMapItem.Money1 * fuBenInfoItem.AwardRate), fuBenMapItem, strName);
						}
						GameManager.DBCmdMgr.AddDBCmd(10053, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							client.ClientData.RoleID,
							Global.FormatRoleName(client, client.ClientData.RoleName),
							fuBenID,
							usedSecs
						}), null, client.ServerId);
						bool bActiveChengJiu = true;
						if (GameManager.BloodCastleCopySceneMgr.IsBloodCastleCopyScene(copyMap.FubenMapID) && GameManager.DaimonSquareCopySceneMgr.IsDaimonSquareCopyScene(copyMap.FubenMapID))
						{
							bActiveChengJiu = false;
						}
						GameManager.ClientMgr.UpdateRoleDailyData_FuBenNum(client, 1, nLev, bActiveChengJiu);
					}
				}
			}
			GameManager.ClientMgr.NotifyAllFuBenBeginInfo(copyMap, client.ClientData.RoleID, !anyAlive);
			if (fubenTongGuanData != null)
			{
				byte[] bytesData = DataHelper.ObjectToBytes<FuBenTongGuanData>(fubenTongGuanData);
				GameManager.ClientMgr.NotifyAllFuBenTongGuanJiangLi(copyMap, bytesData);
			}
		}

		// Token: 0x06001F42 RID: 8002 RVA: 0x001B0710 File Offset: 0x001AE910
		public void CopyMapPassAwardForAll(GameClient client, CopyMap copyMap, bool anyAlive)
		{
			if (copyMap.CopyMapPassAwardFlag)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("CopyMapPassAwardForAll: 组队副本{0}序列ID({1})完成并给过奖励,不应再次给予", copyMap.FubenMapID, copyMap.FuBenSeqID), null, true);
			}
			else
			{
				copyMap.CopyMapPassAwardFlag = true;
				int fuBenSeqID = copyMap.FuBenSeqID;
				List<GameClient> objsList = new List<GameClient>();
				if (LuoLanFaZhenCopySceneManager.IsLuoLanFaZhen(copyMap.FubenMapID))
				{
					List<int> mapCodeList = FuBenManager.FindMapCodeListByFuBenID(copyMap.FubenMapID);
					if (null != mapCodeList)
					{
						foreach (int mapcode in mapCodeList)
						{
							int copyMapID = this.FindCopyID(fuBenSeqID, mapcode);
							if (copyMapID >= 0)
							{
								CopyMap child_map = this.FindCopyMap(copyMapID);
								if (null != child_map)
								{
									objsList.AddRange(child_map.GetClientsList());
								}
							}
						}
					}
				}
				else
				{
					objsList.AddRange(copyMap.GetClientsList());
				}
				objsList = Global.DistinctGameClientList(objsList);
				if (null != objsList)
				{
					if (fuBenSeqID > 0)
					{
						FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(fuBenSeqID);
						if (null != fuBenInfoItem)
						{
							fuBenInfoItem.EndTicks = TimeUtil.NOW();
							int addFuBenNum = 1;
							if (fuBenInfoItem.nDayOfYear != TimeUtil.NowDateTime().DayOfYear)
							{
								addFuBenNum = 0;
							}
							int fuBenID = FuBenManager.FindFuBenIDByMapCode(copyMap.MapCode);
							if (fuBenID > 0)
							{
								int usedSecs = (int)((fuBenInfoItem.EndTicks - fuBenInfoItem.StartTicks) / 1000L);
								int nLev = -1;
								string strName = null;
								SystemXmlItem systemFuBenItem = null;
								if (GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(fuBenID, out systemFuBenItem))
								{
									nLev = systemFuBenItem.GetIntValue("FuBenLevel", -1);
									strName = systemFuBenItem.GetStringValue("CopyName");
								}
								FuBenMapItem fuBenMapItem = FuBenManager.FindMapCodeByFuBenID(fuBenID, copyMap.MapCode);
								if (fuBenMapItem.Experience > 0 && fuBenMapItem.Money1 > 0)
								{
									int nMaxTime = fuBenMapItem.MaxTime * 60;
									long startTicks = fuBenInfoItem.StartTicks;
									long endTicks = fuBenInfoItem.EndTicks;
									int nFinishTimer = (int)(endTicks - startTicks) / 1000;
									int killedNum = 0;
									int nDieCount = fuBenInfoItem.nDieCount;
									for (int i = 0; i < objsList.Count; i++)
									{
										GameClient c = objsList[i];
										if (null != c)
										{
											FuBenTongGuanData fubenTongGuanData = Global.GiveCopyMapGiftForScore(c, fuBenID, copyMap.MapCode, nMaxTime, nFinishTimer, killedNum, nDieCount, (int)((double)fuBenMapItem.Experience * fuBenInfoItem.AwardRate), (int)((double)fuBenMapItem.Money1 * fuBenInfoItem.AwardRate), fuBenMapItem, strName);
											if (fubenTongGuanData != null)
											{
												byte[] bytesData = DataHelper.ObjectToBytes<FuBenTongGuanData>(fubenTongGuanData);
												GameManager.ClientMgr.SendToClient(c, bytesData, 521);
											}
										}
									}
								}
								for (int i = 0; i < objsList.Count; i++)
								{
									GameClient c = objsList[i];
									if (null != c)
									{
										Global.UpdateFuBenDataForQuickPassTimer(c, fuBenID, usedSecs, addFuBenNum);
										GameManager.DBCmdMgr.AddDBCmd(10053, string.Format("{0}:{1}:{2}:{3}", new object[]
										{
											c.ClientData.RoleID,
											Global.FormatRoleName(c, c.ClientData.RoleName),
											fuBenID,
											usedSecs
										}), null, c.ServerId);
										bool bActiveChengJiu = true;
										if (GameManager.BloodCastleCopySceneMgr.IsBloodCastleCopyScene(copyMap.FubenMapID) && GameManager.DaimonSquareCopySceneMgr.IsDaimonSquareCopyScene(copyMap.FubenMapID))
										{
											bActiveChengJiu = false;
										}
										GameManager.ClientMgr.UpdateRoleDailyData_FuBenNum(c, 1, nLev, bActiveChengJiu);
									}
								}
							}
						}
					}
					if (LuoLanFaZhenCopySceneManager.IsLuoLanFaZhen(copyMap.FubenMapID))
					{
						GameManager.ClientMgr.NotifyAllMapFuBenBeginInfo(copyMap, client.ClientData.RoleID, !anyAlive);
					}
					else
					{
						GameManager.ClientMgr.NotifyAllFuBenBeginInfo(copyMap, client.ClientData.RoleID, !anyAlive);
					}
				}
			}
		}

		// Token: 0x06001F43 RID: 8003 RVA: 0x001B0B90 File Offset: 0x001AED90
		public void CopyMapFaildForAll(List<GameClient> objsList, CopyMap copyMap)
		{
			if (copyMap.CopyMapPassAwardFlag)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("CopyMapPassAwardForAll: 组队副本{0}序列ID({1})完成并给过奖励,不应再次给予", copyMap.FubenMapID, copyMap.FuBenSeqID), null, true);
			}
			else
			{
				copyMap.CopyMapPassAwardFlag = true;
				int fuBenSeqID = copyMap.FuBenSeqID;
				int mapCode = copyMap.MapCode;
				objsList = Global.DistinctGameClientList(objsList);
				if (null != objsList)
				{
					FuBenTongGuanData fubenTongGuanData = null;
					if (fuBenSeqID > 0)
					{
						FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(fuBenSeqID);
						if (null != fuBenInfoItem)
						{
							fuBenInfoItem.EndTicks = TimeUtil.NOW();
							int fuBenID = FuBenManager.FindFuBenIDByMapCode(mapCode);
							if (fuBenID > 0)
							{
								int usedSecs = (int)((fuBenInfoItem.EndTicks - fuBenInfoItem.StartTicks) / 1000L);
								fubenTongGuanData = new FuBenTongGuanData();
								fubenTongGuanData.FuBenID = copyMap.FubenMapID;
								fubenTongGuanData.MapCode = mapCode;
								fubenTongGuanData.ResultMark = 1;
							}
						}
					}
					foreach (GameClient client in objsList)
					{
						GameManager.ClientMgr.NotifyAllFuBenBeginInfo(copyMap, client.ClientData.RoleID, false);
					}
					if (fubenTongGuanData != null && objsList.Count > 0)
					{
						byte[] bytesData = DataHelper.ObjectToBytes<FuBenTongGuanData>(fubenTongGuanData);
						GameManager.ClientMgr.NotifyAllFuBenTongGuanJiangLi(copyMap, bytesData);
					}
				}
			}
		}

		// Token: 0x06001F44 RID: 8004 RVA: 0x001B0D1C File Offset: 0x001AEF1C
		public void ProcessKilledMonster(GameClient client, Monster monster)
		{
			if (monster.CopyMapID > 0)
			{
				CopyMap copyMap = this.FindCopyMap(monster.CopyMapID);
				if (null != copyMap)
				{
					if (monster.ManagerType == SceneUIClasses.EMoLaiXiCopy)
					{
						copyMap.SetKilledDynamicMonsterDict(monster.UniqueID);
					}
					else if (monster.CurrentMapCode == SingletonTemplate<MoRiJudgeManager>.Instance().MapCode)
					{
						copyMap.SetKilledDynamicMonsterDict(monster.UniqueID);
					}
					else if (!copyMap.CustomPassAwards)
					{
						bool bIsStoryCopyMap = false;
						SystemXmlItem systemFuBenItem = null;
						if (GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(copyMap.FubenMapID, out systemFuBenItem))
						{
							int nKillAll = systemFuBenItem.GetIntValue("KillAll", -1);
							if (nKillAll == 2)
							{
								bIsStoryCopyMap = true;
							}
						}
						if (!monster.MonsterZoneNode.IsDynamicZone() || bIsStoryCopyMap)
						{
							if (101 == monster.MonsterType)
							{
								copyMap.SetKilledNormalDict(monster.RoleID);
							}
							else
							{
								copyMap.SetKilledBossDict(monster.RoleID);
							}
							bool autoGetFuBenAwards = false;
							bool anyAlive = GameManager.MonsterMgr.IsAnyMonsterAliveByCopyMapID(monster.CopyMapID);
							if (bIsStoryCopyMap && !copyMap.bStoryCopyMapFinishStatus)
							{
								int nNeedKillBoos = systemFuBenItem.GetIntValue("BossID", -1);
								if (monster.MonsterInfo.ExtensionID == nNeedKillBoos)
								{
									if (Global.IsInTeamCopyScene(client.ClientData.MapCode) || GameManager.GuildCopyMapMgr.IsGuildCopyMap(monster.CurrentMapCode))
									{
										this.CopyMapPassAwardForAll(client, copyMap, true);
									}
									else
									{
										this.CopyMapPassAward(client, copyMap, true);
									}
									Global.NotifyClientStoryCopyMapInfo(copyMap.CopyMapID, 3);
									copyMap.bStoryCopyMapFinishStatus = true;
									this.KillAllMonster(copyMap);
								}
							}
							if (!bIsStoryCopyMap && ((copyMap.KilledNormalNum >= copyMap.TotalNormalNum && copyMap.KilledBossNum >= copyMap.TotalBossNum) || !anyAlive))
							{
								if (this.IsHeroMapCode(monster.MonsterZoneNode.MapCode))
								{
									int currentMapCodeIndex = FuBenManager.FindMapCodeIndexByFuBenID(monster.MonsterZoneNode.MapCode);
									GameManager.ClientMgr.ChangeRoleHeroIndex(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, currentMapCodeIndex, false);
								}
								int toNextMapCode = FuBenManager.FindNextMapCodeByFuBenID(monster.MonsterZoneNode.MapCode);
								if (-1 == toNextMapCode)
								{
									GameManager.ClientMgr.NotifyAllFuBenMonstersNum(copyMap, !anyAlive);
									if (WanMotaCopySceneManager.IsWanMoTaMapCode(monster.MonsterZoneNode.MapCode))
									{
										WanMotaCopySceneManager.SendMsgToClientForWanMoTaCopyMapAward(client, copyMap, anyAlive);
									}
									else if (Global.IsInTeamCopyScene(client.ClientData.MapCode) || GameManager.GuildCopyMapMgr.IsGuildCopyMap(monster.CurrentMapCode))
									{
										this.CopyMapPassAwardForAll(client, copyMap, anyAlive);
									}
									else
									{
										this.CopyMapPassAward(client, copyMap, anyAlive);
									}
								}
								else
								{
									GameManager.ClientMgr.NotifyAllFuBenMonstersNum(copyMap, !anyAlive);
								}
							}
							else
							{
								GameManager.ClientMgr.NotifyAllFuBenMonstersNum(copyMap, !anyAlive);
							}
							if (autoGetFuBenAwards)
							{
							}
						}
					}
				}
			}
		}

		// Token: 0x06001F45 RID: 8005 RVA: 0x001B107C File Offset: 0x001AF27C
		public void KillAllMonster(CopyMap copyMap)
		{
			List<object> objList = GameManager.MonsterMgr.GetCopyMapIDMonsterList(copyMap.CopyMapID);
			objList = Global.ConvertObjsList(copyMap.MapCode, copyMap.CopyMapID, objList, false);
			if (null != objList)
			{
				int i = 0;
				while (i < objList.Count)
				{
					Monster monster = objList[i] as Monster;
					if (null != monster)
					{
						if (monster.MonsterType != 1001)
						{
							Global.SystemKillMonster(monster);
						}
					}
					IL_6A:
					i++;
					continue;
					goto IL_6A;
				}
			}
		}

		// Token: 0x06001F46 RID: 8006 RVA: 0x001B1108 File Offset: 0x001AF308
		public string GetCopyMapStrInfo()
		{
			Dictionary<int, int> copyMapInfoDict = new Dictionary<int, int>();
			int totalCopyMapMonsterCount = 0;
			int totalCount = 0;
			int index = 0;
			CopyMap copyMap = this.GetNextCopyMap(index);
			while (null != copyMap)
			{
				totalCount++;
				index++;
				int totalMapCount = 0;
				if (copyMapInfoDict.TryGetValue(copyMap.MapCode, out totalMapCount))
				{
					copyMapInfoDict[copyMap.MapCode] = totalMapCount + 1;
				}
				else
				{
					copyMapInfoDict[copyMap.MapCode] = 1;
				}
				totalCopyMapMonsterCount += copyMap.TotalNormalNum;
				totalCopyMapMonsterCount += copyMap.TotalBossNum;
				copyMap = this.GetNextCopyMap(index);
			}
			StringBuilder infoTxt = new StringBuilder();
			infoTxt.AppendFormat(string.Format("当前总的副本数量 {0} 个 \r\n", totalCount), new object[0]);
			infoTxt.AppendFormat(string.Format("当前总的副本怪物数量 {0} 个, 总的动态怪物 {1} 个 \r\n", totalCopyMapMonsterCount, Monster.GetMonsterCount()), new object[0]);
			infoTxt.AppendFormat(string.Format("WaitingAddFuBenMonsterQueueCount {0} \r\n", GameManager.MonsterZoneMgr.WaitingAddFuBenMonsterQueueCount()), new object[0]);
			infoTxt.AppendFormat(string.Format("WaitingDestroyFuBenMonsterQueueCount {0} \r\n", GameManager.MonsterZoneMgr.WaitingDestroyFuBenMonsterQueueCount()), new object[0]);
			infoTxt.AppendFormat(string.Format("WaitingReloadFuBenMonsterQueueCount {0} \r\n", GameManager.MonsterZoneMgr.WaitingReloadFuBenMonsterQueueCount()), new object[0]);
			foreach (KeyValuePair<int, int> item in copyMapInfoDict)
			{
				infoTxt.AppendFormat(string.Format("MapCode {0} 副本数量 {1} 个 \r\n", item.Key, item.Value), new object[0]);
			}
			return infoTxt.ToString();
		}

		// Token: 0x06001F47 RID: 8007 RVA: 0x001B12E8 File Offset: 0x001AF4E8
		public string ListCopyMapStrInfo()
		{
			Dictionary<int, int> copyMapInfoDict = new Dictionary<int, int>();
			int totalCopyMapMonsterCount = 0;
			int totalCount = 0;
			int index = 0;
			CopyMap copyMap = this.GetNextCopyMap(index);
			StringBuilder infoTxt = new StringBuilder();
			while (null != copyMap)
			{
				totalCount++;
				index++;
				int totalMapCount = 0;
				if (copyMapInfoDict.TryGetValue(copyMap.MapCode, out totalMapCount))
				{
					copyMapInfoDict[copyMap.MapCode] = totalMapCount + 1;
				}
				else
				{
					copyMapInfoDict[copyMap.MapCode] = 1;
				}
				totalCopyMapMonsterCount += copyMap.TotalNormalNum;
				totalCopyMapMonsterCount += copyMap.TotalBossNum;
				infoTxt.AppendFormat(string.Format("{0,10} {1,10} {2,10} \r\n", copyMap.FuBenSeqID, copyMap.MapCode, copyMap.GetClientsList().Count), new object[0]);
				copyMap = this.GetNextCopyMap(index);
			}
			infoTxt.AppendFormat(string.Format("当前总的副本数量 {0} 个 \r\n", totalCount), new object[0]);
			infoTxt.AppendFormat(string.Format("当前总的副本怪物数量 {0} 个, 总的动态怪物 {1} 个 \r\n", totalCopyMapMonsterCount, Monster.GetMonsterCount()), new object[0]);
			infoTxt.AppendFormat(string.Format("WaitingAddFuBenMonsterQueueCount {0} \r\n", GameManager.MonsterZoneMgr.WaitingAddFuBenMonsterQueueCount()), new object[0]);
			infoTxt.AppendFormat(string.Format("WaitingDestroyFuBenMonsterQueueCount {0} \r\n", GameManager.MonsterZoneMgr.WaitingDestroyFuBenMonsterQueueCount()), new object[0]);
			infoTxt.AppendFormat(string.Format("WaitingReloadFuBenMonsterQueueCount {0} \r\n", GameManager.MonsterZoneMgr.WaitingReloadFuBenMonsterQueueCount()), new object[0]);
			foreach (KeyValuePair<int, int> item in copyMapInfoDict)
			{
				infoTxt.AppendFormat(string.Format("MapCode {0} 副本数量 {1} 个 \r\n", item.Key, item.Value), new object[0]);
			}
			return infoTxt.ToString();
		}

		// Token: 0x06001F48 RID: 8008 RVA: 0x001B1510 File Offset: 0x001AF710
		public void AddGuangMuEvent(CopyMap copyMap, int guangMuId, int show)
		{
			copyMap.AddGuangMuEvent(guangMuId, show);
			GameManager.ClientMgr.BroadSpecialMapAIEvent(copyMap.MapCode, copyMap.CopyMapID, guangMuId, show);
		}

		// Token: 0x04002C00 RID: 11264
		private int BaseCopyMapID = 1;

		// Token: 0x04002C01 RID: 11265
		private Dictionary<string, int> FuBenSeqID2CopyIDDict = new Dictionary<string, int>();

		// Token: 0x04002C02 RID: 11266
		private Dictionary<string, int> FuBenSeqID2MonsterStateDict = new Dictionary<string, int>();

		// Token: 0x04002C03 RID: 11267
		private Dictionary<string, int> RoleIDFuBenSeqID2AwardStateDict = new Dictionary<string, int>();

		// Token: 0x04002C04 RID: 11268
		private List<CopyMap> _ListCopyMaps = new List<CopyMap>(300);

		// Token: 0x04002C05 RID: 11269
		private Dictionary<int, CopyMap> _DictCopyMaps = new Dictionary<int, CopyMap>(300);

		// Token: 0x04002C06 RID: 11270
		private object _RoleDamageDict_TeamCopyMapDict_Mutex = new object();

		// Token: 0x04002C07 RID: 11271
		private Dictionary<int, Dictionary<int, RoleDamage>> RoleDamageDict = new Dictionary<int, Dictionary<int, RoleDamage>>();

		// Token: 0x04002C08 RID: 11272
		private List<CopyMap> TeamCopyMapDict = new List<CopyMap>();

		// Token: 0x04002C09 RID: 11273
		private long LastNotifyTeamDamageTicks = 0L;

		// Token: 0x04002C0A RID: 11274
		private static string[] HeroMapCodeFileds = null;
	}
}
