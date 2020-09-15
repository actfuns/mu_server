using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x020001CA RID: 458
	public class GameConfig
	{
		// Token: 0x06000938 RID: 2360 RVA: 0x00058EF4 File Offset: 0x000570F4
		public void InitGameDBManagerFlags(bool init = false)
		{
			GameDBManager.Flag_t_goods_delete_immediately = (GameDBManager.GameConfigMgr.GetGameConfigItemInt("flag_t_goods_delete_immediately", 1) > 0);
			GameDBManager.Flag_Query_Total_UserMoney_Minute = GameDBManager.GameConfigMgr.GetGameConfigItemInt("query_total_usermoney_minute", 60);
			GameDBManager.PreDeleteRoleDelaySeconds = GameDBManager.GameConfigMgr.GetGameConfigItemInt("DeleteRoleNeedTime", 120) * 60;
			GameDBManager.DisableSomeLog = ((GameDBManager.GameConfigMgr.GetGameConfigItemInt("opflags", 0) & 1) != 0);
			GameDBManager.PTID = GameDBManager.GameConfigMgr.GetGameConfigItemInt("ptid", 0);
			BangHuiNumLevelMgr.MaxQueryTimeSlotTicks = (long)(GameDBManager.GameConfigMgr.GetGameConfigItemInt("banghuiproctime1", 60) * 1000 * 10000);
			string str = GameDBManager.GameConfigMgr.GetGameConfigItemStr("rolepaihangkeys", "yinliang,combatforce,killboss");
			if (!string.IsNullOrEmpty(str))
			{
				lock (this.RolePaiHangKeys)
				{
					this.RolePaiHangKeys.Clear();
					string[] array = str.Split(new char[]
					{
						','
					});
					foreach (string key in array)
					{
						if (!string.IsNullOrEmpty(key))
						{
							this.RolePaiHangKeys.Add(key);
						}
					}
				}
			}
			if (!this.Initialized)
			{
				this.Initialized = true;
				GameDBManager.Flag_Splite_RoleParams_Table = GameDBManager.GameConfigMgr.GetGameConfigItemInt("opt_roleparams", 1);
			}
		}

		// Token: 0x06000939 RID: 2361 RVA: 0x0005908C File Offset: 0x0005728C
		public bool IsPaiHangKey(string key)
		{
			bool result;
			if (string.IsNullOrEmpty(key))
			{
				result = false;
			}
			else
			{
				lock (this.RolePaiHangKeys)
				{
					result = this.RolePaiHangKeys.Contains(key);
				}
			}
			return result;
		}

		// Token: 0x0600093A RID: 2362 RVA: 0x000590F0 File Offset: 0x000572F0
		public void LoadGameConfigFromDB(DBManager dbMgr)
		{
			this._GameConfigDict = DBQuery.QueryGameConfigDict(dbMgr);
			if (null == this._GameConfigDict)
			{
				this._GameConfigDict = new Dictionary<string, string>();
			}
			if (!string.IsNullOrEmpty(GameDBManager.serverDBInfo.ServerKey))
			{
				this._GameConfigDict["loginwebkey"] = GameDBManager.serverDBInfo.ServerKey;
			}
			this.InitGameDBManagerFlags(false);
		}

		// Token: 0x0600093B RID: 2363 RVA: 0x00059160 File Offset: 0x00057360
		public void UpdateGameConfigItem(string paramName, string paramValue)
		{
			lock (this._GameConfigDict)
			{
				this._GameConfigDict[paramName] = paramValue;
			}
			this.InitGameDBManagerFlags(false);
		}

		// Token: 0x0600093C RID: 2364 RVA: 0x000591BC File Offset: 0x000573BC
		public string GetGameConifgItem(string paramName)
		{
			string paramValue = null;
			lock (this._GameConfigDict)
			{
				if (!this._GameConfigDict.TryGetValue(paramName, out paramValue))
				{
					paramValue = null;
				}
			}
			return paramValue;
		}

		// Token: 0x0600093D RID: 2365 RVA: 0x00059224 File Offset: 0x00057424
		public string GetGameConfigItemStr(string paramName, string defVal)
		{
			string ret = this.GetGameConifgItem(paramName);
			string result;
			if (string.IsNullOrEmpty(ret))
			{
				result = defVal;
			}
			else
			{
				result = ret;
			}
			return result;
		}

		// Token: 0x0600093E RID: 2366 RVA: 0x00059254 File Offset: 0x00057454
		public int GetGameConfigItemInt(string paramName, int defVal)
		{
			string str = this.GetGameConifgItem(paramName);
			int result;
			if (string.IsNullOrEmpty(str))
			{
				result = defVal;
			}
			else
			{
				int ret = 0;
				try
				{
					ret = Convert.ToInt32(str);
				}
				catch (Exception)
				{
					ret = defVal;
				}
				result = ret;
			}
			return result;
		}

		// Token: 0x0600093F RID: 2367 RVA: 0x000592A8 File Offset: 0x000574A8
		public double GetGameConfigItemInt(string paramName, double defVal)
		{
			string str = this.GetGameConifgItem(paramName);
			double result;
			if (string.IsNullOrEmpty(str))
			{
				result = defVal;
			}
			else
			{
				double ret = 0.0;
				try
				{
					ret = Convert.ToDouble(str);
				}
				catch (Exception)
				{
					ret = defVal;
				}
				result = ret;
			}
			return result;
		}

		// Token: 0x06000940 RID: 2368 RVA: 0x00059304 File Offset: 0x00057504
		public TCPOutPacket GetGameConfigDictTCPOutPacket(TCPOutPacketPool pool, int cmdID)
		{
			TCPOutPacket tcpOutPacket = null;
			lock (this._GameConfigDict)
			{
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<string, string>>(this._GameConfigDict, pool, cmdID);
			}
			return tcpOutPacket;
		}

		// Token: 0x04000BBA RID: 3002
		private Dictionary<string, string> _GameConfigDict = new Dictionary<string, string>();

		// Token: 0x04000BBB RID: 3003
		private bool Initialized = false;

		// Token: 0x04000BBC RID: 3004
		private HashSet<string> RolePaiHangKeys = new HashSet<string>();
	}
}
