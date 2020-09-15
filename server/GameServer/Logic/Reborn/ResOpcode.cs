using System;

namespace GameServer.Logic.Reborn
{
	// Token: 0x020003F5 RID: 1013
	public enum ResOpcode
	{
		// Token: 0x04001B01 RID: 6913
		Succ = 1,
		// Token: 0x04001B02 RID: 6914
		Fail,
		// Token: 0x04001B03 RID: 6915
		GongNengWeiKaiQi,
		// Token: 0x04001B04 RID: 6916
		ChooseYinJiTypeErr,
		// Token: 0x04001B05 RID: 6917
		ChooseGetInfoYinJiNotActive,
		// Token: 0x04001B06 RID: 6918
		ChooseGetInfoYinJiIsActive,
		// Token: 0x04001B07 RID: 6919
		ChooseYinJiIsActiveErr,
		// Token: 0x04001B08 RID: 6920
		ResetYinJiZuanShiErr,
		// Token: 0x04001B09 RID: 6921
		ResetYinJiInfoErr,
		// Token: 0x04001B0A RID: 6922
		GetYinJiInfoErr,
		// Token: 0x04001B0B RID: 6923
		LevelUpYinJiPointErr,
		// Token: 0x04001B0C RID: 6924
		LevelUpYinJiTypeErr,
		// Token: 0x04001B0D RID: 6925
		LevelUpYinJiCheckErr,
		// Token: 0x04001B0E RID: 6926
		LevelUpYinJiMaxLv,
		// Token: 0x04001B0F RID: 6927
		LevelUpYinJiSaveErr,
		// Token: 0x04001B10 RID: 6928
		LevelUpYinJiUpNumErr,
		// Token: 0x04001B11 RID: 6929
		LevelUpYinJiOverUpLvErr
	}
}
