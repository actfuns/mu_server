using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Logic.FluorescentGem;
using GameServer.Logic.Marriage.CoupleArena;
using GameServer.Logic.TuJian;
using GameServer.Server;
using HSGameEngine.Tools.AStar;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class GlobalNew
	{
		
		public static bool IsGongNengOpened(GameClient client, GongNengIDs id, bool hint = false)
		{
			int versionGongNengId = (int)id;
			if (versionGongNengId >= 100000 && versionGongNengId < 120000)
			{
				versionGongNengId -= 100000;
			}
			bool result;
			if (!GameManager.VersionSystemOpenMgr.IsVersionSystemOpen(versionGongNengId))
			{
				result = false;
			}
			else if (null == client)
			{
				result = true;
			}
			else if (client.ClientData.HideGM > 0)
			{
				result = true;
			}
			else
			{
				SystemXmlItem xmlItem = null;
				if (GameManager.SystemSystemOpen.SystemXmlItemDict.TryGetValue((int)id, out xmlItem))
				{
					int trigger = xmlItem.GetIntValue("TriggerCondition", -1);
					if (trigger == 1)
					{
						int[] paramArray = xmlItem.GetIntArrayValue("TimeParameters", ',');
						if (paramArray.Length == 2)
						{
							if (Global.GetUnionLevel(paramArray[0], paramArray[1], false) > Global.GetUnionLevel(client, false))
							{
								if (hint)
								{
									string msg = string.Format(GLang.GetLang(374, new object[0]), paramArray[0], paramArray[1]);
									GameManager.ClientMgr.NotifyHintMsg(client, msg);
								}
								return false;
							}
						}
						return true;
					}
					if (trigger == 7)
					{
						int taskId = xmlItem.GetIntValue("TimeParameters", -1);
						if (client.ClientData.MainTaskID < taskId)
						{
							if (hint)
							{
								string msg = string.Format(GLang.GetLang(375, new object[0]), GlobalNew.GetTaskName(taskId));
								GameManager.ClientMgr.NotifyHintMsg(client, msg);
							}
							return false;
						}
						return true;
					}
					else if (trigger == 14)
					{
						string str = xmlItem.GetStringValue("TimeParameters");
						if (string.IsNullOrEmpty(str))
						{
							return true;
						}
						string[] fields = str.Split(new char[]
						{
							','
						});
						if (fields.Length != 2)
						{
							return true;
						}
						int suit = Convert.ToInt32(fields[0]);
						int star = Convert.ToInt32(fields[1]);
						return client.ClientData.MyWingData.WingID > suit || (client.ClientData.MyWingData.WingID == suit && client.ClientData.MyWingData.ForgeLevel >= star);
					}
					else if (trigger == 15)
					{
						if (client.ClientData.ChengJiuLevel < xmlItem.GetIntValue("TimeParameters", -1))
						{
							return false;
						}
					}
					else if (trigger == 16)
					{
						int junxian = GameManager.ClientMgr.GetShengWangLevelValue(client);
						if (junxian < xmlItem.GetIntValue("TimeParameters", -1))
						{
							return false;
						}
					}
					else if (trigger == 17)
					{
						if (TimeUtil.GetOffsetDays(Global.GetKaiFuTime()) < xmlItem.GetIntValue("TimeParameters", -1))
						{
							return false;
						}
					}
					else if (trigger == 18)
					{
						if ((ulong)client.ClientData.TotalDayLoginNum < (ulong)((long)xmlItem.GetIntValue("TimeParameters", -1)))
						{
							return false;
						}
					}
					else if (trigger == 20)
					{
						int bangHuiLevel = Global.GetBangHuiLevel(client);
						if (bangHuiLevel < xmlItem.GetIntValue("TimeParameters", -1))
						{
							return false;
						}
					}
					else if (trigger == 21)
					{
						if (client.ClientData.RebornLevel < xmlItem.GetIntValue("TimeParameters", -1))
						{
							return false;
						}
					}
				}
				result = true;
			}
			return result;
		}

		
		public static void RefreshGongNeng(GameClient client)
		{
			CaiJiLogic.InitRoleDailyCaiJiData(client, false, false);
			HuanYingSiYuanManager.getInstance().InitRoleDailyHYSYData(client);
			Global.InitRoleDailyTaskData(client, false);
			SingletonTemplate<GuardStatueManager>.Instance().OnTaskComplete(client);
			GameManager.MerlinMagicBookMgr.InitMerlinMagicBook(client);
			SingletonTemplate<SoulStoneManager>.Instance().CheckOpen(client);
			SingletonTemplate<ZhengBaManager>.Instance().CheckGongNengCanOpen(client);
			FundManager.initFundData(client);
			SingletonTemplate<CoupleArenaManager>.Instance().CheckGongNengOpen(client);
			ShenShiManager.getInstance().InitRoleShenShiData(client);
			JueXingManager.getInstance().InitRoleJueXingData(client);
			ZuoQiManager.getInstance().InitRoleZuoQiData(client);
			GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
			GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
		}

		
		public static int GetFuBenTabNeedTask(int fuBenTabId)
		{
			int needTaskId = 0;
			if (!Data.FuBenNeedDict.TryGetValue(fuBenTabId, out needTaskId))
			{
				needTaskId = 0;
			}
			return needTaskId;
		}

		
		public static bool IsExtraGongNengOpen(GameClient client, ExtraGongNengIds extGongId)
		{
			int needLevel = 0;
			int needTask = 0;
			int needVip = 0;
			if (extGongId == ExtraGongNengIds.DiaoXiangMoBai)
			{
				needLevel = (int)GameManager.systemParamsList.GetParamValueIntByName("MoBaiLevel", -1);
			}
			return (needLevel <= 0 || needLevel <= Global.GetUnionLevel(client, false)) && (needTask <= 0 || needTask <= client.ClientData.MainTaskID) && (needVip <= 0 || needVip <= client.ClientData.VipLevel);
		}

		
		public static string GetTaskName(int taskId)
		{
			SystemXmlItem systemTask = null;
			string result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskId, out systemTask))
			{
				result = taskId.ToString();
			}
			else
			{
				result = systemTask.GetStringValue("Title");
			}
			return result;
		}

		
		public static string PrintRoleProps(string otherRoleIdOrName)
		{
			string rolePropsStr = null;
			try
			{
				int roleId = RoleName2IDs.FindRoleIDByName(otherRoleIdOrName, false);
				if (-1 == roleId)
				{
					if (!int.TryParse(otherRoleIdOrName, out roleId))
					{
						return rolePropsStr;
					}
				}
				GameClient otherClient = GameManager.ClientMgr.FindClient(roleId);
				if (null == otherClient)
				{
					return rolePropsStr;
				}
				StringBuilder sb = new StringBuilder();
				Global.PrintSomeProps(otherClient, ref sb);
				rolePropsStr = sb.ToString();
			}
			catch (Exception ex)
			{
			}
			return rolePropsStr;
		}

		
		public static bool GetNpcTaskData(GameClient client, int extensionID, NPCData npcData)
		{
			List<int> tasksList = null;
			bool result;
			if (!GameManager.NPCTasksMgr.SourceNPCTasksDict.TryGetValue(extensionID, out tasksList))
			{
				result = false;
			}
			else if (0 == tasksList.Count)
			{
				result = false;
			}
			else
			{
				Dictionary<int, GlobalNew.NpcCircleTaskData> all_circleTask = null;
				for (int i = 0; i < tasksList.Count; i++)
				{
					int taskID = tasksList[i];
					SystemXmlItem systemTask = null;
					if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTask))
					{
						int taskClass = systemTask.GetIntValue("TaskClass", -1);
						if ((taskClass >= 3 && taskClass <= 9) || (taskClass >= 100 && taskClass <= 150))
						{
							if (Global.CanTaskPaoHuanTask(client, taskClass))
							{
								if (Global.CanTakeNewTask(client, taskID, systemTask))
								{
									GlobalNew.NpcCircleTaskData circletask = null;
									if (all_circleTask == null || !all_circleTask.TryGetValue(taskClass, out circletask))
									{
										circletask = new GlobalNew.NpcCircleTaskData();
										circletask.taskclass = taskClass;
										circletask.oldTaskID = PaoHuanTasksMgr.FindPaoHuanHistTaskID(client.ClientData.RoleID, taskClass);
										if (circletask.oldTaskID >= 0)
										{
											if (!Global.CanTakeNewTask(client, circletask.oldTaskID, null))
											{
												circletask.oldTaskID = -1;
											}
										}
										if (null == all_circleTask)
										{
											all_circleTask = new Dictionary<int, GlobalNew.NpcCircleTaskData>();
										}
										all_circleTask[taskClass] = circletask;
									}
									if (null != circletask)
									{
										circletask.NpcAttachedTaskID.Add(taskID);
									}
								}
							}
						}
						else if (Global.CanTakeNewTask(client, taskID, systemTask))
						{
							if (null == npcData.NewTaskIDs)
							{
								npcData.NewTaskIDs = new List<int>();
							}
							npcData.NewTaskIDs.Add(taskID);
							if (2 == taskClass)
							{
								OldTaskData oldTaskData = Global.FindOldTaskByTaskID(client, tasksList[i]);
								int doneCount = (oldTaskData == null) ? 0 : oldTaskData.DoCount;
								if (null == npcData.NewTaskIDsDoneCount)
								{
									npcData.NewTaskIDsDoneCount = new List<int>();
								}
								npcData.NewTaskIDsDoneCount.Add(doneCount);
							}
							else
							{
								if (null == npcData.NewTaskIDsDoneCount)
								{
									npcData.NewTaskIDsDoneCount = new List<int>();
								}
								npcData.NewTaskIDsDoneCount.Add(0);
							}
						}
					}
				}
				if (null == all_circleTask)
				{
					result = true;
				}
				else
				{
					foreach (KeyValuePair<int, GlobalNew.NpcCircleTaskData> circletask2 in all_circleTask)
					{
						bool needRandom = false;
						if (-1 != circletask2.Value.oldTaskID)
						{
							if (0 == circletask2.Value.NpcAttachedTaskID.Count)
							{
								continue;
							}
							if (-1 != circletask2.Value.NpcAttachedTaskID.IndexOf(circletask2.Value.oldTaskID))
							{
								if (null == npcData.NewTaskIDs)
								{
									npcData.NewTaskIDs = new List<int>();
								}
								npcData.NewTaskIDs.Add(circletask2.Value.oldTaskID);
								if (null == npcData.NewTaskIDsDoneCount)
								{
									npcData.NewTaskIDsDoneCount = new List<int>();
								}
								npcData.NewTaskIDsDoneCount.Add(0);
							}
							else
							{
								needRandom = true;
							}
						}
						else
						{
							needRandom = true;
						}
						if (needRandom)
						{
							int randTaskId = circletask2.Value.DoRandomTaskID(client);
							if (-1 != randTaskId)
							{
								if (null == npcData.NewTaskIDs)
								{
									npcData.NewTaskIDs = new List<int>();
								}
								npcData.NewTaskIDs.Add(randTaskId);
								if (null == npcData.NewTaskIDsDoneCount)
								{
									npcData.NewTaskIDsDoneCount = new List<int>();
								}
								npcData.NewTaskIDsDoneCount.Add(0);
								PaoHuanTasksMgr.SetPaoHuanHistTaskID(client.ClientData.RoleID, circletask2.Value.taskclass, randTaskId);
							}
						}
					}
					result = true;
				}
			}
			return result;
		}

		
		public static bool GetNpcFunctionData(GameClient client, int extensionID, NPCData npcData, SystemXmlItem systemNPC)
		{
			bool result;
			if (null == systemNPC)
			{
				result = false;
			}
			else
			{
				string operaIDsByString = systemNPC.GetStringValue("Operations");
				operaIDsByString.Trim();
				if (operaIDsByString != "")
				{
					int[] operaIDsByInt = Global.StringArray2IntArray(operaIDsByString.Split(new char[]
					{
						','
					}));
					if (null == npcData.OperationIDs)
					{
						npcData.OperationIDs = new List<int>();
					}
					for (int i = 0; i < operaIDsByInt.Length; i++)
					{
						if (!Global.FilterNPCOperationByID(client, operaIDsByInt[i], extensionID))
						{
							npcData.OperationIDs.Add(operaIDsByInt[i]);
						}
					}
				}
				string scriptIDsByString = systemNPC.GetStringValue("Scripts");
				if (null != scriptIDsByString)
				{
					scriptIDsByString = scriptIDsByString.Trim();
				}
				if (!string.IsNullOrEmpty(scriptIDsByString))
				{
					int[] scriptIDsByInt = Global.StringArray2IntArray(scriptIDsByString.Split(new char[]
					{
						','
					}));
					if (null == npcData.ScriptIDs)
					{
						npcData.ScriptIDs = new List<int>();
					}
					for (int i = 0; i < scriptIDsByInt.Length; i++)
					{
						int errorCode = 0;
						if (!Global.FilterNPCScriptByID(client, scriptIDsByInt[i], out errorCode))
						{
							npcData.ScriptIDs.Add(scriptIDsByInt[i]);
						}
					}
				}
				result = true;
			}
			return result;
		}

		
		public static TCPClient PopGameDbClient(int serverId, int poolId)
		{
			TCPClient result;
			if (serverId <= 0 || serverId == GameManager.ServerId || serverId == GameManager.KuaFuServerId)
			{
				if (poolId == 0)
				{
					result = Global._TCPManager.tcpClientPool.Pop();
				}
				else
				{
					result = Global._TCPManager.tcpLogClientPool.Pop();
				}
			}
			else
			{
				result = KuaFuManager.getInstance().PopGameDbClient(serverId, poolId);
			}
			return result;
		}

		
		public static void PushGameDbClient(int serverId, TCPClient tcpClient, int poolId)
		{
			if (serverId <= 0 || serverId == GameManager.ServerId || serverId == GameManager.KuaFuServerId)
			{
				if (poolId == 0)
				{
					Global._TCPManager.tcpClientPool.Push(tcpClient);
				}
				else
				{
					Global._TCPManager.tcpLogClientPool.Push(tcpClient);
				}
			}
			else
			{
				KuaFuManager.getInstance().PushGameDbClient(serverId, tcpClient, poolId);
			}
		}

		
		public static void UpdateKuaFuRoleDayLogData(int serverId, int roleId, DateTime now, int zoneId, int signUpCount, int startGameCount, int successCount, int faildCount, int gameType)
		{
			Global.SendToDB<RoleKuaFuDayLogData>(20003, new RoleKuaFuDayLogData
			{
				RoleID = roleId,
				Day = now.Date.ToString("yyyy-MM-dd"),
				ZoneId = zoneId,
				SignupCount = signUpCount,
				StartGameCount = startGameCount,
				SuccessCount = successCount,
				FaildCount = faildCount,
				GameType = gameType
			}, serverId);
		}

		
		public static void RecordSwitchKuaFuServerLog(GameClient client)
		{
			ushort LastMapCode = 0;
			ushort LastPosX = 0;
			ushort LastPosY = 0;
			if (SceneUIClasses.Normal == Global.GetMapSceneType(client.ClientData.MapCode))
			{
				LastMapCode = (ushort)client.CurrentMapCode;
				LastPosX = (ushort)client.CurrentGrid.X;
				LastPosY = (ushort)client.CurrentGrid.Y;
			}
			Global.ModifyMapRecordData(client, LastMapCode, LastPosX, LastPosY, 0);
			KuaFuServerLoginData kuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
			LogManager.WriteLog(LogTypes.Error, string.Format("GameType={5},RoleId={0},GameId={1},SrcServerId={2},KfIp={3},KfPort={4}", new object[]
			{
				kuaFuServerLoginData.RoleId,
				kuaFuServerLoginData.GameId,
				kuaFuServerLoginData.ServerId,
				kuaFuServerLoginData.ServerIp,
				kuaFuServerLoginData.ServerPort,
				kuaFuServerLoginData.GameType
			}), null, true);
			EventLogManager.AddGameEvent(LogRecordType.KuaFu, new object[]
			{
				kuaFuServerLoginData.GameType,
				kuaFuServerLoginData.RoleId,
				client.ClientData.Faction,
				client.ClientData.JunTuanId,
				kuaFuServerLoginData.GameId
			});
		}

		
		public static List<int[]> FindPath(Point startPoint, Point endPoint, int mapCode)
		{
			GameMap gameMap = GameManager.MapMgr.DictMaps[mapCode];
			List<int[]> result;
			if (null == gameMap)
			{
				result = null;
			}
			else
			{
				PathFinderFast pathFinderFast;
				if (GlobalNew._pathStack.Count <= 0)
				{
					pathFinderFast = new PathFinderFast(gameMap.MyNodeGrid.GetFixedObstruction())
					{
						Formula = HeuristicFormula.Manhattan,
						Diagonals = true,
						HeuristicEstimate = 2,
						ReopenCloseNodes = true,
						SearchLimit = int.MaxValue,
						Punish = null,
						MaxNum = Global.GMax(gameMap.MapGridWidth, gameMap.MapGridHeight)
					};
				}
				else
				{
					pathFinderFast = GlobalNew._pathStack.Pop();
				}
				startPoint.X = (double)(gameMap.CorrectWidthPointToGridPoint((int)startPoint.X) / gameMap.MapGridWidth);
				startPoint.Y = (double)(gameMap.CorrectHeightPointToGridPoint((int)startPoint.Y) / gameMap.MapGridHeight);
				endPoint.X = (double)(gameMap.CorrectWidthPointToGridPoint((int)endPoint.X) / gameMap.MapGridWidth);
				endPoint.Y = (double)(gameMap.CorrectHeightPointToGridPoint((int)endPoint.Y) / gameMap.MapGridHeight);
				pathFinderFast.EnablePunish = false;
				List<PathFinderNode> nodeList = pathFinderFast.FindPath(startPoint, endPoint);
				if (nodeList == null || nodeList.Count <= 0)
				{
					result = null;
				}
				else
				{
					List<int[]> path = new List<int[]>();
					for (int i = 0; i < nodeList.Count; i++)
					{
						path.Add(new int[]
						{
							nodeList[i].X,
							nodeList[i].Y
						});
					}
					result = path;
				}
			}
			return result;
		}

		
		public static bool Copy<T>(T sData, ref T rData)
		{
			try
			{
				foreach (FieldInfo info in rData.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					info.SetValue(rData, info.GetValue(sData));
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, ex.ToString(), null, true);
			}
			return false;
		}

		
		public static GoodsData ParseGoodsData(string fields)
		{
			try
			{
				string[] sa = fields.Split(new char[]
				{
					','
				});
				if (sa.Length != 7)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("解析 ParseGoodsData 配置项错误 fields={0}", fields), null, true);
					return null;
				}
				int[] goodsFields = Global.StringArray2IntArray(sa);
				return Global.GetNewGoodsData(goodsFields[0], goodsFields[1], 0, goodsFields[3], goodsFields[2], 0, goodsFields[5], 0, goodsFields[6], goodsFields[4], 0);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, ex.ToString(), null, true);
			}
			return null;
		}

		
		private static Stack<PathFinderFast> _pathStack = new Stack<PathFinderFast>();

		
		public class NpcCircleTaskData
		{
			
			public int DoRandomTaskID(GameClient client)
			{
				int result;
				if (0 == this.NpcAttachedTaskID.Count)
				{
					result = -1;
				}
				else if (this.taskclass == 8)
				{
					result = Global.GetDailyCircleTaskIDBaseChangeLifeLev(client);
				}
				else if (this.taskclass == 9)
				{
					result = Global.GetTaofaTaskIDBaseChangeLifeLev(client);
				}
				else
				{
					int randIndex = Global.GetRandomNumber(0, this.NpcAttachedTaskID.Count);
					result = this.NpcAttachedTaskID[randIndex];
				}
				return result;
			}

			
			public int taskclass = 0;

			
			public int oldTaskID = 0;

			
			public List<int> NpcAttachedTaskID = new List<int>();
		}
	}
}
