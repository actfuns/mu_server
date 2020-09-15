using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000807 RID: 2055
	[ProtoContract]
	public class PrisonMainData
	{
		// Token: 0x04004404 RID: 17412
		[ProtoMember(1)]
		public PrisonRoleData roleData = new PrisonRoleData();

		// Token: 0x04004405 RID: 17413
		[ProtoMember(2)]
		public int MineFuLuState = 0;

		// Token: 0x04004406 RID: 17414
		[ProtoMember(3)]
		public long RevoltCD = 0L;

		// Token: 0x04004407 RID: 17415
		[ProtoMember(4)]
		public int JieJiuCount = 0;

		// Token: 0x04004408 RID: 17416
		[ProtoMember(5)]
		public int ZhengFuCount = 0;

		// Token: 0x04004409 RID: 17417
		[ProtoMember(6)]
		public int ZhengFuLeftCount = 0;

		// Token: 0x0400440A RID: 17418
		[ProtoMember(7)]
		public int LaoDongCount = 0;

		// Token: 0x0400440B RID: 17419
		[ProtoMember(8)]
		public List<PrisonFuLuData> fuLuDataList = new List<PrisonFuLuData>();
	}
}
