using System;

namespace GameServer.Logic
{
	// Token: 0x0200047C RID: 1148
	public enum EHolyResult
	{
		// Token: 0x04001E30 RID: 7728
		Error = -1,
		// Token: 0x04001E31 RID: 7729
		Success,
		// Token: 0x04001E32 RID: 7730
		Fail,
		// Token: 0x04001E33 RID: 7731
		NeedGold,
		// Token: 0x04001E34 RID: 7732
		NeedHolyItemPart,
		// Token: 0x04001E35 RID: 7733
		PartSuitIsMax,
		// Token: 0x04001E36 RID: 7734
		NotOpen,
		// Token: 0x04001E37 RID: 7735
		NeedGoods
	}
}
