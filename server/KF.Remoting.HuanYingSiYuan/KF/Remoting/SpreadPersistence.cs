using System;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Maticsoft.DBUtility;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	// Token: 0x02000062 RID: 98
	public class SpreadPersistence
	{
		// Token: 0x0600046A RID: 1130 RVA: 0x00039A80 File Offset: 0x00037C80
		private SpreadPersistence()
		{
		}

		// Token: 0x0600046B RID: 1131 RVA: 0x00039AA0 File Offset: 0x00037CA0
		public void InitConfig()
		{
			try
			{
				XElement xmlFile = ConfigHelper.Load("config.xml");
				Consts.TelMaxCount = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "add", "key", "SpreadTelMaxCount", "value", 5L);
				Consts.TelTimeLimit = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "add", "key", "SpreadTelTimeLimit", "value", 20L) * 60;
				Consts.TelTimeStop = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "add", "key", "SpreadTelTimeStop", "value", 120L) * 60;
				Consts.VerifyRoleMaxCount = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "add", "key", "SpreadVerifyRoleMaxCount", "value", 5L);
				Consts.VerifyRoleTimeLimit = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "add", "key", "SpreadVerifyRoleTimeLimit", "value", 5L) * 60;
				Consts.VerifyRoleTimeStop = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "add", "key", "SpreadVerifyRoleTimeStop", "value", 30L) * 60;
				Consts.IsTest = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "add", "key", "IsTest", "value", 0L);
				this.Initialized = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x00039BF0 File Offset: 0x00037DF0
		private int ExecuteSqlNoQuery2(string sqlCmd)
		{
			int i = 0;
			try
			{
				i = DbHelperMySQL.ExecuteSql(sqlCmd);
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return i;
		}

		// Token: 0x0600046D RID: 1133 RVA: 0x00039C34 File Offset: 0x00037E34
		public bool DBSpreadSign(int pzoneID, int proleID)
		{
			string sql = string.Format("REPLACE INTO t_spread(logTime, zoneID, roleID) VALUES('{0}','{1}','{2}')", TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss"), pzoneID, proleID);
			int i = this.ExecuteSqlNoQuery2(sql);
			return i > 0;
		}

		// Token: 0x0600046E RID: 1134 RVA: 0x00039C7C File Offset: 0x00037E7C
		public bool DBSpreadSignCheck(int pzoneID, int proleID)
		{
			try
			{
				object ageObj = DbHelperMySQL.GetSingle(string.Format("select IFNULL(id,0) d from t_spread where zoneID = {0} and roleID={1}", pzoneID, proleID));
				if (null != ageObj)
				{
					int age = Convert.ToInt32(ageObj);
					if (age > 0)
					{
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return false;
		}

		// Token: 0x0600046F RID: 1135 RVA: 0x00039CF8 File Offset: 0x00037EF8
		public bool DBSpreadVeruftCheck(int czoneID, int croleID, string cuserID)
		{
			try
			{
				object ageObj = DbHelperMySQL.GetSingle(string.Format("select IFNULL(id,0) d from t_spread_role where (czoneID = {0} and croleID={1}) or (cuserID='{2}') limit 1", czoneID, croleID, cuserID));
				if (null != ageObj)
				{
					int age = Convert.ToInt32(ageObj);
					if (age > 0)
					{
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return false;
		}

		// Token: 0x06000470 RID: 1136 RVA: 0x00039D74 File Offset: 0x00037F74
		public int DBSpreadCountAll(int pzoneID, int proleID)
		{
			try
			{
				object ageObj = DbHelperMySQL.GetSingle(string.Format("select count(*) c from t_spread_role where pzoneID={0} and proleID={1}", pzoneID, proleID));
				if (null != ageObj)
				{
					return Convert.ToInt32(ageObj);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return 0;
		}

		// Token: 0x06000471 RID: 1137 RVA: 0x00039DDC File Offset: 0x00037FDC
		public int DBSpreadCountVip(int pzoneID, int proleID)
		{
			try
			{
				object ageObj = DbHelperMySQL.GetSingle(string.Format("select count(*) c from t_spread_role where isVip>0 and pzoneID={0} and proleID={1}", pzoneID, proleID));
				if (null != ageObj)
				{
					return Convert.ToInt32(ageObj);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return 0;
		}

		// Token: 0x06000472 RID: 1138 RVA: 0x00039E44 File Offset: 0x00038044
		public int DBSpreadCountLevel(int pzoneID, int proleID)
		{
			try
			{
				object ageObj = DbHelperMySQL.GetSingle(string.Format("select count(*) c from t_spread_role where isLevel>0 and pzoneID={0} and proleID={1}", pzoneID, proleID));
				if (null != ageObj)
				{
					return Convert.ToInt32(ageObj);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return 0;
		}

		// Token: 0x06000473 RID: 1139 RVA: 0x00039EAC File Offset: 0x000380AC
		public bool DBSpreadIsVip(int pzoneID, int proleID, int czoneID, int croleID)
		{
			string sql = string.Format("UPDATE t_spread_role set isVip=1 where pzoneID={0} and proleID={1} and czoneID={2} and croleID={3};", new object[]
			{
				pzoneID,
				proleID,
				czoneID,
				croleID
			});
			int i = this.ExecuteSqlNoQuery2(sql);
			return i > 0;
		}

		// Token: 0x06000474 RID: 1140 RVA: 0x00039F04 File Offset: 0x00038104
		public bool DBSpreadIsLevel(int pzoneID, int proleID, int czoneID, int croleID)
		{
			string sql = string.Format("UPDATE t_spread_role set isLevel=1 where  pzoneID={0} and proleID={1} and czoneID={2} and croleID={3};", new object[]
			{
				pzoneID,
				proleID,
				czoneID,
				croleID
			});
			int i = this.ExecuteSqlNoQuery2(sql);
			return i > 0;
		}

		// Token: 0x06000475 RID: 1141 RVA: 0x00039F5C File Offset: 0x0003815C
		public bool DBSpreadRoleAdd(int pzoneID, int proleID, string cuserID, int czoneID, int croleID, string tel, int isVip, int isLevel)
		{
			string sql = string.Format("INSERT INTO t_spread_role(pzoneID,proleID,cuserID,czoneID,croleID,tel,isVip,isLevel,logTime) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')", new object[]
			{
				pzoneID,
				proleID,
				cuserID,
				czoneID,
				croleID,
				tel,
				isVip,
				isLevel,
				TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss")
			});
			int i = this.ExecuteSqlNoQuery2(sql);
			return i > 0;
		}

		// Token: 0x06000476 RID: 1142 RVA: 0x00039FE8 File Offset: 0x000381E8
		public bool DBSpreadTelCodeAdd(int pzoneID, int proleID, int czoneID, int croleID, string tel, int telCode)
		{
			string sql = string.Format("INSERT INTO t_spread_tel(pzoneID, proleID, czoneID,croleID,tel,telCode,logTime) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", new object[]
			{
				pzoneID,
				proleID,
				czoneID,
				croleID,
				tel,
				telCode,
				TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss")
			});
			int i = this.ExecuteSqlNoQuery2(sql);
			return i > 0;
		}

		// Token: 0x06000477 RID: 1143 RVA: 0x0003A064 File Offset: 0x00038264
		public bool DBSpreadTelBind(string tel)
		{
			try
			{
				object ageObj = DbHelperMySQL.GetSingle(string.Format("select IFNULL(id,0) d from t_spread_role where tel='{0}'", tel));
				if (null != ageObj)
				{
					int age = Convert.ToInt32(ageObj);
					if (age > 0)
					{
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return false;
		}

		// Token: 0x0400026A RID: 618
		public static readonly SpreadPersistence Instance = new SpreadPersistence();

		// Token: 0x0400026B RID: 619
		public object Mutex = new object();

		// Token: 0x0400026C RID: 620
		public bool Initialized = false;
	}
}
