using System;
using System.Collections.Generic;
using System.Windows;

namespace GameServer.Logic
{
	
	public class SearchTable
	{
		
		public static List<Point> GetSearchTableList()
		{
			return SearchTable._SearchTableList;
		}

		
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

		
		private static List<Point> _SearchTableList = new List<Point>();
	}
}
