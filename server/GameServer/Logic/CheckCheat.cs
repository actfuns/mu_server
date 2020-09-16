using System;
using System.Collections.Generic;
using System.Windows;

namespace GameServer.Logic
{
	
	public class CheckCheat
	{
		
		public bool DisableAttack = false;

		
		public bool MismatchingMapCode = false;

		
		public long LastNotifyLeaveGuMuTick = 0L;

		
		public double MaxClientSpeed = 0.0;

		
		public long ProcessBoosterTicks = 0L;

		
		public bool ProcessBooster = false;

		
		public string RobotTaskListData = "";

		
		public int BanCheckMaxCount = 0;

		
		public int KickWarnMaxCount = 0;

		
		public bool DropRateDown = false;

		
		public bool KickState = false;

		
		public long RobotDetectedKickTime = 0L;

		
		public string RobotDetectedReason = "";

		
		public long NextTaskListTimeout = 0L;

		
		public Dictionary<int, int> LogCountDic = new Dictionary<int, int>();

		
		public bool RobotTaskCheckInitialed;

		
		public long LastStartMoveServerTicks;

		
		public long LastStartMoveTicks;

		
		public long LastMoveStartMoveTicksCheatNum;

		
		public bool IsKickedRole;

		
		public int LastMagicCode = 0;

		
		public long LastDamage = 0L;

		
		public int LastDamageType = 0;

		
		public int LastEnemyID = 0;

		
		public string LastEnemyName = "";

		
		public Point LastEnemyPos = new Point(0.0, 0.0);

		
		public int GmGotoShadowMapCode;

		
		public bool DisableAutoKuaFu;
	}
}
