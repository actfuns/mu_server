using System;

namespace GameServer.Logic
{
	// Token: 0x020002A9 RID: 681
	public enum OpTypes
	{
		// Token: 0x04001130 RID: 4400
		None,
		// Token: 0x04001131 RID: 4401
		AddOrSub,
		// Token: 0x04001132 RID: 4402
		Trade,
		// Token: 0x04001133 RID: 4403
		Move = 5,
		// Token: 0x04001134 RID: 4404
		Exchange,
		// Token: 0x04001135 RID: 4405
		Input,
		// Token: 0x04001136 RID: 4406
		Destory,
		// Token: 0x04001137 RID: 4407
		Set,
		// Token: 0x04001138 RID: 4408
		Fall,
		// Token: 0x04001139 RID: 4409
		Sort,
		// Token: 0x0400113A RID: 4410
		Forge = 20,
		// Token: 0x0400113B RID: 4411
		Upgrade,
		// Token: 0x0400113C RID: 4412
		JuHun,
		// Token: 0x0400113D RID: 4413
		ElementhrtsSlotExtend,
		// Token: 0x0400113E RID: 4414
		FuMo,
		// Token: 0x0400113F RID: 4415
		Reset,
		// Token: 0x04001140 RID: 4416
		Trace = 30,
		// Token: 0x04001141 RID: 4417
		Hold,
		// Token: 0x04001142 RID: 4418
		Join,
		// Token: 0x04001143 RID: 4419
		Enter,
		// Token: 0x04001144 RID: 4420
		GiveAwards,
		// Token: 0x04001145 RID: 4421
		Result,
		// Token: 0x04001146 RID: 4422
		RoleRealive = 200
	}
}
