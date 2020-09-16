using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using GameServer.Core.Executor;
using GameServer.Logic;
using KF.Contract;
using KF.Contract.Data;
using KF.Contract.Interface;
using Server.Tools;
using Tmsk.Contract;

namespace KF.Client
{
	
	[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, IgnoreExtensionDataObject = true, UseSynchronizationContext = false)]
	public class SpreadClient : MarshalByRefObject, IKuaFuClient, IManager2
	{
		
		public static SpreadClient getInstance()
		{
			return SpreadClient.instance;
		}

		
		public bool initialize()
		{
			return true;
		}

		
		public bool initialize(ICoreInterface coreInterface)
		{
			this._CoreInterface = coreInterface;
			this._ClientInfo.PTID = GameManager.PTID;
			this._ClientInfo.ServerId = GameManager.ServerId;
			this._ClientInfo.GameType = 9;
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

		
		public void TimerProc(object sender, EventArgs e)
		{
			try
			{
				string uri = this._CoreInterface.GetRuntimeVariable("SpreadUri", null);
				if (this._RemoteServiceUri != uri)
				{
					this._RemoteServiceUri = uri;
				}
				ISpreadService kuaFuService = this.GetKuaFuService(false);
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
				this.CheckSpreadData();
			}
			catch (Exception ex)
			{
				this.ResetKuaFuService();
			}
		}

		
		private void CheckSpreadData()
		{
			if (this._RoleId2KFSpreadDataDict != null && this._RoleId2KFSpreadDataDict.Count > 0)
			{
				if (!(TimeUtil.NowDateTime() < this._checkSpreadDataTime))
				{
					this._checkSpreadDataTime = TimeUtil.NowDateTime().AddSeconds(43200.0);
					lock (this._RoleId2KFSpreadDataDict)
					{
						List<int> roleList = (from info in this._RoleId2KFSpreadDataDict.Values
						where info.LogTime <= TimeUtil.NowDateTime().AddSeconds(-43200.0)
						select info.RoleID).ToList<int>();
						foreach (int t in roleList)
						{
							KFSpreadData d;
							this._RoleId2KFSpreadDataDict.TryRemove(t, out d);
						}
					}
				}
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

		
		private void CloseConnection()
		{
			this._ClientInfo.ClientId = 0;
			this._RemoteServiceUri = this._CoreInterface.GetRuntimeVariable("SpreadUri", null);
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

		
		private ISpreadService GetKuaFuService(bool noWait = false)
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
				lock (this._RemotingMutex)
				{
					ISpreadService kuaFuService;
					if (this._KuaFuService == null)
					{
						kuaFuService = (ISpreadService)Activator.GetObject(typeof(ISpreadService), this._RemoteServiceUri);
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
					if (clientId <= 0 || Math.Abs(nowTicks - this._ClientInfo.LastInitClientTicks) > 12000L)
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

		
		public void EventCallBackHandler(AsyncDataItem item)
		{
			try
			{
				int eventType = (int)item.EventType;
				object[] args = item.Args;
				int num = eventType;
				if (num == 10008)
				{
					if (args.Length == 5)
					{
						int zoneID = (int)args[0];
						int roleID = (int)args[1];
						int countRole = (int)args[2];
						int countVip = (int)args[3];
						int countLevle = (int)args[4];
						KFSpreadData data;
						if (this._RoleId2KFSpreadDataDict.TryGetValue(roleID, out data))
						{
							lock (data)
							{
								data.CountRole = countRole;
								data.CountVip = countVip;
								data.CountLevel = countLevle;
								data.UpdateLogtime();
								this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifySpreadCountGameEvent(zoneID, roleID, countRole, countVip, countLevle), 10002);
							}
						}
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

		
		public int SpreadSign(int zoneID, int roleID)
		{
			try
			{
				lock (this._Mutex)
				{
					KFSpreadData data;
					if (this._RoleId2KFSpreadDataDict.TryGetValue(roleID, out data))
					{
						return 0;
					}
					ISpreadService kuaFuService = this.GetKuaFuService(false);
					if (null == kuaFuService)
					{
						return -11001;
					}
					try
					{
						int result = kuaFuService.SpreadSign(this._ClientInfo.ServerId, zoneID, roleID);
						if (result > 0)
						{
							KFSpreadData newData = new KFSpreadData();
							newData.ServerID = this._ClientInfo.ServerId;
							newData.ZoneID = zoneID;
							newData.RoleID = roleID;
							this._RoleId2KFSpreadDataDict.TryAdd(roleID, newData);
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
			return 1;
		}

		
		public int[] SpreadCount(int zoneID, int roleID)
		{
			int[] array = new int[3];
			int[] counts = array;
			lock (this._Mutex)
			{
				KFSpreadData data;
				if (this._RoleId2KFSpreadDataDict.TryGetValue(roleID, out data))
				{
					data.UpdateLogtime();
					return new int[]
					{
						data.CountRole,
						data.CountVip,
						data.CountLevel
					};
				}
				ISpreadService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return counts;
				}
				try
				{
					counts = kuaFuService.SpreadCount(this._ClientInfo.ServerId, zoneID, roleID);
					if (counts != null && counts.Length == 3)
					{
						KFSpreadData newData = new KFSpreadData();
						newData.ServerID = this._ClientInfo.ServerId;
						newData.ZoneID = zoneID;
						newData.RoleID = roleID;
						newData.CountRole = counts[0];
						newData.CountVip = counts[1];
						newData.CountLevel = counts[2];
						this._RoleId2KFSpreadDataDict.TryAdd(roleID, newData);
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return counts;
		}

		
		public int CheckVerifyCode(string cuserID, int czoneID, int croleID, int pzoneID, int proleID, int isVip, int isLevel)
		{
			int result = -1;
			try
			{
				ISpreadService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						result = kuaFuService.CheckVerifyCode(this._ClientInfo.ServerId, cuserID, czoneID, croleID, pzoneID, proleID, isVip, isLevel);
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

		
		public int TelCodeGet(int czoneID, int croleID, string tel)
		{
			int result = -1;
			try
			{
				ISpreadService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						result = kuaFuService.TelCodeGet(this._ClientInfo.ServerId, czoneID, croleID, tel);
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

		
		public int TelCodeVerify(int czoneID, int croleID, int telCode)
		{
			int result = -1;
			try
			{
				ISpreadService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						result = kuaFuService.TelCodeVerify(this._ClientInfo.ServerId, czoneID, croleID, telCode);
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

		
		public bool SpreadLevel(int pzoneID, int proleID, int czoneID, int croleID)
		{
			bool result = false;
			try
			{
				ISpreadService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						result = kuaFuService.SpreadLevel(pzoneID, proleID, czoneID, croleID);
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

		
		public bool SpreadVip(int pzoneID, int proleID, int czoneID, int croleID)
		{
			bool result = false;
			try
			{
				ISpreadService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						result = kuaFuService.SpreadVip(pzoneID, proleID, czoneID, croleID);
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

		
		public const int _gameType = 9;

		
		public const int _sceneType = 10002;

		
		private const int CHECK_SPREAD_DATA_SECOND = 43200;

		
		private static SpreadClient instance = new SpreadClient();

		
		private object _Mutex = new object();

		
		private object _RemotingMutex = new object();

		
		private ICoreInterface _CoreInterface = null;

		
		private ISpreadService _KuaFuService = null;

		
		private bool _ClientInitialized = false;

		
		private KuaFuClientContext _ClientInfo = new KuaFuClientContext();

		
		private ConcurrentDictionary<int, KFSpreadData> _RoleId2KFSpreadDataDict = new ConcurrentDictionary<int, KFSpreadData>();

		
		private string _RemoteServiceUri = null;

		
		private DateTime _checkSpreadDataTime = DateTime.MinValue;

		
		private DuplexChannelFactory<ISpreadService> channelFactory;
	}
}
