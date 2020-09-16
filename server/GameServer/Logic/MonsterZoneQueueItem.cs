using System;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class MonsterZoneQueueItem
	{
		
		public int CopyMapID = 0;

		
		public int BirthCount = 0;

		
		public MonsterZone MyMonsterZone = null;

		
		public Monster seedMonster = null;

		
		public int ToX = 0;

		
		public int ToY = 0;

		
		public int Radius = 0;

		
		public int PursuitRadius = 0;

		
		public object Tag;

		
		public SceneUIClasses ManagerType = SceneUIClasses.Normal;

		
		public MonsterFlags Flags;
	}
}
