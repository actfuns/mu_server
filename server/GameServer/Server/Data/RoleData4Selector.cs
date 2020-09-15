using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000588 RID: 1416
	[ProtoContract]
	public class RoleData4Selector
	{
		// Token: 0x06001A0B RID: 6667 RVA: 0x00192180 File Offset: 0x00190380
		public RoleData4Selector Clone()
		{
			return base.MemberwiseClone() as RoleData4Selector;
		}

		// Token: 0x040026EE RID: 9966
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x040026EF RID: 9967
		[ProtoMember(2)]
		public string RoleName = "";

		// Token: 0x040026F0 RID: 9968
		[ProtoMember(3)]
		public int RoleSex = 0;

		// Token: 0x040026F1 RID: 9969
		[ProtoMember(4)]
		public int Occupation = 0;

		// Token: 0x040026F2 RID: 9970
		[ProtoMember(5)]
		public int Level = 1;

		// Token: 0x040026F3 RID: 9971
		[ProtoMember(6)]
		public int Faction = 0;

		// Token: 0x040026F4 RID: 9972
		[ProtoMember(7)]
		public string OtherName = "";

		// Token: 0x040026F5 RID: 9973
		[ProtoMember(8)]
		public List<GoodsData> GoodsDataList = null;

		// Token: 0x040026F6 RID: 9974
		[ProtoMember(9)]
		public WingData MyWingData = null;

		// Token: 0x040026F7 RID: 9975
		[ProtoMember(10)]
		public int CombatForce = 0;

		// Token: 0x040026F8 RID: 9976
		[ProtoMember(11)]
		public int AdmiredCount = 0;

		// Token: 0x040026F9 RID: 9977
		[ProtoMember(12)]
		public int FashionWingsID = 0;

		// Token: 0x040026FA RID: 9978
		[ProtoMember(13)]
		public long SettingBitFlags;

		// Token: 0x040026FB RID: 9979
		[ProtoMember(14)]
		public int ZoneId;

		// Token: 0x040026FC RID: 9980
		[ProtoMember(15)]
		public List<int> OccupationList;

		// Token: 0x040026FD RID: 9981
		[ProtoMember(16)]
		public int JunTuanId;

		// Token: 0x040026FE RID: 9982
		[ProtoMember(17)]
		public string JunTuanName;

		// Token: 0x040026FF RID: 9983
		[ProtoMember(18)]
		public int JunTuanZhiWu;

		// Token: 0x04002700 RID: 9984
		[ProtoMember(19)]
		public int LingDi;

		// Token: 0x04002701 RID: 9985
		[ProtoMember(20)]
		public int BuffFashionID;

		// Token: 0x04002702 RID: 9986
		[ProtoMember(21)]
		public RoleHuiJiData HuiJiData;

		// Token: 0x04002703 RID: 9987
		[ProtoMember(22)]
		public int CompType;

		// Token: 0x04002704 RID: 9988
		[ProtoMember(23)]
		public byte CompZhiWu;

		// Token: 0x04002705 RID: 9989
		[ProtoMember(24)]
		public int SubOccupation;
	}
}
