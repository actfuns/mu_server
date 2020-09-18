using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class AngelTemplePointInfo : IComparer<AngelTemplePointInfo>
	{
		
		public int Compare(AngelTemplePointInfo x, AngelTemplePointInfo y)
		{
			return AngelTemplePointInfo.Compare_static(x, y);
		}

		
		public static int Compare_static(AngelTemplePointInfo x, AngelTemplePointInfo y)
		{
			int result;
			if (x == y)
			{
				result = 0;
			}
			else if (x != null && y != null)
			{
				long ret = y.m_DamagePoint - x.m_DamagePoint;
				if (ret > 0L)
				{
					result = 1;
				}
				else if (ret == 0L)
				{
					result = y.Ranking - x.Ranking;
				}
				else
				{
					result = -1;
				}
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

		
		public int CompareTo(AngelTemplePointInfo y)
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
				long ret = y.m_DamagePoint - this.m_DamagePoint;
				if (ret > 0L)
				{
					result = 1;
				}
				else if (ret == 0L)
				{
					result = y.Ranking - this.Ranking;
				}
				else
				{
					result = -1;
				}
			}
			return result;
		}

		
		public int m_RoleID = 0;

		
		public long m_DamagePoint = 0L;

		
		public bool LeaveScene = false;

		
		public int Ranking = -1;

		
		public string m_RoleName;

		
		public int m_GetAwardFlag = 0;

		
		public string m_LuckPaiMingName = "";

		
		public AwardsItemList GoodsList = new AwardsItemList();

		
		public int m_AwardPaiMing = 0;

		
		public int m_AwardShengWang = 0;

		
		public int m_AwardGold = 0;
	}
}
