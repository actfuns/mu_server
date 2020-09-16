using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ServiceModel;
using GameServer.Core.Executor;
using GameServer.Logic;
using KF.Contract;
using KF.Contract.Data;
using KF.Contract.Interface;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.Interface;
using Tmsk.Contract.KuaFuData;

namespace KF.Client
{
	
	[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, IgnoreExtensionDataObject = true, UseSynchronizationContext = false)]
	public class AllyClient : MarshalByRefObject, IKuaFuClient, IManager2
	{
		
		public static AllyClient getInstance()
		{
			return AllyClient.instance;
		}

		
		public bool initialize(ICoreInterface coreInterface)
		{
			this._CoreInterface = coreInterface;
			this._ClientInfo.PTID = GameManager.PTID;
			this._ClientInfo.ServerId = GameManager.ServerId;
			this._ClientInfo.GameType = 14;
			return true;
		}

		
		public bool startup()
		{
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

		
		public void EventCallBackHandler(AsyncDataItem item)
		{
			try
			{
				int eventType = (int)item.EventType;
				object[] args = item.Args;
				lock (this._Mutex)
				{
					List<AllyData> list = null;
					switch (eventType)
					{
					case 10016:
						if (args.Length == 2)
						{
							List<AllyLogData> logList = (List<AllyLogData>)args[1];
							this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyLogGameEvent(logList), 10004);
						}
						break;
					case 10017:
						if (args.Length == 3)
						{
							int unionID = (int)args[0];
							AllyData target = (AllyData)args[1];
							bool isTipMsg = (bool)args[2];
							if (!this._allyDic.TryGetValue(unionID, out list))
							{
								list = new List<AllyData>();
								this._allyDic.TryAdd(unionID, list);
							}
							AllyData oldData = this.GetAllyData(unionID, target.UnionID);
							if (oldData == null)
							{
								list.Add(target);
								this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyGameEvent(unionID), 10004);
								if (isTipMsg)
								{
									this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyTipGameEvent(unionID, 14113), 10004);
								}
							}
						}
						break;
					case 10018:
						if (args.Length == 2)
						{
							int unionID = (int)args[0];
							int targetID = (int)args[1];
							AllyData data = this.GetAllyData(unionID, targetID);
							if (data != null && this._allyDic.TryGetValue(unionID, out list))
							{
								list.Remove(data);
								this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyTipGameEvent(unionID, 14113), 10004);
								this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyGameEvent(unionID), 10004);
							}
						}
						break;
					case 10019:
						if (args.Length == 2)
						{
							int unionID = (int)args[0];
							AllyData target = (AllyData)args[1];
							if (!this._acceptDic.TryGetValue(unionID, out list))
							{
								list = new List<AllyData>();
								this._acceptDic.TryAdd(unionID, list);
							}
							AllyData oldData = this.GetAcceptData(unionID, target.UnionID);
							if (oldData == null)
							{
								list.Add(target);
								this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyTipGameEvent(unionID, 14112), 10004);
							}
						}
						break;
					case 10020:
						if (args.Length == 2)
						{
							int unionID = (int)args[0];
							int targetID = (int)args[1];
							AllyData data = this.GetAcceptData(unionID, targetID);
							if (data != null && this._acceptDic.TryGetValue(unionID, out list))
							{
								list.Remove(data);
								this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyTipGameEvent(unionID, 14112), 10004);
							}
						}
						break;
					case 10021:
						if (args.Length == 2)
						{
							int unionID = (int)args[0];
							AllyData target = (AllyData)args[1];
							if (!this._requestDic.TryGetValue(unionID, out list))
							{
								list = new List<AllyData>();
								this._requestDic.TryAdd(unionID, list);
							}
							AllyData oldData = this.GetRequestData(unionID, target.UnionID);
							if (oldData == null)
							{
								list.Add(target);
							}
						}
						break;
					case 10022:
						if (args.Length == 2)
						{
							int unionID = (int)args[0];
							int targetID = (int)args[1];
							AllyData data = this.GetRequestData(unionID, targetID);
							if (data != null && this._requestDic.TryGetValue(unionID, out list))
							{
								list.Remove(data);
								this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyTipGameEvent(unionID, 14113), 10004);
							}
						}
						break;
					case 10023:
						if (args.Length == 2)
						{
							int unionID = (int)args[0];
							AllyData targetData = (AllyData)args[1];
							AllyData data = this.GetAllyData(unionID, targetData.UnionID);
							if (data != null && this._allyDic.TryGetValue(unionID, out list))
							{
								list.Remove(data);
								list.Add(targetData);
							}
							data = this.GetRequestData(unionID, targetData.UnionID);
							if (data != null && this._requestDic.TryGetValue(unionID, out list))
							{
								list.Remove(data);
								list.Add(data);
							}
							data = this.GetAcceptData(unionID, targetData.UnionID);
							if (data != null && this._acceptDic.TryGetValue(unionID, out list))
							{
								list.Remove(data);
								list.Add(data);
							}
						}
						break;
					case 10024:
						if (args.Length == 2)
						{
							AllyData unionData = (AllyData)args[0];
							AllyData targetData = (AllyData)args[1];
							if (!this._allyDic.TryGetValue(unionData.UnionID, out list))
							{
								list = new List<AllyData>();
								this._allyDic.TryAdd(unionData.UnionID, list);
							}
							AllyData oldData = this.GetAllyData(unionData.UnionID, targetData.UnionID);
							if (oldData == null)
							{
								list.Add(targetData);
								this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyGameEvent(unionData.UnionID), 10004);
							}
							List<AllyData> list2 = null;
							if (!this._allyDic.TryGetValue(targetData.UnionID, out list2))
							{
								list2 = new List<AllyData>();
								this._allyDic.TryAdd(targetData.UnionID, list2);
							}
							oldData = this.GetAllyData(targetData.UnionID, unionData.UnionID);
							if (oldData == null)
							{
								list2.Add(unionData);
								this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyGameEvent(targetData.UnionID), 10004);
							}
						}
						break;
					case 10025:
						if (args.Length == 2)
						{
							int unionID = (int)args[0];
							int targetID = (int)args[1];
							AllyData data = this.GetAllyData(unionID, targetID);
							if (data != null && this._allyDic.TryGetValue(unionID, out list))
							{
								list.Remove(data);
								this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyGameEvent(unionID), 10004);
							}
							AllyData data2 = this.GetAllyData(targetID, unionID);
							List<AllyData> list2 = null;
							if (data2 != null && this._allyDic.TryGetValue(targetID, out list2))
							{
								list2.Remove(data2);
								this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyGameEvent(targetID), 10004);
							}
						}
						break;
					case 10026:
						if (args.Length == 1)
						{
							KFRankData rankData = (KFRankData)args[0];
							lock (this._lockRank)
							{
								if (!this._rankDic.ContainsKey(rankData.RankType))
								{
									this._rankDic.Add(rankData.RankType, new Dictionary<int, KFRankData>());
								}
								Dictionary<int, KFRankData> dic = this._rankDic[rankData.RankType];
								if (dic.ContainsKey(rankData.RoleID))
								{
									dic[rankData.RoleID] = rankData;
								}
								else
								{
									dic.Add(rankData.RoleID, rankData);
								}
							}
						}
						break;
					case 10027:
						if (args.Length == 3)
						{
							int rankType = (int)args[0];
							int RankRefreshSpanType = (int)args[1];
							lock (this._lockRank)
							{
								List<KFRankData> topList = (List<KFRankData>)args[2];
								if (!this._rankTopDic.ContainsKey(rankType))
								{
									this._rankTopDic.Add(rankType, new List<KFRankData>());
								}
								this._rankTopDic[rankType] = topList;
								if (1 == RankRefreshSpanType && this._rankDic.ContainsKey(rankType))
								{
									this._rankDic[rankType].Clear();
								}
							}
						}
						break;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		
		public object GetDataFromClientServer(int dataType, params object[] args)
		{
			return null;
		}

		
		public int GetNewFuBenSeqId()
		{
			return 0;
		}

		
		public int UpdateRoleData(KuaFuRoleData kuaFuRoleData, int roleId = 0)
		{
			return 0;
		}

		
		public int OnRoleChangeState(int roleId, int state, int age)
		{
			return 0;
		}

		
		public void TimerProc(object sender, EventArgs e)
		{
			try
			{
				string uri = this._CoreInterface.GetRuntimeVariable("AllyUri", null);
				if (this._RemoteServiceUri != uri)
				{
					this._RemoteServiceUri = uri;
				}
				IAllyService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					if (this._ClientInfo.ClientId > 0)
					{
						AsyncDataItem[] items = kuaFuService.GetClientCacheItems(this._ClientInfo.ServerId);
						if (items != null && items.Length > 0)
						{
							this.ExecuteEventCallBackAsync(items);
						}
					}
				}
				DateTime now = TimeUtil.NowDateTime();
				if (now > this._versionTime)
				{
					this._versionTime = now.AddSeconds(30.0);
					if (!this.VersionIsEqual())
					{
						this._unionDic.Clear();
						this._allyDic.Clear();
						this._requestDic.Clear();
						this._acceptDic.Clear();
						this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyStartGameEvent(), 10004);
					}
				}
			}
			catch (Exception ex)
			{
				this.ResetKuaFuService();
			}
		}

		
		public void ExecuteEventCallBackAsync(object state)
		{
			AsyncDataItem[] items = state as AsyncDataItem[];
			if (items != null && items.Length > 0)
			{
				foreach (AsyncDataItem item in items)
				{
					this.EventCallBackHandler(item);
				}
			}
		}

		
		private IAllyService GetKuaFuService(bool noWait = false)
		{
			try
			{
				if (KuaFuManager.KuaFuWorldKuaFuGameServer)
				{
					return null;
				}
				lock (this._Mutex)
				{
					if (string.IsNullOrEmpty(this._RemoteServiceUri))
					{
						return null;
					}
					if (this._KuaFuService == null && noWait)
					{
						return null;
					}
				}
				lock (this._Mutex)
				{
					IAllyService kuaFuService;
					if (this._KuaFuService == null)
					{
						kuaFuService = (IAllyService)Activator.GetObject(typeof(IAllyService), this._RemoteServiceUri);
						if (null == kuaFuService)
						{
							return null;
						}
					}
					else
					{
						kuaFuService = this._KuaFuService;
					}
					int clientId = this._ClientInfo.ClientId;
					long nowTicks = TimeUtil.NOW();
					if (this._ClientInfo.ClientId <= 0 || Math.Abs(nowTicks - this._ClientInfo.LastInitClientTicks) > 12000L)
					{
						this._ClientInfo.LastInitClientTicks = nowTicks;
						clientId = kuaFuService.InitializeClient(this._ClientInfo);
					}
					if (kuaFuService != null && (clientId != this._ClientInfo.ClientId || this._KuaFuService != kuaFuService))
					{
						lock (this._Mutex)
						{
							if (clientId > 0)
							{
								this._KuaFuService = kuaFuService;
							}
							else
							{
								this._KuaFuService = null;
							}
							this._ClientInfo.ClientId = clientId;
							return kuaFuService;
						}
					}
					return this._KuaFuService;
				}
			}
			catch (Exception ex)
			{
				this.ResetKuaFuService();
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return null;
		}

		
		private void CloseConnection()
		{
			this._ClientInfo.ClientId = 0;
			this._RemoteServiceUri = this._CoreInterface.GetRuntimeVariable("AllyUri", null);
			lock (this._Mutex)
			{
				this._KuaFuService = null;
			}
		}

		
		private void OnConnectionClose(object sender, EventArgs e)
		{
			this.CloseConnection();
		}

		
		private void ResetKuaFuService()
		{
			this.CloseConnection();
		}

		
		private bool VersionIsEqual()
		{
			bool result;
			lock (this._Mutex)
			{
				IAllyService kuaFuService = this.GetKuaFuService(true);
				if (null == kuaFuService)
				{
					result = false;
				}
				else
				{
					long oldVersion = this._unionAllyVersion;
					this._unionAllyVersion = kuaFuService.UnionAllyVersion(this._ClientInfo.ServerId);
					result = (this._unionAllyVersion == oldVersion && this._unionAllyVersion > 0L);
				}
			}
			return result;
		}

		
		public EAlly HUnionAllyInit(int unionID, bool isKF)
		{
			EAlly result = EAlly.EFail;
			try
			{
				lock (this._Mutex)
				{
					IAllyService kuaFuService = this.GetKuaFuService(true);
					if (null == kuaFuService)
					{
						return result;
					}
					DateTime oldTime;
					if (this._unionDic.TryGetValue(unionID, out oldTime))
					{
						this._unionDic[unionID] = TimeUtil.NowDateTime();
						return EAlly.Succ;
					}
					try
					{
						result = (EAlly)kuaFuService.UnionAllyInit(this._ClientInfo.ServerId, unionID, isKF);
						if (result == EAlly.Succ)
						{
							this._unionDic.TryAdd(unionID, TimeUtil.NowDateTime());
							this.HAllyDataList(unionID, EAllyDataType.Ally);
							this.HAllyDataList(unionID, EAllyDataType.Request);
							this.HAllyDataList(unionID, EAllyDataType.Accept);
						}
					}
					catch (Exception ex)
					{
						this.ResetKuaFuService();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return result;
		}

		
		public EAlly HUnionDel(int unionID)
		{
			EAlly result = EAlly.EFail;
			try
			{
				lock (this._Mutex)
				{
					IAllyService kuaFuService = this.GetKuaFuService(false);
					if (null == kuaFuService)
					{
						return result;
					}
					try
					{
						result = (EAlly)kuaFuService.UnionDel(this._ClientInfo.ServerId, unionID);
						if (result == EAlly.Succ)
						{
							DateTime oldTime;
							this._unionDic.TryRemove(unionID, out oldTime);
							List<AllyData> list;
							this._allyDic.TryRemove(unionID, out list);
							this._requestDic.TryRemove(unionID, out list);
							this._acceptDic.TryRemove(unionID, out list);
						}
					}
					catch (Exception ex)
					{
						this.ResetKuaFuService();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return result;
		}

		
		public EAlly HUnionDataChange(AllyData unionData)
		{
			EAlly result = EAlly.EFail;
			try
			{
				lock (this._Mutex)
				{
					IAllyService kuaFuService = this.GetKuaFuService(false);
					if (null == kuaFuService)
					{
						return result;
					}
					try
					{
						result = (EAlly)kuaFuService.UnionDataChange(this._ClientInfo.ServerId, unionData);
						if (result == EAlly.Succ)
						{
							int unionID = unionData.UnionID;
							DateTime oldTime;
							if (this._unionDic.TryGetValue(unionID, out oldTime))
							{
								this._unionDic[unionID] = TimeUtil.NowDateTime();
								return EAlly.Succ;
							}
							this._unionDic.TryAdd(unionID, TimeUtil.NowDateTime());
							this.HAllyDataList(unionID, EAllyDataType.Ally);
							this.HAllyDataList(unionID, EAllyDataType.Request);
							this.HAllyDataList(unionID, EAllyDataType.Accept);
						}
					}
					catch (Exception ex)
					{
						this.ResetKuaFuService();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return result;
		}

		
		public List<AllyData> HAllyDataList(int unionID, EAllyDataType type)
		{
			List<AllyData> list = new List<AllyData>();
			lock (this._Mutex)
			{
				ConcurrentDictionary<int, List<AllyData>> dic = null;
				switch (type)
				{
				case EAllyDataType.Ally:
					dic = this._allyDic;
					break;
				case EAllyDataType.Accept:
					dic = this._acceptDic;
					break;
				case EAllyDataType.Request:
					dic = this._requestDic;
					break;
				}
				if (dic.TryGetValue(unionID, out list))
				{
					return list;
				}
				IAllyService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return list;
				}
				try
				{
					list = kuaFuService.AllyDataList(this._ClientInfo.ServerId, unionID, (int)type);
					if (list != null)
					{
						dic.TryAdd(unionID, list);
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return list;
		}

		
		public EAlly HAllyOperate(int unionID, int targetID, EAllyOperate operateType)
		{
			EAlly result = EAlly.EFail;
			try
			{
				lock (this._Mutex)
				{
					if (!this.VersionIsEqual())
					{
						this._unionDic.Clear();
						this._allyDic.Clear();
						this._requestDic.Clear();
						this._acceptDic.Clear();
						this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyStartGameEvent(), 10004);
						return result;
					}
					ConcurrentDictionary<int, List<AllyData>> dic = null;
					switch (operateType)
					{
					case EAllyOperate.Agree:
					case EAllyOperate.Refuse:
						dic = this._acceptDic;
						break;
					case EAllyOperate.Remove:
						dic = this._allyDic;
						break;
					case EAllyOperate.Cancel:
						dic = this._requestDic;
						break;
					}
					List<AllyData> list = null;
					if (!dic.TryGetValue(unionID, out list))
					{
						return EAlly.ENoTargetUnion;
					}
					AllyData targetData = dic[unionID].Find((AllyData data) => data.UnionID == targetID);
					if (targetData == null)
					{
						return EAlly.ENoTargetUnion;
					}
					IAllyService kuaFuService = this.GetKuaFuService(false);
					if (null == kuaFuService)
					{
						return EAlly.EServer;
					}
					try
					{
						result = (EAlly)kuaFuService.AllyOperate(this._ClientInfo.ServerId, unionID, targetID, (int)operateType, false);
						if (result == EAlly.AllyAgree || result == EAlly.AllyRefuse || result == EAlly.AllyCancelSucc || result == EAlly.AllyRemoveSucc)
						{
							dic[unionID].Remove(targetData);
						}
					}
					catch (Exception ex)
					{
						this.ResetKuaFuService();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return result;
		}

		
		public EAlly HAllyRequest(int unionID, int zoneID, string unionName)
		{
			try
			{
				lock (this._Mutex)
				{
					if (!this.VersionIsEqual())
					{
						this._unionDic.Clear();
						this._allyDic.Clear();
						this._requestDic.Clear();
						this._acceptDic.Clear();
						this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyStartGameEvent(), 10004);
						return EAlly.EFail;
					}
					IAllyService kuaFuService = this.GetKuaFuService(false);
					if (null == kuaFuService)
					{
						return EAlly.EServer;
					}
					try
					{
						AllyData d = kuaFuService.AllyRequest(this._ClientInfo.ServerId, unionID, zoneID, unionName);
						if (d.LogState == 1)
						{
							List<AllyData> list = null;
							if (!this._requestDic.TryGetValue(unionID, out list))
							{
								list = new List<AllyData>();
								this._requestDic.TryAdd(unionID, list);
							}
							list.Add(d);
							return (EAlly)d.LogState;
						}
					}
					catch (Exception ex)
					{
						this.ResetKuaFuService();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return EAlly.EAllyRequest;
		}

		
		public bool UnionIsAlly(int unionID, int zoneID, string unionName)
		{
			bool result;
			lock (this._Mutex)
			{
				List<AllyData> list = null;
				if (this._allyDic.TryGetValue(unionID, out list))
				{
					AllyData resultData = list.Find((AllyData data) => data.UnionZoneID == zoneID && data.UnionName == unionName);
					if (resultData != null)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		
		public bool UnionIsRequest(int unionID, int zoneID, string unionName)
		{
			bool result;
			lock (this._Mutex)
			{
				List<AllyData> list = null;
				if (this._requestDic.TryGetValue(unionID, out list))
				{
					AllyData resultData = list.Find((AllyData data) => data.UnionZoneID == zoneID && data.UnionName == unionName);
					if (resultData != null)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		
		public bool UnionIsAccept(int unionID, int zoneID, string unionName)
		{
			bool result;
			lock (this._Mutex)
			{
				List<AllyData> list = null;
				if (this._acceptDic.TryGetValue(unionID, out list))
				{
					AllyData resultData = list.Find((AllyData data) => data.UnionZoneID == zoneID && data.UnionName == unionName);
					if (resultData != null)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		
		public int AllyCount(int unionID)
		{
			int result;
			lock (this._Mutex)
			{
				if (this._allyDic.ContainsKey(unionID))
				{
					result = this._allyDic[unionID].Count;
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		
		public int AllyRequestCount(int unionID)
		{
			int result;
			lock (this._Mutex)
			{
				if (this._requestDic.ContainsKey(unionID))
				{
					result = this._requestDic[unionID].Count;
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		
		private AllyData GetAllyData(int unionID, int targetID)
		{
			AllyData result;
			lock (this._Mutex)
			{
				List<AllyData> list = null;
				if (this._allyDic.TryGetValue(unionID, out list))
				{
					result = list.Find((AllyData data) => data.UnionID == targetID);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		
		private AllyData GetRequestData(int unionID, int targetID)
		{
			AllyData result;
			lock (this._Mutex)
			{
				List<AllyData> list = null;
				if (this._requestDic.TryGetValue(unionID, out list))
				{
					result = list.Find((AllyData data) => data.UnionID == targetID);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		
		private AllyData GetAcceptData(int unionID, int targetID)
		{
			AllyData result;
			lock (this._Mutex)
			{
				List<AllyData> list = null;
				if (this._acceptDic.TryGetValue(unionID, out list))
				{
					result = list.Find((AllyData data) => data.UnionID == targetID);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		
		
		private object _lockRank
		{
			get
			{
				return this._Mutex;
			}
		}

		
		public void HRankClear()
		{
			this._rankTopDic = new Dictionary<int, List<KFRankData>>();
			this._rankDic = new Dictionary<int, Dictionary<int, KFRankData>>();
		}

		
		public KFRankData HRankData(int rankType, int roleID)
		{
			KFRankData result;
			lock (this._lockRank)
			{
				if (this._rankTopDic.ContainsKey(rankType))
				{
					List<KFRankData> list = this._rankTopDic[rankType];
					KFRankData data = list.Find((KFRankData _x) => _x != null && _x.RoleID == roleID);
					if (data != null)
					{
						return data;
					}
				}
				if (this._rankDic.ContainsKey(rankType))
				{
					Dictionary<int, KFRankData> dic = this._rankDic[rankType];
					if (dic.ContainsKey(roleID))
					{
						return dic[roleID];
					}
				}
				else
				{
					this._rankDic.Add(rankType, new Dictionary<int, KFRankData>());
				}
				IAllyService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					result = null;
				}
				else
				{
					try
					{
						KFRankData data = kuaFuService.RankRole(this._ClientInfo.ServerId, rankType, roleID);
						if (data != null)
						{
							this._rankDic[rankType].Add(roleID, data);
						}
						return data;
					}
					catch (Exception ex)
					{
						this.ResetKuaFuService();
					}
					result = null;
				}
			}
			return result;
		}

		
		public List<KFRankData> HRankTopList(int rankType)
		{
			List<KFRankData> list = new List<KFRankData>();
			lock (this._lockRank)
			{
				if (this._rankTopDic.TryGetValue(rankType, out list))
				{
					return list;
				}
				IAllyService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return list;
				}
				try
				{
					list = kuaFuService.RankTopList(this._ClientInfo.ServerId, rankType);
					if (list != null)
					{
						this._rankTopDic.Add(rankType, list);
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					LogManager.WriteLog(LogTypes.Error, "HRankList Error{0}", ex, true);
				}
			}
			return list;
		}

		
		public KFRankData HRankUpdate(int rankType, int grade, int roleID, int zoneID, string roleName, byte[] roleData)
		{
			KFRankData result;
			lock (this._lockRank)
			{
				KFRankData myData = new KFRankData();
				myData.RankType = rankType;
				myData.Rank = -1;
				myData.ZoneID = zoneID;
				myData.RoleID = roleID;
				myData.RoleName = roleName;
				myData.Grade = grade;
				myData.RoleData = roleData;
				myData.RankTime = TimeUtil.NowDateTime();
				myData.ServerID = this._ClientInfo.ServerId;
				IAllyService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					result = null;
				}
				else
				{
					try
					{
						kuaFuService.RankGradeUpdate(this._ClientInfo.ServerId, myData);
					}
					catch (Exception ex)
					{
						this.ResetKuaFuService();
					}
					result = null;
				}
			}
			return result;
		}

		
		public const int _gameType = 14;

		
		public const int _sceneType = 10004;

		
		private const int ALLY_VERSION_SPAN_SECOND = 30;

		
		private static AllyClient instance = new AllyClient();

		
		public object _Mutex = new object();

		
		private ICoreInterface _CoreInterface = null;

		
		private IAllyService _KuaFuService = null;

		
		private KuaFuClientContext _ClientInfo = new KuaFuClientContext();

		
		private string _RemoteServiceUri = null;

		
		private DateTime _versionTime = DateTime.MinValue;

		
		private long _unionAllyVersion = 0L;

		
		private ConcurrentDictionary<int, DateTime> _unionDic = new ConcurrentDictionary<int, DateTime>();

		
		private ConcurrentDictionary<int, List<AllyData>> _allyDic = new ConcurrentDictionary<int, List<AllyData>>();

		
		private ConcurrentDictionary<int, List<AllyData>> _requestDic = new ConcurrentDictionary<int, List<AllyData>>();

		
		private ConcurrentDictionary<int, List<AllyData>> _acceptDic = new ConcurrentDictionary<int, List<AllyData>>();

		
		private DuplexChannelFactory<IAllyService> channelFactory;

		
		public Dictionary<int, List<KFRankData>> _rankTopDic = new Dictionary<int, List<KFRankData>>();

		
		public Dictionary<int, Dictionary<int, KFRankData>> _rankDic = new Dictionary<int, Dictionary<int, KFRankData>>();
	}
}
