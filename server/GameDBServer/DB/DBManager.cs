using System;
using System.Collections.Generic;
using GameDBServer.Logic;
using GameDBServer.Logic.BoCai;
using GameDBServer.Logic.GoldAuction;
using GameDBServer.Logic.Name;
using MySQLDriverCS;
using Server.Tools;

namespace GameDBServer.DB
{
	// Token: 0x020000EA RID: 234
	public class DBManager
	{
		// Token: 0x0600021B RID: 539 RVA: 0x0000B9B0 File Offset: 0x00009BB0
		private DBManager()
		{
		}

		// Token: 0x0600021C RID: 540 RVA: 0x0000B9DC File Offset: 0x00009BDC
		public static DBManager getInstance()
		{
			return DBManager.instance;
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x0600021D RID: 541 RVA: 0x0000B9F4 File Offset: 0x00009BF4
		public DBConnections DBConns
		{
			get
			{
				return this._DBConns;
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x0600021E RID: 542 RVA: 0x0000BA0C File Offset: 0x00009C0C
		public DBUserMgr dbUserMgr
		{
			get
			{
				return this._DBUserMgr;
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600021F RID: 543 RVA: 0x0000BA24 File Offset: 0x00009C24
		public DBRoleMgr DBRoleMgr
		{
			get
			{
				return this._DBRoleMgr;
			}
		}

		// Token: 0x06000220 RID: 544 RVA: 0x0000BA3C File Offset: 0x00009C3C
		public int GetMaxConnsCount()
		{
			return this._DBConns.GetDBConnsCount();
		}

		// Token: 0x06000221 RID: 545 RVA: 0x0000BA59 File Offset: 0x00009C59
		private void CreateMemTables()
		{
		}

		// Token: 0x06000222 RID: 546 RVA: 0x0000BA5C File Offset: 0x00009C5C
		public void LoadDatabase(MySQLConnectionString connstr, int MaxConns, int codePage)
		{
			TianMaCharSet.ConvertToCodePage = codePage;
			this._DBConns.BuidConnections(connstr, MaxConns);
			MySQLConnection conn = this._DBConns.PopDBConnection();
			try
			{
				GameDBManager.BulletinMsgMgr.LoadBulletinMsgFromDB(this);
				GameDBManager.GameConfigMgr.LoadGameConfigFromDB(this);
				LiPinMaManager.LoadLiPinMaDB(this);
				PreNamesManager.LoadPremNamesFromDB(this);
				FuBenHistManager.LoadFuBenHist(this);
				PaiHangManager.ProcessPaiHang(this, true);
				GameDBManager.BangHuiJunQiMgr.LoadBangHuiJunQiItemFromDB(this);
				GameDBManager.PreDelRoleMgr.LoadPreDeleteRoleFromDB(this);
				GameDBManager.BangHuiLingDiMgr.LoadBangHuiLingDiItemsDictFromDB(this);
				HuangDiTeQuanMgr.LoadHuangDiTeQuan(this);
				GameDBManager.MarryPartyDataC.LoadPartyList(this);
				SingletonTemplate<NameUsedMgr>.Instance().LoadFromDatabase(this);
				GameDBManager.BangHuiListMgr.RefreshBangHuiListData(this);
				BanManager.GmBanCheckClear(this);
				FuMoMailManager.getInstance().LoadFuMoInfoFromDB(this);
				RebornStampManager.InitRebornYinJi(this);
				GlodAuctionMsgProcess.getInstance().LoadDataFromDB(this);
				BoCaiManager.getInstance().LoadDataFromDB(this);
			}
			finally
			{
				this._DBConns.PushDBConnection(conn);
			}
			this.CreateMemTables();
		}

		// Token: 0x06000223 RID: 547 RVA: 0x0000BB6C File Offset: 0x00009D6C
		public bool IsRolenameExist(string strRoleName)
		{
			MySQLConnection conn = this._DBConns.PopDBConnection();
			bool result;
			try
			{
				List<Tuple<int, string>> resultList = DBRoleInfo.QueryRoleIdList_ByRolename_IgnoreDbCmp(conn, strRoleName);
				if (resultList != null && resultList.Count > 0)
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
			finally
			{
				this._DBConns.PushDBConnection(conn);
			}
			return result;
		}

		// Token: 0x06000224 RID: 548 RVA: 0x0000BBD4 File Offset: 0x00009DD4
		public bool IsBangHuiNameExist(string strBhName)
		{
			MySQLConnection conn = this._DBConns.PopDBConnection();
			bool result;
			try
			{
				string sql = string.Format("SELECT * FROM t_banghui where bhname='{0}'", strBhName);
				MySQLCommand cmd = new MySQLCommand(sql, conn);
				MySQLDataReader reader = cmd.ExecuteReaderEx();
				if (reader.Read())
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
			catch (Exception ex)
			{
				result = true;
			}
			finally
			{
				this._DBConns.PushDBConnection(conn);
			}
			return result;
		}

		// Token: 0x06000225 RID: 549 RVA: 0x0000BC5C File Offset: 0x00009E5C
		public DBUserInfo GetDBUserInfo(string userID)
		{
			DBUserInfo dbUserInfo = this._DBUserMgr.FindDBUserInfo(userID);
			if (null == dbUserInfo)
			{
				dbUserInfo = new DBUserInfo();
				MySQLConnection conn = this._DBConns.PopDBConnection();
				try
				{
					if (!dbUserInfo.Query(conn, userID))
					{
						return null;
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
				finally
				{
					this._DBConns.PushDBConnection(conn);
				}
				dbUserInfo = this._DBUserMgr.AddDBUserInfo(dbUserInfo);
			}
			return dbUserInfo;
		}

		// Token: 0x06000226 RID: 550 RVA: 0x0000BD04 File Offset: 0x00009F04
		public DBRoleInfo GetDBRoleInfo(string rolename)
		{
			DBRoleInfo result;
			if (string.IsNullOrEmpty(rolename))
			{
				result = null;
			}
			else
			{
				int roleid = -1;
				MySQLConnection conn = this._DBConns.PopDBConnection();
				try
				{
					roleid = DBRoleInfo.QueryRoleID_ByRolename(conn, rolename);
				}
				finally
				{
					this._DBConns.PushDBConnection(conn);
				}
				result = this.GetDBRoleInfo(ref roleid);
			}
			return result;
		}

		// Token: 0x06000227 RID: 551 RVA: 0x0000BD6C File Offset: 0x00009F6C
		public DBRoleInfo FindDBRoleInfo(ref int roleID)
		{
			if (roleID < 200000)
			{
				int tempRoleID = roleID;
				roleID = SingletonTemplate<RoleMapper>.Instance().GetLocalRoleIDByTempID(tempRoleID);
			}
			DBRoleInfo result;
			if (roleID <= 0)
			{
				result = null;
			}
			else
			{
				result = this._DBRoleMgr.FindDBRoleInfo(ref roleID);
			}
			return result;
		}

		// Token: 0x06000228 RID: 552 RVA: 0x0000BDBC File Offset: 0x00009FBC
		public DBRoleInfo GetDBRoleInfo(ref int roleID)
		{
			int tempRoleID = 0;
			if (roleID < 200000)
			{
				tempRoleID = roleID;
				roleID = SingletonTemplate<RoleMapper>.Instance().GetLocalRoleIDByTempID(tempRoleID);
			}
			DBRoleInfo result;
			if (roleID <= 0)
			{
				result = null;
			}
			else
			{
				DBRoleInfo dbRoleInfo = this._DBRoleMgr.FindDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					dbRoleInfo = new DBRoleInfo();
					MySQLConnection conn = this._DBConns.PopDBConnection();
					try
					{
						if (!dbRoleInfo.Query(conn, roleID, false, tempRoleID))
						{
							return null;
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
					}
					finally
					{
						this._DBConns.PushDBConnection(conn);
					}
					DBQuery.QueryDJPointData(this, dbRoleInfo);
					dbRoleInfo = this._DBRoleMgr.AddDBRoleInfo(dbRoleInfo);
				}
				result = dbRoleInfo;
			}
			return result;
		}

		// Token: 0x06000229 RID: 553 RVA: 0x0000BEAC File Offset: 0x0000A0AC
		public DBRoleInfo GetDBAllRoleInfo(int roleID)
		{
			DBRoleInfo dbRoleInfo = this._DBRoleMgr.FindDBRoleInfo(ref roleID);
			if (null == dbRoleInfo)
			{
				dbRoleInfo = new DBRoleInfo();
				MySQLConnection conn = this._DBConns.PopDBConnection();
				try
				{
					if (!dbRoleInfo.Query(conn, roleID, false, 0))
					{
						return null;
					}
				}
				finally
				{
					this._DBConns.PushDBConnection(conn);
				}
				DBQuery.QueryDJPointData(this, dbRoleInfo);
				this._DBRoleMgr.AddDBRoleInfo(dbRoleInfo);
			}
			return dbRoleInfo;
		}

		// Token: 0x0400062E RID: 1582
		private static DBManager instance = new DBManager();

		// Token: 0x0400062F RID: 1583
		private DBConnections _DBConns = new DBConnections();

		// Token: 0x04000630 RID: 1584
		private DBUserMgr _DBUserMgr = new DBUserMgr();

		// Token: 0x04000631 RID: 1585
		private DBRoleMgr _DBRoleMgr = new DBRoleMgr();
	}
}
