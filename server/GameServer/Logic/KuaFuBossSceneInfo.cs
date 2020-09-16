using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class KuaFuBossSceneInfo
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

		
		public int MinZhuanSheng = 1;

		
		public int MinLevel = 1;

		
		public int MaxZhuanSheng = 1;

		
		public int MaxLevel = 1;

		
		public List<TimeSpan> TimePoints = new List<TimeSpan>();

		
		public List<double> SecondsOfDay = new List<double>();

		
		public int WaitingEnterSecs;

		
		public int PrepareSecs;

		
		public int FightingSecs;

		
		public int ClearRolesSecs;

		
		public int SignUpStartSecs;

		
		public int SignUpEndSecs;
	}
}
