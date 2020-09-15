using System;
using System.Collections.Generic;
using Server.Data;

namespace GameDBServer.DB.DBController
{
	// Token: 0x020000E5 RID: 229
	public class JingJiChangZhaoBaoDBController : DBController<JingJiChallengeInfoData>
	{
		// Token: 0x060001FD RID: 509 RVA: 0x0000B3D4 File Offset: 0x000095D4
		private JingJiChangZhaoBaoDBController()
		{
		}

		// Token: 0x060001FE RID: 510 RVA: 0x0000B3E0 File Offset: 0x000095E0
		public static JingJiChangZhaoBaoDBController getInstnace()
		{
			return JingJiChangZhaoBaoDBController.instance;
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000B3F8 File Offset: 0x000095F8
		public List<JingJiChallengeInfoData> getChallengeInfoListByRoleId(int roleId)
		{
			string sql = string.Format("select * from t_jingjichang_zhanbao where roleId={0} order by pkid desc limit 50;", roleId);
			return base.queryForList(sql);
		}

		// Token: 0x06000200 RID: 512 RVA: 0x0000B424 File Offset: 0x00009624
		public bool insertZhanBao(JingJiChallengeInfoData zhanbaoData)
		{
			string sql = string.Format("insert into t_jingjichang_zhanbao (roleId, zhanbaoType, challengeName,value,createTime) values ({0},{1},'{2}',{3},'{4}');", new object[]
			{
				zhanbaoData.roleId,
				zhanbaoData.zhanbaoType,
				zhanbaoData.challengeName,
				zhanbaoData.value,
				zhanbaoData.createTime
			});
			return base.insert(sql) > 0;
		}

		// Token: 0x06000201 RID: 513 RVA: 0x0000B490 File Offset: 0x00009690
		public bool deleteByRoleId(int roleId, string createTime)
		{
			string sql = string.Format("delete from t_jingjichang_zhanbao where roleId = {0} and createTime < '{1}';", roleId, createTime);
			return base.delete(sql) > 0;
		}

		// Token: 0x04000627 RID: 1575
		private static JingJiChangZhaoBaoDBController instance = new JingJiChangZhaoBaoDBController();
	}
}
