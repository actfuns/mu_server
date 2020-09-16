using System;
using System.Collections.Generic;
using KF.Contract.Interface;

namespace KF.Contract.Data
{
	
	[Serializable]
	public class TianTiFuBenData : IKuaFuFuBenData
	{
		
		
		
		public int GameId { get; protected internal set; }

		
		
		
		public int ServerId { get; protected internal set; }

		
		
		
		public int SequenceId { get; protected internal set; }

		
		
		
		public int GroupIndex { get; protected internal set; }

		
		
		
		public GameFuBenState State { get; protected internal set; }

		
		
		
		public int Age { get; protected internal set; }

		
		
		
		public DateTime EndTime { get; protected internal set; }

		
		
		
		public int RoleCountSide1 { get; protected internal set; }

		
		
		
		public int RoleCountSide2 { get; protected internal set; }

		
		
		
		public Dictionary<int, KuaFuFuBenRoleData> RoleDict { get; protected internal set; }

		
		public TianTiFuBenData()
		{
			this.RoleDict = new Dictionary<int, KuaFuFuBenRoleData>();
		}

		
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
	}
}
