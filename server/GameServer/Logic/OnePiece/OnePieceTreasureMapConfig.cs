using System;
using System.Collections.Generic;

namespace GameServer.Logic.OnePiece
{
	// Token: 0x020003A4 RID: 932
	public class OnePieceTreasureMapConfig
	{
		// Token: 0x0400187C RID: 6268
		public int ID = 0;

		// Token: 0x0400187D RID: 6269
		public int Num = 0;

		// Token: 0x0400187E RID: 6270
		public int Floor = 0;

		// Token: 0x0400187F RID: 6271
		public TriggerType Trigger = TriggerType.ETT_Null;

		// Token: 0x04001880 RID: 6272
		public int Score = 0;

		// Token: 0x04001881 RID: 6273
		public List<OnePieceRandomEvent> LisRandomEvent = new List<OnePieceRandomEvent>();
	}
}
