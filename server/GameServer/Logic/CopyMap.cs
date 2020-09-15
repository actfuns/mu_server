using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	// Token: 0x02000611 RID: 1553
	public class CopyMap
	{
		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x06001EF4 RID: 7924 RVA: 0x001AD88C File Offset: 0x001ABA8C
		// (set) Token: 0x06001EF5 RID: 7925 RVA: 0x001AD8A3 File Offset: 0x001ABAA3
		public int CopyMapID { get; set; }

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x06001EF6 RID: 7926 RVA: 0x001AD8AC File Offset: 0x001ABAAC
		// (set) Token: 0x06001EF7 RID: 7927 RVA: 0x001AD8C3 File Offset: 0x001ABAC3
		public int FuBenSeqID { get; set; }

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x06001EF8 RID: 7928 RVA: 0x001AD8CC File Offset: 0x001ABACC
		// (set) Token: 0x06001EF9 RID: 7929 RVA: 0x001AD8E3 File Offset: 0x001ABAE3
		public int MapCode { get; set; }

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x06001EFA RID: 7930 RVA: 0x001AD8EC File Offset: 0x001ABAEC
		// (set) Token: 0x06001EFB RID: 7931 RVA: 0x001AD903 File Offset: 0x001ABB03
		public int FubenMapID { get; set; }

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x06001EFC RID: 7932 RVA: 0x001AD90C File Offset: 0x001ABB0C
		// (set) Token: 0x06001EFD RID: 7933 RVA: 0x001AD923 File Offset: 0x001ABB23
		public MapTypes CopyMapType { get; set; }

		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x06001EFE RID: 7934 RVA: 0x001AD92C File Offset: 0x001ABB2C
		// (set) Token: 0x06001EFF RID: 7935 RVA: 0x001AD943 File Offset: 0x001ABB43
		public long InitTicks { get; set; }

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x06001F00 RID: 7936 RVA: 0x001AD94C File Offset: 0x001ABB4C
		// (set) Token: 0x06001F01 RID: 7937 RVA: 0x001AD963 File Offset: 0x001ABB63
		public bool bStoryCopyMapFinishStatus { get; set; }

		// Token: 0x06001F02 RID: 7938 RVA: 0x001AD96C File Offset: 0x001ABB6C
		public void SetKilledNormalDict(int monsterID)
		{
			lock (this)
			{
				if (-1 != monsterID)
				{
					this._KilledNormalDict[monsterID] = true;
				}
				else
				{
					this._KilledNormalDict.Clear();
				}
			}
		}

		// Token: 0x06001F03 RID: 7939 RVA: 0x001AD9D4 File Offset: 0x001ABBD4
		public void ClearKilledNormalDict()
		{
			lock (this)
			{
				this._KilledNormalDict.Clear();
			}
		}

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x06001F04 RID: 7940 RVA: 0x001ADA20 File Offset: 0x001ABC20
		public int KilledNormalNum
		{
			get
			{
				int count;
				lock (this)
				{
					count = this._KilledNormalDict.Count;
				}
				return count;
			}
		}

		// Token: 0x06001F05 RID: 7941 RVA: 0x001ADA6C File Offset: 0x001ABC6C
		public void SetKilledDynamicMonsterDict(long uniqueID)
		{
			lock (this)
			{
				if (-1L != uniqueID)
				{
					this._KilledDynamicMonsterDict[uniqueID] = true;
				}
				else
				{
					this._KilledDynamicMonsterDict.Clear();
				}
			}
		}

		// Token: 0x06001F06 RID: 7942 RVA: 0x001ADAD8 File Offset: 0x001ABCD8
		public void ClearKilledDynamicMonsterDict()
		{
			lock (this)
			{
				this._KilledDynamicMonsterDict.Clear();
			}
		}

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x06001F07 RID: 7943 RVA: 0x001ADB24 File Offset: 0x001ABD24
		public int KilledDynamicMonsterNum
		{
			get
			{
				int count;
				lock (this)
				{
					count = this._KilledDynamicMonsterDict.Count;
				}
				return count;
			}
		}

		// Token: 0x06001F08 RID: 7944 RVA: 0x001ADB70 File Offset: 0x001ABD70
		public void SetKilledBossDict(int monsterID)
		{
			lock (this)
			{
				if (-1 != monsterID)
				{
					this._KilledBossDict[monsterID] = true;
				}
				else
				{
					this._KilledBossDict.Clear();
				}
			}
		}

		// Token: 0x06001F09 RID: 7945 RVA: 0x001ADBD8 File Offset: 0x001ABDD8
		public void ClearKilledBossDict()
		{
			lock (this)
			{
				this._KilledBossDict.Clear();
			}
		}

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x06001F0A RID: 7946 RVA: 0x001ADC24 File Offset: 0x001ABE24
		public int KilledBossNum
		{
			get
			{
				int count;
				lock (this)
				{
					count = this._KilledBossDict.Count;
				}
				return count;
			}
		}

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x06001F0B RID: 7947 RVA: 0x001ADC70 File Offset: 0x001ABE70
		// (set) Token: 0x06001F0C RID: 7948 RVA: 0x001ADC87 File Offset: 0x001ABE87
		public int FreshPlayerCreateGateFlag { get; set; }

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x06001F0D RID: 7949 RVA: 0x001ADC90 File Offset: 0x001ABE90
		// (set) Token: 0x06001F0E RID: 7950 RVA: 0x001ADCA7 File Offset: 0x001ABEA7
		public int FreshPlayerKillMonsterACount { get; set; }

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x06001F0F RID: 7951 RVA: 0x001ADCB0 File Offset: 0x001ABEB0
		// (set) Token: 0x06001F10 RID: 7952 RVA: 0x001ADCC7 File Offset: 0x001ABEC7
		public int FreshPlayerKillMonsterBCount { get; set; }

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x06001F11 RID: 7953 RVA: 0x001ADCD0 File Offset: 0x001ABED0
		// (set) Token: 0x06001F12 RID: 7954 RVA: 0x001ADCE7 File Offset: 0x001ABEE7
		public bool HaveBirthShuiJingGuan { get; set; }

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x06001F13 RID: 7955 RVA: 0x001ADCF0 File Offset: 0x001ABEF0
		// (set) Token: 0x06001F14 RID: 7956 RVA: 0x001ADD07 File Offset: 0x001ABF07
		public bool ExecEnterMapLuaFile { get; set; }

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x06001F15 RID: 7957 RVA: 0x001ADD10 File Offset: 0x001ABF10
		// (set) Token: 0x06001F16 RID: 7958 RVA: 0x001ADD27 File Offset: 0x001ABF27
		public long CanRemoveTicks { get; private set; }

		// Token: 0x06001F17 RID: 7959 RVA: 0x001ADD30 File Offset: 0x001ABF30
		public void SetRemoveTicks(long ticks)
		{
			this.CanRemoveTicks = ticks;
		}

		// Token: 0x06001F18 RID: 7960 RVA: 0x001ADD3C File Offset: 0x001ABF3C
		public void AddGameClient(GameClient client)
		{
			lock (this._ClientsList)
			{
				this._ClientsList.Add(client);
			}
		}

		// Token: 0x06001F19 RID: 7961 RVA: 0x001ADD90 File Offset: 0x001ABF90
		public void RemoveGameClient(GameClient client)
		{
			long ticks = TimeUtil.NOW();
			lock (this._ClientsList)
			{
				this._ClientsList.Remove(client);
				this.LastLeaveClientTicks = ticks;
			}
		}

		// Token: 0x06001F1A RID: 7962 RVA: 0x001ADDF0 File Offset: 0x001ABFF0
		public List<GameClient> GetClientsList()
		{
			List<GameClient> newClientsList = null;
			lock (this._ClientsList)
			{
				newClientsList = this._ClientsList.GetRange(0, this._ClientsList.Count);
			}
			return newClientsList;
		}

		// Token: 0x06001F1B RID: 7963 RVA: 0x001ADE58 File Offset: 0x001AC058
		public List<object> GetClientsList2()
		{
			List<object> newClientsList = new List<object>(10);
			lock (this._ClientsList)
			{
				foreach (GameClient client in this._ClientsList)
				{
					newClientsList.Add(client);
				}
			}
			return newClientsList;
		}

		// Token: 0x06001F1C RID: 7964 RVA: 0x001ADEFC File Offset: 0x001AC0FC
		public long GetLastLeaveClientTicks()
		{
			long lastLeaveClientTicks;
			lock (this._ClientsList)
			{
				lastLeaveClientTicks = this.LastLeaveClientTicks;
			}
			return lastLeaveClientTicks;
		}

		// Token: 0x06001F1D RID: 7965 RVA: 0x001ADF48 File Offset: 0x001AC148
		public int GetGameClientCount()
		{
			int count;
			lock (this._ClientsList)
			{
				count = this._ClientsList.Count;
			}
			return count;
		}

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x06001F1E RID: 7966 RVA: 0x001ADF9C File Offset: 0x001AC19C
		// (set) Token: 0x06001F1F RID: 7967 RVA: 0x001ADFE4 File Offset: 0x001AC1E4
		public bool IsInitMonster
		{
			get
			{
				bool isInitMonster;
				lock (this)
				{
					isInitMonster = this._IsInitMonster;
				}
				return isInitMonster;
			}
			set
			{
				lock (this)
				{
					this._IsInitMonster = value;
				}
			}
		}

		// Token: 0x06001F20 RID: 7968 RVA: 0x001AE02C File Offset: 0x001AC22C
		public void AddGuangMuEvent(int guangMuID, int show)
		{
			MapAIEvent guangMuEvent = new MapAIEvent
			{
				GuangMuID = guangMuID,
				Show = show
			};
			lock (this.EventQueue)
			{
				this.EventQueue.Add(guangMuEvent);
			}
		}

		// Token: 0x04002BE5 RID: 11237
		public bool bNeedRemove = false;

		// Token: 0x04002BE6 RID: 11238
		public int TotalNormalNum = 0;

		// Token: 0x04002BE7 RID: 11239
		private Dictionary<int, bool> _KilledNormalDict = new Dictionary<int, bool>();

		// Token: 0x04002BE8 RID: 11240
		public int TotalDynamicMonsterNum = 0;

		// Token: 0x04002BE9 RID: 11241
		private Dictionary<long, bool> _KilledDynamicMonsterDict = new Dictionary<long, bool>();

		// Token: 0x04002BEA RID: 11242
		public int TotalBossNum = 0;

		// Token: 0x04002BEB RID: 11243
		private Dictionary<int, bool> _KilledBossDict = new Dictionary<int, bool>();

		// Token: 0x04002BEC RID: 11244
		public bool CopyMapPassAwardFlag = false;

		// Token: 0x04002BED RID: 11245
		public bool IsKuaFuCopy = false;

		// Token: 0x04002BEE RID: 11246
		public bool CustomPassAwards;

		// Token: 0x04002BEF RID: 11247
		private List<GameClient> _ClientsList = new List<GameClient>();

		// Token: 0x04002BF0 RID: 11248
		private long LastLeaveClientTicks = 0L;

		// Token: 0x04002BF1 RID: 11249
		private bool _IsInitMonster = false;

		// Token: 0x04002BF2 RID: 11250
		public List<MapAIEvent> EventQueue = new List<MapAIEvent>();
	}
}
