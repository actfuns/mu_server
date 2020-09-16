using System;
using System.Collections.Generic;
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
	public class IPStatisticsClient : MarshalByRefObject, IKuaFuClient, IManager2
	{
		
		public static IPStatisticsClient getInstance()
		{
			return IPStatisticsClient.instance;
		}

		
		public bool initialize()
		{
			return true;
		}

		
		public bool initialize(ICoreInterface coreInterface)
		{
			this.CoreInterface = coreInterface;
			this.ClientInfo.PTID = GameManager.PTID;
			this.ClientInfo.ServerId = GameManager.ServerId;
			this.ClientInfo.GameType = 18;
			this.ClientInfo.Token = this.CoreInterface.GetLocalAddressIPs();
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

		
		public void TimerProc(object sender, EventArgs e)
		{
			try
			{
				string IPStatisticsUri = this.CoreInterface.GetRuntimeVariable("IPStatisticsUri", null);
				if (this.RemoteServiceUri != IPStatisticsUri)
				{
					this.RemoteServiceUri = IPStatisticsUri;
				}
			}
			catch (Exception ex)
			{
				this.ResetKuaFuService();
			}
		}

		
		private void CloseConnection()
		{
			this.ClientInfo.ClientId = 0;
			this.RemoteServiceUri = this.CoreInterface.GetRuntimeVariable("IPStatisticsUri", null);
			lock (this.Mutex)
			{
				this.KuaFuService = null;
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

		
		private IIPStatisticsService GetKuaFuService(bool noWait = false)
		{
			try
			{
				if (KuaFuManager.KuaFuWorldKuaFuGameServer)
				{
					return null;
				}
				lock (this.Mutex)
				{
					if (string.IsNullOrEmpty(this.RemoteServiceUri))
					{
						return null;
					}
					if (this.KuaFuService == null && noWait)
					{
						return null;
					}
				}
				lock (this.RemotingMutex)
				{
					IIPStatisticsService kuaFuService;
					if (this.KuaFuService == null)
					{
						kuaFuService = (IIPStatisticsService)Activator.GetObject(typeof(IYongZheZhanChangService), this.RemoteServiceUri);
						if (null == kuaFuService)
						{
							return null;
						}
					}
					else
					{
						kuaFuService = this.KuaFuService;
					}
					int clientId = this.ClientInfo.ClientId;
					long nowTicks = TimeUtil.NOW();
					if (clientId <= 0 || Math.Abs(nowTicks - this.ClientInfo.LastInitClientTicks) > 12000L)
					{
						this.ClientInfo.LastInitClientTicks = nowTicks;
						clientId = kuaFuService.InitializeClient(this.ClientInfo);
					}
					if (kuaFuService != null && (clientId != this.ClientInfo.ClientId || this.KuaFuService != kuaFuService))
					{
						lock (this.Mutex)
						{
							if (clientId > 0)
							{
								this.KuaFuService = kuaFuService;
							}
							else
							{
								this.KuaFuService = null;
							}
							this.ClientInfo.ClientId = clientId;
							return this.KuaFuService;
						}
					}
					return this.KuaFuService;
				}
			}
			catch (Exception ex)
			{
				this.ResetKuaFuService();
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return null;
		}

		
		public int GetNewFuBenSeqId()
		{
			return -11;
		}

		
		public object GetDataFromClientServer(int dataType, params object[] args)
		{
			return null;
		}

		
		public void EventCallBackHandler(AsyncDataItem item)
		{
			try
			{
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		
		public int RequestMinite()
		{
			int result = -1;
			try
			{
				IIPStatisticsService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return result;
				}
				try
				{
					result = kuaFuService.RequestMinite();
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return result;
		}

		
		public List<IPOperaData> GetIPStatisticsResult()
		{
			List<IPOperaData> resultList = null;
			try
			{
				IIPStatisticsService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return resultList;
				}
				try
				{
					resultList = kuaFuService.GetIPStatisticsResult(this.ClientInfo.ServerId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return resultList;
		}

		
		public int IPStatisticsDataReport(int lastMinite, List<IPStatisticsData> list)
		{
			try
			{
				IIPStatisticsService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return -11001;
				}
				try
				{
					int result = kuaFuService.IPStatisticsDataReport(this.ClientInfo.ServerId, lastMinite, list);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return 1;
		}

		
		private static IPStatisticsClient instance = new IPStatisticsClient();

		
		private object Mutex = new object();

		
		private object RemotingMutex = new object();

		
		private ICoreInterface CoreInterface = null;

		
		private IIPStatisticsService KuaFuService = null;

		
		private bool ClientInitialized = false;

		
		private KuaFuClientContext ClientInfo = new KuaFuClientContext();

		
		private string RemoteServiceUri = null;

		
		private DuplexChannelFactory<IIPStatisticsService> channelFactory;
	}
}
