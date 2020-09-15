using System;
using ProtoBuf;

namespace KF.Contract.Data
{
	// Token: 0x02000021 RID: 33
	[ProtoContract]
	public class GetKuaFuServerListRequestData
	{
		// Token: 0x040000A4 RID: 164
		[ProtoMember(1)]
		public int GameType;

		// Token: 0x040000A5 RID: 165
		[ProtoMember(2)]
		public int ServerListAge;

		// Token: 0x040000A6 RID: 166
		[ProtoMember(3)]
		public int ServerGameConfigAge;

		// Token: 0x040000A7 RID: 167
		[ProtoMember(4)]
		public int GameConfigAge;
	}
}
