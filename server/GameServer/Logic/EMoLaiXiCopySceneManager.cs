using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	internal class EMoLaiXiCopySceneManager
	{
		
		public static void LoadEMoLaiXiCopySceneInfo()
		{
			try
			{
				int totalWave = 0;
				EMoLaiXiCopySceneManager.EMoLaiXiCopySencedata.FaildEscapeMonsterNum = (int)GameManager.systemParamsList.GetParamValueIntByName("BaoWeiZhan", -1);
				XElement xmlFile = Global.GetGameResXml(string.Format("Config/EMoLaiXiLuXian.xml", new object[0]));
				foreach (XElement args in xmlFile.Elements("Path"))
				{
					int id = (int)Global.GetSafeAttributeLong(args, "ID");
					string sPatorlPathID = Global.GetSafeAttributeStr(args, "Path");
					List<int[]> pointList = new List<int[]>();
					if (string.IsNullOrEmpty(sPatorlPathID))
					{
						LogManager.WriteLog(LogTypes.Warning, string.Format("金币副本怪路径为空", new object[0]), null, true);
					}
					else
					{
						string[] fields = sPatorlPathID.Split(new char[]
						{
							'|'
						});
						if (fields.Length <= 0)
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("金币副本怪路径为空", new object[0]), null, true);
						}
						else
						{
							for (int i = 0; i < fields.Length; i++)
							{
								string[] sa = fields[i].Split(new char[]
								{
									','
								});
								if (sa.Length != 2)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("解析{0}文件中的奖励项时失败,坐标有误", "Config/EMoLaiXiLuXian.xml"), null, true);
								}
								else
								{
									int[] pos = Global.StringArray2IntArray(sa);
									pointList.Add(pos);
								}
							}
						}
					}
					EMoLaiXiCopySceneManager.EMoLaiXiCopySencedata.m_MonsterPatorlPathLists.Add(id, pointList);
				}
				xmlFile = Global.GetGameResXml(string.Format("Config/EMoLaiXi.xml", new object[0]));
				IEnumerable<XElement> MonstersXEle = xmlFile.Elements("FuBen");
				foreach (XElement xmlItem in MonstersXEle)
				{
					if (null != xmlItem)
					{
						EMoLaiXiCopySenceMonster tmpEMoLaiXiCopySceneMon = new EMoLaiXiCopySenceMonster();
						tmpEMoLaiXiCopySceneMon.m_MonsterID = new List<int>();
						int id = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						tmpEMoLaiXiCopySceneMon.m_ID = id;
						tmpEMoLaiXiCopySceneMon.m_Wave = (int)Global.GetSafeAttributeLong(xmlItem, "WaveID");
						tmpEMoLaiXiCopySceneMon.m_Delay1 = (int)Global.GetSafeAttributeLong(xmlItem, "Delay1");
						tmpEMoLaiXiCopySceneMon.m_Delay2 = (int)Global.GetSafeAttributeLong(xmlItem, "Delay2");
						string pathIDArray = Global.GetSafeAttributeStr(xmlItem, "PathID");
						tmpEMoLaiXiCopySceneMon.PathIDArray = Global.String2IntArray(pathIDArray, ',');
						string sMonstersID = Global.GetSafeAttributeStr(xmlItem, "MonsterList");
						if (string.IsNullOrEmpty(sMonstersID))
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("金币副本怪ID为空", new object[0]), null, true);
						}
						else
						{
							string[] fields = sMonstersID.Split(new char[]
							{
								'|'
							});
							if (fields.Length <= 0)
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("金币副本怪ID为空", new object[0]), null, true);
							}
							else
							{
								for (int i = 0; i < fields.Length; i++)
								{
									int[] Monsters = Global.String2IntArray(fields[i], ',');
									if (Monsters != null && Monsters.Length == 2 && Monsters[0] > 0 && Monsters[1] > 0)
									{
										for (int j = 0; j < Monsters[1]; j++)
										{
											tmpEMoLaiXiCopySceneMon.m_MonsterID.Add(Monsters[0]);
											tmpEMoLaiXiCopySceneMon.m_Num++;
										}
									}
								}
							}
						}
						EMoLaiXiCopySceneManager.EMoLaiXiCopySencedata.EMoLaiXiCopySenceMonsterData.Add(tmpEMoLaiXiCopySceneMon);
						totalWave = Global.GMax(totalWave, tmpEMoLaiXiCopySceneMon.m_Wave);
					}
				}
				EMoLaiXiCopySceneManager.EMoLaiXiCopySencedata.TotalWave = totalWave;
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/JinBiFuBen.xml", new object[0])));
			}
		}

		
		public static void AddEMoLaiXiCopySceneList(int nID, CopyMap mapInfo)
		{
			bool bInsert = false;
			lock (EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneLists)
			{
				CopyMap tmp = null;
				if (!EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneLists.TryGetValue(nID, out tmp))
				{
					EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneLists.Add(nID, mapInfo);
					bInsert = true;
				}
				else if (tmp == null)
				{
					EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneLists[nID] = mapInfo;
					bInsert = true;
				}
				lock (EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneInfo)
				{
					if (bInsert)
					{
						EMoLaiXiCopySence eMoLaiXiCopySenceInfo = null;
						if (!EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneInfo.TryGetValue(nID, out eMoLaiXiCopySenceInfo))
						{
							eMoLaiXiCopySenceInfo = new EMoLaiXiCopySence();
							eMoLaiXiCopySenceInfo.InitInfo(mapInfo.MapCode, mapInfo.CopyMapID, nID);
							eMoLaiXiCopySenceInfo.m_StartTimer = TimeUtil.NOW();
							EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneInfo.Add(nID, eMoLaiXiCopySenceInfo);
						}
					}
				}
			}
		}

		
		public static void RemoveEMoLaiXiCopySceneList(int nID, int copyMapID)
		{
			lock (EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneLists)
			{
				EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneLists.Remove(nID);
			}
			lock (EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneInfo)
			{
				EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneInfo.Remove(nID);
			}
			lock (EMoLaiXiCopySceneManager.m_EMoLaiXiEscapeMonsterNumDict)
			{
				EMoLaiXiCopySceneManager.m_EMoLaiXiEscapeMonsterNumDict.Remove(copyMapID);
			}
		}

		
		public static void HeartBeatEMoLaiXiCopyScene()
		{
			long nowTicks = TimeUtil.NOW();
			if (nowTicks - EMoLaiXiCopySceneManager.LastHeartBeatTicks >= 1000L)
			{
				EMoLaiXiCopySceneManager.LastHeartBeatTicks = ((EMoLaiXiCopySceneManager.LastHeartBeatTicks < 86400000L) ? nowTicks : (EMoLaiXiCopySceneManager.LastHeartBeatTicks + 1000L));
				lock (EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneLists)
				{
					foreach (CopyMap item in EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneLists.Values)
					{
						EMoLaiXiCopySence scene = null;
						lock (EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneInfo)
						{
							if (!EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneInfo.TryGetValue(item.FuBenSeqID, out scene))
							{
								continue;
							}
						}
						if (scene != null)
						{
							List<GameClient> clientsList = item.GetClientsList();
							lock (scene)
							{
								if (scene.m_TimeNotifyFlag == 0)
								{
									if (nowTicks <= scene.m_StartTimer + (long)EMoLaiXiCopySceneManager.m_PrepareTime - 3000L)
									{
										continue;
									}
									scene.m_TimeNotifyFlag = 1;
									string msgCmd = string.Format("{0}:{1}${2}${3}", new object[]
									{
										2,
										3,
										1,
										""
									});
									GameManager.ClientMgr.BroadSpecialCopyMapMessage(419, msgCmd, clientsList, true);
								}
								if (nowTicks >= scene.m_StartTimer + (long)EMoLaiXiCopySceneManager.m_PrepareTime)
								{
									if (scene.m_Delay2 > 0L)
									{
										EMoLaiXiCopySceneManager.OnSceneTimer(scene, clientsList, item, nowTicks);
									}
									else
									{
										EMoLaiXiCopySceneManager.InitNextWaveMonsterList(scene);
									}
								}
							}
						}
					}
				}
			}
		}

		
		public static void OnSceneTimer(EMoLaiXiCopySence scene, List<GameClient> clientList, CopyMap copyMap, long nowTicks)
		{
			int nWave = scene.m_CreateMonsterWave;
			int nCount = EMoLaiXiCopySceneManager.EMoLaiXiCopySencedata.TotalWave;
			bool notifyWaveAndNum = false;
			bool notifyEnd = false;
			int escapeCount = EMoLaiXiCopySceneManager.GetEscapeCount(scene.m_CopyMapID);
			if (escapeCount > 0)
			{
				scene.m_EscapedMonsterNum += escapeCount;
				notifyWaveAndNum = true;
			}
			if (scene.m_LoginEnterFlag == 1)
			{
				if (nowTicks - scene.m_LoginEnterTimer > (long)EMoLaiXiCopySceneManager.m_DelayTime)
				{
					scene.m_LoginEnterFlag = 0;
					notifyWaveAndNum = true;
				}
			}
			if (scene.m_EscapedMonsterNum >= EMoLaiXiCopySceneManager.EMoLaiXiCopySencedata.FaildEscapeMonsterNum)
			{
				if (!scene.m_bFinished)
				{
					GameManager.CopyMapMgr.CopyMapFaildForAll(clientList, copyMap);
					scene.m_bFinished = true;
				}
				GameManager.CopyMapMgr.KillAllMonster(copyMap);
				notifyWaveAndNum = true;
			}
			else if (scene.m_bAllMonsterCreated)
			{
				if (!scene.m_bFinished)
				{
					if (copyMap.KilledDynamicMonsterNum + scene.m_EscapedMonsterNum >= scene.m_TotalMonsterCountAllWave)
					{
						if (clientList != null && clientList.Count > 0)
						{
							notifyWaveAndNum = true;
							GameManager.CopyMapMgr.CopyMapPassAwardForAll(clientList[0], copyMap, true);
							scene.m_bFinished = true;
							if (copyMap.KilledDynamicMonsterNum > copyMap.TotalDynamicMonsterNum)
							{
								try
								{
									string log = string.Format("恶魔来袭已成功,但杀怪计数异常,结束时间{0},KilledDynamicMonsterNum:{1},m_EscapedMonsterNum:{2},m_TotalMonsterCountAllWave:{3}", new object[]
									{
										new DateTime(nowTicks * 10000L),
										copyMap.KilledDynamicMonsterNum,
										scene.m_EscapedMonsterNum,
										scene.m_TotalMonsterCountAllWave
									});
									LogManager.WriteLog(LogTypes.Error, log, null, true);
								}
								catch
								{
								}
							}
						}
					}
				}
			}
			else if (nowTicks - scene.m_CreateMonsterTick2 > scene.m_Delay2 * 1000L)
			{
				if (scene.m_CreateMonsterWaveNotify == 0)
				{
					scene.m_CreateMonsterWaveNotify = 1;
					notifyWaveAndNum = true;
				}
				for (int i = 0; i < scene.m_CreateWaveMonsterList.Count; i++)
				{
					EMoLaiXiCopySenceMonster tmpInfo = scene.m_CreateWaveMonsterList[i];
					if (tmpInfo.m_CreateMonsterCount < tmpInfo.m_Num)
					{
						if (nowTicks - tmpInfo.m_CreateMonsterTick1 > (long)(tmpInfo.m_Delay1 * 1000))
						{
							int nIndex = tmpInfo.m_CreateMonsterCount;
							int[] pos = tmpInfo.PatrolPath[0];
							GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_MapCodeID, tmpInfo.m_MonsterID[nIndex], scene.m_CopyMapID, 1, pos[0], pos[1], 0, 0, SceneUIClasses.EMoLaiXiCopy, tmpInfo.PatrolPath, null);
							tmpInfo.m_CreateMonsterCount++;
							scene.m_CreateMonsterCount++;
							tmpInfo.m_CreateMonsterTick1 = nowTicks;
						}
					}
				}
				if (scene.m_CreateMonsterCount >= scene.m_TotalMonsterCount)
				{
					scene.m_CreateMonsterTick2 = nowTicks;
					scene.m_CreateMonsterWave++;
					scene.m_CreateMonsterCount = 0;
					scene.m_CreateMonsterWaveNotify = 0;
					scene.m_Delay2 = 0L;
					notifyWaveAndNum = true;
					copyMap.TotalDynamicMonsterNum = scene.m_TotalMonsterCountAllWave;
					if (scene.m_CreateMonsterWave >= EMoLaiXiCopySceneManager.EMoLaiXiCopySencedata.TotalWave)
					{
						scene.m_Delay2 = 2147483647L;
						scene.m_bAllMonsterCreated = true;
						notifyEnd = true;
					}
				}
			}
			if (notifyWaveAndNum)
			{
				EMoLaiXiCopySceneManager.SendMsgToClientForEMoLaiXiCopySceneMonsterWave(clientList, scene.m_EscapedMonsterNum, scene.m_CreateMonsterWave, EMoLaiXiCopySceneManager.EMoLaiXiCopySencedata.TotalWave, EMoLaiXiCopySceneManager.EMoLaiXiCopySencedata.FaildEscapeMonsterNum);
			}
			if (notifyEnd && null != clientList)
			{
				foreach (GameClient client in clientList)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(108, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				}
			}
		}

		
		public static void InitNextWaveMonsterList(EMoLaiXiCopySence scene)
		{
			if (scene.m_CreateMonsterWave >= 0 && scene.m_CreateMonsterWave < EMoLaiXiCopySceneManager.EMoLaiXiCopySencedata.TotalWave)
			{
				int delay2 = 1;
				int totalNum = 0;
				scene.m_CreateWaveMonsterList.Clear();
				foreach (EMoLaiXiCopySenceMonster i in EMoLaiXiCopySceneManager.EMoLaiXiCopySencedata.EMoLaiXiCopySenceMonsterData)
				{
					if (i.m_Wave == scene.m_CreateMonsterWave + 1)
					{
						EMoLaiXiCopySenceMonster em = i.CloneMini();
						scene.m_CreateWaveMonsterList.Add(em);
						int random = Global.GetRandomNumber(0, em.PathIDArray.Length);
						int pathID = em.PathIDArray[random];
						em.PatrolPath = EMoLaiXiCopySceneManager.EMoLaiXiCopySencedata.m_MonsterPatorlPathLists[pathID];
						delay2 = Global.GMax(delay2, em.m_Delay2);
						totalNum += em.m_Num;
					}
				}
				scene.m_Delay2 = (long)delay2;
				scene.m_TotalMonsterCount = totalNum;
				scene.m_TotalMonsterCountAllWave += totalNum;
			}
		}

		
		public static void IncreaceEscapeCount(int copyMapID)
		{
			lock (EMoLaiXiCopySceneManager.m_EMoLaiXiEscapeMonsterNumDict)
			{
				int count;
				if (EMoLaiXiCopySceneManager.m_EMoLaiXiEscapeMonsterNumDict.TryGetValue(copyMapID, out count))
				{
					EMoLaiXiCopySceneManager.m_EMoLaiXiEscapeMonsterNumDict[copyMapID] = count + 1;
				}
				else
				{
					EMoLaiXiCopySceneManager.m_EMoLaiXiEscapeMonsterNumDict[copyMapID] = 1;
				}
			}
		}

		
		public static int GetEscapeCount(int copyMapID)
		{
			int count;
			lock (EMoLaiXiCopySceneManager.m_EMoLaiXiEscapeMonsterNumDict)
			{
				if (EMoLaiXiCopySceneManager.m_EMoLaiXiEscapeMonsterNumDict.TryGetValue(copyMapID, out count))
				{
					EMoLaiXiCopySceneManager.m_EMoLaiXiEscapeMonsterNumDict[copyMapID] = 0;
				}
				else
				{
					count = 0;
				}
			}
			return count;
		}

		
		public static void MonsterMoveStepEMoLaiXiCopySenceCopyMap(Monster monster)
		{
			long ticks = TimeUtil.NOW();
			if (ticks - monster.MoveTime >= 500L)
			{
				int nStep = monster.Step;
				int nNumStep = monster.PatrolPath.Count<int[]>() - 1;
				int nNextStep = nStep + 1;
				if (nNextStep >= nNumStep)
				{
					EMoLaiXiCopySceneManager.IncreaceEscapeCount(monster.CopyMapID);
					GameManager.MonsterMgr.DeadMonsterImmediately(monster);
				}
				else
				{
					int nMapCode = monster.CurrentMapCode;
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
					if (ChuanQiUtils.WalkTo(monster, (Dircetions)Direction) || ChuanQiUtils.WalkTo(monster, (Dircetions)((Direction + 7.0) % 8.0)) || ChuanQiUtils.WalkTo(monster, (Dircetions)((Direction + 9.0) % 8.0)) || ChuanQiUtils.WalkTo(monster, (Dircetions)((Direction + 6.0) % 8.0)) || ChuanQiUtils.WalkTo(monster, (Dircetions)((Direction + 10.0) % 8.0)) || ChuanQiUtils.WalkTo(monster, (Dircetions)((Direction + 5.0) % 8.0)) || ChuanQiUtils.WalkTo(monster, (Dircetions)((Direction + 11.0) % 8.0)))
					{
						monster.MoveTime = ticks;
					}
					if (Global.GetTwoPointDistance(ToGrid, grid) < 2.0)
					{
						monster.Step = nStep + 1;
					}
				}
			}
		}

		
		public static void SendMsgToClientForEMoLaiXiCopySceneMonsterWave(List<GameClient> clientList, int escapeNum, int nWave, int totalWave, int faildEscapeNum)
		{
			if (clientList != null && clientList.Count > 0)
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					escapeNum,
					nWave,
					EMoLaiXiCopySceneManager.EMoLaiXiCopySencedata.TotalWave,
					faildEscapeNum
				});
				GameManager.ClientMgr.BroadSpecialCopyMapMessage(679, strcmd, clientList, false);
			}
		}

		
		public static bool EnterEMoLaiXiCopySenceWhenLogin(GameClient client, bool bContinue = true)
		{
			bool result;
			if (client != null)
			{
				CopyMap tmp = null;
				EMoLaiXiCopySence EMoLaiXiCopySenceInfo = null;
				lock (EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneLists)
				{
					if (!EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneLists.TryGetValue(client.ClientData.FuBenSeqID, out tmp) || tmp == null)
					{
						return false;
					}
				}
				lock (EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneInfo)
				{
					if (!EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneInfo.TryGetValue(client.ClientData.FuBenSeqID, out EMoLaiXiCopySenceInfo) || EMoLaiXiCopySenceInfo == null)
					{
						return false;
					}
				}
				if (EMoLaiXiCopySenceInfo.m_bFinished)
				{
					result = false;
				}
				else
				{
					long ticks = TimeUtil.NOW();
					EMoLaiXiCopySenceInfo.m_LoginEnterTimer = ticks;
					EMoLaiXiCopySenceInfo.m_LoginEnterFlag = 1;
					result = true;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public static EMoLaiXiCopySenceData EMoLaiXiCopySencedata = new EMoLaiXiCopySenceData();

		
		public static int m_PrepareTime = 9000;

		
		public static int m_DelayTime = 5000;

		
		public static Dictionary<int, CopyMap> m_EMoLaiXiCopySceneLists = new Dictionary<int, CopyMap>();

		
		public static Dictionary<int, EMoLaiXiCopySence> m_EMoLaiXiCopySceneInfo = new Dictionary<int, EMoLaiXiCopySence>();

		
		public static Dictionary<int, int> m_EMoLaiXiEscapeMonsterNumDict = new Dictionary<int, int>();

		
		public static int EMoLaiXiCopySceneMapCode = 4100;

		
		private static long LastHeartBeatTicks = 0L;
	}
}
