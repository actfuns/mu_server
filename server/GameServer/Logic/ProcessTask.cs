using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Server;
using KF.Client;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class ProcessTask
	{
		
		public static void InitBranchTasks(Dictionary<int, SystemXmlItem> taskXmlDic)
		{
			if (taskXmlDic != null)
			{
				Dictionary<int, List<BranchTaskInfo>> _BranchTaskInfoDic = new Dictionary<int, List<BranchTaskInfo>>();
				Dictionary<long, List<int>> _ActiveBranchTaskInfoDic = new Dictionary<long, List<int>>();
				foreach (KeyValuePair<int, SystemXmlItem> t in taskXmlDic)
				{
					if (t.Value.GetIntValue("TaskClass", -1) == 1)
					{
						string ziDongTaskStr = t.Value.GetStringValue("ZiDongTask");
						if (!string.IsNullOrEmpty(ziDongTaskStr))
						{
							string[] fields = ziDongTaskStr.Split(new char[]
							{
								'|'
							});
							foreach (string field in fields)
							{
								string[] ziDongArr = field.Split(new char[]
								{
									','
								});
								if (ziDongArr.Length != 2)
								{
									LogManager.WriteLog(LogTypes.Fatal, string.Format("systemtasks.xml 中，任务编号为 {0} 的任务配置为支线任务，但是 ZiDongTask 字段配置错误，请检查。", t.Key), null, true);
								}
								else
								{
									BranchTaskInfo branchTaskInfo = new BranchTaskInfo();
									branchTaskInfo.TaskID = t.Key;
									branchTaskInfo.triggerType = (BranchTaskTriggerType)Convert.ToInt32(ziDongArr[0]);
									branchTaskInfo.triggerParam = Convert.ToInt64(ziDongArr[1]);
									List<BranchTaskInfo> BranchTaskInfoList = null;
									if (!_BranchTaskInfoDic.TryGetValue(branchTaskInfo.TaskID, out BranchTaskInfoList))
									{
										BranchTaskInfoList = new List<BranchTaskInfo>();
										_BranchTaskInfoDic.Add(branchTaskInfo.TaskID, BranchTaskInfoList);
									}
									BranchTaskInfoList.Add(branchTaskInfo);
									if (branchTaskInfo.triggerType == BranchTaskTriggerType.CompleteTask)
									{
										List<int> list = null;
										if (!_ActiveBranchTaskInfoDic.TryGetValue(branchTaskInfo.triggerParam, out list))
										{
											list = new List<int>();
											_ActiveBranchTaskInfoDic.Add(branchTaskInfo.triggerParam, list);
										}
										list.Add(branchTaskInfo.TaskID);
									}
								}
							}
						}
					}
				}
				ProcessTask.BranchTaskInfoDic = _BranchTaskInfoDic;
				ProcessTask.ActiveBranchTaskInfoDic = _ActiveBranchTaskInfoDic;
			}
		}

		
		public bool IsBranchTask(int taskID)
		{
			SystemXmlItem systemTaskItem = null;
			return GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTaskItem) && systemTaskItem.GetIntValue("TaskClass", -1) == 1;
		}

		
		public static List<BranchTaskInfo> GetBranchTaskTriggerType(int taskID)
		{
			List<BranchTaskInfo> branchTaskInfoList = null;
			ProcessTask.BranchTaskInfoDic.TryGetValue(taskID, out branchTaskInfoList);
			return branchTaskInfoList;
		}

		
		public static List<int> GetActiveBranchTaskInfo(long taskID)
		{
			List<int> list = null;
			ProcessTask.ActiveBranchTaskInfoDic.TryGetValue(taskID, out list);
			return list;
		}

		
		public static void ProcessTakeBranchTasks(GameClient client, BranchTaskTriggerType type, long param)
		{
			if (type == BranchTaskTriggerType.CompleteTask)
			{
				List<int> branchList = ProcessTask.GetActiveBranchTaskInfo(param);
				if (branchList != null && branchList.Count > 0)
				{
					foreach (int branchTaskID in branchList)
					{
						SystemXmlItem systemTaskItem = null;
						if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(branchTaskID, out systemTaskItem))
						{
							List<BranchTaskInfo> BranchTaskInfoList = ProcessTask.GetBranchTaskTriggerType(branchTaskID);
							if (BranchTaskInfoList == null || BranchTaskInfoList.Count <= 0)
							{
								break;
							}
							bool bActive = true;
							foreach (BranchTaskInfo taskInfo in BranchTaskInfoList)
							{
								if (taskInfo.triggerType == BranchTaskTriggerType.CompleteTask)
								{
									SystemXmlItem systemTask = null;
									if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue((int)taskInfo.triggerParam, out systemTask))
									{
										bActive = false;
										break;
									}
									int taskClass = systemTask.GetIntValue("TaskClass", -1);
									if (param != taskInfo.triggerParam)
									{
										if (0 == taskClass)
										{
											if ((long)client.ClientData.MainTaskID < taskInfo.triggerParam)
											{
												bActive = false;
												break;
											}
										}
										else
										{
											if (1 != taskClass)
											{
												bActive = false;
												break;
											}
											OldTaskData oldTaskData = Global.FindOldTaskByTaskID(client, (int)taskInfo.triggerParam);
											if (oldTaskData == null || oldTaskData.DoCount <= 0)
											{
												bActive = false;
												break;
											}
										}
									}
								}
							}
							if (bActive)
							{
								TCPOutPacket tcpOutPacketTemp = null;
								TCPProcessCmdResults result = Global.TakeNewTask(Global._TCPManager, client.ClientSocket, Global._TCPManager.tcpClientPool, Global._TCPManager.tcpRandKey, Global._TCPManager.TcpOutPacketPool, 125, client, client.ClientData.RoleID, systemTaskItem.GetIntValue("ID", -1), systemTaskItem.GetIntValue("SourceNPC", -1), out tcpOutPacketTemp);
								if (result == TCPProcessCmdResults.RESULT_DATA && tcpOutPacketTemp != null)
								{
									client.sendCmd(tcpOutPacketTemp, true);
								}
							}
						}
					}
				}
			}
		}

		
		private static void UpdateTaskDataToDB(GameClient client, TaskData taskData)
		{
			GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				client.ClientData.RoleID,
				taskData.DoingTaskID,
				taskData.DbID,
				taskData.DoingTaskFocus,
				taskData.DoingTaskVal1,
				taskData.DoingTaskVal2
			}), null, client.ServerId);
		}

		
		public static void ProcessAddTaskVal(GameClient client, TaskTypes taskType, int targetId, int val, params object[] args)
		{
			if (null != client.ClientData.TaskDataList)
			{
				long nowTicks = TimeUtil.NOW();
				SystemXmlItem systemTask = null;
				lock (client.ClientData.TaskDataList)
				{
					for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
					{
						int taskid = client.ClientData.TaskDataList[i].DoingTaskID;
						if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskid, out systemTask))
						{
							if (ProcessTask.IsTaskValid(client, systemTask, client.ClientData.TaskDataList[i], nowTicks))
							{
								bool updateTask = false;
								for (int index = 1; index <= 2; index++)
								{
									int[] TargetNPCArray = systemTask.GetIntArrayValue("TargetNPC" + index, '|');
									if (TargetNPCArray != null && TargetNPCArray.Contains(targetId))
									{
										string targetType = string.Format("TargetType{0}", index);
										if (systemTask.GetIntValue(targetType, -1) == (int)taskType)
										{
											int targetNumVal = client.ClientData.TaskDataList[i].GetDoingTaskVal(index);
											string targetNum = string.Format("TargetNum{0}", index);
											if (targetNumVal < systemTask.GetIntValue(targetNum, -1))
											{
												updateTask = true;
												client.ClientData.TaskDataList[i].IncDoingTaskVal(index);
												ProcessTask.UpdateTaskDataToDB(client, client.ClientData.TaskDataList[i]);
											}
										}
									}
								}
								if (updateTask)
								{
									GameManager.ClientMgr.NotifyUpdateTask(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.TaskDataList[i].DbID, taskid, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
									if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
									{
										int destNPC = systemTask.GetIntValue("DestNPC", -1);
										if (-1 != destNPC)
										{
											int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, destNPC, 0);
											GameManager.ClientMgr.NotifyUpdateNPCTaskSate(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, destNPC + 2130706432, state);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		
		public static int GetRoleTaskVal(GameClient client, TaskTypes taskType)
		{
			if (taskType <= TaskTypes.FreindNum)
			{
				switch (taskType)
				{
				case TaskTypes.WingIDLevel:
					return client.ClientData.MyWingData.WingID * 10000 + client.ClientData.MyWingData.ForgeLevel;
				case TaskTypes.XingZuoStar:
					if (null == client.ClientData.RoleStarConstellationInfo)
					{
						return 0;
					}
					lock (client.ClientData.RoleStarConstellationInfo)
					{
						return client.ClientData.RoleStarConstellationInfo.Sum((KeyValuePair<int, int> x) => x.Value);
					}
					break;
				default:
					if (taskType != TaskTypes.FreindNum)
					{
						goto IL_190;
					}
					break;
				}
				return Global.GetFriendCountByType(client, 0);
			}
			switch (taskType)
			{
			case TaskTypes.WanMoTa:
				return GameManager.ClientMgr.GetWanMoTaPassLayerValue(client);
			case TaskTypes.QiFu_10:
				break;
			case TaskTypes.InZhanMeng:
				return (client.ClientData.Faction > 0) ? 1 : 0;
			default:
				if (taskType == TaskTypes.HuFuForgeLevel)
				{
					lock (client.ClientData.GoodsDataList)
					{
						return client.ClientData.GoodsDataList.Max((GoodsData x) => (Global.GetGoodsCatetoriy(x.GoodsID) == 22) ? Global.GetEquipGoodsSuitID(x.GoodsID) : 0);
					}
				}
				break;
			}
			IL_190:
			return -1;
		}

		
		public static void ProcessRoleTaskVal(GameClient client, TaskTypes taskType, int val = -1)
		{
			if (null != client.ClientData.TaskDataList)
			{
				long nowTicks = TimeUtil.NOW();
				SystemXmlItem systemTask = null;
				lock (client.ClientData.TaskDataList)
				{
					for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
					{
						int taskid = client.ClientData.TaskDataList[i].DoingTaskID;
						if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskid, out systemTask))
						{
							if (ProcessTask.IsTaskValid(client, systemTask, client.ClientData.TaskDataList[i], nowTicks))
							{
								bool updateTask = false;
								for (int index = 1; index <= 2; index++)
								{
									string targetType = string.Format("TargetType{0}", index);
									if (systemTask.GetIntValue(targetType, -1) == (int)taskType)
									{
										string targetNum = string.Format("TargetNum{0}", index);
										int targetNumVal;
										if (val < 0)
										{
											targetNumVal = ProcessTask.GetRoleTaskVal(client, taskType);
										}
										else
										{
											targetNumVal = val;
										}
										if (targetNumVal > client.ClientData.TaskDataList[i].GetDoingTaskVal(index))
										{
											updateTask = true;
											client.ClientData.TaskDataList[i].SetDoingTaskVal(index, targetNumVal);
											ProcessTask.UpdateTaskDataToDB(client, client.ClientData.TaskDataList[i]);
										}
									}
								}
								if (updateTask)
								{
									GameManager.ClientMgr.NotifyUpdateTask(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.TaskDataList[i].DbID, taskid, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
									if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
									{
										int destNPC = systemTask.GetIntValue("DestNPC", -1);
										if (-1 != destNPC)
										{
											int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, destNPC, 0);
											GameManager.ClientMgr.NotifyUpdateNPCTaskSate(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, destNPC + 2130706432, state);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		
		public static void InitRoleTaskVal(GameClient client, TaskData taskData, SystemXmlItem systemTask)
		{
			for (int index = 1; index <= 2; index++)
			{
				string targetType = string.Format("TargetType{0}", index);
				int targetNumVal = ProcessTask.GetRoleTaskVal(client, (TaskTypes)systemTask.GetIntValue(targetType, -1));
				if (targetNumVal < 0)
				{
					break;
				}
				if (targetNumVal > taskData.GetDoingTaskVal(index))
				{
					taskData.SetDoingTaskVal(index, targetNumVal);
				}
			}
		}

		
		public static void Process(SocketListener sl, TCPOutPacketPool pool, GameClient client, int npcID, int extensionID, int goodsID, TaskTypes taskType, Monster monster = null, int chengjiuID = 0, long chengjiuValue = -1L, GameClient otherClient = null)
		{
			switch (taskType)
			{
			case TaskTypes.Talk:
			case TaskTypes.GetSomething:
			case TaskTypes.NeedYuanBao:
				ProcessTask.ProcessTalk(sl, pool, client, npcID, extensionID, goodsID, taskType);
				break;
			case TaskTypes.KillMonster:
			case TaskTypes.MonsterSomething:
			case TaskTypes.CaiJiGoods:
			case TaskTypes.KillMonsterForLevel:
				ProcessTask.ProcessKillMonster(sl, pool, client, npcID, extensionID, goodsID, taskType, monster);
				break;
			case TaskTypes.BuySomething:
				ProcessTask.ProcessBuy(sl, pool, client, npcID, extensionID, goodsID, taskType);
				break;
			case TaskTypes.UseSomething:
				ProcessTask.ProcessUsingSomething(sl, pool, client, npcID, extensionID, goodsID, taskType);
				break;
			case TaskTypes.TransferSomething:
				ProcessTask.ProcessTransferSomething(sl, pool, client, npcID, extensionID, goodsID, taskType);
				break;
			case TaskTypes.ZhiLiao:
			case TaskTypes.FangHuo:
				ProcessTask.ProcessLuaHandle(sl, pool, client, npcID, extensionID, goodsID, taskType);
				break;
			case TaskTypes.GatherMonster:
				ProcessTask.ProcessGatherMonster(sl, pool, client, npcID, extensionID, goodsID, taskType);
				break;
			default:
				switch (taskType)
				{
				case TaskTypes.KillRoleOtherComp:
				case TaskTypes.KillRoleOtherCompTop:
					ProcessTask.ProcessKillRole(sl, pool, client, taskType, otherClient);
					break;
				default:
					if (taskType == TaskTypes.ChengJiuUpdate)
					{
						ProcessTask.ProcessChengJiuUpdate(sl, pool, client, chengjiuID, chengjiuValue);
					}
					break;
				}
				break;
			}
			ProcessTask.CheckAutoCompleteTask(client);
		}

		
		public static void CheckAutoCompleteTask(GameClient client)
		{
			if (client != null)
			{
				if (client.ClientData.TaskDataList != null)
				{
					List<TaskData> canCompTasks = null;
					lock (client.ClientData.TaskDataList)
					{
						canCompTasks = client.ClientData.TaskDataList.FindAll(delegate(TaskData _t)
						{
							bool result;
							if (!Global.JugeTaskComplete(client, _t.DoingTaskID, _t.DoingTaskVal1, _t.DoingTaskVal2))
							{
								result = false;
							}
							else
							{
								SystemXmlItem systemTask2 = null;
								result = (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(_t.DoingTaskID, out systemTask2) && systemTask2.GetIntValue("IsComplete", -1) == 1);
							}
							return result;
						});
					}
					if (canCompTasks != null && canCompTasks.Count > 0)
					{
						foreach (TaskData t in canCompTasks)
						{
							SystemXmlItem systemTask = null;
							if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(t.DoingTaskID, out systemTask))
							{
								ProcessTask._ProcessSpriteCompTaskCmd(client, systemTask.GetIntValue("DestNPC", -1), t.DoingTaskID, t.DbID, 0);
							}
						}
					}
				}
			}
		}

		
		public static void _ProcessSpriteCompTaskCmd(GameClient client, int npcID, int taskID, int dbID, int useYuanBao)
		{
			int nID = 140;
			SystemXmlItem systemTaskItem = null;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTaskItem))
			{
				client.sendCmd<SCCompTask>(nID, new SCCompTask(client.ClientData.RoleID, npcID, taskID, -200), false);
			}
			else
			{
				TaskData taskData = Global.GetTaskDataByDbID(client, dbID);
				if (taskData == null || taskData.DoingTaskID != taskID)
				{
					client.sendCmd<SCCompTask>(nID, new SCCompTask(client.ClientData.RoleID, npcID, taskID, -210), false);
				}
				else if (!Global.JugeTaskComplete(client, taskID, taskData.DoingTaskVal1, taskData.DoingTaskVal2))
				{
					client.sendCmd<SCCompTask>(nID, new SCCompTask(client.ClientData.RoleID, npcID, taskID, -220), false);
				}
				else if (!Global.CanCompleteTaskByGridNum(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, taskID))
				{
					client.sendCmd<SCCompTask>(nID, new SCCompTask(client.ClientData.RoleID, npcID, taskID, -2), false);
				}
				else
				{
					if (taskID == Data.InsertAwardtPortableBagTaskID)
					{
						if (client.ClientData.PortableGoodsDataList != null && client.ClientData.PortableGoodsDataList.Count >= client.ClientData.MyPortableBagData.ExtGridNum)
						{
							client.sendCmd<SCCompTask>(nID, new SCCompTask(client.ClientData.RoleID, npcID, taskID, -2), false);
							return;
						}
					}
					if (!Global.CanCompleteTaskByBlessPoint(client, systemTaskItem))
					{
						client.sendCmd<SCCompTask>(nID, new SCCompTask(client.ClientData.RoleID, npcID, taskID, -300), false);
					}
					else
					{
						int taskClass = systemTaskItem.GetIntValue("TaskClass", -1);
						useYuanBao = Math.Max(1, useYuanBao);
						useYuanBao = Math.Min(3, useYuanBao);
						string msg = "完成任务" + taskID;
						if (useYuanBao > 1)
						{
							int needBindTongQian = 0;
							int needZuanShi = 0;
							if (2 == useYuanBao)
							{
								needZuanShi = (int)GameManager.systemParamsList.GetParamValueIntByName("DoubleExp", -1);
							}
							else if (3 == useYuanBao)
							{
								needBindTongQian = (int)GameManager.systemParamsList.GetParamValueIntByName("BindTongQianTask3Awards", -1);
							}
							if (needZuanShi > 0)
							{
								if (!GameManager.ClientMgr.SubUserMoney(client, needZuanShi, "任务完成双倍经验", true, true, true, true, DaiBiSySType.None))
								{
									client.sendCmd<SCCompTask>(nID, new SCCompTask(client.ClientData.RoleID, npcID, taskID, -1004), false);
									return;
								}
							}
							else if (needBindTongQian > 0)
							{
								if (Global.GetTotalBindTongQianAndTongQianVal(client) < needBindTongQian)
								{
									client.sendCmd<SCCompTask>(nID, new SCCompTask(client.ClientData.RoleID, npcID, taskID, -1002), false);
									return;
								}
								if (!Global.SubBindTongQianAndTongQian(client, needBindTongQian, "任务完成多倍奖励"))
								{
									client.sendCmd<SCCompTask>(nID, new SCCompTask(client.ClientData.RoleID, npcID, taskID, -1003), false);
									return;
								}
							}
							else
							{
								useYuanBao = 1;
							}
						}
						int taskClassType = systemTaskItem.GetIntValue("TaskClass", -1);
						int isMainTask = (taskClassType == 0) ? 1 : 0;
						byte[] sendBytesCmd = new UTF8Encoding().GetBytes(string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							client.ClientData.RoleID,
							npcID,
							taskID,
							dbID,
							isMainTask
						}));
						byte[] bytesData = null;
						if (TCPProcessCmdResults.RESULT_FAILED == Global.TransferRequestToDBServer2(Global._TCPManager, client.ClientSocket, Global._TCPManager.tcpClientPool, Global._TCPManager.tcpRandKey, Global._TCPManager.TcpOutPacketPool, nID, sendBytesCmd, sendBytesCmd.Length, out bytesData, client.ServerId))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("与DBServer通讯失败, CMD={0}", (TCPGameServerCmds)nID), null, true);
							client.sendCmd<SCCompTask>(nID, new SCCompTask(client.ClientData.RoleID, npcID, taskID, -3), false);
						}
						else
						{
							int length = BitConverter.ToInt32(bytesData, 0);
							short cmd = BitConverter.ToInt16(bytesData, 4);
							string strData = new UTF8Encoding().GetString(bytesData, 6, length - 2);
							string[] fieldsData = strData.Split(new char[]
							{
								':'
							});
							if (fieldsData.Length < 3 || fieldsData[2] == "-1")
							{
								client.sendCmd<SCCompTask>(nID, new SCCompTask(client.ClientData.RoleID, npcID, taskID, -1), false);
							}
							else
							{
								int zhangJieID = client.ClientData.CompleteTaskZhangJie;
								int nextGoodsID = 0;
								int nextZhangJieID = Global.CalcTaskZhangJieID(client, taskID, out nextGoodsID);
								if (zhangJieID != nextZhangJieID)
								{
									long timestamp = TimeUtil.TimeStamp();
								}
								if (ProcessTask.Complete(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, npcID, -1, taskID, dbID, false, (double)useYuanBao, false))
								{
									if (taskClassType == 0 && taskID > client.ClientData.MainTaskID)
									{
										client.ClientData.MainTaskID = taskID;
										Global.AutoLearnSkills(client);
										GlobalNew.RefreshGongNeng(client);
										HuiJiManager.getInstance().InitDataByTask(client);
										ArmorManager.getInstance().InitDataByTask(client);
										GlobalEventSource.getInstance().fireEvent(new MainTaskProgressEvent(client, taskID));
										client.RunAreaLuaFile(GameManager.MapMgr.DictMaps[client.ClientData.MapCode], RunAreaLuaType.SelfPoint, null, "enterArea", taskID);
									}
									if (0 == taskClassType)
									{
									}
									if (isMainTask > 0)
									{
										ChengJiuManager.ProcessCompleteMainTaskForChengJiu(client, taskID);
									}
									if (taskClassType >= 100 && taskClassType <= 150)
									{
										CompManager.getInstance().HandleCompTaskSomething(client, false);
									}
									ProcessTask.ProcessTakeBranchTasks(client, BranchTaskTriggerType.CompleteTask, (long)taskID);
									client.sendCmd<SCCompTask>(nID, new SCCompTask(client.ClientData.RoleID, npcID, taskID, 0), false);
									if (systemTaskItem.GetIntValue("IsComplete", -1) == 1)
									{
										SystemXmlItem nextTaskXml = null;
										if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(systemTaskItem.GetIntValue("NextTask", -1), out nextTaskXml))
										{
											TCPOutPacket tcpOutPacketTemp = null;
											TCPProcessCmdResults result = Global.TakeNewTask(Global._TCPManager, client.ClientSocket, Global._TCPManager.tcpClientPool, Global._TCPManager.tcpRandKey, Global._TCPManager.TcpOutPacketPool, 125, client, client.ClientData.RoleID, nextTaskXml.GetIntValue("ID", -1), nextTaskXml.GetIntValue("SourceNPC", -1), out tcpOutPacketTemp);
											if (result == TCPProcessCmdResults.RESULT_DATA && tcpOutPacketTemp != null)
											{
												client.sendCmd(tcpOutPacketTemp, true);
											}
											if (TCPProcessCmdResults.RESULT_OK == result)
											{
												if (105 == taskID)
												{
													FreshPlayerCopySceneManager.AddShuiJingGuanCaiMonsters(client);
												}
											}
											client.RunAreaLuaFile(GameManager.MapMgr.DictMaps[client.ClientData.MapCode], RunAreaLuaType.SelfPoint, null, "enterArea", 0);
										}
									}
								}
								else
								{
									client.sendCmd<SCCompTask>(nID, new SCCompTask(client.ClientData.RoleID, npcID, taskID, -1), false);
								}
							}
						}
					}
				}
			}
		}

		
		private static string GetPropNameGoodsName(string propName)
		{
			string result;
			if (string.IsNullOrEmpty(propName))
			{
				result = propName;
			}
			else
			{
				string[] fields = propName.Split(new char[]
				{
					'|'
				});
				if (fields.Length <= 1)
				{
					result = propName;
				}
				else
				{
					result = fields[0];
				}
			}
			return result;
		}

		
		private static int GetPropNameGoodsLevel(string propName)
		{
			int result;
			if (string.IsNullOrEmpty(propName))
			{
				result = 0;
			}
			else
			{
				string[] fields = propName.Split(new char[]
				{
					'|'
				});
				if (fields.Length < 3)
				{
					result = 0;
				}
				else
				{
					result = Global.SafeConvertToInt32(fields[1]);
				}
			}
			return result;
		}

		
		private static int GetPropNameGoodsQuality(string propName)
		{
			int result;
			if (string.IsNullOrEmpty(propName))
			{
				result = 0;
			}
			else
			{
				string[] fields = propName.Split(new char[]
				{
					'|'
				});
				if (fields.Length < 3)
				{
					result = 0;
				}
				else
				{
					result = (int)Global.GetEnchanceQualityByColorName(fields[2]);
				}
			}
			return result;
		}

		
		private static bool IsTaskValid(GameClient client, SystemXmlItem systemTask, TaskData taskData, long nowTicks)
		{
			int taskMaxOverTime = systemTask.GetIntValue("Taketime", -1);
			bool result;
			if (taskMaxOverTime > 0 && nowTicks - taskData.AddDateTime >= (long)(taskMaxOverTime * 1000))
			{
				result = false;
			}
			else
			{
				string pubStartTime = systemTask.GetStringValue("PubStartTime");
				string pubEndTime = systemTask.GetStringValue("PubEndTime");
				if (!string.IsNullOrEmpty(pubStartTime) && !string.IsNullOrEmpty(pubEndTime))
				{
					long startTime = Global.SafeConvertToTicks(pubStartTime);
					long endTime = Global.SafeConvertToTicks(pubEndTime);
					if (nowTicks < startTime || nowTicks > endTime)
					{
						return false;
					}
				}
				int limitZhuanSheng = systemTask.GetIntValue("LimitZhuanSheng", -1);
				int limitLevel = systemTask.GetIntValue("LimitLevel", -1);
				result = (0 == Global.AvalidLevel(client.ClientData.ChangeLifeCount, client.ClientData.Level, limitZhuanSheng, limitLevel, -1, -1));
			}
			return result;
		}

		
		private static void ProcessTalk(SocketListener sl, TCPOutPacketPool pool, GameClient client, int npcID, int extensionID, int goodsID, TaskTypes taskType)
		{
			if (null != client.ClientData.TaskDataList)
			{
				long nowTicks = TimeUtil.NOW();
				SystemXmlItem systemTask = null;
				lock (client.ClientData.TaskDataList)
				{
					for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
					{
						int taskid = client.ClientData.TaskDataList[i].DoingTaskID;
						if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskid, out systemTask))
						{
							if (ProcessTask.IsTaskValid(client, systemTask, client.ClientData.TaskDataList[i], nowTicks))
							{
								int[] TargetNPCArray = systemTask.GetIntArrayValue("TargetNPC1", '|');
								if (TargetNPCArray != null && TargetNPCArray.Contains(extensionID))
								{
									bool updateTask = false;
									if (systemTask.GetIntValue("TargetType1", -1) == 0)
									{
										bool toAddVal = false;
										int ingMaiID = systemTask.GetIntValue("JingMaiID", -1);
										int buffID = systemTask.GetIntValue("BuffID", -1);
										int wuXueID = systemTask.GetIntValue("WuXueID", -1);
										if (ingMaiID <= 0)
										{
											toAddVal = true;
										}
										if (wuXueID <= 0)
										{
											toAddVal = toAddVal;
										}
										if (buffID > 0)
										{
											BufferData bufferData = Global.GetBufferDataByID(client, buffID);
											if (null != bufferData)
											{
												if (!Global.IsBufferDataOver(bufferData, nowTicks))
												{
													toAddVal = toAddVal;
												}
											}
										}
										else
										{
											toAddVal = toAddVal;
										}
										if (toAddVal)
										{
											if (client.ClientData.TaskDataList[i].DoingTaskVal1 < systemTask.GetIntValue("TargetNum1", -1))
											{
												client.ClientData.TaskDataList[i].DoingTaskVal1++;
												GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
												{
													client.ClientData.RoleID,
													client.ClientData.TaskDataList[i].DoingTaskID,
													client.ClientData.TaskDataList[i].DbID,
													client.ClientData.TaskDataList[i].DoingTaskFocus,
													client.ClientData.TaskDataList[i].DoingTaskVal1,
													client.ClientData.TaskDataList[i].DoingTaskVal2
												}), null, client.ServerId);
												updateTask = true;
											}
										}
									}
									else if (systemTask.GetIntValue("TargetType1", -1) == 6 && "" != systemTask.GetStringValue("PropsName1"))
									{
										if (client.ClientData.TaskDataList[i].DoingTaskVal1 < systemTask.GetIntValue("TargetNum1", -1))
										{
											bool toUpdateTask = true;
											string propsName = systemTask.GetStringValue("PropsName1");
											string goodsName = ProcessTask.GetPropNameGoodsName(propsName);
											int goodsLevel = ProcessTask.GetPropNameGoodsLevel(propsName);
											int goodsQuality = ProcessTask.GetPropNameGoodsQuality(propsName);
											int transferGoodsID = Global.GetGoodsByName(goodsName);
											if (transferGoodsID >= 0)
											{
												GoodsData goodsData = Global.GetNotUsingGoodsByID(client, transferGoodsID, goodsLevel, goodsQuality);
												if (null == goodsData)
												{
													if (Global.CanAddGoods(client, transferGoodsID, 1, 1, "1900-01-01 12:00:00", true, false))
													{
														Global.AddGoodsDBCommand(pool, client, transferGoodsID, 1, 0, "", 0, 1, 0, "", true, 1, "获取任务道具", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
													}
													else
													{
														toUpdateTask = false;
														GameManager.ClientMgr.NotifyImportantMsg(sl, pool, client, GLang.GetLang(516, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 1);
													}
												}
											}
											if (toUpdateTask)
											{
												client.ClientData.TaskDataList[i].DoingTaskVal1++;
												GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
												{
													client.ClientData.RoleID,
													client.ClientData.TaskDataList[i].DoingTaskID,
													client.ClientData.TaskDataList[i].DbID,
													client.ClientData.TaskDataList[i].DoingTaskFocus,
													client.ClientData.TaskDataList[i].DoingTaskVal1,
													client.ClientData.TaskDataList[i].DoingTaskVal2
												}), null, client.ServerId);
												updateTask = true;
											}
										}
									}
									else if (systemTask.GetIntValue("TargetType1", -1) == 7)
									{
										if (client.ClientData.TaskDataList[i].DoingTaskVal1 < systemTask.GetIntValue("TargetNum1", -1))
										{
											int totalChongZhiMoney = GameManager.ClientMgr.QueryTotaoChongZhiMoney(client);
											if (totalChongZhiMoney > 0)
											{
												client.ClientData.TaskDataList[i].DoingTaskVal1++;
												GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
												{
													client.ClientData.RoleID,
													client.ClientData.TaskDataList[i].DoingTaskID,
													client.ClientData.TaskDataList[i].DbID,
													client.ClientData.TaskDataList[i].DoingTaskFocus,
													client.ClientData.TaskDataList[i].DoingTaskVal1,
													client.ClientData.TaskDataList[i].DoingTaskVal2
												}), null, client.ServerId);
												updateTask = true;
											}
										}
									}
									if (updateTask)
									{
										GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, taskid, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
										if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
										{
											int destNPC = systemTask.GetIntValue("DestNPC", -1);
											if (-1 != destNPC)
											{
												int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, destNPC, 0);
												GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, destNPC + 2130706432, state);
											}
										}
									}
								}
								if (extensionID == systemTask.GetIntValue("TargetNPC2", -1))
								{
									bool updateTask = false;
									if (systemTask.GetIntValue("TargetType2", -1) == 0)
									{
										bool toAddVal = false;
										int ingMaiID = systemTask.GetIntValue("JingMaiID", -1);
										int buffID = systemTask.GetIntValue("BuffID", -1);
										int wuXueID = systemTask.GetIntValue("WuXueID", -1);
										if (ingMaiID <= 0)
										{
											toAddVal = true;
										}
										if (wuXueID <= 0)
										{
											toAddVal = toAddVal;
										}
										if (buffID > 0)
										{
											BufferData bufferData = Global.GetBufferDataByID(client, buffID);
											if (null != bufferData)
											{
												if (!Global.IsBufferDataOver(bufferData, nowTicks))
												{
													toAddVal = toAddVal;
												}
											}
										}
										else
										{
											toAddVal = toAddVal;
										}
										if (toAddVal)
										{
											if (client.ClientData.TaskDataList[i].DoingTaskVal2 < systemTask.GetIntValue("TargetNum2", -1))
											{
												client.ClientData.TaskDataList[i].DoingTaskVal2++;
												GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
												{
													client.ClientData.RoleID,
													client.ClientData.TaskDataList[i].DoingTaskID,
													client.ClientData.TaskDataList[i].DbID,
													client.ClientData.TaskDataList[i].DoingTaskFocus,
													client.ClientData.TaskDataList[i].DoingTaskVal1,
													client.ClientData.TaskDataList[i].DoingTaskVal2
												}), null, client.ServerId);
												updateTask = true;
											}
										}
									}
									else if (systemTask.GetIntValue("TargetType2", -1) == 6 && "" != systemTask.GetStringValue("PropsName2"))
									{
										if (client.ClientData.TaskDataList[i].DoingTaskVal2 < systemTask.GetIntValue("TargetNum2", -1))
										{
											bool toUpdateTask = true;
											string propsName = systemTask.GetStringValue("PropsName2");
											string goodsName = ProcessTask.GetPropNameGoodsName(propsName);
											int goodsLevel = ProcessTask.GetPropNameGoodsLevel(propsName);
											int goodsQuality = ProcessTask.GetPropNameGoodsQuality(propsName);
											int transferGoodsID = Global.GetGoodsByName(goodsName);
											if (transferGoodsID >= 0)
											{
												GoodsData goodsData = Global.GetNotUsingGoodsByID(client, transferGoodsID, goodsLevel, goodsQuality);
												if (null == goodsData)
												{
													if (Global.CanAddGoods(client, transferGoodsID, 1, 1, "1900-01-01 12:00:00", true, false))
													{
														Global.AddGoodsDBCommand(pool, client, transferGoodsID, 1, 0, "", 0, 1, 0, "", true, 1, "获取任务道具", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
													}
													else
													{
														toUpdateTask = false;
														GameManager.ClientMgr.NotifyImportantMsg(sl, pool, client, GLang.GetLang(516, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 1);
													}
												}
											}
											if (toUpdateTask)
											{
												client.ClientData.TaskDataList[i].DoingTaskVal2++;
												GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
												{
													client.ClientData.RoleID,
													client.ClientData.TaskDataList[i].DoingTaskID,
													client.ClientData.TaskDataList[i].DbID,
													client.ClientData.TaskDataList[i].DoingTaskFocus,
													client.ClientData.TaskDataList[i].DoingTaskVal1,
													client.ClientData.TaskDataList[i].DoingTaskVal2
												}), null, client.ServerId);
												updateTask = true;
											}
										}
									}
									else if (systemTask.GetIntValue("TargetType2", -1) == 7)
									{
										if (client.ClientData.TaskDataList[i].DoingTaskVal2 < systemTask.GetIntValue("TargetNum2", -1))
										{
											int totalChongZhiMoney = GameManager.ClientMgr.QueryTotaoChongZhiMoney(client);
											if (totalChongZhiMoney > 0)
											{
												client.ClientData.TaskDataList[i].DoingTaskVal2++;
												GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
												{
													client.ClientData.RoleID,
													client.ClientData.TaskDataList[i].DoingTaskID,
													client.ClientData.TaskDataList[i].DbID,
													client.ClientData.TaskDataList[i].DoingTaskFocus,
													client.ClientData.TaskDataList[i].DoingTaskVal1,
													client.ClientData.TaskDataList[i].DoingTaskVal2
												}), null, client.ServerId);
												updateTask = true;
											}
										}
									}
									if (updateTask)
									{
										GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, taskid, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
										if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
										{
											int destNPC = systemTask.GetIntValue("DestNPC", -1);
											if (-1 != destNPC)
											{
												int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, destNPC, 0);
												GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, destNPC + 2130706432, state);
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

		
		private static void ProcessTransferSomething(SocketListener sl, TCPOutPacketPool pool, GameClient client, int npcID, int extensionID, int goodsID, TaskTypes taskType)
		{
			if (null != client.ClientData.TaskDataList)
			{
				long nowTicks = TimeUtil.NOW();
				SystemXmlItem systemTask = null;
				lock (client.ClientData.TaskDataList)
				{
					for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
					{
						int taskid = client.ClientData.TaskDataList[i].DoingTaskID;
						if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskid, out systemTask))
						{
							if (ProcessTask.IsTaskValid(client, systemTask, client.ClientData.TaskDataList[i], nowTicks))
							{
								int[] TargetNPCArray = systemTask.GetIntArrayValue("TargetNPC1", '|');
								if (TargetNPCArray != null && TargetNPCArray.Contains(extensionID))
								{
									bool updateTask = false;
									if (systemTask.GetIntValue("TargetType1", -1) == 5 && "" != systemTask.GetStringValue("PropsName1"))
									{
										bool toUpdateTask = true;
										string propsName = systemTask.GetStringValue("PropsName1");
										string goodsName = ProcessTask.GetPropNameGoodsName(propsName);
										int goodsLevel = ProcessTask.GetPropNameGoodsLevel(propsName);
										int goodsQuality = ProcessTask.GetPropNameGoodsQuality(propsName);
										int transferGoodsID = Global.GetGoodsByName(goodsName);
										if (transferGoodsID >= 0)
										{
											GoodsData goodsData = Global.GetNotUsingGoodsByID(client, transferGoodsID, goodsLevel, goodsQuality);
											if (null != goodsData)
											{
												int catetoriy = Global.GetGoodsCatetoriy(transferGoodsID);
												if (catetoriy >= 49)
												{
													GameManager.ClientMgr.NotifyUseGoods(sl, Global._TCPManager.tcpClientPool, pool, client, goodsData.Id, false, false);
												}
											}
											else
											{
												toUpdateTask = false;
											}
										}
										if (toUpdateTask)
										{
											if (client.ClientData.TaskDataList[i].DoingTaskVal1 < systemTask.GetIntValue("TargetNum1", -1))
											{
												client.ClientData.TaskDataList[i].DoingTaskVal1++;
												GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
												{
													client.ClientData.RoleID,
													client.ClientData.TaskDataList[i].DoingTaskID,
													client.ClientData.TaskDataList[i].DbID,
													client.ClientData.TaskDataList[i].DoingTaskFocus,
													client.ClientData.TaskDataList[i].DoingTaskVal1,
													client.ClientData.TaskDataList[i].DoingTaskVal2
												}), null, client.ServerId);
												updateTask = true;
											}
										}
									}
									if (updateTask)
									{
										GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, taskid, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
										if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
										{
											int destNPC = systemTask.GetIntValue("DestNPC", -1);
											if (-1 != destNPC)
											{
												int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, destNPC, 0);
												GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, destNPC + 2130706432, state);
											}
										}
									}
								}
								if (extensionID == systemTask.GetIntValue("TargetNPC2", -1))
								{
									bool updateTask = false;
									if (systemTask.GetIntValue("TargetType2", -1) == 5 && "" != systemTask.GetStringValue("PropsName2"))
									{
										bool toUpdateTask = true;
										string propsName = systemTask.GetStringValue("PropsName2");
										string goodsName = ProcessTask.GetPropNameGoodsName(propsName);
										int goodsLevel = ProcessTask.GetPropNameGoodsLevel(propsName);
										int goodsQuality = ProcessTask.GetPropNameGoodsQuality(propsName);
										int transferGoodsID = Global.GetGoodsByName(goodsName);
										if (transferGoodsID >= 0)
										{
											GoodsData goodsData = Global.GetNotUsingGoodsByID(client, transferGoodsID, goodsLevel, goodsQuality);
											if (null != goodsData)
											{
												int catetoriy = Global.GetGoodsCatetoriy(transferGoodsID);
												if (catetoriy >= 49)
												{
													GameManager.ClientMgr.NotifyUseGoods(sl, Global._TCPManager.tcpClientPool, pool, client, goodsData.Id, false, false);
												}
											}
											else
											{
												toUpdateTask = false;
											}
										}
										if (toUpdateTask)
										{
											if (client.ClientData.TaskDataList[i].DoingTaskVal2 < systemTask.GetIntValue("TargetNum2", -1))
											{
												client.ClientData.TaskDataList[i].DoingTaskVal2++;
												GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
												{
													client.ClientData.RoleID,
													client.ClientData.TaskDataList[i].DoingTaskID,
													client.ClientData.TaskDataList[i].DbID,
													client.ClientData.TaskDataList[i].DoingTaskFocus,
													client.ClientData.TaskDataList[i].DoingTaskVal1,
													client.ClientData.TaskDataList[i].DoingTaskVal2
												}), null, client.ServerId);
												updateTask = true;
											}
										}
									}
									if (updateTask)
									{
										GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, taskid, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
										if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
										{
											int destNPC = systemTask.GetIntValue("DestNPC", -1);
											if (-1 != destNPC)
											{
												int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, destNPC, 0);
												GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, destNPC + 2130706432, state);
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

		
		private static void ProcessGatherMonster(SocketListener sl, TCPOutPacketPool pool, GameClient client, int npcID, int extensionID, int goodsID, TaskTypes taskType)
		{
			if (null != client.ClientData.TaskDataList)
			{
				long nowTicks = TimeUtil.NOW();
				SystemXmlItem systemTask = null;
				lock (client.ClientData.TaskDataList)
				{
					for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
					{
						int taskid = client.ClientData.TaskDataList[i].DoingTaskID;
						if (taskid == goodsID)
						{
							if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskid, out systemTask))
							{
								if (ProcessTask.IsTaskValid(client, systemTask, client.ClientData.TaskDataList[i], nowTicks))
								{
									client.ClientData.TaskDataList[i].DoingTaskVal1++;
									GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
									{
										client.ClientData.RoleID,
										client.ClientData.TaskDataList[i].DoingTaskID,
										client.ClientData.TaskDataList[i].DbID,
										client.ClientData.TaskDataList[i].DoingTaskFocus,
										client.ClientData.TaskDataList[i].DoingTaskVal1,
										client.ClientData.TaskDataList[i].DoingTaskVal2
									}), null, client.ServerId);
									GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, taskid, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
									if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
									{
										int destNPC = systemTask.GetIntValue("DestNPC", -1);
										if (-1 != destNPC)
										{
											int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, destNPC, 0);
											GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, destNPC + 2130706432, state);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		
		public static bool TaskOKForSystemKillMoster(GameClient client, int extentionID, int taskid)
		{
			long nowTicks = TimeUtil.NOW();
			SystemXmlItem systemTask = null;
			lock (client.ClientData.TaskDataList)
			{
				int i = 0;
				while (i < client.ClientData.TaskDataList.Count)
				{
					int mytaskid = client.ClientData.TaskDataList[i].DoingTaskID;
					if (mytaskid == taskid)
					{
						if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(mytaskid, out systemTask))
						{
							return false;
						}
						if (!ProcessTask.IsTaskValid(client, systemTask, client.ClientData.TaskDataList[i], nowTicks))
						{
							return false;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
			bool result;
			if (systemTask == null)
			{
				result = false;
			}
			else
			{
				int[] TargetNPCArray = systemTask.GetIntArrayValue("TargetNPC1", '|');
				if (TargetNPCArray != null && TargetNPCArray.Contains(extentionID))
				{
					if (systemTask.GetIntValue("TargetType1", -1) == 1)
					{
						return true;
					}
				}
				if (extentionID == systemTask.GetIntValue("TargetNPC2", -1))
				{
					if (systemTask.GetIntValue("TargetType2", -1) == 1)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		
		private static void ProcessKillRole(SocketListener sl, TCPOutPacketPool pool, GameClient client, TaskTypes taskType, GameClient otherClient)
		{
			if (null != otherClient)
			{
				if (null != client.ClientData.TaskDataList)
				{
					long nowTicks = TimeUtil.NOW();
					SystemXmlItem systemTask = null;
					lock (client.ClientData.TaskDataList)
					{
						for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
						{
							int taskid = client.ClientData.TaskDataList[i].DoingTaskID;
							if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskid, out systemTask))
							{
								if (ProcessTask.IsTaskValid(client, systemTask, client.ClientData.TaskDataList[i], nowTicks))
								{
									for (int index = 1; index <= 2; index++)
									{
										string targetType = string.Format("TargetType{0}", index);
										string targetNum = string.Format("TargetNum{0}", index);
										bool updateTask = false;
										if (systemTask.GetIntValue(targetType, -1) == (int)taskType)
										{
											if (client.ClientData.CompType > 0 && otherClient.ClientData.CompType > 0)
											{
												SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
												if (sceneType == SceneUIClasses.Comp)
												{
													if (systemTask.GetIntValue(targetType, -1) == 101)
													{
														if (client.ClientData.CompType == otherClient.ClientData.CompType)
														{
															goto IL_42D;
														}
													}
													if (systemTask.GetIntValue(targetType, -1) == 102)
													{
														if (!CompManager.getInstance().IfTopBoomCompType(client, otherClient.ClientData.CompType, false))
														{
															goto IL_42D;
														}
													}
													if (client.ClientData.TaskDataList[i].DoingTaskVal1 < systemTask.GetIntValue(targetNum, -1))
													{
														client.ClientData.TaskDataList[i].DoingTaskVal1++;
														GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
														{
															client.ClientData.RoleID,
															client.ClientData.TaskDataList[i].DoingTaskID,
															client.ClientData.TaskDataList[i].DbID,
															client.ClientData.TaskDataList[i].DoingTaskFocus,
															client.ClientData.TaskDataList[i].DoingTaskVal1,
															client.ClientData.TaskDataList[i].DoingTaskVal2
														}), null, client.ServerId);
														updateTask = true;
													}
													if (updateTask)
													{
														GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, taskid, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
														if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
														{
															int destNPC = systemTask.GetIntValue("DestNPC", -1);
															if (-1 != destNPC)
															{
																int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, destNPC, 0);
																GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, destNPC + 2130706432, state);
															}
														}
													}
												}
											}
										}
										IL_42D:;
									}
								}
							}
						}
					}
				}
			}
		}

		
		private static void ProcessChengJiuUpdate(SocketListener sl, TCPOutPacketPool pool, GameClient client, int chengjiuID, long roleCurrentValue)
		{
			if (chengjiuID > 0)
			{
				if (null != client.ClientData.TaskDataList)
				{
					long nowTicks = TimeUtil.NOW();
					SystemXmlItem systemTask = null;
					lock (client.ClientData.TaskDataList)
					{
						for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
						{
							int taskid = client.ClientData.TaskDataList[i].DoingTaskID;
							if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskid, out systemTask))
							{
								if (ProcessTask.IsTaskValid(client, systemTask, client.ClientData.TaskDataList[i], nowTicks))
								{
									if (systemTask.GetIntValue("ChenJiuID", -1) == chengjiuID)
									{
										if (client.ClientData.TaskDataList[i].ChengJiuVal >= 0L)
										{
											if (ChengJiuManager.IsChengJiuCompleted(client, chengjiuID))
											{
												client.ClientData.TaskDataList[i].ChengJiuVal = -1L;
											}
											else
											{
												client.ClientData.TaskDataList[i].ChengJiuVal = roleCurrentValue;
											}
											GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, taskid, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
											if (client.ClientData.TaskDataList[i].ChengJiuVal < 0L)
											{
												if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
												{
													int destNPC = systemTask.GetIntValue("DestNPC", -1);
													if (-1 != destNPC)
													{
														int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, destNPC, 0);
														GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, destNPC + 2130706432, state);
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

		
		private static void ProcessKillMonster(SocketListener sl, TCPOutPacketPool pool, GameClient client, int npcID, int extensionID, int goodsID, TaskTypes taskType, Monster monster = null)
		{
			if (null != client.ClientData.TaskDataList)
			{
				long nowTicks = TimeUtil.NOW();
				int focusCount = Global.GetFocusTaskCount(client);
				bool updateTask = false;
				SystemXmlItem systemTask = null;
				int monsterLevel = -1;
				SystemXmlItem monsterXml = null;
				if (GameManager.systemMonsterMgr.SystemXmlItemDict.TryGetValue(extensionID, out monsterXml))
				{
					monsterLevel = monsterXml.GetIntValue("Level", -1);
				}
				lock (client.ClientData.TaskDataList)
				{
					for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
					{
						int taskid = client.ClientData.TaskDataList[i].DoingTaskID;
						if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskid, out systemTask))
						{
							if (ProcessTask.IsTaskValid(client, systemTask, client.ClientData.TaskDataList[i], nowTicks))
							{
								if (11 == systemTask.GetIntValue("TargetType1", -1))
								{
									if (monsterLevel >= systemTask.GetIntValue("TargetNPC1", -1) && client.ClientData.TaskDataList[i].DoingTaskVal1 < systemTask.GetIntValue("TargetNum1", -1))
									{
										client.ClientData.TaskDataList[i].DoingTaskVal1++;
										if (focusCount < Data.TaskMaxFocusCount && client.ClientData.TaskDataList[i].DoingTaskFocus <= 0)
										{
											focusCount++;
											client.ClientData.TaskDataList[i].DoingTaskFocus = 1;
										}
										GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
										{
											client.ClientData.RoleID,
											client.ClientData.TaskDataList[i].DoingTaskID,
											client.ClientData.TaskDataList[i].DbID,
											client.ClientData.TaskDataList[i].DoingTaskFocus,
											client.ClientData.TaskDataList[i].DoingTaskVal1,
											client.ClientData.TaskDataList[i].DoingTaskVal2
										}), null, client.ServerId);
										updateTask = true;
									}
									if (updateTask)
									{
										GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, taskid, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
										if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
										{
											int destNPC = systemTask.GetIntValue("DestNPC", -1);
											if (-1 != destNPC)
											{
												int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, destNPC, 0);
												GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, destNPC + 2130706432, state);
											}
										}
									}
								}
								else
								{
									for (int index = 1; index <= 2; index++)
									{
										int[] TargetNPCArray = systemTask.GetIntArrayValue(string.Format("TargetNPC{0}", index), '|');
										if (TargetNPCArray != null && TargetNPCArray.Contains(extensionID))
										{
											string targetType = string.Format("TargetType{0}", index);
											string targetNum = string.Format("TargetNum{0}", index);
											updateTask = false;
											if (systemTask.GetIntValue(targetType, -1) == 1)
											{
												string fallPercent = string.Format("FallPercent{0}", index);
												int randRange = systemTask.GetIntValue(fallPercent, -1);
												int randNum = -1;
												if (randRange > 0)
												{
													randNum = Global.GetRandomNumber(0, 101);
												}
												if (randNum < randRange)
												{
													if (client.ClientData.TaskDataList[i].GetDoingTaskVal(index) < systemTask.GetIntValue(targetNum, -1))
													{
														client.ClientData.TaskDataList[i].IncDoingTaskVal(index);
														if (focusCount < Data.TaskMaxFocusCount && client.ClientData.TaskDataList[i].DoingTaskFocus <= 0)
														{
															focusCount++;
															client.ClientData.TaskDataList[i].DoingTaskFocus = 1;
														}
														if (systemTask.GetIntValue("NeedTargetNum", -1) > 0 && !string.IsNullOrEmpty(systemTask.GetStringValue("FallGoods")))
														{
															if (client.ClientData.TaskDataList[i].GetDoingTaskVal(index) == systemTask.GetIntValue("NeedTargetNum", -1))
															{
																GameManager.GoodsPackMgr.ProcessTaskDropByTargetNum(client, systemTask.GetStringValue("FallGoods"), monster);
															}
														}
														GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
														{
															client.ClientData.RoleID,
															client.ClientData.TaskDataList[i].DoingTaskID,
															client.ClientData.TaskDataList[i].DbID,
															client.ClientData.TaskDataList[i].DoingTaskFocus,
															client.ClientData.TaskDataList[i].DoingTaskVal1,
															client.ClientData.TaskDataList[i].DoingTaskVal2
														}), null, client.ServerId);
														updateTask = true;
													}
												}
											}
											else if (systemTask.GetIntValue(targetType, -1) == 2)
											{
												int randNum = Global.GetRandomNumber(0, 101);
												string fallPercent = string.Format("FallPercent{0}", index);
												int randRange = systemTask.GetIntValue(fallPercent, -1);
												if (randNum < randRange)
												{
													if (client.ClientData.TaskDataList[i].GetDoingTaskVal(index) < systemTask.GetIntValue(targetNum, -1))
													{
														bool toUpdateTask = true;
														string goodsName = systemTask.GetStringValue("PropsName1");
														int transferGoodsID = Global.GetGoodsByName(goodsName);
														if (transferGoodsID >= 0)
														{
															if (Global.CanAddGoods(client, transferGoodsID, 1, 1, "1900-01-01 12:00:00", true, false))
															{
																Global.AddGoodsDBCommand(pool, client, transferGoodsID, 1, 0, "", 0, 1, 0, "", true, 1, "获取杀怪掉落道具", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
															}
															else
															{
																toUpdateTask = false;
																GameManager.ClientMgr.NotifyImportantMsg(sl, pool, client, StringUtil.substitute(GLang.GetLang(517, new object[0]), new object[]
																{
																	goodsName
																}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 1);
															}
														}
														if (toUpdateTask)
														{
															client.ClientData.TaskDataList[i].IncDoingTaskVal(index);
															if (focusCount < Data.TaskMaxFocusCount && client.ClientData.TaskDataList[i].DoingTaskFocus <= 0)
															{
																focusCount++;
																client.ClientData.TaskDataList[i].DoingTaskFocus = 1;
															}
															GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
															{
																client.ClientData.RoleID,
																client.ClientData.TaskDataList[i].DoingTaskID,
																client.ClientData.TaskDataList[i].DbID,
																client.ClientData.TaskDataList[i].DoingTaskFocus,
																client.ClientData.TaskDataList[i].DoingTaskVal1,
																client.ClientData.TaskDataList[i].DoingTaskVal2
															}), null, client.ServerId);
															updateTask = true;
														}
													}
												}
											}
											else if (systemTask.GetIntValue(targetType, -1) == 8)
											{
												int randNum = Global.GetRandomNumber(0, 101);
												string fallPercent = string.Format("FallPercent{0}", index);
												int randRange = systemTask.GetIntValue(fallPercent, -1);
												if (randNum < randRange)
												{
													if (client.ClientData.TaskDataList[i].GetDoingTaskVal(index) < systemTask.GetIntValue(targetNum, -1))
													{
														bool toUpdateTask = true;
														string goodsName = systemTask.GetStringValue("PropsName1");
														int transferGoodsID = Global.GetGoodsByName(goodsName);
														if (transferGoodsID >= 0)
														{
															if (Global.CanAddGoods(client, transferGoodsID, 1, 1, "1900-01-01 12:00:00", true, false))
															{
																Global.AddGoodsDBCommand(pool, client, transferGoodsID, 1, 0, "", 0, 1, 0, "", true, 1, "采集获取道具", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
															}
															else
															{
																toUpdateTask = false;
																GameManager.ClientMgr.NotifyImportantMsg(sl, pool, client, StringUtil.substitute(GLang.GetLang(518, new object[0]), new object[]
																{
																	goodsName
																}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 1);
															}
														}
														if (toUpdateTask)
														{
															client.ClientData.TaskDataList[i].IncDoingTaskVal(index);
															if (focusCount < Data.TaskMaxFocusCount && client.ClientData.TaskDataList[i].DoingTaskFocus <= 0)
															{
																focusCount++;
																client.ClientData.TaskDataList[i].DoingTaskFocus = 1;
															}
															GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
															{
																client.ClientData.RoleID,
																client.ClientData.TaskDataList[i].DoingTaskID,
																client.ClientData.TaskDataList[i].DbID,
																client.ClientData.TaskDataList[i].DoingTaskFocus,
																client.ClientData.TaskDataList[i].DoingTaskVal1,
																client.ClientData.TaskDataList[i].DoingTaskVal2
															}), null, client.ServerId);
															updateTask = true;
														}
													}
												}
											}
											if (updateTask)
											{
												GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, taskid, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
												if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
												{
													int destNPC = systemTask.GetIntValue("DestNPC", -1);
													if (-1 != destNPC)
													{
														int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, destNPC, 0);
														GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, destNPC + 2130706432, state);
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

		
		private static void ProcessBuy(SocketListener sl, TCPOutPacketPool pool, GameClient client, int npcID, int extensionID, int goodsID, TaskTypes taskType)
		{
			if (null != client.ClientData.TaskDataList)
			{
				if (-1 != goodsID)
				{
					long nowTicks = TimeUtil.NOW();
					SystemXmlItem systemTask = null;
					lock (client.ClientData.TaskDataList)
					{
						for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
						{
							int taskid = client.ClientData.TaskDataList[i].DoingTaskID;
							if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskid, out systemTask))
							{
								if (ProcessTask.IsTaskValid(client, systemTask, client.ClientData.TaskDataList[i], nowTicks))
								{
									if (systemTask.GetIntValue("TargetType1", -1) == 3)
									{
										bool updateTask = false;
										string propsName = systemTask.GetStringValue("PropsName1");
										string goodsName = ProcessTask.GetPropNameGoodsName(propsName);
										int goodsLevel = ProcessTask.GetPropNameGoodsLevel(propsName);
										int goodsQuality = ProcessTask.GetPropNameGoodsQuality(propsName);
										int transferGoodsID = Global.GetGoodsByName(goodsName);
										if (goodsID == transferGoodsID)
										{
											GoodsData goodsData = Global.GetNotUsingGoodsByID(client, transferGoodsID, goodsLevel, goodsQuality);
											if (null != goodsData)
											{
												if (client.ClientData.TaskDataList[i].DoingTaskVal1 < systemTask.GetIntValue("TargetNum1", -1))
												{
													client.ClientData.TaskDataList[i].DoingTaskVal1++;
													GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
													{
														client.ClientData.RoleID,
														client.ClientData.TaskDataList[i].DoingTaskID,
														client.ClientData.TaskDataList[i].DbID,
														client.ClientData.TaskDataList[i].DoingTaskFocus,
														client.ClientData.TaskDataList[i].DoingTaskVal1,
														client.ClientData.TaskDataList[i].DoingTaskVal2
													}), null, client.ServerId);
													updateTask = true;
												}
											}
										}
										if (updateTask)
										{
											GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, taskid, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
											if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
											{
												int destNPC = systemTask.GetIntValue("DestNPC", -1);
												if (-1 != destNPC)
												{
													int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, destNPC, 0);
													GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, destNPC + 2130706432, state);
												}
											}
										}
									}
									if (systemTask.GetIntValue("TargetType2", -1) == 3)
									{
										bool updateTask = false;
										string propsName = systemTask.GetStringValue("PropsName2");
										string goodsName = ProcessTask.GetPropNameGoodsName(propsName);
										int goodsLevel = ProcessTask.GetPropNameGoodsLevel(propsName);
										int goodsQuality = ProcessTask.GetPropNameGoodsQuality(propsName);
										int transferGoodsID = Global.GetGoodsByName(goodsName);
										if (goodsID == transferGoodsID)
										{
											GoodsData goodsData = Global.GetNotUsingGoodsByID(client, transferGoodsID, goodsLevel, goodsQuality);
											if (null != goodsData)
											{
												if (client.ClientData.TaskDataList[i].DoingTaskVal2 < systemTask.GetIntValue("TargetNum2", -1))
												{
													client.ClientData.TaskDataList[i].DoingTaskVal2++;
													GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
													{
														client.ClientData.RoleID,
														client.ClientData.TaskDataList[i].DoingTaskID,
														client.ClientData.TaskDataList[i].DbID,
														client.ClientData.TaskDataList[i].DoingTaskFocus,
														client.ClientData.TaskDataList[i].DoingTaskVal1,
														client.ClientData.TaskDataList[i].DoingTaskVal2
													}), null, client.ServerId);
													updateTask = true;
												}
											}
										}
										if (updateTask)
										{
											GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, taskid, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
											if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
											{
												int destNPC = systemTask.GetIntValue("DestNPC", -1);
												if (-1 != destNPC)
												{
													int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, destNPC, 0);
													GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, destNPC + 2130706432, state);
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

		
		private static void ProcessUsingSomething(SocketListener sl, TCPOutPacketPool pool, GameClient client, int npcID, int extensionID, int goodsID, TaskTypes taskType)
		{
			if (null != client.ClientData.TaskDataList)
			{
				if (-1 != goodsID)
				{
					long nowTicks = TimeUtil.NOW();
					SystemXmlItem systemTask = null;
					GameMap gameMap = GameManager.MapMgr.DictMaps[client.ClientData.MapCode];
					lock (client.ClientData.TaskDataList)
					{
						for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
						{
							int taskid = client.ClientData.TaskDataList[i].DoingTaskID;
							if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskid, out systemTask))
							{
								if (ProcessTask.IsTaskValid(client, systemTask, client.ClientData.TaskDataList[i], nowTicks))
								{
									if (systemTask.GetIntValue("TargetType1", -1) == 4)
									{
										bool updateTask = false;
										string goodsName = systemTask.GetStringValue("PropsName1");
										int transferGoodsID = Global.GetGoodsByName(goodsName);
										if (goodsID == transferGoodsID)
										{
											int targetMapCode = systemTask.GetIntValue("TargetMapCode1", -1);
											Point targetPos = Global.StrToPoint(systemTask.GetStringValue("TargetPos1"));
											if (targetMapCode >= 0 && !double.IsNaN(targetPos.X) && !double.IsNaN(targetPos.Y))
											{
												Point clientGrid = client.CurrentGrid;
												Point usingGoodsGrid = new Point((double)((int)(targetPos.X / (double)gameMap.MapGridWidth)), (double)((int)(targetPos.Y / (double)gameMap.MapGridHeight)));
												bool inGrid = Math.Abs(usingGoodsGrid.X - clientGrid.X) < 3.0 && Math.Abs(usingGoodsGrid.Y - clientGrid.Y) < 3.0;
												if (targetMapCode == client.ClientData.MapCode && inGrid)
												{
													if (client.ClientData.TaskDataList[i].DoingTaskVal1 < systemTask.GetIntValue("TargetNum1", -1))
													{
														client.ClientData.TaskDataList[i].DoingTaskVal1++;
														GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
														{
															client.ClientData.RoleID,
															client.ClientData.TaskDataList[i].DoingTaskID,
															client.ClientData.TaskDataList[i].DbID,
															client.ClientData.TaskDataList[i].DoingTaskFocus,
															client.ClientData.TaskDataList[i].DoingTaskVal1,
															client.ClientData.TaskDataList[i].DoingTaskVal2
														}), null, client.ServerId);
														updateTask = true;
													}
												}
											}
										}
										if (updateTask)
										{
											GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, taskid, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
											if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
											{
												int destNPC = systemTask.GetIntValue("DestNPC", -1);
												if (-1 != destNPC)
												{
													int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, destNPC, 0);
													GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, destNPC + 2130706432, state);
												}
											}
										}
									}
									if (systemTask.GetIntValue("TargetType2", -1) == 4)
									{
										bool updateTask = false;
										string goodsName = systemTask.GetStringValue("PropsName2");
										int transferGoodsID = Global.GetGoodsByName(goodsName);
										if (goodsID == transferGoodsID)
										{
											int targetMapCode2 = systemTask.GetIntValue("TargetMapCode2", -1);
											Point targetPos2 = Global.StrToPoint(systemTask.GetStringValue("TargetPos2"));
											if (targetMapCode2 >= 0 && !double.IsNaN(targetPos2.X) && !double.IsNaN(targetPos2.Y))
											{
												Point clientGrid = client.CurrentGrid;
												Point usingGoodsGrid = new Point((double)((int)(targetPos2.X / (double)gameMap.MapGridWidth)), (double)((int)(targetPos2.Y / (double)gameMap.MapGridHeight)));
												bool inGrid = Math.Abs(usingGoodsGrid.X - clientGrid.X) < 3.0 && Math.Abs(usingGoodsGrid.Y - clientGrid.Y) < 3.0;
												if (targetMapCode2 == client.ClientData.MapCode && inGrid)
												{
													if (client.ClientData.TaskDataList[i].DoingTaskVal2 < systemTask.GetIntValue("TargetNum2", -1))
													{
														client.ClientData.TaskDataList[i].DoingTaskVal2++;
														GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
														{
															client.ClientData.RoleID,
															client.ClientData.TaskDataList[i].DoingTaskID,
															client.ClientData.TaskDataList[i].DbID,
															client.ClientData.TaskDataList[i].DoingTaskFocus,
															client.ClientData.TaskDataList[i].DoingTaskVal1,
															client.ClientData.TaskDataList[i].DoingTaskVal2
														}), null, client.ServerId);
														updateTask = true;
													}
												}
											}
										}
										if (updateTask)
										{
											GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, taskid, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
											if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
											{
												int destNPC = systemTask.GetIntValue("DestNPC", -1);
												if (-1 != destNPC)
												{
													int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, destNPC, 0);
													GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, destNPC + 2130706432, state);
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

		
		private static void ProcessLuaHandle(SocketListener sl, TCPOutPacketPool pool, GameClient client, int npcID, int extensionID, int goodsID, TaskTypes taskType)
		{
			if (null != client.ClientData.TaskDataList)
			{
				long nowTicks = TimeUtil.NOW();
				bool updateTask = false;
				SystemXmlItem systemTask = null;
				NPC npc = NPCGeneralManager.FindNPC(client.ClientData.MapCode, extensionID);
				if (null != npc)
				{
					Point clientGrid = client.CurrentGrid;
					Point npcGrid = npc.CurrentGrid;
					if (Math.Abs(npcGrid.X - clientGrid.X) <= 9.0 && Math.Abs(npcGrid.Y - clientGrid.Y) <= 9.0)
					{
						lock (client.ClientData.TaskDataList)
						{
							for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
							{
								int taskid = client.ClientData.TaskDataList[i].DoingTaskID;
								if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskid, out systemTask))
								{
									if (ProcessTask.IsTaskValid(client, systemTask, client.ClientData.TaskDataList[i], nowTicks))
									{
										if (systemTask.GetIntValue("TargetType1", -1) == 9 || systemTask.GetIntValue("TargetType1", -1) == 10)
										{
											updateTask = false;
											if (client.ClientData.TaskDataList[i].DoingTaskVal1 < systemTask.GetIntValue("TargetNum1", -1))
											{
												client.ClientData.TaskDataList[i].DoingTaskVal1++;
												GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
												{
													client.ClientData.RoleID,
													client.ClientData.TaskDataList[i].DoingTaskID,
													client.ClientData.TaskDataList[i].DbID,
													client.ClientData.TaskDataList[i].DoingTaskFocus,
													client.ClientData.TaskDataList[i].DoingTaskVal1,
													client.ClientData.TaskDataList[i].DoingTaskVal2
												}), null, client.ServerId);
												updateTask = true;
											}
											if (updateTask)
											{
												GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, taskid, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
												if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
												{
													int destNPC = systemTask.GetIntValue("DestNPC", -1);
													if (-1 != destNPC)
													{
														int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, destNPC, 0);
														GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, destNPC + 2130706432, state);
													}
												}
											}
										}
										if (systemTask.GetIntValue("TargetType2", -1) == 9 || systemTask.GetIntValue("TargetType2", -1) == 10)
										{
											updateTask = false;
											if (client.ClientData.TaskDataList[i].DoingTaskVal2 < systemTask.GetIntValue("TargetNum2", -1))
											{
												client.ClientData.TaskDataList[i].DoingTaskVal2++;
												GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
												{
													client.ClientData.RoleID,
													client.ClientData.TaskDataList[i].DoingTaskID,
													client.ClientData.TaskDataList[i].DbID,
													client.ClientData.TaskDataList[i].DoingTaskFocus,
													client.ClientData.TaskDataList[i].DoingTaskVal1,
													client.ClientData.TaskDataList[i].DoingTaskVal2
												}), null, client.ServerId);
												updateTask = true;
											}
										}
										if (updateTask)
										{
											GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, taskid, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
											if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
											{
												int destNPC = systemTask.GetIntValue("DestNPC", -1);
												if (-1 != destNPC)
												{
													int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, destNPC, 0);
													GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, destNPC + 2130706432, state);
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

		
		public static bool IfActivateTask(GameClient client, int taskid)
		{
			SystemXmlItem systemTask = null;
			long nowTicks = TimeUtil.NOW();
			lock (client.ClientData.TaskDataList)
			{
				for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
				{
					if (taskid == client.ClientData.TaskDataList[i].DoingTaskID)
					{
						if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskid, out systemTask))
						{
							if (ProcessTask.IsTaskValid(client, systemTask, client.ClientData.TaskDataList[i], nowTicks))
							{
								return !Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2);
							}
						}
					}
				}
			}
			return false;
		}

		
		public static void GMSetMainTaskID(GameClient client, int taskID = 2000)
		{
			int roleID = client.ClientData.RoleID;
			client.ClientData.OldTasks = new List<OldTaskData>();
			client.ClientData.TaskDataList = new List<TaskData>();
			int mainTaskID = int.MaxValue;
			int npcID = 0;
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, SystemXmlItem> kv in GameManager.SystemTasksMgr.SystemXmlItemDict)
			{
				SystemXmlItem systemTask = kv.Value;
				if (kv.Key < mainTaskID && kv.Key >= taskID)
				{
					mainTaskID = kv.Key;
					npcID = kv.Value.GetIntValue("DestNPC", -1);
				}
				if (kv.Key < taskID)
				{
					list.Add(kv.Key);
				}
			}
			list.Sort();
			list.Insert(0, roleID);
			if (list.Count > 2)
			{
				list.RemoveRange(1, list.Count - 2);
			}
			Global.sendToDB<int, byte[]>(13000, DataHelper.ObjectToBytes<List<int>>(list), client.ServerId);
			client.sendCmd(140, string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				roleID,
				npcID,
				list[list.Count - 1],
				1
			}), false);
			TCPOutPacket tcpOutPacketTemp = null;
			TCPProcessCmdResults result = Global.TakeNewTask(TCPManager.getInstance(), client.ClientSocket, TCPManager.getInstance().tcpClientPool, TCPManager.getInstance().tcpRandKey, TCPManager.getInstance().TcpOutPacketPool, 125, client, roleID, mainTaskID, npcID, out tcpOutPacketTemp);
			if (result == TCPProcessCmdResults.RESULT_DATA && null != tcpOutPacketTemp)
			{
				client.sendCmd(tcpOutPacketTemp, true);
			}
			Global.ForceCloseClient(client, "", true);
		}

		
		public static void ProcessTaskValue(SocketListener sl, TCPOutPacketPool pool, GameClient client, string taskName, int valType, int taskVal)
		{
			if (null != client.ClientData.TaskDataList)
			{
				SystemXmlItem systemTask = null;
				lock (client.ClientData.TaskDataList)
				{
					for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
					{
						int taskid = client.ClientData.TaskDataList[i].DoingTaskID;
						if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskid, out systemTask))
						{
							if (!(taskName != systemTask.GetStringValue("Title")))
							{
								if (1 == valType)
								{
									bool updateTask = false;
									if (client.ClientData.TaskDataList[i].DoingTaskVal1 < systemTask.GetIntValue("TargetNum1", -1))
									{
										client.ClientData.TaskDataList[i].DoingTaskVal1 = Global.GMin(taskVal, systemTask.GetIntValue("TargetNum1", -1));
										GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
										{
											client.ClientData.RoleID,
											client.ClientData.TaskDataList[i].DoingTaskID,
											client.ClientData.TaskDataList[i].DbID,
											client.ClientData.TaskDataList[i].DoingTaskFocus,
											client.ClientData.TaskDataList[i].DoingTaskVal1,
											client.ClientData.TaskDataList[i].DoingTaskVal2
										}), null, client.ServerId);
										updateTask = true;
									}
									if (updateTask)
									{
										GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, taskid, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
										if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
										{
											int destNPC = systemTask.GetIntValue("DestNPC", -1);
											if (-1 != destNPC)
											{
												int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, destNPC, 0);
												GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, destNPC + 2130706432, state);
											}
										}
									}
								}
								else if (2 == valType)
								{
									bool updateTask = false;
									if (client.ClientData.TaskDataList[i].DoingTaskVal2 < systemTask.GetIntValue("TargetNum2", -1))
									{
										client.ClientData.TaskDataList[i].DoingTaskVal2 = Global.GMin(taskVal, systemTask.GetIntValue("TargetNum2", -1));
										GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
										{
											client.ClientData.RoleID,
											client.ClientData.TaskDataList[i].DoingTaskID,
											client.ClientData.TaskDataList[i].DbID,
											client.ClientData.TaskDataList[i].DoingTaskFocus,
											client.ClientData.TaskDataList[i].DoingTaskVal1,
											client.ClientData.TaskDataList[i].DoingTaskVal2
										}), null, client.ServerId);
										updateTask = true;
									}
									if (updateTask)
									{
										GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, taskid, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
										if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
										{
											int destNPC = systemTask.GetIntValue("DestNPC", -1);
											if (-1 != destNPC)
											{
												int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, destNPC, 0);
												GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, destNPC + 2130706432, state);
											}
										}
									}
								}
								break;
							}
						}
					}
				}
			}
		}

		
		public static void ClearTaskGoods(SocketListener sl, TCPOutPacketPool pool, GameClient client, int taskID)
		{
			if (null != client.ClientData.TaskDataList)
			{
				TaskData taskData = Global.GetTaskData(client, taskID);
				if (null != taskData)
				{
					SystemXmlItem systemTask = null;
					if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskData.DoingTaskID, out systemTask))
					{
						if (systemTask.GetIntValue("TargetType1", -1) == 5 && "" != systemTask.GetStringValue("PropsName1"))
						{
							string propsName = systemTask.GetStringValue("PropsName1");
							string goodsName = ProcessTask.GetPropNameGoodsName(propsName);
							int goodsLevel = ProcessTask.GetPropNameGoodsLevel(propsName);
							int goodsQuality = ProcessTask.GetPropNameGoodsQuality(propsName);
							int transferGoodsID = Global.GetGoodsByName(goodsName);
							if (transferGoodsID >= 0)
							{
								GoodsData goodsData = Global.GetNotUsingGoodsByID(client, transferGoodsID, goodsLevel, goodsQuality);
								if (null != goodsData)
								{
									int catetoriy = Global.GetGoodsCatetoriy(transferGoodsID);
									if (catetoriy >= 49)
									{
										GameManager.ClientMgr.NotifyUseGoods(sl, Global._TCPManager.tcpClientPool, pool, client, goodsData.Id, false, false);
									}
								}
							}
						}
						else if (systemTask.GetIntValue("TargetType1", -1) == 4 && "" != systemTask.GetStringValue("PropsName1"))
						{
							string goodsName = systemTask.GetStringValue("PropsName1");
							int transferGoodsID = Global.GetGoodsByName(goodsName);
							if (transferGoodsID >= 0)
							{
								GoodsData goodsData = Global.GetNotUsingGoodsByID(client, transferGoodsID, 0, 0);
								if (null != goodsData)
								{
									int catetoriy = Global.GetGoodsCatetoriy(transferGoodsID);
									if (catetoriy >= 49)
									{
										GameManager.ClientMgr.NotifyUseGoods(sl, Global._TCPManager.tcpClientPool, pool, client, goodsData.Id, false, false);
									}
								}
							}
						}
						if (systemTask.GetIntValue("TargetType2", -1) == 5 && "" != systemTask.GetStringValue("PropsName2"))
						{
							string propsName = systemTask.GetStringValue("PropsName2");
							string goodsName = ProcessTask.GetPropNameGoodsName(propsName);
							int goodsLevel = ProcessTask.GetPropNameGoodsLevel(propsName);
							int goodsQuality = ProcessTask.GetPropNameGoodsQuality(propsName);
							int transferGoodsID = Global.GetGoodsByName(goodsName);
							if (transferGoodsID >= 0)
							{
								GoodsData goodsData = Global.GetNotUsingGoodsByID(client, transferGoodsID, goodsLevel, goodsQuality);
								if (null != goodsData)
								{
									int catetoriy = Global.GetGoodsCatetoriy(transferGoodsID);
									if (catetoriy >= 49)
									{
										GameManager.ClientMgr.NotifyUseGoods(sl, Global._TCPManager.tcpClientPool, pool, client, goodsData.Id, false, false);
									}
								}
							}
						}
						else if (systemTask.GetIntValue("TargetType2", -1) == 4 && "" != systemTask.GetStringValue("PropsName2"))
						{
							string goodsName = systemTask.GetStringValue("PropsName2");
							int transferGoodsID = Global.GetGoodsByName(goodsName);
							if (transferGoodsID >= 0)
							{
								GoodsData goodsData = Global.GetNotUsingGoodsByID(client, transferGoodsID, 0, 0);
								if (null != goodsData)
								{
									int catetoriy = Global.GetGoodsCatetoriy(transferGoodsID);
									if (catetoriy >= 49)
									{
										GameManager.ClientMgr.NotifyUseGoods(sl, Global._TCPManager.tcpClientPool, pool, client, goodsData.Id, false, false);
									}
								}
							}
						}
					}
				}
			}
		}

		
		public static bool Complete(SocketListener sl, TCPOutPacketPool pool, GameClient client, int npcID, int extensionID, int taskID, int dbID, bool useYuanBao, double expBeiShu = 1.0, bool bIsOneClickComlete = false)
		{
			bool result;
			if (null == client.ClientData.TaskDataList)
			{
				result = false;
			}
			else
			{
				int findIndex = -1;
				lock (client.ClientData.TaskDataList)
				{
					for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
					{
						if (client.ClientData.TaskDataList[i].DbID == dbID)
						{
							if (bIsOneClickComlete)
							{
								findIndex = i;
								break;
							}
							if (Global.JugeTaskComplete(client, taskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
							{
								findIndex = i;
								break;
							}
						}
					}
				}
				if (findIndex < 0)
				{
					result = false;
				}
				else
				{
					TaskData taskData = null;
					lock (client.ClientData.TaskDataList)
					{
						if (findIndex < client.ClientData.TaskDataList.Count)
						{
							taskData = client.ClientData.TaskDataList[findIndex];
							client.ClientData.TaskDataList.RemoveAt(findIndex);
						}
					}
					if (null == taskData)
					{
						result = false;
					}
					else
					{
						SystemXmlItem systemTask = null;
						if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTask))
						{
							result = false;
						}
						else
						{
							int taskClass = systemTask.GetIntValue("TaskClass", -1);
							if (taskClass != 0)
							{
								Global.AddOldTask(client, taskID);
							}
							if ((taskClass >= 3 && taskClass <= 9) || (taskClass >= 100 && taskClass <= 150))
							{
								int minLevel = systemTask.GetIntValue("MinLevel", -1);
								if (!Global.UpdateDailyTaskData(client, minLevel / 10, taskData.AddDateTime, taskClass, bIsOneClickComlete))
								{
									return false;
								}
							}
							GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, -1, taskID, 0, 0, 0, -1L);
							int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, npcID - 2130706432, taskID);
							GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, npcID, state);
							int sourceNPC = systemTask.GetIntValue("SourceNPC", -1);
							if (-1 != sourceNPC && npcID - 2130706432 != sourceNPC)
							{
								state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, sourceNPC, taskID);
								GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, sourceNPC + 2130706432, state);
							}
							bool usedBinding = false;
							bool usedTimeLimited = false;
							if ((systemTask.GetIntValue("TargetType1", -1) == 2 || systemTask.GetIntValue("TargetType1", -1) == 8) && "" != systemTask.GetStringValue("PropsName1"))
							{
								int targetNum = systemTask.GetIntValue("TargetNum1", -1);
								string goodsName = systemTask.GetStringValue("PropsName1");
								int transferGoodsID = Global.GetGoodsByName(goodsName);
								if (transferGoodsID >= 0)
								{
									int catetoriy = Global.GetGoodsCatetoriy(transferGoodsID);
									if (catetoriy >= 49)
									{
										usedBinding = false;
										GameManager.ClientMgr.NotifyUseGoods(sl, Global._TCPManager.tcpClientPool, pool, client, transferGoodsID, targetNum, false, out usedBinding, out usedTimeLimited, false);
									}
								}
							}
							if ((systemTask.GetIntValue("TargetType2", -1) == 2 || systemTask.GetIntValue("TargetType2", -1) == 8) && "" != systemTask.GetStringValue("PropsName2"))
							{
								int targetNum2 = systemTask.GetIntValue("TargetNum2", -1);
								string goodsName = systemTask.GetStringValue("PropsName2");
								int transferGoodsID = Global.GetGoodsByName(goodsName);
								if (transferGoodsID >= 0)
								{
									int catetoriy = Global.GetGoodsCatetoriy(transferGoodsID);
									if (catetoriy >= 49)
									{
										usedBinding = false;
										GameManager.ClientMgr.NotifyUseGoods(sl, Global._TCPManager.tcpClientPool, pool, client, transferGoodsID, targetNum2, false, out usedBinding, out usedTimeLimited, false);
									}
								}
							}
							int needYuanBao = GameManager.TaskAwardsMgr.FindNeedYuanBao(client, taskID);
							int nAddExp = 0;
							int nAddMoJing = 0;
							int nAddGoodID = 0;
							int nGoodNum = 0;
							int nBinding = 1;
							int nAddXingHun = 0;
							bool bIsDailyCircleTask = false;
							TaskStarDataInfo TaskStarInfoTmp = null;
							int nExBindYuanBao = 0;
							if (taskClass == 8)
							{
								DailyTaskData dailyTaskData = Global.FindDailyTaskDataByTaskClass(client, taskClass);
								if (dailyTaskData != null && taskData.StarLevel > 0 && taskData.StarLevel <= Data.TaskStarInfo.Count)
								{
									if (dailyTaskData.RecNum == Global.GetMaxDailyTaskNum(client, taskClass, dailyTaskData))
									{
										int nIndex = Global.GetDailyCircleTaskAddAward(client);
										if (nIndex > 0)
										{
											nAddExp = Data.DailyCircleTaskAward[nIndex].Experience;
											nAddMoJing = Data.DailyCircleTaskAward[nIndex].MoJing;
											nAddGoodID = Data.DailyCircleTaskAward[nIndex].GoodsID;
											nGoodNum = Data.DailyCircleTaskAward[nIndex].GoodsNum;
											nBinding = Data.DailyCircleTaskAward[nIndex].Binding;
											nAddXingHun = Data.DailyCircleTaskAward[nIndex].XingHun;
										}
									}
									TaskStarInfoTmp = Data.TaskStarInfo[taskData.StarLevel - 1];
									if (TaskStarInfoTmp != null)
									{
										bIsDailyCircleTask = true;
									}
								}
								DailyActiveManager.ProcessCompleteDailyTaskForDailyActive(client, dailyTaskData.RecNum);
							}
							else if (taskClass == 9)
							{
								DailyTaskData dailyTaskData = Global.FindDailyTaskDataByTaskClass(client, taskClass);
								if (dailyTaskData != null)
								{
									if (dailyTaskData.RecNum == Global.GetMaxDailyTaskNum(client, taskClass, dailyTaskData))
									{
										nExBindYuanBao = Data.TaofaTaskExAward.BangZuan;
									}
								}
							}
							long experience = GameManager.TaskAwardsMgr.FindExperience(client, taskID);
							if (experience > 0L)
							{
								if (useYuanBao)
								{
									experience *= 2L;
								}
								experience = (long)((double)experience * expBeiShu);
								if (bIsDailyCircleTask)
								{
									experience = (long)((double)experience * TaskStarInfoTmp.ExpModulus);
								}
								experience += (long)nAddExp;
								GameManager.ClientMgr.ProcessRoleExperience(client, experience, true, false, false, "none");
							}
							if (Global.FilterFallGoods(client))
							{
								List<GoodsData> goodsDataList = Global.GetTaskAwardsGoodsGridCount(client, taskID);
								if (nAddGoodID > 0 && nGoodNum > 0)
								{
									GoodsData addGood = new GoodsData();
									addGood.GoodsID = nAddGoodID;
									addGood.GCount = nGoodNum;
									addGood.Binding = nBinding;
									addGood.Endtime = "1900-01-01 12:00:00";
									if (goodsDataList == null)
									{
										goodsDataList = new List<GoodsData>();
									}
									goodsDataList.Add(addGood);
								}
								if (goodsDataList != null && goodsDataList.Count > 0)
								{
									if (!Global.CanAddGoodsDataList(client, goodsDataList))
									{
										ProcessTask.SendMailWhenPacketFull(client, goodsDataList, GLang.GetLang(4019, new object[0]), GLang.GetLang(4019, new object[0]));
									}
									else
									{
										for (int i = 0; i < goodsDataList.Count; i++)
										{
											Global.AddGoodsDBCommand(pool, client, goodsDataList[i].GoodsID, goodsDataList[i].GCount, goodsDataList[i].Quality, "", goodsDataList[i].Forge_level, goodsDataList[i].Binding, 0, "", true, 1, "任务奖励", goodsDataList[i].Endtime, 0, goodsDataList[i].BornIndex, goodsDataList[i].Lucky, 0, goodsDataList[i].ExcellenceInfo, goodsDataList[i].AppendPropLev, 0, null, null, 0, true);
										}
									}
								}
							}
							int money = GameManager.TaskAwardsMgr.FindMoney(taskID);
							if (0 < money)
							{
								money = Global.FilterValue(client, money);
								GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, money, "完成任务：" + taskID, false);
								GameManager.SystemServerEvents.AddEvent(string.Format("角色获取金钱, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
								{
									client.ClientData.RoleID,
									client.ClientData.RoleName,
									client.ClientData.Money1,
									money
								}), EventLevels.Record);
							}
							int yinLiang = GameManager.TaskAwardsMgr.FindYinLiang(taskID);
							if (0 < yinLiang)
							{
								yinLiang = Global.FilterValue(client, yinLiang);
								GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, yinLiang, "完成任务：" + taskID, false);
								GameManager.SystemServerEvents.AddEvent(string.Format("角色获取银两, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
								{
									client.ClientData.RoleID,
									client.ClientData.RoleName,
									client.ClientData.YinLiang,
									yinLiang
								}), EventLevels.Record);
							}
							int bindYuanBao = GameManager.TaskAwardsMgr.FindBindYuanBao(taskID);
							bindYuanBao += nExBindYuanBao;
							if (0 < bindYuanBao)
							{
								bindYuanBao = Global.FilterValue(client, bindYuanBao);
								GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, bindYuanBao, "完成任务：" + taskID);
								GameManager.SystemServerEvents.AddEvent(string.Format("角色获取绑定元宝, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
								{
									client.ClientData.RoleID,
									client.ClientData.RoleName,
									client.ClientData.Gold,
									bindYuanBao
								}), EventLevels.Record);
							}
							if (0 < nAddMoJing)
							{
								GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, nAddMoJing, "日常跑环", false, true, false);
							}
							int lingLi = GameManager.TaskAwardsMgr.FindLingLi(taskID);
							if (0 < lingLi)
							{
								GameManager.ClientMgr.AddInterPower(client, lingLi, true, false);
								GameManager.SystemServerEvents.AddEvent(string.Format("角色获取灵力, roleID={0}({1}), LingLi={2}, newLingLi={3}", new object[]
								{
									client.ClientData.RoleID,
									client.ClientData.RoleName,
									client.ClientData.InterPower,
									lingLi
								}), EventLevels.Record);
							}
							int blessPoint = GameManager.TaskAwardsMgr.FindBlessPoint(taskID);
							if (blessPoint > 0)
							{
								blessPoint = Global.FilterValue(client, blessPoint);
								int retCode = ProcessHorse.ProcessAddHorseAwardLucky(client, blessPoint, false, "坐骑任务");
								if (retCode >= 0)
								{
									GameManager.SystemServerEvents.AddEvent(string.Format("角色获取任务祝福点成功, roleID={0}({1}), newBlessPoint={2}, RetCode={3}", new object[]
									{
										client.ClientData.RoleID,
										client.ClientData.RoleName,
										blessPoint,
										retCode
									}), EventLevels.Record);
								}
								else
								{
									GameManager.SystemServerEvents.AddEvent(string.Format("角色获取任务祝福点失败, roleID={0}({1}), newBlessPoint={2}, RetCode={3}", new object[]
									{
										client.ClientData.RoleID,
										client.ClientData.RoleName,
										blessPoint,
										retCode
									}), EventLevels.Record);
								}
							}
							int zhenQi = GameManager.TaskAwardsMgr.FindZhenQi(client, taskID);
							if (zhenQi > 0)
							{
								zhenQi = Global.FilterValue(client, zhenQi);
								GameManager.ClientMgr.ModifyZhenQiValue(client, zhenQi, true, true);
								GameManager.SystemServerEvents.AddEvent(string.Format("角色获取任务阵旗成功, roleID={0}({1}), newBlessPoint={2}, RetCode={3}", new object[]
								{
									client.ClientData.RoleID,
									client.ClientData.RoleName,
									zhenQi,
									0
								}), EventLevels.Record);
							}
							int lieSha = GameManager.TaskAwardsMgr.FindLieSha(client, taskID);
							if (lieSha > 0)
							{
								lieSha = Global.FilterValue(client, lieSha);
								if (useYuanBao)
								{
									lieSha *= 2;
								}
								GameManager.ClientMgr.ModifyLieShaValue(client, lieSha, true, true);
								GameManager.SystemServerEvents.AddEvent(string.Format("角色获取任务猎杀值成功, roleID={0}({1}), newBlessPoint={2}, RetCode={3}", new object[]
								{
									client.ClientData.RoleID,
									client.ClientData.RoleName,
									lieSha,
									0
								}), EventLevels.Record);
							}
							int wuXing = GameManager.TaskAwardsMgr.FindWuXing(client, taskID);
							if (wuXing > 0)
							{
								wuXing = Global.FilterValue(client, wuXing);
								GameManager.ClientMgr.ModifyWuXingValue(client, wuXing, true, true, true);
								GameManager.SystemServerEvents.AddEvent(string.Format("角色获取任务悟性值成功, roleID={0}({1}), newBlessPoint={2}, RetCode={3}", new object[]
								{
									client.ClientData.RoleID,
									client.ClientData.RoleName,
									wuXing,
									0
								}), EventLevels.Record);
							}
							int junGong = GameManager.TaskAwardsMgr.FindJunGong(client, taskID);
							if (junGong > 0)
							{
								junGong = Global.FilterValue(client, junGong);
								GameManager.ClientMgr.ModifyJunGongValue(client, junGong, true, true);
								GameManager.SystemServerEvents.AddEvent(string.Format("角色获取任务军功值成功, roleID={0}({1}), newBlessPoint={2}, RetCode={3}", new object[]
								{
									client.ClientData.RoleID,
									client.ClientData.RoleName,
									junGong,
									0
								}), EventLevels.Record);
							}
							int rongYu = GameManager.TaskAwardsMgr.FindRongYu(client, taskID);
							if (rongYu > 0)
							{
								rongYu = Global.FilterValue(client, rongYu);
								GameManager.ClientMgr.ModifyRongYuValue(client, rongYu, true, true);
								GameManager.SystemServerEvents.AddEvent(string.Format("角色获取任务荣誉值成功, roleID={0}({1}), newBlessPoint={2}, RetCode={3}", new object[]
								{
									client.ClientData.RoleID,
									client.ClientData.RoleName,
									rongYu,
									0
								}), EventLevels.Record);
							}
							int nMoJing = GameManager.TaskAwardsMgr.FindMoJing(client, taskID);
							if (nMoJing > 0)
							{
								nMoJing = Global.FilterValue(client, nMoJing);
								if (bIsDailyCircleTask)
								{
									nMoJing = (int)((double)nMoJing * TaskStarInfoTmp.BindYuanBaoModulus);
								}
								GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, nMoJing, "过滤奖励", false, true, false);
								GameManager.SystemServerEvents.AddEvent(string.Format("角色获取任务魔晶值成功, roleID={0}({1}), newBlessPoint={2}, RetCode={3}", new object[]
								{
									client.ClientData.RoleID,
									client.ClientData.RoleName,
									nMoJing,
									0
								}), EventLevels.Record);
							}
							int nXingHun = GameManager.TaskAwardsMgr.FindXingHun(client, taskID);
							if (nXingHun > 0)
							{
								nXingHun = Global.FilterValue(client, nXingHun);
								if (bIsDailyCircleTask)
								{
									nXingHun = (int)((double)nXingHun * TaskStarInfoTmp.StarSoulModulus);
								}
								nXingHun += nAddXingHun;
								GameManager.ClientMgr.ModifyStarSoulValue(client, nXingHun, "过滤奖励", true, true);
							}
							int nCompDonate = GameManager.TaskAwardsMgr.FindCompDonate(client, taskID);
							if (nCompDonate > 0)
							{
								nCompDonate = Global.FilterValue(client, nCompDonate);
								GameManager.ClientMgr.ModifyCompDonateValue(client, nCompDonate, "过滤奖励", true, true, false);
							}
							int nCompJunXian = GameManager.TaskAwardsMgr.FindCompJunXian(client, taskID);
							if (nCompJunXian > 0 && client.ClientData.CompType > 0)
							{
								nCompJunXian = Global.FilterValue(client, nCompJunXian);
								TianTiClient.getInstance().Comp_CompOpt(client.ClientData.CompType, 1, client.ClientData.RoleID, nCompJunXian);
								string broadMsg = string.Format(GLang.GetLang(4017, new object[0]), nCompJunXian);
								GameManager.ClientMgr.NotifyImportantMsg(client, broadMsg, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							}
							if (taskID == Data.InsertAwardtPortableBagTaskID)
							{
								string[] strID = Data.InsertAwardtPortableBagGoodsInfo.Split(new char[]
								{
									','
								});
								if (strID != null)
								{
									GoodsData goodsdata = new GoodsData();
									goodsdata.GoodsID = Global.SafeConvertToInt32(strID[0]);
									goodsdata.GCount = Global.SafeConvertToInt32(strID[1]);
									goodsdata.Binding = Global.SafeConvertToInt32(strID[2]);
									goodsdata.Forge_level = Global.SafeConvertToInt32(strID[3]);
									goodsdata.AppendPropLev = Global.SafeConvertToInt32(strID[4]);
									goodsdata.Lucky = Global.SafeConvertToInt32(strID[5]);
									goodsdata.ExcellenceInfo = Global.SafeConvertToInt32(strID[6]);
									goodsdata.Site = -1000;
									Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsdata.GoodsID, goodsdata.GCount, 0, "", goodsdata.Forge_level, goodsdata.Binding, goodsdata.Site, "", true, 1, "引导给物品到仓库", "1900-01-01 12:00:00", 0, 0, goodsdata.Lucky, 0, goodsdata.ExcellenceInfo, goodsdata.AppendPropLev, goodsdata.ChangeLifeLevForEquip, null, null, 0, true);
								}
							}
							Global.AddRoleTaskEvent(client, taskID);
							if (taskClass == 0)
							{
								Global.UpdateTaskZhangJieProp(client, taskID, false);
							}
							result = true;
						}
					}
				}
			}
			return result;
		}

		
		public static void SendMailWhenPacketFull(GameClient client, List<GoodsData> awardsItemList, string strSubject, string strContent)
		{
			int nTotalGroup = awardsItemList.Count / 5;
			int nRemain = awardsItemList.Count % 5;
			int nCount = 0;
			if (nTotalGroup > 0)
			{
				for (int i = 0; i < nTotalGroup; i++)
				{
					List<GoodsData> goods = new List<GoodsData>();
					for (int j = 0; j < 5; j++)
					{
						goods.Add(awardsItemList[nCount]);
						nCount++;
					}
					Global.UseMailGivePlayerAward2(client, goods, strSubject, strContent, 0, 0, 0);
				}
			}
			if (nRemain > 0)
			{
				List<GoodsData> goods2 = new List<GoodsData>();
				for (int i = 0; i < nRemain; i++)
				{
					goods2.Add(awardsItemList[nCount]);
					nCount++;
				}
				Global.UseMailGivePlayerAward2(client, goods2, strSubject, strContent, 0, 0, 0);
			}
		}

		
		public static Dictionary<int, List<BranchTaskInfo>> BranchTaskInfoDic = new Dictionary<int, List<BranchTaskInfo>>();

		
		public static Dictionary<long, List<int>> ActiveBranchTaskInfoDic = new Dictionary<long, List<int>>();
	}
}
