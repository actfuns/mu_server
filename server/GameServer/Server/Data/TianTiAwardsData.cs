using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200048D RID: 1165
	[ProtoContract]
	public class TianTiAwardsData
	{
		// Token: 0x04001ED9 RID: 7897
		[ProtoMember(1)]
		public int Success;

		// Token: 0x04001EDA RID: 7898
		[ProtoMember(2)]
		public int DuanWeiJiFen;

		// Token: 0x04001EDB RID: 7899
		[ProtoMember(3)]
		public int RongYao;

		// Token: 0x04001EDC RID: 7900
		[ProtoMember(4)]
		public int LianShengJiFen;

		// Token: 0x04001EDD RID: 7901
		[ProtoMember(5)]
		public int DuanWeiId;
	}
}
