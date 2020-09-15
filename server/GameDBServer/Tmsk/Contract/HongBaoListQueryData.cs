using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Tmsk.Contract
{
	// Token: 0x020000FF RID: 255
	[ProtoContract]
	public class HongBaoListQueryData
	{
		// Token: 0x04000721 RID: 1825
		[ProtoMember(1)]
		public int BhId;

		// Token: 0x04000722 RID: 1826
		[ProtoMember(2)]
		public int Success;

		// Token: 0x04000723 RID: 1827
		[ProtoMember(3)]
		public List<HongBaoSendData> List;

		// Token: 0x04000724 RID: 1828
		[ProtoMember(4)]
		public string KeyStr;
	}
}
