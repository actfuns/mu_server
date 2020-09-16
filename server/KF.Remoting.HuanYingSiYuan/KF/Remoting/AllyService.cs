using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.ServiceModel;
using System.Threading;
using GameServer.Core.Executor;
using KF.Contract;
using KF.Contract.Data;
using KF.Remoting.Data;
using Server.Tools;
using Tmsk.Contract.Interface;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting
{
	
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = true)]
	public class AllyService : MarshalByRefObject, IAllyService
	{
		
		public override object InitializeLifetimeService()
		{
			AllyService.Instance = this;
			ILease lease = (ILease)base.InitializeLifetimeService();
			if (lease.CurrentState == LeaseState.Initial)
			{
				lease.InitialLeaseTime = TimeSpan.FromDays(2000.0);
			}
			return lease;
		}

		
		public AllyService()
		{
			AllyService.Instance = this;
			this._BackgroundThread = new Thread(new ParameterizedThreadStart(this.ThreadProc));
			this._BackgroundThread.IsBackground = true;
			this._BackgroundThread.Start();
		}

		
		~AllyService()
		{
			this._BackgroundThread.Abort();
		}

		
		public void ThreadProc(object state)
		{
			do
			{
				Thread.Sleep(1000);
			}
			while (!this._Persistence.Initialized);
			for (;;)
			{
				try
				{
					DateTime now = TimeUtil.NowDateTime();
					Global.UpdateNowTime(now);
					if (now > this._clearTimeRequest)
					{
						this._clearTimeRequest = now.AddSeconds(30.0);
						this.ClearAcceptData();
						this.ClearRequestData();
					}
					RankService.getInstance().Update();
					int sleepMS = (int)(TimeUtil.NowDateTime() - now).TotalMilliseconds;
					sleepMS = 1600 - sleepMS;
					if (sleepMS < 50)
					{
						sleepMS = 50;
					}
					Thread.Sleep(sleepMS);
				}
				catch (Exception ex)
				{
					LogManager.WriteExceptionUseCache(ex.ToString());
				}
			}
		}

		
		private void ClearAcceptData()
		{
			lock (this._Mutex)
			{
				if (this._acceptDic != null && this._acceptDic.Count > 0)
				{
					foreach (KeyValuePair<int, List<KFAllyData>> r in this._acceptDic)
					{
						int unionID = r.Key;
						List<KFAllyData> oldList = r.Value;
						IEnumerable<KFAllyData> temp = from info in oldList
						where info.LogTime <= TimeUtil.NowDateTime().AddSeconds((double)(-(double)Consts.AllyRequestClearSecond))
						select info;
						if (temp.Any<KFAllyData>())
						{
							KFAllyData myData = this.GetUnionData(unionID);
							List<KFAllyData> list = temp.ToList<KFAllyData>();
							foreach (KFAllyData targetData in list)
							{
								int state = this.AllyOperate(myData.ServerID, myData.UnionID, targetData.UnionID, 2, false);
							}
						}
					}
				}
			}
		}

		
		private void ClearRequestData()
		{
			lock (this._Mutex)
			{
				if (this._requestDic != null && this._requestDic.Count > 0)
				{
					foreach (KeyValuePair<int, List<KFAllyData>> r in this._requestDic)
					{
						int unionID = r.Key;
						List<KFAllyData> oldList = r.Value;
						IEnumerable<KFAllyData> temp = from info in oldList
						where info.LogTime <= TimeUtil.NowDateTime().AddSeconds((double)(-(double)Consts.AllyRequestClearSecond))
						select info;
						if (temp.Any<KFAllyData>())
						{
							KFAllyData myData = this.GetUnionData(unionID);
							List<KFAllyData> list = temp.ToList<KFAllyData>();
							foreach (KFAllyData targetData in list)
							{
								int state = this.AllyOperate(targetData.ServerID, targetData.UnionID, myData.UnionID, 2, false);
							}
						}
					}
				}
			}
		}

		
		public int InitializeClient(KuaFuClientContext clientInfo)
		{
			int result;
			try
			{
				if (clientInfo.GameType == (int)this._gameType && clientInfo.ServerId != 0)
				{
					result = ClientAgentManager.Instance().InitializeClient(clientInfo);
				}
				else
				{
					LogManager.WriteLog(LogTypes.Warning, string.Format("InitializeClient时GameType错误,禁止连接.ServerId:{0},GameType:{1}", clientInfo.ServerId, clientInfo.GameType), null, true);
					result = -4003;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(string.Format("InitializeClient服务器ID重复,禁止连接.ServerId:{0},ClientId:{1}", clientInfo.ServerId, clientInfo.ClientId));
				result = -11003;
			}
			return result;
		}

		
		public AsyncDataItem[] GetClientCacheItems(int serverID)
		{
			return ClientAgentManager.Instance().PickAsyncEvent(serverID, this._gameType);
		}

		
		public long UnionAllyVersion(int serverID)
		{
			long result;
			if (!this.IsAgent(serverID))
			{
				result = 0L;
			}
			else
			{
				result = this._Persistence.DataVersion;
			}
			return result;
		}

		
		public int UnionAllyInit(int serverID, int unionID, bool isKF)
		{
			int result;
			if (!this.IsAgent(serverID))
			{
				result = -16;
			}
			else
			{
				lock (this._Mutex)
				{
					KFAllyData oldData;
					if (!this._unionDic.TryGetValue(unionID, out oldData))
					{
						oldData = AllyPersistence.Instance.DBUnionDataGet(unionID);
						if (oldData == null)
						{
							return -18;
						}
						this._unionDic.TryAdd(unionID, oldData);
					}
					if (!isKF && oldData.ServerID != serverID)
					{
						oldData.ServerID = serverID;
						if (!AllyPersistence.Instance.DBUnionDataUpdate(oldData))
						{
							return -17;
						}
					}
					if (!isKF)
					{
						this.CheckAllyLog(serverID, unionID);
					}
					oldData.UpdateLogtime();
					result = 50;
				}
			}
			return result;
		}

		
		private void CheckAllyLog(int serverID, int unionID)
		{
			List<AllyLogData> list = AllyPersistence.Instance.DBAllyLogList(unionID);
			if (list != null && list.Count > 0)
			{
				ClientAgentManager.Instance().PostAsyncEvent(serverID, this._gameType, new AsyncDataItem(KuaFuEventTypes.AllyLog, new object[]
				{
					unionID,
					list
				}));
			}
		}

		
		public int UnionDataChange(int serverID, AllyData newData)
		{
			int result;
			if (!this.IsAgent(serverID))
			{
				result = -16;
			}
			else
			{
				lock (this._Mutex)
				{
					KFAllyData kfData = new KFAllyData();
					kfData.Copy(newData);
					kfData.LogTime = TimeUtil.NowDateTime();
					kfData.ServerID = serverID;
					kfData.UpdateLogtime();
					if (!AllyPersistence.Instance.DBUnionDataUpdate(kfData))
					{
						result = -17;
					}
					else
					{
						KFAllyData oldData;
						if (this._unionDic.TryGetValue(kfData.UnionID, out oldData))
						{
							this._unionDic[kfData.UnionID] = kfData;
						}
						else
						{
							this._unionDic.TryAdd(kfData.UnionID, kfData);
						}
						List<int> oldAllyIDList;
						if (this._allyDic.TryGetValue(kfData.UnionID, out oldAllyIDList))
						{
							AllyData myData = new AllyData();
							myData.Copy(kfData);
							myData.LogState = 12;
							foreach (int id in oldAllyIDList)
							{
								KFAllyData targetData = this.GetUnionData(id);
								if (this.IsAgent(targetData.ServerID))
								{
									ClientAgentManager.Instance().PostAsyncEvent(targetData.ServerID, this._gameType, new AsyncDataItem(KuaFuEventTypes.AllyUnionUpdate, new object[]
									{
										targetData.UnionID,
										myData
									}));
								}
							}
						}
						AllyData myData2 = new AllyData();
						myData2.Copy(kfData);
						myData2.LogState = 1;
						List<KFAllyData> rList = null;
						if (this._requestDic.TryGetValue(kfData.UnionID, out rList))
						{
							foreach (KFAllyData targetData in rList)
							{
								List<KFAllyData> raList = null;
								if (this.IsAgent(targetData.ServerID) && this._acceptDic.TryGetValue(targetData.UnionID, out raList))
								{
									KFAllyData oldAData = this.GetAcceptData(targetData.UnionID, kfData.UnionID);
									if (oldAData != null)
									{
										raList.Remove(oldAData);
										raList.Add(kfData);
										ClientAgentManager.Instance().PostAsyncEvent(targetData.ServerID, this._gameType, new AsyncDataItem(KuaFuEventTypes.AllyUnionUpdate, new object[]
										{
											targetData.UnionID,
											myData2
										}));
									}
								}
							}
						}
						List<KFAllyData> aList = null;
						if (this._acceptDic.TryGetValue(kfData.UnionID, out aList))
						{
							foreach (KFAllyData targetData in aList)
							{
								List<KFAllyData> arList = null;
								if (this.IsAgent(targetData.ServerID) && this._requestDic.TryGetValue(targetData.UnionID, out arList))
								{
									KFAllyData oldRData = this.GetRequestData(targetData.UnionID, kfData.UnionID);
									if (oldRData != null)
									{
										arList.Remove(oldRData);
										arList.Add(kfData);
										ClientAgentManager.Instance().PostAsyncEvent(targetData.ServerID, this._gameType, new AsyncDataItem(KuaFuEventTypes.AllyUnionUpdate, new object[]
										{
											targetData.UnionID,
											myData2
										}));
									}
								}
							}
						}
						result = 50;
					}
				}
			}
			return result;
		}

		
		public int UnionDel(int serverID, int unionID)
		{
			int result;
			if (!this.IsAgent(serverID))
			{
				result = -16;
			}
			else
			{
				lock (this._Mutex)
				{
					KFAllyData oldData;
					if (!this._unionDic.TryGetValue(unionID, out oldData))
					{
						result = -16;
					}
					else
					{
						List<int> oldAllyIDList;
						if (this._allyDic.TryGetValue(unionID, out oldAllyIDList))
						{
							foreach (int targetID in oldAllyIDList)
							{
								int state = this.AllyOperate(serverID, unionID, targetID, 3, true);
							}
							this._allyDic.TryRemove(unionID, out oldAllyIDList);
						}
						List<KFAllyData> rList = null;
						if (this._requestDic.TryGetValue(unionID, out rList))
						{
							foreach (KFAllyData targetData in rList)
							{
								int state = this.AllyOperate(serverID, unionID, targetData.UnionID, 4, true);
							}
							this._requestDic.TryRemove(unionID, out rList);
						}
						List<KFAllyData> aList = null;
						if (this._acceptDic.TryGetValue(unionID, out aList))
						{
							foreach (KFAllyData targetData in aList)
							{
								int state = this.AllyOperate(serverID, unionID, targetData.UnionID, 2, true);
							}
							this._acceptDic.TryRemove(unionID, out aList);
						}
						this._unionDic.TryRemove(unionID, out oldData);
						result = (AllyPersistence.Instance.DBUnionDataDel(unionID) ? 50 : -17);
					}
				}
			}
			return result;
		}

		
		public List<AllyData> AllyDataList(int serverID, int unionID, int type)
		{
			List<AllyData> resultlist = new List<AllyData>();
			List<AllyData> result;
			if (!this.IsAgent(serverID))
			{
				result = resultlist;
			}
			else
			{
				switch (type)
				{
				case 1:
					resultlist = this.AllyList(unionID);
					break;
				case 2:
					resultlist = this.AllyAcceptList(unionID);
					break;
				case 3:
					resultlist = this.AllyRequestList(unionID);
					break;
				}
				result = resultlist;
			}
			return result;
		}

		
		public List<int> InitAllyIDList(int unionID)
		{
			List<int> result;
			lock (this._Mutex)
			{
				List<int> allyIDList = null;
				if (this._allyDic.TryGetValue(unionID, out allyIDList))
				{
					result = allyIDList;
				}
				else
				{
					allyIDList = AllyPersistence.Instance.DBAllyIDList(unionID);
					this._allyDic.TryAdd(unionID, allyIDList);
					result = allyIDList;
				}
			}
			return result;
		}

		
		public List<AllyData> AllyList(int unionID)
		{
			List<AllyData> result;
			lock (this._Mutex)
			{
				List<int> allyIDList = this.InitAllyIDList(unionID);
				List<AllyData> list = new List<AllyData>();
				foreach (int id in allyIDList)
				{
					KFAllyData data = this.GetUnionData(id);
					if (data != null)
					{
						list.Add(new AllyData
						{
							UnionID = data.UnionID,
							UnionZoneID = data.UnionZoneID,
							UnionName = data.UnionName,
							UnionLevel = data.UnionLevel,
							UnionNum = data.UnionNum,
							LeaderID = data.LeaderID,
							LeaderZoneID = data.LeaderZoneID,
							LeaderName = data.LeaderName,
							LogState = 12
						});
					}
				}
				result = list;
			}
			return result;
		}

		
		private List<KFAllyData> InitAllyRequestList(int unionID)
		{
			List<KFAllyData> result;
			lock (this._Mutex)
			{
				List<KFAllyData> requestList = null;
				if (this._requestDic.TryGetValue(unionID, out requestList))
				{
					result = requestList;
				}
				else
				{
					requestList = AllyPersistence.Instance.DBAllyRequestList(unionID);
					this._requestDic.TryAdd(unionID, requestList);
					List<KFAllyData> list = new List<KFAllyData>();
					foreach (KFAllyData rData in requestList)
					{
						KFAllyData data = this.GetUnionData(rData.UnionID);
						if (data != null)
						{
							rData.UnionZoneID = data.UnionZoneID;
							rData.UnionName = data.UnionName;
							rData.UnionLevel = data.UnionLevel;
							rData.UnionNum = data.UnionNum;
							rData.LeaderID = data.LeaderID;
							rData.LeaderZoneID = data.LeaderZoneID;
							rData.LeaderName = data.LeaderName;
							rData.ServerID = data.ServerID;
						}
					}
					result = requestList;
				}
			}
			return result;
		}

		
		public List<AllyData> AllyRequestList(int unionID)
		{
			List<AllyData> result;
			lock (this._Mutex)
			{
				List<KFAllyData> requestList = this.InitAllyRequestList(unionID);
				List<AllyData> list = new List<AllyData>();
				foreach (KFAllyData rData in requestList)
				{
					AllyData d = new AllyData();
					d.Copy(rData);
					list.Add(d);
				}
				result = list;
			}
			return result;
		}

		
		public void AllyRequestAdd(int unionID, KFAllyData item)
		{
			lock (this._Mutex)
			{
				List<KFAllyData> list = this.InitAllyRequestList(unionID);
				list.Add(item);
			}
		}

		
		public List<KFAllyData> InitAllyAcceptList(int unionID)
		{
			List<KFAllyData> result;
			lock (this._Mutex)
			{
				List<KFAllyData> acceptList = null;
				if (this._acceptDic.TryGetValue(unionID, out acceptList))
				{
					result = acceptList;
				}
				else
				{
					acceptList = AllyPersistence.Instance.DBAllyAcceptList(unionID);
					this._acceptDic.TryAdd(unionID, acceptList);
					foreach (KFAllyData rData in acceptList)
					{
						KFAllyData data = this.GetUnionData(rData.UnionID);
						if (data != null)
						{
							rData.UnionZoneID = data.UnionZoneID;
							rData.UnionName = data.UnionName;
							rData.UnionLevel = data.UnionLevel;
							rData.UnionNum = data.UnionNum;
							rData.LeaderID = data.LeaderID;
							rData.LeaderZoneID = data.LeaderZoneID;
							rData.LeaderName = data.LeaderName;
							rData.ServerID = data.ServerID;
						}
					}
					result = acceptList;
				}
			}
			return result;
		}

		
		public List<AllyData> AllyAcceptList(int unionID)
		{
			List<AllyData> result;
			lock (this._Mutex)
			{
				List<KFAllyData> acceptList = this.InitAllyAcceptList(unionID);
				List<AllyData> list = new List<AllyData>();
				foreach (KFAllyData rData in acceptList)
				{
					AllyData d = new AllyData();
					d.Copy(rData);
					list.Add(d);
				}
				result = list;
			}
			return result;
		}

		
		public void AllyAcceptAdd(int unionID, KFAllyData item)
		{
			lock (this._Mutex)
			{
				List<KFAllyData> list = this.InitAllyAcceptList(unionID);
				list.Add(item);
			}
		}

		
		public AllyData AllyRequest(int serverID, int unionID, int zoneID, string unionName)
		{
			AllyData result;
			lock (this._Mutex)
			{
				AllyData clientRequest = new AllyData();
				if (!this.IsAgent(serverID))
				{
					clientRequest.LogState = -16;
					result = clientRequest;
				}
				else
				{
					KFAllyData targetData = this.GetUnionData(zoneID, unionName);
					if (targetData == null)
					{
						clientRequest.LogState = -3;
						result = clientRequest;
					}
					else
					{
						this.InitAllyIDList(targetData.UnionID);
						this.InitAllyRequestList(targetData.UnionID);
						this.InitAllyAcceptList(targetData.UnionID);
						if (this.UnionIsAlly(unionID, targetData.UnionID))
						{
							clientRequest.LogState = -9;
							result = clientRequest;
						}
						else if (this.UnionIsRequest(unionID, targetData.UnionID) || this.UnionIsAccept(targetData.UnionID, unionID))
						{
							clientRequest.LogState = -10;
							result = clientRequest;
						}
						else
						{
							int sum = this._allyDic[unionID].Count + this._requestDic[unionID].Count;
							if (sum >= Consts.AllyNumMax)
							{
								clientRequest.LogState = -11;
								result = clientRequest;
							}
							else
							{
								DateTime logTime = TimeUtil.NowDateTime();
								int logState = 1;
								if (!AllyPersistence.Instance.DBAllyRequestAdd(unionID, targetData.UnionID, logTime, logState))
								{
									clientRequest.LogState = -1;
									result = clientRequest;
								}
								else
								{
									KFAllyData myData = this.GetUnionData(unionID);
									AllyLogData logData = new AllyLogData();
									logData.UnionID = targetData.UnionID;
									logData.UnionZoneID = targetData.UnionZoneID;
									logData.UnionName = targetData.UnionName;
									logData.MyUnionID = unionID;
									logData.LogTime = logTime;
									logData.LogState = logState;
									ClientAgentManager.Instance().PostAsyncEvent(serverID, this._gameType, new AsyncDataItem(KuaFuEventTypes.AllyLog, new object[]
									{
										unionID,
										new List<AllyLogData>
										{
											logData
										}
									}));
									KFAllyData requestData = new KFAllyData();
									requestData.Copy(targetData);
									requestData.LogState = logState;
									requestData.LogTime = logTime;
									requestData.UpdateLogtime();
									this.AllyRequestAdd(unionID, requestData);
									clientRequest.Copy(requestData);
									KFAllyData acceptData = new KFAllyData();
									acceptData.Copy(myData);
									acceptData.LogState = logState;
									acceptData.LogTime = logTime;
									this.AllyAcceptAdd(targetData.UnionID, acceptData);
									if (this.IsAgent(targetData.ServerID))
									{
										AllyData clientAccept = new AllyData();
										clientAccept.Copy(acceptData);
										ClientAgentManager.Instance().PostAsyncEvent(targetData.ServerID, this._gameType, new AsyncDataItem(KuaFuEventTypes.AllyAccept, new object[]
										{
											targetData.UnionID,
											clientAccept
										}));
									}
									result = clientRequest;
								}
							}
						}
					}
				}
			}
			return result;
		}

		
		public int AllyOperate(int serverID, int unionID, int targetID, int operateType, bool isDel = false)
		{
			int result2;
			lock (this._Mutex)
			{
				int result = -17;
				KFAllyData targetData = this.GetUnionData(targetID);
				if (targetData == null)
				{
					result2 = -13;
				}
				else
				{
					this.InitAllyIDList(targetData.UnionID);
					this.InitAllyRequestList(targetData.UnionID);
					this.InitAllyAcceptList(targetData.UnionID);
					switch (operateType)
					{
					case 1:
						result = this.OperateAgree(serverID, unionID, targetID);
						break;
					case 2:
						result = this.OperateRefuse(serverID, unionID, targetID, isDel);
						break;
					case 3:
						result = this.OperateRemove(serverID, unionID, targetID, isDel);
						break;
					case 4:
						result = this.OperateCancel(serverID, unionID, targetID, isDel);
						break;
					}
					result2 = result;
				}
			}
			return result2;
		}

		
		public int OperateRemove(int serverID, int unionID, int targetID, bool isDel = false)
		{
			int result;
			lock (this._Mutex)
			{
				KFAllyData targetData = this.GetUnionData(targetID);
				if (targetData == null)
				{
					result = -13;
				}
				else if (!this.UnionIsAlly(unionID, targetData.UnionID))
				{
					result = -17;
				}
				else if (!this.UnionIsAlly(targetID, unionID))
				{
					result = -17;
				}
				else if (!AllyPersistence.Instance.DBAllyDel(unionID, targetID))
				{
					result = -17;
				}
				else
				{
					DateTime logTime = TimeUtil.NowDateTime();
					int logState = 41;
					if (!isDel)
					{
						this._allyDic[unionID].Remove(targetID);
					}
					AllyLogData logData = new AllyLogData();
					logData.UnionID = targetData.UnionID;
					logData.UnionZoneID = targetData.UnionZoneID;
					logData.UnionName = targetData.UnionName;
					logData.MyUnionID = unionID;
					logData.LogTime = logTime;
					logData.LogState = logState;
					ClientAgentManager.Instance().PostAsyncEvent(serverID, this._gameType, new AsyncDataItem(KuaFuEventTypes.AllyLog, new object[]
					{
						unionID,
						new List<AllyLogData>
						{
							logData
						}
					}));
					if (this._allyDic.ContainsKey(targetData.UnionID))
					{
						this._allyDic[targetData.UnionID].Remove(unionID);
					}
					KFAllyData myData = this.GetUnionData(unionID);
					logData = new AllyLogData();
					logData.UnionID = myData.UnionID;
					logData.UnionZoneID = myData.UnionZoneID;
					logData.UnionName = myData.UnionName;
					logData.MyUnionID = targetID;
					logData.LogTime = logTime;
					logData.LogState = 42;
					if (this.IsAgent(targetData.ServerID))
					{
						ClientAgentManager.Instance().PostAsyncEvent(targetData.ServerID, this._gameType, new AsyncDataItem(KuaFuEventTypes.AllyRemove, new object[]
						{
							targetID,
							unionID
						}));
						ClientAgentManager.Instance().PostAsyncEvent(targetData.ServerID, this._gameType, new AsyncDataItem(KuaFuEventTypes.AllyLog, new object[]
						{
							targetID,
							new List<AllyLogData>
							{
								logData
							}
						}));
					}
					else
					{
						AllyPersistence.Instance.DBAllyLogAdd(logData);
					}
					ClientAgentManager.Instance().KFBroadCastAsyncEvent(this._gameType, new AsyncDataItem(KuaFuEventTypes.KFAllyRemove, new object[]
					{
						targetID,
						unionID
					}));
					result = 41;
				}
			}
			return result;
		}

		
		public int OperateCancel(int serverID, int unionID, int targetID, bool isDel = false)
		{
			int result;
			lock (this._Mutex)
			{
				KFAllyData targetData = this.GetUnionData(targetID);
				if (targetData == null)
				{
					result = -13;
				}
				else if (!this.UnionIsRequest(unionID, targetID))
				{
					result = -17;
				}
				else if (!this.UnionIsAccept(targetID, unionID))
				{
					result = -17;
				}
				else if (!AllyPersistence.Instance.DBAllyRequestDel(unionID, targetID))
				{
					result = -17;
				}
				else
				{
					DateTime logTime = TimeUtil.NowDateTime();
					KFAllyData rData = this.GetRequestData(unionID, targetID);
					if (!isDel)
					{
						this._requestDic[unionID].Remove(rData);
					}
					ClientAgentManager.Instance().PostAsyncEvent(serverID, this._gameType, new AsyncDataItem(KuaFuEventTypes.AllyRequestRemove, new object[]
					{
						unionID,
						targetID
					}));
					if (this._acceptDic.ContainsKey(targetID))
					{
						KFAllyData aData = this.GetAcceptData(targetID, unionID);
						this._acceptDic[targetID].Remove(aData);
					}
					if (this.IsAgent(targetData.ServerID))
					{
						ClientAgentManager.Instance().PostAsyncEvent(targetData.ServerID, this._gameType, new AsyncDataItem(KuaFuEventTypes.AllyAcceptRemove, new object[]
						{
							targetID,
							unionID
						}));
					}
					result = 31;
				}
			}
			return result;
		}

		
		public int OperateAgree(int serverID, int unionID, int targetID)
		{
			int result;
			lock (this._Mutex)
			{
				KFAllyData targetData = this.GetUnionData(targetID);
				if (targetData == null)
				{
					result = -13;
				}
				else if (!this.UnionIsAccept(unionID, targetID))
				{
					result = -17;
				}
				else if (!this.UnionIsRequest(targetID, unionID))
				{
					result = -17;
				}
				else
				{
					int sum = this._allyDic[unionID].Count + this._requestDic[unionID].Count;
					if (sum >= Consts.AllyNumMax)
					{
						result = -11;
					}
					else
					{
						DateTime logTime = TimeUtil.NowDateTime();
						int logState = 12;
						if (!AllyPersistence.Instance.DBAllyRequestDel(targetID, unionID))
						{
							result = -17;
						}
						else if (!AllyPersistence.Instance.DBAllyAdd(unionID, targetID, logTime))
						{
							result = -17;
						}
						else
						{
							KFAllyData aData = this.GetAcceptData(unionID, targetID);
							this._acceptDic[unionID].Remove(aData);
							ClientAgentManager.Instance().PostAsyncEvent(serverID, this._gameType, new AsyncDataItem(KuaFuEventTypes.AllyAcceptRemove, new object[]
							{
								unionID,
								targetID
							}));
							this._allyDic[unionID].Add(targetID);
							aData.LogTime = logTime;
							aData.LogState = logState;
							AllyData clientMy = new AllyData();
							clientMy.Copy(aData);
							ClientAgentManager.Instance().PostAsyncEvent(serverID, this._gameType, new AsyncDataItem(KuaFuEventTypes.Ally, new object[]
							{
								unionID,
								clientMy,
								false
							}));
							if (this._requestDic.ContainsKey(targetID))
							{
								KFAllyData rData = this.GetRequestData(targetID, unionID);
								this._requestDic[targetID].Remove(rData);
							}
							if (this._allyDic.ContainsKey(targetID))
							{
								this._allyDic[targetID].Add(unionID);
							}
							KFAllyData myData = this.GetUnionData(unionID);
							myData.LogTime = logTime;
							myData.LogState = logState;
							AllyLogData logData = new AllyLogData();
							logData.UnionID = myData.UnionID;
							logData.UnionZoneID = myData.UnionZoneID;
							logData.UnionName = myData.UnionName;
							logData.MyUnionID = targetID;
							logData.LogTime = logTime;
							logData.LogState = 21;
							AllyData clientTarget = new AllyData();
							clientTarget.Copy(myData);
							if (this.IsAgent(targetData.ServerID))
							{
								ClientAgentManager.Instance().PostAsyncEvent(targetData.ServerID, this._gameType, new AsyncDataItem(KuaFuEventTypes.Ally, new object[]
								{
									targetID,
									clientTarget,
									true
								}));
								ClientAgentManager.Instance().PostAsyncEvent(targetData.ServerID, this._gameType, new AsyncDataItem(KuaFuEventTypes.AllyRequestRemove, new object[]
								{
									targetID,
									unionID
								}));
								ClientAgentManager.Instance().PostAsyncEvent(targetData.ServerID, this._gameType, new AsyncDataItem(KuaFuEventTypes.AllyLog, new object[]
								{
									targetID,
									new List<AllyLogData>
									{
										logData
									}
								}));
							}
							else
							{
								AllyPersistence.Instance.DBAllyLogAdd(logData);
							}
							ClientAgentManager.Instance().KFBroadCastAsyncEvent(this._gameType, new AsyncDataItem(KuaFuEventTypes.KFAlly, new object[]
							{
								clientMy,
								clientTarget
							}));
							result = 12;
						}
					}
				}
			}
			return result;
		}

		
		public int OperateRefuse(int serverID, int unionID, int targetID, bool isDel = false)
		{
			int result;
			lock (this._Mutex)
			{
				KFAllyData targetData = this.GetUnionData(targetID);
				if (targetData == null)
				{
					result = -13;
				}
				else if (!this.UnionIsAccept(unionID, targetID))
				{
					result = -17;
				}
				else if (!this.UnionIsRequest(targetID, unionID))
				{
					result = -17;
				}
				else if (!AllyPersistence.Instance.DBAllyRequestDel(targetID, unionID))
				{
					result = -17;
				}
				else
				{
					DateTime logTime = TimeUtil.NowDateTime();
					KFAllyData rData = this.GetAcceptData(unionID, targetID);
					if (!isDel)
					{
						this._acceptDic[unionID].Remove(rData);
					}
					ClientAgentManager.Instance().PostAsyncEvent(serverID, this._gameType, new AsyncDataItem(KuaFuEventTypes.AllyAcceptRemove, new object[]
					{
						unionID,
						targetID
					}));
					if (this._requestDic.ContainsKey(targetID))
					{
						KFAllyData aData = this.GetRequestData(targetID, unionID);
						this._requestDic[targetID].Remove(aData);
					}
					KFAllyData myData = this.GetUnionData(unionID);
					AllyLogData logData = new AllyLogData();
					logData.UnionID = myData.UnionID;
					logData.UnionZoneID = myData.UnionZoneID;
					logData.UnionName = myData.UnionName;
					logData.MyUnionID = targetID;
					logData.LogTime = logTime;
					logData.LogState = 20;
					if (this.IsAgent(targetData.ServerID))
					{
						ClientAgentManager.Instance().PostAsyncEvent(targetData.ServerID, this._gameType, new AsyncDataItem(KuaFuEventTypes.AllyRequestRemove, new object[]
						{
							targetID,
							unionID
						}));
						ClientAgentManager.Instance().PostAsyncEvent(targetData.ServerID, this._gameType, new AsyncDataItem(KuaFuEventTypes.AllyLog, new object[]
						{
							targetID,
							new List<AllyLogData>
							{
								logData
							}
						}));
					}
					else
					{
						AllyPersistence.Instance.DBAllyLogAdd(logData);
					}
					result = 11;
				}
			}
			return result;
		}

		
		private KFAllyData GetUnionData(int unionID)
		{
			KFAllyData result;
			lock (this._Mutex)
			{
				KFAllyData data = null;
				if (!this._unionDic.TryGetValue(unionID, out data))
				{
					data = AllyPersistence.Instance.DBUnionDataGet(unionID);
					if (data != null)
					{
						this._unionDic.TryAdd(unionID, data);
					}
				}
				result = data;
			}
			return result;
		}

		
		private KFAllyData GetUnionData(int unionZoneID, string unionName)
		{
			KFAllyData result;
			lock (this._Mutex)
			{
				IEnumerable<KFAllyData> data = from item in this._unionDic.Values
				where item.UnionZoneID == unionZoneID && item.UnionName == unionName
				select item;
				KFAllyData resultData;
				if (data.Any<KFAllyData>())
				{
					resultData = data.First<KFAllyData>();
				}
				else
				{
					resultData = AllyPersistence.Instance.DBUnionDataGet(unionZoneID, unionName);
					if (resultData != null)
					{
						this._unionDic.TryAdd(resultData.UnionID, resultData);
					}
				}
				result = resultData;
			}
			return result;
		}

		
		private bool UnionIsAlly(int unionID, int targetID)
		{
			bool result;
			lock (this._Mutex)
			{
				List<AllyData> list = this.AllyList(unionID);
				if (list != null && list.Count > 0)
				{
					AllyData resultData = list.Find((AllyData data) => data.UnionID == targetID);
					if (resultData != null)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		
		private bool UnionIsRequest(int unionID, int targetID)
		{
			bool result;
			lock (this._Mutex)
			{
				KFAllyData resultData = this.GetRequestData(unionID, targetID);
				if (resultData != null)
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		
		private KFAllyData GetRequestData(int unionID, int targetID)
		{
			KFAllyData result;
			lock (this._Mutex)
			{
				List<KFAllyData> list = this.InitAllyRequestList(unionID);
				if (list != null && list.Count > 0)
				{
					result = list.Find((KFAllyData data) => data.UnionID == targetID);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		
		private bool UnionIsAccept(int unionID, int targetID)
		{
			bool result;
			lock (this._Mutex)
			{
				KFAllyData resultData = this.GetAcceptData(unionID, targetID);
				if (resultData != null)
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		
		private KFAllyData GetAcceptData(int unionID, int targetID)
		{
			KFAllyData result;
			lock (this._Mutex)
			{
				List<KFAllyData> list = this.InitAllyAcceptList(unionID);
				if (list != null && list.Count > 0)
				{
					result = list.Find((KFAllyData data) => data.UnionID == targetID);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		
		public bool IsAgent(int serverID)
		{
			bool isAgent = ClientAgentManager.Instance().ExistAgent(serverID);
			if (!isAgent)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("UnionAlly时ServerId错误.ServerId:{0}", serverID), null, true);
			}
			return isAgent;
		}

		
		public long RankVersion(int serverID)
		{
			return RankService.getInstance().RankVersion(serverID);
		}

		
		public int RankGradeUpdate(int serverID, KFRankData rankData)
		{
			return RankService.getInstance().RankGradeUpdate(serverID, rankData);
		}

		
		public List<KFRankData> RankTopList(int serverID, int rankType)
		{
			return RankService.getInstance().RankTopList(serverID, rankType);
		}

		
		public KFRankData RankRole(int serverID, int rankType, int roleID)
		{
			return RankService.getInstance().RankRole(serverID, rankType, roleID);
		}

		
		private const double REQUEST_SECOND_CLEAR_SPAN = 30.0;

		
		private object _Mutex = new object();

		
		public readonly GameTypes _gameType = GameTypes.Ally;

		
		public readonly GameTypes EvItemGameType = GameTypes.TianTi;

		
		private ConcurrentDictionary<int, KFAllyData> _unionDic = new ConcurrentDictionary<int, KFAllyData>();

		
		private ConcurrentDictionary<int, List<int>> _allyDic = new ConcurrentDictionary<int, List<int>>();

		
		private ConcurrentDictionary<int, List<KFAllyData>> _requestDic = new ConcurrentDictionary<int, List<KFAllyData>>();

		
		private ConcurrentDictionary<int, List<KFAllyData>> _acceptDic = new ConcurrentDictionary<int, List<KFAllyData>>();

		
		public static AllyService Instance = null;

		
		public AllyPersistence _Persistence = AllyPersistence.Instance;

		
		public Thread _BackgroundThread;

		
		private DateTime _clearTimeRequest = DateTime.MinValue;
	}
}
