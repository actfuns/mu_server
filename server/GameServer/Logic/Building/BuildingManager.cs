using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.Building
{
	
	public class BuildingManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		
		public static BuildingManager getInstance()
		{
			return BuildingManager.instance;
		}

		
		public bool initialize()
		{
			return this.InitConfig();
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1550, 1, 1, BuildingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1551, 3, 3, BuildingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1552, 2, 2, BuildingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1553, 2, 2, BuildingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1554, 2, 2, BuildingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1555, 2, 2, BuildingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1556, 1, 1, BuildingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1557, 1, 1, BuildingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1558, 1, 1, BuildingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1559, 1, 1, BuildingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			return true;
		}

		
		public bool showdown()
		{
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.Building, false))
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1550:
					result = this.ProcessBuildGetListCmd(client, nID, bytes, cmdParams);
					break;
				case 1551:
					result = this.ProcessBuildExcuteCmd(client, nID, bytes, cmdParams);
					break;
				case 1552:
					result = this.ProcessBuildFinishCmd(client, nID, bytes, cmdParams);
					break;
				case 1553:
					result = this.ProcessBuildRefreshCmd(client, nID, bytes, cmdParams);
					break;
				case 1554:
					result = this.ProcessBuildGetAllLevelAwardCmd(client, nID, bytes, cmdParams);
					break;
				case 1555:
					result = this.ProcessBuildGetAwardCmd(client, nID, bytes, cmdParams);
					break;
				case 1556:
					result = this.ProcessBuildOpenQueueCmd(client, nID, bytes, cmdParams);
					break;
				case 1557:
					result = this.ProcessBuildGetQueueCmd(client, nID, bytes, cmdParams);
					break;
				case 1558:
					result = this.ProcessBuildGetStateCmd(client, nID, bytes, cmdParams);
					break;
				case 1559:
					result = this.ProcessBuildGetAllLevelAwardStateCmd(client, nID, bytes, cmdParams);
					break;
				default:
					result = true;
					break;
				}
			}
			return result;
		}

		
		public BuildingData GetBuildingData(GameClient client, int BuildID)
		{
			BuildingData BuildData = null;
			for (int i = 0; i < client.ClientData.BuildingDataList.Count; i++)
			{
				if (client.ClientData.BuildingDataList[i].BuildId == BuildID)
				{
					BuildData = client.ClientData.BuildingDataList[i];
					break;
				}
			}
			return BuildData;
		}

		
		public void OnRoleLogin(GameClient client)
		{
			try
			{
				if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot8))
				{
					if (null != client.ClientData.BuildingDataList)
					{
						if (client.ClientData.BuildingDataList.Count == 0)
						{
							this.GeneralBuildingData(client);
						}
						else if (client.ClientData.BuildingDataList.Count < this.BuildDict.Count)
						{
							this.GeneralBuildingData(client);
						}
						else if (this.GetOpenPayTeamNum(client) == 0 && client.ClientData.BuildingDataList[0].TaskID_4 == 0)
						{
							this.GeneralBuildingData(client);
						}
						if (GlobalNew.IsGongNengOpened(client, GongNengIDs.Building, false))
						{
							this.BuildingDataChecking(client);
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
		}

		
		public void RandomBuildTaskData(GameClient client, int BuildID, BuildingData myBuildData, bool ConstRefresh = false)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot8))
			{
				this.BuildTaskData(client, BuildID, myBuildData, ConstRefresh);
			}
		}

		
		public void BuildTaskData(GameClient client, int BuildID, BuildingData myBuildData, bool ConstRefresh = false)
		{
			BuildingConfigData myBCData = null;
			this.BuildDict.TryGetValue(BuildID, out myBCData);
			if (null != myBCData)
			{
				myBuildData.BuildId = BuildID;
				myBuildData.BuildTime = BuildingManager.ConstBuildTime;
				lock (this.RandomTaskMutex)
				{
					myBuildData.TaskID_1 = this.BuildTask(myBuildData.BuildId, BuildingQuality.White);
					myBuildData.TaskID_2 = this.BuildTask(myBuildData.BuildId, BuildingQuality.Green);
					myBuildData.TaskID_3 = this.BuildTask(myBuildData.BuildId, BuildingQuality.Blue);
					myBuildData.TaskID_4 = this.BuildTask(myBuildData.BuildId, BuildingQuality.Purple);
				}
			}
		}

		
		public void ResetRandSkipVavle()
		{
			foreach (KeyValuePair<int, BuildingTaskConfigData> kvp in this.BuildTaskDict)
			{
				if (kvp.Value.RandSkip)
				{
					kvp.Value.RandSkip = false;
				}
			}
		}

		
		public int RandomBuildTask(int BuildID, BuildingQuality quality)
		{
			List<BuildingTaskConfigData> listBuildTask = new List<BuildingTaskConfigData>();
			foreach (KeyValuePair<int, BuildingTaskConfigData> kvp in this.BuildTaskDict)
			{
				if (!kvp.Value.RandSkip && kvp.Value.BuildID == BuildID && kvp.Value.quality == quality)
				{
					listBuildTask.Add(kvp.Value);
				}
			}
			int rate = Global.GetRandomNumber(0, listBuildTask.Count);
			int result;
			if (listBuildTask.Count != 0)
			{
				listBuildTask[rate].RandSkip = true;
				result = listBuildTask[rate].TaskID;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		
		public int BuildTask(int BuildID, BuildingQuality quality)
		{
			List<BuildingTaskConfigData> listBuildTask = new List<BuildingTaskConfigData>();
			foreach (KeyValuePair<int, BuildingTaskConfigData> kvp in this.NewBuildTaskDict)
			{
				if (kvp.Value.BuildID == BuildID && kvp.Value.quality == quality)
				{
					return kvp.Value.TaskID;
				}
			}
			return 0;
		}

		
		public BuildingQuality RandomQualityByList(List<BuildingRandomData> RandomList)
		{
			double rateEnd = 0.0;
			double rate = (double)Global.GetRandomNumber(1, 101) / 100.0;
			for (int i = 0; i < RandomList.Count; i++)
			{
				rateEnd += RandomList[i].rate;
				if (rate <= rateEnd)
				{
					return RandomList[i].quality;
				}
			}
			return BuildingQuality.Null;
		}

		
		public void UpdateBuildingLogDB(GameClient client, BuildingLogType BuildLogType)
		{
			EventLogManager.AddRoleEvent(client, OpTypes.Trace, OpTags.Building, LogRecordType.IntValue, new object[]
			{
				(int)BuildLogType
			});
		}

		
		public void UpdateBuildingDataDB(GameClient client, BuildingData myBuildData)
		{
			if (null != myBuildData)
			{
				string buildtime = null;
				if (!string.IsNullOrEmpty(myBuildData.BuildTime))
				{
					buildtime = myBuildData.BuildTime.Replace(':', '$');
				}
				string cmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
				{
					client.ClientData.RoleID,
					myBuildData.BuildId,
					myBuildData.BuildLev,
					myBuildData.BuildExp,
					buildtime,
					myBuildData.TaskID_1,
					myBuildData.TaskID_2,
					myBuildData.TaskID_3,
					myBuildData.TaskID_4
				});
				Global.ExecuteDBCmd(13300, cmd, client.ServerId);
			}
		}

		
		public void BuildingDataChecking(GameClient client)
		{
			try
			{
				List<BuildTeam> BuildQueue = this.GetBuildingQueueData(client);
				List<BuildTeam> BuildQueueDel = new List<BuildTeam>();
				List<BuildTeam> BuildQueueAdd = new List<BuildTeam>();
				client.ClientData.NengLiangSmall = Global.GetRoleParamsInt32FromDB(client, "10168");
				client.ClientData.NengLiangMedium = Global.GetRoleParamsInt32FromDB(client, "10169");
				client.ClientData.NengLiangBig = Global.GetRoleParamsInt32FromDB(client, "10170");
				client.ClientData.NengLiangSuper = Global.GetRoleParamsInt32FromDB(client, "10171");
				int buildCount = 4;
				for (int i = 0; i < buildCount; i++)
				{
					BuildingData BuildData = client.ClientData.BuildingDataList[i];
					if (BuildData.TaskID_1 == 0 || BuildData.TaskID_2 == 0 || BuildData.TaskID_3 == 0 || (BuildData.TaskID_4 == 0 && BuildData.TaskID_3 >= 10000))
					{
						this.RandomBuildTaskData(client, BuildData.BuildId, BuildData, false);
						this.UpdateBuildingDataDB(client, BuildData);
					}
					if (0 == string.Compare(BuildData.BuildTime, BuildingManager.ConstBuildTime))
					{
						BuildTeam BuildTeamData = BuildQueue.Find((BuildTeam _da) => _da.BuildID == BuildData.BuildId);
						if (null != BuildTeamData)
						{
							BuildQueueDel.Add(BuildTeamData);
						}
					}
					else
					{
						BuildTeam BuildTeamData = BuildQueue.Find((BuildTeam _da) => _da.BuildID == BuildData.BuildId);
						if (null == BuildTeamData)
						{
							BuildQueueAdd.Add(new BuildTeam
							{
								BuildID = BuildData.BuildId,
								TaskID = BuildData.TaskID_1
							});
						}
					}
				}
				foreach (BuildTeam dat in BuildQueueDel)
				{
					this.RemoveBuildingQueueData(client, dat.BuildID, dat.TaskID);
					LogManager.WriteLog(LogTypes.Data, string.Format("领地数据检查RemoveBuildingQueueData, RoleID={0}, RoleName={1}, BuildID={2}, TaskID={3}", new object[]
					{
						client.ClientData.RoleID,
						client.ClientData.RoleName,
						dat.BuildID,
						dat.TaskID
					}), null, true);
				}
				foreach (BuildTeam dat in BuildQueueAdd)
				{
					if (this.AddBuildingQueueData(client, dat.BuildID, dat.TaskID))
					{
						LogManager.WriteLog(LogTypes.Data, string.Format("领地数据检查AddBuildingQueueData, RoleID={0}, RoleName={1}, BuildID={2}, TaskID={3}", new object[]
						{
							client.ClientData.RoleID,
							client.ClientData.RoleName,
							dat.BuildID,
							dat.TaskID
						}), null, true);
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
		}

		
		public void GeneralBuildingData(GameClient client)
		{
			foreach (KeyValuePair<int, BuildingConfigData> kvp in this.BuildDict)
			{
				if (null == this.GetBuildingData(client, kvp.Value.BuildID))
				{
					BuildingData myBuildData = new BuildingData();
					this.RandomBuildTaskData(client, kvp.Value.BuildID, myBuildData, false);
					this.UpdateBuildingDataDB(client, myBuildData);
					if (null != myBuildData)
					{
						client.ClientData.BuildingDataList.Add(myBuildData);
					}
				}
			}
		}

		
		public List<BuildTeam> GetBuildingQueueData(GameClient client)
		{
			List<BuildTeam> BuildQueue = new List<BuildTeam>();
			string strKey = "BuildQueueData";
			string BuildingQueueData = Global.GetRoleParamByName(client, strKey);
			if (!string.IsNullOrEmpty(BuildingQueueData))
			{
				string[] Filed = BuildingQueueData.Split(new char[]
				{
					','
				});
				if (1 >= Filed.Length)
				{
					return BuildQueue;
				}
				for (int i = 1; i < Filed.Length; i++)
				{
					string[] TypeVsBuilID = Filed[i].Split(new char[]
					{
						'|'
					});
					if (TypeVsBuilID.Length == 3)
					{
						BuildQueue.Add(new BuildTeam
						{
							_TeamType = (BuildTeamType)Convert.ToInt32(TypeVsBuilID[0]),
							BuildID = Convert.ToInt32(TypeVsBuilID[1]),
							TaskID = Convert.ToInt32(TypeVsBuilID[2])
						});
					}
				}
			}
			return BuildQueue;
		}

		
		public BuildingState GetBuildState(GameClient client, int BuildID, int TaskID)
		{
			BuildingTaskConfigData TaskConfigData;
			this.BuildTaskDict.TryGetValue(TaskID, out TaskConfigData);
			if (null == TaskConfigData)
			{
				this.NewBuildTaskDict.TryGetValue(TaskID, out TaskConfigData);
				if (null == TaskConfigData)
				{
					return BuildingState.EBS_Null;
				}
			}
			int i = 0;
			while (i < client.ClientData.BuildingDataList.Count)
			{
				BuildingData BuildData = client.ClientData.BuildingDataList[i];
				if (BuildData.BuildTime != BuildingManager.ConstBuildTime && BuildID == BuildData.BuildId)
				{
					DateTime BuildTime;
					DateTime.TryParse(BuildData.BuildTime, out BuildTime);
					long SpendTicks = TimeUtil.NowDateTime().Ticks / 10000L - BuildTime.Ticks / 10000L;
					long SpendSecondes = SpendTicks / 1000L;
					long SubTimeSecondes = (long)(TaskConfigData.Time * 60) - SpendSecondes;
					if (SubTimeSecondes <= 0L)
					{
						return BuildingState.EBS_Finish;
					}
					return BuildingState.EBS_InBuilding;
				}
				else
				{
					i++;
				}
			}
			return BuildingState.EBS_Null;
		}

		
		public void GetTaskNumInEachTeam(GameClient client, out int free, out int pay)
		{
			free = 0;
			pay = 0;
			List<BuildTeam> BuildQueue = this.GetBuildingQueueData(client);
			for (int i = 0; i < BuildQueue.Count; i++)
			{
				if (BuildTeamType.FreeTeam == BuildQueue[i]._TeamType)
				{
					free++;
				}
				else if (BuildTeamType.PayTeam == BuildQueue[i]._TeamType)
				{
					pay++;
				}
			}
		}

		
		public int GetOpenPayTeamNum(GameClient client)
		{
			string strKey = "BuildQueueData";
			string BuildingQueueData = Global.GetRoleParamByName(client, strKey);
			int result;
			if (!string.IsNullOrEmpty(BuildingQueueData))
			{
				string[] Filed = BuildingQueueData.Split(new char[]
				{
					','
				});
				if (0 == Filed.Length)
				{
					result = 0;
				}
				else
				{
					result = Convert.ToInt32(Filed[0]);
				}
			}
			else
			{
				result = 0;
			}
			return result;
		}

		
		public void SaveBuildingQueueData(GameClient client, List<BuildTeam> BuildQueue)
		{
			string BuildingQueueData = "";
			BuildingQueueData += this.GetOpenPayTeamNum(client);
			for (int i = 0; i < BuildQueue.Count; i++)
			{
				if (BuildQueue[i].BuildID != 0)
				{
					BuildingQueueData += ',';
					BuildingQueueData += (int)BuildQueue[i]._TeamType;
					BuildingQueueData += '|';
					BuildingQueueData += BuildQueue[i].BuildID;
					BuildingQueueData += '|';
					BuildingQueueData += BuildQueue[i].TaskID;
				}
			}
			string strKey = "BuildQueueData";
			Global.SaveRoleParamsStringToDB(client, strKey, BuildingQueueData, true);
		}

		
		public void ModifyOpenPayNum(GameClient client, int chg)
		{
			string strKey = "BuildQueueData";
			string BuildingQueueData = Global.GetRoleParamByName(client, strKey);
			if (string.IsNullOrEmpty(BuildingQueueData))
			{
				BuildingQueueData = "0";
			}
			string[] Filed = BuildingQueueData.Split(new char[]
			{
				','
			});
			if (0 != Filed.Length)
			{
				int OpenPayNum = Convert.ToInt32(Filed[0]) + chg;
				if (OpenPayNum < 0)
				{
					OpenPayNum = 0;
				}
				BuildingQueueData = Convert.ToString(OpenPayNum);
				for (int i = 1; i < Filed.Length; i++)
				{
					BuildingQueueData += ',';
					BuildingQueueData += Filed[i];
				}
				Global.SaveRoleParamsStringToDB(client, strKey, BuildingQueueData, true);
			}
		}

		
		public bool RemoveBuildingQueueData(GameClient client, int BuildID, int TaskID)
		{
			List<BuildTeam> BuildQueue = this.GetBuildingQueueData(client);
			int RemoveIndex = -1;
			for (int i = 0; i < BuildQueue.Count; i++)
			{
				if (BuildID == BuildQueue[i].BuildID && TaskID == BuildQueue[i].TaskID)
				{
					RemoveIndex = i;
					BuildQueue[i].BuildID = 0;
					break;
				}
			}
			bool result;
			if (-1 == RemoveIndex)
			{
				result = false;
			}
			else
			{
				if (BuildTeamType.PayTeam == BuildQueue[RemoveIndex]._TeamType)
				{
					this.ModifyOpenPayNum(client, -1);
				}
				this.SaveBuildingQueueData(client, BuildQueue);
				result = true;
			}
			return result;
		}

		
		public BuildTeamType GetBuildTaskQueueType(GameClient client, int BuildID, int TaskID)
		{
			BuildTeamType TeamType = BuildTeamType.NullTeam;
			List<BuildTeam> BuildQueue = this.GetBuildingQueueData(client);
			for (int i = 0; i < BuildQueue.Count; i++)
			{
				if (BuildQueue[i].BuildID == BuildID && BuildQueue[i].TaskID == TaskID)
				{
					TeamType = BuildQueue[i]._TeamType;
					break;
				}
			}
			return TeamType;
		}

		
		public bool AddBuildingQueueData(GameClient client, int BuildID, int TaskID)
		{
			return this.AddBuildingTaskData(client, BuildID, TaskID);
		}

		
		public bool AddBuildingTaskData(GameClient client, int BuildID, int TaskID)
		{
			List<BuildTeam> BuildQueue = this.GetBuildingQueueData(client);
			int index = BuildQueue.FindIndex((BuildTeam x) => x.BuildID == BuildID);
			bool result;
			if (index >= 0)
			{
				result = false;
			}
			else
			{
				BuildTeam TeamData = new BuildTeam
				{
					BuildID = BuildID,
					TaskID = TaskID
				};
				BuildQueue.Add(TeamData);
				this.SaveBuildingQueueData(client, BuildQueue);
				result = true;
			}
			return result;
		}

		
		public bool CheckAnyTaskFinish(GameClient client)
		{
			bool Finished = false;
			List<BuildTeam> BuildQueue = this.GetBuildingQueueData(client);
			for (int i = 0; i < client.ClientData.BuildingDataList.Count; i++)
			{
				int BuildID = client.ClientData.BuildingDataList[i].BuildId;
				if (!(client.ClientData.BuildingDataList[i].BuildTime == BuildingManager.ConstBuildTime))
				{
					BuildingConfigData BConfigData;
					this.BuildDict.TryGetValue(BuildID, out BConfigData);
					if (null != BConfigData)
					{
						int taskID = 0;
						for (int qloop = 0; qloop < BuildQueue.Count; qloop++)
						{
							if (BuildQueue[qloop].BuildID == BuildID)
							{
								taskID = BuildQueue[qloop].TaskID;
								break;
							}
						}
						BuildingTaskConfigData TaskConfigData;
						this.BuildTaskDict.TryGetValue(taskID, out TaskConfigData);
						if (null != TaskConfigData)
						{
							BuildingState state = this.GetBuildState(client, BuildID, taskID);
							if (BuildingState.EBS_Finish == state)
							{
								Finished = true;
								break;
							}
						}
					}
				}
			}
			return Finished;
		}

		
		public bool CheckCanGetAnyAllLevelAward(GameClient client)
		{
			bool CanGetAny = false;
			HashSet<int> AwardedSet = new HashSet<int>();
			string strKey = "BuildAllLevAward";
			string BuildAllLevAwardData = Global.GetRoleParamByName(client, strKey);
			if (!string.IsNullOrEmpty(BuildAllLevAwardData))
			{
				string[] Filed = BuildAllLevAwardData.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < Filed.Length; i++)
				{
					AwardedSet.Add(Global.SafeConvertToInt32(Filed[i]));
				}
			}
			int BuildAllLevel = 0;
			for (int i = 0; i < client.ClientData.BuildingDataList.Count; i++)
			{
				BuildAllLevel += client.ClientData.BuildingDataList[i].BuildLev;
			}
			foreach (KeyValuePair<int, BuildingLevelAwardConfigData> kvp in this.BuildLevelAwardDict)
			{
				if (!AwardedSet.Contains(kvp.Key))
				{
					if (BuildAllLevel >= kvp.Value.AllLevel)
					{
						CanGetAny = true;
						break;
					}
				}
			}
			return CanGetAny;
		}

		
		public void BuildTaskFinish(GameClient client, BuildingData BuildData, BuildingConfigData BConfigData, BuildingTaskConfigData TaskConfigData)
		{
			BuildData.BuildTime = BuildingManager.ConstBuildTime;
			BuildingLevelConfigData myBuildLevel;
			this.BuildLevelDict.TryGetValue(new KeyValuePair<int, int>(BuildData.BuildId, BuildData.BuildLev), out myBuildLevel);
			if (null != myBuildLevel)
			{
				int ExpAdd = (int)(TaskConfigData.ExpNum * myBuildLevel.Exp * (double)TaskConfigData.Time);
				BuildData.BuildExp += ExpAdd;
				BuildingLevelConfigData myBuildLevelUP = myBuildLevel;
				while (BuildData.BuildLev != BConfigData.MaxLevel)
				{
					if (myBuildLevelUP == null || BuildData.BuildExp < myBuildLevelUP.UpNeedExp)
					{
						break;
					}
					BuildData.BuildExp -= myBuildLevelUP.UpNeedExp;
					BuildData.BuildLev++;
					this.BuildLevelDict.TryGetValue(new KeyValuePair<int, int>(BuildData.BuildId, BuildData.BuildLev), out myBuildLevelUP);
				}
				if (BuildData.BuildLev == BConfigData.MaxLevel)
				{
					BuildData.BuildExp = 0;
				}
				double rate = (TaskConfigData.SumNum - TaskConfigData.ExpNum) * (double)TaskConfigData.Time;
				this.RandomBuildTaskData(client, BuildData.BuildId, BuildData, false);
				if (myBuildLevel.Money > 0.0)
				{
					GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, (int)(myBuildLevel.Money * rate), "建造任务完成", true);
				}
				if (myBuildLevel.MoJing > 0.0)
				{
					GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, (int)(myBuildLevel.MoJing * rate), "建造任务完成", false, true, false);
				}
				if (myBuildLevel.XingHun > 0.0)
				{
					GameManager.ClientMgr.ModifyStarSoulValue(client, (int)(myBuildLevel.XingHun * rate), "建造任务完成", true, true);
				}
				if (myBuildLevel.ChengJiu > 0.0)
				{
					GameManager.ClientMgr.ModifyChengJiuPointsValue(client, (int)(myBuildLevel.ChengJiu * rate), "建造任务完成", false, true);
				}
				if (myBuildLevel.ShengWang > 0.0)
				{
					GameManager.ClientMgr.ModifyShengWangValue(client, (int)(myBuildLevel.ShengWang * rate), "建造任务完成", false, true);
				}
				if (myBuildLevel.YuanSu > 0.0)
				{
					GameManager.ClientMgr.ModifyYuanSuFenMoValue(client, (int)(myBuildLevel.YuanSu * rate), "建造任务完成", true, false);
				}
				if (myBuildLevel.YingGuang > 0.0)
				{
					GameManager.FluorescentGemMgr.AddFluorescentPoint(client, (int)(myBuildLevel.YingGuang * rate), "建造任务完成", true);
				}
			}
		}

		
		public void BuildingLevelUp_GM(GameClient client, int buildID)
		{
			BuildingData BuildData = null;
			for (int i = 0; i < client.ClientData.BuildingDataList.Count; i++)
			{
				if (client.ClientData.BuildingDataList[i].BuildId == buildID)
				{
					BuildData = client.ClientData.BuildingDataList[i];
					break;
				}
			}
			if (null != BuildData)
			{
				BuildingConfigData BConfigData;
				this.BuildDict.TryGetValue(BuildData.BuildId, out BConfigData);
				if (null != BConfigData)
				{
					if (BuildData.BuildLev != BConfigData.MaxLevel)
					{
						BuildData.BuildLev++;
						this.UpdateBuildingDataDB(client, BuildData);
						byte[] bytesData = DataHelper.ObjectToBytes<BuildingData>(BuildData);
						GameManager.ClientMgr.SendToClient(client, bytesData, 1560);
					}
				}
			}
		}

		
		public bool ProcessBuildGetListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				for (int i = 0; i < client.ClientData.BuildingDataList.Count; i++)
				{
					BuildingData BuildData = client.ClientData.BuildingDataList[i];
					if (BuildData.BuildTime == BuildingManager.ConstBuildTime)
					{
						this.RandomBuildTaskData(client, BuildData.BuildId, BuildData, true);
						this.UpdateBuildingDataDB(client, BuildData);
					}
				}
				byte[] bytesData = DataHelper.ObjectToBytes<List<BuildingData>>(client.ClientData.BuildingDataList);
				GameManager.ClientMgr.SendToClient(client, bytesData, nID);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessBuildExcuteCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot8))
				{
					return false;
				}
				int result = 0;
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				int buildID = Global.SafeConvertToInt32(cmdParams[1]);
				int taskID = Global.SafeConvertToInt32(cmdParams[2]);
				BuildingData BuildData = null;
				for (int i = 0; i < client.ClientData.BuildingDataList.Count; i++)
				{
					if (client.ClientData.BuildingDataList[i].BuildId == buildID)
					{
						BuildData = client.ClientData.BuildingDataList[i];
						break;
					}
				}
				string strcmd;
				if (null == BuildData)
				{
					result = 12;
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						result,
						roleID,
						buildID,
						taskID,
						-1
					});
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				if (BuildData.TaskID_1 != taskID && BuildData.TaskID_2 != taskID && BuildData.TaskID_3 != taskID && BuildData.TaskID_4 != taskID)
				{
					result = 13;
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						result,
						roleID,
						buildID,
						taskID,
						-1
					});
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				if (BuildData.BuildTime != BuildingManager.ConstBuildTime)
				{
					result = 6;
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						result,
						roleID,
						buildID,
						taskID,
						-1
					});
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				int nengLiangType = 0;
				if (BuildData.TaskID_1 == taskID)
				{
					if (client.ClientData.NengLiangSmall < 1)
					{
						result = 15;
					}
					nengLiangType = 1;
				}
				else if (BuildData.TaskID_2 == taskID)
				{
					if (client.ClientData.NengLiangMedium < 1)
					{
						result = 15;
					}
					nengLiangType = 2;
				}
				else if (BuildData.TaskID_3 == taskID)
				{
					if (client.ClientData.NengLiangBig < 1)
					{
						result = 15;
					}
					nengLiangType = 3;
				}
				else if (BuildData.TaskID_4 == taskID)
				{
					if (client.ClientData.NengLiangSuper < 1)
					{
						result = 15;
					}
					nengLiangType = 4;
				}
				if (result == 15)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						result,
						roleID,
						buildID,
						taskID,
						-1
					});
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				if (!this.AddBuildingQueueData(client, buildID, taskID))
				{
					result = 5;
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						result,
						roleID,
						buildID,
						taskID,
						-1
					});
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				BuildTeamType TeamType = BuildTeamType.FreeTeam;
				this.ModifyNengLiangPointsValue(client, nengLiangType, -1, "领地升级启动", true, true);
				BuildData.BuildTime = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
				this.UpdateBuildingDataDB(client, BuildData);
				this.UpdateBuildingLogDB(client, BuildingLogType.BuildLog_Task);
				this.UpdateBuildingLogDB(client, BuildingLogType.BuildLog_TaskRole);
				if (client._IconStateMgr.CheckBuildingIcon(client, false))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
				strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					result,
					roleID,
					buildID,
					taskID,
					TeamType
				});
				client.sendCmd(nID, strcmd, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessBuildFinishCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot8))
				{
					return false;
				}
				return false;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessBuildRefreshCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot8))
				{
					return false;
				}
				return false;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessBuildGetAllLevelAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot8))
				{
					return false;
				}
				int result = 0;
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				int awardID = Global.SafeConvertToInt32(cmdParams[1]);
				BuildingLevelAwardConfigData myAwardData = null;
				this.BuildLevelAwardDict.TryGetValue(awardID, out myAwardData);
				string strcmd;
				if (null == myAwardData)
				{
					result = 2;
					strcmd = string.Format("{0}:{1}:{2}", result, roleID, awardID);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				string strKey = "BuildAllLevAward";
				string BuildAllLevAwardData = Global.GetRoleParamByName(client, strKey);
				if (!string.IsNullOrEmpty(BuildAllLevAwardData))
				{
					string[] Filed = BuildAllLevAwardData.Split(new char[]
					{
						'|'
					});
					for (int i = 0; i < Filed.Length; i++)
					{
						if (awardID == Global.SafeConvertToInt32(Filed[i]))
						{
							result = 1;
							strcmd = string.Format("{0}:{1}:{2}", result, roleID, awardID);
							client.sendCmd(nID, strcmd, false);
							return true;
						}
					}
				}
				int BuildAllLevel = 0;
				for (int i = 0; i < client.ClientData.BuildingDataList.Count; i++)
				{
					BuildAllLevel += client.ClientData.BuildingDataList[i].BuildLev;
				}
				if (BuildAllLevel < myAwardData.AllLevel)
				{
					result = 2;
					strcmd = string.Format("{0}:{1}:{2}", result, roleID, awardID);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				List<GoodsData> goodsDataList = Global.ConvertToGoodsDataList(myAwardData.GoodsList.Items, -1);
				if (!Global.CanAddGoodsDataList(client, goodsDataList))
				{
					result = 14;
					strcmd = string.Format("{0}:{1}:{2}", result, roleID, awardID);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				for (int j = 0; j < goodsDataList.Count; j++)
				{
					GoodsData goodsData = goodsDataList[j];
					if (null != goodsData)
					{
						goodsData.Id = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, goodsData.GCount, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 0, goodsData.Jewellist, true, 1, "获得领地总等级奖励", goodsData.Endtime, goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, 0, 0, 0, null, null, 0, true);
					}
				}
				if (string.IsNullOrEmpty(BuildAllLevAwardData))
				{
					BuildAllLevAwardData += awardID;
				}
				else
				{
					BuildAllLevAwardData += '|';
					BuildAllLevAwardData += awardID;
				}
				Global.SaveRoleParamsStringToDB(client, strKey, BuildAllLevAwardData, true);
				strcmd = string.Format("{0}:{1}:{2}", result, roleID, awardID);
				client.sendCmd(nID, strcmd, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessBuildGetAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot8))
				{
					return false;
				}
				int result = 0;
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				int buildID = Global.SafeConvertToInt32(cmdParams[1]);
				int taskID = 0;
				BuildingData BuildData = null;
				for (int i = 0; i < client.ClientData.BuildingDataList.Count; i++)
				{
					if (client.ClientData.BuildingDataList[i].BuildId == buildID)
					{
						BuildData = client.ClientData.BuildingDataList[i];
						break;
					}
				}
				string strcmd;
				if (null == BuildData)
				{
					result = 12;
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						result,
						roleID,
						buildID,
						0,
						0
					});
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				BuildingConfigData BConfigData;
				this.BuildDict.TryGetValue(BuildData.BuildId, out BConfigData);
				if (null == BConfigData)
				{
					result = 12;
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						result,
						roleID,
						buildID,
						0,
						0
					});
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				if (BuildData.BuildTime == BuildingManager.ConstBuildTime)
				{
					result = 13;
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						result,
						roleID,
						buildID,
						0,
						0
					});
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				List<BuildTeam> BuildQueue = this.GetBuildingQueueData(client);
				for (int i = 0; i < BuildQueue.Count; i++)
				{
					if (BuildQueue[i].BuildID == buildID)
					{
						taskID = BuildQueue[i].TaskID;
						break;
					}
				}
				if (0 == taskID)
				{
					result = 13;
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						result,
						roleID,
						buildID,
						0,
						0
					});
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				BuildingTaskConfigData TaskConfigData;
				this.BuildTaskDict.TryGetValue(taskID, out TaskConfigData);
				if (null == TaskConfigData)
				{
					this.NewBuildTaskDict.TryGetValue(taskID, out TaskConfigData);
					if (null == TaskConfigData)
					{
						result = 13;
						strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							result,
							roleID,
							buildID,
							0,
							0
						});
						client.sendCmd(nID, strcmd, false);
						return true;
					}
				}
				BuildingState state = this.GetBuildState(client, buildID, taskID);
				if (BuildingState.EBS_Finish != state)
				{
					result = 6;
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						result,
						roleID,
						buildID,
						0,
						0
					});
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				if (!this.RemoveBuildingQueueData(client, buildID, taskID))
				{
					result = 13;
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						result,
						roleID,
						buildID,
						BuildData.BuildLev,
						BuildData.BuildExp
					});
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				this.BuildTaskFinish(client, BuildData, BConfigData, TaskConfigData);
				this.UpdateBuildingDataDB(client, BuildData);
				byte[] bytesData = DataHelper.ObjectToBytes<BuildingData>(BuildData);
				GameManager.ClientMgr.SendToClient(client, bytesData, 1560);
				if (client._IconStateMgr.CheckBuildingIcon(client, false))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
				strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					result,
					roleID,
					buildID,
					BuildData.BuildLev,
					0
				});
				client.sendCmd(nID, strcmd, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessBuildOpenQueueCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot8))
				{
					return false;
				}
				return false;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessBuildGetQueueCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				string strcmd = string.Format("{0}:{1}", result, roleID);
				string strKey = "BuildQueueData";
				string BuildingQueueData = Global.GetRoleParamByName(client, strKey);
				if (!string.IsNullOrEmpty(BuildingQueueData))
				{
					string[] Filed = BuildingQueueData.Split(new char[]
					{
						','
					});
					strcmd += ':';
					strcmd += Filed[0];
					for (int i = 1; i < Filed.Length; i++)
					{
						strcmd += ':';
						strcmd += Filed[i];
					}
				}
				else
				{
					strcmd += ':';
					strcmd += 0;
				}
				client.sendCmd(nID, strcmd, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessBuildGetAllLevelAwardStateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				string strKey = "BuildAllLevAward";
				string BuildAllLevAwardData = Global.GetRoleParamByName(client, strKey);
				if (!string.IsNullOrEmpty(BuildAllLevAwardData))
				{
					List<int> AwardedList = new List<int>();
					string[] Filed = BuildAllLevAwardData.Split(new char[]
					{
						'|'
					});
					for (int i = 0; i < Filed.Length; i++)
					{
						AwardedList.Add(Global.SafeConvertToInt32(Filed[i]));
					}
					AwardedList.Sort(delegate(int left, int right)
					{
						int result2;
						if (left < right)
						{
							result2 = -1;
						}
						else if (left > right)
						{
							result2 = 1;
						}
						else
						{
							result2 = 0;
						}
						return result2;
					});
					BuildAllLevAwardData = string.Join<int>("|", AwardedList.ToArray());
				}
				string strcmd = string.Format("{0}:{1}:{2}", result, roleID, BuildAllLevAwardData);
				client.sendCmd(nID, strcmd, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessBuildGetStateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				string strcmd = string.Format("{0}:{1}", result, roleID);
				List<BuildTeam> BuildQueue = this.GetBuildingQueueData(client);
				for (int i = 0; i < BuildQueue.Count; i++)
				{
					strcmd += ':';
					strcmd += BuildQueue[i].BuildID;
					strcmd += '|';
					strcmd += (int)this.GetBuildState(client, BuildQueue[i].BuildID, BuildQueue[i].TaskID);
				}
				strcmd += string.Format(":{0}|{1}", 7, YaoSaiJianYuManager.getInstance().GetYaoSaiJianYuState(client.ClientData.RoleID, 0));
				client.sendCmd(nID, strcmd, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool InitConfig()
		{
			string QueueNumMax = GameManager.systemParamsList.GetParamValueByName("ManorQueueNum");
			if (!string.IsNullOrEmpty(QueueNumMax))
			{
				this.ManorQueueNumMax = Global.SafeConvertToInt32(QueueNumMax);
			}
			QueueNumMax = GameManager.systemParamsList.GetParamValueByName("ManorFreeQueueNum");
			if (!string.IsNullOrEmpty(QueueNumMax))
			{
				this.ManorFreeQueueNumMax = Global.SafeConvertToInt32(QueueNumMax);
			}
			string QuickFinishNum = GameManager.systemParamsList.GetParamValueByName("ManorQuickFinishNum");
			if (!string.IsNullOrEmpty(QuickFinishNum))
			{
				this.ManorQuickFinishNum = Global.SafeConvertToInt32(QuickFinishNum);
			}
			string RandomTaskPrice = GameManager.systemParamsList.GetParamValueByName("ManorRandomTaskPrice");
			if (!string.IsNullOrEmpty(RandomTaskPrice))
			{
				this.ManorRandomTaskPrice = Global.SafeConvertToInt32(RandomTaskPrice);
			}
			string PayQueueOpenPrice = GameManager.systemParamsList.GetParamValueByName("ManorQueuePrice");
			if (!string.IsNullOrEmpty(PayQueueOpenPrice))
			{
				this.ManorQueuePrice = Global.SafeConvertToInt32(PayQueueOpenPrice);
			}
			return this.LoadBuildFile() && this.LoadBuildTaskFile() && this.LoadBuildLevelFile() && this.LoadBuildLevelAwardFile();
		}

		
		public bool LoadBuildFile()
		{
			try
			{
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/Manor/Build.xml"));
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						BuildingConfigData myBuild = new BuildingConfigData();
						myBuild.BuildID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						myBuild.MaxLevel = (int)Global.GetSafeAttributeLong(xmlItem, "MaxLevel");
						string RateFreeRandom = Global.GetSafeAttributeStr(xmlItem, "FreeRandomTask");
						if (string.IsNullOrEmpty(RateFreeRandom))
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("读取建筑物配置文件中的免费任务配置项1失败", new object[0]), null, true);
						}
						else
						{
							string[] fields = RateFreeRandom.Split(new char[]
							{
								'|'
							});
							if (fields.Length <= 0)
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("解析建筑物配置文件中的免费任务配置项1失败", new object[0]), null, true);
							}
							else
							{
								for (int i = 0; i < fields.Length; i++)
								{
									string[] IDVsRate = fields[i].Split(new char[]
									{
										','
									});
									if (IDVsRate.Length >= 2)
									{
										BuildingRandomData brdata = new BuildingRandomData();
										brdata.quality = (BuildingQuality)Global.SafeConvertToInt32(IDVsRate[0]);
										brdata.rate = Convert.ToDouble(IDVsRate[1]);
										myBuild.FreeRandomList.Add(brdata);
									}
								}
							}
						}
						string RateRandom = Global.GetSafeAttributeStr(xmlItem, "RandomTask");
						if (string.IsNullOrEmpty(RateRandom))
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("读取建筑物配置文件中的任务配置项1失败", new object[0]), null, true);
						}
						else
						{
							string[] fields = RateRandom.Split(new char[]
							{
								'|'
							});
							if (fields.Length <= 0)
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("解析建筑物配置文件中的任务配置项1失败", new object[0]), null, true);
							}
							else
							{
								for (int i = 0; i < fields.Length; i++)
								{
									string[] IDVsRate = fields[i].Split(new char[]
									{
										','
									});
									if (IDVsRate.Length >= 2)
									{
										BuildingRandomData brdata = new BuildingRandomData();
										brdata.quality = (BuildingQuality)Global.SafeConvertToInt32(IDVsRate[0]);
										brdata.rate = Convert.ToDouble(IDVsRate[1]);
										myBuild.RandomList.Add(brdata);
									}
								}
							}
						}
						this.BuildDict[myBuild.BuildID] = myBuild;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Build.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		public bool LoadBuildTaskFile()
		{
			try
			{
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/Manor/BuildTask.xml"));
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				Dictionary<int, List<BuildingTaskConfigData>> newBuildTaskDict = new Dictionary<int, List<BuildingTaskConfigData>>();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						BuildingTaskConfigData myBuildTask = new BuildingTaskConfigData();
						myBuildTask.TaskID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						myBuildTask.BuildID = (int)Global.GetSafeAttributeLong(xmlItem, "BuildID");
						myBuildTask.quality = (BuildingQuality)Global.GetSafeAttributeLong(xmlItem, "Quality");
						myBuildTask.SumNum = Global.GetSafeAttributeDouble(xmlItem, "Sum");
						myBuildTask.ExpNum = Global.GetSafeAttributeDouble(xmlItem, "ExpNum");
						myBuildTask.Time = (int)Global.GetSafeAttributeLong(xmlItem, "Time");
						if (myBuildTask.TaskID < 1000)
						{
							this.BuildTaskDict[myBuildTask.TaskID] = myBuildTask;
						}
						else
						{
							this.NewBuildTaskDict[myBuildTask.TaskID] = myBuildTask;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "BuildTask.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		public bool LoadBuildLevelFile()
		{
			try
			{
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/Manor/BuildLevel.xml"));
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						BuildingLevelConfigData myBuildLevel = new BuildingLevelConfigData();
						myBuildLevel.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						myBuildLevel.BuildID = (int)Global.GetSafeAttributeLong(xmlItem, "BuildID");
						myBuildLevel.Level = (int)Global.GetSafeAttributeLong(xmlItem, "Level");
						myBuildLevel.UpNeedExp = (int)Global.GetSafeAttributeLong(xmlItem, "UpNeedExp");
						myBuildLevel.Exp = Global.GetSafeAttributeDouble(xmlItem, "Exp");
						myBuildLevel.Money = Global.GetSafeAttributeDouble(xmlItem, "Money");
						myBuildLevel.MoJing = Global.GetSafeAttributeDouble(xmlItem, "MoJing");
						myBuildLevel.XingHun = Global.GetSafeAttributeDouble(xmlItem, "XingHun");
						myBuildLevel.ChengJiu = Global.GetSafeAttributeDouble(xmlItem, "ChengJiu");
						myBuildLevel.ShengWang = Global.GetSafeAttributeDouble(xmlItem, "ShengWang");
						myBuildLevel.YuanSu = Global.GetSafeAttributeDouble(xmlItem, "YuanSu");
						myBuildLevel.YingGuang = Global.GetSafeAttributeDouble(xmlItem, "YingGuang");
						this.BuildLevelDict.Add(new KeyValuePair<int, int>(myBuildLevel.BuildID, myBuildLevel.Level), myBuildLevel);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "BuildTask.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		public bool LoadBuildLevelAwardFile()
		{
			try
			{
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/Manor/BuildLevelAward.xml"));
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					BuildingLevelAwardConfigData myBuildLevelAward = new BuildingLevelAwardConfigData();
					myBuildLevelAward.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
					myBuildLevelAward.AllLevel = (int)Global.GetSafeAttributeLong(xmlItem, "AllLevel");
					string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "Award");
					if (string.IsNullOrEmpty(goodsIDs))
					{
						LogManager.WriteLog(LogTypes.Warning, string.Format("读取建筑物总等级奖励配置项1失败", new object[0]), null, true);
					}
					else
					{
						ConfigParser.ParseAwardsItemList(goodsIDs, ref myBuildLevelAward.GoodsList, '|', ',');
					}
					this.BuildLevelAwardDict[myBuildLevelAward.ID] = myBuildLevelAward;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "BuildLevelAward.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		public void ModifyNengLiangPointsValue(GameClient client, int addType, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			if (addValue != 0 && addType >= 1 && addType <= 4)
			{
				switch (addType)
				{
				case 1:
					client.ClientData.NengLiangSmall += addValue;
					client.ClientData.NengLiangSmall = Math.Max(client.ClientData.NengLiangSmall, 0);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "能量核心_小", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, client.ClientData.NengLiangSmall, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10168", client.ClientData.NengLiangSmall, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.NengLiangSmall, (long)addValue, (long)client.ClientData.NengLiangSmall, strFrom);
					if (notifyClient)
					{
						GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.NengLiangSmall, client.ClientData.NengLiangSmall);
					}
					break;
				case 2:
					client.ClientData.NengLiangMedium += addValue;
					client.ClientData.NengLiangMedium = Math.Max(client.ClientData.NengLiangMedium, 0);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "能量核心_中", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, client.ClientData.NengLiangMedium, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10169", client.ClientData.NengLiangMedium, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.NengLiangMedium, (long)addValue, (long)client.ClientData.NengLiangMedium, strFrom);
					if (notifyClient)
					{
						GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.NengLiangMedium, client.ClientData.NengLiangMedium);
					}
					break;
				case 3:
					client.ClientData.NengLiangBig += addValue;
					client.ClientData.NengLiangBig = Math.Max(client.ClientData.NengLiangBig, 0);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "能量核心_大", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, client.ClientData.NengLiangBig, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10170", client.ClientData.NengLiangBig, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.NengLiangBig, (long)addValue, (long)client.ClientData.NengLiangBig, strFrom);
					if (notifyClient)
					{
						GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.NengLiangBig, client.ClientData.NengLiangBig);
					}
					break;
				case 4:
					client.ClientData.NengLiangSuper += addValue;
					client.ClientData.NengLiangSuper = Math.Max(client.ClientData.NengLiangSuper, 0);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "能量核心_超", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, client.ClientData.NengLiangSuper, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10171", client.ClientData.NengLiangSuper, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.NengLiangSuper, (long)addValue, (long)client.ClientData.NengLiangSuper, strFrom);
					if (notifyClient)
					{
						GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.NengLiangSuper, client.ClientData.NengLiangSuper);
					}
					break;
				}
			}
		}

		
		private const string Build_fileName = "Config/Manor/Build.xml";

		
		private const string BuildTask_fileName = "Config/Manor/BuildTask.xml";

		
		private const string BuildLevel_fileName = "Config/Manor/BuildLevel.xml";

		
		private const string BuildLevelAward_fileName = "Config/Manor/BuildLevelAward.xml";

		
		private const int NewBuildTaskIDBind = 1000;

		
		public static readonly string ConstBuildTime = "0000-00-00 00:00:00";

		
		public object RandomTaskMutex = new object();

		
		private static BuildingManager instance = new BuildingManager();

		
		protected Dictionary<int, BuildingConfigData> BuildDict = new Dictionary<int, BuildingConfigData>();

		
		protected Dictionary<int, BuildingTaskConfigData> BuildTaskDict = new Dictionary<int, BuildingTaskConfigData>();

		
		protected Dictionary<KeyValuePair<int, int>, BuildingLevelConfigData> BuildLevelDict = new Dictionary<KeyValuePair<int, int>, BuildingLevelConfigData>();

		
		protected Dictionary<int, BuildingLevelAwardConfigData> BuildLevelAwardDict = new Dictionary<int, BuildingLevelAwardConfigData>();

		
		protected Dictionary<int, BuildingTaskConfigData> NewBuildTaskDict = new Dictionary<int, BuildingTaskConfigData>();

		
		public int ManorFreeQueueNumMax = 0;

		
		public int ManorQueueNumMax = 0;

		
		public int ManorQuickFinishNum = 0;

		
		public int ManorRandomTaskPrice = 0;

		
		public int ManorQueuePrice = 0;
	}
}
