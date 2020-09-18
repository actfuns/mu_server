using System;
using System.Collections.Generic;
using GameDBServer.Data;

namespace GameDBServer.DB.DBController
{
	
	public class ZhanMengShiJianDBController : DBController<ZhanMengShiJianData>
	{
		
		private ZhanMengShiJianDBController()
		{
		}

		
		public static ZhanMengShiJianDBController getInstance()
		{
			return ZhanMengShiJianDBController.instance;
		}

		
		public List<ZhanMengShiJianData> getZhanMengShiJianDataList()
		{
			string sql = "select * from t_zhanmengshijian order by bhid, createTime desc";
			return base.queryForList(sql);
		}

		
		public int insert(List<ZhanMengShiJianData> dataList)
		{
			int i = 0;
			foreach (ZhanMengShiJianData data in dataList)
			{
				i += this.insert(data);
			}
			return i;
		}

		
		public int insert(ZhanMengShiJianData data)
		{
			string sql = string.Format("insert into t_zhanmengshijian (bhId,shijianType,roleName,createTime,subValue1,subValue2,subValue3,subSzValue1) values ({0},{1},'{2}','{3}',{4},{5},{6},'{7}');", new object[]
			{
				data.BHID,
				data.ShiJianType,
				data.RoleName,
				data.CreateTime,
				data.SubValue1,
				data.SubValue2,
				data.SubValue3,
				data.SubSzValue1
			});
			return base.insert(sql);
		}

		
		public int delete(int bhId, string createTime)
		{
			string sql = string.Format("delete from t_zhanmengshijian where bhId={0} and createTime<'{1}';", bhId, createTime);
			return base.delete(sql);
		}

		
		public int delete(int bhId)
		{
			string sql = string.Format("delete from t_zhanmengshijian where bhId={0};", bhId);
			return base.delete(sql);
		}

		
		private static ZhanMengShiJianDBController instance = new ZhanMengShiJianDBController();
	}
}
