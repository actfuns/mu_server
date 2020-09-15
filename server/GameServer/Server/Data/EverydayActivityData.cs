using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000031 RID: 49
	[ProtoContract]
	public class EverydayActivityData
	{
		// Token: 0x04000109 RID: 265
		[ProtoMember(1)]
		public List<EverydayActInfo> EveryActInfoList;
	}
}
