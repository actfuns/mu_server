using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020005C5 RID: 1477
	public class BattlePointInfo : IComparable<BattlePointInfo>, IComparer<BattlePointInfo>
	{
		// Token: 0x06001AD0 RID: 6864 RVA: 0x00198EA4 File Offset: 0x001970A4
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

		// Token: 0x06001AD1 RID: 6865 RVA: 0x00198EE4 File Offset: 0x001970E4
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

		// Token: 0x04002981 RID: 10625
		public int m_RoleID = 0;

		// Token: 0x04002982 RID: 10626
		public int m_DamagePoint = 0;

		// Token: 0x04002983 RID: 10627
		public bool LeaveScene = false;

		// Token: 0x04002984 RID: 10628
		public int Ranking = -1;

		// Token: 0x04002985 RID: 10629
		public int Side = 0;

		// Token: 0x04002986 RID: 10630
		public string m_RoleName;

		// Token: 0x04002987 RID: 10631
		public int m_GetAwardFlag = 0;

		// Token: 0x04002988 RID: 10632
		public string m_LuckPaiMingName = "";

		// Token: 0x04002989 RID: 10633
		public AwardsItemList GoodsList = new AwardsItemList();

		// Token: 0x0400298A RID: 10634
		public int m_AwardPaiMing = 0;

		// Token: 0x0400298B RID: 10635
		public int m_AwardShengWang = 0;

		// Token: 0x0400298C RID: 10636
		public int m_AwardGold = 0;
	}
}
