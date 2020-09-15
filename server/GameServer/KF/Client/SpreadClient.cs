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
	// Token: 0x0200041A RID: 1050
	[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, IgnoreExtensionDataObject = true, UseSynchronizationContext = false)]
	public class SpreadClient : MarshalByRefObject, IKuaFuClient, IManager2
	{
		// Token: 0x06001294 RID: 4756 RVA: 0x001280BC File Offset: 0x001262BC
		public static SpreadClient getInstance()
		{
			return SpreadClient.instance;
		}

		// Token: 0x06001295 RID: 4757 RVA: 0x001280D4 File Offset: 0x001262D4
		public bool initialize()
		{
			return true;
		}

		// Token: 0x06001296 RID: 4758 RVA: 0x001280E8 File Offset: 0x001262E8
		public bool initialize(ICoreInterface coreInterface)
		{
			this._CoreInterface = coreInterface;
			this._ClientInfo.PTID = GameManager.PTID;
			this._ClientInfo.ServerId = GameManager.ServerId;
			this._ClientInfo.GameType = 9;
			return true;
		}

		// Token: 0x06001297 RID: 4759 RVA: 0x00128130 File Offset: 0x00126330
		public bool startup()
		{
			return true;
		}

		// Token: 0x06001298 RID: 4760 RVA: 0x00128144 File Offset: 0x00126344
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06001299 RID: 4761 RVA: 0x00128158 File Offset: 0x00126358
		public bool destroy()
		{
			return true;
		}

		// Token: 0x0600129A RID: 4762 RVA: 0x0012816C File Offset: 0x0012636C
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

		// Token: 0x0600129B RID: 4763 RVA: 0x00128280 File Offset: 0x00126480
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

		// Token: 0x0600129C RID: 4764 RVA: 0x001283D4 File Offset: 0x001265D4
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

		// Token: 0x0600129D RID: 4765 RVA: 0x00128428 File Offset: 0x00126628
		private void CloseConnection()
		{
			this._ClientInfo.ClientId = 0;
			this._RemoteServiceUri = this._CoreInterface.GetRuntimeVariable("SpreadUri", null);
			lock (this._Mutex)
			{
				this._KuaFuService = null;
			}
		}

		// Token: 0x0600129E RID: 4766 RVA: 0x00128498 File Offset: 0x00126698
		private void OnConnectionClose(object sender, EventArgs e)
		{
			this.CloseConnection();
		}

		// Token: 0x0600129F RID: 4767 RVA: 0x001284A2 File Offset: 0x001266A2
		private void ResetKuaFuService()
		{
			this.CloseConnection();
		}

		// Token: 0x060012A0 RID: 4768 RVA: 0x001284AC File Offset: 0x001266AC
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

		// Token: 0x060012A1 RID: 4769 RVA: 0x00128724 File Offset: 0x00126924
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

		// Token: 0x060012A2 RID: 4770 RVA: 0x00128858 File Offset: 0x00126A58
		public object GetDataFromClientServer(int dataType, params object[] args)
		{
			return null;
		}

		// Token: 0x060012A3 RID: 4771 RVA: 0x0012886C File Offset: 0x00126A6C
		public int GetNewFuBenSeqId()
		{
			return 0;
		}

		// Token: 0x060012A4 RID: 4772 RVA: 0x00128880 File Offset: 0x00126A80
		public int UpdateRoleData(KuaFuRoleData kuaFuRoleData, int roleId = 0)
		{
			return 0;
		}

		// Token: 0x060012A5 RID: 4773 RVA: 0x00128894 File Offset: 0x00126A94
		public int OnRoleChangeState(int roleId, int state, int age)
		{
			return 0;
		}

		// Token: 0x060012A6 RID: 4774 RVA: 0x001288A8 File Offset: 0x00126AA8
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

		// Token: 0x060012A7 RID: 4775 RVA: 0x001289D4 File Offset: 0x00126BD4
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

		// Token: 0x060012A8 RID: 4776 RVA: 0x00128B50 File Offset: 0x00126D50
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

		// Token: 0x060012A9 RID: 4777 RVA: 0x00128BDC File Offset: 0x00126DDC
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

		// Token: 0x060012AA RID: 4778 RVA: 0x00128C60 File Offset: 0x00126E60
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

		// Token: 0x060012AB RID: 4779 RVA: 0x00128CE4 File Offset: 0x00126EE4
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

		// Token: 0x060012AC RID: 4780 RVA: 0x00128D5C File Offset: 0x00126F5C
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

		// Token: 0x04001C01 RID: 7169
		public const int _gameType = 9;

		// Token: 0x04001C02 RID: 7170
		public const int _sceneType = 10002;

		// Token: 0x04001C03 RID: 7171
		private const int CHECK_SPREAD_DATA_SECOND = 43200;

		// Token: 0x04001C04 RID: 7172
		private static SpreadClient instance = new SpreadClient();

		// Token: 0x04001C05 RID: 7173
		private object _Mutex = new object();

		// Token: 0x04001C06 RID: 7174
		private object _RemotingMutex = new object();

		// Token: 0x04001C07 RID: 7175
		private ICoreInterface _CoreInterface = null;

		// Token: 0x04001C08 RID: 7176
		private ISpreadService _KuaFuService = null;

		// Token: 0x04001C09 RID: 7177
		private bool _ClientInitialized = false;

		// Token: 0x04001C0A RID: 7178
		private KuaFuClientContext _ClientInfo = new KuaFuClientContext();

		// Token: 0x04001C0B RID: 7179
		private ConcurrentDictionary<int, KFSpreadData> _RoleId2KFSpreadDataDict = new ConcurrentDictionary<int, KFSpreadData>();

		// Token: 0x04001C0C RID: 7180
		private string _RemoteServiceUri = null;

		// Token: 0x04001C0D RID: 7181
		private DateTime _checkSpreadDataTime = DateTime.MinValue;

		// Token: 0x04001C0E RID: 7182
		private DuplexChannelFactory<ISpreadService> channelFactory;
	}
}
