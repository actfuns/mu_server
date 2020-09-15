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
	// Token: 0x02000348 RID: 840
	[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, IgnoreExtensionDataObject = true, UseSynchronizationContext = false)]
	public class IPStatisticsClient : MarshalByRefObject, IKuaFuClient, IManager2
	{
		// Token: 0x06000E27 RID: 3623 RVA: 0x000E0A60 File Offset: 0x000DEC60
		public static IPStatisticsClient getInstance()
		{
			return IPStatisticsClient.instance;
		}

		// Token: 0x06000E28 RID: 3624 RVA: 0x000E0A78 File Offset: 0x000DEC78
		public bool initialize()
		{
			return true;
		}

		// Token: 0x06000E29 RID: 3625 RVA: 0x000E0A8C File Offset: 0x000DEC8C
		public bool initialize(ICoreInterface coreInterface)
		{
			this.CoreInterface = coreInterface;
			this.ClientInfo.PTID = GameManager.PTID;
			this.ClientInfo.ServerId = GameManager.ServerId;
			this.ClientInfo.GameType = 18;
			this.ClientInfo.Token = this.CoreInterface.GetLocalAddressIPs();
			return true;
		}

		// Token: 0x06000E2A RID: 3626 RVA: 0x000E0AEC File Offset: 0x000DECEC
		public bool startup()
		{
			return true;
		}

		// Token: 0x06000E2B RID: 3627 RVA: 0x000E0B00 File Offset: 0x000DED00
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06000E2C RID: 3628 RVA: 0x000E0B14 File Offset: 0x000DED14
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06000E2D RID: 3629 RVA: 0x000E0B28 File Offset: 0x000DED28
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

		// Token: 0x06000E2E RID: 3630 RVA: 0x000E0B7C File Offset: 0x000DED7C
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

		// Token: 0x06000E2F RID: 3631 RVA: 0x000E0BDC File Offset: 0x000DEDDC
		private void CloseConnection()
		{
			this.ClientInfo.ClientId = 0;
			this.RemoteServiceUri = this.CoreInterface.GetRuntimeVariable("IPStatisticsUri", null);
			lock (this.Mutex)
			{
				this.KuaFuService = null;
			}
		}

		// Token: 0x06000E30 RID: 3632 RVA: 0x000E0C4C File Offset: 0x000DEE4C
		private void OnConnectionClose(object sender, EventArgs e)
		{
			this.CloseConnection();
		}

		// Token: 0x06000E31 RID: 3633 RVA: 0x000E0C56 File Offset: 0x000DEE56
		private void ResetKuaFuService()
		{
			this.CloseConnection();
		}

		// Token: 0x06000E32 RID: 3634 RVA: 0x000E0C60 File Offset: 0x000DEE60
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

		// Token: 0x06000E33 RID: 3635 RVA: 0x000E0EDC File Offset: 0x000DF0DC
		public int GetNewFuBenSeqId()
		{
			return -11;
		}

		// Token: 0x06000E34 RID: 3636 RVA: 0x000E0EF0 File Offset: 0x000DF0F0
		public object GetDataFromClientServer(int dataType, params object[] args)
		{
			return null;
		}

		// Token: 0x06000E35 RID: 3637 RVA: 0x000E0F04 File Offset: 0x000DF104
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

		// Token: 0x06000E36 RID: 3638 RVA: 0x000E0F38 File Offset: 0x000DF138
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

		// Token: 0x06000E37 RID: 3639 RVA: 0x000E0FB4 File Offset: 0x000DF1B4
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

		// Token: 0x06000E38 RID: 3640 RVA: 0x000E103C File Offset: 0x000DF23C
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

		// Token: 0x04001623 RID: 5667
		private static IPStatisticsClient instance = new IPStatisticsClient();

		// Token: 0x04001624 RID: 5668
		private object Mutex = new object();

		// Token: 0x04001625 RID: 5669
		private object RemotingMutex = new object();

		// Token: 0x04001626 RID: 5670
		private ICoreInterface CoreInterface = null;

		// Token: 0x04001627 RID: 5671
		private IIPStatisticsService KuaFuService = null;

		// Token: 0x04001628 RID: 5672
		private bool ClientInitialized = false;

		// Token: 0x04001629 RID: 5673
		private KuaFuClientContext ClientInfo = new KuaFuClientContext();

		// Token: 0x0400162A RID: 5674
		private string RemoteServiceUri = null;

		// Token: 0x0400162B RID: 5675
		private DuplexChannelFactory<IIPStatisticsService> channelFactory;
	}
}
