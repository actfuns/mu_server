using System;
using ProtoBuf;

namespace KF.Contract.Data
{
	// Token: 0x02000023 RID: 35
	[ProtoContract]
	public class KuaFuServerGameConfigProtoData
	{
		// Token: 0x040000AE RID: 174
		[ProtoMember(1)]
		public int ServerId;

		// Token: 0x040000AF RID: 175
		[ProtoMember(2)]
		public int GameType;

		// Token: 0x040000B0 RID: 176
		[ProtoMember(3)]
		public int Capacity;
	}
}
