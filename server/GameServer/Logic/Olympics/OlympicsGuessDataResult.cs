using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic.Olympics
{
	// Token: 0x0200038D RID: 909
	[ProtoContract]
	public class OlympicsGuessDataResult
	{
		// Token: 0x040017F8 RID: 6136
		[ProtoMember(1)]
		public int Type = 0;

		// Token: 0x040017F9 RID: 6137
		[ProtoMember(2)]
		public List<OlympicsGuessData> List = new List<OlympicsGuessData>();

		// Token: 0x040017FA RID: 6138
		[ProtoMember(3)]
		public int DayID = 0;
	}
}
