using System;

namespace GameServer.Logic
{
	
	public class CompSolderSiteConfig
	{
		
		public object Clone()
		{
			return new CompSolderSiteConfig
			{
				ID = this.ID,
				PosX = this.PosX,
				PosY = this.PosY,
				RefreshTimeBegin = this.RefreshTimeBegin,
				RefreshTimeEnd = this.RefreshTimeEnd
			};
		}

		
		public int ID;

		
		public int PosX;

		
		public int PosY;

		
		public TimeSpan RefreshTimeBegin;

		
		public TimeSpan RefreshTimeEnd;

		
		public int MonsterState;

		
		public CompSolderConfig SolderConfig;
	}
}
