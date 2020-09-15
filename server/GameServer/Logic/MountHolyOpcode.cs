using System;

namespace GameServer.Logic
{
	// Token: 0x020000B6 RID: 182
	public enum MountHolyOpcode
	{
		// Token: 0x0400043A RID: 1082
		Succ = 1,
		// Token: 0x0400043B RID: 1083
		NotOpen,
		// Token: 0x0400043C RID: 1084
		ParamErr,
		// Token: 0x0400043D RID: 1085
		UpGradeGoodTypeErr,
		// Token: 0x0400043E RID: 1086
		NotExsitGood,
		// Token: 0x0400043F RID: 1087
		NotExsitPhoyGood,
		// Token: 0x04000440 RID: 1088
		PhoyGoodCountErr,
		// Token: 0x04000441 RID: 1089
		NotExsitGoodXml,
		// Token: 0x04000442 RID: 1090
		NotExsitInfo,
		// Token: 0x04000443 RID: 1091
		HasMaxLevel,
		// Token: 0x04000444 RID: 1092
		CategoriyErr,
		// Token: 0x04000445 RID: 1093
		UpGradeErr,
		// Token: 0x04000446 RID: 1094
		UpGradeCountAttrErr,
		// Token: 0x04000447 RID: 1095
		MakeAttrErr,
		// Token: 0x04000448 RID: 1096
		UseGoodErr,
		// Token: 0x04000449 RID: 1097
		GoodHasUsing,
		// Token: 0x0400044A RID: 1098
		NotUsingType,
		// Token: 0x0400044B RID: 1099
		GetHoleNumErr,
		// Token: 0x0400044C RID: 1100
		CurrHoleLock,
		// Token: 0x0400044D RID: 1101
		GoodHasNot,
		// Token: 0x0400044E RID: 1102
		HoleInfoIsNull,
		// Token: 0x0400044F RID: 1103
		HolyGoodListNotFree,
		// Token: 0x04000450 RID: 1104
		DataModifyErr
	}
}
