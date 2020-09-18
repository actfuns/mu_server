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
	
	public class DBManager
	{
		
		private DBManager()
		{
		}

		
		public static DBManager getInstance()
		{
			return DBManager.instance;
		}

		
		
		public DBConnections DBConns
		{
			get
			{
				return this._DBConns;
			}
		}

		
		
		public DBUserMgr dbUserMgr
		{
			get
			{
				return this._DBUserMgr;
			}
		}

		
		
		public DBRoleMgr DBRoleMgr
		{
			get
			{
				return this._DBRoleMgr;
			}
		}

		
		public int GetMaxConnsCount()
		{
			return this._DBConns.GetDBConnsCount();
		}

		
		private void CreateMemTables()
		{
		}

		
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

		
		private static DBManager instance = new DBManager();

		
		private DBConnections _DBConns = new DBConnections();

		
		private DBUserMgr _DBUserMgr = new DBUserMgr();

		
		private DBRoleMgr _DBRoleMgr = new DBRoleMgr();
	}
}
