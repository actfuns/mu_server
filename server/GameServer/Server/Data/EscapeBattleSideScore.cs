using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class EscapeBattleSideScore
	{
		
		public EscapeBattleSideScore Clone()
		{
			return base.MemberwiseClone() as EscapeBattleSideScore;
		}

		
		[ProtoMember(1)]
		public List<EscapeBattleRoleInfo> BattleRoleList = new List<EscapeBattleRoleInfo>();

		
		[ProtoMember(2)]
		public List<EscapeBattleTeamInfo> BattleTeamList = new List<EscapeBattleTeamInfo>();

		
		[ProtoMember(3)]
		public EscapeBattleAreaInfo targetSafeArea = new EscapeBattleAreaInfo();

		
		[ProtoMember(4)]
		public EscapeBattleAreaInfo safeArea = new EscapeBattleAreaInfo();

		
		[ProtoMember(5)]
		public DateTime AreaChangeTm;

		
		[ProtoMember(6)]
		public int ReliveCount;

		
		[ProtoMember(7)]
		public EscapeBattleGameSceneStatuses eStatus;
	}
}
