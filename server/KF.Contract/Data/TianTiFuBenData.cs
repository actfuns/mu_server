using System;
using System.Collections.Generic;
using KF.Contract.Interface;

namespace KF.Contract.Data
{
	// Token: 0x02000016 RID: 22
	[Serializable]
	public class TianTiFuBenData : IKuaFuFuBenData
	{
		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600005C RID: 92 RVA: 0x00002B94 File Offset: 0x00000D94
		// (set) Token: 0x0600005D RID: 93 RVA: 0x00002BAB File Offset: 0x00000DAB
		public int GameId { get; protected internal set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600005E RID: 94 RVA: 0x00002BB4 File Offset: 0x00000DB4
		// (set) Token: 0x0600005F RID: 95 RVA: 0x00002BCB File Offset: 0x00000DCB
		public int ServerId { get; protected internal set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000060 RID: 96 RVA: 0x00002BD4 File Offset: 0x00000DD4
		// (set) Token: 0x06000061 RID: 97 RVA: 0x00002BEB File Offset: 0x00000DEB
		public int SequenceId { get; protected internal set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000062 RID: 98 RVA: 0x00002BF4 File Offset: 0x00000DF4
		// (set) Token: 0x06000063 RID: 99 RVA: 0x00002C0B File Offset: 0x00000E0B
		public int GroupIndex { get; protected internal set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000064 RID: 100 RVA: 0x00002C14 File Offset: 0x00000E14
		// (set) Token: 0x06000065 RID: 101 RVA: 0x00002C2B File Offset: 0x00000E2B
		public GameFuBenState State { get; protected internal set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000066 RID: 102 RVA: 0x00002C34 File Offset: 0x00000E34
		// (set) Token: 0x06000067 RID: 103 RVA: 0x00002C4B File Offset: 0x00000E4B
		public int Age { get; protected internal set; }

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000068 RID: 104 RVA: 0x00002C54 File Offset: 0x00000E54
		// (set) Token: 0x06000069 RID: 105 RVA: 0x00002C6B File Offset: 0x00000E6B
		public DateTime EndTime { get; protected internal set; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600006A RID: 106 RVA: 0x00002C74 File Offset: 0x00000E74
		// (set) Token: 0x0600006B RID: 107 RVA: 0x00002C8B File Offset: 0x00000E8B
		public int RoleCountSide1 { get; protected internal set; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600006C RID: 108 RVA: 0x00002C94 File Offset: 0x00000E94
		// (set) Token: 0x0600006D RID: 109 RVA: 0x00002CAB File Offset: 0x00000EAB
		public int RoleCountSide2 { get; protected internal set; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600006E RID: 110 RVA: 0x00002CB4 File Offset: 0x00000EB4
		// (set) Token: 0x0600006F RID: 111 RVA: 0x00002CCB File Offset: 0x00000ECB
		public Dictionary<int, KuaFuFuBenRoleData> RoleDict { get; protected internal set; }

		// Token: 0x06000070 RID: 112 RVA: 0x00002CD4 File Offset: 0x00000ED4
		public TianTiFuBenData()
		{
			this.RoleDict = new Dictionary<int, KuaFuFuBenRoleData>();
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00002CEC File Offset: 0x00000EEC
		public TianTiFuBenData Clone()
		{
			TianTiFuBenData result;
			lock (this)
			{
				result = new TianTiFuBenData
				{
					GameId = this.GameId,
					ServerId = this.ServerId,
					SequenceId = this.SequenceId,
					GroupIndex = this.GroupIndex,
					State = this.State,
					Age = this.Age,
					EndTime = this.EndTime,
					RoleDict = new Dictionary<int, KuaFuFuBenRoleData>(this.RoleDict)
				};
			}
			return result;
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00002DA4 File Offset: 0x00000FA4
		public int AddKuaFuFuBenRoleData(KuaFuFuBenRoleData kuaFuFuBenRoleData)
		{
			int roleCount = -1;
			lock (this)
			{
				if (this.RoleDict.Count < 2 && !this.RoleDict.ContainsKey(kuaFuFuBenRoleData.RoleId))
				{
					if (this.RoleCountSide1 < 1)
					{
						this.RoleCountSide1++;
						kuaFuFuBenRoleData.Side = 1;
					}
					else
					{
						if (this.RoleCountSide2 >= 1)
						{
							return roleCount;
						}
						this.RoleCountSide2++;
						kuaFuFuBenRoleData.Side = 2;
					}
					this.RoleDict[kuaFuFuBenRoleData.RoleId] = kuaFuFuBenRoleData;
					roleCount = this.RoleDict.Count;
					return roleCount;
				}
			}
			return roleCount;
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00002EA0 File Offset: 0x000010A0
		public int RemoveKuaFuFuBenRoleData(int roleId)
		{
			int result;
			lock (this)
			{
				KuaFuFuBenRoleData kuaFuFuBenRoleData;
				if (this.RoleDict.TryGetValue(roleId, out kuaFuFuBenRoleData))
				{
					this.RoleDict.Remove(roleId);
					if (kuaFuFuBenRoleData.Side == 1)
					{
						this.RoleCountSide1--;
					}
					else
					{
						this.RoleCountSide2--;
					}
				}
				int roleCount = this.RoleDict.Count;
				result = roleCount;
			}
			return result;
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00002F50 File Offset: 0x00001150
		public int GetFuBenRoleCount()
		{
			int count;
			lock (this)
			{
				count = this.RoleDict.Count;
			}
			return count;
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00002F9C File Offset: 0x0000119C
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
	}
}
