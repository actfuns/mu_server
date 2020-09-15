using System;
using ProtoBuf;

namespace GameServer.Logic.Olympics
{
	// Token: 0x0200038C RID: 908
	[ProtoContract]
	public class OlympicsGuessData
	{
		// Token: 0x06000F7E RID: 3966 RVA: 0x000F2F20 File Offset: 0x000F1120
		public OlympicsGuessData Clone(OlympicsGuessData data)
		{
			this.ID = data.ID;
			this.DayID = data.DayID;
			this.Content = data.Content;
			this.A = data.A;
			this.B = data.B;
			this.C = data.C;
			this.D = data.D;
			this.Answer = data.Answer;
			this.Grade = data.Grade;
			this.Select = data.Select;
			return this;
		}

		// Token: 0x040017EE RID: 6126
		[ProtoMember(1)]
		public int ID = 0;

		// Token: 0x040017EF RID: 6127
		[ProtoMember(2)]
		public int DayID = 0;

		// Token: 0x040017F0 RID: 6128
		[ProtoMember(3)]
		public string Content = "";

		// Token: 0x040017F1 RID: 6129
		[ProtoMember(4)]
		public string A = "";

		// Token: 0x040017F2 RID: 6130
		[ProtoMember(5)]
		public string B = "";

		// Token: 0x040017F3 RID: 6131
		[ProtoMember(6)]
		public string C = "";

		// Token: 0x040017F4 RID: 6132
		[ProtoMember(7)]
		public string D = "";

		// Token: 0x040017F5 RID: 6133
		[ProtoMember(8)]
		public int Answer = -1;

		// Token: 0x040017F6 RID: 6134
		[ProtoMember(9)]
		public int Grade = 0;

		// Token: 0x040017F7 RID: 6135
		[ProtoMember(10)]
		public int Select = -1;
	}
}
