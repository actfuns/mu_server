using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200019B RID: 411
	[ProtoContract]
	public class WarnInfo
	{
		// Token: 0x04000912 RID: 2322
		[ProtoMember(1, IsRequired = true)]
		public int ID = 0;

		// Token: 0x04000913 RID: 2323
		[ProtoMember(2, IsRequired = true)]
		public string Desc = "";

		// Token: 0x04000914 RID: 2324
		[ProtoMember(3, IsRequired = true)]
		public int TimeSec = 0;

		// Token: 0x04000915 RID: 2325
		[ProtoMember(4, IsRequired = true)]
		public int Operate = 0;
	}
}
