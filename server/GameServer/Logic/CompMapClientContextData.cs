using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class CompMapClientContextData
	{
		
		public int RoleId;

		
		public int ServerId;

		
		public int BattleWhichSide;

		
		public string RoleName;

		
		public int Occupation;

		
		public int RoleSex;

		
		public int ZoneID;

		
		public long TotalScore;

		
		public Dictionary<int, long> InjureBossDeltaDict = new Dictionary<int, long>();
	}
}
