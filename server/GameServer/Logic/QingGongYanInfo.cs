using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000541 RID: 1345
	public class QingGongYanInfo
	{
		// Token: 0x060019A7 RID: 6567 RVA: 0x0018F3E4 File Offset: 0x0018D5E4
		public bool IfBanTime(DateTime time)
		{
			int dayofweek = (int)time.DayOfWeek;
			if (dayofweek == 0)
			{
				dayofweek = 7;
			}
			foreach (string item in this.ProhibitedTimeList)
			{
				string[] strFields = this.ProhibitedTimeList[0].Split(new char[]
				{
					','
				});
				if (Convert.ToInt32(strFields[0]) == dayofweek)
				{
					DateTime beginTime = DateTime.Parse(strFields[1]);
					DateTime endTime = DateTime.Parse(strFields[2]);
					if (time >= beginTime && time <= endTime)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x040023FB RID: 9211
		public int Index;

		// Token: 0x040023FC RID: 9212
		public int NpcID;

		// Token: 0x040023FD RID: 9213
		public int MapCode;

		// Token: 0x040023FE RID: 9214
		public int X;

		// Token: 0x040023FF RID: 9215
		public int Y;

		// Token: 0x04002400 RID: 9216
		public int Direction;

		// Token: 0x04002401 RID: 9217
		public List<string> ProhibitedTimeList = new List<string>();

		// Token: 0x04002402 RID: 9218
		public string BeginTime;

		// Token: 0x04002403 RID: 9219
		public string OverTime;

		// Token: 0x04002404 RID: 9220
		public int FunctionID;

		// Token: 0x04002405 RID: 9221
		public int HoldBindJinBi;

		// Token: 0x04002406 RID: 9222
		public int TotalNum;

		// Token: 0x04002407 RID: 9223
		public int SingleNum;

		// Token: 0x04002408 RID: 9224
		public int JoinBindJinBi;

		// Token: 0x04002409 RID: 9225
		public int ExpAward;

		// Token: 0x0400240A RID: 9226
		public int XingHunAward;

		// Token: 0x0400240B RID: 9227
		public int ZhanGongAward;

		// Token: 0x0400240C RID: 9228
		public int ZuanShiCoe;
	}
}
