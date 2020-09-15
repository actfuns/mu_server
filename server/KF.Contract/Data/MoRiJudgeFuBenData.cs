using System;
using System.Collections.Generic;
using System.Linq;
using KF.Contract.Interface;

namespace KF.Contract.Data
{
	// Token: 0x02000019 RID: 25
	[Serializable]
	public class MoRiJudgeFuBenData : IKuaFuFuBenData
	{
		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000093 RID: 147 RVA: 0x00003594 File Offset: 0x00001794
		// (set) Token: 0x06000094 RID: 148 RVA: 0x000035AB File Offset: 0x000017AB
		public int GameId { get; protected internal set; }

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000095 RID: 149 RVA: 0x000035B4 File Offset: 0x000017B4
		// (set) Token: 0x06000096 RID: 150 RVA: 0x000035CB File Offset: 0x000017CB
		public int ServerId { get; protected internal set; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000097 RID: 151 RVA: 0x000035D4 File Offset: 0x000017D4
		// (set) Token: 0x06000098 RID: 152 RVA: 0x000035EB File Offset: 0x000017EB
		public int SequenceId { get; protected internal set; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000099 RID: 153 RVA: 0x000035F4 File Offset: 0x000017F4
		// (set) Token: 0x0600009A RID: 154 RVA: 0x0000360B File Offset: 0x0000180B
		public int GroupIndex { get; protected internal set; }

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600009B RID: 155 RVA: 0x00003614 File Offset: 0x00001814
		// (set) Token: 0x0600009C RID: 156 RVA: 0x0000362B File Offset: 0x0000182B
		public GameFuBenState State { get; protected internal set; }

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x0600009D RID: 157 RVA: 0x00003634 File Offset: 0x00001834
		// (set) Token: 0x0600009E RID: 158 RVA: 0x0000364B File Offset: 0x0000184B
		public int Age { get; protected internal set; }

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x0600009F RID: 159 RVA: 0x00003654 File Offset: 0x00001854
		// (set) Token: 0x060000A0 RID: 160 RVA: 0x0000366B File Offset: 0x0000186B
		public DateTime EndTime { get; protected internal set; }

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060000A1 RID: 161 RVA: 0x00003674 File Offset: 0x00001874
		// (set) Token: 0x060000A2 RID: 162 RVA: 0x0000368B File Offset: 0x0000188B
		public long TeamCombatSum { get; protected internal set; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000A3 RID: 163 RVA: 0x00003694 File Offset: 0x00001894
		// (set) Token: 0x060000A4 RID: 164 RVA: 0x000036AB File Offset: 0x000018AB
		public Dictionary<int, KuaFuFuBenRoleData> RoleDict { get; protected internal set; }

		// Token: 0x060000A5 RID: 165 RVA: 0x000036B4 File Offset: 0x000018B4
		public MoRiJudgeFuBenData()
		{
			this.RoleDict = new Dictionary<int, KuaFuFuBenRoleData>();
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x000036CC File Offset: 0x000018CC
		public MoRiJudgeFuBenData Clone()
		{
			MoRiJudgeFuBenData result;
			lock (this)
			{
				result = new MoRiJudgeFuBenData
				{
					GameId = this.GameId,
					ServerId = this.ServerId,
					SequenceId = this.SequenceId,
					GroupIndex = this.GroupIndex,
					State = this.State,
					Age = this.Age,
					EndTime = this.EndTime,
					TeamCombatSum = this.TeamCombatSum,
					RoleDict = new Dictionary<int, KuaFuFuBenRoleData>(this.RoleDict)
				};
			}
			return result;
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00003790 File Offset: 0x00001990
		public int AddKuaFuFuBenRoleData(KuaFuFuBenRoleData kuaFuFuBenRoleData, int maxRoleCount)
		{
			int roleCount = -1;
			lock (this)
			{
				if (this.RoleDict.Count < maxRoleCount && !this.RoleDict.ContainsKey(kuaFuFuBenRoleData.RoleId))
				{
					this.TeamCombatSum += (long)kuaFuFuBenRoleData.ZhanDouLi;
					this.RoleDict[kuaFuFuBenRoleData.RoleId] = kuaFuFuBenRoleData;
					roleCount = this.RoleDict.Count;
					return roleCount;
				}
			}
			return roleCount;
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x0000383C File Offset: 0x00001A3C
		public int RemoveKuaFuFuBenRoleData(int roleId)
		{
			int result;
			lock (this)
			{
				KuaFuFuBenRoleData kuaFuFuBenRoleData;
				if (this.RoleDict.TryGetValue(roleId, out kuaFuFuBenRoleData))
				{
					this.RoleDict.Remove(roleId);
					this.TeamCombatSum -= (long)kuaFuFuBenRoleData.ZhanDouLi;
					this.TeamCombatSum = Math.Max(this.TeamCombatSum, 0L);
				}
				int roleCount = this.RoleDict.Count;
				result = roleCount;
			}
			return result;
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x000038E0 File Offset: 0x00001AE0
		public int GetFuBenRoleCount()
		{
			int count;
			lock (this)
			{
				count = this.RoleDict.Count;
			}
			return count;
		}

		// Token: 0x060000AA RID: 170 RVA: 0x0000392C File Offset: 0x00001B2C
		public bool CanRemove()
		{
			lock (this)
			{
				if (this.State == GameFuBenState.End)
				{
					return true;
				}
				if (this.State == GameFuBenState.Start && this.RoleDict.Count == 0)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x000039B0 File Offset: 0x00001BB0
		public List<KuaFuFuBenRoleData> GetRoleList()
		{
			List<KuaFuFuBenRoleData> result;
			lock (this)
			{
				result = this.RoleDict.Values.ToList<KuaFuFuBenRoleData>();
			}
			return result;
		}
	}
}
