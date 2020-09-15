using System;
using ProtoBuf;

namespace Tmsk.Contract
{
	// Token: 0x02000101 RID: 257
	[ProtoContract]
	public class HongBaoRecvData
	{
		// Token: 0x04000733 RID: 1843
		[ProtoMember(1)]
		public int HongBaoID;

		// Token: 0x04000734 RID: 1844
		[ProtoMember(2)]
		public int RoleId;

		// Token: 0x04000735 RID: 1845
		[ProtoMember(3)]
		public string RoleName;

		// Token: 0x04000736 RID: 1846
		[ProtoMember(4)]
		public int ZuanShi;

		// Token: 0x04000737 RID: 1847
		[ProtoMember(5)]
		public DateTime RecvTime;

		// Token: 0x04000738 RID: 1848
		[ProtoMember(6)]
		public int ZuiJia;

		// Token: 0x04000739 RID: 1849
		[ProtoMember(7)]
		public int BhId;
	}
}
