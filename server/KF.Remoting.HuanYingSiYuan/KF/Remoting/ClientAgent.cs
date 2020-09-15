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
	// Token: 0x02000002 RID: 2
	internal sealed class ClientAgent
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		// (set) Token: 0x06000002 RID: 2 RVA: 0x00002067 File Offset: 0x00000267
		public KuaFuClientContext ClientInfo { get; private set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000003 RID: 3 RVA: 0x00002070 File Offset: 0x00000270
		public bool IsAlive
		{
			get
			{
				return this.MaxActiveTicks > Global.NowTime.Ticks;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000004 RID: 4 RVA: 0x00002098 File Offset: 0x00000298
		public long DeltaTime
		{
			get
			{
				return (Global.NowTime.Ticks - this.MaxActiveTicks) / 10000000L;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000005 RID: 5 RVA: 0x000020C8 File Offset: 0x000002C8
		public bool IsDead
		{
			get
			{
				return this.MaxDeadTicks < Global.NowTime.Ticks;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000006 RID: 6 RVA: 0x000020F0 File Offset: 0x000002F0
		public long TotalRolePayload
		{
			get
			{
				return this.TotalFubenRolePayLoad + this.TotalMainlineRolePayLoad;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000007 RID: 7 RVA: 0x00002110 File Offset: 0x00000310
		// (set) Token: 0x06000008 RID: 8 RVA: 0x00002127 File Offset: 0x00000327
		public long TotalFubenRolePayLoad { get; private set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000009 RID: 9 RVA: 0x00002130 File Offset: 0x00000330
		// (set) Token: 0x0600000A RID: 10 RVA: 0x00002147 File Offset: 0x00000347
		public long TotalMainlineRolePayLoad { get; private set; }

		// Token: 0x0600000B RID: 11 RVA: 0x00002150 File Offset: 0x00000350
		public ClientAgent(KuaFuClientContext clientInfo)
		{
			this.ClientInfo = clientInfo;
			this.ClientHeartTick();
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000021C8 File Offset: 0x000003C8
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

		// Token: 0x0600000D RID: 13 RVA: 0x0000223C File Offset: 0x0000043C
		public void AddSession(string sessionId, int gameType, IKuaFuClient callback)
		{
			lock (this.Mutex)
			{
				this.SessionId2GameTypeDict[sessionId] = gameType;
				this.KuaFuClientDict[gameType] = callback;
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000022A0 File Offset: 0x000004A0
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

		// Token: 0x0600000F RID: 15 RVA: 0x00002330 File Offset: 0x00000530
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

		// Token: 0x06000010 RID: 16 RVA: 0x000024BC File Offset: 0x000006BC
		public void SetMainlinePayload(int payload)
		{
			lock (this.Mutex)
			{
				this.TotalMainlineRolePayLoad = (long)payload;
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x0000250C File Offset: 0x0000070C
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

		// Token: 0x06000012 RID: 18 RVA: 0x00002594 File Offset: 0x00000794
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

		// Token: 0x06000013 RID: 19 RVA: 0x00002620 File Offset: 0x00000820
		public void ClientHeartTick()
		{
			this.MaxActiveTicks = Global.NowTime.AddSeconds((double)Consts.RenewServerActiveTicks).Ticks;
			this.MaxDeadTicks = Global.NowTime.AddSeconds((double)Consts.RenewServerDeadTicks).Ticks;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002670 File Offset: 0x00000870
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

		// Token: 0x06000015 RID: 21 RVA: 0x00002714 File Offset: 0x00000914
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

		// Token: 0x06000016 RID: 22 RVA: 0x0000278C File Offset: 0x0000098C
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

		// Token: 0x06000017 RID: 23 RVA: 0x0000284C File Offset: 0x00000A4C
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

		// Token: 0x04000001 RID: 1
		private const int MaxCachedAsyncDataItemCount = 100000;

		// Token: 0x04000002 RID: 2
		private object Mutex = new object();

		// Token: 0x04000003 RID: 3
		public AgentDataObj AgentData = new AgentDataObj();

		// Token: 0x04000004 RID: 4
		public Func<ReturnValue<KFCallMsg>, bool> KFCallMsg;

		// Token: 0x04000005 RID: 5
		private Dictionary<int, IKuaFuClient> KuaFuClientDict = new Dictionary<int, IKuaFuClient>();

		// Token: 0x04000006 RID: 6
		private Dictionary<string, int> SessionId2GameTypeDict = new Dictionary<string, int>();

		// Token: 0x04000007 RID: 7
		private long MaxActiveTicks = 0L;

		// Token: 0x04000008 RID: 8
		private long MaxDeadTicks = 0L;

		// Token: 0x04000009 RID: 9
		private Dictionary<int, Queue<AsyncDataItem>> EvItemOfGameType = new Dictionary<int, Queue<AsyncDataItem>>();

		// Token: 0x0400000A RID: 10
		private Dictionary<int, Dictionary<long, int>> AlivedGameDict = new Dictionary<int, Dictionary<long, int>>();
	}
}
