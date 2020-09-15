using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x0200074E RID: 1870
	public struct GridMagicHelperItemKey : IComparer<GridMagicHelperItemKey>
	{
		// Token: 0x06002F15 RID: 12053 RVA: 0x002A16EC File Offset: 0x0029F8EC
		public int Compare(GridMagicHelperItemKey x, GridMagicHelperItemKey y)
		{
			int ret = x.MapCode - y.MapCode;
			int result;
			if (ret != 0)
			{
				result = ret;
			}
			else
			{
				ret = x.CopyMapID - y.CopyMapID;
				if (ret != 0)
				{
					result = ret;
				}
				else
				{
					ret = x.PosX - y.PosX;
					if (ret != 0)
					{
						result = ret;
					}
					else
					{
						ret = x.PosY - y.PosY;
						if (ret != 0)
						{
							result = ret;
						}
						else
						{
							ret = x.MagicActionID - y.MagicActionID;
							if (ret != 0)
							{
								result = ret;
							}
							else
							{
								ret = x.MagicActionID2 - y.MagicActionID2;
								result = ret;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x04003CAD RID: 15533
		public static GridMagicHelperItemKey Comparer = default(GridMagicHelperItemKey);

		// Token: 0x04003CAE RID: 15534
		public int MapCode;

		// Token: 0x04003CAF RID: 15535
		public int PosX;

		// Token: 0x04003CB0 RID: 15536
		public int PosY;

		// Token: 0x04003CB1 RID: 15537
		public int CopyMapID;

		// Token: 0x04003CB2 RID: 15538
		public MagicActionIDs MagicActionID;

		// Token: 0x04003CB3 RID: 15539
		public MagicActionIDs MagicActionID2;
	}
}
