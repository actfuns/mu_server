using System;
using ProtoBuf;

namespace GameServer.Logic.Marriage.CoupleArena
{
	// Token: 0x02000366 RID: 870
	[ProtoContract]
	public class CoupleArenaPkResultData
	{
		// Token: 0x04001703 RID: 5891
		[ProtoMember(1)]
		public int PKResult;

		// Token: 0x04001704 RID: 5892
		[ProtoMember(2)]
		public int GetRongYao;

		// Token: 0x04001705 RID: 5893
		[ProtoMember(3)]
		public int GetJiFen;

		// Token: 0x04001706 RID: 5894
		[ProtoMember(4)]
		public int DuanWeiType;

		// Token: 0x04001707 RID: 5895
		[ProtoMember(5)]
		public int DuanWeiLevel;
	}
}
