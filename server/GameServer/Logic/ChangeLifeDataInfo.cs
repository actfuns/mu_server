using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x0200053A RID: 1338
	public class ChangeLifeDataInfo
	{
		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06001975 RID: 6517 RVA: 0x0018D4CC File Offset: 0x0018B6CC
		// (set) Token: 0x06001976 RID: 6518 RVA: 0x0018D4E3 File Offset: 0x0018B6E3
		public int ChangeLifeID { get; set; }

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06001977 RID: 6519 RVA: 0x0018D4EC File Offset: 0x0018B6EC
		// (set) Token: 0x06001978 RID: 6520 RVA: 0x0018D503 File Offset: 0x0018B703
		public int NeedLevel { get; set; }

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06001979 RID: 6521 RVA: 0x0018D50C File Offset: 0x0018B70C
		// (set) Token: 0x0600197A RID: 6522 RVA: 0x0018D523 File Offset: 0x0018B723
		public int NeedMoney { get; set; }

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x0600197B RID: 6523 RVA: 0x0018D52C File Offset: 0x0018B72C
		// (set) Token: 0x0600197C RID: 6524 RVA: 0x0018D543 File Offset: 0x0018B743
		public int NeedMoJing { get; set; }

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x0600197D RID: 6525 RVA: 0x0018D54C File Offset: 0x0018B74C
		// (set) Token: 0x0600197E RID: 6526 RVA: 0x0018D563 File Offset: 0x0018B763
		public List<GoodsData> NeedGoodsDataList { get; set; }

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x0600197F RID: 6527 RVA: 0x0018D56C File Offset: 0x0018B76C
		// (set) Token: 0x06001980 RID: 6528 RVA: 0x0018D583 File Offset: 0x0018B783
		public List<GoodsData> AwardGoodsDataList { get; set; }

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06001981 RID: 6529 RVA: 0x0018D58C File Offset: 0x0018B78C
		// (set) Token: 0x06001982 RID: 6530 RVA: 0x0018D5A3 File Offset: 0x0018B7A3
		public long ExpProportion { get; set; }

		// Token: 0x0400239A RID: 9114
		public ChangeLifePropertyInfo Propertyinfo = null;
	}
}
