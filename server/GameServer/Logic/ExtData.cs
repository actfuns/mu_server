using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class ExtData
	{
		
		public int OffsetDay;

		
		public long ZhanLiLogNextWriteTicks;

		
		public long LastZhanLi;

		
		public long ZhanLiWriteten;

		
		public HashSet<long> ZhanLiLogged = new HashSet<long>();

		
		public long HuiJiCDTicks;

		
		public long HuiJiCdTime;

		
		public long ZuoQiSkillCDTicks;

		
		public long ZuoQiSkillCdTime;

		
		public int ArmorCurrentV;

		
		public int ArmorMaxV;

		
		public double ArmorPercent;

		
		public long BianShenCDTicks;

		
		public long BianShenCdTime;

		
		public long BianShenToTicks;

		
		public List<int> skillIDList;
	}
}
