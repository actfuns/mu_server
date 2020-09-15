using System;
using System.Collections.Generic;
using ProtoBuf;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic.BocaiSys
{
	// Token: 0x02000070 RID: 112
	[ProtoContract]
	public class GetBoCaiResult
	{
		// Token: 0x0600017F RID: 383 RVA: 0x00018B09 File Offset: 0x00016D09
		public GetBoCaiResult()
		{
			this.ItemList = new List<BoCaiBuyItem>();
			this.OpenHistory = new List<BoCaiOpenHistory>();
			this.WinLotteryRoleList = new List<KFBoCaoHistoryData>();
		}

		// Token: 0x0400028F RID: 655
		[ProtoMember(1)]
		public int Info;

		// Token: 0x04000290 RID: 656
		[ProtoMember(2)]
		public int BocaiType;

		// Token: 0x04000291 RID: 657
		[ProtoMember(3)]
		public long NowPeriods;

		// Token: 0x04000292 RID: 658
		[ProtoMember(4)]
		public List<BoCaiBuyItem> ItemList;

		// Token: 0x04000293 RID: 659
		[ProtoMember(5)]
		public List<KFBoCaoHistoryData> WinLotteryRoleList;

		// Token: 0x04000294 RID: 660
		[ProtoMember(6)]
		public long OpenTime;

		// Token: 0x04000295 RID: 661
		[ProtoMember(7)]
		public bool IsOpen;

		// Token: 0x04000296 RID: 662
		[ProtoMember(8)]
		public string Value1;

		// Token: 0x04000297 RID: 663
		[ProtoMember(9)]
		public List<BoCaiOpenHistory> OpenHistory;

		// Token: 0x04000298 RID: 664
		[ProtoMember(10)]
		public int Stage;
	}
}
