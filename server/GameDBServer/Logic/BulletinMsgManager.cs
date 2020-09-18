using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	
	public class BulletinMsgManager
	{
		
		public void LoadBulletinMsgFromDB(DBManager dbMgr)
		{
			this._BulletinMsgDict = DBQuery.QueryBulletinMsgDict(dbMgr);
			if (null == this._BulletinMsgDict)
			{
				this._BulletinMsgDict = new Dictionary<string, BulletinMsgData>();
			}
		}

		
		public BulletinMsgData AddBulletinMsg(string msgID, string fromDate, string toDate, int interval, string bulletinText)
		{
			BulletinMsgData bulletinMsgData = new BulletinMsgData
			{
				MsgID = msgID,
				Interval = interval,
				BulletinText = bulletinText,
				BulletinTicks = DataHelper.ConvertToTicks(fromDate)
			};
			long PlayTicks = DataHelper.ConvertToTicks(toDate) - bulletinMsgData.BulletinTicks;
			bulletinMsgData.PlayMinutes = (int)(PlayTicks / 60000L);
			lock (this._BulletinMsgDict)
			{
				this._BulletinMsgDict[msgID] = bulletinMsgData;
			}
			return bulletinMsgData;
		}

		
		public void RemoveBulletinMsg(string msgID)
		{
			lock (this._BulletinMsgDict)
			{
				this._BulletinMsgDict.Remove(msgID);
			}
		}

		
		public TCPOutPacket GetBulletinMsgDictTCPOutPacket(TCPOutPacketPool pool, int cmdID)
		{
			TCPOutPacket tcpOutPacket = null;
			lock (this._BulletinMsgDict)
			{
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<string, BulletinMsgData>>(this._BulletinMsgDict, pool, cmdID);
			}
			return tcpOutPacket;
		}

		
		private Dictionary<string, BulletinMsgData> _BulletinMsgDict = new Dictionary<string, BulletinMsgData>();
	}
}
