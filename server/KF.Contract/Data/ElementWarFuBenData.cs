using System;
using System.Collections.Generic;
using KF.Contract.Interface;

namespace KF.Contract.Data
{
	// Token: 0x0200001A RID: 26
	[Serializable]
	public class ElementWarFuBenData : IKuaFuFuBenData
	{
		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000AC RID: 172 RVA: 0x00003A04 File Offset: 0x00001C04
		// (set) Token: 0x060000AD RID: 173 RVA: 0x00003A1B File Offset: 0x00001C1B
		public int GameId { get; protected internal set; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000AE RID: 174 RVA: 0x00003A24 File Offset: 0x00001C24
		// (set) Token: 0x060000AF RID: 175 RVA: 0x00003A3B File Offset: 0x00001C3B
		public int ServerId { get; protected internal set; }

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000B0 RID: 176 RVA: 0x00003A44 File Offset: 0x00001C44
		// (set) Token: 0x060000B1 RID: 177 RVA: 0x00003A5B File Offset: 0x00001C5B
		public int SequenceId { get; protected internal set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x00003A64 File Offset: 0x00001C64
		// (set) Token: 0x060000B3 RID: 179 RVA: 0x00003A7B File Offset: 0x00001C7B
		public GameFuBenState State { get; protected internal set; }

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x00003A84 File Offset: 0x00001C84
		// (set) Token: 0x060000B5 RID: 181 RVA: 0x00003A9B File Offset: 0x00001C9B
		public int Age { get; protected internal set; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x00003AA4 File Offset: 0x00001CA4
		// (set) Token: 0x060000B7 RID: 183 RVA: 0x00003ABB File Offset: 0x00001CBB
		public DateTime EndTime { get; protected internal set; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x00003AC4 File Offset: 0x00001CC4
		// (set) Token: 0x060000B9 RID: 185 RVA: 0x00003ADB File Offset: 0x00001CDB
		public long TeamCombatSum { get; protected internal set; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060000BA RID: 186 RVA: 0x00003AE4 File Offset: 0x00001CE4
		// (set) Token: 0x060000BB RID: 187 RVA: 0x00003AFB File Offset: 0x00001CFB
		public int RoleCount { get; protected internal set; }

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000BC RID: 188 RVA: 0x00003B04 File Offset: 0x00001D04
		// (set) Token: 0x060000BD RID: 189 RVA: 0x00003B1B File Offset: 0x00001D1B
		public Dictionary<int, KuaFuFuBenRoleData> RoleDict { get; protected internal set; }

		// Token: 0x060000BE RID: 190 RVA: 0x00003B24 File Offset: 0x00001D24
		public ElementWarFuBenData()
		{
			this.RoleDict = new Dictionary<int, KuaFuFuBenRoleData>();
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00003B3C File Offset: 0x00001D3C
		public ElementWarFuBenData Clone()
		{
			ElementWarFuBenData result;
			lock (this)
			{
				result = new ElementWarFuBenData
				{
					GameId = this.GameId,
					ServerId = this.ServerId,
					SequenceId = this.SequenceId,
					State = this.State,
					Age = this.Age,
					EndTime = this.EndTime,
					TeamCombatSum = this.TeamCombatSum,
					RoleCount = this.RoleCount,
					RoleDict = new Dictionary<int, KuaFuFuBenRoleData>(this.RoleDict)
				};
			}
			return result;
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00003C00 File Offset: 0x00001E00
		public int AddKuaFuFuBenRoleData(KuaFuFuBenRoleData kuaFuFuBenRoleData, int maxRoleCount, GameElementWarRoleCountChanged handler)
		{
			int roleCount = -1;
			lock (this)
			{
				if (this.RoleDict.Count < maxRoleCount && !this.RoleDict.ContainsKey(kuaFuFuBenRoleData.RoleId))
				{
					this.RoleCount++;
					this.TeamCombatSum += (long)kuaFuFuBenRoleData.ZhanDouLi;
					this.RoleDict[kuaFuFuBenRoleData.RoleId] = kuaFuFuBenRoleData;
					roleCount = this.RoleDict.Count;
					if (null != handler)
					{
						handler(this, roleCount);
					}
					return roleCount;
				}
			}
			return roleCount;
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00003CD0 File Offset: 0x00001ED0
		public int RemoveKuaFuFuBenRoleData(int roleId, GameElementWarRoleCountChanged handler)
		{
			bool changed = false;
			int result;
			lock (this)
			{
				KuaFuFuBenRoleData kuaFuFuBenRoleData;
				if (this.RoleDict.TryGetValue(roleId, out kuaFuFuBenRoleData))
				{
					this.RoleDict.Remove(roleId);
					changed = true;
					this.RoleCount--;
					this.TeamCombatSum -= (long)kuaFuFuBenRoleData.ZhanDouLi;
					this.TeamCombatSum = Math.Max(this.TeamCombatSum, 0L);
				}
				int roleCount = this.RoleDict.Count;
				if (changed && null != handler)
				{
					handler(this, roleCount);
				}
				result = roleCount;
			}
			return result;
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00003DA4 File Offset: 0x00001FA4
		public int GetFuBenRoleCount()
		{
			int count;
			lock (this)
			{
				count = this.RoleDict.Count;
			}
			return count;
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00003DF0 File Offset: 0x00001FF0
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

		// Token: 0x060000C4 RID: 196 RVA: 0x00003E74 File Offset: 0x00002074
		public bool CanEnter(int maxRoleCount)
		{
			if (this.State == GameFuBenState.Wait)
			{
				if (this.RoleCount < maxRoleCount)
				{
					return true;
				}
			}
			return false;
		}
	}
}
