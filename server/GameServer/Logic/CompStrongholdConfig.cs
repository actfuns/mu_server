using System;

namespace GameServer.Logic
{
	// Token: 0x02000259 RID: 601
	public class CompStrongholdConfig
	{
		// Token: 0x06000864 RID: 2148 RVA: 0x00080EE8 File Offset: 0x0007F0E8
		public object Clone()
		{
			return new CompStrongholdConfig
			{
				ID = this.ID,
				MapCode = this.MapCode,
				QiZhiID = this.QiZhiID,
				Name = this.Name,
				QiZuoID = this.QiZuoID,
				PosX = this.PosX,
				PosY = this.PosY,
				Rate = this.Rate,
				Point = this.Point
			};
		}

		// Token: 0x04000EB3 RID: 3763
		public int ID;

		// Token: 0x04000EB4 RID: 3764
		public int MapCode;

		// Token: 0x04000EB5 RID: 3765
		public int[] QiZhiID;

		// Token: 0x04000EB6 RID: 3766
		public string Name;

		// Token: 0x04000EB7 RID: 3767
		public int QiZuoID;

		// Token: 0x04000EB8 RID: 3768
		public int PosX;

		// Token: 0x04000EB9 RID: 3769
		public int PosY;

		// Token: 0x04000EBA RID: 3770
		public double Rate;

		// Token: 0x04000EBB RID: 3771
		public int Point;

		// Token: 0x04000EBC RID: 3772
		public int BattleWhichSide;

		// Token: 0x04000EBD RID: 3773
		public int BattleWhichSideLast;

		// Token: 0x04000EBE RID: 3774
		public bool Alive;

		// Token: 0x04000EBF RID: 3775
		public long DeadTicks;
	}
}
