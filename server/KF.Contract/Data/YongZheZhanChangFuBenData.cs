using System;
using System.Collections.Generic;
using System.Linq;
using KF.Contract.Interface;

namespace KF.Contract.Data
{
	// Token: 0x02000018 RID: 24
	[Serializable]
	public class YongZheZhanChangFuBenData : IKuaFuFuBenData
	{
		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000077 RID: 119 RVA: 0x00003034 File Offset: 0x00001234
		// (set) Token: 0x06000078 RID: 120 RVA: 0x0000304B File Offset: 0x0000124B
		public int GameId { get; protected internal set; }

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000079 RID: 121 RVA: 0x00003054 File Offset: 0x00001254
		// (set) Token: 0x0600007A RID: 122 RVA: 0x0000306B File Offset: 0x0000126B
		public int ServerId { get; protected internal set; }

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600007B RID: 123 RVA: 0x00003074 File Offset: 0x00001274
		// (set) Token: 0x0600007C RID: 124 RVA: 0x0000308B File Offset: 0x0000128B
		public int SequenceId { get; set; }

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x0600007D RID: 125 RVA: 0x00003094 File Offset: 0x00001294
		// (set) Token: 0x0600007E RID: 126 RVA: 0x000030AB File Offset: 0x000012AB
		public int GroupIndex { get; protected internal set; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x0600007F RID: 127 RVA: 0x000030B4 File Offset: 0x000012B4
		// (set) Token: 0x06000080 RID: 128 RVA: 0x000030CB File Offset: 0x000012CB
		public GameFuBenState State { get; set; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000081 RID: 129 RVA: 0x000030D4 File Offset: 0x000012D4
		// (set) Token: 0x06000082 RID: 130 RVA: 0x000030EB File Offset: 0x000012EB
		public int Age { get; protected internal set; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000083 RID: 131 RVA: 0x000030F4 File Offset: 0x000012F4
		// (set) Token: 0x06000084 RID: 132 RVA: 0x0000310B File Offset: 0x0000130B
		public DateTime EndTime { get; protected internal set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000085 RID: 133 RVA: 0x00003114 File Offset: 0x00001314
		// (set) Token: 0x06000086 RID: 134 RVA: 0x0000312B File Offset: 0x0000132B
		public int RoleCountSide1 { get; protected internal set; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000087 RID: 135 RVA: 0x00003134 File Offset: 0x00001334
		// (set) Token: 0x06000088 RID: 136 RVA: 0x0000314B File Offset: 0x0000134B
		public int RoleCountSide2 { get; protected internal set; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000089 RID: 137 RVA: 0x00003154 File Offset: 0x00001354
		// (set) Token: 0x0600008A RID: 138 RVA: 0x0000316B File Offset: 0x0000136B
		public Dictionary<int, KuaFuFuBenRoleData> RoleDict { get; protected internal set; }

		// Token: 0x0600008B RID: 139 RVA: 0x00003174 File Offset: 0x00001374
		public YongZheZhanChangFuBenData()
		{
			this.RoleDict = new Dictionary<int, KuaFuFuBenRoleData>();
		}

		// Token: 0x0600008C RID: 140 RVA: 0x0000318C File Offset: 0x0000138C
		public YongZheZhanChangFuBenData Clone()
		{
			YongZheZhanChangFuBenData result;
			lock (this)
			{
				result = new YongZheZhanChangFuBenData
				{
					GameId = this.GameId,
					ServerId = this.ServerId,
					SequenceId = this.SequenceId,
					GroupIndex = this.GroupIndex,
					State = this.State,
					Age = this.Age,
					EndTime = this.EndTime,
					RoleCountSide1 = this.RoleCountSide1,
					RoleCountSide2 = this.RoleCountSide2,
					RoleDict = new Dictionary<int, KuaFuFuBenRoleData>(this.RoleDict)
				};
			}
			return result;
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00003260 File Offset: 0x00001460
		public int AddKuaFuFuBenRoleData(KuaFuFuBenRoleData kuaFuFuBenRoleData)
		{
			int roleCount = -1;
			lock (this)
			{
				if (this.RoleDict.Count < 220 && !this.RoleDict.ContainsKey(kuaFuFuBenRoleData.RoleId))
				{
					if (this.RoleCountSide1 < this.RoleCountSide2)
					{
						this.RoleCountSide1++;
						kuaFuFuBenRoleData.Side = 1;
					}
					else
					{
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

		// Token: 0x0600008E RID: 142 RVA: 0x00003348 File Offset: 0x00001548
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

		// Token: 0x0600008F RID: 143 RVA: 0x000033F8 File Offset: 0x000015F8
		public int GetFuBenRoleCount()
		{
			int count;
			lock (this)
			{
				count = this.RoleDict.Count;
			}
			return count;
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00003444 File Offset: 0x00001644
		public bool CanRemove()
		{
			lock (this)
			{
				if (this.State == GameFuBenState.End)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x000034C0 File Offset: 0x000016C0
		public List<KuaFuFuBenRoleData> SortFuBenRoleList()
		{
			List<KuaFuFuBenRoleData> result;
			lock (this)
			{
				List<KuaFuFuBenRoleData> roleList = this.RoleDict.Values.ToList<KuaFuFuBenRoleData>();
				roleList.Sort((KuaFuFuBenRoleData x, KuaFuFuBenRoleData y) => x.ZhanDouLi - y.ZhanDouLi);
				for (int i = 0; i < roleList.Count; i++)
				{
					int r = i % 4;
					if (r == 0 || r == 3)
					{
						roleList[i].Side = 1;
					}
					else
					{
						roleList[i].Side = 2;
					}
				}
				result = roleList;
			}
			return result;
		}
	}
}
