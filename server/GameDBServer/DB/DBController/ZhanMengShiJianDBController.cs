using System;
using System.Collections.Generic;
using GameDBServer.Data;

namespace GameDBServer.DB.DBController
{
	// Token: 0x020000EE RID: 238
	public class ZhanMengShiJianDBController : DBController<ZhanMengShiJianData>
	{
		// Token: 0x06000412 RID: 1042 RVA: 0x0001FD50 File Offset: 0x0001DF50
		private ZhanMengShiJianDBController()
		{
		}

		// Token: 0x06000413 RID: 1043 RVA: 0x0001FD5C File Offset: 0x0001DF5C
		public static ZhanMengShiJianDBController getInstance()
		{
			return ZhanMengShiJianDBController.instance;
		}

		// Token: 0x06000414 RID: 1044 RVA: 0x0001FD74 File Offset: 0x0001DF74
		public List<ZhanMengShiJianData> getZhanMengShiJianDataList()
		{
			string sql = "select * from t_zhanmengshijian order by bhid, createTime desc";
			return base.queryForList(sql);
		}

		// Token: 0x06000415 RID: 1045 RVA: 0x0001FD94 File Offset: 0x0001DF94
		public int insert(List<ZhanMengShiJianData> dataList)
		{
			int i = 0;
			foreach (ZhanMengShiJianData data in dataList)
			{
				i += this.insert(data);
			}
			return i;
		}

		// Token: 0x06000416 RID: 1046 RVA: 0x0001FDF8 File Offset: 0x0001DFF8
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

		// Token: 0x06000417 RID: 1047 RVA: 0x0001FE88 File Offset: 0x0001E088
		public int delete(int bhId, string createTime)
		{
			string sql = string.Format("delete from t_zhanmengshijian where bhId={0} and createTime<'{1}';", bhId, createTime);
			return base.delete(sql);
		}

		// Token: 0x06000418 RID: 1048 RVA: 0x0001FEB4 File Offset: 0x0001E0B4
		public int delete(int bhId)
		{
			string sql = string.Format("delete from t_zhanmengshijian where bhId={0};", bhId);
			return base.delete(sql);
		}

		// Token: 0x040006D0 RID: 1744
		private static ZhanMengShiJianDBController instance = new ZhanMengShiJianDBController();
	}
}
