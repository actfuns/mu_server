using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200021B RID: 539
	[ProtoContract]
	public class BangHuiMatchAwardsData
	{
		// Token: 0x04000C3C RID: 3132
		[ProtoMember(1)]
		public int Success;

		// Token: 0x04000C3D RID: 3133
		[ProtoMember(2)]
		public int BindJinBi;

		// Token: 0x04000C3E RID: 3134
		[ProtoMember(3)]
		public long Exp;

		// Token: 0x04000C3F RID: 3135
		[ProtoMember(4)]
		public List<AwardsItemData> AwardsItemDataList;

		// Token: 0x04000C40 RID: 3136
		[ProtoMember(5)]
		public string MvpRoleName;

		// Token: 0x04000C41 RID: 3137
		[ProtoMember(6)]
		public int MvpOccupation;

		// Token: 0x04000C42 RID: 3138
		[ProtoMember(7)]
		public int MvpRoleSex;

		// Token: 0x04000C43 RID: 3139
		[ProtoMember(8)]
		public string SuccessBHName;
	}
}
