using System;
using ProtoBuf;

namespace GameServer.Logic.BocaiSys
{
	// Token: 0x02000071 RID: 113
	[ProtoContract]
	public class BoCaiOpenHistory
	{
		// Token: 0x04000299 RID: 665
		[ProtoMember(1)]
		public long DataPeriods;

		// Token: 0x0400029A RID: 666
		[ProtoMember(2)]
		public string OpenValue;
	}
}
