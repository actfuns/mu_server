using System;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x02000766 RID: 1894
	public class MonsterZoneQueueItem
	{
		// Token: 0x04003D42 RID: 15682
		public int CopyMapID = 0;

		// Token: 0x04003D43 RID: 15683
		public int BirthCount = 0;

		// Token: 0x04003D44 RID: 15684
		public MonsterZone MyMonsterZone = null;

		// Token: 0x04003D45 RID: 15685
		public Monster seedMonster = null;

		// Token: 0x04003D46 RID: 15686
		public int ToX = 0;

		// Token: 0x04003D47 RID: 15687
		public int ToY = 0;

		// Token: 0x04003D48 RID: 15688
		public int Radius = 0;

		// Token: 0x04003D49 RID: 15689
		public int PursuitRadius = 0;

		// Token: 0x04003D4A RID: 15690
		public object Tag;

		// Token: 0x04003D4B RID: 15691
		public SceneUIClasses ManagerType = SceneUIClasses.Normal;

		// Token: 0x04003D4C RID: 15692
		public MonsterFlags Flags;
	}
}
