using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200004A RID: 74
	[ProtoContract]
	public class DJPointsData
	{
		// Token: 0x04000185 RID: 389
		[ProtoMember(1)]
		public DJPointData SelfDJPointData = null;

		// Token: 0x04000186 RID: 390
		[ProtoMember(2)]
		public List<DJPointData> HotDJPointDataList = null;
	}
}
