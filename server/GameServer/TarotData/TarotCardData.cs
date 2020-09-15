using System;
using ProtoBuf;

namespace GameServer.TarotData
{
	// Token: 0x02000198 RID: 408
	[ProtoContract]
	public class TarotCardData
	{
		// Token: 0x060004DB RID: 1243 RVA: 0x0004286C File Offset: 0x00040A6C
		public string GetDataStrInfo()
		{
			return string.Format("{0}_{1}_{2}_{3}", new object[]
			{
				this.GoodId,
				this.Level,
				this.Postion,
				this.TarotMoney
			});
		}

		// Token: 0x04000900 RID: 2304
		[ProtoMember(1)]
		public int GoodId = 0;

		// Token: 0x04000901 RID: 2305
		[ProtoMember(2)]
		public int Level = 0;

		// Token: 0x04000902 RID: 2306
		[ProtoMember(3)]
		public byte Postion = 0;

		// Token: 0x04000903 RID: 2307
		[ProtoMember(4)]
		public int TarotMoney;
	}
}
