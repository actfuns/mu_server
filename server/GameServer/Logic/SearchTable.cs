using System;
using System.Collections.Generic;
using System.Windows;

namespace GameServer.Logic
{
	// Token: 0x02000791 RID: 1937
	public class SearchTable
	{
		// Token: 0x06003274 RID: 12916 RVA: 0x002CD0B4 File Offset: 0x002CB2B4
		public static List<Point> GetSearchTableList()
		{
			return SearchTable._SearchTableList;
		}

		// Token: 0x06003275 RID: 12917 RVA: 0x002CD13C File Offset: 0x002CB33C
		public static void Init(int num)
		{
			for (int i = -num; i <= num; i++)
			{
				for (int j = -num; j <= num; j++)
				{
					SearchTable._SearchTableList.Add(new Point((double)i, (double)j));
				}
			}
			SearchTable._SearchTableList.Sort((Point x, Point y) => (int)Math.Pow(x.X, 2.0) + (int)Math.Pow(x.Y, 2.0) - ((int)Math.Pow(y.X, 2.0) + (int)Math.Pow(y.Y, 2.0)));
		}

		// Token: 0x04003E94 RID: 16020
		private static List<Point> _SearchTableList = new List<Point>();
	}
}
