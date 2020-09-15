using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000280 RID: 640
	[ProtoContract]
	public class CompBattleCifyData
	{
		// Token: 0x04000FF2 RID: 4082
		[ProtoMember(1)]
		public int CityID;

		// Token: 0x04000FF3 RID: 4083
		[ProtoMember(2)]
		public int RoleNum;

		// Token: 0x04000FF4 RID: 4084
		[ProtoMember(3)]
		public Dictionary<int, int> StrongholdDict = new Dictionary<int, int>();

		// Token: 0x04000FF5 RID: 4085
		[ProtoMember(4)]
		public List<CompBattleZhuJiangInfo> ZhuJiangList = new List<CompBattleZhuJiangInfo>();

		// Token: 0x04000FF6 RID: 4086
		[ProtoMember(5)]
		public int OwnCompType;
	}
}
