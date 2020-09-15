using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000028 RID: 40
	[ProtoContract]
	public class AwardsItemData
	{
		// Token: 0x04000091 RID: 145
		[ProtoMember(1)]
		public int Occupation = 0;

		// Token: 0x04000092 RID: 146
		[ProtoMember(2)]
		public int GoodsID = 0;

		// Token: 0x04000093 RID: 147
		[ProtoMember(3)]
		public int GoodsNum = 0;

		// Token: 0x04000094 RID: 148
		[ProtoMember(4)]
		public int Binding = 0;

		// Token: 0x04000095 RID: 149
		[ProtoMember(5)]
		public int Level = 0;

		// Token: 0x04000096 RID: 150
		[ProtoMember(6)]
		public int Quality = 0;

		// Token: 0x04000097 RID: 151
		[ProtoMember(7)]
		public string EndTime = "";

		// Token: 0x04000098 RID: 152
		[ProtoMember(8)]
		public int BornIndex = 0;

		// Token: 0x04000099 RID: 153
		[ProtoMember(9)]
		public int RoleSex = 0;
	}
}
