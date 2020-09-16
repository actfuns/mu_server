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
using KF.Contract.Interface;
using KF.Remoting.Data;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = true)]
	public class SpreadService : MarshalByRefObject, ISpreadService
	{
		
		public override object InitializeLifetimeService()
		{
			SpreadService.Instance = this;
			ILease lease = (ILease)base.InitializeLifetimeService();
			if (lease.CurrentState == LeaseState.Initial)
			{
				lease.InitialLeaseTime = TimeSpan.FromDays(2000.0);
			}
			return lease;
		}

		
		public SpreadService()
		{
			SpreadService.Instance = this;
			this._BackgroundThread = new Thread(new ParameterizedThreadStart(this.ThreadProc));
			this._BackgroundThread.IsBackground = true;
			this._BackgroundThread.Start();
		}

		
		~SpreadService()
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
					if (now > this._clearTimeSpread)
					{
						this._clearTimeSpread = now.AddSeconds(86400.0);
						this.ClearSpreadData();
					}
					if (now > this._clearTimeVerify)
					{
						this._clearTimeVerify = now.AddSeconds(3600.0);
						this.ClearVerifyData();
						this.ClearTelData();
						this.ClearRoleData();
					}
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

		
		private void ClearSpreadData()
		{
			if (this._spreadDataDic != null && this._spreadDataDic.Count > 0)
			{
				List<KFSpreadKey> list = (from info in this._spreadDataDic.Values
				where info.LogTime <= TimeUtil.NowDateTime().AddSeconds(-86400.0)
				select KFSpreadKey.Get(info.ZoneID, info.RoleID)).ToList<KFSpreadKey>();
				foreach (KFSpreadKey t in list)
				{
					KFSpreadData d;
					this._spreadDataDic.TryRemove(t, out d);
				}
			}
		}

		
		private void ClearVerifyData()
		{
			if (this._spreadVerifyDataDic != null && this._spreadVerifyDataDic.Count > 0)
			{
				List<KFSpreadKey> list = (from info in this._spreadVerifyDataDic.Values
				where info.LogTime <= TimeUtil.NowDateTime().AddHours(-3600.0)
				select KFSpreadKey.Get(info.CZoneID, info.CRoleID)).ToList<KFSpreadKey>();
				foreach (KFSpreadKey t in list)
				{
					KFSpreadVerifyData d;
					this._spreadVerifyDataDic.TryRemove(t, out d);
				}
			}
		}

		
		private void ClearTelData()
		{
			if (this._telTotalDic != null && this._telTotalDic.Count > 0)
			{
				List<string> list = (from info in this._telTotalDic.Values
				where info.LogTime <= TimeUtil.NowDateTime().AddHours(-3600.0) && !info.IsStop
				select info.Tel).ToList<string>();
				foreach (string t in list)
				{
					KFSpreadTelTotal d;
					this._telTotalDic.TryRemove(t, out d);
				}
			}
		}

		
		private void ClearRoleData()
		{
			if (this._roleTotalDic != null && this._roleTotalDic.Count > 0)
			{
				List<KFSpreadKey> list = (from info in this._roleTotalDic.Values
				where info.LogTime <= TimeUtil.NowDateTime().AddHours(-3600.0) && !info.IsStop
				select KFSpreadKey.Get(info.CZoneID, info.CRoleID)).ToList<KFSpreadKey>();
				foreach (KFSpreadKey t in list)
				{
					KFSpreadRoleTotal d;
					this._roleTotalDic.TryRemove(t, out d);
				}
			}
		}

		
		public int InitializeClient(KuaFuClientContext clientInfo)
		{
			int result;
			try
			{
				if (clientInfo.GameType == 9 && clientInfo.ServerId != 0)
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

		
		public int SpreadSign(int serverID, int zoneID, int roleID)
		{
			int result;
			if (!this.IsAgent(serverID))
			{
				result = -5;
			}
			else
			{
				KFSpreadKey key = KFSpreadKey.Get(zoneID, roleID);
				KFSpreadData oldData;
				if (this._spreadDataDic.TryGetValue(key, out oldData))
				{
					oldData.UpdateLogtime();
					result = -21;
				}
				else if (!SpreadPersistence.Instance.DBSpreadSign(zoneID, roleID))
				{
					result = -1;
				}
				else
				{
					oldData = new KFSpreadData
					{
						ServerID = serverID,
						ZoneID = zoneID,
						RoleID = roleID
					};
					this._spreadDataDic.TryAdd(key, oldData);
					result = 1;
				}
			}
			return result;
		}

		
		public int[] SpreadCount(int serverID, int zoneID, int roleID)
		{
			int[] array = new int[3];
			int[] result = array;
			int[] result2;
			if (!this.IsAgent(serverID))
			{
				result2 = result;
			}
			else
			{
				KFSpreadKey key = KFSpreadKey.Get(zoneID, roleID);
				KFSpreadData oldData;
				if (this._spreadDataDic.TryGetValue(key, out oldData))
				{
					result[0] = oldData.CountRole;
					result[1] = oldData.CountVip;
					result[2] = oldData.CountLevel;
					oldData.UpdateLogtime();
					result2 = result;
				}
				else
				{
					result[0] = SpreadPersistence.Instance.DBSpreadCountAll(zoneID, roleID);
					result[1] = SpreadPersistence.Instance.DBSpreadCountVip(zoneID, roleID);
					result[2] = SpreadPersistence.Instance.DBSpreadCountLevel(zoneID, roleID);
					oldData = new KFSpreadData
					{
						ServerID = serverID,
						ZoneID = zoneID,
						RoleID = roleID,
						CountRole = result[0],
						CountVip = result[1],
						CountLevel = result[2]
					};
					this._spreadDataDic.TryAdd(key, oldData);
					result2 = result;
				}
			}
			return result2;
		}

		
		public int CheckVerifyCode(int cserverID, string cuserID, int czoneID, int croleID, int pzoneID, int proleID, int isVip, int isLevel)
		{
			int result;
			if (!this.IsAgent(cserverID))
			{
				result = -5;
			}
			else
			{
				KFSpreadData pData = this.GetSpreadData(pzoneID, proleID);
				if (pData == null)
				{
					result = -14;
				}
				else
				{
					bool isVerify = SpreadPersistence.Instance.DBSpreadVeruftCheck(czoneID, croleID, cuserID);
					if (isVerify)
					{
						result = -12;
					}
					else
					{
						KFSpreadRoleTotal roleTotalData = this.GetRoleTotalData(cserverID, czoneID, croleID, false);
						if (roleTotalData.IsStop)
						{
							result = -16;
						}
						else
						{
							KFSpreadKey ckey = KFSpreadKey.Get(czoneID, croleID);
							KFSpreadVerifyData verifyData = null;
							this._spreadVerifyDataDic.TryRemove(ckey, out verifyData);
							verifyData = new KFSpreadVerifyData
							{
								CUserID = cuserID,
								CServerID = cserverID,
								CZoneID = czoneID,
								CRoleID = croleID,
								PZoneID = pzoneID,
								PRoleID = proleID,
								IsVip = isVip,
								IsLevel = isLevel
							};
							this._spreadVerifyDataDic.TryAdd(ckey, verifyData);
							result = 1;
						}
					}
				}
			}
			return result;
		}

		
		public int TelCodeGet(int cserverID, int czoneID, int croleID, string tel)
		{
			int result;
			if (!this.IsAgent(cserverID))
			{
				result = -5;
			}
			else
			{
				KFSpreadKey ckey = KFSpreadKey.Get(czoneID, croleID);
				KFSpreadVerifyData verifyData = null;
				if (!this._spreadVerifyDataDic.TryGetValue(ckey, out verifyData))
				{
					result = -10;
				}
				else
				{
					bool isTelBind = SpreadPersistence.Instance.DBSpreadTelBind(tel);
					if (isTelBind)
					{
						result = -32;
					}
					else
					{
						KFSpreadTelTotal totalData = this.GetTelTotalData(tel, true);
						if (totalData.IsStop)
						{
							result = -36;
						}
						else
						{
							KFSpreadRoleTotal roleTotalData = this.GetRoleTotalData(cserverID, czoneID, croleID, true);
							if (roleTotalData.IsStop)
							{
								result = -16;
							}
							else
							{
								verifyData.Tel = tel;
								verifyData.TelCode = this.GetTelCodeRandom();
								verifyData.TelTime = TimeUtil.NowDateTime();
								if (!SpreadPersistence.Instance.DBSpreadTelCodeAdd(verifyData.PZoneID, verifyData.PRoleID, czoneID, croleID, tel, verifyData.TelCode))
								{
									result = -33;
								}
								else
								{
									result = 1;
								}
							}
						}
					}
				}
			}
			return result;
		}

		
		public int TelCodeVerify(int serverID, int czoneID, int croleID, int telCode)
		{
			int result;
			if (!this.IsAgent(serverID))
			{
				result = -5;
			}
			else
			{
				KFSpreadKey ckey = KFSpreadKey.Get(czoneID, croleID);
				KFSpreadVerifyData verifyData = null;
				if (!this._spreadVerifyDataDic.TryGetValue(ckey, out verifyData))
				{
					result = -10;
				}
				else
				{
					KFSpreadData pData = this.GetSpreadData(verifyData.PZoneID, verifyData.PRoleID);
					if (pData == null)
					{
						result = -14;
					}
					else
					{
						pData.UpdateLogtime();
						if (verifyData.TelCode != telCode)
						{
							result = -34;
						}
						else if (TimeUtil.NowDateTime().AddSeconds(-90.0) > verifyData.TelTime)
						{
							result = -35;
						}
						else if (!SpreadPersistence.Instance.DBSpreadRoleAdd(verifyData.PZoneID, verifyData.PRoleID, verifyData.CUserID, verifyData.CZoneID, verifyData.CRoleID, verifyData.Tel, verifyData.IsVip, verifyData.IsLevel))
						{
							result = -1;
						}
						else
						{
							lock (pData)
							{
								pData.CountLevel += verifyData.IsLevel;
								pData.CountVip += verifyData.IsVip;
								pData.CountRole++;
								if (pData.ServerID > 0)
								{
									this.NotifySpreadData(pData);
								}
							}
							this._spreadVerifyDataDic.TryRemove(ckey, out verifyData);
							result = 1;
						}
					}
				}
			}
			return result;
		}

		
		public bool SpreadLevel(int pzoneID, int proleID, int czoneID, int croleID)
		{
			KFSpreadData pData = this.GetSpreadData(pzoneID, proleID);
			bool result2;
			if (pData == null)
			{
				result2 = false;
			}
			else
			{
				pData.UpdateLogtime();
				lock (pData)
				{
					bool result = SpreadPersistence.Instance.DBSpreadIsLevel(pzoneID, proleID, czoneID, croleID);
					if (result)
					{
						pData.CountLevel++;
						if (pData.ServerID > 0)
						{
							this.NotifySpreadData(pData);
						}
						return true;
					}
				}
				result2 = false;
			}
			return result2;
		}

		
		public bool SpreadVip(int pzoneID, int proleID, int czoneID, int croleID)
		{
			KFSpreadData pData = this.GetSpreadData(pzoneID, proleID);
			bool result2;
			if (pData == null)
			{
				result2 = false;
			}
			else
			{
				pData.UpdateLogtime();
				lock (pData)
				{
					bool result = SpreadPersistence.Instance.DBSpreadIsVip(pzoneID, proleID, czoneID, croleID);
					if (result)
					{
						pData.CountVip++;
						if (pData.ServerID > 0)
						{
							this.NotifySpreadData(pData);
						}
						return true;
					}
				}
				result2 = false;
			}
			return result2;
		}

		
		private void NotifySpreadData(KFSpreadData data)
		{
			ClientAgentManager.Instance().PostAsyncEvent(data.ServerID, this.GameType, new AsyncDataItem(KuaFuEventTypes.SpreadCount, new object[]
			{
				data.ZoneID,
				data.RoleID,
				data.CountRole,
				data.CountVip,
				data.CountLevel
			}));
		}

		
		private int GetTelCodeRandom()
		{
			int result;
			if (Consts.IsTest > 0)
			{
				result = 123456;
			}
			else
			{
				result = RandomHelper.GetRandomNumber(100000, 999999);
			}
			return result;
		}

		
		private KFSpreadData GetSpreadData(int pzoneID, int proleID)
		{
			KFSpreadKey pkey = KFSpreadKey.Get(pzoneID, proleID);
			KFSpreadData pData = null;
			if (!this._spreadDataDic.TryGetValue(pkey, out pData))
			{
				if (!SpreadPersistence.Instance.DBSpreadSignCheck(pzoneID, proleID))
				{
					return null;
				}
				pData = new KFSpreadData
				{
					ServerID = 0,
					ZoneID = pzoneID,
					RoleID = proleID,
					CountRole = SpreadPersistence.Instance.DBSpreadCountAll(pzoneID, proleID),
					CountVip = SpreadPersistence.Instance.DBSpreadCountVip(pzoneID, proleID),
					CountLevel = SpreadPersistence.Instance.DBSpreadCountLevel(pzoneID, proleID)
				};
				this._spreadDataDic.TryAdd(pkey, pData);
			}
			if (pData != null)
			{
				pData.UpdateLogtime();
			}
			return pData;
		}

		
		private KFSpreadRoleTotal GetRoleTotalData(int cserverID, int czoneId, int croleID, bool isAddCount = false)
		{
			KFSpreadKey key = KFSpreadKey.Get(czoneId, croleID);
			KFSpreadRoleTotal data = null;
			if (!this._roleTotalDic.TryGetValue(key, out data))
			{
				data = new KFSpreadRoleTotal
				{
					CServerID = cserverID,
					CZoneID = czoneId,
					CRoleID = croleID
				};
				this._roleTotalDic.TryAdd(key, data);
			}
			int spanSecond = this.TimeSpanSecond(data.LogTime, TimeUtil.NowDateTime());
			KFSpreadRoleTotal result;
			if (data.IsStop)
			{
				if (spanSecond > Consts.VerifyRoleTimeStop)
				{
					data.LogTime = TimeUtil.NowDateTime();
					data.Count = 0;
					data.IsStop = false;
				}
				result = data;
			}
			else
			{
				if (spanSecond > Consts.VerifyRoleTimeLimit)
				{
					data.LogTime = TimeUtil.NowDateTime();
					data.Count = 0;
				}
				if (isAddCount)
				{
					data.AddCount();
				}
				if (data.Count > Consts.VerifyRoleMaxCount)
				{
					data.IsStop = true;
				}
				result = data;
			}
			return result;
		}

		
		private KFSpreadTelTotal GetTelTotalData(string tel, bool isAddCount = false)
		{
			KFSpreadTelTotal data = null;
			if (!this._telTotalDic.TryGetValue(tel, out data))
			{
				data = new KFSpreadTelTotal
				{
					Tel = tel
				};
				this._telTotalDic.TryAdd(tel, data);
			}
			int spanSecond = this.TimeSpanSecond(data.LogTime, TimeUtil.NowDateTime());
			KFSpreadTelTotal result;
			if (data.IsStop)
			{
				if (spanSecond > Consts.TelTimeStop)
				{
					data.LogTime = TimeUtil.NowDateTime();
					data.Count = 0;
					data.IsStop = false;
				}
				result = data;
			}
			else
			{
				if (spanSecond > Consts.TelTimeLimit)
				{
					data.LogTime = TimeUtil.NowDateTime();
					data.Count = 0;
				}
				if (isAddCount)
				{
					data.AddCount();
				}
				if (data.Count > Consts.TelMaxCount)
				{
					data.IsStop = true;
				}
				result = data;
			}
			return result;
		}

		
		private int TimeSpanSecond(DateTime begin, DateTime end)
		{
			TimeSpan tb = new TimeSpan(begin.Ticks);
			TimeSpan te = new TimeSpan(end.Ticks);
			return te.Subtract(tb).Duration().Seconds;
		}

		
		public bool IsAgent(int serverID)
		{
			bool isAgent = ClientAgentManager.Instance().ExistAgent(serverID);
			if (!isAgent)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("SpreadSign时ServerId错误.ServerId:{0}", serverID), null, true);
			}
			return isAgent;
		}

		
		public AsyncDataItem[] GetClientCacheItems(int serverID)
		{
			return ClientAgentManager.Instance().PickAsyncEvent(serverID, this.GameType);
		}

		
		private const double CLEAR_INTERVAL_SPREAD = 86400.0;

		
		private const double CLEAR_INTERVAL_VERIFY = 3600.0;

		
		private const int TEL_CODE_OUT_TIME = 90;

		
		public static SpreadService Instance = null;

		
		public SpreadPersistence _Persistence = SpreadPersistence.Instance;

		
		private object _Mutex = new object();

		
		public readonly GameTypes GameType = GameTypes.Spread;

		
		private DateTime _clearTimeSpread = DateTime.MinValue;

		
		private DateTime _clearTimeVerify = DateTime.MinValue;

		
		private ConcurrentDictionary<KFSpreadKey, KFSpreadData> _spreadDataDic = new ConcurrentDictionary<KFSpreadKey, KFSpreadData>();

		
		private ConcurrentDictionary<KFSpreadKey, KFSpreadVerifyData> _spreadVerifyDataDic = new ConcurrentDictionary<KFSpreadKey, KFSpreadVerifyData>();

		
		private ConcurrentDictionary<string, KFSpreadTelTotal> _telTotalDic = new ConcurrentDictionary<string, KFSpreadTelTotal>();

		
		private ConcurrentDictionary<KFSpreadKey, KFSpreadRoleTotal> _roleTotalDic = new ConcurrentDictionary<KFSpreadKey, KFSpreadRoleTotal>();

		
		public Thread _BackgroundThread;
	}
}
