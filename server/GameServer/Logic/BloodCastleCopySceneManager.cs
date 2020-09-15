using System;
using System.Collections.Generic;
using System.Threading;
using GameServer.Core.Executor;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x020005CB RID: 1483
	public class BloodCastleCopySceneManager
	{
		// Token: 0x06001B80 RID: 7040 RVA: 0x0019E3ED File Offset: 0x0019C5ED
		public void InitBloodCastleCopyScene()
		{
			Global.QueryDayActivityTotalPointInfoToDB(SpecialActivityTypes.BloodCastle);
		}

		// Token: 0x06001B81 RID: 7041 RVA: 0x0019E3F7 File Offset: 0x0019C5F7
		public void LoadBloodCastleListScenes()
		{
		}

		// Token: 0x06001B82 RID: 7042 RVA: 0x0019E3FA File Offset: 0x0019C5FA
		public void SetBloodCastleCopySceneTotalPoint(string sName, int nPoint)
		{
			this.m_sTotalPointName = sName;
			this.m_nTotalPointValue = nPoint;
		}

		// Token: 0x06001B83 RID: 7043 RVA: 0x0019E40C File Offset: 0x0019C60C
		public bool CanEnterExistCopyScene(GameClient client)
		{
			CopyMap copyMap = null;
			int fuBenSeqId = client.ClientData.FuBenSeqID;
			lock (this.m_BloodCastleCopyScenesList)
			{
				if (!this.m_BloodCastleCopyScenesList.TryGetValue(fuBenSeqId, out copyMap))
				{
					return false;
				}
			}
			bool result;
			lock (this.m_BloodCastleCopyScenesInfo)
			{
				Dictionary<int, BloodCastleScene> dicTmp = null;
				BloodCastleScene bcData = null;
				int nFubenID = copyMap.FubenMapID;
				if (!this.m_BloodCastleCopyScenesInfo.TryGetValue(nFubenID, out dicTmp))
				{
					result = false;
				}
				else if (!dicTmp.TryGetValue(fuBenSeqId, out bcData))
				{
					result = false;
				}
				else if (bcData.m_eStatus != BloodCastleStatus.FIGHT_STATUS_BEGIN)
				{
					result = false;
				}
				else
				{
					result = bcData.CantiansRole(client);
				}
			}
			return result;
		}

		// Token: 0x06001B84 RID: 7044 RVA: 0x0019E518 File Offset: 0x0019C718
		public void AddBloodCastleCopyScenes(int nSequenceID, int nFubenID, int nMapCodeID, CopyMap mapInfo)
		{
			lock (this.m_BloodCastleCopyScenesList)
			{
				CopyMap cmInfo = null;
				if (!this.m_BloodCastleCopyScenesList.TryGetValue(nSequenceID, out cmInfo) || cmInfo == null)
				{
					this.m_BloodCastleCopyScenesList.Add(nSequenceID, mapInfo);
				}
			}
			lock (this.m_BloodCastleCopyScenesInfo)
			{
				Dictionary<int, BloodCastleScene> dicTmp = null;
				BloodCastleScene bcData = null;
				if (!this.m_BloodCastleCopyScenesInfo.TryGetValue(nFubenID, out dicTmp))
				{
					dicTmp = new Dictionary<int, BloodCastleScene>();
					this.m_BloodCastleCopyScenesInfo.Add(nFubenID, dicTmp);
				}
				if (!dicTmp.TryGetValue(nSequenceID, out bcData))
				{
					bcData = new BloodCastleScene();
					dicTmp.Add(nSequenceID, bcData);
					bcData.CleanAllInfo();
				}
				bcData.m_nMapCode = nMapCodeID;
				bcData.m_CopyMap = mapInfo;
			}
		}

		// Token: 0x06001B85 RID: 7045 RVA: 0x0019E634 File Offset: 0x0019C834
		public void RemoveBloodCastleListCopyScenes(CopyMap cmInfo, int nSqeID, int nCopyID)
		{
			lock (this.m_BloodCastleCopyScenesList)
			{
				CopyMap cmTmp = null;
				if (this.m_BloodCastleCopyScenesList.TryGetValue(nSqeID, out cmTmp) && cmTmp != null)
				{
					this.m_BloodCastleCopyScenesList.Remove(nSqeID);
				}
			}
			lock (this.m_BloodCastleCopyScenesInfo)
			{
				Dictionary<int, BloodCastleScene> dicTmp = null;
				if (this.m_BloodCastleCopyScenesInfo.TryGetValue(nCopyID, out dicTmp) && dicTmp != null)
				{
					BloodCastleScene bcTmp = null;
					if (dicTmp.TryGetValue(nSqeID, out bcTmp) && bcTmp != null)
					{
						dicTmp.Remove(nSqeID);
					}
					if (dicTmp.Count <= 0)
					{
						this.m_BloodCastleCopyScenesInfo.Remove(nCopyID);
					}
				}
			}
		}

		// Token: 0x06001B86 RID: 7046 RVA: 0x0019E748 File Offset: 0x0019C948
		public int CheckBloodCastleListScenes(int nFuBenMapID)
		{
			lock (this.m_BloodCastleCopyScenesInfo)
			{
				Dictionary<int, BloodCastleScene> tmpData = null;
				if (!this.m_BloodCastleCopyScenesInfo.TryGetValue(nFuBenMapID, out tmpData))
				{
					return -1;
				}
				if (tmpData == null)
				{
					return -1;
				}
				BloodCastleDataInfo bcDataTmp = null;
				if (!Data.BloodCastleDataInfoList.TryGetValue(nFuBenMapID, out bcDataTmp))
				{
					return -1;
				}
				if (bcDataTmp == null)
				{
					return -1;
				}
				foreach (KeyValuePair<int, BloodCastleScene> bcData in tmpData)
				{
					int nID = bcData.Key;
					BloodCastleScene tmpbcinfo = bcData.Value;
					if (nID >= 0 && tmpbcinfo != null)
					{
						if (nID == nFuBenMapID && tmpbcinfo.m_nPlarerCount < bcDataTmp.MaxEnterNum && tmpbcinfo.m_eStatus < BloodCastleStatus.FIGHT_STATUS_BEGIN)
						{
							return nID;
						}
					}
				}
			}
			return -1;
		}

		// Token: 0x06001B87 RID: 7047 RVA: 0x0019E8B8 File Offset: 0x0019CAB8
		public bool IsBloodCastleCopyScene(int nFuBenMapID)
		{
			return Data.BloodCastleDataInfoList.ContainsKey(nFuBenMapID);
		}

		// Token: 0x06001B88 RID: 7048 RVA: 0x0019E8E4 File Offset: 0x0019CAE4
		public bool IsBloodCastleCopyScene2(int nMpaCodeID)
		{
			SceneUIClasses sceneType = Global.GetMapSceneType(nMpaCodeID);
			return sceneType == SceneUIClasses.BloodCastle;
		}

		// Token: 0x06001B89 RID: 7049 RVA: 0x0019E910 File Offset: 0x0019CB10
		public CopyMap GetBloodCastleCopySceneInfo(int nSequenceID)
		{
			CopyMap result;
			if (nSequenceID < 0)
			{
				result = null;
			}
			else
			{
				CopyMap copymapInfo = null;
				if (!this.m_BloodCastleCopyScenesList.TryGetValue(nSequenceID, out copymapInfo))
				{
					result = null;
				}
				else
				{
					result = copymapInfo;
				}
			}
			return result;
		}

		// Token: 0x06001B8A RID: 7050 RVA: 0x0019E94C File Offset: 0x0019CB4C
		public BloodCastleScene GetBloodCastleCopySceneDataInfo(CopyMap cmInfo, int nSequenceID, int nFuBenID)
		{
			BloodCastleScene result;
			if (cmInfo == null || nSequenceID < 0)
			{
				result = null;
			}
			else
			{
				Dictionary<int, BloodCastleScene> dicTmp = null;
				if (!this.m_BloodCastleCopyScenesInfo.TryGetValue(nFuBenID, out dicTmp) || dicTmp == null)
				{
					result = null;
				}
				else
				{
					BloodCastleScene bcInfo = null;
					if (!dicTmp.TryGetValue(nSequenceID, out bcInfo) || bcInfo == null)
					{
						result = null;
					}
					else
					{
						result = bcInfo;
					}
				}
			}
			return result;
		}

		// Token: 0x06001B8B RID: 7051 RVA: 0x0019E9BC File Offset: 0x0019CBBC
		public int EnterBloodCastSceneCopySceneCount(GameClient client, int nFubenID, out int nBloodNum)
		{
			nBloodNum = -1;
			BloodCastleDataInfo bcDataTmp = null;
			int result;
			if (!Data.BloodCastleDataInfoList.TryGetValue(nFubenID, out bcDataTmp))
			{
				result = -1;
			}
			else
			{
				int nDate = TimeUtil.NowDateTime().DayOfYear;
				int nType = 1;
				int nCount = Global.QueryDayActivityEnterCountToDB(client, client.ClientData.RoleID, nDate, nType);
				nBloodNum = nCount;
				if (nCount >= bcDataTmp.MaxEnterNum)
				{
					bool nRet = true;
					int dayID = TimeUtil.NowDateTime().DayOfYear;
					int nVipLev = client.ClientData.VipLevel;
					int[] nArry = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPEnterBloodCastleCountAddValue", ',');
					if (nVipLev > 0 && nArry != null && nArry[nVipLev] > 0)
					{
						int nNum = nArry[nVipLev];
						if (nCount < bcDataTmp.MaxEnterNum + nNum)
						{
							Global.UpdateVipDailyData(client, dayID, 1000001);
							nRet = false;
						}
					}
					if (nRet)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(15, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						return -1;
					}
				}
				result = 1;
			}
			return result;
		}

		// Token: 0x06001B8C RID: 7052 RVA: 0x0019EB04 File Offset: 0x0019CD04
		public void SendMegToClient(GameClient client, int nFubenID, int nSquID, int nCmdID)
		{
			CopyMap cmInfo = null;
			lock (this.m_BloodCastleCopyScenesList)
			{
				if (!this.m_BloodCastleCopyScenesList.TryGetValue(nSquID, out cmInfo) || cmInfo == null)
				{
					return;
				}
			}
			long ticks = TimeUtil.NOW();
			lock (this.m_BloodCastleCopyScenesInfo)
			{
				Dictionary<int, BloodCastleScene> dicTmp = null;
				if (this.m_BloodCastleCopyScenesInfo.TryGetValue(nFubenID, out dicTmp) && dicTmp != null)
				{
					BloodCastleScene bcTmp = null;
					if (dicTmp.TryGetValue(nSquID, out bcTmp) && bcTmp != null)
					{
						BloodCastleDataInfo bcDataTmp = null;
						if (Data.BloodCastleDataInfoList.TryGetValue(nFubenID, out bcDataTmp) && bcDataTmp != null)
						{
							if (nCmdID == 545)
							{
								if (bcTmp.m_eStatus <= BloodCastleStatus.FIGHT_STATUS_PREPARE)
								{
									GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, cmInfo, nCmdID, 0, 0, 0, bcTmp.m_nPlarerCount, null);
								}
							}
							else if (nCmdID == 531)
							{
								if (bcTmp.m_lPrepareTime > 0L)
								{
									int nTimer = (int)(((long)(bcDataTmp.PrepareTime * 1000) - (ticks - bcTmp.m_lPrepareTime)) / 1000L);
									string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
									{
										nFubenID,
										nTimer,
										bcDataTmp.NeedKillMonster1Num,
										1,
										bcDataTmp.NeedKillMonster2Num,
										1,
										1,
										1
									});
									GameManager.ClientMgr.SendToClient(client, strcmd, 531);
									GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, cmInfo, 545, 0, 0, 0, bcTmp.m_nPlarerCount, client);
									if (bcTmp.m_eStatus == BloodCastleStatus.FIGHT_STATUS_BEGIN)
									{
										nTimer = (int)(((long)(bcDataTmp.DurationTime * 1000) - (ticks - bcTmp.m_lBeginTime)) / 1000L);
										GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, cmInfo, 517, nTimer, 0, 0, bcTmp.m_nPlarerCount, client);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06001B8D RID: 7053 RVA: 0x0019EDFC File Offset: 0x0019CFFC
		public int EnterBloodCastSceneCopyScene(GameClient client, int nFubenID, int nBloodNum, out int nSeqID, int mapCode)
		{
			nSeqID = -1;
			if (client.ClientData.BloodCastleAwardPoint > 0)
			{
				int FuBenSeqID = Global.GetRoleParamsInt32FromDB(client, "BloodCastleFuBenSeqID");
				int nSceneID = Global.GetRoleParamsInt32FromDB(client, "BloodCastleSceneid");
				int nFlag = Global.GetRoleParamsInt32FromDB(client, "BloodCastleSceneFinishFlag");
				int awardState = GameManager.CopyMapMgr.FindAwardState(client.ClientData.RoleID, FuBenSeqID, nSceneID);
				if (awardState == 0)
				{
					BloodCastleDataInfo bcDataTmp = null;
					if (Data.BloodCastleDataInfoList.TryGetValue(nSceneID, out bcDataTmp))
					{
						if (bcDataTmp == null)
						{
							client.ClientData.BloodCastleAwardPoint = 0;
							return 1;
						}
						string AwardItem = null;
						string AwardItem2 = null;
						for (int i = 0; i < bcDataTmp.AwardItem2.Length; i++)
						{
							AwardItem2 += bcDataTmp.AwardItem2[i];
							if (i != bcDataTmp.AwardItem2.Length - 1)
							{
								AwardItem2 += "|";
							}
						}
						string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
						{
							-1,
							nFlag,
							client.ClientData.BloodCastleAwardPoint,
							Global.CalcExpForRoleScore(client.ClientData.BloodCastleAwardPoint, bcDataTmp.ExpModulus),
							client.ClientData.BloodCastleAwardPoint * bcDataTmp.MoneyModulus,
							AwardItem,
							AwardItem2
						});
						GameManager.ClientMgr.SendToClient(client, strcmd, 519);
						return -1;
					}
				}
				else
				{
					client.ClientData.BloodCastleAwardPoint = 0;
					Global.SaveRoleParamsInt32ValueToDB(client, "BloodCastlePlayerPoint", client.ClientData.BloodCastleAwardPoint, true);
				}
			}
			int nFubenMapID = Global.GetBloodCastleCopySceneIDForRole(client);
			int result;
			if (nFubenMapID <= 0)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(16, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 39);
				LogManager.WriteLog(LogTypes.Error, string.Format("enter bloodcastle scene fail!! get scene info fail!!!!", new object[0]), null, true);
				result = -1;
			}
			else
			{
				BloodCastleDataInfo bcInfo = null;
				if (!Data.BloodCastleDataInfoList.TryGetValue(nFubenMapID, out bcInfo) || bcInfo == null)
				{
					result = -1;
				}
				else if (!Global.CanEnterBloodCastleOnTime(bcInfo.BeginTime, bcInfo.PrepareTime))
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(17, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = -1;
				}
				else
				{
					GoodsData goodData = Global.GetGoodsByID(client, bcInfo.NeedGoodsID);
					if (goodData == null)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(18, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						result = -1;
					}
					else if (goodData.GCount < bcInfo.NeedGoodsNum)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(19, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						result = -1;
					}
					else
					{
						bool usedBinding = false;
						bool usedTimeLimited = false;
						if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, bcInfo.NeedGoodsID, 1, false, out usedBinding, out usedTimeLimited, false))
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(20, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							result = -1;
						}
						else
						{
							Dictionary<int, BloodCastleScene> dicTmp = null;
							lock (this.m_BloodCastleCopyScenesInfo)
							{
								if (this.m_BloodCastleCopyScenesInfo.TryGetValue(nFubenMapID, out dicTmp) && dicTmp != null)
								{
									foreach (KeyValuePair<int, BloodCastleScene> bcsceneInfo in dicTmp)
									{
										if (bcsceneInfo.Value.m_eStatus < BloodCastleStatus.FIGHT_STATUS_BEGIN)
										{
											if (bcsceneInfo.Value.m_nPlarerCount < bcInfo.MaxPlayerNum)
											{
												bcsceneInfo.Value.m_nPlarerCount++;
												nSeqID = bcsceneInfo.Key;
											}
										}
									}
								}
								if (nSeqID < 0)
								{
									string[] dbFields = Global.ExecuteDBCmd(10049, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
									if (dbFields != null && dbFields.Length >= 2)
									{
										nSeqID = Global.SafeConvertToInt32(dbFields[1]);
										if (nSeqID > 0)
										{
											BloodCastleScene bcData = null;
											if (!this.m_BloodCastleCopyScenesInfo.TryGetValue(nFubenID, out dicTmp) || dicTmp == null)
											{
												dicTmp = new Dictionary<int, BloodCastleScene>();
												this.m_BloodCastleCopyScenesInfo.Add(nFubenID, dicTmp);
											}
											if (!dicTmp.TryGetValue(nSeqID, out bcData) || bcData == null)
											{
												bcData = new BloodCastleScene();
												bcData.CleanAllInfo();
												bcData.m_nMapCode = mapCode;
												bcData.m_nPlarerCount = 1;
												dicTmp[nSeqID] = bcData;
											}
										}
									}
								}
							}
							Global.SaveRoleParamsInt32ValueToDB(client, "BloodCastleFuBenSeqID", nSeqID, true);
							Global.SaveRoleParamsInt32ValueToDB(client, "BloodCastleSceneid", nFubenMapID, true);
							Global.SaveRoleParamsInt32ValueToDB(client, "BloodCastleSceneFinishFlag", 0, true);
							result = 0;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06001B8E RID: 7054 RVA: 0x0019F474 File Offset: 0x0019D674
		public void HeartBeatBloodCastScene()
		{
			long nowTicks = TimeUtil.NOW();
			if (Math.Abs(nowTicks - BloodCastleCopySceneManager.LastHeartBeatTicks) >= 1000L)
			{
				BloodCastleCopySceneManager.LastHeartBeatTicks = nowTicks;
				HashSet<int> mapCodeHashSet = new HashSet<int>();
				lock (this.m_BloodCastleCopyScenesList)
				{
					CopyMap copyMap = null;
					foreach (KeyValuePair<int, CopyMap> BloodCastleCopySceneList in this.m_BloodCastleCopyScenesList)
					{
						int nID = BloodCastleCopySceneList.Value.FuBenSeqID;
						int nCopyID = BloodCastleCopySceneList.Value.FubenMapID;
						int nMapCodeID = -1;
						nMapCodeID = BloodCastleCopySceneList.Value.MapCode;
						if (nID >= 0 && nCopyID >= 0 && nMapCodeID >= 0)
						{
							copyMap = BloodCastleCopySceneList.Value;
							lock (this.m_BloodCastleCopyScenesInfo)
							{
								BloodCastleDataInfo bcDataTmp = null;
								if (Data.BloodCastleDataInfoList.TryGetValue(nCopyID, out bcDataTmp) && bcDataTmp != null)
								{
									Dictionary<int, BloodCastleScene> dicTmp = null;
									if (this.m_BloodCastleCopyScenesInfo.TryGetValue(nCopyID, out dicTmp) && dicTmp != null)
									{
										BloodCastleScene bcTmp = null;
										if (dicTmp.TryGetValue(nID, out bcTmp) && bcTmp != null)
										{
											long ticks = TimeUtil.NOW();
											if (bcTmp.m_eStatus == BloodCastleStatus.FIGHT_STATUS_NULL)
											{
												int nSecond = 0;
												string strTimer = null;
												if (Global.CanEnterBloodCastleCopySceneOnTime(bcDataTmp.BeginTime, bcDataTmp.PrepareTime + bcDataTmp.DurationTime, out nSecond, out strTimer))
												{
													bcTmp.m_eStatus = BloodCastleStatus.FIGHT_STATUS_PREPARE;
													DateTime staticTime = DateTime.Parse(strTimer);
													bcTmp.m_lPrepareTime = staticTime.Ticks / 10000L;
													List<GameClient> objsList = BloodCastleCopySceneList.Value.GetClientsList();
													if (null == objsList)
													{
														return;
													}
													for (int i = 0; i < objsList.Count; i++)
													{
														this.SendMegToClient(objsList[i], nCopyID, nID, 531);
													}
												}
											}
											else if (bcTmp.m_eStatus == BloodCastleStatus.FIGHT_STATUS_PREPARE)
											{
												if (ticks >= bcTmp.m_lPrepareTime + (long)(bcDataTmp.PrepareTime * 1000))
												{
													bcTmp.m_Step++;
													GameManager.CopyMapMgr.AddGuangMuEvent(bcTmp.m_CopyMap, 1, 0);
													bcTmp.m_eStatus = BloodCastleStatus.FIGHT_STATUS_BEGIN;
													bcTmp.m_lBeginTime = TimeUtil.NOW();
													int nTimer = (int)(((long)(bcDataTmp.DurationTime * 1000) - (ticks - bcTmp.m_lBeginTime)) / 1000L);
													List<GameClient> clientList = bcTmp.m_CopyMap.GetClientsList();
													if (null != clientList)
													{
														foreach (GameClient c in clientList)
														{
															if (bcTmp.AddRole(c))
															{
																Global.UpdateRoleEnterActivityCount(c, SpecialActivityTypes.BloodCastle);
															}
														}
													}
													GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, BloodCastleCopySceneList.Value, 517, nTimer, 0, 0, 0, null);
													int monsterID = bcDataTmp.GateID;
													string[] sfields = bcDataTmp.GatePos.Split(new char[]
													{
														','
													});
													int nPosX = Global.SafeConvertToInt32(sfields[0]);
													int nPosY = Global.SafeConvertToInt32(sfields[1]);
													GameMap gameMap = null;
													if (!GameManager.MapMgr.DictMaps.TryGetValue(bcTmp.m_nMapCode, out gameMap))
													{
														LogManager.WriteLog(LogTypes.Error, string.Format("血色城堡报错 地图配置 ID = {0}", bcDataTmp.MapCode), null, true);
														return;
													}
													int gridX = gameMap.CorrectWidthPointToGridPoint(nPosX) / gameMap.MapGridWidth;
													int gridY = gameMap.CorrectHeightPointToGridPoint(nPosY) / gameMap.MapGridHeight;
													GameManager.MonsterZoneMgr.AddDynamicMonsters(nMapCodeID, monsterID, BloodCastleCopySceneList.Value.CopyMapID, 1, gridX, gridY, 0, 0, SceneUIClasses.Normal, null, null);
													GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, BloodCastleCopySceneList.Value, 533, 0, 0, 1, 0, null);
												}
											}
											else if (bcTmp.m_eStatus == BloodCastleStatus.FIGHT_STATUS_BEGIN)
											{
												if (ticks < bcTmp.m_lBeginTime + 20000L)
												{
													List<GameClient> clientList = bcTmp.m_CopyMap.GetClientsList();
													if (null != clientList)
													{
														foreach (GameClient c in clientList)
														{
															if (bcTmp.AddRole(c))
															{
																LogManager.WriteLog(LogTypes.Error, string.Format("EnterCount#BloodCastle#rid={0}", c.ClientData.RoleID), null, true);
																Global.UpdateRoleEnterActivityCount(c, SpecialActivityTypes.BloodCastle);
															}
														}
													}
												}
												if (ticks >= bcTmp.m_lBeginTime + (long)(bcDataTmp.DurationTime * 1000))
												{
													bcTmp.m_eStatus = BloodCastleStatus.FIGHT_STATUS_END;
													bcTmp.m_lEndTime = TimeUtil.NOW();
													try
													{
														string log = string.Format("血色城堡已结束,是否完成{0},结束时间{1},m_bIsFinishTask:{2},m_nKillMonsterACount:{3},m_bKillMonsterAStatus:{4},m_nRoleID:{5}", new object[]
														{
															bcTmp.m_bIsFinishTask,
															new DateTime(bcTmp.m_lEndTime * 10000L),
															bcTmp.m_bIsFinishTask,
															bcTmp.m_nKillMonsterACount,
															bcTmp.m_bKillMonsterAStatus,
															bcTmp.m_nRoleID
														});
														LogManager.WriteLog(LogTypes.Error, log, null, true);
													}
													catch
													{
													}
												}
												mapCodeHashSet.Add(copyMap.MapCode);
											}
											else if (bcTmp.m_eStatus == BloodCastleStatus.FIGHT_STATUS_END)
											{
												int nTimer = (int)(((long)(bcDataTmp.LeaveTime * 1000) - (ticks - bcTmp.m_lEndTime)) / 1000L);
												if (!bcTmp.m_bEndFlag)
												{
													long nTimerInfo = bcTmp.m_lEndTime - bcTmp.m_lBeginTime;
													long nRemain = ((long)(bcDataTmp.DurationTime * 1000) - nTimerInfo) / 1000L;
													if (nRemain >= (long)bcDataTmp.DurationTime)
													{
														nRemain = (long)(bcDataTmp.DurationTime / 2);
													}
													int nTimeAward = (int)((long)bcDataTmp.TimeModulus * nRemain);
													if (nTimeAward < 0)
													{
														nTimeAward = 0;
													}
													GameManager.ClientMgr.NotifyBloodCastleCopySceneMsgEndFight(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, BloodCastleCopySceneList.Value, bcTmp, 519, nTimer, nTimeAward);
												}
											}
										}
									}
								}
							}
						}
					}
					foreach (int mapCode in mapCodeHashSet)
					{
						GameManager.MonsterZoneMgr.ReloadCopyMapMonsters(mapCode, -1);
					}
				}
			}
		}

		// Token: 0x06001B8F RID: 7055 RVA: 0x0019FCC4 File Offset: 0x0019DEC4
		public void CreateMonsterBBloodCastScene(int mapid, BloodCastleDataInfo bcDataTmp, BloodCastleScene bcTmp, int nCopyMapID)
		{
			int monsterID = bcDataTmp.NeedKillMonster2ID;
			string[] sfields = bcDataTmp.NeedCreateMonster2Pos.Split(new char[]
			{
				','
			});
			int nPosX = Global.SafeConvertToInt32(sfields[0]);
			int nPosY = Global.SafeConvertToInt32(sfields[1]);
			GameMap gameMap = null;
			if (!GameManager.MapMgr.DictMaps.TryGetValue(bcTmp.m_nMapCode, out gameMap))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("血色城堡报错 地图配置 ID = {0}", bcDataTmp.MapCode), null, true);
			}
			else
			{
				int gridX = gameMap.CorrectWidthPointToGridPoint(nPosX) / gameMap.MapGridWidth;
				int gridY = gameMap.CorrectHeightPointToGridPoint(nPosY) / gameMap.MapGridHeight;
				int gridNum = gameMap.CorrectPointToGrid(bcDataTmp.NeedCreateMonster2Radius);
				for (int i = 0; i < bcDataTmp.NeedCreateMonster2Num; i++)
				{
					GameManager.MonsterZoneMgr.AddDynamicMonsters(mapid, monsterID, nCopyMapID, 1, gridX, gridY, gridNum, bcDataTmp.NeedCreateMonster2PursuitRadius, SceneUIClasses.Normal, null, null);
				}
			}
		}

		// Token: 0x06001B90 RID: 7056 RVA: 0x0019FDB8 File Offset: 0x0019DFB8
		public void OnStartPlayGame(GameClient client)
		{
			if (client.ClientData.FuBenSeqID >= 0 && client.ClientData.CopyMapID >= 0 && this.IsBloodCastleCopyScene(client.ClientData.FuBenID))
			{
				BloodCastleDataInfo bcDataTmp = null;
				if (Data.BloodCastleDataInfoList.TryGetValue(client.ClientData.FuBenID, out bcDataTmp) && bcDataTmp != null)
				{
					Dictionary<int, BloodCastleScene> dicTmp = null;
					if (this.m_BloodCastleCopyScenesInfo.TryGetValue(client.ClientData.FuBenID, out dicTmp) && dicTmp != null)
					{
						BloodCastleScene bcTmp = null;
						if (dicTmp.TryGetValue(client.ClientData.FuBenSeqID, out bcTmp) && bcTmp != null)
						{
							CopyMap cmInfo = null;
							if (this.m_BloodCastleCopyScenesList.TryGetValue(client.ClientData.FuBenSeqID, out cmInfo) && cmInfo != null)
							{
								if (!bcTmp.m_bEndFlag)
								{
									this.SendMegToClient(client, client.ClientData.FuBenID, client.ClientData.FuBenSeqID, 531);
									string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.BloodCastleAwardPoint);
									GameManager.ClientMgr.SendToClient(client, strcmd, 532);
									if (bcTmp.m_Step != 0)
									{
										if (bcTmp.m_Step == 1)
										{
											GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, cmInfo, 533, 0, bcTmp.m_nKillMonsterACount, 1, 0, null);
										}
										else if (bcTmp.m_Step == 2)
										{
											GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, cmInfo, 533, 0, 0, 2, 0, null);
										}
										else if (bcTmp.m_Step == 3)
										{
											GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, cmInfo, 533, 0, bcTmp.m_nKillMonsterBCount, 3, 0, null);
										}
										else if (bcTmp.m_Step == 4)
										{
											GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, cmInfo, 533, 0, 0, 4, 0, null);
										}
										else if (!bcTmp.m_bIsFinishTask)
										{
											GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, cmInfo, 533, 0, 0, 5, 0, null);
										}
										else
										{
											GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, cmInfo, 533, 0, 1, 5, 0, null);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06001B91 RID: 7057 RVA: 0x001A00CC File Offset: 0x0019E2CC
		public void KillMonsterABloodCastCopyScene(GameClient client, Monster monster)
		{
			if (client.ClientData.FuBenSeqID >= 0 && client.ClientData.CopyMapID >= 0 && this.IsBloodCastleCopyScene(client.ClientData.FuBenID))
			{
				BloodCastleDataInfo bcDataTmp = null;
				if (Data.BloodCastleDataInfoList.TryGetValue(client.ClientData.FuBenID, out bcDataTmp) && bcDataTmp != null)
				{
					Dictionary<int, BloodCastleScene> dicTmp = null;
					if (this.m_BloodCastleCopyScenesInfo.TryGetValue(client.ClientData.FuBenID, out dicTmp) && dicTmp != null)
					{
						BloodCastleScene bcTmp = null;
						if (dicTmp.TryGetValue(client.ClientData.FuBenSeqID, out bcTmp) && bcTmp != null)
						{
							CopyMap cmInfo = null;
							if (this.m_BloodCastleCopyScenesList.TryGetValue(client.ClientData.FuBenSeqID, out cmInfo) && cmInfo != null)
							{
								if (!bcTmp.m_bEndFlag && bcTmp.m_eStatus == BloodCastleStatus.FIGHT_STATUS_BEGIN)
								{
									if (bcTmp.m_eStatus == BloodCastleStatus.FIGHT_STATUS_BEGIN)
									{
										client.ClientData.BloodCastleAwardPoint += monster.MonsterInfo.BloodCastJiFen;
									}
									client.ClientData.BloodCastleAwardPointTmp = client.ClientData.BloodCastleAwardPoint;
									Global.SaveRoleParamsInt32ValueToDB(client, "BloodCastlePlayerPoint", client.ClientData.BloodCastleAwardPoint, false);
									string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.BloodCastleAwardPoint);
									GameManager.ClientMgr.SendToClient(client, strcmd, 532);
									if (monster.MonsterInfo.VLevel >= bcDataTmp.NeedKillMonster1Level && bcTmp.m_bKillMonsterAStatus == 0)
									{
										int killedMonster = Interlocked.Increment(ref bcTmp.m_nKillMonsterACount);
										GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, cmInfo, 533, 0, bcTmp.m_nKillMonsterACount, 1, 0, null);
										if (killedMonster == bcDataTmp.NeedKillMonster1Num)
										{
											bcTmp.m_Step++;
											GameManager.CopyMapMgr.AddGuangMuEvent(cmInfo, 2, 0);
											GameManager.CopyMapMgr.AddGuangMuEvent(cmInfo, 22, 2);
											GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, cmInfo, 518, 0, 0, 0, 0, null);
											GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, cmInfo, 533, 0, 0, 2, 0, null);
											bcTmp.m_bKillMonsterAStatus = 1;
										}
									}
									if (monster.MonsterInfo.ExtensionID == bcDataTmp.GateID)
									{
										bcTmp.m_Step++;
										GameManager.CopyMapMgr.AddGuangMuEvent(cmInfo, 3, 0);
										this.CreateMonsterBBloodCastScene(bcTmp.m_nMapCode, bcDataTmp, bcTmp, client.ClientData.CopyMapID);
										GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, cmInfo, 533, 0, 1, 2, 0, null);
										GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, cmInfo, 533, 0, 0, 3, 0, null);
									}
									if (monster.MonsterInfo.ExtensionID == bcDataTmp.NeedKillMonster2ID && bcTmp.m_bKillMonsterBStatus == 0)
									{
										int killedMonster = Interlocked.Increment(ref bcTmp.m_nKillMonsterBCount);
										GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, cmInfo, 533, 0, bcTmp.m_nKillMonsterBCount, 3, 0, null);
										if (killedMonster == bcDataTmp.NeedKillMonster2Num)
										{
											bcTmp.m_Step++;
											int monsterID = bcDataTmp.CrystalID;
											string[] sfields = bcDataTmp.CrystalPos.Split(new char[]
											{
												','
											});
											int nPosX = Global.SafeConvertToInt32(sfields[0]);
											int nPosY = Global.SafeConvertToInt32(sfields[1]);
											GameMap gameMap = null;
											if (!GameManager.MapMgr.DictMaps.TryGetValue(bcTmp.m_nMapCode, out gameMap))
											{
												LogManager.WriteLog(LogTypes.Error, string.Format("血色城堡报错 地图配置 ID = {0}", bcDataTmp.MapCode), null, true);
												return;
											}
											int gridX = gameMap.CorrectWidthPointToGridPoint(nPosX) / gameMap.MapGridWidth;
											int gridY = gameMap.CorrectHeightPointToGridPoint(nPosY) / gameMap.MapGridHeight;
											GameManager.MonsterZoneMgr.AddDynamicMonsters(bcTmp.m_nMapCode, monsterID, cmInfo.CopyMapID, 1, gridX, gridY, 0, 0, SceneUIClasses.Normal, null, null);
											GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, cmInfo, 533, 0, 0, 4, 0, null);
											bcTmp.m_bKillMonsterBStatus = 1;
										}
									}
									if (monster.MonsterInfo.ExtensionID == bcDataTmp.CrystalID)
									{
										bcTmp.m_Step++;
										GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, cmInfo, 533, 0, 1, 4, 0, null);
										GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, cmInfo, 533, 0, 0, 5, 0, null);
									}
									if (monster.MonsterInfo.ExtensionID == bcDataTmp.DiaoXiangID)
									{
										bcTmp.m_Step++;
										GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, cmInfo, 533, 0, 1, 5, 0, null);
										this.CompleteBloodcastleAndGiveAwards(client, bcTmp, bcDataTmp);
									}
									if (monster.MonsterInfo.ExtensionID == bcDataTmp.CrystalID)
									{
										int monsterID = bcDataTmp.DiaoXiangID;
										string[] sfields = bcDataTmp.DiaoXiangPos.Split(new char[]
										{
											','
										});
										int nPosX = Global.SafeConvertToInt32(sfields[0]);
										int nPosY = Global.SafeConvertToInt32(sfields[1]);
										GameMap gameMap = null;
										if (GameManager.MapMgr.DictMaps.TryGetValue(bcTmp.m_nMapCode, out gameMap))
										{
											int gridX = gameMap.CorrectWidthPointToGridPoint(nPosX) / gameMap.MapGridWidth;
											int gridY = gameMap.CorrectHeightPointToGridPoint(nPosY) / gameMap.MapGridHeight;
											GameManager.MonsterZoneMgr.AddDynamicMonsters(bcTmp.m_nMapCode, monsterID, cmInfo.CopyMapID, 1, gridX, gridY, 0, 0, SceneUIClasses.Normal, null, null);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06001B92 RID: 7058 RVA: 0x001A077C File Offset: 0x0019E97C
		public void GiveAwardBloodCastCopyScene(GameClient client, int nMultiple)
		{
			int FuBenSeqID = Global.GetRoleParamsInt32FromDB(client, "BloodCastleFuBenSeqID");
			int nSceneID = Global.GetRoleParamsInt32FromDB(client, "BloodCastleSceneid");
			int nFlag = Global.GetRoleParamsInt32FromDB(client, "BloodCastleSceneFinishFlag");
			int awardState = GameManager.CopyMapMgr.FindAwardState(client.ClientData.RoleID, FuBenSeqID, nSceneID);
			if (awardState > 0)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(21, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
			}
			else
			{
				BloodCastleDataInfo bcDataTmp = null;
				if (Data.BloodCastleDataInfoList.TryGetValue(nSceneID, out bcDataTmp))
				{
					if (nFlag == 1)
					{
						string[] sItem = bcDataTmp.AwardItem2;
						if (sItem != null && sItem.Length > 0)
						{
							for (int i = 0; i < sItem.Length; i++)
							{
								if (!string.IsNullOrEmpty(sItem[i].Trim()))
								{
									string[] sFields = sItem[i].Split(new char[]
									{
										','
									});
									if (!string.IsNullOrEmpty(sFields[i].Trim()))
									{
										int nGoodsID = Convert.ToInt32(sFields[0].Trim());
										int nGoodsNum = Convert.ToInt32(sFields[1].Trim());
										int nBinding = Convert.ToInt32(sFields[2].Trim());
										GoodsData goodsData = new GoodsData
										{
											Id = -1,
											GoodsID = nGoodsID,
											Using = 0,
											Forge_level = 0,
											Starttime = "1900-01-01 12:00:00",
											Endtime = "1900-01-01 12:00:00",
											Site = 0,
											Quality = 0,
											Props = "",
											GCount = nGoodsNum,
											Binding = nBinding,
											Jewellist = "",
											BagIndex = 0,
											AddPropIndex = 0,
											BornIndex = 0,
											Lucky = 0,
											Strong = 0,
											ExcellenceInfo = 0,
											AppendPropLev = 0,
											ChangeLifeLevForEquip = 0
										};
										string sMsg = "血色堡垒奖励--统一奖励";
										if (!Global.CanAddGoodsNum(client, nGoodsNum))
										{
											Global.UseMailGivePlayerAward(client, goodsData, GLang.GetLang(1, new object[0]), GLang.GetLang(2602, new object[0]), 1.0);
										}
										else
										{
											Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, nGoodsID, nGoodsNum, goodsData.Quality, "", goodsData.Forge_level, goodsData.Binding, 0, "", true, 1, sMsg, goodsData.Endtime, 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
										}
									}
								}
							}
						}
					}
					if (client.ClientData.BloodCastleAwardPoint > 0)
					{
						long nExp = (long)nMultiple * Global.CalcExpForRoleScore(client.ClientData.BloodCastleAwardPoint, bcDataTmp.ExpModulus);
						int nMoney = client.ClientData.BloodCastleAwardPoint * bcDataTmp.MoneyModulus;
						if (nExp > 0L)
						{
							GameManager.ClientMgr.ProcessRoleExperience(client, nExp, false, true, false, "none");
							GameManager.ClientMgr.NotifyAddExpMsg(client, nExp);
						}
						if (nMoney > 0)
						{
							GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, nMoney, "血色城堡副本", false);
							GameManager.ClientMgr.NotifyAddJinBiMsg(client, nMoney);
						}
						if (client.ClientData.BloodCastleAwardPoint > client.ClientData.BloodCastleAwardTotalPoint)
						{
							client.ClientData.BloodCastleAwardTotalPoint = client.ClientData.BloodCastleAwardPoint;
						}
						if (client.ClientData.BloodCastleAwardPoint > this.m_nTotalPointValue)
						{
							this.SetBloodCastleCopySceneTotalPoint(client.ClientData.RoleName, client.ClientData.BloodCastleAwardPoint);
						}
						client.ClientData.BloodCastleAwardPoint = 0;
						Global.SaveRoleParamsInt32ValueToDB(client, "BloodCastlePlayerPoint", client.ClientData.BloodCastleAwardPoint, true);
					}
					GameManager.CopyMapMgr.AddAwardState(client.ClientData.RoleID, FuBenSeqID, nSceneID, 1);
					ProcessTask.ProcessAddTaskVal(client, TaskTypes.BloodCastle, -1, 1, new object[0]);
				}
			}
		}

		// Token: 0x06001B93 RID: 7059 RVA: 0x001A0BF4 File Offset: 0x0019EDF4
		public void LeaveBloodCastCopyScene(GameClient client, bool clearScore = false)
		{
			int nFuBenId = Global.GetRoleParamsInt32FromDB(client, "BloodCastleSceneid");
			if (client.ClientData.CopyMapID >= 0 && client.ClientData.FuBenSeqID >= 0 && this.IsBloodCastleCopyScene(nFuBenId))
			{
				CopyMap cmInfo = null;
				lock (this.m_BloodCastleCopyScenesList)
				{
					if (!this.m_BloodCastleCopyScenesList.TryGetValue(client.ClientData.FuBenSeqID, out cmInfo) || cmInfo == null)
					{
						return;
					}
				}
				Dictionary<int, BloodCastleScene> dicTmp = null;
				lock (this.m_BloodCastleCopyScenesInfo)
				{
					if (!this.m_BloodCastleCopyScenesInfo.TryGetValue(client.ClientData.FuBenID, out dicTmp) || dicTmp == null)
					{
						return;
					}
					BloodCastleScene bcTmp = null;
					if (!dicTmp.TryGetValue(client.ClientData.FuBenSeqID, out bcTmp) || bcTmp == null)
					{
						return;
					}
					Interlocked.Decrement(ref bcTmp.m_nPlarerCount);
					GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, cmInfo, 545, 0, 0, 0, bcTmp.m_nPlarerCount, null);
					if (clearScore && bcTmp.m_eStatus == BloodCastleStatus.FIGHT_STATUS_BEGIN)
					{
						client.ClientData.BloodCastleAwardPoint = 0;
					}
				}
				Global.SaveRoleParamsInt32ValueToDB(client, "BloodCastlePlayerPoint", client.ClientData.BloodCastleAwardPoint, true);
			}
		}

		// Token: 0x06001B94 RID: 7060 RVA: 0x001A0DC8 File Offset: 0x0019EFC8
		public void LogOutWhenInBloodCastleCopyScene(GameClient client)
		{
			this.LeaveBloodCastCopyScene(client, false);
		}

		// Token: 0x06001B95 RID: 7061 RVA: 0x001A0DD4 File Offset: 0x0019EFD4
		public void CompleteBloodCastScene(GameClient client, BloodCastleScene bsInfo, BloodCastleDataInfo bsData)
		{
			int nFlag = Global.GetRoleParamsInt32FromDB(client, "BloodCastleSceneFinishFlag");
			if (nFlag != 1)
			{
				Global.SaveRoleParamsInt32ValueToDB(client, "BloodCastleSceneFinishFlag", 1, true);
			}
		}

		// Token: 0x06001B96 RID: 7062 RVA: 0x001A0E08 File Offset: 0x0019F008
		public void CompleteBloodcastleAndGiveAwards(GameClient client, BloodCastleScene bcTmp, BloodCastleDataInfo bcDataTmp)
		{
			CopyMap cmInfo = GameManager.BloodCastleCopySceneMgr.GetBloodCastleCopySceneInfo(client.ClientData.FuBenSeqID);
			if (cmInfo != null)
			{
				if (bcTmp.m_eStatus != BloodCastleStatus.FIGHT_STATUS_END)
				{
					bcTmp.m_nRoleID = client.ClientData.RoleID;
					string[] sItem = bcDataTmp.AwardItem1;
					if (sItem != null && sItem.Length > 0)
					{
						for (int i = 0; i < sItem.Length; i++)
						{
							if (!string.IsNullOrEmpty(sItem[i].Trim()))
							{
								string[] sFields = sItem[i].Split(new char[]
								{
									','
								});
								if (!string.IsNullOrEmpty(sFields[i].Trim()))
								{
									int nGoodsID = Convert.ToInt32(sFields[0].Trim());
									int nGoodsNum = Convert.ToInt32(sFields[1].Trim());
									int nBinding = Convert.ToInt32(sFields[2].Trim());
									GoodsData goods = new GoodsData
									{
										Id = -1,
										GoodsID = nGoodsID,
										Using = 0,
										Forge_level = 0,
										Starttime = "1900-01-01 12:00:00",
										Endtime = "1900-01-01 12:00:00",
										Site = 0,
										Quality = 0,
										Props = "",
										GCount = nGoodsNum,
										Binding = nBinding,
										Jewellist = "",
										BagIndex = 0,
										AddPropIndex = 0,
										BornIndex = 0,
										Lucky = 0,
										Strong = 0,
										ExcellenceInfo = 0,
										AppendPropLev = 0,
										ChangeLifeLevForEquip = 0
									};
									string sMsg = GLang.GetLang(23, new object[0]);
									if (!Global.CanAddGoodsNum(client, nGoodsNum))
									{
										Global.UseMailGivePlayerAward(client, goods, GLang.GetLang(1, new object[0]), sMsg, 1.0);
									}
									else
									{
										Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, nGoodsID, nGoodsNum, 0, "", 0, goods.Binding, 0, "", true, 1, sMsg, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
									}
								}
							}
						}
					}
					bcTmp.m_eStatus = BloodCastleStatus.FIGHT_STATUS_END;
					bcTmp.m_lEndTime = TimeUtil.NOW();
					bcTmp.m_bIsFinishTask = true;
					this.CompleteBloodCastScene(client, bcTmp, bcDataTmp);
				}
			}
		}

		// Token: 0x06001B97 RID: 7063 RVA: 0x001A1082 File Offset: 0x0019F282
		public void CleanBloodCastScene(int mapid)
		{
		}

		// Token: 0x06001B98 RID: 7064 RVA: 0x001A1088 File Offset: 0x0019F288
		public void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				if (!string.IsNullOrEmpty(this.m_sTotalPointName) && this.m_sTotalPointName == oldName)
				{
					this.m_sTotalPointName = newName;
				}
			}
		}

		// Token: 0x040029E4 RID: 10724
		public Dictionary<int, CopyMap> m_BloodCastleCopyScenesList = new Dictionary<int, CopyMap>();

		// Token: 0x040029E5 RID: 10725
		public Dictionary<int, Dictionary<int, BloodCastleScene>> m_BloodCastleCopyScenesInfo = new Dictionary<int, Dictionary<int, BloodCastleScene>>();

		// Token: 0x040029E6 RID: 10726
		public static object m_Mutex = new object();

		// Token: 0x040029E7 RID: 10727
		public int m_nTotalPointValue = -1;

		// Token: 0x040029E8 RID: 10728
		public string m_sTotalPointName = "";

		// Token: 0x040029E9 RID: 10729
		private static long LastHeartBeatTicks = 0L;
	}
}
