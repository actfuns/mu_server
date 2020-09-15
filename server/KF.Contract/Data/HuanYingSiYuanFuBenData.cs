using System;
using System.Collections.Generic;
using System.Linq;
using KF.Contract.Interface;

namespace KF.Contract.Data
{
	// Token: 0x02000015 RID: 21
	[Serializable]
	public class HuanYingSiYuanFuBenData : IKuaFuFuBenData
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600003F RID: 63 RVA: 0x0000254C File Offset: 0x0000074C
		// (set) Token: 0x06000040 RID: 64 RVA: 0x00002563 File Offset: 0x00000763
		public int GameId { get; protected internal set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000041 RID: 65 RVA: 0x0000256C File Offset: 0x0000076C
		// (set) Token: 0x06000042 RID: 66 RVA: 0x00002583 File Offset: 0x00000783
		public int ServerId { get; protected internal set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000043 RID: 67 RVA: 0x0000258C File Offset: 0x0000078C
		// (set) Token: 0x06000044 RID: 68 RVA: 0x000025A3 File Offset: 0x000007A3
		public int SequenceId { get; protected internal set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000045 RID: 69 RVA: 0x000025AC File Offset: 0x000007AC
		// (set) Token: 0x06000046 RID: 70 RVA: 0x000025C3 File Offset: 0x000007C3
		public int GroupIndex { get; protected internal set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000047 RID: 71 RVA: 0x000025CC File Offset: 0x000007CC
		// (set) Token: 0x06000048 RID: 72 RVA: 0x000025E3 File Offset: 0x000007E3
		public GameFuBenState State { get; protected internal set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000049 RID: 73 RVA: 0x000025EC File Offset: 0x000007EC
		// (set) Token: 0x0600004A RID: 74 RVA: 0x00002603 File Offset: 0x00000803
		public int Age { get; protected internal set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600004B RID: 75 RVA: 0x0000260C File Offset: 0x0000080C
		// (set) Token: 0x0600004C RID: 76 RVA: 0x00002623 File Offset: 0x00000823
		public DateTime EndTime { get; protected internal set; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600004D RID: 77 RVA: 0x0000262C File Offset: 0x0000082C
		// (set) Token: 0x0600004E RID: 78 RVA: 0x00002643 File Offset: 0x00000843
		public int RoleCountSide1 { get; protected internal set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600004F RID: 79 RVA: 0x0000264C File Offset: 0x0000084C
		// (set) Token: 0x06000050 RID: 80 RVA: 0x00002663 File Offset: 0x00000863
		public int RoleCountSide2 { get; protected internal set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000051 RID: 81 RVA: 0x0000266C File Offset: 0x0000086C
		// (set) Token: 0x06000052 RID: 82 RVA: 0x00002683 File Offset: 0x00000883
		public Dictionary<int, KuaFuFuBenRoleData> RoleDict { get; protected internal set; }

		// Token: 0x06000053 RID: 83 RVA: 0x0000268C File Offset: 0x0000088C
		public HuanYingSiYuanFuBenData()
		{
			this.RoleDict = new Dictionary<int, KuaFuFuBenRoleData>();
		}

		// Token: 0x06000054 RID: 84 RVA: 0x000026A4 File Offset: 0x000008A4
		public HuanYingSiYuanFuBenData Clone()
		{
			HuanYingSiYuanFuBenData result;
			lock (this)
			{
				result = new HuanYingSiYuanFuBenData
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

		// Token: 0x06000055 RID: 85 RVA: 0x0000275C File Offset: 0x0000095C
		public int AddKuaFuFuBenRoleData(KuaFuFuBenRoleData kuaFuFuBenRoleData, GameFuBenRoleCountChanged handler)
		{
			int roleCount = -1;
			lock (this)
			{
				if (this.RoleDict.Count < Consts.HuanYingSiYuanRoleCountTotal && !this.RoleDict.ContainsKey(kuaFuFuBenRoleData.RoleId))
				{
					if (this.RoleCountSide1 < Consts.HuanYingSiYuanRoleCountPerSide)
					{
						this.RoleCountSide1++;
						kuaFuFuBenRoleData.Side = 1;
					}
					else
					{
						if (this.RoleCountSide2 >= Consts.HuanYingSiYuanRoleCountPerSide)
						{
							return roleCount;
						}
						this.RoleCountSide2++;
						kuaFuFuBenRoleData.Side = 2;
					}
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

		// Token: 0x06000056 RID: 86 RVA: 0x00002878 File Offset: 0x00000A78
		public int RemoveKuaFuFuBenRoleData(int roleId, GameFuBenRoleCountChanged handler)
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
				if (changed && null != handler)
				{
					handler(this, roleCount);
				}
				result = roleCount;
			}
			return result;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00002948 File Offset: 0x00000B48
		public int GetFuBenRoleCount()
		{
			int count;
			lock (this)
			{
				count = this.RoleDict.Count;
			}
			return count;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00002994 File Offset: 0x00000B94
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

		// Token: 0x06000059 RID: 89 RVA: 0x00002A18 File Offset: 0x00000C18
		public bool CanEnter(int groupIndex, long waitTicks1, long waitTicks2)
		{
			if (this.State == GameFuBenState.Wait)
			{
				if (this.EndTime.Ticks < waitTicks2)
				{
					return true;
				}
				if (this.EndTime.Ticks < waitTicks1)
				{
					if (groupIndex >= this.GroupIndex - 1 && groupIndex <= this.GroupIndex + 1)
					{
						return true;
					}
				}
			}
			return this.GroupIndex == groupIndex;
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00002AC0 File Offset: 0x00000CC0
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
