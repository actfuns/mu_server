using System;
using System.Collections.Generic;
using KF.Contract.Interface;

namespace KF.Contract.Data
{
	
	[Serializable]
	public class ElementWarFuBenData : IKuaFuFuBenData
	{
		
		
		
		public int GameId { get; protected internal set; }

		
		
		
		public int ServerId { get; protected internal set; }

		
		
		
		public int SequenceId { get; protected internal set; }

		
		
		
		public GameFuBenState State { get; protected internal set; }

		
		
		
		public int Age { get; protected internal set; }

		
		
		
		public DateTime EndTime { get; protected internal set; }

		
		
		
		public long TeamCombatSum { get; protected internal set; }

		
		
		
		public int RoleCount { get; protected internal set; }

		
		
		
		public Dictionary<int, KuaFuFuBenRoleData> RoleDict { get; protected internal set; }

		
		public ElementWarFuBenData()
		{
			this.RoleDict = new Dictionary<int, KuaFuFuBenRoleData>();
		}

		
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

		
		public int GetFuBenRoleCount()
		{
			int count;
			lock (this)
			{
				count = this.RoleDict.Count;
			}
			return count;
		}

		
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
