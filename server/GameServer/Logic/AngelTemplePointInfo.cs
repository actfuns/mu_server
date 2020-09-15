using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020005BA RID: 1466
	public class AngelTemplePointInfo : IComparer<AngelTemplePointInfo>
	{
		// Token: 0x06001A9E RID: 6814 RVA: 0x00197EB8 File Offset: 0x001960B8
		public int Compare(AngelTemplePointInfo x, AngelTemplePointInfo y)
		{
			return AngelTemplePointInfo.Compare_static(x, y);
		}

		// Token: 0x06001A9F RID: 6815 RVA: 0x00197ED4 File Offset: 0x001960D4
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

		// Token: 0x06001AA0 RID: 6816 RVA: 0x00197F5C File Offset: 0x0019615C
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

		// Token: 0x0400294F RID: 10575
		public int m_RoleID = 0;

		// Token: 0x04002950 RID: 10576
		public long m_DamagePoint = 0L;

		// Token: 0x04002951 RID: 10577
		public bool LeaveScene = false;

		// Token: 0x04002952 RID: 10578
		public int Ranking = -1;

		// Token: 0x04002953 RID: 10579
		public string m_RoleName;

		// Token: 0x04002954 RID: 10580
		public int m_GetAwardFlag = 0;

		// Token: 0x04002955 RID: 10581
		public string m_LuckPaiMingName = "";

		// Token: 0x04002956 RID: 10582
		public AwardsItemList GoodsList = new AwardsItemList();

		// Token: 0x04002957 RID: 10583
		public int m_AwardPaiMing = 0;

		// Token: 0x04002958 RID: 10584
		public int m_AwardShengWang = 0;

		// Token: 0x04002959 RID: 10585
		public int m_AwardGold = 0;
	}
}
