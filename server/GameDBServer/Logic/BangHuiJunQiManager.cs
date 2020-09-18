using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	
	public class BangHuiJunQiManager
	{
		
		public void LoadBangHuiJunQiItemFromDB(DBManager dbMgr)
		{
			DBQuery.QueryBangQiDict(dbMgr, this._BangHuiJunQiItemsDict);
		}

		
		public void AddBangHuiJunQi(int bhid, string qiName, int qiLevel)
		{
			lock (this._BangHuiJunQiItemsDict)
			{
				this._BangHuiJunQiItemsDict[bhid] = new BangHuiJunQiItemData
				{
					BHID = bhid,
					QiName = qiName,
					QiLevel = qiLevel
				};
			}
		}

		
		public void RemoveBangHuiJunQi(int bhid)
		{
			lock (this._BangHuiJunQiItemsDict)
			{
				this._BangHuiJunQiItemsDict.Remove(bhid);
			}
		}

		
		public void UpdateBangHuiQiName(int bhid, string qiName)
		{
			BangHuiJunQiItemData bangHuiJunQiItemData = null;
			lock (this._BangHuiJunQiItemsDict)
			{
				if (this._BangHuiJunQiItemsDict.TryGetValue(bhid, out bangHuiJunQiItemData))
				{
					bangHuiJunQiItemData.QiName = qiName;
				}
			}
		}

		
		public void UpdateBangHuiQiLevel(int bhid, int qiLevel)
		{
			BangHuiJunQiItemData bangHuiJunQiItemData = null;
			lock (this._BangHuiJunQiItemsDict)
			{
				if (this._BangHuiJunQiItemsDict.TryGetValue(bhid, out bangHuiJunQiItemData))
				{
					bangHuiJunQiItemData.QiLevel = qiLevel;
				}
			}
		}

		
		public TCPOutPacket GetBangHuiJunQiItemsDictTCPOutPacket(TCPOutPacketPool pool, int cmdID)
		{
			TCPOutPacket tcpOutPacket = null;
			lock (this._BangHuiJunQiItemsDict)
			{
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<int, BangHuiJunQiItemData>>(this._BangHuiJunQiItemsDict, pool, cmdID);
			}
			return tcpOutPacket;
		}

		
		private Dictionary<int, BangHuiJunQiItemData> _BangHuiJunQiItemsDict = new Dictionary<int, BangHuiJunQiItemData>();
	}
}
