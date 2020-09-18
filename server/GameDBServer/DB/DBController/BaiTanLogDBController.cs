using System;
using System.Collections.Generic;
using Server.Data;

namespace GameDBServer.DB.DBController
{
	
	public class BaiTanLogDBController : DBController<BaiTanLogItemData>
	{
		
		private BaiTanLogDBController()
		{
		}

		
		public static BaiTanLogDBController getInstance()
		{
			return BaiTanLogDBController.instance;
		}

		
		public List<BaiTanLogItemData> getBaiTanLogItemDataList()
		{
			string sql = "select * from t_baitanbuy order by rid, buytime desc";
			return base.queryForList(sql);
		}

		
		public int insert(List<BaiTanLogItemData> dataList)
		{
			int i = 0;
			foreach (BaiTanLogItemData data in dataList)
			{
				i += this.insert(data);
			}
			return i;
		}

		
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

		
		public int delete(int rid, string buytime)
		{
			string sql = string.Format("delete from t_baitanbuy where rid={0} and buytime<'{1}';", rid, buytime);
			return base.delete(sql);
		}

		
		public int delete(int rid)
		{
			string sql = string.Format("delete from t_baitanbuy where rid={0};", rid);
			return base.delete(sql);
		}

		
		private static BaiTanLogDBController instance = new BaiTanLogDBController();
	}
}
