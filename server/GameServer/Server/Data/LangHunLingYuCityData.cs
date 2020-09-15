using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020003C4 RID: 964
	[ProtoContract]
	public class LangHunLingYuCityData
	{
		// Token: 0x04001927 RID: 6439
		[ProtoMember(1)]
		public int CityId;

		// Token: 0x04001928 RID: 6440
		[ProtoMember(2)]
		public int CityLevel;

		// Token: 0x04001929 RID: 6441
		[ProtoMember(3)]
		public BangHuiMiniData Owner;

		// Token: 0x0400192A RID: 6442
		[ProtoMember(4)]
		public List<BangHuiMiniData> AttackerList = new List<BangHuiMiniData>();
	}
}
