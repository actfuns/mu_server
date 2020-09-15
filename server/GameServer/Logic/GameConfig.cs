using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020006C8 RID: 1736
	public class GameConfig
	{
		// Token: 0x060023A7 RID: 9127 RVA: 0x001E6B54 File Offset: 0x001E4D54
		public void LoadGameConfigFromDBServer()
		{
			this._GameConfigDict = Global.LoadDBGameConfigDict();
			if (null == this._GameConfigDict)
			{
				this._GameConfigDict = new Dictionary<string, string>();
			}
		}

		// Token: 0x060023A8 RID: 9128 RVA: 0x001E6B8C File Offset: 0x001E4D8C
		public void SetGameConfigItem(string paramName, string paramValue)
		{
			lock (this._GameConfigDict)
			{
				this._GameConfigDict[paramName] = paramValue;
			}
			this.ChangeParams(paramName, paramValue);
		}

		// Token: 0x060023A9 RID: 9129 RVA: 0x001E6BE8 File Offset: 0x001E4DE8
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

		// Token: 0x060023AA RID: 9130 RVA: 0x001E6C6C File Offset: 0x001E4E6C
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

		// Token: 0x060023AB RID: 9131 RVA: 0x001E6CE4 File Offset: 0x001E4EE4
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

		// Token: 0x060023AC RID: 9132 RVA: 0x001E6D4C File Offset: 0x001E4F4C
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

		// Token: 0x060023AD RID: 9133 RVA: 0x001E6D7C File Offset: 0x001E4F7C
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

		// Token: 0x060023AE RID: 9134 RVA: 0x001E6DD0 File Offset: 0x001E4FD0
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

		// Token: 0x060023AF RID: 9135 RVA: 0x001E6E2C File Offset: 0x001E502C
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

		// Token: 0x060023B0 RID: 9136 RVA: 0x001E6F08 File Offset: 0x001E5108
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

		// Token: 0x040036F0 RID: 14064
		private Dictionary<string, string> _GameConfigDict = new Dictionary<string, string>();
	}
}
