using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class QiZhiConfig : ICloneable
	{
		
		public object Clone()
		{
			return base.MemberwiseClone() as QiZhiConfig;
		}

		
		public int NPCID;

		
		public int BufferID;

		
		public int PosX;

		
		public int PosY;

		
		public HashSet<int> UseAuthority = new HashSet<int>();

		
		public int MonsterId;

		
		public int Injure;

		
		public int RebirthSiteX;

		
		public int RebirthSiteY;

		
		public int RebirthRadius;

		
		public int ProduceTime;

		
		public int ProduceNum;

		
		public int BattleWhichSide;

		
		public bool Alive;

		
		public long DeadTicks;

		
		public long KillerBhid;

		
		public long InstallBhid;

		
		public string InstallBhName;
	}
}
