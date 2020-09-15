using System;
using KF.Contract.Interface;

namespace KF.Contract.Data
{
	// Token: 0x02000010 RID: 16
	[Serializable]
	public class KuaFuRoleData : ICloneable
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000013 RID: 19 RVA: 0x0000218C File Offset: 0x0000038C
		// (set) Token: 0x06000014 RID: 20 RVA: 0x000021A3 File Offset: 0x000003A3
		public int Age { get; protected internal set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000015 RID: 21 RVA: 0x000021AC File Offset: 0x000003AC
		// (set) Token: 0x06000016 RID: 22 RVA: 0x000021C3 File Offset: 0x000003C3
		public string UserId { get; protected internal set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000017 RID: 23 RVA: 0x000021CC File Offset: 0x000003CC
		// (set) Token: 0x06000018 RID: 24 RVA: 0x000021E3 File Offset: 0x000003E3
		public int ServerId { get; protected internal set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000019 RID: 25 RVA: 0x000021EC File Offset: 0x000003EC
		// (set) Token: 0x0600001A RID: 26 RVA: 0x00002203 File Offset: 0x00000403
		public int ZoneId { get; protected internal set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600001B RID: 27 RVA: 0x0000220C File Offset: 0x0000040C
		// (set) Token: 0x0600001C RID: 28 RVA: 0x00002223 File Offset: 0x00000423
		public int RoleId { get; protected internal set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600001D RID: 29 RVA: 0x0000222C File Offset: 0x0000042C
		// (set) Token: 0x0600001E RID: 30 RVA: 0x00002243 File Offset: 0x00000443
		public KuaFuRoleStates State { get; protected internal set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600001F RID: 31 RVA: 0x0000224C File Offset: 0x0000044C
		// (set) Token: 0x06000020 RID: 32 RVA: 0x00002263 File Offset: 0x00000463
		public int GameId { get; protected internal set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000021 RID: 33 RVA: 0x0000226C File Offset: 0x0000046C
		// (set) Token: 0x06000022 RID: 34 RVA: 0x00002283 File Offset: 0x00000483
		public int GameType { get; protected internal set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000023 RID: 35 RVA: 0x0000228C File Offset: 0x0000048C
		// (set) Token: 0x06000024 RID: 36 RVA: 0x000022A3 File Offset: 0x000004A3
		public int GroupIndex { get; protected internal set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000025 RID: 37 RVA: 0x000022AC File Offset: 0x000004AC
		// (set) Token: 0x06000026 RID: 38 RVA: 0x000022C3 File Offset: 0x000004C3
		public int KuaFuServerId { get; protected internal set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000027 RID: 39 RVA: 0x000022CC File Offset: 0x000004CC
		// (set) Token: 0x06000028 RID: 40 RVA: 0x000022E3 File Offset: 0x000004E3
		public int ZhanDouLi { get; protected internal set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000029 RID: 41 RVA: 0x000022EC File Offset: 0x000004EC
		// (set) Token: 0x0600002A RID: 42 RVA: 0x00002303 File Offset: 0x00000503
		public long StateEndTicks { get; protected internal set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600002B RID: 43 RVA: 0x0000230C File Offset: 0x0000050C
		// (set) Token: 0x0600002C RID: 44 RVA: 0x00002323 File Offset: 0x00000523
		public IGameData GameData { get; protected internal set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600002D RID: 45 RVA: 0x0000232C File Offset: 0x0000052C
		// (set) Token: 0x0600002E RID: 46 RVA: 0x00002343 File Offset: 0x00000543
		public int TeamCombatAvg { get; protected internal set; }

		// Token: 0x0600002F RID: 47 RVA: 0x0000234C File Offset: 0x0000054C
		public object Clone()
		{
			return new KuaFuRoleData
			{
				Age = this.Age,
				UserId = this.UserId,
				ServerId = this.ServerId,
				ZoneId = this.ZoneId,
				RoleId = this.RoleId,
				State = this.State,
				GameId = this.GameId,
				GameType = this.GameType,
				GroupIndex = this.GroupIndex,
				ZhanDouLi = this.ZhanDouLi,
				StateEndTicks = this.StateEndTicks,
				GameData = ((this.GameData == null) ? null : ((IGameData)this.GameData.Clone())),
				TeamCombatAvg = this.TeamCombatAvg
			};
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002424 File Offset: 0x00000624
		public void UpdateStateTime(int gameId, KuaFuRoleStates state, long stateEndTicks)
		{
			lock (this)
			{
				this.Age++;
				this.GameId = gameId;
				this.State = state;
				this.StateEndTicks = stateEndTicks;
			}
		}

		// Token: 0x0400003C RID: 60
		[NonSerialized]
		public KuaFuRoleData Next;
	}
}
