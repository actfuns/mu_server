using System;
using ProtoBuf;

namespace GameServer.Logic.UserReturn
{
	// Token: 0x020004B2 RID: 1202
	[ProtoContract]
	public class ReturnData
	{
		// Token: 0x04001FE2 RID: 8162
		[ProtoMember(1)]
		public int DBID = 0;

		// Token: 0x04001FE3 RID: 8163
		[ProtoMember(2)]
		public int ActivityID = 0;

		// Token: 0x04001FE4 RID: 8164
		[ProtoMember(3)]
		public string ActivityDay = "";

		// Token: 0x04001FE5 RID: 8165
		[ProtoMember(4)]
		public int PZoneID = 0;

		// Token: 0x04001FE6 RID: 8166
		[ProtoMember(5)]
		public int PRoleID = 0;

		// Token: 0x04001FE7 RID: 8167
		[ProtoMember(6)]
		public int CZoneID = 0;

		// Token: 0x04001FE8 RID: 8168
		[ProtoMember(7)]
		public int CRoleID = 0;

		// Token: 0x04001FE9 RID: 8169
		[ProtoMember(8)]
		public int Vip = 0;

		// Token: 0x04001FEA RID: 8170
		[ProtoMember(9)]
		public int Level = 0;

		// Token: 0x04001FEB RID: 8171
		[ProtoMember(10)]
		public DateTime LogTime = DateTime.MinValue;

		// Token: 0x04001FEC RID: 8172
		[ProtoMember(11)]
		public int StateCheck = 0;

		// Token: 0x04001FED RID: 8173
		[ProtoMember(12)]
		public int StateLog = 0;

		// Token: 0x04001FEE RID: 8174
		[ProtoMember(13)]
		public int LeiJiChongZhi = 0;
	}
}
