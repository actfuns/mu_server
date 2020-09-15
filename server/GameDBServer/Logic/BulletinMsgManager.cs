using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x020001A8 RID: 424
	public class BulletinMsgManager
	{
		// Token: 0x060008FE RID: 2302 RVA: 0x00053E34 File Offset: 0x00052034
		public void LoadBulletinMsgFromDB(DBManager dbMgr)
		{
			this._BulletinMsgDict = DBQuery.QueryBulletinMsgDict(dbMgr);
			if (null == this._BulletinMsgDict)
			{
				this._BulletinMsgDict = new Dictionary<string, BulletinMsgData>();
			}
		}

		// Token: 0x060008FF RID: 2303 RVA: 0x00053E6C File Offset: 0x0005206C
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

		// Token: 0x06000900 RID: 2304 RVA: 0x00053F14 File Offset: 0x00052114
		public void RemoveBulletinMsg(string msgID)
		{
			lock (this._BulletinMsgDict)
			{
				this._BulletinMsgDict.Remove(msgID);
			}
		}

		// Token: 0x06000901 RID: 2305 RVA: 0x00053F68 File Offset: 0x00052168
		public TCPOutPacket GetBulletinMsgDictTCPOutPacket(TCPOutPacketPool pool, int cmdID)
		{
			TCPOutPacket tcpOutPacket = null;
			lock (this._BulletinMsgDict)
			{
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<string, BulletinMsgData>>(this._BulletinMsgDict, pool, cmdID);
			}
			return tcpOutPacket;
		}

		// Token: 0x040009AC RID: 2476
		private Dictionary<string, BulletinMsgData> _BulletinMsgDict = new Dictionary<string, BulletinMsgData>();
	}
}
