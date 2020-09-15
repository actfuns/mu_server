using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000C4 RID: 196
	[ProtoContract]
	public class TianTiMonthPaiHangData
	{
		// Token: 0x0400054A RID: 1354
		[ProtoMember(1)]
		public TianTiPaiHangRoleData SelfPaiHangRoleData;

		// Token: 0x0400054B RID: 1355
		[ProtoMember(2)]
		public List<TianTiPaiHangRoleData> PaiHangRoleDataList;
	}
}
