using System;
using KF.Contract.Interface;

namespace KF.Contract.Data
{
	
	[Serializable]
	public class KuaFuRoleData : ICloneable
	{
		
		
		
		public int Age { get; protected internal set; }

		
		
		
		public string UserId { get; protected internal set; }

		
		
		
		public int ServerId { get; protected internal set; }

		
		
		
		public int ZoneId { get; protected internal set; }

		
		
		
		public int RoleId { get; protected internal set; }

		
		
		
		public KuaFuRoleStates State { get; protected internal set; }

		
		
		
		public int GameId { get; protected internal set; }

		
		
		
		public int GameType { get; protected internal set; }

		
		
		
		public int GroupIndex { get; protected internal set; }

		
		
		
		public int KuaFuServerId { get; protected internal set; }

		
		
		
		public int ZhanDouLi { get; protected internal set; }

		
		
		
		public long StateEndTicks { get; protected internal set; }

		
		
		
		public IGameData GameData { get; protected internal set; }

		
		
		
		public int TeamCombatAvg { get; protected internal set; }

		
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

		
		[NonSerialized]
		public KuaFuRoleData Next;
	}
}
