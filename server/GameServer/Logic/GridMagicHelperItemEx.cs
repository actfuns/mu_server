using System;
using System.Collections.Generic;
using System.Windows;

namespace GameServer.Logic
{
	
	public class GridMagicHelperItemEx
	{
		
		public int PosX;

		
		public int PosY;

		
		public int CopyMapID;

		
		public MagicActionIDs MagicActionID;

		
		public double[] MagicActionParams;

		
		public long StartedTicks;

		
		public long LastTicks;

		
		public int ExecutedNum;

		
		public int MapCode;

		
		public int MaxNum = 8;

		
		public MagicActionIDs MagicActionID2;

		
		public double[] MagicActionParams2;

		
		public List<Point> PointList = new List<Point>();

		
		public int AttackerRoleId;
	}
}
