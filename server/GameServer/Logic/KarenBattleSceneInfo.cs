using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class KarenBattleSceneInfo
	{
		
		
		public int TotalSecs
		{
			get
			{
				return this.WaitingEnterSecs + this.PrepareSecs + this.FightingSecs + this.ClearRolesSecs + 120;
			}
		}

		
		public int Id;

		
		public int MapCode;

		
		public int MaxLegions = 4;

		
		public int MaxEnterNum = 30;

		
		public int EnterCD = 5;

		
		public List<TimeSpan> TimePoints = new List<TimeSpan>();

		
		public List<double> SecondsOfDay = new List<double>();

		
		public int WaitingEnterSecs;

		
		public int PrepareSecs;

		
		public int FightingSecs;

		
		public int ClearRolesSecs;

		
		public long Exp;

		
		public int BandJinBi;

		
		public AwardsItemList WinAwardsItemList = new AwardsItemList();

		
		public AwardsItemList LoseAwardsItemList = new AwardsItemList();
	}
}
