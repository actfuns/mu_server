using System;
using ProtoBuf;

namespace GameDBServer.Logic.UserReturn
{
	// Token: 0x02000189 RID: 393
	[ProtoContract]
	public class ReturnActivity
	{
		// Token: 0x04000914 RID: 2324
		[ProtoMember(1)]
		public bool IsOpen;

		// Token: 0x04000915 RID: 2325
		[ProtoMember(2)]
		public DateTime NotLoggedInBegin;

		// Token: 0x04000916 RID: 2326
		[ProtoMember(3)]
		public DateTime NotLoggedInFinish;

		// Token: 0x04000917 RID: 2327
		[ProtoMember(4)]
		public int Level;

		// Token: 0x04000918 RID: 2328
		[ProtoMember(5)]
		public int VIPNeedExp;

		// Token: 0x04000919 RID: 2329
		[ProtoMember(6)]
		public int ActivityID;

		// Token: 0x0400091A RID: 2330
		[ProtoMember(7)]
		public string ActivityDay;
	}
}
