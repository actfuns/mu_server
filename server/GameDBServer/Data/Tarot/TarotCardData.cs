using System;
using ProtoBuf;

namespace GameDBServer.Data.Tarot
{
	// Token: 0x020000B7 RID: 183
	[ProtoContract]
	public class TarotCardData
	{
		// Token: 0x06000199 RID: 409 RVA: 0x00008B09 File Offset: 0x00006D09
		public TarotCardData()
		{
		}

		// Token: 0x0600019A RID: 410 RVA: 0x00008B30 File Offset: 0x00006D30
		public TarotCardData(string data)
		{
			string[] info = data.Split(new char[]
			{
				'_'
			}, StringSplitOptions.RemoveEmptyEntries);
			if (info.Length >= 3)
			{
				this.GoodId = Convert.ToInt32(info[0]);
				this.Level = Convert.ToInt32(info[1]);
				this.Postion = Convert.ToByte(info[2]);
				if (info.Length >= 4)
				{
					this.TarotMoney = Convert.ToInt32(info[3]);
				}
			}
		}

		// Token: 0x0600019B RID: 411 RVA: 0x00008BC8 File Offset: 0x00006DC8
		public string GetDataStrInfo()
		{
			return string.Format("{0}_{1}_{2}_{3};", new object[]
			{
				this.GoodId,
				this.Level,
				this.Postion,
				this.TarotMoney
			});
		}

		// Token: 0x040004D4 RID: 1236
		[ProtoMember(1)]
		public int GoodId = 0;

		// Token: 0x040004D5 RID: 1237
		[ProtoMember(2)]
		public int Level = 0;

		// Token: 0x040004D6 RID: 1238
		[ProtoMember(3)]
		public byte Postion = 0;

		// Token: 0x040004D7 RID: 1239
		[ProtoMember(4)]
		public int TarotMoney = 0;
	}
}
