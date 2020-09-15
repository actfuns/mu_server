using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200019D RID: 413
	[ProtoContract]
	public class YueKaData
	{
		// Token: 0x060004E1 RID: 1249 RVA: 0x00042A25 File Offset: 0x00040C25
		public YueKaData()
		{
			this.HasYueKa = false;
			this.CurrDay = 0;
			this.AwardInfo = "";
			this.RemainDay = 0;
		}

		// Token: 0x0400091B RID: 2331
		[ProtoMember(1)]
		public bool HasYueKa;

		// Token: 0x0400091C RID: 2332
		[ProtoMember(2)]
		public int CurrDay;

		// Token: 0x0400091D RID: 2333
		[ProtoMember(3)]
		public string AwardInfo;

		// Token: 0x0400091E RID: 2334
		[ProtoMember(4)]
		public int RemainDay;
	}
}
