using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000A4 RID: 164
	[ProtoContract]
	public class RoleData4Selector
	{
		// Token: 0x040003A3 RID: 931
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x040003A4 RID: 932
		[ProtoMember(2)]
		public string RoleName = "";

		// Token: 0x040003A5 RID: 933
		[ProtoMember(3)]
		public int RoleSex = 0;

		// Token: 0x040003A6 RID: 934
		[ProtoMember(4)]
		public int Occupation = 0;

		// Token: 0x040003A7 RID: 935
		[ProtoMember(5)]
		public int Level = 1;

		// Token: 0x040003A8 RID: 936
		[ProtoMember(6)]
		public int Faction = 0;

		// Token: 0x040003A9 RID: 937
		[ProtoMember(7)]
		public string OtherName = "";

		// Token: 0x040003AA RID: 938
		[ProtoMember(8)]
		public List<GoodsData> GoodsDataList = null;

		// Token: 0x040003AB RID: 939
		[ProtoMember(9)]
		public WingData MyWingData = null;

		// Token: 0x040003AC RID: 940
		[ProtoMember(10)]
		public int CombatForce = 0;

		// Token: 0x040003AD RID: 941
		[ProtoMember(11)]
		public int AdmiredCount = 0;

		// Token: 0x040003AE RID: 942
		[ProtoMember(12)]
		public int FashionWingsID = 0;

		// Token: 0x040003AF RID: 943
		[ProtoMember(13)]
		public long SettingBitFlags;

		// Token: 0x040003B0 RID: 944
		[ProtoMember(14)]
		public int ZoneId;

		// Token: 0x040003B1 RID: 945
		[ProtoMember(15)]
		public List<int> OccupationList;

		// Token: 0x040003B2 RID: 946
		[ProtoMember(16)]
		public int JunTuanId;

		// Token: 0x040003B3 RID: 947
		[ProtoMember(17)]
		public string JunTuanName;

		// Token: 0x040003B4 RID: 948
		[ProtoMember(18)]
		public int JunTuanZhiWu;

		// Token: 0x040003B5 RID: 949
		[ProtoMember(19)]
		public int LingDi;

		// Token: 0x040003B6 RID: 950
		[ProtoMember(20)]
		public int BuffFashionID;

		// Token: 0x040003B7 RID: 951
		[ProtoMember(21)]
		public RoleHuiJiData HuiJiData;

		// Token: 0x040003B8 RID: 952
		[ProtoMember(22)]
		public int CompType;

		// Token: 0x040003B9 RID: 953
		[ProtoMember(23)]
		public byte CompZhiWu;

		// Token: 0x040003BA RID: 954
		[ProtoMember(24)]
		public int SubOccupation;
	}
}
