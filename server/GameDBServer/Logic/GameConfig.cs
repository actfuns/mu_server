using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	
	public class GameConfig
	{
		
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

		
		public void UpdateGameConfigItem(string paramName, string paramValue)
		{
			lock (this._GameConfigDict)
			{
				this._GameConfigDict[paramName] = paramValue;
			}
			this.InitGameDBManagerFlags(false);
		}

		
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

		
		public TCPOutPacket GetGameConfigDictTCPOutPacket(TCPOutPacketPool pool, int cmdID)
		{
			TCPOutPacket tcpOutPacket = null;
			lock (this._GameConfigDict)
			{
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<string, string>>(this._GameConfigDict, pool, cmdID);
			}
			return tcpOutPacket;
		}

		
		private Dictionary<string, string> _GameConfigDict = new Dictionary<string, string>();

		
		private bool Initialized = false;

		
		private HashSet<string> RolePaiHangKeys = new HashSet<string>();
	}
}
