using System;
using ProtoBuf;

namespace KF.Contract.Data
{
	// Token: 0x02000024 RID: 36
	[ProtoContract]
	public class KuaFuServerInfoProtoData
	{
		// Token: 0x040000B1 RID: 177
		[ProtoMember(1)]
		public int ServerId;

		// Token: 0x040000B2 RID: 178
		[ProtoMember(2)]
		public string Ip;

		// Token: 0x040000B3 RID: 179
		[ProtoMember(3)]
		public int Port;

		// Token: 0x040000B4 RID: 180
		[ProtoMember(4)]
		public int DbPort;

		// Token: 0x040000B5 RID: 181
		[ProtoMember(5)]
		public int LogDbPort;

		// Token: 0x040000B6 RID: 182
		[ProtoMember(6)]
		public int Flags;
	}
}
