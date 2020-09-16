using System;
using Server.Data;

namespace GameServer.Logic
{
	
	public class EscapeBattleCollection
	{
		
		public EscapeBattleCollection Clone()
		{
			return base.MemberwiseClone() as EscapeBattleCollection;
		}

		
		public int ID;

		
		public int MapCodeID;

		
		public EscapeBCollectType cType;

		
		public EscapeBattleGameSceneStatuses eState;

		
		public int RefreshRegion;

		
		public int RefreshTime;

		
		public int RefreshMonsterId;

		
		public int RefreshMonsterNum;

		
		public int CollectTime;

		
		public int CollectGodNum;

		
		public int CollectLiveTime;

		
		public int IsDeath;
	}
}
