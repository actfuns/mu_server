using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class GameConfig
	{
		
		public void LoadGameConfigFromDBServer()
		{
			this._GameConfigDict = Global.LoadDBGameConfigDict();
			if (null == this._GameConfigDict)
			{
				this._GameConfigDict = new Dictionary<string, string>();
			}
		}

		
		public void SetGameConfigItem(string paramName, string paramValue)
		{
			lock (this._GameConfigDict)
			{
				this._GameConfigDict[paramName] = paramValue;
			}
			this.ChangeParams(paramName, paramValue);
		}

		
		public void UpdateGameConfigItem(string paramName, string paramValue, bool force = false)
		{
			lock (this._GameConfigDict)
			{
				string oldValue;
				if (this._GameConfigDict.TryGetValue(paramName, out oldValue))
				{
					if (oldValue == paramValue && !force)
					{
						return;
					}
				}
			}
			this.SetGameConfigItem(paramName, paramValue);
			Global.UpdateDBGameConfigg(paramName, paramValue);
		}

		
		public void ModifyGameConfigItem(string paramName, int paramValue)
		{
			int value = 0;
			lock (this._GameConfigDict)
			{
				value = this.GetGameConfigItemInt(paramName, 0) + paramValue;
				this._GameConfigDict[paramName] = value.ToString();
			}
			this.ChangeParams(paramName, value.ToString());
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

		
		public double GetGameConfigItemDouble(string paramName, double defVal)
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

		
		public void SendAllGameConfigItemsToGM(GameClient client)
		{
			lock (this._GameConfigDict)
			{
				foreach (string key in this._GameConfigDict.Keys)
				{
					string paramValue = this._GameConfigDict[key];
					string textMsg = string.Format("{0}={1}", key, paramValue);
					GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg);
				}
			}
		}

		
		private void ChangeParams(string paramName, string paramValue)
		{
			bool updateHuoID = false;
			if ("big_award_id" == paramName)
			{
				updateHuoID = true;
			}
			else if ("songli_id" == paramName)
			{
				updateHuoID = true;
			}
			if (updateHuoID)
			{
				int bigAwardID = GameManager.GameConfigMgr.GetGameConfigItemInt("big_award_id", 0);
				int songLiID = GameManager.GameConfigMgr.GetGameConfigItemInt("songli_id", 0);
				GameManager.ClientMgr.NotifyAllChangeHuoDongID(bigAwardID, songLiID);
			}
			if ("half_yinliang_period" == paramName)
			{
				int halfYinLiangPeriod = GameManager.GameConfigMgr.GetGameConfigItemInt("half_yinliang_period", 0);
				GameManager.ClientMgr.NotifyAllChangeHalfYinLiangPeriod(halfYinLiangPeriod);
			}
		}

		
		private Dictionary<string, string> _GameConfigDict = new Dictionary<string, string>();
	}
}
