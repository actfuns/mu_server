using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200048E RID: 1166
	[ProtoContract]
	public class TianTiPaiHangRoleData
	{
		// Token: 0x04001EDE RID: 7902
		[ProtoMember(1)]
		public int RoleId;

		// Token: 0x04001EDF RID: 7903
		[ProtoMember(2)]
		public string RoleName;

		// Token: 0x04001EE0 RID: 7904
		[ProtoMember(3)]
		public int Occupation;

		// Token: 0x04001EE1 RID: 7905
		[ProtoMember(4)]
		public int ZoneId;

		// Token: 0x04001EE2 RID: 7906
		[ProtoMember(5)]
		public int ZhanLi;

		// Token: 0x04001EE3 RID: 7907
		[ProtoMember(6)]
		public int DuanWeiId;

		// Token: 0x04001EE4 RID: 7908
		[ProtoMember(7)]
		public int DuanWeiJiFen;

		// Token: 0x04001EE5 RID: 7909
		[ProtoMember(8)]
		public int DuanWeiRank;

		// Token: 0x04001EE6 RID: 7910
		[ProtoMember(9)]
		public RoleData4Selector RoleData4Selector;

		// Token: 0x04001EE7 RID: 7911
		[ProtoMember(10)]
		public int ZhengBaGrade;

		// Token: 0x04001EE8 RID: 7912
		[ProtoMember(11)]
		public int ZhengBaGroup;

		// Token: 0x04001EE9 RID: 7913
		[ProtoMember(12)]
		public int ZhengBaState;
	}
}
