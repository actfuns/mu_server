using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class KingOfBattleClientContextData
	{
		
		public int RoleId;

		
		public int ServerId;

		
		public int BattleWhichSide;

		
		public string RoleName;

		
		public int Occupation;

		
		public int RoleSex;

		
		public int ZoneID;

		
		public int TotalScore;

		
		public int KillNum;

		
		public Dictionary<int, double> InjureBossDeltaDict = new Dictionary<int, double>();
	}
}
