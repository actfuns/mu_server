using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class BattlePointInfo : IComparable<BattlePointInfo>, IComparer<BattlePointInfo>
	{
		
		public int CompareTo(BattlePointInfo y)
		{
			int result;
			if (this == y)
			{
				result = 0;
			}
			else if (y == null)
			{
				result = -1;
			}
			else
			{
				result = y.m_DamagePoint - this.m_DamagePoint;
			}
			return result;
		}

		
		public int Compare(BattlePointInfo x, BattlePointInfo y)
		{
			int result;
			if (x == y)
			{
				result = 0;
			}
			else if (x != null && y != null)
			{
				result = y.m_DamagePoint - x.m_DamagePoint;
			}
			else if (x == null)
			{
				result = 1;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		
		public int m_RoleID = 0;

		
		public int m_DamagePoint = 0;

		
		public bool LeaveScene = false;

		
		public int Ranking = -1;

		
		public int Side = 0;

		
		public string m_RoleName;

		
		public int m_GetAwardFlag = 0;

		
		public string m_LuckPaiMingName = "";

		
		public AwardsItemList GoodsList = new AwardsItemList();

		
		public int m_AwardPaiMing = 0;

		
		public int m_AwardShengWang = 0;

		
		public int m_AwardGold = 0;
	}
}
