using System;
using System.Collections.Generic;
using System.Linq;
using KF.Contract.Interface;

namespace KF.Contract.Data
{
	
	[Serializable]
	public class MoRiJudgeFuBenData : IKuaFuFuBenData
	{
		
		
		
		public int GameId { get; protected internal set; }

		
		
		
		public int ServerId { get; protected internal set; }

		
		
		
		public int SequenceId { get; protected internal set; }

		
		
		
		public int GroupIndex { get; protected internal set; }

		
		
		
		public GameFuBenState State { get; protected internal set; }

		
		
		
		public int Age { get; protected internal set; }

		
		
		
		public DateTime EndTime { get; protected internal set; }

		
		
		
		public long TeamCombatSum { get; protected internal set; }

		
		
		
		public Dictionary<int, KuaFuFuBenRoleData> RoleDict { get; protected internal set; }

		
		public MoRiJudgeFuBenData()
		{
			this.RoleDict = new Dictionary<int, KuaFuFuBenRoleData>();
		}

		
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
