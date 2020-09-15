using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000140 RID: 320
	[ProtoContract]
	public class GiftCodeInfo
	{
		// Token: 0x04000727 RID: 1831
		[ProtoMember(1)]
		public string GiftCodeTypeID = "";

		// Token: 0x04000728 RID: 1832
		[ProtoMember(2)]
		public string GiftCodeName = "";

		// Token: 0x04000729 RID: 1833
		[ProtoMember(3)]
		public List<string> ChannelList = new List<string>();

		// Token: 0x0400072A RID: 1834
		[ProtoMember(4)]
		public List<int> PlatformList = new List<int>();

		// Token: 0x0400072B RID: 1835
		[ProtoMember(5)]
		public DateTime TimeBegin = DateTime.MinValue;

		// Token: 0x0400072C RID: 1836
		[ProtoMember(6)]
		public DateTime TimeEnd = DateTime.MinValue;

		// Token: 0x0400072D RID: 1837
		[ProtoMember(7)]
		public List<int> ZoneList = new List<int>();

		// Token: 0x0400072E RID: 1838
		[ProtoMember(8)]
		public GiftCodeUserType UserType = GiftCodeUserType.All;

		// Token: 0x0400072F RID: 1839
		[ProtoMember(9)]
		public int UseCount = 0;

		// Token: 0x04000730 RID: 1840
		[ProtoMember(10)]
		public List<GoodsData> GoodsList = new List<GoodsData>();

		// Token: 0x04000731 RID: 1841
		[ProtoMember(11)]
		public List<GoodsData> ProGoodsList = new List<GoodsData>();

		// Token: 0x04000732 RID: 1842
		[ProtoMember(12)]
		public string Description = "";
	}
}
