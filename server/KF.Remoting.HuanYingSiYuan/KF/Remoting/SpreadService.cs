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
	// Token: 0x02000063 RID: 99
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = true)]
	public class SpreadService : MarshalByRefObject, ISpreadService
	{
		// Token: 0x06000479 RID: 1145 RVA: 0x0003A0E0 File Offset: 0x000382E0
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

		// Token: 0x0600047A RID: 1146 RVA: 0x0003A12C File Offset: 0x0003832C
		public SpreadService()
		{
			SpreadService.Instance = this;
			this._BackgroundThread = new Thread(new ParameterizedThreadStart(this.ThreadProc));
			this._BackgroundThread.IsBackground = true;
			this._BackgroundThread.Start();
		}

		// Token: 0x0600047B RID: 1147 RVA: 0x0003A1D8 File Offset: 0x000383D8
		~SpreadService()
		{
			this._BackgroundThread.Abort();
		}

		// Token: 0x0600047C RID: 1148 RVA: 0x0003A210 File Offset: 0x00038410
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

		// Token: 0x0600047D RID: 1149 RVA: 0x0003A378 File Offset: 0x00038578
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

		// Token: 0x0600047E RID: 1150 RVA: 0x0003A4AC File Offset: 0x000386AC
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

		// Token: 0x0600047F RID: 1151 RVA: 0x0003A5E4 File Offset: 0x000387E4
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

		// Token: 0x06000480 RID: 1152 RVA: 0x0003A728 File Offset: 0x00038928
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

		// Token: 0x06000481 RID: 1153 RVA: 0x0003A804 File Offset: 0x00038A04
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

		// Token: 0x06000482 RID: 1154 RVA: 0x0003A8B4 File Offset: 0x00038AB4
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

		// Token: 0x06000483 RID: 1155 RVA: 0x0003A950 File Offset: 0x00038B50
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

		// Token: 0x06000484 RID: 1156 RVA: 0x0003AA40 File Offset: 0x00038C40
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

		// Token: 0x06000485 RID: 1157 RVA: 0x0003AB4C File Offset: 0x00038D4C
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

		// Token: 0x06000486 RID: 1158 RVA: 0x0003AC50 File Offset: 0x00038E50
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

		// Token: 0x06000487 RID: 1159 RVA: 0x0003AE00 File Offset: 0x00039000
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

		// Token: 0x06000488 RID: 1160 RVA: 0x0003AEB4 File Offset: 0x000390B4
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

		// Token: 0x06000489 RID: 1161 RVA: 0x0003AF68 File Offset: 0x00039168
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

		// Token: 0x0600048A RID: 1162 RVA: 0x0003AFE8 File Offset: 0x000391E8
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

		// Token: 0x0600048B RID: 1163 RVA: 0x0003B020 File Offset: 0x00039220
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

		// Token: 0x0600048C RID: 1164 RVA: 0x0003B0E0 File Offset: 0x000392E0
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

		// Token: 0x0600048D RID: 1165 RVA: 0x0003B1E8 File Offset: 0x000393E8
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

		// Token: 0x0600048E RID: 1166 RVA: 0x0003B2D8 File Offset: 0x000394D8
		private int TimeSpanSecond(DateTime begin, DateTime end)
		{
			TimeSpan tb = new TimeSpan(begin.Ticks);
			TimeSpan te = new TimeSpan(end.Ticks);
			return te.Subtract(tb).Duration().Seconds;
		}

		// Token: 0x0600048F RID: 1167 RVA: 0x0003B324 File Offset: 0x00039524
		public bool IsAgent(int serverID)
		{
			bool isAgent = ClientAgentManager.Instance().ExistAgent(serverID);
			if (!isAgent)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("SpreadSign时ServerId错误.ServerId:{0}", serverID), null, true);
			}
			return isAgent;
		}

		// Token: 0x06000490 RID: 1168 RVA: 0x0003B364 File Offset: 0x00039564
		public AsyncDataItem[] GetClientCacheItems(int serverID)
		{
			return ClientAgentManager.Instance().PickAsyncEvent(serverID, this.GameType);
		}

		// Token: 0x0400026D RID: 621
		private const double CLEAR_INTERVAL_SPREAD = 86400.0;

		// Token: 0x0400026E RID: 622
		private const double CLEAR_INTERVAL_VERIFY = 3600.0;

		// Token: 0x0400026F RID: 623
		private const int TEL_CODE_OUT_TIME = 90;

		// Token: 0x04000270 RID: 624
		public static SpreadService Instance = null;

		// Token: 0x04000271 RID: 625
		public SpreadPersistence _Persistence = SpreadPersistence.Instance;

		// Token: 0x04000272 RID: 626
		private object _Mutex = new object();

		// Token: 0x04000273 RID: 627
		public readonly GameTypes GameType = GameTypes.Spread;

		// Token: 0x04000274 RID: 628
		private DateTime _clearTimeSpread = DateTime.MinValue;

		// Token: 0x04000275 RID: 629
		private DateTime _clearTimeVerify = DateTime.MinValue;

		// Token: 0x04000276 RID: 630
		private ConcurrentDictionary<KFSpreadKey, KFSpreadData> _spreadDataDic = new ConcurrentDictionary<KFSpreadKey, KFSpreadData>();

		// Token: 0x04000277 RID: 631
		private ConcurrentDictionary<KFSpreadKey, KFSpreadVerifyData> _spreadVerifyDataDic = new ConcurrentDictionary<KFSpreadKey, KFSpreadVerifyData>();

		// Token: 0x04000278 RID: 632
		private ConcurrentDictionary<string, KFSpreadTelTotal> _telTotalDic = new ConcurrentDictionary<string, KFSpreadTelTotal>();

		// Token: 0x04000279 RID: 633
		private ConcurrentDictionary<KFSpreadKey, KFSpreadRoleTotal> _roleTotalDic = new ConcurrentDictionary<KFSpreadKey, KFSpreadRoleTotal>();

		// Token: 0x0400027A RID: 634
		public Thread _BackgroundThread;
	}
}
