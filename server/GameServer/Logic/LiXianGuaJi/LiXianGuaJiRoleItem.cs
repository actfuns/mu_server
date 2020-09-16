using System;
using System.Windows;
using GameServer.Core.Executor;

namespace GameServer.Logic.LiXianGuaJi
{
	
	public class LiXianGuaJiRoleItem
	{
		
		public int ZoneID = 0;

		
		public string UserID = "";

		
		public int RoleID = 0;

		
		public string RoleName = "";

		
		public int RoleLevel = 0;

		
		public Point CurrentGrid;

		
		public long StartTicks = 0L;

		
		public int FakeRoleID = 0;

		
		public int MeditateTime = 0;

		
		public int NotSafeMeditateTime = 0;

		
		public long MeditateTicks = TimeUtil.NOW();

		
		public long BiGuanTime = TimeUtil.NOW();

		
		public int MapCode = 0;
	}
}
