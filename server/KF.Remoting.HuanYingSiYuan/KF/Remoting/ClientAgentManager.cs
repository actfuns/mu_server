using System;
using System.Collections.Generic;
using System.ServiceModel;
using KF.Contract;
using KF.Contract.Data;
using KF.Remoting.Data;
using Server.Tools;

namespace KF.Remoting
{
	// Token: 0x02000003 RID: 3
	internal class ClientAgentManager
	{
		// Token: 0x06000019 RID: 25 RVA: 0x000028B8 File Offset: 0x00000AB8
		public static ClientAgentManager Instance()
		{
			return ClientAgentManager._AgentMgr;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000028D0 File Offset: 0x00000AD0
		private ClientAgentManager()
		{
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002928 File Offset: 0x00000B28
		public bool IsAgentAlive(int serverId)
		{
			lock (this.Mutex)
			{
				ClientAgent agent = null;
				if (this.ServerId2ClientAgent.TryGetValue(serverId, out agent) && agent.IsAlive)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000029A0 File Offset: 0x00000BA0
		public bool ExistAgent(int serverId)
		{
			bool result;
			lock (this.Mutex)
			{
				result = this.ServerId2ClientAgent.ContainsKey(serverId);
			}
			return result;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000029F4 File Offset: 0x00000BF4
		public bool IsAnyKfAgentAlive()
		{
			lock (this.Mutex)
			{
				foreach (int sid in this.AutoKfServerId)
				{
					if (this.IsAgentAlive(sid))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002A9C File Offset: 0x00000C9C
		public bool IsKfAgent(int serverId)
		{
			bool result;
			lock (this.Mutex)
			{
				result = (this.AllKfServerId.Contains(serverId) && this.ExistAgent(serverId));
			}
			return result;
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002AFC File Offset: 0x00000CFC
		public void SetAllKfServerId(HashSet<int> existKfIds)
		{
			lock (this.Mutex)
			{
				this.AllKfServerId = new HashSet<int>(existKfIds);
				this.AutoKfServerId = new HashSet<int>(existKfIds);
				this.AutoKfServerId.ExceptWith(KuaFuServerManager.SpecialLineDict.Keys);
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002B70 File Offset: 0x00000D70
		public ClientAgent GetCurrentClientAgent(int serverId)
		{
			ClientAgent agent;
			ClientAgent result;
			if (this.ServerId2ClientAgent.TryGetValue(serverId, out agent))
			{
				result = agent;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002BA8 File Offset: 0x00000DA8
		public void OnConnectionClose(object sender, EventArgs args)
		{
			if (null != OperationContext.Current)
			{
				string sessionId = OperationContext.Current.SessionId;
				lock (this.Mutex)
				{
					ClientAgent agent;
					if (this.SessionId2ClientAgent.TryGetValue(sessionId, out agent))
					{
						agent.RemoveSession(sessionId);
					}
				}
			}
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002C28 File Offset: 0x00000E28
		public int InitializeClient(KuaFuClientContext clientInfo, out bool bFistInit)
		{
			bFistInit = false;
			if (KuaFuServerManager.LimitIP)
			{
				bool denied = true;
				KuaFuServerInfo serverInfo = KuaFuServerManager.GetKuaFuServerInfo(clientInfo.ServerId);
				if (null == serverInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("非法连接,无效的服务器编号#serverid={0},ip={1},gametype={2}", clientInfo.ServerId, clientInfo.Token, (GameTypes)clientInfo.GameType), null, true);
					return -1;
				}
				if (serverInfo != null && !string.IsNullOrEmpty(clientInfo.Token))
				{
					if (!string.IsNullOrEmpty(serverInfo.LanIp) && clientInfo.Token.Contains(serverInfo.LanIp))
					{
						denied = false;
					}
					else if (clientInfo.Token.Contains(serverInfo.Ip))
					{
						denied = false;
					}
				}
				if (denied)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("非法连接#serverid={0},ip={1},ip={2},lanip={3},gametype={4}", new object[]
					{
						clientInfo.ServerId,
						clientInfo.Token,
						serverInfo.Ip,
						serverInfo.LanIp,
						(GameTypes)clientInfo.GameType
					}), null, true);
					return -1;
				}
			}
			int clientId;
			lock (this.Mutex)
			{
				ClientAgent agent = null;
				if (!this.ServerId2ClientAgent.TryGetValue(clientInfo.ServerId, out agent))
				{
					LogManager.WriteLog(LogTypes.Info, string.Format("InitializeClient服务器连接1.ServerId:{0},ClientId:{1},info:{2},GameType:{3} [Service首次连接过来]", new object[]
					{
						clientInfo.ServerId,
						clientInfo.ClientId,
						clientInfo.Token,
						(GameTypes)clientInfo.GameType
					}), null, true);
					bFistInit = true;
					clientInfo.ClientId = KuaFuServerManager.GetUniqueClientId();
					agent = new ClientAgent(clientInfo);
					this.ServerId2ClientAgent[clientInfo.ServerId] = agent;
				}
				else if (clientInfo.Token != agent.ClientInfo.Token)
				{
					if (clientInfo.ClientId == agent.ClientInfo.ClientId)
					{
						LogManager.WriteLog(LogTypes.Info, string.Format("InitializeClient服务器IP变化.ServerId:{0},ClientId:{1},info:{2},GameType:{3}", new object[]
						{
							clientInfo.ServerId,
							clientInfo.ClientId,
							clientInfo.Token,
							(GameTypes)clientInfo.GameType
						}), null, true);
					}
					else
					{
						if (agent.IsAlive)
						{
							LogManager.WriteLog(LogTypes.Info, string.Format("InitializeClient服务器ID重复,禁止连接.ServerId:{0},ClientId:{1},info:{2},GameType:{3}", new object[]
							{
								clientInfo.ServerId,
								clientInfo.ClientId,
								clientInfo.Token,
								(GameTypes)clientInfo.GameType
							}), null, true);
							return -11002;
						}
						bFistInit = true;
						clientInfo.ClientId = KuaFuServerManager.GetUniqueClientId();
						agent.Reset(clientInfo);
						LogManager.WriteLog(LogTypes.Info, string.Format("InitializeClient服务器IP变化.ServerId:{0},ClientId:{1},info:{2},GameType:{3}", new object[]
						{
							clientInfo.ServerId,
							clientInfo.ClientId,
							clientInfo.Token,
							(GameTypes)clientInfo.GameType
						}), null, true);
					}
				}
				else if (clientInfo.ClientId != agent.ClientInfo.ClientId)
				{
					if (clientInfo.ClientId <= 0)
					{
						clientInfo.ClientId = agent.ClientInfo.ClientId;
						agent.Reset(clientInfo);
						LogManager.WriteLog(LogTypes.Info, string.Format("InitializeClient服务器重连.ServerId:{0},ClientId:{1},info:{2},GameType:{3} [首次连接过来]", new object[]
						{
							clientInfo.ServerId,
							clientInfo.ClientId,
							clientInfo.Token,
							(GameTypes)clientInfo.GameType
						}), null, true);
					}
					else
					{
						LogManager.WriteLog(LogTypes.Info, string.Format("InitializeClient服务器重连.ServerId:{0},ClientId:{1},info:{2},GameType:{3} [非首次连接过来]", new object[]
						{
							clientInfo.ServerId,
							clientInfo.ClientId,
							clientInfo.Token,
							(GameTypes)clientInfo.GameType
						}), null, true);
					}
				}
				else if (!Global.TestMode && !agent.IsAlive)
				{
					LogManager.WriteLog(LogTypes.Info, string.Format("InitializeClient服务器连接和上次心跳时间间隔过长.ServerId:{0},ClientId:{1},info:{2},GameType:{3},time={4}", new object[]
					{
						clientInfo.ServerId,
						clientInfo.ClientId,
						clientInfo.Token,
						(GameTypes)clientInfo.GameType,
						agent.DeltaTime
					}), null, true);
				}
				if (agent != null)
				{
					clientInfo.ClientId = agent.ClientInfo.ClientId;
					agent.ClientHeartTick();
					agent.TryInitGameType(clientInfo.GameType);
				}
				clientId = clientInfo.ClientId;
			}
			return clientId;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00003184 File Offset: 0x00001384
		public int InitializeClient(KuaFuClientContext clientInfo)
		{
			bool bPlaceHolder = false;
			return this.InitializeClient(clientInfo, out bPlaceHolder);
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000031A4 File Offset: 0x000013A4
		public void BroadCastAsyncEvent(GameTypes gameType, AsyncDataItem[] evItems)
		{
			if (evItems != null && evItems.Length > 0)
			{
				lock (this.Mutex)
				{
					for (int i = 0; i < evItems.Length; i++)
					{
						this.BroadCastAsyncEvent(gameType, evItems[i], 0);
					}
				}
			}
		}

		// Token: 0x06000025 RID: 37 RVA: 0x0000321C File Offset: 0x0000141C
		public void BroadCastAsyncEvent(GameTypes gameType, AsyncDataItem evItem, int srcServerId = 0)
		{
			if (evItem != null)
			{
				lock (this.Mutex)
				{
					foreach (int sid in this.ServerId2ClientAgent.Keys)
					{
						if (sid != srcServerId)
						{
							this.PostAsyncEvent(sid, gameType, evItem);
						}
					}
				}
			}
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000032CC File Offset: 0x000014CC
		public void KFBroadCastAsyncEvent(GameTypes gameType, AsyncDataItem evItem)
		{
			if (evItem != null)
			{
				lock (this.Mutex)
				{
					foreach (int sid in this.AllKfServerId)
					{
						this.PostAsyncEvent(sid, gameType, evItem);
					}
				}
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x0000336C File Offset: 0x0000156C
		public void PostAsyncEvent(int ServerId, GameTypes gameType, AsyncDataItem evItem)
		{
			lock (this.Mutex)
			{
				ClientAgent agent = null;
				if (this.ServerId2ClientAgent.TryGetValue(ServerId, out agent))
				{
					agent.PostAsyncEvent(gameType, evItem);
				}
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x000033D4 File Offset: 0x000015D4
		public AsyncDataItem[] PickAsyncEvent(int serverId, GameTypes gameType)
		{
			lock (this.Mutex)
			{
				ClientAgent agent = null;
				if (this.ServerId2ClientAgent.TryGetValue(serverId, out agent))
				{
					return agent.PickAsyncEvent(gameType);
				}
			}
			return null;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00003448 File Offset: 0x00001648
		public void BroadCastMsg(KFCallMsg msg, int srcServerId = 0)
		{
			lock (this.Mutex)
			{
				foreach (int sid in this.ServerId2ClientAgent.Keys)
				{
					if (sid != srcServerId)
					{
						this.SendMsg(sid, msg);
					}
				}
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000034F0 File Offset: 0x000016F0
		public void KFBroadCastMsg(KFCallMsg msg)
		{
			if (msg != null)
			{
				lock (this.Mutex)
				{
					foreach (int sid in this.AllKfServerId)
					{
						this.SendMsg(sid, msg);
					}
				}
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00003590 File Offset: 0x00001790
		public void SendMsg(int serverId, KFCallMsg msg)
		{
			lock (this.Mutex)
			{
				lock (this.Mutex)
				{
					ClientAgent agent;
					if (this.ServerId2ClientAgent.TryGetValue(serverId, out agent))
					{
						agent.PostAsyncEvent(msg);
					}
				}
			}
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00003630 File Offset: 0x00001830
		public void SendAsyncKuaFuMsg()
		{
			lock (this.Mutex)
			{
				foreach (ClientAgent agent in this.ServerId2ClientAgent.Values)
				{
					int sendCount = 0;
					if (agent.KFCallMsg != null && agent.AgentData.MsgQueue.Count > 0)
					{
						Queue<KFCallMsg> evQ = agent.AgentData.MsgQueue;
						do
						{
							try
							{
								KFCallMsg item = evQ.Peek();
								if (!agent.KFCallMsg(item))
								{
									break;
								}
								evQ.Dequeue();
							}
							catch (Exception ex)
							{
								LogManager.WriteException(ex.ToString());
								break;
							}
						}
						while (evQ.Count > 0 && sendCount++ < 2000);
					}
				}
			}
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00003774 File Offset: 0x00001974
		public bool AssginKfFuben(GameTypes gameType, long gameId, int roleNum, out int kfSrvId)
		{
			kfSrvId = 0;
			lock (this.Mutex)
			{
				long payload = long.MaxValue;
				ClientAgent assignAgent = null;
				foreach (int sid in this.AutoKfServerId)
				{
					ClientAgent agent = null;
					if (this.ServerId2ClientAgent.TryGetValue(sid, out agent) && agent.IsAlive && agent.TotalRolePayload < payload)
					{
						payload = agent.TotalRolePayload;
						assignAgent = agent;
					}
				}
				if (assignAgent != null)
				{
					assignAgent.AssginKfFuben(gameType, gameId, roleNum);
					kfSrvId = assignAgent.ClientInfo.ServerId;
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00003880 File Offset: 0x00001A80
		public bool SpecialKfFuben(GameTypes gameType, long gameId, int roleNum, out int kfSrvId)
		{
			kfSrvId = 0;
			ClientAgent assignAgent = null;
			int serverId = KuaFuServerManager.GetSpecialLineId(gameType);
			bool result;
			if (serverId <= 0)
			{
				kfSrvId = -3;
				result = false;
			}
			else
			{
				lock (this.Mutex)
				{
					ClientAgent agent = null;
					if (this.ServerId2ClientAgent.TryGetValue(serverId, out agent) && agent.IsAlive)
					{
						assignAgent = agent;
					}
					else
					{
						kfSrvId = -100;
					}
					if (assignAgent != null)
					{
						assignAgent.AssginKfFuben(gameType, gameId, roleNum);
						kfSrvId = assignAgent.ClientInfo.ServerId;
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00003950 File Offset: 0x00001B50
		public void RemoveKfFuben(GameTypes gameType, int kfSrvId, long gameId)
		{
			lock (this.Mutex)
			{
				ClientAgent agent = null;
				if (this.ServerId2ClientAgent.TryGetValue(kfSrvId, out agent))
				{
					agent.RemoveKfFuben(gameType, gameId);
				}
			}
		}

		// Token: 0x06000030 RID: 48 RVA: 0x000039B8 File Offset: 0x00001BB8
		public void SetMainlinePayload(int serverId, int payload)
		{
			lock (this.Mutex)
			{
				ClientAgent agent = null;
				if (this.ServerId2ClientAgent.TryGetValue(serverId, out agent))
				{
					agent.SetMainlinePayload(payload);
				}
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00003A20 File Offset: 0x00001C20
		public void SetGameTypeLoad(GameTypes gameType, int signUpCount, int startCount)
		{
			lock (this.Mutex)
			{
				GameTypeStaticsData data = null;
				if (!this.GameTypeLoadDict.TryGetValue((int)gameType, out data))
				{
					data = new GameTypeStaticsData();
					this.GameTypeLoadDict[(int)gameType] = data;
				}
				data.SingUpRoleCount = signUpCount;
				data.StartGameRoleCount = startCount;
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00003AA0 File Offset: 0x00001CA0
		public void GetServerState(int serverId, out int state, out int load)
		{
			state = 0;
			load = 0;
			lock (this.Mutex)
			{
				ClientAgent agent = null;
				if (this.ServerId2ClientAgent.TryGetValue(serverId, out agent))
				{
					if (agent.IsAlive)
					{
						state = 1;
						load = (int)agent.TotalRolePayload;
					}
				}
			}
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00003B20 File Offset: 0x00001D20
		public Dictionary<int, GameTypeStaticsData> GetGameTypeStatics()
		{
			Dictionary<int, GameTypeStaticsData> result = new Dictionary<int, GameTypeStaticsData>();
			lock (this.Mutex)
			{
				foreach (int serverId in this.AllKfServerId)
				{
					ClientAgent agent = null;
					if (this.ServerId2ClientAgent.TryGetValue(serverId, out agent))
					{
						foreach (KeyValuePair<int, GameTypeStaticsData> tmpKvp in agent.GetGameTypeStatics())
						{
							GameTypeStaticsData existData = null;
							if (!result.TryGetValue(tmpKvp.Key, out existData))
							{
								existData = new GameTypeStaticsData();
								result[tmpKvp.Key] = existData;
							}
							existData.ServerAlived += tmpKvp.Value.ServerAlived;
							existData.FuBenAlived += tmpKvp.Value.FuBenAlived;
							existData.SingUpRoleCount += tmpKvp.Value.SingUpRoleCount;
							existData.StartGameRoleCount += tmpKvp.Value.StartGameRoleCount;
						}
					}
				}
				foreach (KeyValuePair<int, GameTypeStaticsData> kvp in this.GameTypeLoadDict)
				{
					if (result.ContainsKey(kvp.Key))
					{
						result[kvp.Key].SingUpRoleCount = kvp.Value.SingUpRoleCount;
						result[kvp.Key].StartGameRoleCount = kvp.Value.StartGameRoleCount;
					}
				}
			}
			return result;
		}

		// Token: 0x0400000F RID: 15
		private static ClientAgentManager _AgentMgr = new ClientAgentManager();

		// Token: 0x04000010 RID: 16
		private object Mutex = new object();

		// Token: 0x04000011 RID: 17
		private Dictionary<int, ClientAgent> ServerId2ClientAgent = new Dictionary<int, ClientAgent>();

		// Token: 0x04000012 RID: 18
		private Dictionary<string, ClientAgent> SessionId2ClientAgent = new Dictionary<string, ClientAgent>();

		// Token: 0x04000013 RID: 19
		private HashSet<int> AllKfServerId = new HashSet<int>();

		// Token: 0x04000014 RID: 20
		private HashSet<int> AutoKfServerId = new HashSet<int>();

		// Token: 0x04000015 RID: 21
		private Dictionary<int, GameTypeStaticsData> GameTypeLoadDict = new Dictionary<int, GameTypeStaticsData>();
	}
}
