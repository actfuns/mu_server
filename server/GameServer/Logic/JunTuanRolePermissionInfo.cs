using System;
using System.Collections.Generic;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	
	[TemplateMappingOptions(null, "LegionsManager", "ID")]
	public class JunTuanRolePermissionInfo
	{
		
		public int ID;

		
		public string Name;

		
		public int Manager;

		
		public int AppointLeader;

		
		public int AppointElite;

		
		public int Quit;

		
		public int Dissolution;

		
		public int BulletinCD;

		
		[TemplateMappingField(SpliteChars = ",")]
		public List<int> TalkLevel;

		
		public int TalkCD;
	}
}
