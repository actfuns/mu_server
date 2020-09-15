using System;

namespace GameServer.Logic
{
	// Token: 0x0200021F RID: 543
	public class KuaFuLueDuoMonsterItem
	{
		// Token: 0x06000737 RID: 1847 RVA: 0x0006AE90 File Offset: 0x00069090
		public object Clone()
		{
			return new KuaFuLueDuoMonsterItem
			{
				ID = this.ID,
				MonsterID = this.MonsterID,
				Type = this.Type,
				Name = this.Name,
				GatherTime = this.GatherTime,
				FuHuoTime = this.FuHuoTime,
				ZiYuan = this.ZiYuan,
				JiFen = this.JiFen,
				X = this.X,
				Y = this.Y
			};
		}

		// Token: 0x04000C77 RID: 3191
		public int ID;

		// Token: 0x04000C78 RID: 3192
		public int MonsterID;

		// Token: 0x04000C79 RID: 3193
		public int Type;

		// Token: 0x04000C7A RID: 3194
		public string Name;

		// Token: 0x04000C7B RID: 3195
		public int GatherTime;

		// Token: 0x04000C7C RID: 3196
		public int FuHuoTime;

		// Token: 0x04000C7D RID: 3197
		public int ZiYuan;

		// Token: 0x04000C7E RID: 3198
		public int JiFen;

		// Token: 0x04000C7F RID: 3199
		public int X;

		// Token: 0x04000C80 RID: 3200
		public int Y;

		// Token: 0x04000C81 RID: 3201
		public bool Alive;

		// Token: 0x04000C82 RID: 3202
		public long FuHuoTicks;
	}
}
