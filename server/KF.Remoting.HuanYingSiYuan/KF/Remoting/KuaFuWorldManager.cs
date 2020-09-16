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
	
	public class KuaFuWorldManager : TSingleton<KuaFuWorldManager>, IKuaFuWorld
	{
		
		public KuaFuWorldManager()
		{
			this.BackgroundThread = new Thread(new ParameterizedThreadStart(this.ThreadProc));
			this.BackgroundThread.IsBackground = true;
			this.BackgroundThread.Start();
		}

		
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

		
		public AsyncDataItem[] GetClientCacheItems(int serverId)
		{
			return ClientAgentManager.Instance().PickAsyncEvent(serverId, this.GameType);
		}

		
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

		
		public AsyncDataItem GetKuaFuLineDataList(int mapCode)
		{
			return new AsyncDataItem(KuaFuEventTypes.Other, new object[]
			{
				KuaFuServerManager.GetKuaFuLineDataList(mapCode)
			});
		}

		
		public List<KuaFuServerInfo> GetKuaFuServerInfoData(int age)
		{
			return KuaFuServerManager.GetKuaFuServerInfoData(age);
		}

		
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

		
		public void Reborn_SetRoleData4Selector(int ptId, int roleId, byte[] bytes)
		{
			RebornService.Instance().SetRoleData4Selector(ptId, roleId, bytes);
		}

		
		public int Reborn_RoleReborn(int ptId, int roleId, string roleName, int level)
		{
			return RebornService.Instance().RoleReborn(ptId, roleId, roleName, level);
		}

		
		public RebornSyncData Reborn_SyncData(long ageRank, long ageBoss)
		{
			return RebornService.Instance().Reborn_SyncData(ageRank, ageBoss);
		}

		
		public void Reborn_RebornOpt(int ptid, int rid, int optType, int param1, int param2, string param3)
		{
			RebornService.Instance().RebornOpt(ptid, rid, optType, param1, param2, param3);
		}

		
		public KuaFuCmdData Reborn_GetRebornRoleData(int ptId, int roleId, long dataAge)
		{
			return RebornService.Instance().GetRebornRoleData(ptId, roleId, dataAge);
		}

		
		public void Reborn_ChangeName(int ptId, int roleId, string roleName)
		{
			RebornService.Instance().ChangeName(ptId, roleId, roleName);
		}

		
		public void Reborn_PlatFormChat(int serverId, byte[] bytes)
		{
			RebornService.Instance().PlatFormChat(serverId, bytes);
		}

		
		private void Broadcast2GsAgent(AsyncDataItem item)
		{
			ClientAgentManager.Instance().BroadCastAsyncEvent(this.GameType, item, 0);
		}

		
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

		
		private const string PTKuaFuManager = "AliasName";

		
		private object Mutex = new object();

		
		private bool ConfigLoadFinished;

		
		public GameTypes GameType = GameTypes.KuaFuWorld;

		
		private Dictionary<string, KuaFuWorldRoleData> RoleDataDict = new Dictionary<string, KuaFuWorldRoleData>();

		
		private Dictionary<string, KuaFuServerLoginData> WorldRoleIDDict = new Dictionary<string, KuaFuServerLoginData>();

		
		private Dictionary<string, KuaFuServerLoginData> RandomTokenDict = new Dictionary<string, KuaFuServerLoginData>();

		
		private Thread BackgroundThread;

		
		public static char[] WriteSpaceChars = new char[]
		{
			' '
		};
	}
}
