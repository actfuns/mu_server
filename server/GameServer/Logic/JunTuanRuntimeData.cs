using System;
using System.Collections.Generic;
using Server.Data;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	// Token: 0x02000317 RID: 791
	public class JunTuanRuntimeData
	{
		// Token: 0x0400145E RID: 5214
		public object Mutex = new object();

		// Token: 0x0400145F RID: 5215
		public int LegionsNeed;

		// Token: 0x04001460 RID: 5216
		public int LegionsCastZuanShi;

		// Token: 0x04001461 RID: 5217
		public int LegionsCreateCD;

		// Token: 0x04001462 RID: 5218
		public int LegionsJionCD;

		// Token: 0x04001463 RID: 5219
		public int LegionsEliteNum;

		// Token: 0x04001464 RID: 5220
		public int[] LegionProsperityCost;

		// Token: 0x04001465 RID: 5221
		public TemplateLoader<Dictionary<int, JunTuanRolePermissionInfo>> RolePermissionDict = new TemplateLoader<Dictionary<int, JunTuanRolePermissionInfo>>();

		// Token: 0x04001466 RID: 5222
		public TemplateLoader<Dictionary<int, JunTuanTaskInfo>> TaskList = new TemplateLoader<Dictionary<int, JunTuanTaskInfo>>();

		// Token: 0x04001467 RID: 5223
		public List<TimeSpan> TaskStartEndTimeList = new List<TimeSpan>();

		// Token: 0x04001468 RID: 5224
		public HashSet<int> KillMonsterIds = new HashSet<int>();

		// Token: 0x04001469 RID: 5225
		public Dictionary<int, int> Task2IdxDict = new Dictionary<int, int>();

		// Token: 0x0400146A RID: 5226
		public int TaskCount = 0;

		// Token: 0x0400146B RID: 5227
		public Dictionary<int, JunTuanBaseData> JunTuanBaseDict = new Dictionary<int, JunTuanBaseData>();

		// Token: 0x0400146C RID: 5228
		public Dictionary<int, int> BangHuiJunTuanIdDict = new Dictionary<int, int>();

		// Token: 0x0400146D RID: 5229
		public Queue<Tuple<int, int, int, int, long>> JunTuanTaskQueue = new Queue<Tuple<int, int, int, int, long>>();

		// Token: 0x0400146E RID: 5230
		public List<KFChat> JunTuanChatList = new List<KFChat>();

		// Token: 0x0400146F RID: 5231
		public HashSet<int> HasUpdateRoleDataHashSet = new HashSet<int>();

		// Token: 0x04001470 RID: 5232
		public long NextUpdateTicks;
	}
}
