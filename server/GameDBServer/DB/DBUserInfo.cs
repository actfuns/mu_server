using System;
using System.Collections.Generic;
using GameDBServer.Logic;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.DB
{
	
	public class DBUserInfo
	{
		
		
		
		public string UserID { get; set; }

		
		
		public List<int> ListRoleIDs
		{
			get
			{
				return this._ListRoleIDs;
			}
		}

		
		
		public List<int> ListRoleSexes
		{
			get
			{
				return this._ListRoleSexes;
			}
		}

		
		
		public List<int> ListRoleOccups
		{
			get
			{
				return this._ListRoleOccups;
			}
		}

		
		
		public List<string> ListRoleNames
		{
			get
			{
				return this._ListRoleNames;
			}
		}

		
		
		public List<int> ListRoleLevels
		{
			get
			{
				return this._ListRoleLevels;
			}
		}

		
		
		public List<int> ListRoleZoneIDs
		{
			get
			{
				return this._ListRoleZoneIDs;
			}
		}

		
		
		public List<int> ListRoleChangeLifeCount
		{
			get
			{
				return this._ListRoleChangeLifeCount;
			}
		}

		
		
		public List<string> ListRolePreRemoveTime
		{
			get
			{
				return this._ListRolePreRemoveTime;
			}
		}

		
		
		
		public string SecPwd { get; set; }

		
		
		
		public int Money { get; set; }

		
		
		
		public int RealMoney { get; set; }

		
		
		
		public int GiftID { get; set; }

		
		
		
		public int GiftJiFen { get; set; }

		
		
		
		public int InputPoints { get; set; }

		
		
		
		public int SpecJiFen { get; set; }

		
		
		
		public int EveryJiFen { get; set; }

		
		
		
		public string PushMessageID { get; set; }

		
		
		
		public long LastReferenceTicks
		{
			get
			{
				return this._LastReferenceTicks;
			}
			set
			{
				this._LastReferenceTicks = value;
			}
		}

		
		public bool Query(MySQLConnection conn, string userID)
		{
			LogManager.WriteLog(LogTypes.Info, string.Format("从数据库加载用户数据: {0}", userID), null, true);
			this.UserID = userID;
			string[] fields = new string[]
			{
				"rid",
				"userid",
				"rname",
				"sex",
				"occupation",
				"level",
				"zoneid",
				"changelifecount",
				"predeltime"
			};
			string[] tables = new string[]
			{
				"t_roles"
			};
			object[,] array = new object[2, 3];
			array[0, 0] = "userid";
			array[0, 1] = "=";
			array[0, 2] = userID;
			array[1, 0] = "isdel";
			array[1, 1] = "=";
			array[1, 2] = 0;
			MySQLSelectCommand cmd = new MySQLSelectCommand(conn, fields, tables, array, null, null);
			if (cmd.Table.Rows.Count > 0)
			{
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					this.ListRoleIDs.Add(Convert.ToInt32(cmd.Table.Rows[i]["rid"].ToString()));
					this.ListRoleNames.Add(cmd.Table.Rows[i]["rname"].ToString());
					this.ListRoleSexes.Add(Convert.ToInt32(cmd.Table.Rows[i]["sex"].ToString()));
					this.ListRoleOccups.Add(Convert.ToInt32(cmd.Table.Rows[i]["occupation"].ToString()));
					this.ListRoleLevels.Add(Convert.ToInt32(cmd.Table.Rows[i]["level"].ToString()));
					this.ListRoleZoneIDs.Add(Convert.ToInt32(cmd.Table.Rows[i]["zoneid"].ToString()));
					this.ListRoleChangeLifeCount.Add(Convert.ToInt32(cmd.Table.Rows[i]["changelifecount"].ToString()));
					this.ListRolePreRemoveTime.Add(cmd.Table.Rows[i]["predeltime"].ToString());
				}
			}
			this.Money = 0;
			string[] fields2 = new string[]
			{
				"money",
				"realmoney",
				"giftid",
				"giftjifen",
				"points",
				"specjifen",
				"everyjifen",
				"cc"
			};
			string[] tables2 = new string[]
			{
				"t_money"
			};
			array = new object[1, 3];
			array[0, 0] = "userid";
			array[0, 1] = "=";
			array[0, 2] = userID;
			cmd = new MySQLSelectCommand(conn, fields2, tables2, array, null, null);
			if (cmd.Table.Rows.Count > 0)
			{
				this.Money = Convert.ToInt32(cmd.Table.Rows[0]["money"].ToString());
				this.RealMoney = Convert.ToInt32(cmd.Table.Rows[0]["realmoney"].ToString());
				this.GiftID = Convert.ToInt32(cmd.Table.Rows[0]["giftid"].ToString());
				this.GiftJiFen = Convert.ToInt32(cmd.Table.Rows[0]["giftjifen"].ToString());
				this.InputPoints = Convert.ToInt32(cmd.Table.Rows[0]["points"].ToString());
				this.SpecJiFen = Convert.ToInt32(cmd.Table.Rows[0]["specjifen"].ToString());
				this.EveryJiFen = Convert.ToInt32(cmd.Table.Rows[0]["everyjifen"].ToString());
				string cc = cmd.Table.Rows[0]["cc"].ToString();
				if (!Global.CCC(cc, 3, new object[]
				{
					userID,
					this.Money,
					this.RealMoney
				}))
				{
					LogManager.WriteLog(LogTypes.DataCheck, string.Format("DataCheckFaild#t_money#userid={0},money={1},cc={2}", userID, this.Money, cc), null, true);
					return false;
				}
			}
			string[] fields3 = new string[]
			{
				"userid",
				"pushid",
				"lastlogintime"
			};
			string[] tables3 = new string[]
			{
				"t_pushmessageinfo"
			};
			array = new object[1, 3];
			array[0, 0] = "userid";
			array[0, 1] = "=";
			array[0, 2] = userID;
			cmd = new MySQLSelectCommand(conn, fields3, tables3, array, null, null);
			if (cmd.Table.Rows.Count > 0)
			{
				this.PushMessageID = cmd.Table.Rows[0]["pushid"].ToString();
			}
			string[] fields4 = new string[]
			{
				"userid",
				"secpwd"
			};
			string[] tables4 = new string[]
			{
				"t_secondpassword"
			};
			array = new object[1, 3];
			array[0, 0] = "userid";
			array[0, 1] = "=";
			array[0, 2] = userID;
			cmd = new MySQLSelectCommand(conn, fields4, tables4, array, null, null);
			if (cmd.Table.Rows.Count > 0)
			{
				this.SecPwd = cmd.Table.Rows[0]["secpwd"].ToString();
			}
			else
			{
				this.SecPwd = "";
			}
			return true;
		}

		
		public UserMiniData GetUserMiniData(string userId, int roleId, int OnlyZoneId)
		{
			UserMiniData userMimiData = new UserMiniData();
			userMimiData.UserId = this.UserID;
			userMimiData.RealMoney = this.RealMoney;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				MySQLConnection dbConn = conn.DbConn;
				string[] fields = new string[]
				{
					"rid",
					"rname",
					"sex",
					"occupation",
					"level",
					"zoneid",
					"changelifecount",
					"regtime",
					"lasttime",
					"logofftime"
				};
				string[] tables = new string[]
				{
					"t_roles"
				};
				object[,] array = new object[3, 3];
				array[0, 0] = "userid";
				array[0, 1] = "=";
				array[0, 2] = userId;
				array[1, 0] = "isdel";
				array[1, 1] = "=";
				array[1, 2] = 0;
				array[2, 0] = "zoneid";
				array[2, 1] = "=";
				array[2, 2] = OnlyZoneId;
				MySQLSelectCommand cmd = new MySQLSelectCommand(dbConn, fields, tables, array, null, null);
				if (cmd.Table.Rows.Count > 0)
				{
					for (int i = 0; i < cmd.Table.Rows.Count; i++)
					{
						int rid = Convert.ToInt32(cmd.Table.Rows[i]["rid"].ToString());
						string rname = cmd.Table.Rows[i]["rname"].ToString();
						int sex = Convert.ToInt32(cmd.Table.Rows[i]["sex"].ToString());
						int occupation = Convert.ToInt32(cmd.Table.Rows[i]["occupation"].ToString());
						int level = Convert.ToInt32(cmd.Table.Rows[i]["level"].ToString());
						int zoneId = Convert.ToInt32(cmd.Table.Rows[i]["zoneid"].ToString());
						int changeLifeCount = Convert.ToInt32(cmd.Table.Rows[i]["changelifecount"].ToString());
						DateTime createTime;
						DateTime.TryParse(cmd.Table.Rows[i]["regtime"].ToString(), out createTime);
						DateTime lastTime;
						DateTime.TryParse(cmd.Table.Rows[i]["lasttime"].ToString(), out lastTime);
						DateTime logoffTime;
						DateTime.TryParse(cmd.Table.Rows[i]["logofftime"].ToString(), out logoffTime);
						if (rid == roleId)
						{
							userMimiData.RoleCreateTime = createTime;
							userMimiData.RoleLastLoginTime = lastTime;
							userMimiData.RoleLastLogoutTime = logoffTime;
						}
						if (userMimiData.MinCreateRoleTime > createTime)
						{
							userMimiData.MinCreateRoleTime = createTime;
						}
						if (userMimiData.LastLoginTime < lastTime)
						{
							userMimiData.LastLoginTime = lastTime;
							userMimiData.LastRoleId = rid;
						}
						if (userMimiData.LastLogoutTime < logoffTime)
						{
							userMimiData.LastLogoutTime = logoffTime;
						}
						if ((userMimiData.MaxChangeLifeCount << 16) + userMimiData.MaxLevel < (changeLifeCount << 16) + level)
						{
							userMimiData.MaxChangeLifeCount = changeLifeCount;
							userMimiData.MaxLevel = level;
						}
					}
				}
			}
			return userMimiData;
		}

		
		private List<int> _ListRoleIDs = new List<int>();

		
		private List<int> _ListRoleSexes = new List<int>();

		
		private List<int> _ListRoleOccups = new List<int>();

		
		private List<string> _ListRoleNames = new List<string>();

		
		private List<int> _ListRoleLevels = new List<int>();

		
		private List<int> _ListRoleZoneIDs = new List<int>();

		
		private List<int> _ListRoleChangeLifeCount = new List<int>();

		
		private List<string> _ListRolePreRemoveTime = new List<string>();

		
		private long _LastReferenceTicks = DateTime.Now.Ticks / 10000L;

		
		public long LogoutServerTicks = 0L;
	}
}
