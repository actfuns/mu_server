using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000490 RID: 1168
	[ProtoContract]
	public class TianTiDataAndDayPaiHang
	{
		// Token: 0x04001EEC RID: 7916
		[ProtoMember(1)]
		public RoleTianTiData TianTiData;

		// Token: 0x04001EED RID: 7917
		[ProtoMember(2)]
		public List<TianTiPaiHangRoleData> PaiHangRoleDataList;
	}
}
