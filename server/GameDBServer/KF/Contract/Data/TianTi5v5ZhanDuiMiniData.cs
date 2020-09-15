using System;
using System.Collections.Generic;
using ProtoBuf;

namespace KF.Contract.Data
{
	// Token: 0x020000BF RID: 191
	[ProtoContract]
	[Serializable]
	public class TianTi5v5ZhanDuiMiniData
	{
		// Token: 0x04000511 RID: 1297
		[ProtoMember(1)]
		public int ZhanDuiID;

		// Token: 0x04000512 RID: 1298
		[ProtoMember(2)]
		public string Name;

		// Token: 0x04000513 RID: 1299
		[ProtoMember(3)]
		public string DuiZhangName;

		// Token: 0x04000514 RID: 1300
		[ProtoMember(4)]
		public int DuanWeiID;

		// Token: 0x04000515 RID: 1301
		[ProtoMember(5)]
		public List<RoleNameLevelData> MemberList;

		// Token: 0x04000516 RID: 1302
		[ProtoMember(6)]
		public string XuanYan;

		// Token: 0x04000517 RID: 1303
		[ProtoMember(7)]
		public long ZhanDouLi;
	}
}
