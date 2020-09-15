using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000548 RID: 1352
	[ProtoContract]
	internal class DailyActiveData
	{
		// Token: 0x04002441 RID: 9281
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04002442 RID: 9282
		[ProtoMember(2)]
		public long DailyActiveValues = 0L;

		// Token: 0x04002443 RID: 9283
		[ProtoMember(3)]
		public long TotalKilledMonsterCount = 0L;

		// Token: 0x04002444 RID: 9284
		[ProtoMember(4)]
		public long DailyActiveTotalLoginCount = 0L;

		// Token: 0x04002445 RID: 9285
		[ProtoMember(5)]
		public int DailyActiveOnLineTimer = 0;

		// Token: 0x04002446 RID: 9286
		[ProtoMember(6)]
		public List<ushort> DailyActiveInforFlags = null;

		// Token: 0x04002447 RID: 9287
		[ProtoMember(7)]
		public int NowCompletedDailyActiveID = 0;

		// Token: 0x04002448 RID: 9288
		[ProtoMember(8)]
		public int TotalKilledBossCount = 0;

		// Token: 0x04002449 RID: 9289
		[ProtoMember(9)]
		public int PassNormalCopySceneNum = 0;

		// Token: 0x0400244A RID: 9290
		[ProtoMember(10)]
		public int PassHardCopySceneNum = 0;

		// Token: 0x0400244B RID: 9291
		[ProtoMember(11)]
		public int PassDifficultCopySceneNum = 0;

		// Token: 0x0400244C RID: 9292
		[ProtoMember(12)]
		public int BuyItemInMall = 0;

		// Token: 0x0400244D RID: 9293
		[ProtoMember(13)]
		public int CompleteDailyTaskCount = 0;

		// Token: 0x0400244E RID: 9294
		[ProtoMember(14)]
		public int CompleteBloodCastleCount = 0;

		// Token: 0x0400244F RID: 9295
		[ProtoMember(15)]
		public int CompleteDaimonSquareCount = 0;

		// Token: 0x04002450 RID: 9296
		[ProtoMember(16)]
		public int CompleteBattleCount = 0;

		// Token: 0x04002451 RID: 9297
		[ProtoMember(17)]
		public int EquipForge = 0;

		// Token: 0x04002452 RID: 9298
		[ProtoMember(18)]
		public int EquipAppend = 0;

		// Token: 0x04002453 RID: 9299
		[ProtoMember(19)]
		public int ChangeLife = 0;

		// Token: 0x04002454 RID: 9300
		[ProtoMember(20)]
		public int MergeFruit = 0;

		// Token: 0x04002455 RID: 9301
		[ProtoMember(21)]
		public int GetAwardFlag = 0;
	}
}
