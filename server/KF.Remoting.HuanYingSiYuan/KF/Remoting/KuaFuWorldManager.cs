using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GameServer.Core.Executor;
using KF.Contract;
using KF.Contract.Data;
using KF.Contract.Interface;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting
{
	// Token: 0x02000057 RID: 87
	public class KuaFuWorldManager : TSingleton<KuaFuWorldManager>, IKuaFuWorld
	{
		// Token: 0x060003E7 RID: 999 RVA: 0x000335EC File Offset: 0x000317EC
		public KuaFuWorldManager()
		{
			this.BackgroundThread = new Thread(new ParameterizedThreadStart(this.ThreadProc));
			this.BackgroundThread.IsBackground = true;
			this.BackgroundThread.Start();
		}

		// Token: 0x060003E8 RID: 1000 RVA: 0x00033668 File Offset: 0x00031868
		public void LoadConfig(bool reload = false)
		{
			try
			{
				this.ConfigLoadFinished = false;
			}
			catch (Exception ex)
			{
			}
			finally
			{
				this.ConfigLoadFinished = true;
			}
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x000336B0 File Offset: 0x000318B0
		public void ThreadProc(object state)
		{
			int sleepMS = 0;
			for (;;)
			{
				DateTime now = TimeUtil.NowDateTime();
				try
				{
					if (this.ConfigLoadFinished)
					{
						RebornService.Instance().Update(now);
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteExceptionUseCache(ex.ToString());
				}
				finally
				{
					sleepMS = (int)(TimeUtil.NowDateTime() - now).TotalMilliseconds;
				}
				Thread.Sleep(Math.Max(50, 1000 - sleepMS));
			}
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x0003374C File Offset: 0x0003194C
		public AsyncDataItem[] GetClientCacheItems(int serverId)
		{
			return ClientAgentManager.Instance().PickAsyncEvent(serverId, this.GameType);
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x00033770 File Offset: 0x00031970
		public int InitializeClient(KuaFuClientContext clientInfo)
		{
			int result;
			try
			{
				if (clientInfo.ServerId != 0)
				{
					bool bFirstInit = false;
					int ret = ClientAgentManager.Instance().InitializeClient(clientInfo, out bFirstInit);
					if (ret > 0)
					{
						if (clientInfo.MapClientCountDict != null && clientInfo.MapClientCountDict.Count > 0)
						{
							KuaFuServerManager.UpdateKuaFuLineData(clientInfo.ServerId, clientInfo.MapClientCountDict);
							ClientAgentManager.Instance().SetMainlinePayload(clientInfo.ServerId, clientInfo.MapClientCountDict.Values.ToList<int>().Sum());
						}
					}
					result = ret;
				}
				else
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("InitializeClient时GameType错误,禁止连接.ServerId:{0},GameType:{1}", clientInfo.ServerId, clientInfo.GameType), null, true);
					result = -4003;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(string.Format("InitializeClient服务器ID重复,禁止连接.ServerId:{0},ClientId:{1},info:{2}", clientInfo.ServerId, clientInfo.ClientId, clientInfo.Token));
				result = -11003;
			}
			return result;
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x0003388C File Offset: 0x00031A8C
		public void UpdateKuaFuMapClientCount(int serverId, Dictionary<int, int> mapClientCountDict)
		{
			if (mapClientCountDict != null && mapClientCountDict.Count > 0)
			{
				ClientAgent agent = ClientAgentManager.Instance().GetCurrentClientAgent(serverId);
				if (null != agent)
				{
					KuaFuServerManager.UpdateKuaFuLineData(agent.ClientInfo.ServerId, mapClientCountDict);
					ClientAgentManager.Instance().SetMainlinePayload(agent.ClientInfo.ServerId, mapClientCountDict.Values.ToList<int>().Sum());
				}
			}
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x00033904 File Offset: 0x00031B04
		public int ExecuteCommand(string cmd)
		{
			int result;
			if (string.IsNullOrEmpty(cmd))
			{
				result = -18;
			}
			else
			{
				string[] args = cmd.Split(KuaFuWorldManager.WriteSpaceChars, StringSplitOptions.RemoveEmptyEntries);
				result = this.ExecCommand(args);
			}
			return result;
		}

		// Token: 0x060003EE RID: 1006 RVA: 0x00033940 File Offset: 0x00031B40
		public AsyncDataItem GetKuaFuLineDataList(int mapCode)
		{
			return new AsyncDataItem(KuaFuEventTypes.Other, new object[]
			{
				KuaFuServerManager.GetKuaFuLineDataList(mapCode)
			});
		}

		// Token: 0x060003EF RID: 1007 RVA: 0x00033970 File Offset: 0x00031B70
		public List<KuaFuServerInfo> GetKuaFuServerInfoData(int age)
		{
			return KuaFuServerManager.GetKuaFuServerInfoData(age);
		}

		// Token: 0x060003F0 RID: 1008 RVA: 0x00033988 File Offset: 0x00031B88
		public int RegPTKuaFuRoleData(ref KuaFuWorldRoleData data)
		{
			data.WorldRoleID = ConstData.FormatWorldRoleID(data.LocalRoleID, data.PTID);
			KuaFuWorldRoleData roleData = this.LoadKuaFuWorldRoleData(data.LocalRoleID, data.PTID, data.WorldRoleID);
			int result;
			if (null != roleData)
			{
				if (data.RoleData != null)
				{
					roleData.RoleData = data.RoleData;
					int dbRet = YongZheZhanChangPersistence.Instance.WriteKuaFuWorldRoleData(roleData);
					if (dbRet < 0)
					{
						return dbRet;
					}
				}
				data = roleData;
				result = 0;
			}
			else
			{
				for (int i = 0; i < 10; i++)
				{
					int tempRoleIDLimit = 0;
					int maxTempRoleID = YongZheZhanChangPersistence.Instance.GetKuaFoWorldMaxTempRoleID(out tempRoleIDLimit);
					if (maxTempRoleID >= tempRoleIDLimit)
					{
						return -22;
					}
					roleData = YongZheZhanChangPersistence.Instance.InsertKuaFuWorldRoleData(data, maxTempRoleID + 1);
					if (null != roleData)
					{
						lock (this.Mutex)
						{
							KuaFuWorldRoleData temp;
							if (!this.RoleDataDict.TryGetValue(data.WorldRoleID, out temp) || temp == null)
							{
								this.RoleDataDict[data.WorldRoleID] = roleData;
							}
						}
						break;
					}
					Thread.Sleep(500);
				}
				if (null != roleData)
				{
					data = roleData;
					result = roleData.TempRoleID;
				}
				else
				{
					result = -15;
				}
			}
			return result;
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x00033B30 File Offset: 0x00031D30
		public KuaFuWorldRoleData LoadKuaFuWorldRoleData(int roleId, int ptid, string worldRoleID)
		{
			KuaFuWorldRoleData roleData;
			lock (this.Mutex)
			{
				if (this.RoleDataDict.TryGetValue(worldRoleID, out roleData) && roleData != null)
				{
					return roleData;
				}
			}
			roleData = YongZheZhanChangPersistence.Instance.QueryKuaFuWorldRoleData(roleId, ptid);
			lock (this.Mutex)
			{
				KuaFuWorldRoleData temp;
				if (!this.RoleDataDict.TryGetValue(worldRoleID, out temp) || temp == null)
				{
					this.RoleDataDict[worldRoleID] = roleData;
				}
			}
			return roleData;
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x00033C20 File Offset: 0x00031E20
		public int EnterPTKuaFuMap(int serverID, int roleId, int ptid, int mapCode, int kuaFuLine, out string signToken, out string signKey, out int kuaFuServerID, out string[] ips, out int[] ports)
		{
			ips = null;
			ports = null;
			signToken = null;
			signKey = null;
			kuaFuServerID = 0;
			string worldRoleID = ConstData.FormatWorldRoleID(roleId, ptid);
			KuaFuWorldRoleData roleData = this.LoadKuaFuWorldRoleData(roleId, ptid, worldRoleID);
			int result;
			if (null == roleData)
			{
				result = -4010;
			}
			else
			{
				kuaFuServerID = KuaFuServerManager.EnterKuaFuMapLine(kuaFuLine, mapCode);
				if (kuaFuServerID <= 0)
				{
					result = -100;
				}
				else
				{
					KuaFuServerInfo serverInfo = KuaFuServerManager.GetKuaFuServerInfo(kuaFuServerID);
					if (null != serverInfo)
					{
						ips = new string[]
						{
							serverInfo.Ip
						};
						ports = new int[]
						{
							serverInfo.Port
						};
					}
					signToken = Guid.NewGuid().ToString("N");
					signKey = Guid.NewGuid().ToString("N");
					long utcTicks = TimeUtil.UTCTicks();
					lock (this.Mutex)
					{
						KuaFuServerLoginData loginData;
						if (!this.WorldRoleIDDict.TryGetValue(worldRoleID, out loginData))
						{
							loginData = new KuaFuServerLoginData();
							loginData.TempRoleID = roleData.TempRoleID;
							this.WorldRoleIDDict[worldRoleID] = loginData;
						}
						loginData.SignKey = signKey;
						loginData.SignToken = signToken;
						loginData.EndTicks = utcTicks + 86400000L;
						loginData.TargetServerID = kuaFuServerID;
						loginData.ServerId = ConstData.ConvertToKuaFuServerID(serverID, ptid);
						loginData.RoleId = roleId;
						loginData.PTID = ptid;
						loginData.GameId = (long)mapCode;
						result = loginData.TempRoleID;
					}
				}
			}
			return result;
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x00033DE0 File Offset: 0x00031FE0
		public int CheckEnterWorldKuaFuSign(string worldRoleID, string token, out string signKey, out string[] ips, out int[] ports)
		{
			int result = -100;
			signKey = null;
			ips = null;
			ports = null;
			long utcTicks = TimeUtil.UTCTicks();
			lock (this.Mutex)
			{
				KuaFuServerLoginData data;
				if (this.WorldRoleIDDict.TryGetValue(worldRoleID, out data) && token == data.SignToken)
				{
					if (utcTicks > data.EndTicks)
					{
						return result;
					}
					result = 0;
					signKey = data.SignKey;
				}
				KuaFuServerInfo serverInfo = KuaFuServerManager.GetKuaFuServerInfo(data.ServerId);
				if (null != serverInfo)
				{
					ips = new string[]
					{
						serverInfo.DbIp,
						serverInfo.DbIp
					};
					ports = new int[]
					{
						serverInfo.DbPort,
						serverInfo.LogDbPort
					};
				}
			}
			return result;
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x00033EF4 File Offset: 0x000320F4
		public void Reborn_SetRoleData4Selector(int ptId, int roleId, byte[] bytes)
		{
			RebornService.Instance().SetRoleData4Selector(ptId, roleId, bytes);
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x00033F08 File Offset: 0x00032108
		public int Reborn_RoleReborn(int ptId, int roleId, string roleName, int level)
		{
			return RebornService.Instance().RoleReborn(ptId, roleId, roleName, level);
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x00033F2C File Offset: 0x0003212C
		public RebornSyncData Reborn_SyncData(long ageRank, long ageBoss)
		{
			return RebornService.Instance().Reborn_SyncData(ageRank, ageBoss);
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x00033F4A File Offset: 0x0003214A
		public void Reborn_RebornOpt(int ptid, int rid, int optType, int param1, int param2, string param3)
		{
			RebornService.Instance().RebornOpt(ptid, rid, optType, param1, param2, param3);
		}

		// Token: 0x060003F8 RID: 1016 RVA: 0x00033F64 File Offset: 0x00032164
		public KuaFuCmdData Reborn_GetRebornRoleData(int ptId, int roleId, long dataAge)
		{
			return RebornService.Instance().GetRebornRoleData(ptId, roleId, dataAge);
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x00033F83 File Offset: 0x00032183
		public void Reborn_ChangeName(int ptId, int roleId, string roleName)
		{
			RebornService.Instance().ChangeName(ptId, roleId, roleName);
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x00033F94 File Offset: 0x00032194
		public void Reborn_PlatFormChat(int serverId, byte[] bytes)
		{
			RebornService.Instance().PlatFormChat(serverId, bytes);
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x00033FA4 File Offset: 0x000321A4
		private void Broadcast2GsAgent(AsyncDataItem item)
		{
			ClientAgentManager.Instance().BroadCastAsyncEvent(this.GameType, item, 0);
		}

		// Token: 0x060003FC RID: 1020 RVA: 0x00034018 File Offset: 0x00032218
		public int ExecCommand(string[] args)
		{
			int result = 0;
			try
			{
				if (0 == string.Compare(args[0], "-settime", true))
				{
					if (KuaFuServerManager.EnableGMSetAllServerTime && args.Length >= 3)
					{
						string datetimeStr = args[2];
						if (args.Length > 3)
						{
							datetimeStr = datetimeStr + " " + args[3];
						}
						ThreadPool.QueueUserWorkItem(delegate(object x)
						{
							Thread.Sleep(10000);
							string timeStr = x as string;
							if (!string.IsNullOrEmpty(timeStr))
							{
								TimeUtil.SetTime(timeStr);
								LogManager.WriteLog(LogTypes.Error, string.Format("GM命令修改时间#from={0},to={1}", TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss"), timeStr), null, true);
							}
						}, datetimeStr);
						this.Broadcast2GsAgent(new AsyncDataItem(KuaFuEventTypes.GMSetTime, args));
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return result;
		}

		// Token: 0x0400021C RID: 540
		private const string PTKuaFuManager = "AliasName";

		// Token: 0x0400021D RID: 541
		private object Mutex = new object();

		// Token: 0x0400021E RID: 542
		private bool ConfigLoadFinished;

		// Token: 0x0400021F RID: 543
		public GameTypes GameType = GameTypes.KuaFuWorld;

		// Token: 0x04000220 RID: 544
		private Dictionary<string, KuaFuWorldRoleData> RoleDataDict = new Dictionary<string, KuaFuWorldRoleData>();

		// Token: 0x04000221 RID: 545
		private Dictionary<string, KuaFuServerLoginData> WorldRoleIDDict = new Dictionary<string, KuaFuServerLoginData>();

		// Token: 0x04000222 RID: 546
		private Dictionary<string, KuaFuServerLoginData> RandomTokenDict = new Dictionary<string, KuaFuServerLoginData>();

		// Token: 0x04000223 RID: 547
		private Thread BackgroundThread;

		// Token: 0x04000224 RID: 548
		public static char[] WriteSpaceChars = new char[]
		{
			' '
		};
	}
}
