using System;

namespace GameDBServer.Logic.UnionAlly
{
	// Token: 0x02000184 RID: 388
	public enum EAlly
	{
		// Token: 0x040008EC RID: 2284
		EUnionLevel = -9,
		// Token: 0x040008ED RID: 2285
		EAllyMax,
		// Token: 0x040008EE RID: 2286
		EAllyRequestMax,
		// Token: 0x040008EF RID: 2287
		ENotLeader,
		// Token: 0x040008F0 RID: 2288
		EIsSelf,
		// Token: 0x040008F1 RID: 2289
		EZoneID,
		// Token: 0x040008F2 RID: 2290
		EName,
		// Token: 0x040008F3 RID: 2291
		EMoney,
		// Token: 0x040008F4 RID: 2292
		EMore,
		// Token: 0x040008F5 RID: 2293
		SuccRequest = 1,
		// Token: 0x040008F6 RID: 2294
		EAlly = 10,
		// Token: 0x040008F7 RID: 2295
		AllyRefuse,
		// Token: 0x040008F8 RID: 2296
		AllyAgree,
		// Token: 0x040008F9 RID: 2297
		AllyRefuseOther = 20,
		// Token: 0x040008FA RID: 2298
		AllyAgreeOther,
		// Token: 0x040008FB RID: 2299
		EAllyCancel = 30,
		// Token: 0x040008FC RID: 2300
		AllyCancelSucc,
		// Token: 0x040008FD RID: 2301
		EAllyRemove = 40,
		// Token: 0x040008FE RID: 2302
		AllyRemoveSucc,
		// Token: 0x040008FF RID: 2303
		AllyRemoveSuccOther,
		// Token: 0x04000900 RID: 2304
		Default = 0
	}
}
