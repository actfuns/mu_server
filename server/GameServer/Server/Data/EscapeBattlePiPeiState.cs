using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class EscapeBattlePiPeiState
	{
		
		[ProtoMember(1)]
		public List<EscapeBattleJoinRoleInfo> RoleList = new List<EscapeBattleJoinRoleInfo>();

		
		public int EscapeJiFen;

		
		public int ReadyNum;

		
		public int GameID;

		
		public int State;

		
		public DateTime FightTime;

		
		public DateTime EndTime;
	}
}
