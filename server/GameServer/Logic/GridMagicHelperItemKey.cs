using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public struct GridMagicHelperItemKey : IComparer<GridMagicHelperItemKey>
	{
		
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

		
		public static GridMagicHelperItemKey Comparer = default(GridMagicHelperItemKey);

		
		public int MapCode;

		
		public int PosX;

		
		public int PosY;

		
		public int CopyMapID;

		
		public MagicActionIDs MagicActionID;

		
		public MagicActionIDs MagicActionID2;
	}
}
