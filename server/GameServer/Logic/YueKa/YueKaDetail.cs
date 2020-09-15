using System;
using Server.Data;

namespace GameServer.Logic.YueKa
{
	// Token: 0x02000821 RID: 2081
	public class YueKaDetail
	{
		// Token: 0x06003AE0 RID: 15072 RVA: 0x0031F7E0 File Offset: 0x0031D9E0
		public YueKaDetail()
		{
			this.HasYueKa = 0;
			this.BegOffsetDay = 0;
			this.EndOffsetDay = 0;
			this.CurOffsetDay = 0;
			this.AwardInfo = "";
		}

		// Token: 0x06003AE1 RID: 15073 RVA: 0x0031F844 File Offset: 0x0031DA44
		public YueKaData ToYueKaData()
		{
			return new YueKaData
			{
				HasYueKa = (this.HasYueKa == 1),
				CurrDay = this.CurDayOfPerYueKa(),
				AwardInfo = this.AwardInfo,
				RemainDay = this.RemainDayOfYueKa()
			};
		}

		// Token: 0x06003AE2 RID: 15074 RVA: 0x0031F898 File Offset: 0x0031DA98
		public void ParseFrom(string str)
		{
			if (!string.IsNullOrEmpty(str))
			{
				string[] fields = str.Split(new char[]
				{
					','
				});
				if (fields.Length == 5)
				{
					this.HasYueKa = Convert.ToInt32(fields[0]);
					this.BegOffsetDay = Convert.ToInt32(fields[1]);
					this.EndOffsetDay = Convert.ToInt32(fields[2]);
					this.CurOffsetDay = Convert.ToInt32(fields[3]);
					this.AwardInfo = fields[4];
				}
			}
		}

		// Token: 0x06003AE3 RID: 15075 RVA: 0x0031F91C File Offset: 0x0031DB1C
		public string SerializeToString()
		{
			string result;
			if (this.HasYueKa == 0)
			{
				result = "0,0,0,0,0";
			}
			else
			{
				result = string.Format("{0},{1},{2},{3},{4}", new object[]
				{
					1,
					this.BegOffsetDay,
					this.EndOffsetDay,
					this.CurOffsetDay,
					this.AwardInfo
				});
			}
			return result;
		}

		// Token: 0x06003AE4 RID: 15076 RVA: 0x0031F998 File Offset: 0x0031DB98
		public int CurDayOfPerYueKa()
		{
			int result;
			if (this.HasYueKa == 0)
			{
				result = 0;
			}
			else
			{
				result = (this.CurOffsetDay - this.BegOffsetDay) % YueKaManager.DAYS_PER_YUE_KA + 1;
			}
			return result;
		}

		// Token: 0x06003AE5 RID: 15077 RVA: 0x0031F9D8 File Offset: 0x0031DBD8
		public int RemainDayOfYueKa()
		{
			int result;
			if (this.HasYueKa == 0)
			{
				result = 0;
			}
			else
			{
				result = this.EndOffsetDay - this.CurOffsetDay;
			}
			return result;
		}

		// Token: 0x040044F1 RID: 17649
		public int HasYueKa = 0;

		// Token: 0x040044F2 RID: 17650
		public int BegOffsetDay = 0;

		// Token: 0x040044F3 RID: 17651
		public int EndOffsetDay = 0;

		// Token: 0x040044F4 RID: 17652
		public int CurOffsetDay = 0;

		// Token: 0x040044F5 RID: 17653
		public string AwardInfo = "";
	}
}
