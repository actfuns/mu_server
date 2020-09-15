using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000420 RID: 1056
	[ProtoContract]
	public class ZhengBaMainInfoData
	{
		// Token: 0x04001C68 RID: 7272
		[ProtoMember(1)]
		public int RealActDay;

		// Token: 0x04001C69 RID: 7273
		[ProtoMember(2)]
		public List<TianTiPaiHangRoleData> Top16List;

		// Token: 0x04001C6A RID: 7274
		[ProtoMember(3)]
		public int MaxSupportGroup;

		// Token: 0x04001C6B RID: 7275
		[ProtoMember(4)]
		public int MaxOpposeGroup;

		// Token: 0x04001C6C RID: 7276
		[ProtoMember(5, IsRequired = true)]
		public int CanGetAwardId;

		// Token: 0x04001C6D RID: 7277
		[ProtoMember(6, IsRequired = true)]
		public int RankResultOfDay;
	}
}
