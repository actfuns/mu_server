using System;
using System.Collections.Generic;
using System.Windows;

namespace GameServer.Logic
{
	// Token: 0x020004CF RID: 1231
	public class CheckCheat
	{
		// Token: 0x0400207A RID: 8314
		public bool DisableAttack = false;

		// Token: 0x0400207B RID: 8315
		public bool MismatchingMapCode = false;

		// Token: 0x0400207C RID: 8316
		public long LastNotifyLeaveGuMuTick = 0L;

		// Token: 0x0400207D RID: 8317
		public double MaxClientSpeed = 0.0;

		// Token: 0x0400207E RID: 8318
		public long ProcessBoosterTicks = 0L;

		// Token: 0x0400207F RID: 8319
		public bool ProcessBooster = false;

		// Token: 0x04002080 RID: 8320
		public string RobotTaskListData = "";

		// Token: 0x04002081 RID: 8321
		public int BanCheckMaxCount = 0;

		// Token: 0x04002082 RID: 8322
		public int KickWarnMaxCount = 0;

		// Token: 0x04002083 RID: 8323
		public bool DropRateDown = false;

		// Token: 0x04002084 RID: 8324
		public bool KickState = false;

		// Token: 0x04002085 RID: 8325
		public long RobotDetectedKickTime = 0L;

		// Token: 0x04002086 RID: 8326
		public string RobotDetectedReason = "";

		// Token: 0x04002087 RID: 8327
		public long NextTaskListTimeout = 0L;

		// Token: 0x04002088 RID: 8328
		public Dictionary<int, int> LogCountDic = new Dictionary<int, int>();

		// Token: 0x04002089 RID: 8329
		public bool RobotTaskCheckInitialed;

		// Token: 0x0400208A RID: 8330
		public long LastStartMoveServerTicks;

		// Token: 0x0400208B RID: 8331
		public long LastStartMoveTicks;

		// Token: 0x0400208C RID: 8332
		public long LastMoveStartMoveTicksCheatNum;

		// Token: 0x0400208D RID: 8333
		public bool IsKickedRole;

		// Token: 0x0400208E RID: 8334
		public int LastMagicCode = 0;

		// Token: 0x0400208F RID: 8335
		public long LastDamage = 0L;

		// Token: 0x04002090 RID: 8336
		public int LastDamageType = 0;

		// Token: 0x04002091 RID: 8337
		public int LastEnemyID = 0;

		// Token: 0x04002092 RID: 8338
		public string LastEnemyName = "";

		// Token: 0x04002093 RID: 8339
		public Point LastEnemyPos = new Point(0.0, 0.0);

		// Token: 0x04002094 RID: 8340
		public int GmGotoShadowMapCode;

		// Token: 0x04002095 RID: 8341
		public bool DisableAutoKuaFu;
	}
}
