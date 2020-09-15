using System;
using System.Collections.Generic;

namespace GameServer.Logic.ZhuanPan
{
	// Token: 0x0200082F RID: 2095
	public class ZhuanPanData
	{
		// Token: 0x0400456D RID: 17773
		public object Mutex = new object();

		// Token: 0x0400456E RID: 17774
		public bool ZhuanPanOpen;

		// Token: 0x0400456F RID: 17775
		public List<List<int>> ZhuanPanConstArray;

		// Token: 0x04004570 RID: 17776
		public int ZhuanPanFree;

		// Token: 0x04004571 RID: 17777
		public int ZhuanPanZuanShiFuLi;

		// Token: 0x04004572 RID: 17778
		public DateTime BeginTime;

		// Token: 0x04004573 RID: 17779
		public DateTime EndTime;

		// Token: 0x04004574 RID: 17780
		public List<ZhuanPanItem> ZhuanPanItemXmlList;

		// Token: 0x04004575 RID: 17781
		public Dictionary<int, Dictionary<int, ZhuanPanAwardItem>> ZhuanPanAwardXmlDict;

		// Token: 0x04004576 RID: 17782
		public List<ZhuanPanGongGaoData> GongGaoList = new List<ZhuanPanGongGaoData>();
	}
}
