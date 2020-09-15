using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000158 RID: 344
	[ProtoContract]
	public class LuoLanChengZhuInfo
	{
		// Token: 0x040007A0 RID: 1952
		[ProtoMember(1)]
		public int BHID = 0;

		// Token: 0x040007A1 RID: 1953
		[ProtoMember(2)]
		public string BHName = "";

		// Token: 0x040007A2 RID: 1954
		[ProtoMember(3)]
		public int ZoneID = 0;

		// Token: 0x040007A3 RID: 1955
		[ProtoMember(4)]
		public List<int> ZhiWuList = new List<int>();

		// Token: 0x040007A4 RID: 1956
		[ProtoMember(5)]
		public List<RoleData4Selector> RoleInfoList = new List<RoleData4Selector>();

		// Token: 0x040007A5 RID: 1957
		[ProtoMember(6)]
		public bool isGetReward = true;
	}
}
