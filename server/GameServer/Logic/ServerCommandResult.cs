using System;
using ProtoBuf;

namespace GameServer.Logic
{
	// Token: 0x020006D6 RID: 1750
	[ProtoContract]
	public class ServerCommandResult
	{
		// Token: 0x0400395B RID: 14683
		[ProtoMember(1)]
		public string Cmd;

		// Token: 0x0400395C RID: 14684
		[ProtoMember(2)]
		public string ResultString;
	}
}
