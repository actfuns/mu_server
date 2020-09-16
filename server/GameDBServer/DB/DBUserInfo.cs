using System;
using System.Collections.Generic;
using GameDBServer.Logic;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.DB
{
	// Token: 0x0200019F RID: 415
	public class DBUserInfo
	{
		// Token: 0x170000BD RID: 189
		
		
		public string UserID { get; set; }

		// Token: 0x170000BE RID: 190
		
		public List<int> ListRoleIDs
		{
			get
			{
				return this._ListRoleIDs;
			}
		}

		// Token: 0x170000BF RID: 191
		
		public List<int> ListRoleSexes
		{
			get
			{
				return this._ListRoleSexes;
			}
		}

		// Token: 0x170000C0 RID: 192
		
		public List<int> ListRoleOccups
		{
			get
			{
				return this._ListRoleOccups;
			}
		}

		// Token: 0x170000C1 RID: 193
		
		public List<string> ListRoleNames
		{
			get
			{
				return this._ListRoleNames;
			}
		}

		// Token: 0x170000C2 RID: 194
		
		public List<int> ListRoleLevels
		{
			get
			{
				return this._ListRoleLevels;
			}
		}

		// Token: 0x170000C3 RID: 195
		
		public List<int> ListRoleZoneIDs
		{
			get
			{
				return this._ListRoleZoneIDs;
			}
		}

		// Token: 0x170000C4 RID: 196
		
		public List<int> ListRoleChangeLifeCount
		{
			get
			{
				return this._ListRoleChangeLifeCount;
			}
		}

		// Token: 0x170000C5 RID: 197
		
		public List<string> ListRolePreRemoveTime
		{
			get
			{
				return this._ListRolePreRemoveTime;
			}
		}

		// Token: 0x170000C6 RID: 198
		
		
		public string SecPwd { get; set; }

		// Token: 0x170000C7 RID: 199
		
		
		public int Money { get; set; }

		// Token: 0x170000C8 RID: 200
		
		
		public int RealMoney { get; set; }

		// Token: 0x170000C9 RID: 201
		
		
		public int GiftID { get; set; }

		// Token: 0x170000CA RID: 202
		
		
		public int GiftJiFen { get; set; }

		// Token: 0x170000CB RID: 203
		
		
		public int InputPoints { get; set; }

		// Token: 0x170000CC RID: 204
		
		
		public int SpecJiFen { get; set; }

		// Token: 0x170000CD RID: 205
		
		
		public int EveryJiFen { get; set; }

		// Token: 0x170000CE RID: 206
		
		
		public string PushMessageID { get; set; }

		// Token: 0x170000CF RID: 207
		
		
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

		// Token: 0x060007BF RID: 1983 RVA: 0x00046E64 File Offset: 0x00045064
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

		// Token: 0x060007C0 RID: 1984 RVA: 0x00047524 File Offset: 0x00045724
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

		// Token: 0x04000975 RID: 2421
		private List<int> _ListRoleIDs = new List<int>();

		// Token: 0x04000976 RID: 2422
		private List<int> _ListRoleSexes = new List<int>();

		// Token: 0x04000977 RID: 2423
		private List<int> _ListRoleOccups = new List<int>();

		// Token: 0x04000978 RID: 2424
		private List<string> _ListRoleNames = new List<string>();

		// Token: 0x04000979 RID: 2425
		private List<int> _ListRoleLevels = new List<int>();

		// Token: 0x0400097A RID: 2426
		private List<int> _ListRoleZoneIDs = new List<int>();

		// Token: 0x0400097B RID: 2427
		private List<int> _ListRoleChangeLifeCount = new List<int>();

		// Token: 0x0400097C RID: 2428
		private List<string> _ListRolePreRemoveTime = new List<string>();

		// Token: 0x0400097D RID: 2429
		private long _LastReferenceTicks = DateTime.Now.Ticks / 10000L;

		// Token: 0x0400097E RID: 2430
		public long LogoutServerTicks = 0L;
	}
}
