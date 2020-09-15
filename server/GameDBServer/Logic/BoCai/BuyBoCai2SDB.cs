using System;
using ProtoBuf;

namespace GameDBServer.Logic.BoCai
{
	// Token: 0x0200011D RID: 285
	[ProtoContract]
	public class BuyBoCai2SDB
	{
		// Token: 0x04000790 RID: 1936
		[ProtoMember(1)]
		public int m_RoleID;

		// Token: 0x04000791 RID: 1937
		[ProtoMember(2)]
		public string m_RoleName;

		// Token: 0x04000792 RID: 1938
		[ProtoMember(3)]
		public int ZoneID;

		// Token: 0x04000793 RID: 1939
		[ProtoMember(4)]
		public string strUserID;

		// Token: 0x04000794 RID: 1940
		[ProtoMember(5)]
		public int ServerId;

		// Token: 0x04000795 RID: 1941
		[ProtoMember(6)]
		public int BuyNum;

		// Token: 0x04000796 RID: 1942
		[ProtoMember(7)]
		public string strBuyValue;

		// Token: 0x04000797 RID: 1943
		[ProtoMember(8)]
		public bool IsSend;

		// Token: 0x04000798 RID: 1944
		[ProtoMember(9)]
		public bool IsWin;

		// Token: 0x04000799 RID: 1945
		[ProtoMember(10)]
		public int BocaiType;

		// Token: 0x0400079A RID: 1946
		[ProtoMember(11)]
		public long DataPeriods;
	}
}
