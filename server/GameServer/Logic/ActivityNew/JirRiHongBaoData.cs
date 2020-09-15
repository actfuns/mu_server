using System;
using ProtoBuf;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x0200002A RID: 42
	[ProtoContract]
	public class JirRiHongBaoData
	{
		// Token: 0x040000EA RID: 234
		[ProtoMember(1)]
		public int ID;

		// Token: 0x040000EB RID: 235
		[ProtoMember(2)]
		public int State;
	}
}
