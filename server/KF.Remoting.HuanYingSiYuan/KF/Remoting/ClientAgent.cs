using System;
using System.Collections.Generic;
using System.Linq;
using AutoCSer.Net.TcpServer;
using KF.Contract;
using KF.Contract.Data;
using KF.Contract.Interface;
using KF.Remoting.Data;

namespace KF.Remoting
{
	
	internal sealed class ClientAgent
	{
		
		
		
		public KuaFuClientContext ClientInfo { get; private set; }

		
		
		public bool IsAlive
		{
			get
			{
				return this.MaxActiveTicks > Global.NowTime.Ticks;
			}
		}

		
		
		public long DeltaTime
		{
			get
			{
				return (Global.NowTime.Ticks - this.MaxActiveTicks) / 10000000L;
			}
		}

		
		
		public bool IsDead
		{
			get
			{
				return this.MaxDeadTicks < Global.NowTime.Ticks;
			}
		}

		
		
		public long TotalRolePayload
		{
			get
			{
				return this.TotalFubenRolePayLoad + this.TotalMainlineRolePayLoad;
			}
		}

		
		
		
		public long TotalFubenRolePayLoad { get; private set; }

		
		
		
		public long TotalMainlineRolePayLoad { get; private set; }

		
		public ClientAgent(KuaFuClientContext clientInfo)
		{
			this.ClientInfo = clientInfo;
			this.ClientHeartTick();
		}

		
		public void Reset(KuaFuClientContext clientInfo)
		{
			lock (this.Mutex)
			{
				this.ClientInfo = clientInfo;
				this.ClientHeartTick();
				this.AlivedGameDict.Clear();
				this.TotalFubenRolePayLoad = 0L;
				this.TotalMainlineRolePayLoad = 0L;
			}
		}

		
		public void AddSession(string sessionId, int gameType, IKuaFuClient callback)
		{
			lock (this.Mutex)
			{
				this.SessionId2GameTypeDict[sessionId] = gameType;
				this.KuaFuClientDict[gameType] = callback;
			}
		}

		
		public void RemoveSession(string sessionId)
		{
			lock (this.Mutex)
			{
				int gameType;
				if (this.SessionId2GameTypeDict.TryGetValue(sessionId, out gameType))
				{
					this.KuaFuClientDict.Remove(gameType);
					this.SessionId2GameTypeDict.Remove(sessionId);
				}
			}
		}

		
		public Dictionary<int, GameTypeStaticsData> GetGameTypeStatics()
		{
			Dictionary<int, GameTypeStaticsData> result = new Dictionary<int, GameTypeStaticsData>();
			if (this.IsAlive)
			{
				lock (this.Mutex)
				{
					foreach (KeyValuePair<int, Dictionary<long, int>> kvp in this.AlivedGameDict)
					{
						GameTypeStaticsData data = null;
						if (!result.TryGetValue(kvp.Key, out data))
						{
							data = new GameTypeStaticsData();
							data.ServerAlived = 1;
							result[kvp.Key] = data;
						}
						data.FuBenAlived = kvp.Value.Count;
						data.SingUpRoleCount = 0;
						data.StartGameRoleCount = kvp.Value.ToList<KeyValuePair<long, int>>().Sum((KeyValuePair<long, int> c) => c.Value);
					}
					if (this.TotalMainlineRolePayLoad > 0L)
					{
						result.Add(7, new GameTypeStaticsData
						{
							ServerAlived = 1,
							StartGameRoleCount = (int)this.TotalMainlineRolePayLoad
						});
					}
				}
			}
			return result;
		}

		
		public void SetMainlinePayload(int payload)
		{
			lock (this.Mutex)
			{
				this.TotalMainlineRolePayLoad = (long)payload;
			}
		}

		
		public void AssginKfFuben(GameTypes gameType, long gameId, int roleNum)
		{
			lock (this.Mutex)
			{
				Dictionary<long, int> dict = null;
				if (!this.AlivedGameDict.TryGetValue((int)gameType, out dict))
				{
					dict = new Dictionary<long, int>();
					this.AlivedGameDict[(int)gameType] = dict;
				}
				dict.Add(gameId, roleNum);
				this.TotalFubenRolePayLoad += (long)roleNum;
			}
		}

		
		public void RemoveKfFuben(GameTypes gameType, long gameId)
		{
			lock (this.Mutex)
			{
				Dictionary<long, int> dict = null;
				if (this.AlivedGameDict.TryGetValue((int)gameType, out dict) && dict.ContainsKey(gameId))
				{
					this.TotalFubenRolePayLoad -= (long)dict[gameId];
					dict.Remove(gameId);
				}
			}
		}

		
		public void ClientHeartTick()
		{
			this.MaxActiveTicks = Global.NowTime.AddSeconds((double)Consts.RenewServerActiveTicks).Ticks;
			this.MaxDeadTicks = Global.NowTime.AddSeconds((double)Consts.RenewServerDeadTicks).Ticks;
		}

		
		public void PostAsyncEvent(GameTypes gameType, AsyncDataItem evItem)
		{
			lock (this.Mutex)
			{
				Queue<AsyncDataItem> evQ = null;
				if (!this.EvItemOfGameType.TryGetValue((int)gameType, out evQ))
				{
					evQ = new Queue<AsyncDataItem>();
					this.EvItemOfGameType[(int)gameType] = evQ;
				}
				evQ.Enqueue(evItem);
				while (evQ.Count > 100000)
				{
					evQ.Dequeue();
				}
			}
		}

		
		public void PostAsyncEvent(KFCallMsg evItem)
		{
			lock (this.Mutex)
			{
				Queue<KFCallMsg> evQ = this.AgentData.MsgQueue;
				evQ.Enqueue(evItem);
				while (evQ.Count > 100000)
				{
					evQ.Dequeue();
				}
			}
		}

		
		public AsyncDataItem[] PickAsyncEvent(GameTypes gameType)
		{
			lock (this.Mutex)
			{
				this.ClientHeartTick();
				Queue<AsyncDataItem> evQ = null;
				if (this.EvItemOfGameType.TryGetValue((int)gameType, out evQ))
				{
					int count = Math.Min(evQ.Count, KuaFuServerManager.MaxGetAsyncItemDataCount);
					if (count > 0)
					{
						AsyncDataItem[] result = new AsyncDataItem[count];
						for (int i = 0; i < count; i++)
						{
							result[i] = evQ.Dequeue();
						}
						return result;
					}
				}
			}
			return null;
		}

		
		public void TryInitGameType(int gameType)
		{
			lock (this.Mutex)
			{
				if (!this.AlivedGameDict.ContainsKey(gameType))
				{
					this.AlivedGameDict[gameType] = new Dictionary<long, int>();
				}
			}
		}

		
		private const int MaxCachedAsyncDataItemCount = 100000;

		
		private object Mutex = new object();

		
		public AgentDataObj AgentData = new AgentDataObj();

		
		public Func<ReturnValue<KFCallMsg>, bool> KFCallMsg;

		
		private Dictionary<int, IKuaFuClient> KuaFuClientDict = new Dictionary<int, IKuaFuClient>();

		
		private Dictionary<string, int> SessionId2GameTypeDict = new Dictionary<string, int>();

		
		private long MaxActiveTicks = 0L;

		
		private long MaxDeadTicks = 0L;

		
		private Dictionary<int, Queue<AsyncDataItem>> EvItemOfGameType = new Dictionary<int, Queue<AsyncDataItem>>();

		
		private Dictionary<int, Dictionary<long, int>> AlivedGameDict = new Dictionary<int, Dictionary<long, int>>();
	}
}
