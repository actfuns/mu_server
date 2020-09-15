using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000C5 RID: 197
	[ProtoContract]
	public class TianTiDataAndDayPaiHang
	{
		// Token: 0x0400054C RID: 1356
		[ProtoMember(1)]
		public RoleTianTiData TianTiData;

		// Token: 0x0400054D RID: 1357
		[ProtoMember(2)]
		public List<TianTiPaiHangRoleData> PaiHangRoleDataList;
	}
}
