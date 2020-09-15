using System;
using System.Collections.Generic;
using ProtoBuf;

namespace KF.Contract.Data
{
	// Token: 0x02000022 RID: 34
	[ProtoContract]
	public class GetKuaFuServerListResponseData
	{
		// Token: 0x040000A8 RID: 168
		[ProtoMember(1)]
		public int GameType;

		// Token: 0x040000A9 RID: 169
		[ProtoMember(2)]
		public int ServerListAge;

		// Token: 0x040000AA RID: 170
		[ProtoMember(3)]
		public int ServerGameConfigAge;

		// Token: 0x040000AB RID: 171
		[ProtoMember(4)]
		public int GameConfigAge;

		// Token: 0x040000AC RID: 172
		[ProtoMember(5)]
		public List<KuaFuServerInfoProtoData> ServerList;

		// Token: 0x040000AD RID: 173
		[ProtoMember(6)]
		public List<KuaFuServerGameConfigProtoData> ServerGameConfigList;
	}
}
