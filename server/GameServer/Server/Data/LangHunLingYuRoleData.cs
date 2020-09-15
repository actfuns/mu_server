using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020003C5 RID: 965
	[ProtoContract]
	public class LangHunLingYuRoleData
	{
		// Token: 0x0400192B RID: 6443
		[ProtoMember(1)]
		public int SignUpState;

		// Token: 0x0400192C RID: 6444
		[ProtoMember(2)]
		public List<int> GetDayAwardsState;

		// Token: 0x0400192D RID: 6445
		[ProtoMember(3)]
		public List<LangHunLingYuCityData> SelfCityList = new List<LangHunLingYuCityData>();

		// Token: 0x0400192E RID: 6446
		[ProtoMember(4)]
		public List<LangHunLingYuCityData> OtherCityList = new List<LangHunLingYuCityData>();
	}
}
