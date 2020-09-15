using System;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	// Token: 0x02000617 RID: 1559
	[TemplateMappingOptions(null, "Map", "Code")]
	public class MapSettingItem
	{
		// Token: 0x04002CA8 RID: 11432
		public int Code;

		// Token: 0x04002CA9 RID: 11433
		public string Name;

		// Token: 0x04002CAA RID: 11434
		public int MapType;

		// Token: 0x04002CAB RID: 11435
		public int PicCode;

		// Token: 0x04002CAC RID: 11436
		public int AutoStart;

		// Token: 0x04002CAD RID: 11437
		public int RealiveType;

		// Token: 0x04002CAE RID: 11438
		public int Transfer;

		// Token: 0x04002CAF RID: 11439
		public int MoveType;

		// Token: 0x04002CB0 RID: 11440
		public int Horse;

		// Token: 0x04002CB1 RID: 11441
		public int ElsePeople;

		// Token: 0x04002CB2 RID: 11442
		public int Transfiguration;

		// Token: 0x04002CB3 RID: 11443
		public double NormalHuntNum = 1.0;

		// Token: 0x04002CB4 RID: 11444
		public double RebornHuntNum = 0.0;
	}
}
