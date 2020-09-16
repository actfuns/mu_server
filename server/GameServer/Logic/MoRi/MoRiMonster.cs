using System;
using System.Collections.Generic;

namespace GameServer.Logic.MoRi
{
	
	public class MoRiMonster
	{
		
		public int Id;

		
		public string Name;

		
		public int MonsterId;

		
		public int BirthX;

		
		public int BirthY;

		
		public int KillLimitSecond;

		
		public Dictionary<int, float> ExtPropDict = new Dictionary<int, float>();
	}
}
