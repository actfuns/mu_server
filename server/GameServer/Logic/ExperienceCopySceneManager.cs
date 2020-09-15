using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;
using Server.Data;
using Server.Protocol;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x020006BB RID: 1723
	internal class ExperienceCopySceneManager
	{
		// Token: 0x06002064 RID: 8292 RVA: 0x001BE380 File Offset: 0x001BC580
		public static void AddExperienceListCopyMap(int nID, CopyMap mapInfo)
		{
			bool bInsert = false;
			lock (ExperienceCopySceneManager.m_ExperienceListCopyMaps)
			{
				CopyMap tmp = null;
				if (!ExperienceCopySceneManager.m_ExperienceListCopyMaps.TryGetValue(nID, out tmp))
				{
					ExperienceCopySceneManager.m_ExperienceListCopyMaps.Add(nID, mapInfo);
					bInsert = true;
				}
				else if (tmp == null)
				{
					ExperienceCopySceneManager.m_ExperienceListCopyMaps[nID] = mapInfo;
					bInsert = true;
				}
			}
			lock (ExperienceCopySceneManager.m_ExperienceListCopyMapsInfo)
			{
				if (bInsert)
				{
					ExperienceCopyScene ExperienceSceneInfo = null;
					if (!ExperienceCopySceneManager.m_ExperienceListCopyMapsInfo.TryGetValue(nID, out ExperienceSceneInfo))
					{
						ExperienceSceneInfo = new ExperienceCopyScene();
						ExperienceSceneInfo.InitInfo(mapInfo.MapCode, mapInfo.CopyMapID, nID);
						ExperienceSceneInfo.m_StartTimer = TimeUtil.NOW();
						ExperienceCopySceneManager.m_ExperienceListCopyMapsInfo.Add(nID, ExperienceSceneInfo);
					}
				}
			}
		}

		// Token: 0x06002065 RID: 8293 RVA: 0x001BE4A0 File Offset: 0x001BC6A0
		public static void RemoveExperienceListCopyMap(int nID)
		{
			bool bRemve = false;
			lock (ExperienceCopySceneManager.m_ExperienceListCopyMaps)
			{
				CopyMap tmp = null;
				if (ExperienceCopySceneManager.m_ExperienceListCopyMaps.TryGetValue(nID, out tmp))
				{
					ExperienceCopySceneManager.m_ExperienceListCopyMaps.Remove(nID);
					bRemve = true;
				}
			}
			lock (ExperienceCopySceneManager.m_ExperienceListCopyMapsInfo)
			{
				if (bRemve)
				{
					ExperienceCopyScene ExperienceSceneInfo = null;
					if (ExperienceCopySceneManager.m_ExperienceListCopyMapsInfo.TryGetValue(nID, out ExperienceSceneInfo))
					{
						ExperienceCopySceneManager.m_ExperienceListCopyMapsInfo.Remove(nID);
					}
				}
			}
		}

		// Token: 0x06002066 RID: 8294 RVA: 0x001BE57C File Offset: 0x001BC77C
		public static void HeartBeatExperienceCopyMap()
		{
			long nowTicks = TimeUtil.NOW();
			if (nowTicks - ExperienceCopySceneManager.LastHeartBeatTicks >= 1000L)
			{
				ExperienceCopySceneManager.LastHeartBeatTicks = ((ExperienceCopySceneManager.LastHeartBeatTicks < 86400000L) ? nowTicks : (ExperienceCopySceneManager.LastHeartBeatTicks + 1000L));
				List<CopyMap> CopyMapList = new List<CopyMap>();
				lock (ExperienceCopySceneManager.m_ExperienceListCopyMaps)
				{
					foreach (CopyMap item in ExperienceCopySceneManager.m_ExperienceListCopyMaps.Values)
					{
						List<GameClient> clientsList = item.GetClientsList();
						ExperienceCopyMapDataInfo tmp = null;
						tmp = Data.ExperienceCopyMapDataInfoList[item.MapCode];
						if (tmp != null)
						{
							ExperienceCopyScene tmpExSceneInfo = null;
							lock (ExperienceCopySceneManager.m_ExperienceListCopyMapsInfo)
							{
								if (!ExperienceCopySceneManager.m_ExperienceListCopyMapsInfo.TryGetValue(item.FuBenSeqID, out tmpExSceneInfo))
								{
									continue;
								}
							}
							if (tmpExSceneInfo != null)
							{
								int nWave = tmpExSceneInfo.m_ExperienceCopyMapCreateMonsterWave;
								int nCount = tmp.MonsterIDList.Count;
								if (nWave < nCount)
								{
									if (tmpExSceneInfo.m_ExperienceCopyMapCreateMonsterFlag == 0)
									{
										if (clientsList.Count<GameClient>() != 0 && clientsList[0] != null)
										{
											ExperienceCopySceneManager.ExperienceCopyMapCreateMonster(clientsList[0], tmpExSceneInfo, tmp, nWave);
										}
										else
										{
											ExperienceCopySceneManager.ExperienceCopyMapCreateMonster(null, tmpExSceneInfo, tmp, nWave);
										}
									}
								}
							}
						}
					}
				}
				for (int i = 0; i < CopyMapList.Count; i++)
				{
					GameManager.CopyMapMgr.ProcessRemoveCopyMap(CopyMapList[i]);
				}
			}
		}

		// Token: 0x06002067 RID: 8295 RVA: 0x001BE7C4 File Offset: 0x001BC9C4
		public static void ExperienceCopyMapCreateMonster(GameClient client, ExperienceCopyScene ExperienceMapInfo, ExperienceCopyMapDataInfo exMap, int nWave)
		{
			ExperienceMapInfo.m_ExperienceCopyMapCreateMonsterFlag = 1;
			ExperienceMapInfo.m_ExperienceCopyMapCreateMonsterWave++;
			GameMap gameMap = null;
			if (!GameManager.MapMgr.DictMaps.TryGetValue(ExperienceMapInfo.m_MapCodeID, out gameMap))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("经验副本 地图配置 ID = {0}", ExperienceMapInfo.m_MapCodeID), null, true);
			}
			else
			{
				int gridX = gameMap.CorrectWidthPointToGridPoint(exMap.posX) / gameMap.MapGridWidth;
				int gridY = gameMap.CorrectHeightPointToGridPoint(exMap.posZ) / gameMap.MapGridHeight;
				int gridNum = gameMap.CorrectWidthPointToGridPoint(exMap.Radius);
				int nTotal = 0;
				List<int> nListID = exMap.MonsterIDList[nWave];
				List<int> nListNum = exMap.MonsterNumList[nWave];
				for (int i = 0; i < nListID.Count; i++)
				{
					int nID = nListID[i];
					int nNum = nListNum[i];
					GameManager.MonsterZoneMgr.AddDynamicMonsters(ExperienceMapInfo.m_MapCodeID, nID, ExperienceMapInfo.m_CopyMapID, nNum, gridX, gridY, gridNum, 0, SceneUIClasses.Normal, null, null);
					nTotal += nNum;
					ExperienceMapInfo.m_ExperienceCopyMapCreateMonsterNum += nNum;
					ExperienceMapInfo.m_ExperienceCopyMapRemainMonsterNum += nNum;
				}
				ExperienceMapInfo.m_ExperienceCopyMapNeedKillMonsterNum = ExperienceMapInfo.m_ExperienceCopyMapCreateMonsterNum * exMap.CreateNextWaveMonsterCondition[nWave] / 100;
				if (client != null)
				{
					ExperienceCopySceneManager.SendMsgToClientForExperienceCopyMapInfo(client, ExperienceMapInfo.m_ExperienceCopyMapCreateMonsterWave);
				}
			}
		}

		// Token: 0x06002068 RID: 8296 RVA: 0x001BE930 File Offset: 0x001BCB30
		public static void ExperienceCopyMapKillMonster(GameClient client, Monster monster)
		{
			ExperienceCopyMapDataInfo TmpExInfo = null;
			if (Data.ExperienceCopyMapDataInfoList.TryGetValue(client.ClientData.MapCode, out TmpExInfo))
			{
				ExperienceCopyScene tmpExSceneInfo = null;
				lock (ExperienceCopySceneManager.m_ExperienceListCopyMapsInfo)
				{
					if (!ExperienceCopySceneManager.m_ExperienceListCopyMapsInfo.TryGetValue(client.ClientData.FuBenSeqID, out tmpExSceneInfo))
					{
						return;
					}
				}
				if (tmpExSceneInfo != null)
				{
					CopyMap TmpCopyMapInfo = null;
					if (ExperienceCopySceneManager.m_ExperienceListCopyMaps.TryGetValue(client.ClientData.FuBenSeqID, out TmpCopyMapInfo))
					{
						if (TmpCopyMapInfo != null)
						{
							tmpExSceneInfo.m_ExperienceCopyMapKillMonsterNum++;
							tmpExSceneInfo.m_ExperienceCopyMapKillMonsterTotalNum++;
							tmpExSceneInfo.m_ExperienceCopyMapRemainMonsterNum--;
							if (tmpExSceneInfo.m_ExperienceCopyMapCreateMonsterFlag == 1 && tmpExSceneInfo.m_ExperienceCopyMapKillMonsterNum == tmpExSceneInfo.m_ExperienceCopyMapNeedKillMonsterNum)
							{
								tmpExSceneInfo.m_ExperienceCopyMapCreateMonsterFlag = 0;
								tmpExSceneInfo.m_ExperienceCopyMapKillMonsterNum = 0;
								tmpExSceneInfo.m_ExperienceCopyMapCreateMonsterNum = 0;
							}
							if (tmpExSceneInfo.m_ExperienceCopyMapCreateMonsterWave == TmpExInfo.MonsterIDList.Count && tmpExSceneInfo.m_ExperienceCopyMapKillMonsterTotalNum == TmpExInfo.MonsterSum)
							{
								ExperienceCopySceneManager.SendMsgToClientForExperienceCopyMapAward(client);
							}
							int nWave = tmpExSceneInfo.m_ExperienceCopyMapCreateMonsterWave;
							if (tmpExSceneInfo.m_ExperienceCopyMapKillMonsterTotalNum == TmpExInfo.MonsterSum || tmpExSceneInfo.m_ExperienceCopyMapRemainMonsterNum == 0)
							{
								nWave++;
							}
							ExperienceCopySceneManager.SendMsgToClientForExperienceCopyMapInfo(client, nWave);
						}
					}
				}
			}
		}

		// Token: 0x06002069 RID: 8297 RVA: 0x001BEAE8 File Offset: 0x001BCCE8
		public static void SendMsgToClientForExperienceCopyMapInfo(GameClient client, int nWave)
		{
			ExperienceCopyScene tmpExSceneInfo = null;
			lock (ExperienceCopySceneManager.m_ExperienceListCopyMapsInfo)
			{
				ExperienceCopySceneManager.m_ExperienceListCopyMapsInfo.TryGetValue(client.ClientData.FuBenSeqID, out tmpExSceneInfo);
			}
			if (tmpExSceneInfo != null)
			{
				int nRealyWave = nWave;
				int nTotalWave = Data.ExperienceCopyMapDataInfoList[client.ClientData.MapCode].MonsterIDList.Count;
				if (nRealyWave > nTotalWave)
				{
					nRealyWave = nTotalWave;
				}
				string strcmd = string.Format("{0}:{1}:{2}", nRealyWave, nTotalWave, tmpExSceneInfo.m_ExperienceCopyMapRemainMonsterNum);
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strcmd, 565);
				Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true);
			}
		}

		// Token: 0x0600206A RID: 8298 RVA: 0x001BEBE8 File Offset: 0x001BCDE8
		public static void SendMsgToClientForExperienceCopyMapAward(GameClient client)
		{
			CopyMap tmpCopyMap = ExperienceCopySceneManager.m_ExperienceListCopyMaps[client.ClientData.FuBenSeqID];
			if (tmpCopyMap != null)
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
							Global.UpdateFuBenDataForQuickPassTimer(client, fuBenID, usedSecs, addFuBenNum);
							FuBenMapItem fuBenMapItem = FuBenManager.FindMapCodeByFuBenID(fuBenID, client.ClientData.MapCode);
							if (fuBenMapItem.Experience > 0 && fuBenMapItem.Money1 > 0)
							{
								int nMaxTime = fuBenMapItem.MaxTime * 60;
								long startTicks = fuBenInfoItem.StartTicks;
								long endTicks = fuBenInfoItem.EndTicks;
								int nFinishTimer = (int)(endTicks - startTicks) / 1000;
								int killedNum = 0;
								int nDieCount = fuBenInfoItem.nDieCount;
								fubenTongGuanData = Global.GiveCopyMapGiftForScore(client, fuBenID, client.ClientData.MapCode, nMaxTime, nFinishTimer, killedNum, nDieCount, (int)((double)fuBenMapItem.Experience * fuBenInfoItem.AwardRate), (int)((double)fuBenMapItem.Money1 * fuBenInfoItem.AwardRate), fuBenMapItem, null);
							}
							GameManager.DBCmdMgr.AddDBCmd(10053, string.Format("{0}:{1}:{2}:{3}", new object[]
							{
								client.ClientData.RoleID,
								Global.FormatRoleName(client, client.ClientData.RoleName),
								fuBenID,
								usedSecs
							}), null, client.ServerId);
							int nLev = -1;
							SystemXmlItem systemFuBenItem = null;
							if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(fuBenID, out systemFuBenItem))
							{
								nLev = systemFuBenItem.GetIntValue("FuBenLevel", -1);
							}
							GameManager.ClientMgr.UpdateRoleDailyData_FuBenNum(client, 1, nLev, false);
						}
					}
				}
				if (fubenTongGuanData != null)
				{
					TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<FuBenTongGuanData>(fubenTongGuanData, Global._TCPManager.TcpOutPacketPool, 521);
					if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
					{
					}
				}
			}
		}

		// Token: 0x0400366A RID: 13930
		public static Dictionary<int, CopyMap> m_ExperienceListCopyMaps = new Dictionary<int, CopyMap>();

		// Token: 0x0400366B RID: 13931
		public static Dictionary<int, ExperienceCopyScene> m_ExperienceListCopyMapsInfo = new Dictionary<int, ExperienceCopyScene>();

		// Token: 0x0400366C RID: 13932
		private static long LastHeartBeatTicks = 0L;
	}
}
