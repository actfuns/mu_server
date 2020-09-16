using System;
using System.Collections.Generic;
using System.ServiceModel;
using KF.Contract;
using KF.Contract.Data;
using KF.Remoting.Data;
using Server.Tools;

namespace KF.Remoting
{
	
	internal class ClientAgentManager
	{
		
		public static ClientAgentManager Instance()
		{
			return ClientAgentManager._AgentMgr;
		}

		
		private ClientAgentManager()
		{
		}

		
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

		
		public bool ExistAgent(int serverId)
		{
			bool result;
			lock (this.Mutex)
			{
				result = this.ServerId2ClientAgent.ContainsKey(serverId);
			}
			return result;
		}

		
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

		
		public bool IsKfAgent(int serverId)
		{
			bool result;
			lock (this.Mutex)
			{
				result = (this.AllKfServerId.Contains(serverId) && this.ExistAgent(serverId));
			}
			return result;
		}

		
		public void SetAllKfServerId(HashSet<int> existKfIds)
		{
			lock (this.Mutex)
			{
				this.AllKfServerId = new HashSet<int>(existKfIds);
				this.AutoKfServerId = new HashSet<int>(existKfIds);
				this.AutoKfServerId.ExceptWith(KuaFuServerManager.SpecialLineDict.Keys);
			}
		}

		
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

		
		public int InitializeClient(KuaFuClientContext clientInfo)
		{
			bool bPlaceHolder = false;
			return this.InitializeClient(clientInfo, out bPlaceHolder);
		}

		
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

		
		private static ClientAgentManager _AgentMgr = new ClientAgentManager();

		
		private object Mutex = new object();

		
		private Dictionary<int, ClientAgent> ServerId2ClientAgent = new Dictionary<int, ClientAgent>();

		
		private Dictionary<string, ClientAgent> SessionId2ClientAgent = new Dictionary<string, ClientAgent>();

		
		private HashSet<int> AllKfServerId = new HashSet<int>();

		
		private HashSet<int> AutoKfServerId = new HashSet<int>();

		
		private Dictionary<int, GameTypeStaticsData> GameTypeLoadDict = new Dictionary<int, GameTypeStaticsData>();
	}
}
