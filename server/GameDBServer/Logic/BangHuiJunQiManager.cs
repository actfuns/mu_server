using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x020001A3 RID: 419
	public class BangHuiJunQiManager
	{
		// Token: 0x060008DB RID: 2267 RVA: 0x00052AE2 File Offset: 0x00050CE2
		public void LoadBangHuiJunQiItemFromDB(DBManager dbMgr)
		{
			DBQuery.QueryBangQiDict(dbMgr, this._BangHuiJunQiItemsDict);
		}

		// Token: 0x060008DC RID: 2268 RVA: 0x00052AF4 File Offset: 0x00050CF4
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

		// Token: 0x060008DD RID: 2269 RVA: 0x00052B64 File Offset: 0x00050D64
		public void RemoveBangHuiJunQi(int bhid)
		{
			lock (this._BangHuiJunQiItemsDict)
			{
				this._BangHuiJunQiItemsDict.Remove(bhid);
			}
		}

		// Token: 0x060008DE RID: 2270 RVA: 0x00052BB8 File Offset: 0x00050DB8
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

		// Token: 0x060008DF RID: 2271 RVA: 0x00052C1C File Offset: 0x00050E1C
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

		// Token: 0x060008E0 RID: 2272 RVA: 0x00052C80 File Offset: 0x00050E80
		public TCPOutPacket GetBangHuiJunQiItemsDictTCPOutPacket(TCPOutPacketPool pool, int cmdID)
		{
			TCPOutPacket tcpOutPacket = null;
			lock (this._BangHuiJunQiItemsDict)
			{
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<int, BangHuiJunQiItemData>>(this._BangHuiJunQiItemsDict, pool, cmdID);
			}
			return tcpOutPacket;
		}

		// Token: 0x040009A0 RID: 2464
		private Dictionary<int, BangHuiJunQiItemData> _BangHuiJunQiItemsDict = new Dictionary<int, BangHuiJunQiItemData>();
	}
}
