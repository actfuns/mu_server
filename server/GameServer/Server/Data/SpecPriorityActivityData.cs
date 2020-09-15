using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200001F RID: 31
	[ProtoContract]
	public class SpecPriorityActivityData
	{
		// Token: 0x040000BE RID: 190
		[ProtoMember(1)]
		public Dictionary<int, int> ConditionDict = new Dictionary<int, int>();

		// Token: 0x040000BF RID: 191
		[ProtoMember(2)]
		public List<SpecPriorityActInfo> SpecActInfoList = new List<SpecPriorityActInfo>();

		// Token: 0x040000C0 RID: 192
		[ProtoMember(3)]
		public int DonateNum;

		// Token: 0x040000C1 RID: 193
		[ProtoMember(4)]
		public int DonateNumKF;
	}
}
