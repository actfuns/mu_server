using System;
using System.Collections.Generic;
using Server.Data;

namespace GameDBServer.DB.DBController
{
	// Token: 0x020000E1 RID: 225
	public class BaiTanLogDBController : DBController<BaiTanLogItemData>
	{
		// Token: 0x060001DC RID: 476 RVA: 0x0000A340 File Offset: 0x00008540
		private BaiTanLogDBController()
		{
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0000A34C File Offset: 0x0000854C
		public static BaiTanLogDBController getInstance()
		{
			return BaiTanLogDBController.instance;
		}

		// Token: 0x060001DE RID: 478 RVA: 0x0000A364 File Offset: 0x00008564
		public List<BaiTanLogItemData> getBaiTanLogItemDataList()
		{
			string sql = "select * from t_baitanbuy order by rid, buytime desc";
			return base.queryForList(sql);
		}

		// Token: 0x060001DF RID: 479 RVA: 0x0000A384 File Offset: 0x00008584
		public int insert(List<BaiTanLogItemData> dataList)
		{
			int i = 0;
			foreach (BaiTanLogItemData data in dataList)
			{
				i += this.insert(data);
			}
			return i;
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x0000A3E8 File Offset: 0x000085E8
		public int insert(BaiTanLogItemData data)
		{
			string sql = string.Format("insert into t_baitanbuy (rid,otherroleid,otherrname,goodsid,goodsnum,forgelevel,totalprice,leftyuanbao,buytime,yinliang,left_yinliang,tax,excellenceinfo,washprops) values ({0},{1},'{2}',{3},{4},{5},{6},{7},'{8}',{9},{10},{11},{12},'{13}');", new object[]
			{
				data.rid,
				data.OtherRoleID,
				data.OtherRName,
				data.GoodsID,
				data.GoodsNum,
				data.ForgeLevel,
				data.TotalPrice,
				data.LeftYuanBao,
				data.BuyTime,
				data.YinLiang,
				data.LeftYinLiang,
				data.Tax,
				data.Excellenceinfo,
				data.Washprops
			});
			return base.insert(sql);
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x0000A4D0 File Offset: 0x000086D0
		public int delete(int rid, string buytime)
		{
			string sql = string.Format("delete from t_baitanbuy where rid={0} and buytime<'{1}';", rid, buytime);
			return base.delete(sql);
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x0000A4FC File Offset: 0x000086FC
		public int delete(int rid)
		{
			string sql = string.Format("delete from t_baitanbuy where rid={0};", rid);
			return base.delete(sql);
		}

		// Token: 0x0400061C RID: 1564
		private static BaiTanLogDBController instance = new BaiTanLogDBController();
	}
}
