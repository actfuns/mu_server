using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Data;
using Server.Tools;
using Tmsk.Contract.KuaFuData;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	// Token: 0x02000023 RID: 35
	public class JunTuanEraService
	{
		// Token: 0x0600010B RID: 267 RVA: 0x0000D80C File Offset: 0x0000BA0C
		public static JunTuanEraService Instance()
		{
			return JunTuanEraService._instance;
		}

		// Token: 0x0600010C RID: 268 RVA: 0x0000D824 File Offset: 0x0000BA24
		public void InitConfig()
		{
			try
			{
				lock (this.RuntimeData.Mutex)
				{
					string fileName = "Config/EraUI.xml";
					string fullPathFileName = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
					this.RuntimeData.EraUIConfigDict.Load(fullPathFileName, null);
					fileName = "Config/EraTask.xml";
					fullPathFileName = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
					this.RuntimeData.EraTaskConfigDict.Clear();
					XElement xml = ConfigHelper.Load(fullPathFileName);
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						EraTaskConfig data = new EraTaskConfig();
						data.ID = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "ID", 0L);
						data.EraID = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "EraID", 0L);
						data.EraStage = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "EraStage", 0L);
						data.Reward = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "Reward", 0L);
						string tempValue = ConfigHelper.GetElementAttributeValue(xmlItem, "CompletionCondition", "");
						string[] tempFields = tempValue.Split(new char[]
						{
							'|'
						});
						foreach (string item in tempFields)
						{
							string[] strkvp = item.Split(new char[]
							{
								','
							});
							if (strkvp.Length == 2)
							{
								int goodsid = Convert.ToInt32(strkvp[0]);
								int num = Convert.ToInt32(strkvp[1]);
								data.CompletionCondition.Add(new KeyValuePair<int, int>(goodsid, num));
							}
						}
						this.RuntimeData.EraTaskConfigDict[data.ID] = data;
					}
					fileName = "Config/EraReward.xml";
					fullPathFileName = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
					this.RuntimeData.EraAwardConfigDict.Clear();
					xml = ConfigHelper.Load(fullPathFileName);
					xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						EraAwardConfigBase data2 = new EraAwardConfigBase();
						data2.ID = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "ID", 0L);
						data2.EraID = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "EraID", 0L);
						data2.AwardType = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "Type", 0L);
						string StartTime = ConfigHelper.GetElementAttributeValue(xmlItem, "StartTime", "");
						if (!string.IsNullOrEmpty(StartTime))
						{
							DateTime.TryParse(StartTime, out data2.StartTime);
						}
						string EndTime = ConfigHelper.GetElementAttributeValue(xmlItem, "EndTime", "");
						if (!string.IsNullOrEmpty(EndTime))
						{
							DateTime.TryParse(EndTime, out data2.EndTime);
						}
						this.RuntimeData.EraAwardConfigDict[data2.ID] = data2;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		// Token: 0x0600010D RID: 269 RVA: 0x0000DBC8 File Offset: 0x0000BDC8
		public KuaFuCmdData GetEraRankData(long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				if (dataAge != this.EraRankList.Age)
				{
					result = new KuaFuCmdData
					{
						Age = this.EraRankList.Age,
						Bytes0 = DataHelper2.ObjectToBytes<List<KFEraRankData>>(this.EraRankList.V)
					};
				}
				else
				{
					result = new KuaFuCmdData
					{
						Age = this.EraRankList.Age
					};
				}
			}
			return result;
		}

		// Token: 0x0600010E RID: 270 RVA: 0x0000DC74 File Offset: 0x0000BE74
		public KuaFuCmdData GetEraData(int juntuanid, long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				if (0 == this.RuntimeData.CurrentEraID)
				{
					result = null;
				}
				else
				{
					KuaFuData<KFEraData> data = null;
					if (juntuanid == 0 && 0L == dataAge)
					{
						data = new KuaFuData<KFEraData>();
						data.V.EraID = this.RuntimeData.CurrentEraID;
						TimeUtil.AgeByNow(ref data.Age);
					}
					else
					{
						if (!this.EraDataDict.TryGetValue(juntuanid, out data))
						{
							data = new KuaFuData<KFEraData>();
							data.V.EraID = this.RuntimeData.CurrentEraID;
							data.V.JunTuanID = juntuanid;
							data.V.EraStage = 1;
							TimeUtil.AgeByNow(ref data.Age);
							this.EraDataDict[juntuanid] = data;
						}
						if (data.V.FastEraStage != this.RuntimeData.CurFastEraStage || data.V.FastEraStateProcess != this.RuntimeData.CurFastEraStateProcess)
						{
							TimeUtil.AgeByNow(ref data.Age);
						}
					}
					data.V.FastEraStage = this.RuntimeData.CurFastEraStage;
					data.V.FastEraStateProcess = this.RuntimeData.CurFastEraStateProcess;
					if (dataAge != data.Age)
					{
						result = new KuaFuCmdData
						{
							Age = data.Age,
							Bytes0 = DataHelper2.ObjectToBytes<KFEraData>(data.V)
						};
					}
					else
					{
						result = new KuaFuCmdData
						{
							Age = data.Age
						};
					}
				}
			}
			return result;
		}

		// Token: 0x0600010F RID: 271 RVA: 0x0000DE80 File Offset: 0x0000C080
		public bool EraDonate(int juntuanid, int taskid, int var1, int var2, int var3)
		{
			bool result;
			lock (this.Mutex)
			{
				if (0 == this.RuntimeData.CurrentEraID)
				{
					result = false;
				}
				else
				{
					KuaFuData<KFEraData> data = null;
					if (!this.EraDataDict.TryGetValue(juntuanid, out data))
					{
						result = false;
					}
					else
					{
						EraTaskConfig taskConfig = null;
						lock (this.RuntimeData.Mutex)
						{
							if (!this.RuntimeData.EraTaskConfigDict.TryGetValue(taskid, out taskConfig))
							{
								return false;
							}
						}
						if (taskConfig.EraID != this.RuntimeData.CurrentEraID)
						{
							result = false;
						}
						else
						{
							EraTaskData taskData = data.V.EraTaskList.Find((EraTaskData x) => x.TaskID == taskid);
							if (null == taskData)
							{
								taskData = new EraTaskData();
								taskData.TaskID = taskid;
								data.V.EraTaskList.Add(taskData);
							}
							if (this.CheckTaskComplete(taskData, taskConfig))
							{
								result = true;
							}
							else
							{
								for (int dataidx = 0; dataidx < taskConfig.CompletionCondition.Count; dataidx++)
								{
									int conditionNum = taskConfig.CompletionCondition[dataidx].Value;
									switch (dataidx)
									{
									case 0:
										taskData.TaskVal1 = Math.Min(taskData.TaskVal1 + var1, conditionNum);
										break;
									case 1:
										taskData.TaskVal2 = Math.Min(taskData.TaskVal2 + var2, conditionNum);
										break;
									case 2:
										taskData.TaskVal3 = Math.Min(taskData.TaskVal3 + var3, conditionNum);
										break;
									}
								}
								if (this.CheckTaskComplete(taskData, taskConfig))
								{
									if (this.HandleAddEraProcess(data, taskConfig))
									{
										this.SaveEraData(data.V, true);
									}
								}
								else
								{
									this.SaveEraData(data.V, false);
								}
								this.SaveEraTaskData(juntuanid, taskData);
								TimeUtil.AgeByNow(ref data.Age);
								result = true;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000110 RID: 272 RVA: 0x0000E130 File Offset: 0x0000C330
		private void HandleFastEraStage(KFEraData data)
		{
			if (data.EraStage > this.RuntimeData.CurFastEraStage)
			{
				this.RuntimeData.CurFastEraStage = data.EraStage;
				this.RuntimeData.CurFastEraStateProcess = data.EraStageProcess;
			}
			else if (data.EraStage == this.RuntimeData.CurFastEraStage && data.EraStageProcess > this.RuntimeData.CurFastEraStateProcess)
			{
				this.RuntimeData.CurFastEraStateProcess = data.EraStageProcess;
			}
		}

		// Token: 0x06000111 RID: 273 RVA: 0x0000E2AC File Offset: 0x0000C4AC
		private bool HandleAddEraProcess(KuaFuData<KFEraData> data, EraTaskConfig taskConfig)
		{
			bool result;
			if (taskConfig.EraStage != (int)data.V.EraStage)
			{
				result = false;
			}
			else
			{
				int oldEraStageProcess = data.V.EraStageProcess;
				int oldEraStage = (int)data.V.EraStage;
				data.V.EraStageProcess = Math.Min(data.V.EraStageProcess + taskConfig.Reward, 100);
				if (data.V.EraStageProcess == 100 && data.V.EraStage < 4)
				{
					data.V.EraStage = (byte)Math.Min((int)(data.V.EraStage + 1), 4);
					data.V.EraStageProcess = 0;
					AsyncDataItem evItem = new AsyncDataItem(KuaFuEventTypes.JunTuanEraStage, new object[]
					{
						data.V.JunTuanID
					});
					ClientAgentManager.Instance().BroadCastAsyncEvent(GameTypes.JunTuan, evItem, 0);
					data.V.EraTimePointList.Add(TimeUtil.NowDateTime());
				}
				if (data.V.EraStageProcess == oldEraStageProcess && (int)data.V.EraStage == oldEraStage)
				{
					result = false;
				}
				else
				{
					if (data.V.EraStageProcess == 100 && data.V.EraStage == 4)
					{
						data.V.EraTimePointList.Add(TimeUtil.NowDateTime());
					}
					this.HandleFastEraStage(data.V);
					bool needSort = false;
					KFEraRankData rankData = this.EraRankList.V.Find((KFEraRankData x) => x.JunTuanID == data.V.JunTuanID);
					if (null != rankData)
					{
						needSort = true;
						rankData.JunTuanID = data.V.JunTuanID;
						rankData.EraStage = data.V.EraStage;
						rankData.EraStageProcess = data.V.EraStageProcess;
						rankData.RankTime = TimeUtil.NowDateTime();
					}
					else if (this.EraRankList.V.Count < 5)
					{
						needSort = true;
						rankData = new KFEraRankData();
						rankData.JunTuanID = data.V.JunTuanID;
						rankData.EraStage = data.V.EraStage;
						rankData.EraStageProcess = data.V.EraStageProcess;
						rankData.RankTime = TimeUtil.NowDateTime();
						this.EraRankList.V.Add(rankData);
					}
					else
					{
						KFEraRankData minRankData = this.EraRankList.V[this.EraRankList.V.Count - 1];
						if (data.V.EraStage > minRankData.EraStage || (data.V.EraStage == minRankData.EraStage && data.V.EraStageProcess > minRankData.EraStageProcess))
						{
							needSort = true;
							minRankData.JunTuanID = data.V.JunTuanID;
							minRankData.EraStage = data.V.EraStage;
							minRankData.EraStageProcess = data.V.EraStageProcess;
							minRankData.RankTime = TimeUtil.NowDateTime();
						}
					}
					if (needSort)
					{
						this.EraRankList.V.Sort(delegate(KFEraRankData left, KFEraRankData right)
						{
							int result2;
							if (left.EraStage > right.EraStage)
							{
								result2 = -1;
							}
							else if (left.EraStage < right.EraStage)
							{
								result2 = 1;
							}
							else if (left.EraStageProcess > right.EraStageProcess)
							{
								result2 = -1;
							}
							else if (left.EraStageProcess < right.EraStageProcess)
							{
								result2 = 1;
							}
							else if (left.RankTime < right.RankTime)
							{
								result2 = -1;
							}
							else if (left.RankTime > right.RankTime)
							{
								result2 = 1;
							}
							else
							{
								result2 = 0;
							}
							return result2;
						});
						for (int loop = 0; loop < this.EraRankList.V.Count; loop++)
						{
							this.EraRankList.V[loop].RankValue = loop + 1;
						}
						TimeUtil.AgeByNow(ref this.EraRankList.Age);
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06000112 RID: 274 RVA: 0x0000E748 File Offset: 0x0000C948
		private bool CheckTaskComplete(EraTaskData taskData, EraTaskConfig taskConfig)
		{
			bool taskcomplete = true;
			for (int dataidx = 0; dataidx < taskConfig.CompletionCondition.Count; dataidx++)
			{
				int conditionNum = taskConfig.CompletionCondition[dataidx].Value;
				switch (dataidx)
				{
				case 0:
					taskcomplete &= (taskData.TaskVal1 >= conditionNum);
					break;
				case 1:
					taskcomplete &= (taskData.TaskVal2 >= conditionNum);
					break;
				case 2:
					taskcomplete &= (taskData.TaskVal3 >= conditionNum);
					break;
				}
			}
			return taskcomplete;
		}

		// Token: 0x06000113 RID: 275 RVA: 0x0000E7EC File Offset: 0x0000C9EC
		private int CalCurrentEraID(DateTime now)
		{
			lock (this.RuntimeData.Mutex)
			{
				Dictionary<int, EraUIConfig> eraConfig = this.RuntimeData.EraUIConfigDict.Value;
				foreach (EraUIConfig item in eraConfig.Values)
				{
					if (now >= item.StartTime && now <= item.EndTime)
					{
						return item.ID;
					}
				}
			}
			return 0;
		}

		// Token: 0x06000114 RID: 276 RVA: 0x0000E8C8 File Offset: 0x0000CAC8
		public void HandleChangeEraID(DateTime now, bool broadCast = false)
		{
			lock (this.Mutex)
			{
				int dayID = TimeUtil.GetOffsetDay(now);
				if (dayID != this.RuntimeData.EraUpdateDayID)
				{
					this.RuntimeData.EraUpdateDayID = dayID;
					int CurrentEraID = this.CalCurrentEraID(now);
					if (CurrentEraID != this.RuntimeData.CurrentEraID)
					{
						this.EraDataDict.Clear();
						this.EraRankList.V.Clear();
						TimeUtil.AgeByNow(ref this.EraRankList.Age);
						this.RuntimeData.CurrentEraID = CurrentEraID;
						this.RuntimeData.CurFastEraStage = 1;
						this.RuntimeData.CurFastEraStateProcess = 0;
						if (broadCast)
						{
							AsyncDataItem evItem = new AsyncDataItem(KuaFuEventTypes.JunTuanEraChg, new object[]
							{
								CurrentEraID
							});
							ClientAgentManager.Instance().BroadCastAsyncEvent(GameTypes.JunTuan, evItem, 0);
						}
					}
				}
			}
		}

		// Token: 0x06000115 RID: 277 RVA: 0x0000E9EC File Offset: 0x0000CBEC
		private bool InRankAwardTime()
		{
			int curEraID = this.RuntimeData.CurrentEraID;
			bool result;
			if (curEraID <= 0)
			{
				result = false;
			}
			else
			{
				Dictionary<int, EraAwardConfigBase> tempEraAwardConfigDict = null;
				lock (this.RuntimeData.Mutex)
				{
					tempEraAwardConfigDict = this.RuntimeData.EraAwardConfigDict;
				}
				foreach (EraAwardConfigBase item in tempEraAwardConfigDict.Values)
				{
					if (item.EraID == curEraID && item.AwardType == 2)
					{
						DateTime now = TimeUtil.NowDateTime();
						if (now >= item.StartTime && now <= item.EndTime)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06000116 RID: 278 RVA: 0x0000EB08 File Offset: 0x0000CD08
		public void OnJunTuanDestroy(int juntuanId)
		{
			if (!this.InRankAwardTime())
			{
				lock (this.Mutex)
				{
					KuaFuData<KFEraData> data = null;
					if (this.EraDataDict.TryGetValue(juntuanId, out data))
					{
						bool reloadRank = false;
						foreach (KFEraRankData item in this.EraRankList.V)
						{
							if (item.JunTuanID == juntuanId)
							{
								reloadRank = true;
							}
						}
						this.ClearEraData(this.RuntimeData.CurrentEraID, juntuanId);
						this.EraDataDict.Remove(juntuanId);
						if (reloadRank)
						{
							this.LoadEraRankData();
						}
					}
				}
			}
		}

		// Token: 0x06000117 RID: 279 RVA: 0x0000EC14 File Offset: 0x0000CE14
		public void LoadJunTuanEraData()
		{
			this.HandleChangeEraID(TimeUtil.NowDateTime(), false);
			this.LoadEraData();
			this.HandleEraTaskAccident();
			this.LoadEraRankData();
		}

		// Token: 0x06000118 RID: 280 RVA: 0x0000EC3C File Offset: 0x0000CE3C
		private void HandleEraTaskAccident()
		{
			if (this.EraDataDict.Count > 0)
			{
				this.RuntimeData.CurFastEraStage = 1;
				this.RuntimeData.CurFastEraStateProcess = 0;
				foreach (KuaFuData<KFEraData> item_era in this.EraDataDict.Values)
				{
					int calProcess = 0;
					foreach (EraTaskData item_task in item_era.V.EraTaskList)
					{
						EraTaskConfig taskConfig = null;
						lock (this.RuntimeData.Mutex)
						{
							if (!this.RuntimeData.EraTaskConfigDict.TryGetValue(item_task.TaskID, out taskConfig))
							{
								continue;
							}
						}
						if (taskConfig.EraStage == (int)item_era.V.EraStage)
						{
							if (this.CheckTaskComplete(item_task, taskConfig))
							{
								calProcess += taskConfig.Reward;
							}
						}
					}
					if (calProcess != item_era.V.EraStageProcess || item_era.V.EraStageProcess > 100)
					{
						LogManager.WriteLog(LogTypes.Analysis, string.Format("HandleEraTaskAccident JunTuanId:{0} Stage:{1} BeforeProcess:{2} AfterProcess:{3}", new object[]
						{
							item_era.V.JunTuanID,
							item_era.V.EraStage,
							item_era.V.EraStageProcess,
							calProcess
						}), null, true);
						item_era.V.EraStageProcess = Math.Min(calProcess, 100);
						this.SaveEraData(item_era.V, false);
					}
					this.HandleFastEraStage(item_era.V);
				}
			}
		}

		// Token: 0x06000119 RID: 281 RVA: 0x0000EE9C File Offset: 0x0000D09C
		private void LoadEraRankData()
		{
			MySqlDataReader sdr = null;
			try
			{
				this.EraRankList.V.Clear();
				string strSql = string.Format("SELECT `juntuanid`, `stage`, `process`, `ranktm` FROM t_juntuan_era WHERE `eraid`={0} AND (`stage`>1 OR (`stage`=1 AND `process`>0))\r\n                                ORDER BY `stage` DESC, `process` DESC, `ranktm` ASC LIMIT {1};", this.RuntimeData.CurrentEraID, 5);
				sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				int index = 1;
				while (sdr != null && sdr.Read())
				{
					KFEraRankData data = new KFEraRankData();
					data.RankValue = index;
					data.JunTuanID = Convert.ToInt32(sdr["juntuanid"].ToString());
					data.EraStage = Convert.ToByte(sdr["stage"].ToString());
					data.EraStageProcess = Convert.ToInt32(sdr["process"].ToString());
					string strRankTm = sdr["ranktm"].ToString();
					if (!string.IsNullOrEmpty(strRankTm))
					{
						DateTime.TryParse(strRankTm, out data.RankTime);
					}
					this.EraRankList.V.Add(data);
					index++;
				}
				TimeUtil.AgeByNow(ref this.EraRankList.Age);
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			finally
			{
				if (null != sdr)
				{
					sdr.Close();
				}
			}
		}

		// Token: 0x0600011A RID: 282 RVA: 0x0000F01C File Offset: 0x0000D21C
		private void LoadEraData()
		{
			MySqlDataReader sdr = null;
			try
			{
				string strSql = string.Format("SELECT * FROM t_juntuan_era WHERE `eraid`={0};", this.RuntimeData.CurrentEraID);
				sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				this.EraDataDict.Clear();
				int index = 1;
				while (sdr != null && sdr.Read())
				{
					KuaFuData<KFEraData> data = new KuaFuData<KFEraData>();
					data.V.EraID = Convert.ToInt32(sdr["eraid"].ToString());
					data.V.JunTuanID = Convert.ToInt32(sdr["juntuanid"].ToString());
					data.V.EraStage = Convert.ToByte(sdr["stage"].ToString());
					data.V.EraStageProcess = Convert.ToInt32(sdr["process"].ToString());
					data.V.EraTaskList = this.LoadEraTaskList(data.V.JunTuanID);
					data.V.ParseEraTimePointsData(sdr["tmpoints"].ToString());
					TimeUtil.AgeByNow(ref data.Age);
					this.EraDataDict[data.V.JunTuanID] = data;
					this.HandleFastEraStage(data.V);
					index++;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			finally
			{
				if (null != sdr)
				{
					sdr.Close();
				}
			}
		}

		// Token: 0x0600011B RID: 283 RVA: 0x0000F1D4 File Offset: 0x0000D3D4
		private List<EraTaskData> LoadEraTaskList(int junTuanId)
		{
			MySqlDataReader sdr = null;
			try
			{
				List<EraTaskData> EraTaskList = new List<EraTaskData>();
				string strSql = string.Format("SELECT * FROM t_juntuan_era_task WHERE `eraid`={0} AND `juntuanid`={1};", this.RuntimeData.CurrentEraID, junTuanId);
				sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				while (sdr != null && sdr.Read())
				{
					EraTaskList.Add(new EraTaskData
					{
						TaskID = Convert.ToInt32(sdr["taskid"].ToString()),
						TaskVal1 = Convert.ToInt32(sdr["taskv1"].ToString()),
						TaskVal2 = Convert.ToInt32(sdr["taskv2"].ToString()),
						TaskVal3 = Convert.ToInt32(sdr["taskv3"].ToString())
					});
				}
				return EraTaskList;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			finally
			{
				if (null != sdr)
				{
					sdr.Close();
				}
			}
			return null;
		}

		// Token: 0x0600011C RID: 284 RVA: 0x0000F300 File Offset: 0x0000D500
		private void SaveEraTaskData(int junTuanId, EraTaskData data)
		{
			string sql = string.Format("INSERT INTO `t_juntuan_era_task` (`eraid`,`juntuanid`,`taskid`,`taskv1`,`taskv2`,`taskv3`) VALUES ({0},{1},{2},{3},{4},{5})\r\n                                ON DUPLICATE KEY UPDATE `taskv1`={3}, `taskv2`={4}, `taskv3`={5};", new object[]
			{
				this.RuntimeData.CurrentEraID,
				junTuanId,
				data.TaskID,
				data.TaskVal1,
				data.TaskVal2,
				data.TaskVal3
			});
			this.ExecuteSqlNoQuery(sql);
		}

		// Token: 0x0600011D RID: 285 RVA: 0x0000F380 File Offset: 0x0000D580
		private void SaveEraData(KFEraData data, bool chgProcess = false)
		{
			string sql = string.Format("INSERT INTO `t_juntuan_era` (`eraid`,`juntuanid`,`stage`,`process`,`tmpoints`,`ranktm`) VALUES ({0},{1},{2},{3},'{4}',NOW())\r\n                                ON DUPLICATE KEY UPDATE `stage`={2}, `process`={3}, `tmpoints`='{4}';", new object[]
			{
				data.EraID,
				data.JunTuanID,
				data.EraStage,
				data.EraStageProcess,
				data.getStringValue(data.EraTimePointList)
			});
			this.ExecuteSqlNoQuery(sql);
			if (chgProcess)
			{
				sql = string.Format("UPDATE t_juntuan_era SET ranktm=NOW() WHERE `eraid`={0} AND `juntuanid`={1};", data.EraID, data.JunTuanID);
				this.ExecuteSqlNoQuery(sql);
			}
		}

		// Token: 0x0600011E RID: 286 RVA: 0x0000F424 File Offset: 0x0000D624
		private void ClearEraData(int eraID, int juntuanId)
		{
			string sql = string.Format("DELETE FROM `t_juntuan_era` WHERE `eraid`={0} AND `juntuanid`={1}", eraID, juntuanId);
			this.ExecuteSqlNoQuery(sql);
		}

		// Token: 0x0600011F RID: 287 RVA: 0x0000F454 File Offset: 0x0000D654
		private int ExecuteSqlNoQuery(string sqlCmd)
		{
			int result;
			try
			{
				LogManager.WriteLog(LogTypes.SQL, sqlCmd, null, true);
				result = DbHelperMySQL.ExecuteSql(sqlCmd);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(sqlCmd + ex.ToString());
				result = -1;
			}
			return result;
		}

		// Token: 0x040000CB RID: 203
		private static JunTuanEraService _instance = new JunTuanEraService();

		// Token: 0x040000CC RID: 204
		private object Mutex = new object();

		// Token: 0x040000CD RID: 205
		private JunTuanEraRuntimeData RuntimeData = new JunTuanEraRuntimeData();

		// Token: 0x040000CE RID: 206
		public KuaFuData<List<KFEraRankData>> EraRankList = new KuaFuData<List<KFEraRankData>>();

		// Token: 0x040000CF RID: 207
		public Dictionary<int, KuaFuData<KFEraData>> EraDataDict = new Dictionary<int, KuaFuData<KFEraData>>();
	}
}
