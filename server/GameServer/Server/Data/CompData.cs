using System;
using System.Collections.Generic;
using ProtoBuf;
using Tmsk.Contract.KuaFuData;

namespace Server.Data
{
	// Token: 0x02000283 RID: 643
	[ProtoContract]
	public class CompData
	{
		// Token: 0x04000FFA RID: 4090
		[ProtoMember(1)]
		public KFCompData kfCompData = new KFCompData();

		// Token: 0x04000FFB RID: 4091
		[ProtoMember(2)]
		public List<int> YestdBoomValueList = new List<int>();

		// Token: 0x04000FFC RID: 4092
		[ProtoMember(3)]
		public CompSelectData SelectData = new CompSelectData();
	}
}
