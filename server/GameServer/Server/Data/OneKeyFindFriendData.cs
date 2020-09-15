using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000578 RID: 1400
	[ProtoContract]
	internal class OneKeyFindFriendData
	{
		// Token: 0x040025C4 RID: 9668
		[ProtoMember(1)]
		public int m_nRoleID = 0;

		// Token: 0x040025C5 RID: 9669
		[ProtoMember(2)]
		public string m_nRoleName = "";

		// Token: 0x040025C6 RID: 9670
		[ProtoMember(3)]
		public int m_nLevel = 0;

		// Token: 0x040025C7 RID: 9671
		[ProtoMember(4)]
		public int m_nChangeLifeLev = 0;

		// Token: 0x040025C8 RID: 9672
		[ProtoMember(5)]
		public int m_nOccupation = 0;
	}
}
