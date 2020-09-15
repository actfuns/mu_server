using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200013F RID: 319
	[ProtoContract]
	public class GiftCodeAwardData
	{
		// Token: 0x0600045A RID: 1114 RVA: 0x0003F880 File Offset: 0x0003DA80
		public void reset()
		{
			this.Dbid = 0;
			this.UserId = "";
			this.RoleID = 0;
			this.GiftId = "";
			this.CodeNo = "";
		}

		// Token: 0x04000722 RID: 1826
		[ProtoMember(1)]
		public int Dbid = 0;

		// Token: 0x04000723 RID: 1827
		[ProtoMember(2)]
		public string UserId = "";

		// Token: 0x04000724 RID: 1828
		[ProtoMember(3)]
		public int RoleID = 0;

		// Token: 0x04000725 RID: 1829
		[ProtoMember(4)]
		public string GiftId = "";

		// Token: 0x04000726 RID: 1830
		[ProtoMember(5)]
		public string CodeNo = "";
	}
}
