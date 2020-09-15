using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GameServer.Core.Executor;
using Server.Protocol;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x020006D4 RID: 1748
	internal class GlodCopySceneManager
	{
		// Token: 0x060029A0 RID: 10656 RVA: 0x00239648 File Offset: 0x00237848
		public static void AddGlodCopySceneList(int nID, CopyMap mapInfo)
		{
			bool bInsert = false;
			lock (GlodCopySceneManager.m_GlodCopySceneLists)
			{
				CopyMap tmp = null;
				if (!GlodCopySceneManager.m_GlodCopySceneLists.TryGetValue(nID, out tmp))
				{
					GlodCopySceneManager.m_GlodCopySceneLists.Add(nID, mapInfo);
					bInsert = true;
				}
				else if (tmp == null)
				{
					GlodCopySceneManager.m_GlodCopySceneLists[nID] = mapInfo;
					bInsert = true;
				}
				lock (GlodCopySceneManager.m_GlodCopySceneInfo)
				{
					if (bInsert)
					{
						GoldCopyScene GoldCopySceneInfo = null;
						if (!GlodCopySceneManager.m_GlodCopySceneInfo.TryGetValue(nID, out GoldCopySceneInfo))
						{
							GoldCopySceneInfo = new GoldCopyScene();
							GoldCopySceneInfo.InitInfo(mapInfo.MapCode, mapInfo.CopyMapID, nID);
							GoldCopySceneInfo.m_StartTimer = TimeUtil.NOW();
							GlodCopySceneManager.m_GlodCopySceneInfo.Add(nID, GoldCopySceneInfo);
						}
					}
				}
			}
		}

		// Token: 0x060029A1 RID: 10657 RVA: 0x00239768 File Offset: 0x00237968
		public static void RemoveGlodCopySceneList(int nID)
		{
			lock (GlodCopySceneManager.m_GlodCopySceneLists)
			{
				GlodCopySceneManager.m_GlodCopySceneLists.Remove(nID);
			}
			lock (GlodCopySceneManager.m_GlodCopySceneInfo)
			{
				GlodCopySceneManager.m_GlodCopySceneInfo.Remove(nID);
			}
		}

		// Token: 0x060029A2 RID: 10658 RVA: 0x002397F8 File Offset: 0x002379F8
		public static void HeartBeatGlodCopyScene()
		{
			long nowTicks = TimeUtil.NOW();
			if (nowTicks - GlodCopySceneManager.LastHeartBeatTicks >= 1000L)
			{
				GlodCopySceneManager.LastHeartBeatTicks = ((GlodCopySceneManager.LastHeartBeatTicks < 86400000L) ? nowTicks : (GlodCopySceneManager.LastHeartBeatTicks + 1000L));
				lock (GlodCopySceneManager.m_GlodCopySceneLists)
				{
					foreach (CopyMap item in GlodCopySceneManager.m_GlodCopySceneLists.Values)
					{
						List<GameClient> clientsList = item.GetClientsList();
						GoldCopyScene tmpGoldCopySceneData = null;
						lock (GlodCopySceneManager.m_GlodCopySceneInfo)
						{
							if (!GlodCopySceneManager.m_GlodCopySceneInfo.TryGetValue(item.FuBenSeqID, out tmpGoldCopySceneData))
							{
								continue;
							}
						}
						if (tmpGoldCopySceneData != null)
						{
							lock (tmpGoldCopySceneData)
							{
								if (tmpGoldCopySceneData.m_TimeNotifyFlag == 0)
								{
									tmpGoldCopySceneData.m_TimeNotifyFlag = 1;
									if (clientsList.Count<GameClient>() != 0 && clientsList[0] != null)
									{
										GlodCopySceneManager.SendMsgToClientForGlodCopyScenePrepare(clientsList[0], tmpGoldCopySceneData);
									}
								}
								if (nowTicks >= tmpGoldCopySceneData.m_StartTimer + (long)GlodCopySceneManager.m_PrepareTime)
								{
									int nWave = tmpGoldCopySceneData.m_CreateMonsterWave;
									int nCount = Data.Goldcopyscenedata.GoldCopySceneMonsterData.Count<KeyValuePair<int, GoldCopySceneMonster>>();
									if (nWave <= nCount)
									{
										if (nWave == 0 && tmpGoldCopySceneData.m_CreateMonsterFirstWaveFlag == 0)
										{
											tmpGoldCopySceneData.m_CreateMonsterFirstWaveFlag = 1;
											tmpGoldCopySceneData.m_CreateMonsterWave = 1;
										}
										GoldCopySceneMonster tmpMonsterInfo = null;
										if (Data.Goldcopyscenedata.GoldCopySceneMonsterData.TryGetValue(nWave, out tmpMonsterInfo))
										{
											if (tmpMonsterInfo != null)
											{
												if (nowTicks - tmpGoldCopySceneData.m_CreateMonsterTick2 > (long)(tmpMonsterInfo.m_Delay2 * 1000))
												{
													if (tmpGoldCopySceneData.m_CreateMonsterWaveNotify == 0)
													{
														tmpGoldCopySceneData.m_CreateMonsterWaveNotify = 1;
														if (clientsList.Count<GameClient>() != 0 && clientsList[0] != null)
														{
															GlodCopySceneManager.SendMsgToClientForGlodCopySceneMonsterWave(clientsList[0], tmpGoldCopySceneData.m_CreateMonsterWave);
														}
													}
													if (nowTicks - tmpGoldCopySceneData.m_CreateMonsterTick1 > (long)(tmpMonsterInfo.m_Delay1 * 1000))
													{
														if (tmpGoldCopySceneData.m_LoginEnterFlag == 1)
														{
															if (clientsList.Count<GameClient>() != 0 && clientsList[0] != null && nowTicks - tmpGoldCopySceneData.m_LoginEnterTimer > (long)GlodCopySceneManager.m_DelayTime)
															{
																tmpGoldCopySceneData.m_LoginEnterFlag = 0;
																GlodCopySceneManager.SendMsgToClientForGlodCopySceneMonsterWave(clientsList[0], tmpGoldCopySceneData.m_CreateMonsterWave);
															}
														}
														tmpGoldCopySceneData.m_CreateMonsterTick1 = ((tmpGoldCopySceneData.m_CreateMonsterTick1 < 86400000L) ? nowTicks : (tmpGoldCopySceneData.m_CreateMonsterTick1 + (long)(tmpMonsterInfo.m_Delay1 * 1000)));
														if (clientsList.Count<GameClient>() != 0 && clientsList[0] != null)
														{
															GlodCopySceneManager.CreateMonsterForGoldCopyScene(clientsList[0], tmpGoldCopySceneData, tmpGoldCopySceneData.m_CreateMonsterWave);
														}
														else
														{
															GlodCopySceneManager.CreateMonsterForGoldCopyScene(null, tmpGoldCopySceneData, tmpGoldCopySceneData.m_CreateMonsterWave);
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
		}

		// Token: 0x060029A3 RID: 10659 RVA: 0x00239BF8 File Offset: 0x00237DF8
		public static void CreateMonsterForGoldCopyScene(GameClient client, GoldCopyScene goldcopyscene, int nWave)
		{
			GoldCopySceneMonster tmpInfo = Data.Goldcopyscenedata.GoldCopySceneMonsterData[nWave];
			long ticks = TimeUtil.NOW();
			int nRom = Global.GetRandomNumber(0, 10);
			int[] pos = Data.Goldcopyscenedata.m_MonsterPatorlPathList[0];
			Point toPos = new Point((double)pos[0], (double)pos[1]);
			GameManager.MonsterZoneMgr.AddDynamicMonsters(goldcopyscene.m_MapCodeID, tmpInfo.m_MonsterID[nRom], goldcopyscene.m_CopyMapID, 1, (int)toPos.X, (int)toPos.Y, 1, 0, SceneUIClasses.Normal, null, null);
			goldcopyscene.m_CreateMonsterCount++;
			if (goldcopyscene.m_CreateMonsterCount == tmpInfo.m_Num)
			{
				goldcopyscene.m_CreateMonsterTick2 = ticks;
				goldcopyscene.m_CreateMonsterWave = nWave + 1;
				goldcopyscene.m_CreateMonsterCount = 0;
				goldcopyscene.m_CreateMonsterWaveNotify = 0;
				if (goldcopyscene.m_CreateMonsterWave > Data.Goldcopyscenedata.GoldCopySceneMonsterData.Count<KeyValuePair<int, GoldCopySceneMonster>>() && client != null)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(376, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				}
			}
		}

		// Token: 0x060029A4 RID: 10660 RVA: 0x00239D2C File Offset: 0x00237F2C
		public static void MonsterMoveStepGoldCopySceneCopyMap(Monster monster)
		{
			long ticks = TimeUtil.NOW();
			if (ticks - monster.MoveTime >= 500L)
			{
				int nStep = monster.Step;
				int nNumStep = monster.PatrolPath.Count<int[]>() - 1;
				int nNextStep = nStep + 1;
				if (nNextStep >= nNumStep)
				{
					GameManager.MonsterMgr.AddDelayDeadMonster(monster);
				}
				else
				{
					int nMapCode = 5100;
					MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[nMapCode];
					int nNextX = monster.PatrolPath[nNextStep][0];
					int nNextY = monster.PatrolPath[nNextStep][1];
					int gridX = nNextX / mapGrid.MapGridWidth;
					int gridY = nNextY / mapGrid.MapGridHeight;
					Point ToGrid = new Point((double)gridX, (double)gridY);
					Point grid = monster.CurrentGrid;
					int nCurrX = (int)grid.X;
					int nCurrY = (int)grid.Y;
					double Direction = Global.GetDirectionByAspect(gridX, gridY, nCurrX, nCurrY);
					ChuanQiUtils.WalkTo(monster, (Dircetions)Direction);
					monster.MoveTime = ticks;
					if (Global.GetTwoPointDistance(ToGrid, grid) < 2.0)
					{
						monster.Step = nStep + 1;
					}
				}
			}
		}

		// Token: 0x060029A5 RID: 10661 RVA: 0x00239E58 File Offset: 0x00238058
		public static void SendMsgToClientForGlodCopyScenePrepare(GameClient client, GoldCopyScene goldcopyscene)
		{
			if (client != null)
			{
				int fuBenID = FuBenManager.FindFuBenIDByMapCode(client.ClientData.MapCode);
				if (fuBenID > 0)
				{
					long ticks = TimeUtil.NOW();
					int nTimer = (int)(((long)GlodCopySceneManager.m_PrepareTime - (ticks - goldcopyscene.m_StartTimer)) / 1000L);
					string strcmd = string.Format("{0}", nTimer);
					GameManager.ClientMgr.SendToClient(client, strcmd, 627);
				}
			}
		}

		// Token: 0x060029A6 RID: 10662 RVA: 0x00239EDC File Offset: 0x002380DC
		public static void SendMsgToClientForGlodCopySceneMonsterWave(GameClient client, int nWave)
		{
			if (client != null)
			{
				int fuBenID = FuBenManager.FindFuBenIDByMapCode(client.ClientData.MapCode);
				if (fuBenID > 0)
				{
					string strcmd = string.Format("{0}:{1}", nWave, Data.Goldcopyscenedata.GoldCopySceneMonsterData.Count<KeyValuePair<int, GoldCopySceneMonster>>());
					TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strcmd, 628);
					Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true);
				}
			}
		}

		// Token: 0x060029A7 RID: 10663 RVA: 0x00239F68 File Offset: 0x00238168
		public static bool EnterGoldCopySceneWhenLogin(GameClient client, bool bContinue = true)
		{
			bool result;
			if (client != null)
			{
				CopyMap tmp = null;
				GoldCopyScene GoldCopySceneInfo = null;
				lock (GlodCopySceneManager.m_GlodCopySceneLists)
				{
					if (!GlodCopySceneManager.m_GlodCopySceneLists.TryGetValue(client.ClientData.FuBenSeqID, out tmp) || tmp == null)
					{
						return false;
					}
				}
				lock (GlodCopySceneManager.m_GlodCopySceneInfo)
				{
					if (!GlodCopySceneManager.m_GlodCopySceneInfo.TryGetValue(client.ClientData.FuBenSeqID, out GoldCopySceneInfo) || GoldCopySceneInfo == null)
					{
						return false;
					}
				}
				long ticks = TimeUtil.NOW();
				GoldCopySceneInfo.m_LoginEnterTimer = ticks;
				GoldCopySceneInfo.m_LoginEnterFlag = 1;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x04003950 RID: 14672
		public static int m_PrepareTime = 10000;

		// Token: 0x04003951 RID: 14673
		public static int m_DelayTime = 2000;

		// Token: 0x04003952 RID: 14674
		public static Dictionary<int, CopyMap> m_GlodCopySceneLists = new Dictionary<int, CopyMap>();

		// Token: 0x04003953 RID: 14675
		public static Dictionary<int, GoldCopyScene> m_GlodCopySceneInfo = new Dictionary<int, GoldCopyScene>();

		// Token: 0x04003954 RID: 14676
		private static long LastHeartBeatTicks = 0L;
	}
}
