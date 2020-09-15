using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000508 RID: 1288
	public class HuanYingSiYuanData
	{
		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060017BF RID: 6079 RVA: 0x00173720 File Offset: 0x00171920
		public int TotalSecs
		{
			get
			{
				return this.WaitingEnterSecs + this.PrepareSecs + this.FightingSecs + this.ClearRolesSecs;
			}
		}

		// Token: 0x04002212 RID: 8722
		public object Mutex = new object();

		// Token: 0x04002213 RID: 8723
		public Dictionary<RangeKey, int> Range2GroupIndexDict = new Dictionary<RangeKey, int>(RangeKey.Comparer);

		// Token: 0x04002214 RID: 8724
		public Dictionary<int, HuanYingSiYuanBirthPoint> MapBirthPointDict = new Dictionary<int, HuanYingSiYuanBirthPoint>();

		// Token: 0x04002215 RID: 8725
		public Dictionary<int, ContinuityKillAward> ContinuityKillAwardDict = new Dictionary<int, ContinuityKillAward>();

		// Token: 0x04002216 RID: 8726
		public int MapCode;

		// Token: 0x04002217 RID: 8727
		public List<TimeSpan> TimePoints = new List<TimeSpan>();

		// Token: 0x04002218 RID: 8728
		public int MinZhuanSheng = 1;

		// Token: 0x04002219 RID: 8729
		public int MinLevel = 1;

		// Token: 0x0400221A RID: 8730
		public int MinRequestNum = 1;

		// Token: 0x0400221B RID: 8731
		public int MaxEnterNum = 10;

		// Token: 0x0400221C RID: 8732
		public int FuBenId = 13000;

		// Token: 0x0400221D RID: 8733
		public int HoldShengBeiSecs = 60;

		// Token: 0x0400221E RID: 8734
		public int MinSubmitShengBeiSecs = 13;

		// Token: 0x0400221F RID: 8735
		public int TempleMirageMinJiFen = 0;

		// Token: 0x04002220 RID: 8736
		public int WaitingEnterSecs;

		// Token: 0x04002221 RID: 8737
		public int PrepareSecs;

		// Token: 0x04002222 RID: 8738
		public int FightingSecs;

		// Token: 0x04002223 RID: 8739
		public int ClearRolesSecs;

		// Token: 0x04002224 RID: 8740
		public Dictionary<int, ShengBeiData> ShengBeiDataDict = new Dictionary<int, ShengBeiData>();

		// Token: 0x04002225 RID: 8741
		public int MapGridWidth = 100;

		// Token: 0x04002226 RID: 8742
		public int MapGridHeight = 100;

		// Token: 0x04002227 RID: 8743
		public Dictionary<int, HuanYingSiYuanShengBeiContextData> ShengBeiContextDict = new Dictionary<int, HuanYingSiYuanShengBeiContextData>();

		// Token: 0x04002228 RID: 8744
		public long TempleMirageEXPAward;

		// Token: 0x04002229 RID: 8745
		public int TempleMirageAwardChengJiu;

		// Token: 0x0400222A RID: 8746
		public int TempleMirageAwardShengWang;

		// Token: 0x0400222B RID: 8747
		public int TempleMirageWin = 1000;

		// Token: 0x0400222C RID: 8748
		public int TempleMiragePK = 8;

		// Token: 0x0400222D RID: 8749
		public int TempleMirageWinExtraNum = 3;

		// Token: 0x0400222E RID: 8750
		public int TempleMirageWinExtraRate = 10;

		// Token: 0x0400222F RID: 8751
		public int FuBenGoodsBinding = 1;

		// Token: 0x04002230 RID: 8752
		public string AwardGoods;

		// Token: 0x04002231 RID: 8753
		public Dictionary<int, int> GameId2FuBenSeq = new Dictionary<int, int>();
	}
}
