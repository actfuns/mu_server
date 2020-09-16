using System;
using System.Collections.Generic;
using System.Linq;
using KF.Contract.Interface;

namespace KF.Contract.Data
{
	
	[Serializable]
	public class YongZheZhanChangFuBenData : IKuaFuFuBenData
	{
		
		
		
		public int GameId { get; protected internal set; }

		
		
		
		public int ServerId { get; protected internal set; }

		
		
		
		public int SequenceId { get; set; }

		
		
		
		public int GroupIndex { get; protected internal set; }

		
		
		
		public GameFuBenState State { get; set; }

		
		
		
		public int Age { get; protected internal set; }

		
		
		
		public DateTime EndTime { get; protected internal set; }

		
		
		
		public int RoleCountSide1 { get; protected internal set; }

		
		
		
		public int RoleCountSide2 { get; protected internal set; }

		
		
		
		public Dictionary<int, KuaFuFuBenRoleData> RoleDict { get; protected internal set; }

		
		public YongZheZhanChangFuBenData()
		{
			this.RoleDict = new Dictionary<int, KuaFuFuBenRoleData>();
		}

		
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
			}
			return false;
		}

		
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
