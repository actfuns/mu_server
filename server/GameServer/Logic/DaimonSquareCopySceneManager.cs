using System;
using System.Collections.Generic;
using System.Threading;
using GameServer.Core.Executor;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class DaimonSquareCopySceneManager
	{
		
		public void InitDaimonSquareCopyScene()
		{
			Global.QueryDayActivityTotalPointInfoToDB(SpecialActivityTypes.DemoSque);
		}

		
		public void SetDaimonSquareCopySceneTotalPoint(string sName, int nPoint)
		{
			this.m_nDaimonSquareMaxName = sName;
			this.m_nDaimonSquareMaxPoint = nPoint;
		}

		
		public bool CanEnterExistCopyScene(GameClient client)
		{
			CopyMap copyMap = null;
			int fuBenSeqId = client.ClientData.FuBenSeqID;
			lock (this.m_DaimonSquareCopyScenesList)
			{
				if (!this.m_DaimonSquareCopyScenesList.TryGetValue(fuBenSeqId, out copyMap))
				{
					return false;
				}
			}
			bool result;
			lock (this.m_DaimonSquareCopyScenesInfo)
			{
				Dictionary<int, DaimonSquareScene> dicTmp = null;
				DaimonSquareScene bcData = null;
				int nFubenID = copyMap.FubenMapID;
				if (!this.m_DaimonSquareCopyScenesInfo.TryGetValue(nFubenID, out dicTmp))
				{
					result = false;
				}
				else if (!dicTmp.TryGetValue(fuBenSeqId, out bcData))
				{
					result = false;
				}
				else if (bcData.m_eStatus >= DaimonSquareStatus.FIGHT_STATUS_END)
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

		
		public void AddDaimonSquareCopyScenes(int nSequenceID, int nFubenID, int nMapCodeID, CopyMap mapInfo)
		{
			lock (this.m_DaimonSquareCopyScenesList)
			{
				CopyMap cmInfo = null;
				if (!this.m_DaimonSquareCopyScenesList.TryGetValue(nSequenceID, out cmInfo) || cmInfo == null)
				{
					this.m_DaimonSquareCopyScenesList.Add(nSequenceID, mapInfo);
				}
			}
			lock (this.m_DaimonSquareCopyScenesInfo)
			{
				Dictionary<int, DaimonSquareScene> dicTmp = null;
				DaimonSquareScene bcData = null;
				if (!this.m_DaimonSquareCopyScenesInfo.TryGetValue(nFubenID, out dicTmp))
				{
					dicTmp = new Dictionary<int, DaimonSquareScene>();
					this.m_DaimonSquareCopyScenesInfo.Add(nFubenID, dicTmp);
				}
				if (!dicTmp.TryGetValue(nSequenceID, out bcData))
				{
					bcData = new DaimonSquareScene();
					bcData.CleanAllInfo();
					dicTmp[nSequenceID] = bcData;
				}
				bcData.m_nMapCode = nMapCodeID;
				bcData.m_CopyMap = mapInfo;
			}
		}

		
		public void RemoveDaimonSquareListCopyScenes(CopyMap cmInfo, int nSqeID, int nCopyID)
		{
			lock (this.m_DaimonSquareCopyScenesList)
			{
				CopyMap cmTmp = null;
				if (this.m_DaimonSquareCopyScenesList.TryGetValue(nSqeID, out cmTmp) && cmTmp != null)
				{
					this.m_DaimonSquareCopyScenesList.Remove(nSqeID);
				}
			}
			lock (this.m_DaimonSquareCopyScenesInfo)
			{
				Dictionary<int, DaimonSquareScene> dicTmp = null;
				if (this.m_DaimonSquareCopyScenesInfo.TryGetValue(nCopyID, out dicTmp) && dicTmp != null)
				{
					DaimonSquareScene bcTmp = null;
					if (dicTmp.TryGetValue(nSqeID, out bcTmp) && bcTmp != null)
					{
						dicTmp.Remove(nSqeID);
					}
					if (dicTmp.Count <= 0)
					{
						this.m_DaimonSquareCopyScenesInfo.Remove(nCopyID);
					}
				}
			}
		}

		
		public int CheckDaimonSquareListScenes(int nFuBenMapID)
		{
			lock (this.m_DaimonSquareCopyScenesInfo)
			{
				Dictionary<int, DaimonSquareScene> tmpData = null;
				if (!this.m_DaimonSquareCopyScenesInfo.TryGetValue(nFuBenMapID, out tmpData))
				{
					return -1;
				}
				if (tmpData == null)
				{
					return -1;
				}
				DaimonSquareDataInfo dsDataTmp = null;
				if (!Data.DaimonSquareDataInfoList.TryGetValue(nFuBenMapID, out dsDataTmp))
				{
					return -1;
				}
				if (dsDataTmp == null)
				{
					return -1;
				}
				foreach (KeyValuePair<int, DaimonSquareScene> dsData in tmpData)
				{
					int nID = dsData.Key;
					DaimonSquareScene tmpdsinfo = dsData.Value;
					if (nID >= 0 && tmpdsinfo != null)
					{
						if (nID == nFuBenMapID && tmpdsinfo.m_nPlarerCount < dsDataTmp.MaxEnterNum && tmpdsinfo.m_eStatus < DaimonSquareStatus.FIGHT_STATUS_BEGIN)
						{
							return nID;
						}
					}
				}
			}
			return -1;
		}

		
		public bool IsDaimonSquareCopyScene(int nFuBenMapID)
		{
			return Data.DaimonSquareDataInfoList.ContainsKey(nFuBenMapID);
		}

		
		public bool IsDaimonSquareCopyScene2(int nMpaCodeID)
		{
			SceneUIClasses sceneType = Global.GetMapSceneType(nMpaCodeID);
			return sceneType == SceneUIClasses.Demon;
		}

		
		public CopyMap GetDaimonSquareCopySceneInfo(int nSequenceID)
		{
			CopyMap result;
			if (nSequenceID < 0)
			{
				result = null;
			}
			else
			{
				CopyMap copymapInfo = null;
				if (!this.m_DaimonSquareCopyScenesList.TryGetValue(nSequenceID, out copymapInfo))
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

		
		public DaimonSquareScene GetDaimonSquareCopySceneDataInfo(CopyMap cmInfo, int nSequenceID, int nFuBenID)
		{
			DaimonSquareScene result;
			if (cmInfo == null || nSequenceID < 0)
			{
				result = null;
			}
			else
			{
				Dictionary<int, DaimonSquareScene> dicTmp = null;
				if (!this.m_DaimonSquareCopyScenesInfo.TryGetValue(nFuBenID, out dicTmp) || dicTmp == null)
				{
					result = null;
				}
				else
				{
					DaimonSquareScene dsInfo = null;
					if (!dicTmp.TryGetValue(nSequenceID, out dsInfo) || dsInfo == null)
					{
						result = null;
					}
					else
					{
						result = dsInfo;
					}
				}
			}
			return result;
		}

		
		public int EnterDaimonSquareSceneCopySceneCount(GameClient client, int nFubenID, out int nDemonNum)
		{
			nDemonNum = -1;
			DaimonSquareDataInfo dsDataTmp = null;
			int result;
			if (!Data.DaimonSquareDataInfoList.TryGetValue(nFubenID, out dsDataTmp))
			{
				result = -1;
			}
			else
			{
				int nDate = TimeUtil.NowDateTime().DayOfYear;
				int nType = 2;
				int nCount = Global.QueryDayActivityEnterCountToDB(client, client.ClientData.RoleID, nDate, nType);
				nDemonNum = nCount;
				if (nCount >= dsDataTmp.MaxEnterNum)
				{
					bool nRet = true;
					int dayID = TimeUtil.NowDateTime().DayOfYear;
					int nVipLev = client.ClientData.VipLevel;
					int[] nArry = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPEnterDaimonSquareCountAddValue", ',');
					if (nVipLev > 0 && nArry != null && nArry[nVipLev] > 0)
					{
						int nNum = nArry[nVipLev];
						if (nCount < dsDataTmp.MaxEnterNum + nNum)
						{
							Global.UpdateVipDailyData(client, dayID, 1000002);
							nRet = false;
						}
					}
					if (nRet)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(101, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						return -1;
					}
				}
				result = 1;
			}
			return result;
		}

		
		public void SendMegToClient(GameClient client, int nFubenID, int nSquID, int nCmdID)
		{
			CopyMap cmInfo = null;
			lock (this.m_DaimonSquareCopyScenesList)
			{
				if (!this.m_DaimonSquareCopyScenesList.TryGetValue(nSquID, out cmInfo) || cmInfo == null)
				{
					return;
				}
			}
			lock (this.m_DaimonSquareCopyScenesInfo)
			{
				Dictionary<int, DaimonSquareScene> dicTmp = null;
				if (this.m_DaimonSquareCopyScenesInfo.TryGetValue(nFubenID, out dicTmp) && dicTmp != null)
				{
					DaimonSquareScene dsTmp = null;
					if (dicTmp.TryGetValue(nSquID, out dsTmp) && dsTmp != null)
					{
						DaimonSquareDataInfo dsDataTmp = null;
						if (Data.DaimonSquareDataInfoList.TryGetValue(nFubenID, out dsDataTmp) && dsDataTmp != null)
						{
							if (nCmdID == 536)
							{
								long ticks = TimeUtil.NOW();
								if (dsTmp.m_lPrepareTime > 0L)
								{
									int nTimer = 15;
									if (dsTmp.m_eStatus == DaimonSquareStatus.FIGHT_STATUS_PREPARE)
									{
										nTimer = (int)(((long)(dsDataTmp.PrepareTime * 1000) - (ticks - dsTmp.m_lPrepareTime)) / 1000L);
									}
									else if (dsTmp.m_eStatus == DaimonSquareStatus.FIGHT_STATUS_BEGIN)
									{
										nTimer = (int)(((long)(dsDataTmp.DurationTime * 1000) - (ticks - dsTmp.m_lBeginTime)) / 1000L);
									}
									string strcmd = string.Format("{0}:{1}", (int)dsTmp.m_eStatus, nTimer);
									GameManager.ClientMgr.SendToClient(client, strcmd, 536);
								}
							}
							else if (nCmdID == 546)
							{
								if (dsTmp.m_eStatus <= DaimonSquareStatus.FIGHT_STATUS_PREPARE)
								{
									string strcmd = string.Format("{0}", dsTmp.m_nPlarerCount);
									GameManager.ClientMgr.NotifyDaimonSquareCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, cmInfo, 546, 0, 0, 0, dsTmp.m_nPlarerCount);
								}
							}
						}
					}
				}
			}
		}

		
		public int EnterDaimonSquareSceneCopyScene(GameClient client, int nFubenID, int nDemonNum, out int nSeqID, int mapCode)
		{
			nSeqID = -1;
			if (client.ClientData.DaimonSquarePoint > 0)
			{
				int FuBenSeqID = Global.GetRoleParamsInt32FromDB(client, "DaimonSquareFuBenSeqID");
				int nSceneID = Global.GetRoleParamsInt32FromDB(client, "DaimonSquareSceneid");
				int nFlag = Global.GetRoleParamsInt32FromDB(client, "DaimonSquareSceneFinishFlag");
				int awardState = GameManager.CopyMapMgr.FindAwardState(client.ClientData.RoleID, FuBenSeqID, nSceneID);
				if (awardState == 0)
				{
					DaimonSquareDataInfo dsDataTmp = null;
					if (Data.DaimonSquareDataInfoList.TryGetValue(nSceneID, out dsDataTmp))
					{
						if (dsDataTmp == null)
						{
							client.ClientData.DaimonSquarePoint = 0;
							return 1;
						}
						string sAwardItem = null;
						for (int i = 0; i < dsDataTmp.AwardItem.Length; i++)
						{
							sAwardItem += dsDataTmp.AwardItem[i];
							if (i != dsDataTmp.AwardItem.Length - 1)
							{
								sAwardItem += "|";
							}
						}
						string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							nFlag,
							client.ClientData.DaimonSquarePoint,
							Global.CalcExpForRoleScore(client.ClientData.DaimonSquarePoint, dsDataTmp.ExpModulus),
							client.ClientData.DaimonSquarePoint * dsDataTmp.MoneyModulus,
							sAwardItem
						});
						GameManager.ClientMgr.SendToClient(client, strcmd, 538);
						return -1;
					}
				}
				else
				{
					client.ClientData.DaimonSquarePoint = 0;
					Global.SaveRoleParamsInt32ValueToDB(client, "DaimonSquarePlayerPoint", client.ClientData.DaimonSquarePoint, true);
				}
			}
			int nFubenMapID = Global.GetDaimonSquareCopySceneIDForRole(client);
			int result;
			if (nFubenMapID <= 0)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(102, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 39);
				LogManager.WriteLog(LogTypes.Error, string.Format("enter bloodcastle scene fail!! get scene info fail!!!!", new object[0]), null, true);
				result = -1;
			}
			else
			{
				DaimonSquareDataInfo bcInfo = null;
				if (!Data.DaimonSquareDataInfoList.TryGetValue(nFubenMapID, out bcInfo) || bcInfo == null)
				{
					result = -1;
				}
				else if (!Global.CanEnterDaimonSquareOnTime(bcInfo.BeginTime, bcInfo.PrepareTime))
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(103, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = -1;
				}
				else
				{
					GoodsData goodData = Global.GetGoodsByID(client, bcInfo.NeedGoodsID);
					if (goodData == null)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(104, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						result = -1;
					}
					else if (goodData.GCount < bcInfo.NeedGoodsNum)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(105, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						result = -1;
					}
					else
					{
						bool usedBinding = false;
						bool usedTimeLimited = false;
						if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, bcInfo.NeedGoodsID, 1, false, out usedBinding, out usedTimeLimited, false))
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(106, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							result = -1;
						}
						else
						{
							Dictionary<int, DaimonSquareScene> dicTmp = null;
							lock (this.m_DaimonSquareCopyScenesInfo)
							{
								if (this.m_DaimonSquareCopyScenesInfo.TryGetValue(nFubenMapID, out dicTmp) && dicTmp != null)
								{
									foreach (KeyValuePair<int, DaimonSquareScene> dssceneInfo in dicTmp)
									{
										if (dssceneInfo.Value.m_eStatus < DaimonSquareStatus.FIGHT_STATUS_BEGIN)
										{
											if (dssceneInfo.Value.m_nPlarerCount < bcInfo.MaxPlayerNum)
											{
												dssceneInfo.Value.m_nPlarerCount++;
												nSeqID = dssceneInfo.Key;
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
											DaimonSquareScene bcData = null;
											if (!this.m_DaimonSquareCopyScenesInfo.TryGetValue(nFubenID, out dicTmp) || dicTmp == null)
											{
												dicTmp = new Dictionary<int, DaimonSquareScene>();
												this.m_DaimonSquareCopyScenesInfo.Add(nFubenID, dicTmp);
											}
											if (!dicTmp.TryGetValue(nSeqID, out bcData) || bcData == null)
											{
												bcData = new DaimonSquareScene();
												bcData.CleanAllInfo();
												bcData.m_nMapCode = mapCode;
												bcData.m_nPlarerCount = 1;
												dicTmp[nSeqID] = bcData;
											}
										}
									}
								}
							}
							Global.SaveRoleParamsInt32ValueToDB(client, "DaimonSquareFuBenSeqID", nSeqID, true);
							Global.SaveRoleParamsInt32ValueToDB(client, "DaimonSquareSceneid", nFubenMapID, true);
							Global.SaveRoleParamsInt32ValueToDB(client, "DaimonSquareSceneFinishFlag", 0, true);
							Global.SaveRoleParamsInt32ValueToDB(client, "DaimonSquareSceneTimer", 0, true);
							result = 0;
						}
					}
				}
			}
			return result;
		}

		
		public void HeartBeatDaimonSquareScene()
		{
			long nowTicks = TimeUtil.NOW();
			if (Math.Abs(nowTicks - DaimonSquareCopySceneManager.LastHeartBeatTicks) >= 1000L)
			{
				DaimonSquareCopySceneManager.LastHeartBeatTicks = nowTicks;
				lock (this.m_DaimonSquareCopyScenesList)
				{
					foreach (KeyValuePair<int, CopyMap> DaimonSquareScenes in this.m_DaimonSquareCopyScenesList)
					{
						int nID = DaimonSquareScenes.Value.FuBenSeqID;
						int nCopyID = DaimonSquareScenes.Value.FubenMapID;
						int nMapCodeID = DaimonSquareScenes.Value.MapCode;
						if (nID >= 0 && nCopyID >= 0 && nMapCodeID >= 0)
						{
							lock (this.m_DaimonSquareCopyScenesInfo)
							{
								DaimonSquareDataInfo dsDataTmp = null;
								if (Data.DaimonSquareDataInfoList.TryGetValue(nCopyID, out dsDataTmp) && dsDataTmp != null)
								{
									Dictionary<int, DaimonSquareScene> dicTmp = null;
									if (this.m_DaimonSquareCopyScenesInfo.TryGetValue(nCopyID, out dicTmp) && dicTmp != null)
									{
										DaimonSquareScene dsTmp = null;
										if (dicTmp.TryGetValue(nID, out dsTmp) && dsTmp != null)
										{
											long ticks = TimeUtil.NOW();
											if (dsTmp.m_eStatus == DaimonSquareStatus.FIGHT_STATUS_NULL)
											{
												int nSecond = 0;
												string strTimer = null;
												if (Global.CanEnterDaimonSquareCopySceneOnTime(dsDataTmp.BeginTime, dsDataTmp.PrepareTime + dsDataTmp.DurationTime, out nSecond, out strTimer))
												{
													dsTmp.m_eStatus = DaimonSquareStatus.FIGHT_STATUS_PREPARE;
													DateTime staticTime = DateTime.Parse(strTimer);
													dsTmp.m_lPrepareTime = staticTime.Ticks / 10000L;
													dsTmp.m_nMonsterTotalWave = dsDataTmp.MonsterID.Length;
													List<GameClient> objsList = DaimonSquareScenes.Value.GetClientsList();
													if (null == objsList)
													{
														break;
													}
													for (int i = 0; i < objsList.Count; i++)
													{
														this.SendMegToClient(objsList[i], nCopyID, nID, 536);
														this.SendMegToClient(objsList[i], nCopyID, nID, 546);
													}
												}
											}
											else if (dsTmp.m_eStatus == DaimonSquareStatus.FIGHT_STATUS_PREPARE)
											{
												if (ticks >= dsTmp.m_lPrepareTime + (long)(dsDataTmp.PrepareTime * 1000))
												{
													dsTmp.m_eStatus = DaimonSquareStatus.FIGHT_STATUS_BEGIN;
													dsTmp.m_lBeginTime = TimeUtil.NOW();
													int nTimer = (int)(((long)(dsDataTmp.DurationTime * 1000) - (ticks - dsTmp.m_lBeginTime)) / 1000L);
													GameManager.ClientMgr.NotifyDaimonSquareCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, DaimonSquareScenes.Value, 536, 3, nTimer, 0, 0, 0);
													List<GameClient> clientList = dsTmp.m_CopyMap.GetClientsList();
													if (null != clientList)
													{
														foreach (GameClient c in clientList)
														{
															if (dsTmp.AddRole(c))
															{
																Global.UpdateRoleEnterActivityCount(c, SpecialActivityTypes.DemoSque);
															}
														}
													}
												}
											}
											else if (dsTmp.m_eStatus == DaimonSquareStatus.FIGHT_STATUS_BEGIN)
											{
												if (ticks < dsTmp.m_lBeginTime + 20000L)
												{
													List<GameClient> clientList = dsTmp.m_CopyMap.GetClientsList();
													if (null != clientList)
													{
														foreach (GameClient c in clientList)
														{
															if (dsTmp.AddRole(c))
															{
																LogManager.WriteLog(LogTypes.Error, string.Format("EnterCount#DemoSque#rid={0}", c.ClientData.RoleID), null, true);
																Global.UpdateRoleEnterActivityCount(c, SpecialActivityTypes.DemoSque);
															}
														}
													}
												}
												bool bNeedCreateMonster = false;
												lock (dsTmp.m_CreateMonsterMutex)
												{
													if (dsTmp.m_nCreateMonsterFlag == 0 && dsTmp.m_nMonsterWave < dsTmp.m_nMonsterTotalWave)
													{
														bNeedCreateMonster = true;
													}
													if (ticks >= dsTmp.m_lBeginTime + (long)(dsDataTmp.DurationTime * 1000) || dsTmp.m_nKillMonsterTotalNum == dsDataTmp.MonsterSum)
													{
														dsTmp.m_eStatus = DaimonSquareStatus.FIGHT_STATUS_END;
														dsTmp.m_lEndTime = TimeUtil.NOW();
														try
														{
															string log = string.Format("恶魔广场已结束,是否完成{0},结束时间{1},m_nCreateMonsterFlag:{2},m_nMonsterWave:{3},m_nMonsterTotalWave:{4},m_nKillMonsterNum:{5},m_nNeedKillMonsterNum:{6}", new object[]
															{
																dsTmp.m_bIsFinishTask,
																new DateTime(dsTmp.m_lEndTime * 10000L),
																dsTmp.m_nCreateMonsterFlag,
																dsTmp.m_nMonsterWave,
																dsTmp.m_nMonsterTotalWave,
																dsTmp.m_nKillMonsterNum,
																dsTmp.m_nNeedKillMonsterNum
															});
															LogManager.WriteLog(LogTypes.Error, log, null, true);
														}
														catch
														{
														}
													}
												}
												if (bNeedCreateMonster)
												{
													this.DaimonSquareSceneCreateMonster(dsTmp, dsDataTmp, DaimonSquareScenes.Value.CopyMapID, DaimonSquareScenes.Value);
												}
											}
											else if (dsTmp.m_eStatus == DaimonSquareStatus.FIGHT_STATUS_END)
											{
												int nTimer = (int)(((long)(dsDataTmp.LeaveTime * 1000) - (ticks - dsTmp.m_lEndTime)) / 1000L);
												if (!dsTmp.m_bEndFlag)
												{
													GameManager.ClientMgr.NotifyDaimonSquareCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, DaimonSquareScenes.Value, 536, 4, nTimer, 0, 0, 0);
													long nTimeInfo = dsTmp.m_lEndTime - dsTmp.m_lBeginTime;
													long nRemain = ((long)(dsDataTmp.DurationTime * 1000) - nTimeInfo) / 1000L;
													if (nRemain >= (long)dsDataTmp.DurationTime)
													{
														nRemain = (long)(dsDataTmp.DurationTime / 2);
													}
													int nTimeAward = (int)((long)dsDataTmp.TimeModulus * nRemain);
													if (nTimeAward < 0)
													{
														nTimeAward = 0;
													}
													GameManager.ClientMgr.NotifyDaimonSquareCopySceneMsgEndFight(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, DaimonSquareScenes.Value, dsTmp, 538, nTimeAward);
													dsTmp.m_bEndFlag = true;
												}
												if (ticks >= dsTmp.m_lEndTime + (long)(dsDataTmp.LeaveTime * 1000) + 3000L)
												{
													try
													{
														if (DaimonSquareScenes.Value == GameManager.CopyMapMgr.FindCopyMap(DaimonSquareScenes.Value.MapCode, DaimonSquareScenes.Value.FuBenSeqID))
														{
															if (!dsTmp.ClearRole)
															{
																dsTmp.ClearRole = true;
																List<GameClient> objsList = DaimonSquareScenes.Value.GetClientsList();
																if (objsList != null && objsList.Count > 0)
																{
																	string log = string.Format("恶魔广场已结束,是否完成{0},结束时间{1},m_nCreateMonsterFlag:{2},m_nMonsterWave:{3},m_nMonsterTotalWave:{4},m_nKillMonsterNum:{5},m_nNeedKillMonsterNum:{6},踢出玩家列表:", new object[]
																	{
																		dsTmp.m_bIsFinishTask,
																		new DateTime(dsTmp.m_lEndTime * 10000L),
																		dsTmp.m_nCreateMonsterFlag,
																		dsTmp.m_nMonsterWave,
																		dsTmp.m_nMonsterTotalWave,
																		dsTmp.m_nKillMonsterNum,
																		dsTmp.m_nNeedKillMonsterNum
																	});
																	LogManager.WriteLog(LogTypes.Error, log, null, true);
																}
															}
														}
													}
													catch (Exception ex)
													{
														DataHelper.WriteExceptionLogEx(ex, "恶魔广场调度异常");
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		
		public void DaimonSquareSceneCreateMonster(DaimonSquareScene bcTmp, DaimonSquareDataInfo bcDataTmp, int nCopyMapID, CopyMap cmInfo)
		{
			string[] sNum = null;
			string[] sID = null;
			lock (bcTmp.m_CreateMonsterMutex)
			{
				if (bcTmp.m_nMonsterWave >= bcTmp.m_nMonsterTotalWave)
				{
					return;
				}
				bcTmp.m_nCreateMonsterFlag = 1;
				string sMonsterNum = bcDataTmp.MonsterNum[bcTmp.m_nMonsterWave];
				string sMonsterID = bcDataTmp.MonsterID[bcTmp.m_nMonsterWave];
				string sNeedSkillMonster = bcDataTmp.CreateNextWaveMonsterCondition[bcTmp.m_nMonsterWave];
				if (sMonsterID == null || sMonsterNum == null || sNeedSkillMonster == null)
				{
					return;
				}
				sNum = sMonsterNum.Split(new char[]
				{
					','
				});
				sID = sMonsterID.Split(new char[]
				{
					','
				});
				string[] sRate = sNeedSkillMonster.Split(new char[]
				{
					','
				});
				if (sNum.Length != sID.Length)
				{
					return;
				}
				for (int i = 0; i < sNum.Length; i++)
				{
					int nNum = Global.SafeConvertToInt32(sNum[i]);
					int nID = Global.SafeConvertToInt32(sID[i]);
					for (int j = 0; j < nNum; j++)
					{
						bcTmp.m_nCreateMonsterCount++;
					}
				}
				bcTmp.m_nNeedKillMonsterNum = (int)Math.Ceiling((double)bcTmp.m_nCreateMonsterCount * Global.SafeConvertToDouble(sRate[0]) / 100.0);
				bcTmp.m_nMonsterWave++;
			}
			GameMap gameMap = null;
			if (!GameManager.MapMgr.DictMaps.TryGetValue(bcTmp.m_nMapCode, out gameMap))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("恶魔广场报错 地图配置 ID = {0}", bcDataTmp.MapCode), null, true);
			}
			else
			{
				int gridX = gameMap.CorrectWidthPointToGridPoint(bcDataTmp.posX + Global.GetRandomNumber(-bcDataTmp.Radius, bcDataTmp.Radius)) / gameMap.MapGridWidth;
				int gridY = gameMap.CorrectHeightPointToGridPoint(bcDataTmp.posZ + Global.GetRandomNumber(-bcDataTmp.Radius, bcDataTmp.Radius)) / gameMap.MapGridHeight;
				int gridNum = gameMap.CorrectWidthPointToGridPoint(bcDataTmp.Radius);
				for (int i = 0; i < sNum.Length; i++)
				{
					int nNum = Global.SafeConvertToInt32(sNum[i]);
					int nID = Global.SafeConvertToInt32(sID[i]);
					for (int j = 0; j < nNum; j++)
					{
						GameManager.MonsterZoneMgr.AddDynamicMonsters(bcTmp.m_nMapCode, nID, nCopyMapID, 1, gridX, gridY, gridNum, 0, SceneUIClasses.Normal, null, null);
					}
				}
				GameManager.ClientMgr.NotifyDaimonSquareCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, cmInfo, 537, 0, 0, bcDataTmp.MonsterID.Length - bcTmp.m_nMonsterWave, -100, 0);
			}
		}

		
		public void OnStartPlayGame(GameClient client)
		{
			if (client.ClientData.FuBenSeqID >= 0 && client.ClientData.CopyMapID >= 0 && this.IsDaimonSquareCopyScene(client.ClientData.FuBenID))
			{
				DaimonSquareDataInfo bcDataTmp = null;
				if (Data.DaimonSquareDataInfoList.TryGetValue(client.ClientData.FuBenID, out bcDataTmp) && bcDataTmp != null)
				{
					Dictionary<int, DaimonSquareScene> dicTmp = null;
					if (this.m_DaimonSquareCopyScenesInfo.TryGetValue(client.ClientData.FuBenID, out dicTmp) && dicTmp != null)
					{
						DaimonSquareScene bcTmp = null;
						if (dicTmp.TryGetValue(client.ClientData.FuBenSeqID, out bcTmp) && bcTmp != null)
						{
							string strcmd = string.Format("{0}:{1}", bcDataTmp.MonsterID.Length - bcTmp.m_nMonsterWave, client.ClientData.DaimonSquarePoint);
							GameManager.ClientMgr.SendToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strcmd, 537);
						}
					}
				}
			}
		}

		
		public void DaimonSquareSceneKillMonster(GameClient client, Monster monster)
		{
			if (client.ClientData.FuBenSeqID >= 0 && client.ClientData.CopyMapID >= 0 && this.IsDaimonSquareCopyScene(client.ClientData.FuBenID))
			{
				DaimonSquareDataInfo bcDataTmp = null;
				if (Data.DaimonSquareDataInfoList.TryGetValue(client.ClientData.FuBenID, out bcDataTmp) && bcDataTmp != null)
				{
					Dictionary<int, DaimonSquareScene> dicTmp = null;
					if (this.m_DaimonSquareCopyScenesInfo.TryGetValue(client.ClientData.FuBenID, out dicTmp) && dicTmp != null)
					{
						DaimonSquareScene bcTmp = null;
						if (dicTmp.TryGetValue(client.ClientData.FuBenSeqID, out bcTmp) && bcTmp != null)
						{
							if (bcTmp.AddKilledMonster(monster))
							{
								if (!bcTmp.m_bEndFlag)
								{
									bool bCompleteDaimonSquareScene = false;
									lock (bcTmp.m_CreateMonsterMutex)
									{
										bcTmp.m_nKillMonsterNum++;
										client.ClientData.DaimonSquarePoint += monster.MonsterInfo.DaimonSquareJiFen;
										if (bcTmp.m_nCreateMonsterFlag == 1 && bcTmp.m_nKillMonsterNum == bcTmp.m_nNeedKillMonsterNum)
										{
											bcTmp.m_nCreateMonsterFlag = 0;
											bcTmp.m_nNeedKillMonsterNum = 0;
											bcTmp.m_nKillMonsterNum = 0;
											bcTmp.m_nCreateMonsterCount = 0;
										}
										if (bcTmp.m_nKillMonsterTotalNum == bcDataTmp.MonsterSum)
										{
											bcTmp.m_eStatus = DaimonSquareStatus.FIGHT_STATUS_END;
											bcTmp.m_lEndTime = TimeUtil.NOW();
											bcTmp.m_bIsFinishTask = true;
										}
									}
									if (bCompleteDaimonSquareScene)
									{
										DaimonSquareCopySceneManager.CompleteDaimonSquareScene(client, bcTmp, bcDataTmp);
									}
									string strcmd = string.Format("{0}:{1}", bcDataTmp.MonsterID.Length - bcTmp.m_nMonsterWave, client.ClientData.DaimonSquarePoint);
									GameManager.ClientMgr.SendToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strcmd, 537);
								}
							}
						}
					}
				}
			}
		}

		
		public static void CompleteDaimonSquareScene(GameClient client, DaimonSquareScene bsInfo, DaimonSquareDataInfo dsData)
		{
			int nFlag = Global.GetRoleParamsInt32FromDB(client, "DaimonSquareSceneFinishFlag");
			if (nFlag != 1)
			{
				Global.SaveRoleParamsInt32ValueToDB(client, "DaimonSquareSceneFinishFlag", 1, true);
				long nTimer = bsInfo.m_lEndTime - bsInfo.m_lBeginTime;
				long nRemain = ((long)(dsData.DurationTime * 1000) - nTimer) / 1000L;
				Global.SaveRoleParamsInt32ValueToDB(client, "DaimonSquareSceneTimer", (int)nRemain, true);
			}
		}

		
		public void GiveAwardDaimonSquareCopyScene(GameClient client, int nMultiple)
		{
			int FuBenSeqID = Global.GetRoleParamsInt32FromDB(client, "DaimonSquareFuBenSeqID");
			int nSceneID = Global.GetRoleParamsInt32FromDB(client, "DaimonSquareSceneid");
			int nFlag = Global.GetRoleParamsInt32FromDB(client, "DaimonSquareSceneFinishFlag");
			int nTimer = Global.GetRoleParamsInt32FromDB(client, "DaimonSquareSceneTimer");
			int awardState = GameManager.CopyMapMgr.FindAwardState(client.ClientData.RoleID, FuBenSeqID, nSceneID);
			if (awardState > 0)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(21, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
			}
			else
			{
				DaimonSquareDataInfo bcDataTmp = null;
				if (Data.DaimonSquareDataInfoList.TryGetValue(nSceneID, out bcDataTmp))
				{
					if (bcDataTmp != null)
					{
						if (nFlag == 1)
						{
							string[] sItem = bcDataTmp.AwardItem;
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
											int nBinding = 1;
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
											string sMsg = "恶魔广场--统一奖励";
											if (!Global.CanAddGoodsNum(client, nGoodsNum))
											{
												Global.UseMailGivePlayerAward(client, goodsData, GLang.GetLang(2, new object[0]), GLang.GetLang(2603, new object[0]), 1.0);
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
						int nTimeAward = bcDataTmp.TimeModulus * nTimer;
						if (client.ClientData.DaimonSquarePoint > 0)
						{
							long nExp = (long)nMultiple * Global.CalcExpForRoleScore(client.ClientData.DaimonSquarePoint, bcDataTmp.ExpModulus);
							int nMoney = client.ClientData.DaimonSquarePoint * bcDataTmp.MoneyModulus;
							if (nExp > 0L)
							{
								GameManager.ClientMgr.ProcessRoleExperience(client, nExp, false, true, false, "none");
								GameManager.ClientMgr.NotifyAddExpMsg(client, nExp);
							}
							if (nMoney > 0)
							{
								GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, nMoney, "恶魔广场副本", false);
								GameManager.ClientMgr.NotifyAddJinBiMsg(client, nMoney);
							}
							if (client.ClientData.DaimonSquarePoint > client.ClientData.DaimonSquarePointTotalPoint)
							{
								client.ClientData.DaimonSquarePointTotalPoint = client.ClientData.DaimonSquarePoint;
							}
							if (client.ClientData.DaimonSquarePoint > this.m_nDaimonSquareMaxPoint)
							{
								this.SetDaimonSquareCopySceneTotalPoint(client.ClientData.RoleName, client.ClientData.DaimonSquarePoint);
							}
							client.ClientData.DaimonSquarePoint = 0;
							Global.SaveRoleParamsInt32ValueToDB(client, "DaimonSquarePlayerPoint", client.ClientData.DaimonSquarePoint, true);
						}
						GameManager.CopyMapMgr.AddAwardState(client.ClientData.RoleID, FuBenSeqID, nSceneID, 1);
						ProcessTask.ProcessAddTaskVal(client, TaskTypes.Daimon, -1, 1, new object[0]);
					}
				}
			}
		}

		
		public void LeaveDaimonSquareCopyScene(GameClient client, bool clearScore = false)
		{
			int nFuBenId = Global.GetRoleParamsInt32FromDB(client, "DaimonSquareSceneid");
			if (client.ClientData.CopyMapID >= 0 && client.ClientData.FuBenSeqID >= 0 && this.IsDaimonSquareCopyScene(nFuBenId))
			{
				CopyMap cmInfo = null;
				lock (this.m_DaimonSquareCopyScenesList)
				{
					if (!this.m_DaimonSquareCopyScenesList.TryGetValue(client.ClientData.FuBenSeqID, out cmInfo) || cmInfo == null)
					{
						return;
					}
				}
				Dictionary<int, DaimonSquareScene> dicTmp = null;
				lock (this.m_DaimonSquareCopyScenesInfo)
				{
					if (!this.m_DaimonSquareCopyScenesInfo.TryGetValue(client.ClientData.FuBenID, out dicTmp) || dicTmp == null)
					{
						return;
					}
					DaimonSquareScene bcTmp = null;
					if (!dicTmp.TryGetValue(client.ClientData.FuBenSeqID, out bcTmp) || bcTmp == null)
					{
						return;
					}
					Interlocked.Decrement(ref bcTmp.m_nPlarerCount);
					if (bcTmp.m_eStatus < DaimonSquareStatus.FIGHT_STATUS_BEGIN || (bcTmp.m_eStatus == DaimonSquareStatus.FIGHT_STATUS_BEGIN && TimeUtil.NOW() < bcTmp.m_lBeginTime + 30000L))
					{
						GameManager.ClientMgr.NotifyDaimonSquareCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, cmInfo, 546, 0, 0, 0, bcTmp.m_nPlarerCount);
					}
					if (clearScore && bcTmp.m_eStatus == DaimonSquareStatus.FIGHT_STATUS_BEGIN)
					{
						client.ClientData.DaimonSquarePoint = 0;
					}
				}
				Global.SaveRoleParamsInt32ValueToDB(client, "DaimonSquarePlayerPoint", client.ClientData.DaimonSquarePoint, true);
			}
		}

		
		public void LogOutWhenInDaimonSquareCopyMap(GameClient client)
		{
			this.LeaveDaimonSquareCopyScene(client, false);
		}

		
		public static void CleanDaimonSquareCopyScene(int mapid)
		{
		}

		
		public void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				if (!string.IsNullOrEmpty(this.m_nDaimonSquareMaxName) && this.m_nDaimonSquareMaxName == oldName)
				{
					this.m_nDaimonSquareMaxName = newName;
				}
			}
		}

		
		public Dictionary<int, CopyMap> m_DaimonSquareCopyScenesList = new Dictionary<int, CopyMap>();

		
		public Dictionary<int, Dictionary<int, DaimonSquareScene>> m_DaimonSquareCopyScenesInfo = new Dictionary<int, Dictionary<int, DaimonSquareScene>>();

		
		public static object m_Mutex = new object();

		
		public int m_nDaimonSquareMaxPoint = -1;

		
		public string m_nDaimonSquareMaxName = "";

		
		private static long LastHeartBeatTicks = 0L;
	}
}
